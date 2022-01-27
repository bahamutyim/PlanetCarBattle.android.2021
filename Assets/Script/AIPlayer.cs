using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.ahyim.planet
{
    [System.Serializable]
    public class AIPlayer
    {

        public string carUName;
        public string weaponAName;
        public string weaponBName;
        public string aiPlayerName;
        

        public override string ToString()
        {
            return string.Format(
                "[carUName:{0}, weaponAName:{1}, weaponBName:{2}, aiPlayerName:{3}]",
                carUName, weaponAName, weaponBName, aiPlayerName
                );
        }
    }

    

}