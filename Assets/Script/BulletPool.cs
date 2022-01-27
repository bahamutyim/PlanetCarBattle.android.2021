using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.ahyim.planet
{
    public class BulletPool : NetworkBehaviour
    {
        public static BulletPool singleton;
        public Dictionary<string, Queue<GameObject>> bulletPool = new Dictionary<string, Queue<GameObject>>();
        public MyNetworkManager myNetworkManager;
        private Dictionary<string, GameObject> bulletPrefabDict = new Dictionary<string, GameObject>();
        private Dictionary<string, int> bulletCountPRefabDict = new Dictionary<string, int>();
        private Dictionary<string, GameObject> bulletContainerDict = new Dictionary<string, GameObject>();

        public const string NORMAL_BULLET = "NormalBullet";
        public const string PLASMA_BULLET = "PlasmaBullet";


        private void Awake()
        {
            singleton = this;
        }
        // Use this for initialization
        void Start()
        {
            if( isServer)
            {
                
                bulletPool.Add(NORMAL_BULLET, CreateBulletGameObjectList(NORMAL_BULLET, 1));
                bulletPool.Add(PLASMA_BULLET, CreateBulletGameObjectList(PLASMA_BULLET, 1));
            }
            
            

        }

        // Update is called once per frame
        void Update()
        {

        }

        private Queue<GameObject> CreateBulletGameObjectList(string prefabName, int poolSize)
        {
            Queue<GameObject> gameObjectList = new Queue<GameObject>();
            GameObject prefab = myNetworkManager.getPlayerPrefabByName(prefabName);
            prefab.layer = LayerMask.NameToLayer("Bullet");
            bulletPrefabDict.Add(prefabName, prefab);
            bulletCountPRefabDict.Add(prefabName, 1);
            GameObject bulletContainer = new GameObject(prefabName + "_Pool");
            bulletContainer.transform.parent = transform;
            bulletContainerDict.Add(prefabName, bulletContainer);

            for ( int index=0; index < poolSize; index++ )
            {
                GameObject gObj = CreateBullet(prefabName, prefab);
                gameObjectList.Enqueue(gObj);
            }

            return gameObjectList;
        }

        private GameObject CreateBullet(string prefabName, GameObject prefab)
        {
            GameObject gObj = GameObject.Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity);
            gObj.GetComponent<Bullet>().prefabName = prefabName;
            gObj.name = string.Format("{0}_{1}", prefabName, bulletCountPRefabDict[prefabName]++);
            gObj.transform.parent = bulletContainerDict[prefabName].transform;
            gObj.layer = LayerMask.NameToLayer("Bullet");
            //gObj.SetActive(false);
            NetworkServer.Spawn(gObj);
            return gObj;
        }

        public GameObject GetBullet(string prefabName)
        {
            
            Queue<GameObject> gObjectQueue = bulletPool[prefabName];
            if (gObjectQueue.Count > 0)
            {
                return gObjectQueue.Dequeue();
            }
            else
            {
                GameObject gObj = CreateBullet(prefabName, bulletPrefabDict[prefabName]);
                //gObj.SetActive(false);
                return gObj;
            }
            
        }

        public void BackToPool(string prefabName, GameObject gObj)
        {
            
            //gObj.SetActive(false);
            gObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gObj.transform.position = Vector3.zero;
            gObj.GetComponent<NetworkTransform>().SetDirtyBit(1U);
            if (!bulletPool[prefabName].Contains(gObj))
            {
                bulletPool[prefabName].Enqueue(gObj);
            }
            
                
        }



    } 
}
