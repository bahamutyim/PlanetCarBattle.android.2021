using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
namespace com.ahyim.planet
{
    public class NetworkGameController : MonoBehaviour
    {
        //public NetworkManager networkManager;
        //public ServerCaller serverCaller;
        public static NetworkGameController singleton;
        public Text netControlButtonText;
        public Text scoreText;
        public Text scoreBoardPlayerNameText;
        public Text scoreBoardScoreText;
        [SerializeField] private Text goldText;
        [SerializeField] private Text diamondText;
        [SerializeField] private Transform aiSpwanPoint; 


        private Dictionary<string, GameObject> playerPrefabDict = new Dictionary<string, GameObject>();
        private GameObject playerPrefab;
        private bool isServer;
        private NetworkClient client;

        public GameObject LeavePanel;
        

        public int score = 0;
        public int gold = 0;
        public int diamond = 0;
        public ScoreName[] scoreNameArray;

        //for debug
        public string status;

#if UNITY_ANDROID || UNITY_IOS
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        // called when the game is terminated
        void OnDisable()
        {
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ClientConnect();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
#endif
        public void Start()
        {

            
            singleton = this;
            UpdateGoldText();
            UpdateDiamondText();

#if UNITY_STANDALONE
            NetworkManager.singleton.StartServer();
            
#endif  
        }

        private void Update()
        {
#if UNITY_ANDROID
            if (Input.GetKeyUp("escape"))
            {
                LeavePanel.SetActive(true);
            }
#endif
        }

        public void HideLeavePanel()
        {
            LeavePanel.SetActive(false);
        }

        public void ClientConnect()
        {
#if UNITY_EDITOR
            ///For Testing only
            if (AppSetting.Values == null)
            {
                AppSetting.TestSetting();

                //NetworkManager.singleton.networkAddress = "192.168.137.150";
                //NetworkManager.singleton.networkAddress = "ec2-35-162-60-75.us-west-2.compute.amazonaws.com";
            }
            if (Player.CurrentPlayer == null)
            {
                Player.DummyPlayer();
            }
#endif
            
            NetworkManager.singleton.networkAddress = AppSetting.Values.planetUrlList[Player.CurrentPlayer.CurrPlanetIndex];
            
            Debug.LogFormat("Try to connect {0}:{1}", NetworkManager.singleton.networkAddress, NetworkManager.singleton.networkPort);
            if (NetworkManager.singleton.client == null)
            {
                Debug.Log("client is null");
                client = NetworkManager.singleton.StartClient();
            }
            else 
            {
                Debug.Log("client exist, try to reconnect");
                client = NetworkManager.singleton.client;
                client = NetworkManager.singleton.StartClient();
                
                //client.Connect(NetworkManager.singleton.networkAddress, NetworkManager.singleton.networkPort);
            }
            //if (netControlButtonText)
            //{
            //    netControlButtonText.text = "Disconnect";
            //}
            
            client.RegisterHandler(MessageTypes.MSG_SCORE_LIST, OnScoreListChange);
            client.RegisterHandler(MessageTypes.MSG_SCORE, OnScoreChange);
            client.RegisterHandler(MessageTypes.MSG_GOLD, OnGoldChange);
            client.RegisterHandler(MessageTypes.MSG_DIAMOND, OnDiamondChange);
            
            
        }

        private void OnDiamondChange(NetworkMessage netMsg)
        {
            IntegerMessage diamondMsg = netMsg.ReadMessage<IntegerMessage>();
            diamond += diamondMsg.value;
            //scoreText.text = string.Format("SCORE : {0}", score);
            //Debug.LogFormat("OnDiamondChange diamond:{0}", diamond);
            UpdateDiamondText();
        }

        private void OnGoldChange(NetworkMessage netMsg)
        {
            IntegerMessage goldMsg = netMsg.ReadMessage<IntegerMessage>();
            gold += goldMsg.value;
            //scoreText.text = string.Format("SCORE : {0}", score);
            //Debug.LogFormat("OnDiamondChange gold:{0}", gold);
            UpdateGoldText();
        }

        private void OnScoreChange(NetworkMessage netMsg)
        {
            IntegerMessage scoreMsg = netMsg.ReadMessage<IntegerMessage>();
            score += scoreMsg.value;
            scoreText.text = string.Format("SCORE : {0}", score);
            //Debug.LogFormat("OnScoreChange score:{0}", score);
        }

        private void OnScoreListChange(NetworkMessage netMsg)
        {
            ScoreListMessage scoreListMessage = netMsg.ReadMessage<ScoreListMessage>();
            scoreNameArray = scoreListMessage.scoreNameArray;
            Array.Sort<ScoreName>(scoreNameArray);

            //Debug.LogFormat("OnScoreListChange scoreNameArray(Lenght{0}):{1}", scoreNameArray.Length, scoreNameArray);
            StringBuilder playerName = new StringBuilder();
            StringBuilder score = new StringBuilder();
            int index = 0;
            foreach( ScoreName scoreName in scoreNameArray )
            {
                index++;
                playerName.Append(scoreName.playerName);
                playerName.Append("\n");
                score.Append(scoreName.score);
                score.Append("\n");
                if (index >=8)
                {
                    break;
                }
            }
            scoreBoardPlayerNameText.text = playerName.ToString();
            scoreBoardScoreText.text = score.ToString();


        }

        

        
        public void StopClient()
        {
            client.ResetConnectionStats();
            client.Disconnect();
            client.Shutdown();
            //if (netControlButtonText)
            //{
            //    netControlButtonText.text = "Connect";
            //}
            client.UnregisterHandler(MessageTypes.MSG_SCORE_LIST);
            client.UnregisterHandler(MessageTypes.MSG_SCORE);
            client.UnregisterHandler(MessageTypes.MSG_GOLD);
            client.UnregisterHandler(MessageTypes.MSG_DIAMOND);

        }

        public void NetControl()
        {
            if (client != null)
            {
                if ( client.isConnected)
                {
                    CarUserControl.UnSwpanPlayer();
                    
                    //StopClient();
                }
                else
                {
                    
                    ClientConnect();
                }
            }



        }

        public void EndGame()
        {
            
            try
            {
                NetworkGameController.singleton.StopClient();
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                MySceneManager.LoadSceneStatic("MainMenu2");
            }
            
        }
        
        public void ExitGame()
        {
            try
            {
                CarUserControl.UnSwpanPlayer();
                NetworkGameController.singleton.StopClient();
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                MySceneManager.LoadSceneStatic("MainMenu2");
            }
            
            
        }
        
        public void UpdateGoldText()
        {
            goldText.text = string.Format("{0:,0}", gold); 
        }

        public void UpdateDiamondText()
        {
            diamondText.text = string.Format("{0:,0}", diamond);
        }

        public void SavePlayer()
        {
            Player.CurrentPlayer.gold += gold;
            Player.CurrentPlayer.diamond += diamond;
            Player.CurrentPlayer.SavePlayer(
                task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {

                    }
                    else if (task.IsCompleted)
                    {

                    }
                }
            );
        }

