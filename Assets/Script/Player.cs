using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ahyim.planet
{
    [System.Serializable]
    public class Player
    {
        public string uid;
        public string name;
        public int score;
        public int gold;
        public int diamond;
        public List<string> battleCarUNameList;
        public List<string> weaponAUNameList;
        public List<string> weaponBUNameList;
        public string selectedCarUName;
        public string selectedweaponAUName;
        public string selectedWeaponBUName;

        private static Player currInstance;
        private BattleCar currBattleCar;
        private Weapon currWeaponA;
        private Weapon currWeaponB;
        private int currPlanetIndex = 0;
        
        public static Player CurrentPlayer
        {
            get { return currInstance; }
            
        }

        public BattleCar CurrBattleCar
        {
            get
            {
                return currBattleCar;
            }

        }

        public Weapon CurrWeaponA
        {
            get
            {
                return currWeaponA;
            }
            
        }

        public Weapon CurrWeaponB
        {
            get
            {
                return currWeaponB;
            }

        }

        public int CurrPlanetIndex
        {
            get
            {
                return currPlanetIndex;
            }
            set
            {
                currPlanetIndex = value;
            }
        }

        public string ToJSONString()
        {
            return JsonUtility.ToJson(this, true);
        }
        public static void LoadCurrPlayerFromJSON(string jsonString)
        {
            currInstance = JsonUtility.FromJson<Player>(jsonString);
        }
        public static void InitPlayer(string name)
        {
            Debug.Log("Start init player");
            currInstance = new Player();
            currInstance.uid = PlayerAuthInfo.UserUID;
            currInstance.name = name;
            currInstance.score = 0;
            currInstance.gold = 1000;
            currInstance.diamond = 50;
            currInstance.battleCarUNameList = new List<string>();
            currInstance.weaponAUNameList = new List<string>();
            currInstance.weaponBUNameList = new List<string>();
            Debug.Log("Player Get free Battle Car");
            foreach (BattleCar bCar in AppSetting.Values.battleCarList)
            {
                if (bCar.priceDiamond == 0 && bCar.priceGold == 0)
                {
                    currInstance.battleCarUNameList.Add(bCar.uName);
                }
            }
            Debug.Log("Player Get free weapon A");
            foreach (Weapon weaponA in AppSetting.Values.weaponAList)
            {
                if (weaponA.priceDiamond == 0 && weaponA.priceGold == 0)
                {
                    currInstance.weaponAUNameList.Add(weaponA.uName);
                }
            }
            Debug.Log("Player Get free weapon B");
            foreach (Weapon weaponB in AppSetting.Values.weaponBList)
            {
                if (weaponB.priceDiamond == 0 && weaponB.priceGold == 0)
                {
                    currInstance.weaponBUNameList.Add(weaponB.uName);
                }
            }
            Debug.Log("Player select default car and weapon");
            if (currInstance.battleCarUNameList.Count > 0)
            {
                currInstance.selectedCarUName = currInstance.battleCarUNameList[0];
                currInstance.updateCurrentBattleCar();
            }
            if (currInstance.weaponAUNameList.Count > 0)
            {
                currInstance.selectedweaponAUName = currInstance.weaponAUNameList[0];
                currInstance.updateCurrentWeaponA();
            }
            if (currInstance.weaponBUNameList.Count > 0)
            {
                currInstance.selectedWeaponBUName = currInstance.weaponBUNameList[0];
                currInstance.updateCurrentWeaponB();
            }

            Debug.Log("init completed");

        }



        public void SavePlayer( System.Action<System.Threading.Tasks.Task> callback)
        {
#if UNITY_EDITOR == false
            FireBaseDatabase.WritePlayerData(this, callback);
#else
            System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Run( () => Debug.Log("Bypass player save in editor mod")  );
            task.Wait();
            callback.Invoke(task);
#endif
        }
        /// <summary>
        /// Load current player by uid and assign true to callback if record found 
        /// </summary>
        /// <param name="completedLoadCallback"></param>
        public static void LoadCurrPlayer(System.Action<bool> completedLoadCallback)
        {
            currInstance = new Player();
            currInstance.uid = PlayerAuthInfo.UserUID;
            FireBaseDatabase.ReadUserData(currInstance,
                task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        Debug.LogFormat("Load Player fault, {0}", task.Exception.Message);
                    }
                    Firebase.Database.DataSnapshot snapshot = task.Result;
                    string resultJSON = snapshot.GetRawJsonValue();
                    Debug.LogFormat("resultJSON:{0}", resultJSON);
                    if (resultJSON != null && resultJSON.Trim().Length > 0)
                    {
                        currInstance = JsonUtility.FromJson<Player>(resultJSON);
                        if (currInstance.selectedCarUName == null && currInstance.battleCarUNameList.Count > 0)
                        {
                            currInstance.selectedCarUName = currInstance.battleCarUNameList[0];
                        }
                        
                       if(currInstance.selectedweaponAUName == null && currInstance.weaponAUNameList.Count > 0)
                        {
                            currInstance.selectedweaponAUName = currInstance.weaponAUNameList[0];
                        }
                        if(currInstance.selectedWeaponBUName == null && currInstance.weaponBUNameList.Count > 0)
                        {
                            currInstance.selectedWeaponBUName = currInstance.weaponBUNameList[0];
                        }
                        currInstance.updateCurrentBattleCar();
                        currInstance.updateCurrentWeaponA();
                        currInstance.updateCurrentWeaponB();

                        completedLoadCallback.Invoke(true);
                    }
                    else
                    {
                        completedLoadCallback.Invoke(false);
                    }
                    
                }


                );


        }
