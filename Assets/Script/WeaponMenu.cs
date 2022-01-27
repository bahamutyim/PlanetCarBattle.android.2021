using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.ahyim.planet
{
    public class WeaponMenu : MonoBehaviour
    {
        public Text damageValueText;
        public Text reloadValueText;
        public Text priceValueText;
        public Image priceImage;
        public GameObject pricePanel;
        public GameObject buyButtonObject;
        public GameObject selectButtonObject;

        public GameObject weaponScrollPanel;
        public GameObject weaponAGameObject;
        public GameObject weaponLockImage;

        public Camera weaponRenderTextureCamera;

        private MainMenu mainMenu;
        private Weapon selectedWeapon;
        private WeaponBut selectWeaponBut;
        private Texture selectedWeaponTexture;

        
        // Use this for initialization
        void Start()
        {
            
        }
        public void init()
        {
            mainMenu = GetComponent<MainMenu>();
            CreateWeaponAButtons();

            ChangeSelectWeaponA(Player.CurrentPlayer.CurrWeaponA.uName);

            StartCoroutine(ExpendScrollPanelOnEnable());
        }

        // Update is called once per frame
        void Update()
        {
            if (weaponAGameObject)
            {
                weaponAGameObject.transform.Rotate(Vector3.up, 30.0f * Time.deltaTime);
            }
        }
        /// <summary>
        /// Expend scroll Panel when object is enable
        /// </summary>
        /// <returns></returns>
        private IEnumerator ExpendScrollPanelOnEnable()
        {
            
            //yield return new WaitUntil(() => weaponScrollPanel.activeInHierarchy);
            Debug.LogFormat("weaponScrollPanel.transform.childCount:{0}", weaponScrollPanel.transform.childCount);
            Transform lastWeaponButtonTransform = weaponScrollPanel.transform.GetChild(weaponScrollPanel.transform.childCount - 1);

            yield return new WaitUntil(() => lastWeaponButtonTransform.localPosition.x > 0  );

            mainMenu.ExpendScrollPanel(weaponScrollPanel, lastWeaponButtonTransform.gameObject);
        }

        public void UpdateWeaponUIValue(Weapon weapon)
        {
            Debug.LogFormat("UpdateWeaponUIValue weapon:{0}", weapon.uName);

            damageValueText.text = string.Format( "{0:0}", weapon.damage);
            reloadValueText.text = string.Format("{0:0.0}s", weapon.reload);

            if( Player.CurrentPlayer.weaponAUNameList.Contains(weapon.uName))
            {
                pricePanel.SetActive(false);
                buyButtonObject.SetActive(false);
                selectButtonObject.SetActive(true);
                weaponLockImage.SetActive(false);
            }
            else
            {
                buyButtonObject.SetActive(true);
                selectButtonObject.SetActive(false);
                weaponLockImage.SetActive(true);

                if (weapon.priceDiamond > 0)
                {
                    priceValueText.text = string.Format("{0:0}", weapon.priceDiamond);
                    priceImage.sprite = Resources.Load<Sprite>("Sprites/Rewards_Diamond");
                    pricePanel.SetActive(true);
                }
                else if (weapon.priceGold > 0)
                {
                    priceValueText.text = string.Format("{0:0}", weapon.priceGold);
                    priceImage.sprite = Resources.Load<Sprite>("Sprites/Rewards_Coins");
                    pricePanel.SetActive(true);
                }
                else
                {
                    pricePanel.SetActive(false);
                }
            }

            
            
        }

        private void CreateWeaponAButtons()
        {
            GameObject weaponButPrefab = Resources.Load<GameObject>("Prefabs/UI/WeaponBut");
            foreach (Weapon weaponA in AppSetting.Values.weaponAList)
            {
                
                ///Instantiate weapon button
                GameObject weaponAObj = Instantiate<GameObject>(weaponButPrefab, weaponScrollPanel.transform);

                WeaponBut butScript = weaponAObj.GetComponent<WeaponBut>();
                Button weaponBut = weaponAObj.GetComponent<Button>();
                butScript.Weapon = weaponA;
                butScript.WeaponMenu = this;

                string textureName = string.Format("{0}_{1}", weaponA.uName, weaponA.version);
                Texture2D weaponeTexture = Tool.LoadImage(textureName);
                if (weaponeTexture)
                {
                    butScript.weaponImage.texture = weaponeTexture;
                }
                else
                {
                    ///Enable Weapon by uName
                    ChangeSelectWeaponA(weaponA.uName);
                    ///Convert Render Texture to Sprite
                    butScript.weaponImage.texture = Tool.RTImage(weaponRenderTextureCamera, textureName);
                }


                

                weaponBut.onClick.AddListener(butScript.OnClick);
                butScript.lockImageObject.SetActive(!Player.CurrentPlayer.weaponAUNameList.Contains(weaponA.uName));

                if (Player.CurrentPlayer.CurrWeaponA.uName == weaponA.uName)
                {
                    mainMenu.UpdateWeaponInfo(weaponA, butScript.weaponImage.texture);
                }
                
            }
        }


        
        public void WeaponButtonOnClick(WeaponBut weaponBut)
        {
            
            ChangeSelectWeaponA(weaponBut.Weapon.uName);
            UpdateWeaponUIValue(weaponBut.Weapon);
            selectedWeapon = weaponBut.Weapon;
            selectedWeaponTexture = weaponBut.weaponImage.texture;
            selectWeaponBut = weaponBut;
        }
        private void ChangeSelectWeaponA(string uName)
        {
            if (weaponAGameObject)
            {
                for (int index = 0; index < weaponAGameObject.transform.childCount; index++)
                {
                    Transform weaponA = weaponAGameObject.transform.GetChild(index);

                    //Debug.LogFormat("weaponA.gameObject.name:{0},uName{1}", weaponA.gameObject.name, uName);
                    weaponA.gameObject.SetActive(weaponA.gameObject.name == uName);
                }

            }
        }
        public void ConfirmSelectWeapon()
        {
            if (selectedWeapon != null)
            {
                Player.CurrentPlayer.selectedweaponAUName = selectedWeapon.uName;
                Player.CurrentPlayer.updateCurrentWeaponA();
                Player.CurrentPlayer.SavePlayer(
                    task =>
                    {
                        if (task.IsCanceled || task.IsFaulted)
                        {
                            Debug.LogErrorFormat("ConfirmSelectWeapon Save Player Exception: {0}", task.Exception.Message);
                        }
                        else if (task.IsCompleted)
                        {
                            mainMenu.UpdateWeaponInfo(selectedWeapon, selectedWeaponTexture);
                            mainMenu.ShowMainMenu();
                        }
                    }

                    );

            }
        }
        public void BuyWeapon()
        {
            if (selectWeaponBut)
            {
                Weapon weapon = selectWeaponBut.Weapon;
                if (weapon.priceDiamond > 0)
                {
                    if (Player.CurrentPlayer.diamond >= weapon.priceDiamond)
                    {
                        Player.CurrentPlayer.diamond -= weapon.priceDiamond;
                        Player.CurrentPlayer.weaponAUNameList.Add(weapon.uName);
                        Player.CurrentPlayer.SavePlayer(
                            task => {
                                if (task.IsFaulted || task.IsCanceled)
                                {
                                    Player.CurrentPlayer.diamond += weapon.priceDiamond;
                                    Player.CurrentPlayer.weaponAUNameList.Remove(weapon.uName);
                                    Debug.LogErrorFormat("BuyWeapon exception:{0}", task.Exception.Message);
                                }
                                else if (task.IsCompleted)
                                {
                                    BuyWeaponSuccessful();
                                }
                            }

                            );
                    }
                    else
                    {
                        Debug.Log("not enough diamand");
                    }
                }
                else if (weapon.priceGold > 0)
                {
                    if (Player.CurrentPlayer.gold >= weapon.priceGold)
                    {
                        Player.CurrentPlayer.gold -= weapon.priceGold;
                        Player.CurrentPlayer.weaponAUNameList.Add(weapon.uName);
                        Player.CurrentPlayer.SavePlayer(
                            task => {
                                if (task.IsFaulted || task.IsCanceled)
                                {
                                    Player.CurrentPlayer.gold += weapon.priceGold;
                                    Player.CurrentPlayer.weaponAUNameList.Remove(weapon.uName);
                                    Debug.LogErrorFormat("BuyWeapon exception:{0}", task.Exception.Message);
                                }
                                else if (task.IsCompleted)
                                {
                                    BuyWeaponSuccessful();
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
        private void BuyWeaponSuccessful()
        {
            pricePanel.SetActive(false);
            buyButtonObject.SetActive(false);
            selectButtonObject.SetActive(true);
            weaponLockImage.SetActive(false);
            selectWeaponBut.lockImageObject.SetActive(false);
            mainMenu.UpldatePlayerInfo();
        }
    }

}