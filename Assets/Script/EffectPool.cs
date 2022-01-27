using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace com.ahyim.planet
{
    public class EffectPool : MonoBehaviour
    {
        public const string SPARKS = "Sparks";
        public const string BIG_EXPLOSION_EFFECT = "BigExplosionEffect";
        public const string FIRE_EFFECT = "FireEffect";
        public const string EFFECT_RESOURCE_PATH = "Prefabs/Effect/";

        public static EffectPool singleton;
        public Dictionary<string, Queue<ParticleSystem>> effectPool = new Dictionary<string, Queue<ParticleSystem>>();
        private Dictionary<string, GameObject> effectContainerDict = new Dictionary<string, GameObject>();
        private Dictionary<string, int> poolInitDict = new Dictionary<string, int>();

        private void Awake()
        {
            //define pool name and number of obj created initial.
            poolInitDict.Add(SPARKS, 15);
            poolInitDict.Add(BIG_EXPLOSION_EFFECT, 2);
            poolInitDict.Add(FIRE_EFFECT, 5);
        }

        // Use this for initialization
        void Start()
        {
            singleton = this;

#if UNITY_ANDROID || UNITY_IOS
            foreach( string prefabName in poolInitDict.Keys  )
            {
                effectPool.Add(prefabName, CreateEffectGameObjectList(prefabName, poolInitDict[prefabName]));
            }

            
            
#endif
        }

        // Update is called once per frame
        void Update()
        {

        }

        private Queue<ParticleSystem> CreateEffectGameObjectList(string prefabName, int poolSize)
        {
            Queue<ParticleSystem> gameObjectList = new Queue<ParticleSystem>();
            GameObject effectContainer = new GameObject(prefabName + "_Pool");
            effectContainer.transform.parent = transform;
            effectContainerDict.Add(prefabName, effectContainer);



            for (int index = 0; index < poolSize; index++)
            {
                ParticleSystem gObj = CraeteEffect(prefabName);
                gameObjectList.Enqueue(gObj);
            }

            return gameObjectList;
        }

        private ParticleSystem CraeteEffect(string prefabName)
        {
            GameObject effectObj = Resources.Load<GameObject>(EFFECT_RESOURCE_PATH + prefabName  );
            GameObject returnObj = Instantiate<GameObject>(effectObj);
            returnObj.SetActive(false);
            returnObj.transform.parent = effectContainerDict[prefabName].transform;
            return returnObj.GetComponent< ParticleSystem>();
        }

        public ParticleSystem GetEffect(string prefabName)
        {
            lock (effectPool)
            {
                Queue<ParticleSystem> gObjectQueue = effectPool[prefabName];
                if (gObjectQueue.Count > 0)
                {
                    return gObjectQueue.Dequeue();
                }
                else
                {
                    ParticleSystem particleSystem = CraeteEffect(prefabName);
                    particleSystem.gameObject.SetActive(false);
                    return particleSystem;
                }
            }
        }

        public void BackToPool(string prefabName, ParticleSystem particleSystem)
        {
            lock (effectPool)
            {
                particleSystem.gameObject.SetActive(false);
                if (!effectPool[prefabName].Contains(particleSystem))
                {
                    effectPool[prefabName].Enqueue(particleSystem);
                }
            }

        }

        /// <summary>
        /// Particle System will be recycled to pool after the during time
        /// </summary>
        /// <param name="prefabName"></param>
        /// <param name="particleSystem"></param>
        public void BackToPoolOnStop(string prefabName, ParticleSystem particleSystem)
        {
            StartCoroutine(EffectRecycle(prefabName, particleSystem));
        }



        private IEnumerator EffectRecycle(string prefabName, ParticleSystem particleSystem)
        {
            yield return new WaitForSeconds(particleSystem.main.duration);
            particleSystem.gameObject.SetActive(false);
            BackToPool(prefabName, particleSystem);
        }

    }

}