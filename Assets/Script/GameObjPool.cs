using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjPool : MonoBehaviour {

    public const string POPUP_DAMAGE = "PopUpDamage";

    public const string ITEM_RESOURCE_PATH = "Prefabs/Object/";


    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, GameObject> objectContainerDict = new Dictionary<string, GameObject>();
    private Dictionary<string, int> poolInitDict = new Dictionary<string, int>();

    public static GameObjPool singleton;

    private void Awake()
    {
        poolInitDict.Add(POPUP_DAMAGE, 5);
    }

    // Use this for initialization
    void Start () {
        singleton = this;

        foreach (string prefabName in poolInitDict.Keys)
        {
            objectPool.Add(prefabName, CreateItemGameObjectQuene(prefabName, poolInitDict[prefabName]));
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private Queue<GameObject> CreateItemGameObjectQuene(string prefabName, int poolSize)
    {
        Queue<GameObject> itemQuene = new Queue<GameObject>();
        GameObject objectContainer = new GameObject(prefabName + "_Pool");
        objectContainer.transform.parent = transform;
        objectContainerDict.Add(prefabName, objectContainer);

        for (int index = 0; index < poolSize; index++)
        {
            GameObject gObj = CraeteItem(prefabName);
            itemQuene.Enqueue(gObj);
        }

        return itemQuene;

    }
    private GameObject CraeteItem(string prefabName)
    {
        GameObject itemObj = Resources.Load<GameObject>(ITEM_RESOURCE_PATH + prefabName);
        GameObject returnObj = Instantiate<GameObject>(itemObj);
        //returnObj.SetActive(false);
        returnObj.transform.parent = objectContainerDict[prefabName].transform;
        return returnObj;
    }

    public GameObject GetObject(string prefabName)
    {
        lock (objectPool)
        {
            Queue<GameObject> gObjectQueue = objectPool[prefabName];
            if (gObjectQueue.Count > 0)
            {
                return gObjectQueue.Dequeue();
            }
            else
            {
                return CraeteItem(prefabName);
            }
            return null;
        }
    }

    public void BackToPool(string prefabName, GameObject gObj)
    {
        lock (objectPool)
        {
            //gObj.SetActive(false);
            gObj.transform.position = Vector3.zero;
            if (!objectPool[prefabName].Contains(gObj))
            {
                objectPool[prefabName].Enqueue(gObj);
            }
        }

    }
}
