using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.ahyim.planet
{
    public class Role : MonoBehaviour
    {

        public bool isServer;

        // Use this for initialization
        void Start()
        {
            NetworkManager net = GetComponent<NetworkManager>();


            if (net != null)
            {
                if (isServer)
                {
                    Debug.Log("prepare server......");
                    net.StartServer();
                    Debug.Log("Server started");
                }
                else
                {
                    net.networkAddress = "127.0.0.1";
                    net.StartClient();
                }
            }


        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
