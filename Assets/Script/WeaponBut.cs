using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ahyim.planet
{
    public class WeaponBut : MonoBehaviour
    {
        public RawImage weaponImage;
        public GameObject lockImageObject;

        private WeaponMenu weaponMenu;
        private Weapon weapon;

        public WeaponMenu WeaponMenu
        {
            

            set
            {
                weaponMenu = value;
            }
        }

        public Weapon Weapon
        {

            get
            {
                return weapon;
            }
            set
            {
                weapon = value;
                //vehicleImage.sprite = Resources.Load<Sprite>("Image/UI/Vehicle/" + battleCar.image);
            }
        }

        public void OnClick(  )
        {
            weaponMenu.WeaponButtonOnClick(this);

        }
    } 
}
