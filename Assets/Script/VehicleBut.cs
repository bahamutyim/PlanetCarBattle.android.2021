using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ahyim.planet
{
    public class VehicleBut : MonoBehaviour
    {
        public RawImage vehicleImage;
        public GameObject lockImageObject;

        private GarageMenu garageMenu;
        private BattleCar battleCar;

        public GarageMenu GarageMenu
        {
            

            set
            {
                garageMenu = value;
            }
        }

        public BattleCar BattleCar
        {

            get
            {
                return battleCar;
            }
            set
            {
                battleCar = value;
                //vehicleImage.sprite = Resources.Load<Sprite>("Image/UI/Vehicle/" + battleCar.image);
            }
        }

        public void OnClick(  )
        {
            garageMenu.VehicleButtonOnClick(this);

        }
    } 
}
