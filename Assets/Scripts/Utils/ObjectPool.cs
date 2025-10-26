using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Pooling Settings")]
    public GameObject prefab;
    public int initialCount = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private Sprite currentArrowSprite;

    public static ObjectPool Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Pre-instantiate pool objects
        for (int i = 0; i < initialCount; i++)
        {
            GameObject obj = CreateNewObject();
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);

        // Apply current sprite if exists
        if (currentArrowSprite != null)
        {
            obj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = currentArrowSprite;
        }

        return obj;
    }

    public GameObject GetObject()
    {
        GameObject arrow = null;

        // Try to find an inactive object first
        foreach (var pooledObj in pool)
        {
            if (!pooledObj.activeInHierarchy)
            {
                arrow = pooledObj;
                break;
            }
        }

        // If all are active, then expand pool
        if (arrow == null)
        {
            arrow = CreateNewObject();
            pool.Enqueue(arrow);
        }

        // Reactivate and reset arrow
        arrow.transform.localScale = Vector3.one;
        arrow.GetComponent<Arrow>().swiped = false;
        arrow.SetActive(true);

        return arrow;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void SpriteChange(Sprite arrowSprite)
    {
        currentArrowSprite = arrowSprite;

        foreach (var obj in pool)
        {
            var renderer = obj.transform.GetChild(0).GetComponent<SpriteRenderer>();
            renderer.sprite = arrowSprite;
        }
    }
}
