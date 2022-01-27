using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading;


namespace com.ahyim.planet
{
    [RequireComponent(typeof(WeaponMenu))]
    [RequireComponent(typeof(GarageMenu))]
    public class MainMenu : MonoBehaviour
    {
        public Image vehicleMainImage;
        public Image vehicleGarageImage;
        public RawImage weaponAImage;
        
        
        
        public Text GoldText;
        public Text DiamondText;
        public Text PlayerNameText;
        public Text messageButtonText;

        public GameObject mainPanel;
        public GameObject planetPanel;
        public GameObject garagePanel;
        public GameObject weaponPanel;
        public GameObject shopPanel;
        public GameObject messagePanel;
        public GameObject settingPanel;



        public Text weaponDamageValueText;
        public Text weaponReloadValueText;

        public Slider powerValueSlider;
        public Slider ammoValueSlider;
        public Slider weightValueSlider;

        public Text versionText;

        

        private MySceneManager mySceneManager;

        private GameObject activePanel;

        private BattleCar currBattleCar;
        private Weapon currWeaponA;
        private Weapon currWeaponB;
        

        
        private WeaponMenu weaponMenu;
        private GarageMenu garageMenu;

        private Action messageCallback;

        public BattleCar CurrBattleCar
        {
            get
            {
                return currBattleCar;
            }

            set
            {
                currBattleCar = value;
                //vehicleMainImage.sprite = Resources.Load<Sprite>("Image/UI/Vehicle/" + currBattleCar.image);
                //vehicleGarageImage.sprite = Resources.Load<Sprite>("Image/UI/Vehicle/" + currBattleCar.image);
            }
        }

        public Weapon CurrWeaponA
        {
            get
            {
                return currWeaponA;
            }

            set
            {
                currWeaponA = value;
            }
        }

        public Weapon CurrWeaponB
        {
            get
            {
                return currWeaponB;
            }

            set
            {
                currWeaponB = value;
            }
        }
        private void Awake()
        {
#if UNITY_EDITOR
            if (AppSetting.Values == null)
            {
                StartCoroutine(AppSetting.LoadSettingFromServer(AppSettingLoadingCallBack));

                //NetworkManager.singleton.networkAddress = "192.168.137.150";
                //NetworkManager.singleton.networkAddress = "ec2-35-162-60-75.us-west-2.compute.amazonaws.com";
            }
            
#endif
        }

#if UNITY_EDITOR
        private void AppSettingLoadingCallBack(bool isSuccess, string message)
        {
            Debug.Log("Load setting success");
            if (Player.CurrentPlayer == null)
            {
                //Player.DummyPlayer();
                StartCoroutine(Player.LoadPlayerByUIDFromServer("LnHVnAGfKlWgZ1i3WVeaG0A3tXw1", PlayerLoadingCallBack));
            }
        }
        private void PlayerLoadingCallBack(bool isSuccess, string message, Player player)
        {
            Debug.Log("Load Player success");
            init();
        }

#endif
        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR == false
            init();
#endif
            versionText.text = string.Format( "ver. {0}", Application.version);

            

        }
        private void init()
        {
            Debug.LogFormat("Curr Player:" + Player.CurrentPlayer.ToString()
                );
            weaponMenu = GetComponent<WeaponMenu>();
            mySceneManager = GetComponent<MySceneManager>();
            garageMenu = GetComponent<GarageMenu>();

            CurrBattleCar = Player.CurrentPlayer.CurrBattleCar;
            CurrWeaponA = Player.CurrentPlayer.CurrWeaponA;
            CurrWeaponB = Player.CurrentPlayer.CurrWeaponB;
            activePanel = mainPanel;
            mainPanel.SetActive(true);
            planetPanel.SetActive(false);
            garagePanel.SetActive(false);
            weaponPanel.SetActive(false);
            shopPanel.SetActive(false);
            settingPanel.SetActive(false);
            UpldatePlayerInfo();


            UpdateVehicleValues(currBattleCar);
            weaponMenu.UpdateWeaponUIValue(CurrWeaponA);
            garageMenu.init();
            weaponMenu.init();
        }
        
        
        

        // Update is called once per frame
        void Update()
        {
            
            

        }

        public void ShowMainMenu()
        {
            //FadeOutAndFadeIn(activePanel, mainPanel);
            ChangePanel(mainPanel);
        }

        public void ShowPlanetPanel()
        {
            ChangePanel(planetPanel);
        }

        public void ShowGaragePanel()
        {
            Debug.Log("ShowGaragePanel");
            garageMenu.lockImage.SetActive(!Player.CurrentPlayer.battleCarUNameList.Contains( Player.CurrentPlayer.selectedCarUName));

            //FadeOutAndFadeIn(activePanel, garagePanel);
            ChangePanel(garagePanel);
        }

        public void ShowWeapon()
        {
            ChangePanel(weaponPanel);
        }

