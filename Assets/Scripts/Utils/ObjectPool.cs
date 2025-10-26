using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int initialCount = 10;
    private Queue<GameObject> pool = new Queue<GameObject>();
    Sprite CurrentArrowSprite;
    public static ObjectPool Instance;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        for (int i = 0; i < initialCount; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        if (pool.Count == 0)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
            SpriteChange(CurrentArrowSprite);
        }

        GameObject arrow = pool.Dequeue();
        //SpriteChange(CurrentArrowSprite);
        arrow.transform.localScale = new Vector3(1, 1, 1);
        arrow.GetComponent<Arrow>().swiped = false;
        arrow.SetActive(true);
        return arrow;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
    public void SpriteChange(Sprite arrowsprite)
    {
        CurrentArrowSprite = arrowsprite;
        foreach (var obj in pool)
        {
            obj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = arrowsprite;

        }

    }
}
