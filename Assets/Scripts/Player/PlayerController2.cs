using System.Collections.Generic;
using UnityEngine;

namespace Clase10
{
    public class PlayerController2 : MonoBehaviour
    {
        public Transform pivot;
        public Transform cameraTransform;

        [Header("Movement")] private Rigidbody rb;
        private bool isMoving = false;
        [SerializeField] private float maxAngleMovement = 30f;
        [SerializeField] private float moveSpeed = 5f;

        [Header("Rotation")] [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private float maxAngle = 45f;
        private float rotationY = 0f;
        private float rotationX = 0f;

        [Header("Audio")] [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
        [SerializeField] float maxTimeAudio = 0.7f;
        [SerializeField] float timeAudio = 0;

        private void Awake ()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update ()
        {
            HandleRotation();
            HandleMovement();
        }

        private void HandleRotation ()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            rotationY += mouseX;
            pivot.rotation = Quaternion.Euler(0f, rotationY, 0f);

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -maxAngle, maxAngle);

            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0f);
        }

        private void HandleMovement ()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 camForward = cameraTransform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = cameraTransform.right;
            camRight.y = 0f;
            camRight.Normalize();

            Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;
            Vector3 velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);

            if (CanMove(moveDir))
            {
                PlayAudio();
                rb.velocity = velocity;
            }
        }

        private bool CanMove (Vector3 moveDir)
        {
            Terrain terrain = Terrain.activeTerrain;
            Vector3 relativePos = GetMapPos();
            Vector3 normal = terrain.terrainData.GetInterpolatedNormal(relativePos.x, relativePos.z);
            float angle = Vector3.Angle(normal, Vector3.up);

            float currentHeight = terrain.SampleHeight(rb.position);
            float nextHeight = terrain.SampleHeight(rb.position + moveDir * 5);

            if (angle > maxAngleMovement && nextHeight > currentHeight)
                return false;
            return true;
        }

        private void PlayAudio ()
        {
            timeAudio += Time.deltaTime;
            //if (rb.velocity.magnitude < 0.1f)
            //    return;

            Terrain terrain = Terrain.activeTerrain;
            Vector3 pos = GetMapPos();

            int mapX = Mathf.FloorToInt(pos.x * terrain.terrainData.alphamapWidth);
            int mapZ = Mathf.FloorToInt(pos.z * terrain.terrainData.alphamapHeight);

            float[,,] splatmapData = terrain.terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
            int maxTextures = terrain.terrainData.alphamapLayers;

            float maxValue = 0;
            int index = 0;
            for (int i = 0; i < maxTextures; i++)
            {
                if (splatmapData[0, 0, i] > maxValue)
                {
                    maxValue = splatmapData[0, 0, i];
                    index = i;
                }
            }


            if (timeAudio > maxTimeAudio)
            {
                timeAudio = 0;
                if (index < clips.Count)
                    audioSource.clip = clips[index];
                audioSource.Play();
            }

        }

        private Vector3 GetMapPos ()
        {
            Vector3 pos = rb.position;
            Terrain terrain = Terrain.activeTerrain;

            return new Vector3((pos.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                               0,
                               (pos.z - terrain.transform.position.z) / terrain.terrainData.size.z);
        }
    }
}