        public void ShowShop()
        {
            ChangePanel(shopPanel);
        }

        public void ShowSetting()
        {
            ChangePanel(settingPanel);
        }
        

        

        public void ExpendScrollPanel( GameObject scrollPanel, GameObject scrollPanelChild )
        {
            ///Get scroll panel child Rect transform for get component width
            RectTransform scrollPanelChildRectTransform = scrollPanelChild.GetComponent<RectTransform>();
            ///Get scroll panel Rect transform for set component width
            RectTransform scrollPanelRectTransform = scrollPanel.GetComponent<RectTransform>();
            scrollPanelRectTransform.sizeDelta = new Vector2(scrollPanelChildRectTransform.sizeDelta.x + scrollPanelChild.transform.localPosition.x, scrollPanelRectTransform.sizeDelta.y);
            Debug.LogFormat("scrollPanelRectTransform sizeDelta:{0}", scrollPanelRectTransform.sizeDelta);
        }

        

        

        
        public void UpldatePlayerInfo()
        {
            GoldText.text = string.Format("{0:N0}", Player.CurrentPlayer.gold);
            DiamondText.text = string.Format("{0:N0}", Player.CurrentPlayer.diamond);
            PlayerNameText.text = Player.CurrentPlayer.name;
        }

        public void StartGame()
        {
            NetworkManager netManager = MyNetworkManager.singleton;
            if (netManager )
            {
                if (netManager.isNetworkActive)
                {
                    netManager.client.Disconnect();
                }
               

            }

            mySceneManager.StartLoadNextScreen("Planet");
        }

        public void ChangePanel(GameObject targetPanel)
        {
            FadeOutAndFadeIn(activePanel, targetPanel);
            activePanel = targetPanel;
        }

        private void FadeOutAndFadeIn(GameObject fadeOutPanel, GameObject fadeInPanel)
        {
            StartCoroutine(DoFadeInOut(fadeOutPanel, fadeInPanel));
            //activePanel = fadeInPanel;
            //fadeOutPanel.SetActive(false);
            //fadeInPanel.SetActive(true);
        }

        private IEnumerator DoFadeInOut(GameObject fadeOutPanel, GameObject fadeInPanel)
        {
            
            CanvasGroup canvasGroup = fadeOutPanel.GetComponent<CanvasGroup>();
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime * 4;
                yield return null;
            }
            fadeOutPanel.SetActive(false);
            canvasGroup = fadeInPanel.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            fadeInPanel.SetActive(true);
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime * 4;
                yield return null;
            }

            yield return null;
        }

        public void UpdateWeaponInfo( Weapon weapon, Texture weaponTexture )
        {
            if (weapon != null)
            {
                weaponReloadValueText.text = string.Format("{0:0.0}s", weapon.reload);
                weaponDamageValueText.text = string.Format("{0:0}", weapon.damage);
                weaponAImage.texture = weaponTexture;
            }
            
        }
        public void UpdateVehicleValues(BattleCar bCar)
        {
            powerValueSlider.value = (float)bCar.power / 100.0f;
            ammoValueSlider.value = (float)bCar.amno / 100.0f;
            weightValueSlider.value = (float)bCar.weight / 100.0f;
        }
        public void ShowMessage(string message)
        {
            UnityEngine.UI.Text messageText = messagePanel.transform.Find("Message").GetComponent<UnityEngine.UI.Text>();
            messageText.text = message;
            messagePanel.SetActive(true);
        }
        public void ShowMessage(string message, Action callback)
        {
            messageCallback = callback;
            ShowMessage(message);
        }
        public void ShowMessage(string message, params object[] args)
        {
            ShowMessage(string.Format(message, args));

        }
        public void ShowMessage(string message, Action callback, params object[] args)
        {
            messageCallback = callback;
            ShowMessage(string.Format(message, args));
        }
        public void ShowMessage(string message, string buttonText, Action callback, params object[] args)
        {
            messageCallback = callback;
            messageButtonText.text = buttonText;

            ShowMessage(string.Format(message, args));
        }


        public void HideMessage()
        {
            messagePanel.SetActive(false);
            if (messageCallback != null)
            {
                messageCallback.Invoke();
                messageCallback = null;
            }
            Text messButtonText = messagePanel.transform.GetComponentInChildren<Text>();
            messButtonText.text = "Close";
        }
        public void ExitGame()
        {
            Application.Quit();
        }
        public void setPlanetIndex(int index)
        {
            Player.CurrentPlayer.CurrPlanetIndex = index;
            ShowMainMenu();
        }

        //private async System.Threading.Tasks.Task<IEnumerator> CreateUPNPAsync()
        //{
        //    var discoverer = new NatDiscoverer();
        //    var cts = new CancellationTokenSource();
        //    var device = discoverer.

        //}
    } 
}
