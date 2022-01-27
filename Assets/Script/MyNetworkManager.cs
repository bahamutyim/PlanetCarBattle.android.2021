using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;


namespace com.ahyim.planet
{ 
    public class MyNetworkManager : NetworkManager
    {

        [SerializeField]
        private GameObject disconnectPanel;
        [SerializeField]
        private GameObject networkErrorPanel;
        [SerializeField]
        private Text networkErrorMessageText;
        [SerializeField]
        private Transform playerSpwanPoint;

        //private GameObject[] spwanObjs;
        //private int spwanIndex = 0;

        /// <summary>
        /// Get the index from Spawn Prefabs
        /// </summary>
        /// <param name="prefabName">Prefab Name</param>
        /// <returns>index of Spawn Prefab List</returns>

        //private void Start()
        //{
        //    NetworkServer.SetNetworkConnectionClass<DebugConnection>();
            
            
        //}

        public int getPlayerPrefabPositionByName(string prefabName)
        {
            int returnIndex = 0;
            for (returnIndex = 0; returnIndex < this.spawnPrefabs.Count; returnIndex++)
            {
                if (this.spawnPrefabs[returnIndex].name == prefabName)
                {
                    break;
                }
            }

            return returnIndex;
        }

        public GameObject getPlayerPrefabByName(string prefabName)
        {
            int returnIndex = 0;
            for (returnIndex = 0; returnIndex < this.spawnPrefabs.Count; returnIndex++)
            {
                if (this.spawnPrefabs[returnIndex].name == prefabName)
                {
                    break;
                }
            }

            return this.spawnPrefabs[returnIndex];
        }

        /// <summary>
        /// Override OnClientConnect, pass spawn prefab index in message for change player prefab
        /// </summary>
        /// <param name="conn"></param>
        public override void OnClientConnect(NetworkConnection conn)
        {
            //base.OnClientConnect(conn);
            Debug.Log(string.Format("NetworkConnection address:'{0}' connectionId:'{1}'", conn.address, conn.connectionId));
            if (this.clientLoadedScene)
                return;
            ClientScene.Ready(conn);
            string prefabName = null;
#if UNITY_EDITOR
            if (Player.CurrentPlayer == null)
            {
                prefabName = "NetSportCar";
            }
            else
            {
                prefabName = Player.CurrentPlayer.CurrBattleCar.prefebName;
            }
#endif
#if UNITY_EDITOR == false
            prefabName = Player.CurrentPlayer.CurrBattleCar.prefebName;
#endif


            StringMessage strMsg = new StringMessage( string.Format("{0}|{1}", prefabName , Player.CurrentPlayer.uid ));
            //IntegerMessage intMsg = new IntegerMessage(getPlayerPrefabPositionByName(prefabName));
            ClientScene.AddPlayer(conn, 0, strMsg);
        } 
               
         
        
        /// <summary>
        /// Overrid OnServerAddPlayer, get the selected prefab index from message and instantiate it
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="playerControllerId"></param>
        /// <param name="extraMessageReader"></param>
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
        {
            StringMessage prefabAndUid = extraMessageReader.ReadMessage<StringMessage>();
            char[] splitChars = new char[] { '|' };
            string[] messageArray = prefabAndUid.value.Split(splitChars);
            string prefabName = messageArray[0];
            string uid = messageArray[1];

            Debug.Log(string.Format("prefabName={0},uid={1} ", prefabName, uid));

            GameObject player;
            lock (playerSpwanPoint)
            {
                RandomPlayerSpwanCenter();
                player = GameObject.Instantiate<GameObject>(spawnPrefabs[getPlayerPrefabPositionByName(prefabName)], playerSpwanPoint.position, playerSpwanPoint.rotation);
            }
            //player.name = string.Format("{0}_{1}", player.name, conn.connectionId);
            NetPlayerManager netPlayerManager = player.GetComponent<NetPlayerManager>();

            netPlayerManager.LoadPlayerFromServer(uid,
                (bool isSuccess, string errMsg) =>
                {
                    Debug.LogErrorFormat("isSuccess:{0}, errMsg:{1}", isSuccess, errMsg);
                    NetworkServer.AddPlayerForConnection(conn, player, 0);

                    //spwanIndex++;
                    //if (spwanIndex >= spwanObjs.Length)
                    //{
                    //    spwanIndex = 0;
                    //}

                    base.OnServerAddPlayer(conn, playerControllerId);
                    NewPlayerConnect();

                    
                    ScoreListManager.AddPlayer(conn.connectionId, netPlayerManager.playerName);
                    NetworkServer.SendToAll(MessageTypes.MSG_SCORE_LIST, ScoreListManager.ScoreListMessage);
                }
            );

            
        }

        //public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
        //{
        //    base.OnServerRemovePlayer(conn, player);
        //    ScoreListManager.RemovePlayer(conn.connectionId);
        //    NetworkServer.SendToAll(MessageTypes.MSG_SCORE_LIST, ScoreListManager.ScoreListMessage);
        //}

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            ScoreListManager.RemovePlayer(conn.connectionId);
            NetworkServer.SendToAll(MessageTypes.MSG_SCORE_LIST, ScoreListManager.ScoreListMessage);
        }

        public override void OnStartServer()
        {
            //spwanObjs = GameObject.FindGameObjectsWithTag("playerSpawn");
            base.OnStartServer();
        }

        //Detect when a client connects to the Server
        public override void OnClientDisconnect(NetworkConnection connection)
        {
            disconnectPanel.SetActive(true);
            base.OnClientDisconnect(connection);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            networkErrorPanel.SetActive(true);
            networkErrorMessageText.text = string.Format("Network Error\nError Code:{0}", errorCode);
            base.OnClientError(conn, errorCode);
        }


        private void NewPlayerConnect()
        {
            EventManager.TriggerEvent(Constants.EVENT_NEW_PLAYER);
        }

        public void RandomPlayerSpwanCenter()
        {
            int overlapColliderCount = 0;
            do
            {
                float randamX = UnityEngine.Random.Range(0f, 360f);
                float randamY = UnityEngine.Random.Range(0f, 360f);
                playerSpwanPoint.parent.localEulerAngles = new Vector3(randamX, 0, randamY);
                Collider[] hitColliders = Physics.OverlapBox(playerSpwanPoint.position, new Vector3(2f, 0.5f, 2f), playerSpwanPoint.rotation);
                overlapColliderCount = hitColliders.Length;
                
            }
            while (overlapColliderCount > 1);





        }
    }
}
