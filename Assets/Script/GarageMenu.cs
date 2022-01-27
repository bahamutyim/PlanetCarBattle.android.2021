using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ahyim.planet
{
    [RequireComponent(typeof(MainMenu))]
    public class GarageMenu : MonoBehaviour
    {
        public Camera renderTextureCamera;
        public GameObject vechicleScrollPanel;
        public GameObject lockImage;
        public GameObject pricePlane;
        public Text priceValueText;
        public Image priceImage;
        public GameObject buyButtonObject;
        public GameObject selectButtonObject;

        public Slider powerValueSlider;
        public Slider ammoValueSlider;
        public Slider weightValueSlider;

        private MainMenu mainMenu;
        private GameObject VehicleGameObject;
        private VehicleBut currVehiclBut;

        // Use this for initialization
        void Start()
        {
            
        }
        public void init()
        {
            mainMenu = GetComponent<MainMenu>();


            CreateVehicleButtons();

            ChangeSelectVehicleModel(Player.CurrentPlayer.CurrBattleCar.prefebName);
            UpdateVehicleValues(Player.CurrentPlayer.CurrBattleCar);
            StartCoroutine(ExpendScrollPanelOnEnable());
        }

        // Update is called once per frame
        void Update()
        {
            if (VehicleGameObject)
            {
                VehicleGameObject.transform.Rotate(Vector3.up, 30.0f * Time.deltaTime);
            }
        }

        /// <summary>
        /// Expend scroll Panel when object is enable
        /// </summary>
        /// <returns></returns>
        private IEnumerator ExpendScrollPanelOnEnable()
        {

            //yield return new WaitUntil(() => weaponScrollPanel.activeInHierarchy);
            Debug.LogFormat("weaponScrollPanel.transform.childCount:{0}", vechicleScrollPanel.transform.childCount);
            Transform lastVechicleButtonTransform = vechicleScrollPanel.transform.GetChild(vechicleScrollPanel.transform.childCount - 1);

            yield return new WaitUntil(() => lastVechicleButtonTransform.localPosition.x > 0);

            mainMenu.ExpendScrollPanel(vechicleScrollPanel, lastVechicleButtonTransform.gameObject);
        }

        private void CreateVehicleButtons()
        {
            GameObject VehicleButPrefab = Resources.Load<GameObject>("Prefabs/UI/VehicleBut");
            foreach (BattleCar bcar in AppSetting.Values.battleCarList)
            {

                Debug.LogFormat("Start build car button: {0}", bcar.uName);
                GameObject vehicleButObj = Instantiate<GameObject>(VehicleButPrefab, vechicleScrollPanel.transform);
                VehicleBut butScript = vehicleButObj.GetComponent<VehicleBut>();
                Button vehicleBut = vehicleButObj.GetComponent<Button>();
                butScript.BattleCar = bcar;
                butScript.GarageMenu = this;
                string textureName = string.Format("{0}_{1}", bcar.uName, bcar.version);
                Texture2D bcarTexture = Tool.LoadImage(textureName);
                if (bcarTexture)
                {
                    
                    butScript.vehicleImage.texture = bcarTexture;
                }
                else
                {
                    ChangeSelectVehicleModel(bcar.prefebName);
                    butScript.vehicleImage.texture = Tool.RTImage(renderTextureCamera, textureName);
                }
                vehicleBut.onClick.AddListener(butScript.OnClick);
                butScript.lockImageObject.SetActive(!Player.CurrentPlayer.battleCarUNameList.Contains(bcar.uName));
                Debug.LogFormat("Completed build car button: {0}", bcar.uName);

                //mainMenu.ExpendScrollPanel(vechicleScrollPanel, vehicleButObj);
            }
        }

        private void ChangeSelectVehicleModel(string prefebName)
        {
            Quaternion quaternion = new Quaternion(0, 1, 0, 135.0f * Mathf.Deg2Rad);

            if (VehicleGameObject)
            {
                quaternion = VehicleGameObject.transform.rotation;
                GameObject.DestroyImmediate(VehicleGameObject);
            }
            GameObject loadObj = Resources.Load<GameObject>("Prefabs/Vehicle/" + prefebName);
            if (loadObj)
            {
                GameObject carModel = loadObj.transform.Find("CarModel").gameObject;
                //carModel.GetComponent<CarControl>().enabled = false;
                VehicleGameObject = GameObject.Instantiate<GameObject>(carModel, new Vector3(0, -1f, -5f), quaternion);
                VehicleGameObject.SetActive(true);
                ChangeAllChildrenLayer(VehicleGameObject, 5);
                renderTextureCamera.transform.LookAt(VehicleGameObject.transform);
            }

        }

        private void ChangeAllChildrenLayer(GameObject gObj, int layer)
        {
            gObj.layer = layer;
            foreach (Transform gTransform in gObj.transform)
            {
                ChangeAllChildrenLayer(gTransform.gameObject, layer);
            }

        }

        public void VehicleButtonOnClick(VehicleBut vehicleBut)
        {
            currVehiclBut = vehicleBut;
            ChangeSelectVehicleModel(vehicleBut.BattleCar.prefebName);
            //vehicleGarageImage.sprite = vehicleBut.vehicleImage.sprite;
            
            UpdateVehicleValues(vehicleBut.BattleCar);

        }

        public void UpdateVehicleValues(BattleCar bCar)
        {
            powerValueSlider.value = (float)bCar.power / 100.0f;
            ammoValueSlider.value = (float)bCar.amno / 100.0f;
            weightValueSlider.value = (float)bCar.weight / 100.0f;

            if (Player.CurrentPlayer.battleCarUNameList.Contains(bCar.uName))
            {
                buyButtonObject.SetActive(false);
                selectButtonObject.SetActive(true);
                lockImage.SetActive(false);
                pricePlane.SetActive(false);
            }
            else
            {
                buyButtonObject.SetActive(true);
                selectButtonObject.SetActive(false);
                lockImage.SetActive(true);
                pricePlane.SetActive(true);
                if (bCar.priceDiamond > 0)
                {
                    priceValueText.text = string.Format("{0:0}", bCar.priceDiamond);
                    priceImage.sprite = Resources.Load<Sprite>("Sprites/Rewards_Diamond");
                    
                }
                else if (bCar.priceGold > 0)
                {
                    priceValueText.text = string.Format("{0:0}", bCar.priceGold);
                    priceImage.sprite = Resources.Load<Sprite>("Sprites/Rewards_Coins");
                    
                }
                else
                {
                    pricePlane.SetActive(false);
                }
            }

            
        }

        public void SelectVehicle()
        {
            if (currVehiclBut)
            {
                mainMenu.CurrBattleCar = currVehiclBut.BattleCar;
                Player.CurrentPlayer.selectedCarUName = currVehiclBut.BattleCar.uName;
                Player.CurrentPlayer.updateCurrentBattleCar();

                Player.CurrentPlayer.SavePlayer(
                    task =>
                    {
                        if (task.IsCanceled || task.IsFaulted)
                        {
                            Debug.LogFormat("Player save fail: {0}", task.Exception.Message);
                        }
                        else if (task.IsCompleted)
                        {
                            ChangeSelectVehicleModel(currVehiclBut.BattleCar.prefebName);
                            mainMenu.UpdateVehicleValues(currVehiclBut.BattleCar);
                            mainMenu.ShowMainMenu();
                        }



                    }

                    );

            }
            else
            {
                mainMenu.ShowMainMenu();
            }


        }

        public void BuyVehicle()
        {
            if(currVehiclBut)
            {
                BattleCar bcar = currVehiclBut.BattleCar;
                if (bcar.priceDiamond > 0)
                {
                    if (Player.CurrentPlayer.diamond >= bcar.priceDiamond)
                    {
                        Player.CurrentPlayer.diamond -= bcar.priceDiamond;
                        Player.CurrentPlayer.battleCarUNameList.Add(bcar.uName);
                        Player.CurrentPlayer.SavePlayer(
                            task => {
                                if (task.IsFaulted || task.IsCanceled)
                                {
                                    Player.CurrentPlayer.diamond += bcar.priceDiamond;
                                    Player.CurrentPlayer.battleCarUNameList.Remove(bcar.uName);
                                    Debug.LogErrorFormat("BuyVehicle exception:{0}", task.Exception.Message);
                                }
                                else if (task.IsCompleted)
                                {
                                    BuyVehicleSuccessful();
                                }
                            }

                            );
                    }
                    else
                    {
                        Debug.Log("not enough diamand");
                    }
                }
                else if(bcar.priceGold > 0)
                {
                    if (Player.CurrentPlayer.gold >= bcar.priceGold)
                    {
                        Player.CurrentPlayer.gold -= bcar.priceGold;
                        Player.CurrentPlayer.battleCarUNameList.Add(bcar.uName);
                        Player.CurrentPlayer.SavePlayer(
                            task => {
                                if (task.IsFaulted || task.IsCanceled)
                                {
                                    Player.CurrentPlayer.gold += bcar.priceGold;
                                    Player.CurrentPlayer.battleCarUNameList.Remove(bcar.uName);
                                    Debug.LogErrorFormat("BuyVehicle exception:{0}", task.Exception.Message);
                                }
                                else if (task.IsCompleted)
                                {
                                    BuyVehicleSuccessful();
                                }
                            }

                            );
                    }
                    else
                    {
                        Debug.Log("not enought gold");
                    }
                }
            }
        }
        private void BuyVehicleSuccessful()
        {
            buyButtonObject.SetActive(false);
            selectButtonObject.SetActive(true);
            lockImage.SetActive(false);
            pricePlane.SetActive(false);
            currVehiclBut.lockImageObject.SetActive(false);
            mainMenu.UpldatePlayerInfo();
        }
        public void BackToMainMenu()
        {
            if (Player.CurrentPlayer.selectedCarUName != currVehiclBut.BattleCar.uName)
            {
                ChangeSelectVehicleModel(Player.CurrentPlayer.CurrBattleCar.prefebName);
            }
            mainMenu.ShowMainMenu();
        }
    } 
}
