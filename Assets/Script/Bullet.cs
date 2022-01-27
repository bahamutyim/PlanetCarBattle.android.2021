using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.ahyim.planet
{
    
    public class Bullet : NetworkBehaviour
    {

        public Rigidbody rb;
        private int damage;
        public string prefabName;
        public GameObject bulletObject;
        public string effectPrefabName;
        
        private EffectPool effectPool;
        private int playerConnectionId;
        private float speed = 100f;
        private MeshRenderer meshRenderer;
        private TrailRenderer trailRenderer;
        private NetworkTransform networkTransform;

        // Use this for initialization
        void Start()
        {
            //rb = GetComponent<Rigidbody>();
            effectPrefabName = "Sparks";
            effectPool = EffectPool.singleton;
            if (!effectPool)
            {
                Debug.LogError("EffectPool not found, please add back to the sence");
            }
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();
            networkTransform = GetComponent<NetworkTransform>();
        }
         
        public void Fire(int damage, Vector3 startPosition, Vector3 targetPosition, int playerConnectionId, string effectPrefabName)
        {
            this.damage = damage;
            this.playerConnectionId = playerConnectionId;
            this.effectPrefabName = effectPrefabName;
            transform.position = startPosition;
            Vector3 direction = targetPosition - startPosition;
            transform.rotation = Quaternion.LookRotation(direction);
            rb.velocity = direction.normalized * speed;
            //force update
            GetComponent<NetworkTransform>().SetDirtyBit(1U);
            
            StartCoroutine(UnSpawn(3.0f));

        }

        //public override bool OnSerialize(NetworkWriter writer, bool initialState)
        //{
        //    Debug.LogFormat("Bullet OnSerialize position:{0}, velocity:{1}", transform.position, rb.velocity);
        //    return base.OnSerialize(writer, initialState);
        //}


        //public override void OnDeserialize(NetworkReader reader, bool initialState)
        //{
        //    Debug.LogFormat("Bullet OnSerialize position:{0}, velocity:{1}", transform.position, rb.velocity);
        //    base.OnDeserialize(reader, initialState);
            
        //}

        public IEnumerator UnSpawn(float time)
        {
            yield return new WaitForSeconds(time);
            Recall();

        }

        private void Recall()
        {
            
                //gameObject.SetActive(false);
                //NetworkServer.UnSpawn(gameObject);
                BulletPool.singleton.BackToPool(prefabName, gameObject);
            
        }
        [Command]
        public void CmdRecall()
        {
            Recall();
        }


        //private void OnCollisionEnter(Collision collision)
        //{


        //    if (!isServer)
        //    {
        //        Debug.LogFormat("Client Bullet Collision on {0}", collision.gameObject.name);
        //        bulletObject.SetActive(false);
        //        effect.Play();
        //    }
        //    else
        //    {
        //        Debug.LogFormat("Server Bullet Collision on {0}", collision.gameObject.name);
        //        CmdRecall();
        //        //RpcCollisionEffect();
        //    }

        //}

        private void OnTriggerEnter(Collider other)
        {
            string colliderName = other.gameObject.name;
            if (!isServer)
            {
                if (colliderName != "WeaponACollider" && other.gameObject.tag != "bullet")
                {
                    //Debug.LogFormat("Client Bullet Trigger on {0}", other.gameObject.name);
                    
                    transform.position = Vector3.zero;
                    rb.velocity = Vector3.zero;
                    
                    //Debug.LogFormat("Bullet OnTriggerEnter");
                    //gameObject.SetActive(false);
                }

            }
            else
            {
                if (colliderName == "CarCollider")
                {
                    
                    other.GetComponentInParent<Health>().TakeDamage(damage, playerConnectionId, transform.position, transform.TransformVector(Vector3.forward));
                    RpcCollisionEffect(transform.position, transform.rotation);
                    
                    
                    //Debug.LogFormat("Health TakeDamage:{0}", damage);
                }
            }

        }

        [ClientRpc(channel = 1)]
        private void RpcCollisionEffect(Vector3 position, Quaternion rotation)
        {
            CollisionEffect(position, rotation);
        } 



        private void CollisionEffect(Vector3 position, Quaternion rotation)
        {
            ParticleSystem particleSystem = effectPool.GetEffect(effectPrefabName);
            particleSystem.gameObject.transform.position = position;
            particleSystem.gameObject.transform.rotation = rotation;
            particleSystem.gameObject.SetActive(true);
            particleSystem.Play();

            effectPool.BackToPoolOnStop(effectPrefabName, particleSystem);


        }
        
        
    }
}
