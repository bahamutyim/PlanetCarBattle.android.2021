using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ahyim.planet
{
    public static class AppParameters
    {

        private static Firebase.Auth.FirebaseUser user;

        private static Dictionary<string, string> parameter = new Dictionary<string, string>();

        public static Dictionary<string, string> getParameters()
        {
            return parameter;
        }

        public static string getParameter(string key)
        {
            if (parameter.ContainsKey(key))
            {
                return parameter[key];
            }
            return null;
        }

        public static void setParmeter(string key, string value)
        {
            parameter[key] = value;
        }

        public static Firebase.Auth.FirebaseUser User
        {
            get { return user; }
            set { user = value; }

        }

        
    }
}
