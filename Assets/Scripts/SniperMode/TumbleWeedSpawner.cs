using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbleWeedSpawner : Singleton<TumbleWeedSpawner>
{
    [SerializeField] private PrefabData prefabData = null;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private const int MAX = 5;
    private WaitForSecondsRealtime spawnTime = new WaitForSecondsRealtime(1f);

    [SerializeField] private Transform[] Pos = new Transform[5];
    private void Awake()
    {
        GameObject temp = null;
        for(int i = 0; i < MAX; i++)
        {
            temp = Instantiate(prefabData.TumbleWeed1, this.transform);
            temp.SetActive(false);
            temp.transform.SetParent(this.transform);
            pool.Enqueue(temp);

            temp = Instantiate(prefabData.TumbleWeed2, this.transform);
            temp.SetActive(false);
            temp.transform.SetParent(this.transform);
            pool.Enqueue(temp);
        }
    }
    private void Start()
    {
        StartCoroutine(SpawnTumbleWeed());
    }

    private IEnumerator SpawnTumbleWeed()
    {
        GameObject output = null;

        while(true)
        {
            if(pool.Count == 0)
            {
                output = Instantiate(prefabData.TumbleWeed2, this.transform);
            }
            else
            {
                output = pool.Dequeue();
            }
            output.transform.SetParent(null);
            output.SetActive(true);


            output.transform.position = RandomPos().position;

            yield return spawnTime;
        }
    }

    private Transform RandomPos()
    {
        return Pos[(int)Random.Range(0, 5)];
    }
    public void ReturnTumble(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        pool.Enqueue(obj);
    }
}
