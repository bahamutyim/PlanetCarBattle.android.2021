using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ahyim.planet
{
    public class ServerInitLoading : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
#if UNITY_STANDALONE
            StartCoroutine(AppSetting.LoadSettingFromServer(loadSettingCompleted));
#endif

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void loadSettingCompleted(bool loadSuccess, string exceptionMessage)
        {
            if (loadSuccess)
            {
                Debug.Log("Setting load completed");
                int tempId = -1000;

                List<AIPlayer> aiPlayerList = AppSetting.Values.aiPlayerList;
                foreach( AIPlayer aiPlayer in aiPlayerList )
                {
                    NetworkGameController.singleton.CreateAIPlayer(aiPlayer.carUName, aiPlayer.weaponAName, aiPlayer.weaponBName, aiPlayer.aiPlayerName, tempId--);
                }

                //NetworkGameController.singleton.CreateAIPlayer("sportCar", "LaserGun",null, "Sunny Dog", tempId--);
                //NetworkGameController.singleton.CreateAIPlayer("AR8C", "LaserGun", null, "Mimi Dog", tempId--);
                //NetworkGameController.singleton.CreateAIPlayer("MuscleCarRim2", "LaserGun", null, "Sunny King", tempId--);
                //NetworkGameController.singleton.CreateAIPlayer("GTCoupe", "LaserGun", null, "Mimi Queen", tempId--);
                


            }
            else
            {
                Debug.LogErrorFormat("Setting load error: {0}", exceptionMessage);
            }
        }
    } 
}
