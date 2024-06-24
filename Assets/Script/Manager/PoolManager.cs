using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject objectPrefab;
    public int poolSize = 10;
    private List<GameObject> pool;

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetObjectFromPool(Vector3 position, Quaternion rotation)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                return obj;
            }
        }

        // Si aucun objet n'est disponible, en instancier un nouveau
        GameObject newObj = Instantiate(objectPrefab, position, rotation);
        pool.Add(newObj);
        return newObj;
    }

    public void DeactivateAllObjects()
    {
        foreach (GameObject obj in pool)
        {
            obj.SetActive(false);
        }
    }
}