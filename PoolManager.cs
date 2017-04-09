using UnityEngine;
using System.Collections.Generic;

public static class PoolManager
{
    const int DefaultPoolSize = 3;

    class Pool
    {
        int nextId = 1;
        int preferredPoolSize;
        Stack<GameObject> inactive;
        GameObject prefabToPool;

        public int Count
        {
            get { return inactive.Count; }
        }

        public Pool(GameObject prefab) : this(prefab, DefaultPoolSize)
        {

        }

        public Pool(GameObject prefab, int poolSize)
        {
            prefabToPool = prefab;
            inactive = new Stack<GameObject>(poolSize);
            preferredPoolSize = poolSize;
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            GameObject gameObject;
            if (inactive.Count == 0)
            {
                gameObject = GameObject.Instantiate(prefabToPool, position, rotation);
                gameObject.name = prefabToPool.name + " (" + (nextId++) + ")";
                gameObject.AddComponent<PoolMember>().pool = this;
            }
            else
            {
                gameObject = inactive.Pop();
                if (gameObject == null)
                {
                    //INFO: the item in our stack is null
                    Debug.Log("Tried to spawn but returned null");
                    return Spawn(position, rotation);
                }
            }

            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
            gameObject.SetActive(true);
            return gameObject;
        }

        public void Despawn(GameObject gameObject)
        {
            if (Count >= preferredPoolSize)
            {
                GameObject.Destroy(gameObject);
                return;
            }
            gameObject.transform.parent = null;
            gameObject.SetActive(false);
            inactive.Push(gameObject);
        }
    }

    class PoolMember : MonoBehaviour
    {
        public Pool pool;
    }

    static Dictionary<GameObject, Pool> pools = new Dictionary<GameObject, Pool>();

    public static void Init(GameObject prefab, int poolSize = DefaultPoolSize)
    {
        if (!pools.ContainsKey(prefab))
        {
            pools[prefab] = new Pool(prefab, poolSize);
        }
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        //INFO: init a pool for this type of prefab if there isn't already one
        Init(prefab);
        return pools[prefab].Spawn(position, rotation);
    }

    public static void Despawn(List<GameObject> gameObjects)
    {
        gameObjects.ForEach(x => Despawn(x));
    }

    public static void Despawn(GameObject gameObject)
    {
        var poolMember = gameObject.GetComponent<PoolMember>();
        if (!poolMember)
        {
            Debug.Log("Object '" + gameObject.name + "' wasn't spawned from a pool. Destroying it instead.");
            GameObject.Destroy(gameObject);
        }
        else
        {
            poolMember.pool.Despawn(gameObject);
        }
    }
}
