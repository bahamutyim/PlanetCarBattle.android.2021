using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
namespace com.ahyim.planet
{
    public class ServerCaller : NetworkBehaviour
    {
        private GameObject[] spwanObjs;
        private int spwanIndex = 0;

        // Use this for initialization
        void Start()
        {
            spwanObjs = GameObject.FindGameObjectsWithTag("playerSpawn");
        }

        // Update is called once per frame
        void Update()
        {

        }

        

        [Command]
        public void CmdSpawnPlayerPrefab(GameObject playerPrefab)
        {
            GameObject go = (GameObject)Instantiate(playerPrefab, spwanObjs[spwanIndex].transform.position, Quaternion.identity);
            go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
            
            NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
            //NetworkServer.Spawn(go); 

            //go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToServer);
            spwanIndex++;
            if (spwanIndex >= spwanObjs.Length)
            {
                spwanIndex = 0;
            }

        }
    }
}
