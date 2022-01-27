using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.Purchasing;
#endif

namespace com.ahyim.planet
{
    public class TransactionLog
    {
        public string productionID;
        public string receipt;

#if UNITY_ANDROID || UNITY_IOS
        public Product TransactionProduct
        {
            set {
                productionID = value.definition.id;
                receipt = value.receipt;

            }
        }
#endif
        public string ToJSONString()
        {
            return JsonUtility.ToJson(this);
        }
    
    }

}