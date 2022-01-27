using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.ahyim.planet
{
    public class ItemPool : NetworkBehaviour
    {

        //public const string ONE_COIN = "1coin";
        //public const string COINS = "Coins";
        public const string COIN_01 = "Coin_01";
        public const string COIN_02 = "Coin_02";
        public const string DIAMOND = "Diamond";
        public const string FIRST_AID = "FirstAid";
        public const string ITEM_RESOURCE_PATH = "Prefabs/Items/";

        public const string CLONE = "(Clone)";

        private Dictionary<string, Queue<GameObject>> itemPool = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<string, GameObject> itemContainerDict = new Dictionary<string, GameObject>();
        private Dictionary<string, int> poolInitDict = new Dictionary<string, int>();

        public static ItemPool singleton;

        private void Awake()
        {

            poolInitDict.Add(COIN_01, 100);
            poolInitDict.Add(COIN_02, 100);
            poolInitDict.Add(DIAMOND, 100);
            poolInitDict.Add(FIRST_AID, 100);
        }

        // Use this for initialization
        void Start()


        {

            singleton = this;
            if (isServer)
            {
                foreach (string prefabName in poolInitDict.Keys)
                {
                    itemPool.Add(prefabName, CreateItemGameObjectQuene(prefabName, poolInitDict[prefabName]));
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private Queue<GameObject> CreateItemGameObjectQuene(string prefabName, int poolSize)
        {
            Queue<GameObject> itemQuene = new Queue<GameObject>();
            GameObject itemContainer = new GameObject(prefabName + "_Pool");
            itemContainer.transform.parent = transform;
            itemContainerDict.Add(prefabName, itemContainer);

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
            returnObj.transform.parent = itemContainerDict[prefabName].transform;
            NetworkServer.Spawn(returnObj);
            return returnObj;
        }

        public GameObject GetItem(string prefabName)
        {
            lock (itemPool)
            {
                Queue<GameObject> gObjectQueue = itemPool[prefabName];
                if (gObjectQueue.Count > 0)
                {
                    return gObjectQueue.Dequeue();
                }
                return null;
            }
        }

        public void BackToPool(string prefabName, GameObject gObj)
        {
            lock (itemPool)
            {
                //gObj.SetActive(false);
                gObj.transform.position = Vector3.zero;
                gObj.GetComponent<NetworkTransform>().SetDirtyBit(1U);
                if (!itemPool[prefabName].Contains(gObj))
                {
                    itemPool[prefabName].Enqueue(gObj);
                }
            }

        }
    }

}