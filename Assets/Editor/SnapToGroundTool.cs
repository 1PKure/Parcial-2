using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class SnapToGroundTool : EditorWindow
{
    [Header("Raycast")]
    [SerializeField] private float castExtraHeight = 2f;     
    [SerializeField] private float castDistance = 500f;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private bool ignoreTriggers = true;

    [Header("Placement")]
    [SerializeField] private float additionalYOffset = 0f;   
    [SerializeField] private bool includeChildren = true;

    [Header("Bounds source")]
    [SerializeField] private bool preferColliderBounds = true; 

    [MenuItem("Tools/Snap To Ground")]
    public static void Open() => GetWindow<SnapToGroundTool>("Snap To Ground");

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Snap objects so their BOTTOM touches ground (not the pivot).", EditorStyles.boldLabel);
        EditorGUILayout.Space(6);

        castExtraHeight = EditorGUILayout.FloatField("Cast Extra Height", castExtraHeight);
        castDistance = EditorGUILayout.FloatField("Cast Distance", castDistance);
        groundMask = LayerMaskField("Ground Mask", groundMask);
        ignoreTriggers = EditorGUILayout.Toggle("Ignore Triggers", ignoreTriggers);

        EditorGUILayout.Space(8);

        additionalYOffset = EditorGUILayout.FloatField("Additional Y Offset", additionalYOffset);
        includeChildren = EditorGUILayout.Toggle("Include Children", includeChildren);
        preferColliderBounds = EditorGUILayout.Toggle("Prefer Collider Bounds", preferColliderBounds);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Snap Selected"))
            SnapSelected();
    }

    private void SnapSelected()
    {
        var selection = Selection.gameObjects;
        if (selection == null || selection.Length == 0) return;

        int moved = 0;

        foreach (var root in selection)
        {
            if (!root) continue;

            var transforms = includeChildren ? root.GetComponentsInChildren<Transform>(true) : new[] { root.transform };

            foreach (var t in transforms)
            {

                if (!TryGetBounds(t, out Bounds b))
                    continue;

                Vector3 origin = new Vector3(b.center.x, b.max.y + castExtraHeight, b.center.z);
                Ray ray = new Ray(origin, Vector3.down);

                var query = ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide;

                var hits = Physics.RaycastAll(ray, castDistance, groundMask, query);
                if (hits == null || hits.Length == 0) continue;

                System.Array.Sort(hits, (a, c) => a.distance.CompareTo(c.distance));

                HashSet<Collider> ownCols = GetOwnColliders(t);

                bool found = false;
                RaycastHit best = default;

                foreach (var h in hits)
                {
                    if (h.collider == null) continue;
                    if (ownCols.Contains(h.collider)) continue; 
                    best = h;
                    found = true;
                    break;
                }

                if (!found) continue;

                Undo.RecordObject(t, "Snap To Ground");

                float currentBottomY = b.min.y;

                float deltaY = (best.point.y + additionalYOffset) - currentBottomY;

                t.position += new Vector3(0f, deltaY, 0f);

                EditorUtility.SetDirty(t);
                moved++;
            }
        }

        Debug.Log($"Snap To Ground (bounds-based): movidos {moved} transforms.");
    }

    private bool TryGetBounds(Transform t, out Bounds b)
    {
        if (preferColliderBounds)
        {
            var col = t.GetComponentInChildren<Collider>();
            if (col != null)
            {
                b = col.bounds;
                return true;
            }
        }

        var rend = t.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            b = rend.bounds;
            return true;
        }

        b = default;
        return false;
    }

    private HashSet<Collider> GetOwnColliders(Transform t)
    {
        var cols = t.GetComponentsInChildren<Collider>(true);
        var set = new HashSet<Collider>();
        foreach (var c in cols) set.Add(c);
        return set;
    }

    private static LayerMask LayerMaskField(string label, LayerMask selected)
    {
        var layers = InternalEditorUtility.layers;
        int mask = selected.value;

        int[] layerNumbers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
            layerNumbers[i] = LayerMask.NameToLayer(layers[i]);

        int maskWithoutEmpty = 0;
        for (int i = 0; i < layerNumbers.Length; i++)
        {
            int layer = layerNumbers[i];
            if (((1 << layer) & mask) != 0)
                maskWithoutEmpty |= (1 << i);
        }

        maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

        int newMask = 0;
        for (int i = 0; i < layerNumbers.Length; i++)
        {
            if ((maskWithoutEmpty & (1 << i)) != 0)
                newMask |= (1 << layerNumbers[i]);
        }

        selected.value = newMask;
        return selected;
    }
}
