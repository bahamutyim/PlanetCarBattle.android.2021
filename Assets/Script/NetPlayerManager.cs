using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace com.ahyim.planet
{
    public class NetPlayerManager : NetworkBehaviour
    {

        public static GameObject[] allPlayer;
        public GameObject targetPlayer;
        public HashSet<GameObject> nearByPlayerSet = new HashSet<GameObject>();
        [SyncVar]
        public string uid;
        [SyncVar(hook = "OnChnagePlayerName")]
        public string playerName;
        [SyncVar]
        public int score;
        [SyncVar]
        public int gold;
        [SyncVar]
        public int diamond;
        [SyncVar]
        public string selectedCarUName;
        [SyncVar (hook = "OnChangeWeaponA") ]
        public string selectedWeaponAUName;
        [SyncVar (hook = "OnChangeWeaponB")]
        public string selectedWeaponBUName;

        public Text playerNameText;

        private Health health;
        private CarUserControl carUserControl;

        public BattleCar bcar;

        public float searchRange = 50f;
        private float sqrSearchRange;
        private int aiConnectionId;

        public int AIConnectId
        {
            get { return aiConnectionId; }
            set { aiConnectionId = value; }
        }
          


        // Use this for initialization
        void Start()
        {
            sqrSearchRange = searchRange * searchRange;


            health = GetComponent<Health>();
            carUserControl = GetComponent<CarUserControl>();

            UpdateAllPlayerArray();
            ChangeWeaponA();
            foreach (GameObject player in allPlayer)
            {
                player.transform.GetComponentInChildren<Billboard>().currCamera = SwitchCarCamera.currentCamera;

            }
            //if (isClient)
            //{
            //    Debug.LogFormat("playerName:{0},selectedweaponAUName:{1},selectedWeaponBUName:{2},selectedCarUName:{3}", playerName, selectedWeaponAUName, selectedWeaponBUName, selectedCarUName);
            //}
            if (isLocalPlayer || carUserControl.IsAIPlayer)
            {
                StartCoroutine(FindNearByPlayerDelay(0.1f));
            }

            ConnectionConfig myConfig = new ConnectionConfig();
            //myConfig.AddChannel(QosType.Unreliable);
            //myConfig.AddChannel(QosType.UnreliableFragmented);
            myConfig.NetworkDropThreshold = 50;         //50%
            myConfig.OverflowDropThreshold = 10;         //10%

        }
        //private void Update()
        //{
        //    if (isLocalPlayer || carUserControl.IsAIPlayer)
        //    {
        //        FindNearByPlayer();
        //        //Debug.LogFormat("{0}: targetPlayer:{1}", gameObject.name, targetPlayer.name);
        //    }
        //}
        public void LoadPlayerFromServer(string uid, System.Action<bool, string> completedLoadCallback)
        {
            Debug.Log("call LoadPlayerFromServer");
            if (!isClient)
            {
                StartCoroutine(
                    Player.LoadPlayerByUIDFromServer(uid,
                        (bool isSuccess, string errMsg, Player player) =>
                        {
                            Debug.LogFormat("isSuccess:{0},errMsg:{1},player:{2}", isSuccess, errMsg, player);

                            if (isSuccess)
                            {
                                this.playerName = player.name;
                                this.score = 0;
                                this.gold = 0;
                                this.diamond = 0;
                                this.selectedCarUName = player.selectedCarUName;
                                this.selectedWeaponAUName = player.selectedweaponAUName;
                                this.selectedWeaponBUName = player.selectedWeaponBUName;
                                this.uid = player.uid;
                                this.bcar = AppSetting.Values.getBattleCarByUName(selectedCarUName);
                                ChangeWeaponA();
                                health.amno = player.CurrBattleCar.amno;
                                carUserControl.weight = player.CurrBattleCar.weight;
                                carUserControl.power = player.CurrBattleCar.power;

                                

                            }
                            completedLoadCallback(isSuccess, errMsg);
                        }

                        )
                    );
            }
            
        }
        private IEnumerator FindNearByPlayerDelay(float delay)
        {
            while(true)
            {
                FindNearByPlayer();
                yield return new WaitForSeconds(delay);
            }
        }

        private void FindNearByPlayer()
        {
            
            //GameObject[] allPlayer = GameObject.FindGameObjectsWithTag("Player");
            GameObject tempNearestPlayer = null;
            if (allPlayer != null)
            {

                float tempNearestDistance = Mathf.Infinity;

                Vector3 thisPosition = transform.position;
                foreach (GameObject player in allPlayer)
                {

                    if (player != null)
                    {
                        Vector3 diff = player.transform.position - thisPosition;
                        float curDistance = diff.sqrMagnitude;
                        //Debug.LogFormat("{0}: {1}  distance:{2}, sqrSearchRange:{3}, tempNearestDistance:{4}", gameObject.name, player.name, curDistance, sqrSearchRange, tempNearestDistance);
                        //by pass self gameplayer
                        if (curDistance == 0)
                        {
                            continue;
                        }

                        if (curDistance < sqrSearchRange)
                        {
                            //Debug.LogFormat("{0}: {1} add nearByPlayerSet", gameObject.name, player.name);
                            nearByPlayerSet.Add(player);
                            

                            if (tempNearestDistance > curDistance)
                            {
                                //Debug.LogFormat("{0}: {1} is nearest",gameObject.name, player.name);
                                tempNearestPlayer = player;
                                tempNearestDistance = curDistance;
                            }

                        }
                        else
                        {
                            nearByPlayerSet.Remove(player);
                            
                        }

                    }




                }


            }
            targetPlayer = tempNearestPlayer;


        }


        public void OnChangeWeaponA(string weaponAName)
        {
            this.selectedWeaponAUName = weaponAName;
            ChangeWeaponA();

        }
        public void OnChangeWeaponB(string weaponBName)
        {
            this.selectedWeaponBUName = weaponBName;
        }
        public void OnChnagePlayerName(string playerName)
        {
            this.playerName = playerName;
            if (playerNameText)
            {
                playerNameText.text = playerName;
            }
        }



        public void UpdateAllPlayerArray()
        {
            
           
            allPlayer = GameObject.FindGameObjectsWithTag("Player");
            
            
            
            
        }

        public void ChangeWeaponA()
        {
            Debug.Log("Change weapon");
            Transform weaponATranform = transform.Find("WeaponAContainer/WeaponA");
            for(int index =0; index < weaponATranform.childCount; index++)
            {
                Transform weaponA = weaponATranform.GetChild(index);
                if (weaponA.gameObject.name == "Cylinder" || weaponA.gameObject.name == "WeaponACollider")
                {
                    continue;
                }
                //Debug.LogFormat("weaponA.gameObject.name:{0},selectedWeaponAUName{1}", weaponA.gameObject.name, selectedWeaponAUName);
                weaponA.gameObject.SetActive(weaponA.gameObject.name == selectedWeaponAUName);
            }
            Debug.Log("Assign weapon attribute");
            WeaponAControl weaponAControl = GetComponent<WeaponAControl>();
            if (!isClient)
            {
                foreach (Weapon weapon in AppSetting.Values.weaponAList)
                {
                    //Debug.LogFormat("weapon.uName:{0}, selectedWeaponAUName:{1}", weapon.uName, selectedWeaponAUName);
                    if (weapon.uName == selectedWeaponAUName)
                    {
                        Debug.Log("Set weaponAControl");
                        weaponAControl.bulletPrefabName = weapon.bulletPrefeb;
                        weaponAControl.effectPrefabName = weapon.bulletEffectPrefeb;
                        weaponAControl.bulletDamage = weapon.damage;
                        weaponAControl.reloadTime = weapon.reload;
                        break;
                    }
                }
            }

        }


        /// <summary>
        /// For get item
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            
            if (isServer && !carUserControl.IsAIPlayer)
            {
                //Debug.LogFormat("NetPlayerManager OnTriggerEnter collider:{0}", other.gameObject.name);
                if (other.gameObject.activeSelf)
                {
                    //Debug.LogFormat("collider is active");
                    string colliderName = other.gameObject.name;


                    if (colliderName == ItemPool.COIN_01 + ItemPool.CLONE )
                    {
                        //Debug.LogFormat("{0} is trigger", colliderName);

                        this.gold += 50;
                        NetworkServer.SendToClient(connectionToClient.connectionId, MessageTypes.MSG_GOLD, new IntegerMessage(50));

                        //other.gameObject.SetActive(false);
                        //NetworkServer.UnSpawn(other.gameObject);
                        ItemPool.singleton.BackToPool(ItemPool.COIN_01, other.gameObject);


                    }
                    else if (colliderName == ItemPool.COIN_02 + ItemPool.CLONE)
                    {
                        //Debug.LogFormat("{0} is trigger", colliderName);
                        this.gold += 10;
                        NetworkServer.SendToClient(connectionToClient.connectionId, MessageTypes.MSG_GOLD, new IntegerMessage(10));

                        //other.gameObject.SetActive(false);
                        //NetworkServer.UnSpawn(other.gameObject);
                        ItemPool.singleton.BackToPool(ItemPool.COIN_02, other.gameObject);
                    }
                    else if (colliderName == ItemPool.DIAMOND + ItemPool.CLONE)
                    {
                        //Debug.LogFormat("{0} is trigger", colliderName);
                        this.diamond += 1;
                        NetworkServer.SendToClient(connectionToClient.connectionId, MessageTypes.MSG_DIAMOND, new IntegerMessage(1));

                        //other.gameObject.SetActive(false);
                        //NetworkServer.UnSpawn(other.gameObject);
                        ItemPool.singleton.BackToPool(ItemPool.DIAMOND, other.gameObject);
                    }
                    else if (colliderName == ItemPool.FIRST_AID + ItemPool.CLONE)
                    {
                        //Debug.LogFormat("{0} is trigger", colliderName);
                        this.health.Aid(50);

                        //other.gameObject.SetActive(false);
                        //NetworkServer.UnSpawn(other.gameObject);
                        ItemPool.singleton.BackToPool(ItemPool.FIRST_AID, other.gameObject);
                    }

                }
            }
            

        }

        public void RebrithAiPlayer()
        {
            NetworkGameController.singleton.CreateAIPlayer(bcar.uName, selectedWeaponAUName, selectedWeaponBUName, playerName, aiConnectionId);
            //NetworkGameController.singleton.ReSpawnAIPlayer(this.gameObject);
        }
        
    } 
}
