using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.Purchasing;
#endif

namespace com.ahyim.planet
{
#if UNITY_ANDROID || UNITY_IOS


    public class MyIAPManager : MonoBehaviour, IStoreListener
    {
        public static MyIAPManager singleton;

        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        // Product identifiers for all products capable of being purchased: 
        // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
        // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
        // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

        // General product identifiers for the consumable, non-consumable, and subscription products.
        // Use these handles in the code to reference which product to purchase. Also use these values 
        // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
        // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
        // specific mapping to Unity Purchasing's AddProduct, below.
        public static string kProductIDConsumable = "consumable";
        public static string kProductIDNonConsumable = "nonconsumable";
        public static string kProductIDSubscription = "subscription";
        public const string PID_1000_COIN = "1000_coin";
        public const string PID_5000_COIN = "5000_coins";
        public const string PID_20000_COIN = "20000_coins";
        public const string PID_50_DIAMOND = "50_diamonds";
        public const string PID_1000_DIAMOND = "1000_diamonds";
        public const string PID_10000_DIAMOND = "10000_diamonds";

        // Apple App Store-specific product identifier for the subscription product.
        private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

        // Google Play Store-specific product identifier subscription product.
        private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

        private System.Action<bool,string, PurchaseFailureReason> purchaseCallBack;

        void Start()
        {
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
                singleton = this;
            }
            
        }

        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            Debug.Log("StandardPurchasingModule.Instance() before");
            StandardPurchasingModule spModule = StandardPurchasingModule.Instance();
            Debug.Log("ConfigurationBuilder.Instance() before");
            var builder = ConfigurationBuilder.Instance(spModule);

            // Add a product to sell / restore by way of its identifier, associating the general identifier
            // with its store-specific identifiers.

            //builder.AddProduct(kProductIDConsumable, ProductType.Consumable);

            // Continue adding the non-consumable product.

            //builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);

            // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
            // if the Product ID was configured differently between Apple and Google stores. Also note that
            // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
            // must only be referenced here. 

            //builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
            //    { kProductNameAppleSubscription, AppleAppStore.Name },
            //    { kProductNameGooglePlaySubscription, GooglePlay.Name },
            //});
            Debug.Log("builder.AddProduct() before");
            builder.AddProduct(PID_1000_COIN, ProductType.Consumable);
            builder.AddProduct(PID_5000_COIN, ProductType.Consumable);
            builder.AddProduct(PID_20000_COIN, ProductType.Consumable);
            builder.AddProduct(PID_50_DIAMOND, ProductType.Consumable);
            builder.AddProduct(PID_1000_DIAMOND, ProductType.Consumable);
            builder.AddProduct(PID_10000_DIAMOND, ProductType.Consumable);


            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            Debug.Log("UnityPurchasing.Initialize before");
            UnityPurchasing.Initialize(this, builder);
            Debug.Log("UnityPurchasing.Initialize after");
            //foreach ( Product product in  m_StoreController.products.all)
            //{
            //    Debug.LogFormat("product id:{0}", product.definition.id);
            //}
        }


        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }


        public void BuyConsumable()
        {
            // Buy the consumable product using its general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            BuyProductID(kProductIDConsumable);
        }


        public void BuyNonConsumable()
        {
            // Buy the non-consumable product using its general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            BuyProductID(kProductIDNonConsumable);
        }


        public void BuySubscription()
        {
            // Buy the subscription product using its the general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            // Notice how we use the general product identifier in spite of this ID being mapped to
            // custom store-specific identifiers above.
            BuyProductID(kProductIDSubscription);
        }

        

        


        void BuyProductID(string productId)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        public Product GetProduct(string productID)
        {
            if (IsInitialized())
            {
                return m_StoreController.products.WithID(productID);

            }
            else
            {
                Debug.Log("GetProduct FAIL. Not initialized.");
                return null;
            }
        }

        public string GetLocalizedPriceString(string productId)
        {
            if (IsInitialized())
            {
                Product product = m_StoreController.products.WithID(productId);

                if(product != null)
                {
                    return product.metadata.localizedPriceString;
                }


            }
            else
            {
                Debug.Log("GetLocalizedPriceString FAIL. Not initialized.");
            }
            return null;
        }



        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) => {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }


        //  
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            // A consumable product has been purchased by this user.
            if (string.Equals(args.purchasedProduct.definition.id, kProductIDConsumable, System.StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
                //ScoreManager.score += 100;
            }
            // Or ... a non-consumable product has been purchased by this user.
            else if (string.Equals(args.purchasedProduct.definition.id, kProductIDNonConsumable, System.StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
            }
            // Or ... a subscription product has been purchased by this user.
            else if (string.Equals(args.purchasedProduct.definition.id, kProductIDSubscription, System.StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // TODO: The subscription item has been successfully purchased, grant this to the player.
            }
            else if (string.Equals(args.purchasedProduct.definition.id, PID_1000_COIN, System.StringComparison.Ordinal )  )
            {
                Player.CurrentPlayer.gold += 1000;
                Player.CurrentPlayer.SavePlayer(
                    (
                    task =>
                    {
                        string callbackMessage = null;
                        if (task.IsCanceled || task.IsFaulted)
                        {
                            Debug.LogFormat("{0} added gold 1000 fault", Player.CurrentPlayer.name);
                            Debug.LogException(task.Exception);
                            callbackMessage = "Save player fail";
                        }
                        else if (task.IsCompleted)
                        {
                            Debug.LogFormat("{0} added gold 1000", Player.CurrentPlayer.name);
                            
                        }
                        if (purchaseCallBack != null)
                        {
                            purchaseCallBack(true, callbackMessage , PurchaseFailureReason.Unknown);
                        }

                        
                    }
                    
                    )
                    );
            }
            // Or ... an unknown product has been purchased by this user. Fill in additional products here....
            else
            {
                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            }

            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed. 
            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
            purchaseCallBack( false, null , failureReason);
        }
    }
#else
     public class MyIAPManager : MonoBehaviour
    {
        public static MyIAPManager singleton;
    

        
    }
#endif
}

