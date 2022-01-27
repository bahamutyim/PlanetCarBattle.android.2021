using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ahyim.planet
{
    public class PlayerAuthInfo : MonoBehaviour
    {
        
        public string Vehicle
        {
            get 
            {
                return AppParameters.getParameter(Constants.PLAYER_VEHICLE_KEY);
            }
            set
            {
                AppParameters.setParmeter(Constants.PLAYER_VEHICLE_KEY, value);
            }
        }

        private const string authMethodKey = "authMethod";
        public static string AuthMethod
        {
            get { return PlayerPrefs.GetString(authMethodKey); }
            set { PlayerPrefs.SetString(authMethodKey, value); }
        }

        private const string authTokenKey = "authToken";
        public static string AuthToken
        {
            get { return PlayerPrefs.GetString(authTokenKey); }
            set { PlayerPrefs.SetString(authTokenKey, value); }
        }

        private const string emailLoginKey = "emailLoginKey";
        public static string EmailLogin
        {
            get { return PlayerPrefs.GetString(emailLoginKey); }
            set { PlayerPrefs.SetString(emailLoginKey, value); }
        }

        private const string emailPasswordKey = "emailPasswordKey";
        public static string EmailPassword
        {
            get { return PlayerPrefs.GetString(emailPasswordKey); }
            set { PlayerPrefs.SetString(emailPasswordKey, value); }
        }

        private const string userUIDKey = "userUIDKey";
        public static string UserUID
        {
            get { return PlayerPrefs.GetString(userUIDKey); }
            set { PlayerPrefs.SetString(userUIDKey, value); }
        }

        

    }
}
