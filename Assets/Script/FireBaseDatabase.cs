using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;


namespace com.ahyim.planet
{
    public static class FireBaseDatabase
    {
        public static void init()
        {
            // Set up the Editor before calling into the realtime database.
            Debug.LogFormat("Firebase DB init : {0}", AppSetting.DatabaseURL);
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(AppSetting.DatabaseURL );
            Debug.Log("Firebase DB init completed");
        }

        public static void GetValueAsync(string referencePath, System.Action<Task<DataSnapshot>> callback)
        {
            FirebaseDatabase.DefaultInstance.GetReference(referencePath)
                .GetValueAsync().ContinueWith(callback);
            

        }

        public static void WritePlayerData(Player savePlayer, System.Action<Task> callback  )
        {
            FirebaseDatabase.DefaultInstance.GetReference("users").Child(savePlayer.uid).SetRawJsonValueAsync(savePlayer.ToJSONString())
                .ContinueWith(callback);
        }

        public static void ReadUserData(Player player, System.Action<Task<DataSnapshot>> callback)
        {
            FirebaseDatabase.DefaultInstance.GetReference("users").Child(player.uid)
                .GetValueAsync().ContinueWith(callback);
        }

        public static void WriteTransactionLog(Player player, TransactionLog transactionLog, System.Action<Task> callback  )
        {
            
            FirebaseDatabase.DefaultInstance.GetReference("transactionLog").Child(player.uid).Child(DateTime.Now.ToString("yyyyMMddHHmmss")).SetRawJsonValueAsync(transactionLog.ToJSONString())
                .ContinueWith(callback);
        }
        
    }

}