        public void CreateAIPlayer(string carUName, string weaponAName, string weaponBName, string aiPlayerName, int aiConnectId)
        {
            Debug.LogFormat("CreateAIPlayer {0}, carUName:{1} ", aiPlayerName, carUName);
            BattleCar battleCar = AppSetting.Values.getBattleCarByUName(carUName);

            RandomAISpwanCenter();



            GameObject aiPlayer = GameObject.Instantiate<GameObject>( 
                MyNetworkManager.singleton.spawnPrefabs[getPlayerPrefabPositionByName(MyNetworkManager.singleton.spawnPrefabs, battleCar.prefebName)], 
                aiSpwanPoint.position,
                aiSpwanPoint.rotation
                );
            aiPlayer.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
            
            Debug.Log(battleCar.ToString());
            NetPlayerManager netPlayerManager = aiPlayer.GetComponent<NetPlayerManager>();
            netPlayerManager.OnChnagePlayerName( aiPlayerName);
            netPlayerManager.score = 0;
            netPlayerManager.gold = 0;
            netPlayerManager.diamond = 0;
            netPlayerManager.selectedCarUName = carUName;
            netPlayerManager.selectedWeaponAUName = weaponAName;
            netPlayerManager.selectedWeaponBUName = weaponBName;
            netPlayerManager.uid = "";
            netPlayerManager.bcar = battleCar;
            netPlayerManager.AIConnectId = aiConnectId;

            ScoreListManager.AddPlayer(netPlayerManager.AIConnectId, netPlayerManager.playerName);

            CarUserControl carUserControl = aiPlayer.GetComponent<CarUserControl>();
            carUserControl.OnChangePower(battleCar.power);
            carUserControl.OnChangeWeight(battleCar.weight);
            Health health = aiPlayer.GetComponent<Health>();
            health.amno = battleCar.amno;
            carUserControl.TurnOnAI();
            NetworkServer.Spawn(aiPlayer);
        }

        public void ReSpawnAIPlayer(GameObject aiPlayer)
        {
            Debug.LogFormat("ReSpawnAIPlayer aiPlayer:{0}", aiPlayer.name);
            lock(aiSpwanPoint)
            {
                RandomAISpwanCenter();
                aiPlayer.transform.position = aiSpwanPoint.position;
                aiPlayer.transform.rotation = aiSpwanPoint.rotation;
            }
            
            NetPlayerManager netPlayerManager = aiPlayer.GetComponent<NetPlayerManager>();
            netPlayerManager.score = 0;
            netPlayerManager.gold = 0;
            netPlayerManager.diamond = 0;

            ScoreListManager.AddPlayer(netPlayerManager.AIConnectId, netPlayerManager.playerName);
            Health health = aiPlayer.GetComponent<Health>();
            health.amno = netPlayerManager.bcar.amno;
            health.currentHealth = Health.maxHealth;
            NetworkServer.Spawn(aiPlayer);
        }

        public void RandomAISpwanCenter()
        {
            int overlapColliderCount = 0;
            //float offset = 0;
            do
            {
                float randamX = UnityEngine.Random.Range(0f, 360f);
                float randamY = UnityEngine.Random.Range(0f, 360f);
                //randamX = 0f;
                //randamY = offset;
                aiSpwanPoint.parent.localEulerAngles = new Vector3(randamX, 0, randamY);
                Collider[] hitColliders = Physics.OverlapBox(aiSpwanPoint.position, new Vector3(2f, 0.5f, 2f), aiSpwanPoint.rotation);
                overlapColliderCount = hitColliders.Length;
                //offset += 0.1f;
            }
            while (overlapColliderCount > 1);
            




        }

        private int getPlayerPrefabPositionByName( List<GameObject> spawnPrefabs, string prefabName)
        {
            int returnIndex = 0;
            for (returnIndex = 0; returnIndex < spawnPrefabs.Count; returnIndex++)
            {
                if (spawnPrefabs[returnIndex].name == prefabName)
                {
                    break;
                }
            }

            return returnIndex;
        }

    }
}
