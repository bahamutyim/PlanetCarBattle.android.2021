using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ahyim.planet
{
    [System.Serializable]
    public class Weapon
    {
        public string name;
        public string uName;
        public string prefebName;
        public string image;
        public string type;
        public int priceGold;
        public int priceDiamond;
        public int damage;
        public float reload;
        public string bulletPrefeb;
        public string bulletEffectPrefeb;
        public string version;

        public static Weapon Clone(Weapon weapon)
        {
            Weapon cloneWeapon = new Weapon();
            cloneWeapon.name = weapon.name;
            cloneWeapon.uName = weapon.uName;
            cloneWeapon.prefebName = weapon.prefebName;
            cloneWeapon.image = weapon.image;
            cloneWeapon.type = weapon.type;
            cloneWeapon.priceGold = weapon.priceGold;
            cloneWeapon.priceDiamond = weapon.priceDiamond;
            cloneWeapon.damage = weapon.damage;
            cloneWeapon.reload = weapon.reload;
            cloneWeapon.bulletPrefeb = weapon.bulletPrefeb;
            cloneWeapon.bulletEffectPrefeb = weapon.bulletEffectPrefeb;
            cloneWeapon.version = weapon.version;
            return cloneWeapon;
        }

        public override string ToString()
        {
            return string.Format(
                "[name:{0}, uName:{1}, prefebName:{2}, image:{3}, type:{4}, priceGold:{5}, priceDiamond:{6}, damage:{7}, reload:{8}, bulletPrefeb:{9}, bulletEffectPrefeb:{10}, version:{11}]",
                name, uName, prefebName, image, type, priceGold, priceDiamond, damage, reload, bulletPrefeb, bulletEffectPrefeb, version
                );
        }
    }

}