using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.Purchasing;
#endif

namespace com.ahyim.planet
{
    public class IAPItem : MonoBehaviour
    {
        public string productID;
        public Text priceText;

#if UNITY_ANDROID || UNITY_IOS
        private Product iapProduct;

        // Use this for initialization
        void Start()
        {
            
        }

        
        private void OnEnable()
        {
            Debug.Log("IAPItem OnEnable");
            iapProduct = MyIAPManager.singleton.GetProduct(productID);
            if (iapProduct != null)
            {
                priceText.text = iapProduct.metadata.localizedPriceString;
            }
            else
            {
                Debug.Log("iapProduct is null");
            }
        }


        // Update is called once per frame
        void Update()
        {

        }
#endif
    }

}