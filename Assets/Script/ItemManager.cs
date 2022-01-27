using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.ahyim.planet
{
    public class ItemManager : NetworkBehaviour
    {

        public Transform itemGenerateCenterTransform;
        public Transform itemGenerateLocationTransform;


        // Use this for initialization
        void Start()
        {
            if (isServer)
            {

                StartCoroutine(ItemGenerate(ItemPool.COIN_01, 0.2f, 30.0f));
                StartCoroutine(ItemGenerate(ItemPool.COIN_02, 0.2f, 30.0f));
                StartCoroutine(ItemGenerate(ItemPool.DIAMOND, 0.2f, 30.0f));
                StartCoroutine(ItemGenerate(ItemPool.FIRST_AID, 0.2f, 30.0f));
            }
        }

        

        private IEnumerator ItemGenerate(string itemPrefabName, float timeDelayToGenerate, float itemExistTime)
        {
            while (true)
            {
                yield return new WaitForSeconds(timeDelayToGenerate);

                //Debug.LogFormat("ItemGenerate itemPrefabName:{0}, randamX:{1}, randamY{2}", itemPrefabName, randamX, randamY);
                
                GameObject item = ItemPool.singleton.GetItem(itemPrefabName);
                if (item)
                {
                    float randamX = Random.Range(0f, 360f);
                    float randamY = Random.Range(0f, 360f);
                    itemGenerateCenterTransform.localEulerAngles = new Vector3(randamX, 0, randamY);
                    item.transform.position = itemGenerateLocationTransform.position;
                    item.transform.rotation = itemGenerateLocationTransform.rotation;
                    //Debug.LogFormat("itemGenerateCenterTransform position:{0}, rotation:{1}", itemGenerateLocationTransform.position, itemGenerateLocationTransform.rotation);
                    //item.SetActive(true);
                    //NetworkServer.Spawn(item);
                    item.GetComponent<NetworkTransform>().SetDirtyBit(1U);
                    StartCoroutine(UnSpawn(itemPrefabName, item, itemExistTime));

                }
                    
                

            }


        }

        public IEnumerator UnSpawn(string itemPrefabName, GameObject item, float time)
        {
            yield return new WaitForSeconds(time);
            RecallItem(itemPrefabName, item);

        }

        private void RecallItem(string itemPrefabName, GameObject item)
        {
            if (item.activeSelf)
            {
                //item.SetActive(false);
                //NetworkServer.UnSpawn(item);
                ItemPool.singleton.BackToPool(itemPrefabName, item);
                //Debug.LogFormat("RecallItem itemPrefabName:{0}", itemPrefabName);
            }
        }

    }

}