#if UNITY_EDITOR
        public static void DummyPlayer()
        {
            string tempJSON = "{ " +
                "  \"battleCarUNameList\" : [ \"sportCar\", \"warTransport\" ], " +
                "  \"diamond\" : 50, " +
                "  \"gold\" : 1000, " +
                "  \"name\" : \"Dummy\", " +
                "  \"score\" : 0, " +
                "  \"selectedCarUName\" : \"sportCar\", " +
                "  \"selectedWeaponBUName\" : \"\", " +
                "  \"selectedweaponAUName\" : \"LaserGun\", " +
                "  \"uid\" : \"LnHVnAGfKlWgZ1i3WVeaG0A3tXw1\", " +
                "  \"weaponAUNameList\" : [ \"LaserGun\", \"archtronic\" ] " +
                "} ";
            currInstance = JsonUtility.FromJson<Player>(tempJSON);
            currInstance.updateCurrentBattleCar();
            currInstance.updateCurrentWeaponA();
            currInstance.updateCurrentWeaponB();

        }
#endif
        public static IEnumerator LoadPlayerByUIDFromServer(string uid, System.Action<bool,string, Player> completedLoadCallback)
        {
            WWW www = new WWW( string.Format("http://localhost:8888/user?{0}",uid));
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {

                completedLoadCallback(false, www.error,null);
            }
            else
            {   Player player = JsonUtility.FromJson<Player>(www.text);
                currInstance = player;
                currInstance.updateCurrentBattleCar();
                currInstance.updateCurrentWeaponA();
                currInstance.updateCurrentWeaponB();
                Debug.LogError(player.ToString());
                completedLoadCallback(true, null, player);
            }
        }



        public void updateCurrentBattleCar()
        {
            currBattleCar = AppSetting.Values.getBattleCarByUName(selectedCarUName);
        }
        public void updateCurrentWeaponA()
        {
            currWeaponA = AppSetting.Values.getWeaponAByUName(selectedweaponAUName);
        }
        public void updateCurrentWeaponB()
        {
            currWeaponB = AppSetting.Values.getWeaponBByUName(selectedWeaponBUName);
        }
        public override string ToString()
        {
            
            return string.Format(
                "uid:{0}, name:{1}, score:{2}, gold:{3}, diamond:{4}, selectedCarUName:{5}, selectedweaponAUName:{6}, selectedWeaponBUName:{7}, battleCarUNameList:{8}, weaponAUNameList:{9}, weaponBUNamelist:{10}",
                uid, name, score, gold, diamond, selectedCarUName, selectedweaponAUName, selectedWeaponBUName, Tool.ListValueToString(battleCarUNameList), Tool.ListValueToString(weaponAUNameList), Tool.ListValueToString(weaponBUNameList));


            
        }
    } 
}
