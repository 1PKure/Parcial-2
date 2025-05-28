using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [SerializeField] public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializePools();
    }

    public void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            if (poolDictionary.ContainsKey(pool.tag))
            {
                Debug.LogWarning($"El pool con el tag '{pool.tag}' ya existe. Se omitirá.");
                continue;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = CreateNewObject(pool.prefab, pool.tag);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"No existe una pool con el tag '{tag}'.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];

        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                pool.Enqueue(pool.Dequeue());
                return obj;
            }

            pool.Enqueue(pool.Dequeue());
        }

        Debug.Log($"[PoolManager] Pool '{tag}' alcanzó su máximo. Se va a extender.");
        Pool config = pools.Find(p => p.tag == tag);

        if (config != null)
        {
            GameObject newObj = CreateNewObject(config.prefab, tag);
            pool.Enqueue(newObj);
            return newObj;
        }

        Debug.LogError($"No se encontró configuración de pool para el tag '{tag}'.");
        return null;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;

        PooledObject pooled = obj.GetComponent<PooledObject>();
        if (pooled == null || string.IsNullOrEmpty(pooled.poolTag))
        {
            Debug.LogWarning("Objeto sin información de pool. Se destruirá.");
            Destroy(obj);
            return;
        }

        if (!poolDictionary.ContainsKey(pooled.poolTag))
        {
            Debug.LogWarning($"Pool no encontrada para tag '{pooled.poolTag}'. Se destruirá.");
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        poolDictionary[pooled.poolTag].Enqueue(obj);
    }

    private GameObject CreateNewObject(GameObject prefab, string tag)
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        obj.transform.SetParent(transform);

        PooledObject pooled = obj.GetComponent<PooledObject>();
        if (pooled == null)
            pooled = obj.AddComponent<PooledObject>();

        pooled.poolTag = tag;
        return obj;
    }

    public void SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        GameObject obj = GetPooledObject(tag);
        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
    }
}
