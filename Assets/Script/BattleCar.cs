using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.ahyim.planet
{
    [System.Serializable]
    public class BattleCar
    {
        public string name;
        public string uName;
        public string prefebName;
        public string image;
        public int priceGold;
        public int priceDiamond;
        public int power;
        public int amno;
        public int weight;
        public string version;
        public int score;

        public static BattleCar Clone(BattleCar bCar)
        {
            BattleCar cloneCar = new BattleCar();
            cloneCar.name = bCar.name;
            cloneCar.uName = bCar.uName;
            cloneCar.prefebName = bCar.prefebName;
            cloneCar.image = bCar.image;
            cloneCar.priceGold = bCar.priceGold;
            cloneCar.priceDiamond = bCar.priceDiamond;
            cloneCar.power = bCar.power;
            cloneCar.amno = bCar.amno;
            cloneCar.weight = bCar.weight;
            cloneCar.version = bCar.version;
            cloneCar.score = bCar.score;

            return cloneCar;
        }

        public override string ToString()
        {
            return string.Format(
                "[name:{0}, uName:{1}, prefebName:{2}, image:{3}, priceGold:{4}, priceDiamond:{5}, power:{6}, amno:{7}, weight:{8}, version:{9}, score:{10}]",
                name, uName, prefebName, image, priceGold, priceDiamond, power, amno, weight, version, score
                );
        }
    } 
}
