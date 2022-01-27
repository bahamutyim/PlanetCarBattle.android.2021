using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ahyim.planet
{
    public class AppSetting
    {
        private static System.Collections.Generic.Dictionary<string, object> defaults =
                new System.Collections.Generic.Dictionary<string, object>();

        private const string STORAGE_URL_KEY = "storageURLKey";
        private const string DATABASE_URL_KEY = "databaseURLKey";
        private const string SCREEN_WIDTH_KEY = "screenWidth";
        private const string WEB_SETTING_URL = "http://localhost:8888/setting";

        private static AppValues appValues;
        
       

        //static AppSetting()
        //{
        //    //defaults.Add(PLANET_URL_KEY, "{\"PlanetUrlArray\":[\"192.168.137.51:7777\",\"192.168.137.52:7777\"]}");
            
        //    //Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
        //    Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync();

        //    //appValues = JsonUtility.FromJson<AppValues>("{\"PlanetUrlArray\":[\"192.168.137.51:7777\",\"192.168.137.52:7777\"]}");

        //}

        public static void LoadSetting(System.Action<bool, string> callback)
        {
            Debug.Log("LoadSetting");
            //when load remote config completed, run ActivateFetched() and load setting from Database
            Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync().ContinueWith(
                (System.Threading.Tasks.Task configTask) => {
                    if (configTask.IsCanceled || configTask.IsFaulted)
                    {
                        Debug.LogErrorFormat("Load Config error: {0}", configTask.Exception.Message);
                        callback(false, configTask.Exception.ToString());
                    }
                    else
                    {
                        Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
                        Debug.Log("Activate Fetched Data");
                        Debug.LogFormat("AppSetting.StorageURL: {0}", AppSetting.StorageURL);
                        Debug.LogFormat("AppSetting.DatabaseURL: {0}", AppSetting.DatabaseURL);
                         

                        if (AppSetting.DatabaseURL != null && AppSetting.DatabaseURL.Trim().Length > 0)
                        {
                            Debug.Log("load setting");
                            FireBaseDatabase.init();
                            Debug.Log("FireBaseDatabase.init()");
                            FireBaseDatabase.GetValueAsync("appSetting",
                                    dbTask =>
                                    {
                                        if (dbTask.IsCanceled || dbTask.IsFaulted)
                                        {
                            // Handle the error...
                                            Debug.LogErrorFormat(dbTask.Exception.Message);
                                            callback(false, dbTask.Exception.ToString());
                                        }
                                        else if (dbTask.IsCompleted)
                                        {
                                            Firebase.Database.DataSnapshot snapshot = dbTask.Result;
                                            appValues = JsonUtility.FromJson<AppValues>(snapshot.GetRawJsonValue());
                                            Debug.Log("app values set from firebase database!");
                                            Debug.Log(appValues.ToString());
                                            callback(true, null);
                                        }
                                    }


                            );
                        }
                        else
                        {
                            callback(false, "Config setup is not completed, Please retry.");
                        }
                    }


                }

                );

            
        }

       
        
        public static IEnumerator LoadSettingFromServer(System.Action<bool, string> callback)
        {
            WWW www = new WWW(WEB_SETTING_URL);
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogErrorFormat("LoadSettingFromServer error:{0}", www.error);
                callback(false, www.error);
            }
            else
            {
                appValues = JsonUtility.FromJson<AppValues>(www.text);
                Debug.LogError(appValues.ToString());
                callback(true, null);
            }
                
            

        }

        

        

        public static bool IsConfigReady
        {
            get { return Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched(); }
        }
      
   

        public static string StorageURL
        {
            get { return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(STORAGE_URL_KEY).StringValue; }
        }
        public static string DatabaseURL
        {
            get { return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(DATABASE_URL_KEY).StringValue; }
        }
        public static string ScreenWidth
        {
            get { return Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(SCREEN_WIDTH_KEY).StringValue; }
        }

        public static AppValues Values
        {
            get { return appValues; }
        }

        

        public static void GenAppValuesJSON()
        {
            AppValues testAppVal = new AppValues();
            testAppVal.planetUrlList = new List<string>();
            testAppVal.planetUrlList.Add("192.168.137.51:7777");
            testAppVal.planetUrlList.Add("192.168.137.52:7777");

            testAppVal.battleCarList = new List<BattleCar>();
            testAppVal.battleCarList.Add(new BattleCar());
            testAppVal.battleCarList[0].name = "car1";
            testAppVal.battleCarList[0].prefebName = "prefebName";
            testAppVal.battleCarList[0].priceDiamond = 100;
            testAppVal.battleCarList[0].priceGold = 1000;
            testAppVal.battleCarList[0].power = 100;
            testAppVal.battleCarList[0].weight = 1000;
            testAppVal.battleCarList[0].amno = 500;

            testAppVal.weaponAList = new List<Weapon>();
            testAppVal.weaponAList.Add(new Weapon());
            testAppVal.weaponAList[0].name = "weapon 1";
            testAppVal.weaponAList[0].prefebName = "Prefeb name";
            testAppVal.weaponAList[0].priceDiamond = 50;
            testAppVal.weaponAList[0].priceGold = 100;
            testAppVal.weaponAList[0].type = "gun";
            testAppVal.weaponAList[0].damage = 100;
            testAppVal.weaponAList[0].reload = 10;

            testAppVal.weaponBList = new List<Weapon>();
            testAppVal.weaponBList.Add(new Weapon());
            testAppVal.weaponBList[0].name = "weapon 2";
            testAppVal.weaponBList[0].prefebName = "Prefeb name";
            testAppVal.weaponBList[0].priceDiamond = 50;
            testAppVal.weaponBList[0].priceGold = 100;
            testAppVal.weaponBList[0].type = "gun";
            testAppVal.weaponBList[0].damage = 100;
            testAppVal.weaponBList[0].reload = 10;


            string tempJson = JsonUtility.ToJson(testAppVal,true);
            System.IO.StreamWriter file = new System.IO.StreamWriter(System.IO.Path.Combine(Application.streamingAssetsPath, "AppValues_simple.json"));
            file.Write(tempJson);
            file.Close();
        }
        public static void LoadSettingFromDB(System.Action<bool,string> callback)
        {
            //Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
            //Firebase.Storage.StorageReference settingRef = storage.GetReferenceFromUrl(StorageURL + "/setting/app.setting.json");
            //const long maxAllowedSize = 1 * 1024 * 1024;
            //settingRef.GetBytesAsync(maxAllowedSize).ContinueWith((System.Threading.Tasks.Task<byte[]> task) => {
            //    if (task.IsFaulted || task.IsCanceled)
            //    {
            //        Debug.Log("Get file fail: " + task.Exception.ToString());
            //        callback(false, task.Exception.ToString());
            //        // Uh-oh, an error occurred!
            //    }
            //    else
            //    {
            //        byte[] fileContents = task.Result;
            //        string settingStr = System.Text.UTF8Encoding.Default.GetString(fileContents);
            //        appValues = JsonUtility.FromJson<AppValues>(settingStr);
            //        Debug.Log("app values set from firebase!");
            //        callback(true, null);
            //    }
            //});

            FireBaseDatabase.init();
            FireBaseDatabase.GetValueAsync("appSetting",
                    task =>
                    {
                        if (task.IsFaulted)
                        {
                            // Handle the error...
                            Debug.Log(task.Exception.Message);
                            callback(false, task.Exception.ToString());
                        }
                        else if (task.IsCompleted)
                        {
                            Firebase.Database.DataSnapshot snapshot = task.Result;
                            appValues = JsonUtility.FromJson<AppValues>(snapshot.GetRawJsonValue());
                            Debug.Log("app values set from firebase database!");
                            Debug.Log(appValues.ToString());
                            callback(true, null);
                        }
                    }


            );


        }
#if UNITY_EDITOR
        public static void TestSetting()
        {
            string appSettingJSON = "{ " +
                "    \"battleCarList\" : [ { " +
               "  \"amno\" : 20, " +
                "  \"image\" : \"SportCarPic\", " +
                "  \"name\" : \"Sport Car\", " +
                "  \"power\" : 80, " +
                "  \"prefebName\" : \"NetSportCar\", " +
                "  \"priceDiamond\" : 0, " +
                "  \"priceGold\" : 0, " +
                "  \"uName\" : \"sportCar\", " +
                "  \"weight\" : 20 " +
                "}, { " +
                "  \"amno\" : 80, " +
                "  \"image\" : \"WarTransportPic\", " +
                "  \"name\" : \"WarTransport\", " +
                "  \"power\" : 70, " +
                "  \"prefebName\" : \"NetWarTransport\", " +
                "  \"priceDiamond\" : 0, " +
                "  \"priceGold\" : 0, " +
                "  \"uName\" : \"warTransport\", " +
                "  \"weight\" : 90 " +
                "}, { " +
                "  \"amno\" : 50, " +
                "  \"image\" : \"car3.jpg\", " +
                "  \"name\" : \"car3\", " +
                "  \"power\" : 30, " +
                "  \"prefebName\" : \"NetFireGTO\", " +
                "  \"priceDiamond\" : 50, " +
                "  \"priceGold\" : 1000, " +
                "  \"uName\" : \"fireGTO\", " +
                "  \"weight\" : 30 " +
                "  } ], " +
                "  \"planetUrlList\" : [ \"192.168.137.150\", \"192.168.137.52:7777\", \"ec2-35-162-60-75.us-west-2.compute.amazonaws.com\" ], " +
                "  \"weaponAList\" : [ { " +
                "  \"bulletEffectPrefeb\" : \"Sparks\", " +
                "  \"bulletPrefeb\" : \"NormalBullet\", " +
                "  \"damage\" : 10, " +
                "  \"image\" : \"weapon1.jpg\", " +
                "  \"name\" : \"Laser Gun\", " +
                "  \"prefebName\" : \"LaserGun\", " +
                "  \"priceDiamond\" : 0, " +
                "  \"priceGold\" : 0, " +
                "  \"reload\" : 0.2, " +
                "  \"type\" : \"gun\", " +
                "  \"uName\" : \"LaserGun\" " +
                "}, { " +
                "  \"bulletEffectPrefeb\" : \"Sparks\", " +
                "  \"bulletPrefeb\" : \"NormalBullet\", " +
                "  \"damage\" : 20, " +
                "  \"image\" : \"weapon1.jpg\", " +
                "  \"name\" : \"Machine Gun\", " +
                "  \"prefebName\" : \"MachineGun\", " +
                "  \"priceDiamond\" : 0, " +
                "  \"priceGold\" : 200, " +
                "  \"reload\" : 0.1, " +
                "  \"type\" : \"gun\", " +
                "  \"uName\" : \"MachineGun\" " +
                "}, { " +
                "  \"bulletEffectPrefeb\" : \"Sparks\", " +
                "  \"bulletPrefeb\" : \"NormalBullet\", " +
                "  \"damage\" : 20, " +
                "  \"image\" : \"weapon1.jpg\", " +
                "  \"name\" : \"Archtronic\", " +
                "  \"prefebName\" : \"archtronic\", " +
                "  \"priceDiamond\" : 5, " +
                "  \"priceGold\" : 0, " +
                "  \"reload\" : 0.1, " +
                "  \"type\" : \"gun\", " +
                "  \"uName\" : \"archtronic\" " +
                "}, { " +
                "  \"bulletEffectPrefeb\" : \"Sparks\", " +
                "  \"bulletPrefeb\" : \"NormalBullet\", " +
                "  \"damage\" : 20, " +
                "  \"image\" : \"weapon1.jpg\", " +
                "  \"name\" : \"FireSleet\", " +
                "  \"prefebName\" : \"fireSleet\", " +
                "  \"priceDiamond\" : 15, " +
                "  \"priceGold\" : 0, " +
                "  \"reload\" : 0.1, " +
                "  \"type\" : \"gun\", " +
                "  \"uName\" : \"fireSleet\" " +
                "}, { " +
                "  \"bulletEffectPrefeb\" : \"Sparks\", " +
                "  \"bulletPrefeb\" : \"NormalBullet\", " +
                "  \"damage\" : 20, " +
                "  \"image\" : \"weapon1.jpg\", " +
                "  \"name\" : \"Hellwailer\", " +
                "  \"prefebName\" : \"hellwailer\", " +
                "  \"priceDiamond\" : 0, " +
                "  \"priceGold\" : 1000, " +
                "  \"reload\" : 0.1, " +
                "  \"type\" : \"gun\", " +
                "  \"uName\" : \"hellwailer\" " +
                "}, { " +
                "  \"bulletEffectPrefeb\" : \"Sparks\", " +
                "  \"bulletPrefeb\" : \"NormalBullet\", " +
                "  \"damage\" : 20, " +
                "  \"image\" : \"weapon1.jpg\", " +
                "  \"name\" : \"Grenade Launcher\", " +
                "  \"prefebName\" : \"GrenadeLauncher\", " +
                "  \"priceDiamond\" : 500, " +
                "  \"priceGold\" : 0, " +
                "  \"reload\" : 0.1, " +
                "  \"type\" : \"gun\", " +
                "  \"uName\" : \"GrenadeLauncher\" " +
                "}, { " +
                "  \"bulletEffectPrefeb\" : \"Sparks\", " +
                "  \"bulletPrefeb\" : \"NormalBullet\", " +
                "  \"damage\" : 20, " +
                "  \"image\" : \"weapon1.jpg\", " +
                "  \"name\" : \"Sci Fi Gun Heavy White\", " +
                "  \"prefebName\" : \"SciFiGunHeavyWhite\", " +
                "  \"priceDiamond\" : 0, " +
                "  \"priceGold\" : 10000, " +
                "  \"reload\" : 0.1, " +
                "  \"type\" : \"gun\", " +
                "  \"uName\" : \"SciFiGunHeavyWhite\" " +
                "}, { " +
                "  \"bulletEffectPrefeb\" : \"Sparks\", " +
                "  \"bulletPrefeb\" : \"NormalBullet\", " +
                "  \"damage\" : 20, " +
                "  \"image\" : \"weapon1.jpg\", " +
                "  \"name\" : \"Sci Fi Gun Light White\", " +
                "  \"prefebName\" : \"SciFiGunLightWhite\", " +
                "  \"priceDiamond\" : 0, " +
                "  \"priceGold\" : 10000000, " +
                "  \"reload\" : 0.1, " +
                "  \"type\" : \"gun\", " +
                "  \"uName\" : \"SciFiGunLightWhite\" " +
                "}, { " +
                "  \"bulletEffectPrefeb\" : \"Sparks\", " +
                "  \"bulletPrefeb\" : \"NormalBullet\", " +
                "  \"damage\" : 20, " +
                "  \"image\" : \"weapon1.jpg\", " +
                "  \"name\" : \"Plaz S1\", " +
                "  \"prefebName\" : \"plaz_s1\", " +
                "  \"priceDiamond\" : 50000, " +
                "  \"priceGold\" : 0, " +
                "  \"reload\" : 0.1, " +
                "  \"type\" : \"gun\", " +
                "  \"uName\" : \"plaz_s1\" " +
                "  } ] " +
                "} ";
            appValues = JsonUtility.FromJson<AppValues>(appSettingJSON);
        }
        

#endif

    }

    [System.Serializable]
    public class AppValues
    {
        public List<string> planetUrlList;
        public List<BattleCar> battleCarList;
        public List<Weapon> weaponAList;
        public List<Weapon> weaponBList;
        public List<AIPlayer> aiPlayerList;

        public override string ToString()
        {
            return string.Format(
                "planetUrlList:{0}, battleCarList:{1}, weaponAList:{2}, weaponBList:{3}, aiPlayerList:{4}",
                Tool.ListValueToString(planetUrlList), Tool.ListValueToString(battleCarList), Tool.ListValueToString(weaponAList), Tool.ListValueToString(weaponBList), Tool.ListValueToString(aiPlayerList)
                );
        }
        public BattleCar getBattleCarByUName(string uName)
        {
            BattleCar returnBattleCar = null;
            foreach(BattleCar bCar in battleCarList )
            {
                if (bCar.uName == uName)
                {
                    returnBattleCar = BattleCar.Clone(bCar);
                    break;
                }
            }

            return returnBattleCar;
        }
        public Weapon getWeaponAByUName(string uName)
        {
            Weapon returnWeapon = null;
            foreach(Weapon weapon in weaponAList)
            {
                if(weapon.uName == uName)
                {
                    returnWeapon = Weapon.Clone(weapon);
                    break;
                }
            }

            return returnWeapon;
        }
        public Weapon getWeaponBByUName(string uName)
        {
            Weapon returnWeapon = null;
            foreach (Weapon weapon in weaponBList)
            {
                if (weapon.uName == uName)
                {
                    returnWeapon = Weapon.Clone(weapon);
                    break;
                }
            }

            return returnWeapon;
        }
    }


}