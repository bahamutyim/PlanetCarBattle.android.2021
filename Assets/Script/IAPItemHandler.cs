using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.Purchasing;
#endif

namespace com.ahyim.planet
{


    public class IAPItemHandler : MonoBehaviour
    {

        private MainMenu mainMenu;

        // Use this for initialization
        void Start()
        {
            mainMenu = GetComponent<MainMenu>();
        }

        // Update is called once per frame
        void Update()
        {

        }
#if UNITY_ANDROID || UNITY_IOS
        public void Fulfill(Product product)
        {

            if (product != null)
            {
                switch (product.definition.id)
                {
                    case MyIAPManager.PID_1000_COIN:
                        Debug.Log("You Got 1000 Coin!");
                        Player.CurrentPlayer.gold += 1000;
                        
                        break;
                    case MyIAPManager.PID_5000_COIN:
                        Debug.Log("You Got 5000 Coin!");
                        Player.CurrentPlayer.gold += 5000;

                        break;
                    case MyIAPManager.PID_20000_COIN:
                        Debug.Log("You Got 20000 Coin!");
                        Player.CurrentPlayer.gold += 20000;

                        break;
                    case MyIAPManager.PID_50_DIAMOND:
                        Debug.Log("You Got 50 Diamonds!");
                        Player.CurrentPlayer.diamond += 50;

                        break;
                    case MyIAPManager.PID_1000_DIAMOND:
                        Debug.Log("You Got 1000 Diamonds!");
                        Player.CurrentPlayer.diamond += 1000;

                        break;
                    case MyIAPManager.PID_10000_DIAMOND:
                        Debug.Log("You Got 10000 Diamonds!");
                        Player.CurrentPlayer.diamond += 10000;

                        break;
                    default:
                        Debug.Log(
                        string.Format("Unrecognized productId \"{0}\"", product.definition.id)
                            );
                        break; 
                }
                mainMenu.UpldatePlayerInfo();

#if UNITY_EDITOR == false
                    SavePlayer(product);
#else
                mainMenu.ShowMessage("Thank you for puchasing.");
#endif






            }
        }

        public void SavePlayer(Product product)
        {
            Player.CurrentPlayer.SavePlayer(
                            task =>
                            {
                                if (task.IsFaulted || task.IsCanceled)
                                {
                                    Debug.Log("Player cannot been saved");
                                    Debug.LogException(task.Exception);
                                    mainMenu.ShowMessage("Player infomation cannot been saved, please click retry.", "Retry",
                                        () =>
                                        {
                                            SavePlayer(product);
                                        }


                                        );
                                }
                                else if (task.IsCompleted)
                                {
                                    Debug.Log("Player is saved");
                                    SaveTransaction(product);
                                }
                            }

                            );
        }
        public void SaveTransaction(Product product)
        {
            TransactionLog transactionLog = new TransactionLog();
            transactionLog.TransactionProduct = product;
            FireBaseDatabase.WriteTransactionLog(Player.CurrentPlayer, transactionLog,
                    task =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            Debug.Log("transaction log cannot be saved");
                            Debug.LogException(task.Exception);
                            mainMenu.ShowMessage("Player infomation cannot been saved, please click retry.", "Retry",
                                        () =>
                                        {
                                            SaveTransaction(product);
                                        }
                                        );
                        }
                        else if (task.IsCompleted)
                        {
                            Debug.Log("transaction log is saved");
                            mainMenu.ShowMessage("Thank you for puchasing.");
                        }
                    }

                );
        }

        public void PurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            Debug.LogFormat("product id {0} purchase fail, reason {1}", product.definition.id, reason.ToString());
            if ( product != null)
            {
                switch (reason)
                {
                    case PurchaseFailureReason.DuplicateTransaction:
                        mainMenu.ShowMessage("{0} have duplicate transaction", product.metadata.localizedTitle);

                        break;
                    case PurchaseFailureReason.ExistingPurchasePending:
                        mainMenu.ShowMessage("{0} is existing purchase pending", product.metadata.localizedTitle);
                        break;
                    case PurchaseFailureReason.PaymentDeclined:
                        mainMenu.ShowMessage("Payment declined for {0}", product.metadata.localizedTitle);
                        break;
                    case PurchaseFailureReason.ProductUnavailable:
                        mainMenu.ShowMessage("{0} is unavailable", product.metadata.localizedTitle);
                        break;
                    case PurchaseFailureReason.PurchasingUnavailable:
                        mainMenu.ShowMessage("{0} purchasing unavailable", product.metadata.localizedTitle);
                        break;
                    case PurchaseFailureReason.SignatureInvalid:
                        mainMenu.ShowMessage("Signature Invalid");
                        break;
                    case PurchaseFailureReason.Unknown:
                        mainMenu.ShowMessage("Unkown issue");
                        break;
                    case PurchaseFailureReason.UserCancelled:
                        mainMenu.ShowMessage("User canacelled transaction");
                        break;

                    default:
                        break;
                }



            }

        }
#endif
    }
 
}
