using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;


namespace com.ahyim.planet
{

    public class Health : NetworkBehaviour
    {
        public Slider healthBar;

        public const int maxHealth = 1000;

        public Transform playerCanvasTransform;

        //[SyncVar]
        public NetworkVariable<int> amno;
        //[SyncVar(hook = "OnChnageHealth")]
        public NetworkVariable<int> currentHealth = new NetworkVariable<int>(maxHealth);


        private string objName = "Test";

        private HealthUIBar healthUIBar;

        private NetPlayerManager netPlayerManager;
        private CarUserControl carUserControl;

        private bool isKilled = false;

        private GameObjPool gameObjPool;

        //public void OnNetworkSpawn()
        //{
        //    currentHealth.Value = maxHealth;
        //}

        // Use this for initialization
        void Start()
        {
            netPlayerManager = GetComponent<NetPlayerManager>();
            carUserControl = GetComponent<CarUserControl>();
            gameObjPool = GameObjPool.singleton;

            healthBar.value = 1f;
            if (IsLocalPlayer)
            {
                
                healthUIBar = HealthUIBar.singleton;
               if (!healthUIBar)
                {
                    Debug.LogError("healthUIBar not found, please add back to the sence");
                }
                healthBar.gameObject.SetActive(false);
            }
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEable()
        {
            currentHealth.OnValueChanged += OnHealthChanged;
        }

        void OnHealthChanged(int oldValue,int newValue)
        {
            int damage = oldValue - newValue;

            //this.currentHealth = newValue;
            float healthBarRatio = (float)newValue / (float) maxHealth;
            healthBar.value = healthBarRatio;
            if (IsLocalPlayer)
            {
                healthUIBar.UpdateHealthBar(healthBarRatio);
                //Debug.LogFormat("currentHealth:{0}", currentHealth.Value);
            }


            GameObject popUpDamage = gameObjPool.GetObject(GameObjPool.POPUP_DAMAGE);
            popUpDamage.transform.parent = playerCanvasTransform;
            popUpDamage.transform.localPosition = Vector3.zero;
            popUpDamage.transform.localRotation = Quaternion.identity;
            GetComponentInChildren<TMPro.TextMeshProUGUI>().text = string.Format("{0}", damage);
            Animation popUpAnimation = popUpDamage.GetComponent<Animation>();
            popUpAnimation.Play();

            StartCoroutine(PopUpAnimationDelayBackToPool(popUpDamage));
            
        }

        private IEnumerator PopUpAnimationDelayBackToPool(GameObject popUpDamage)
        {
            Animation popUpAnimation = popUpDamage.GetComponent<Animation>();
            yield return new WaitForSeconds(popUpAnimation.clip.length);
            popUpAnimation.Stop();
            gameObjPool.BackToPool(GameObjPool.POPUP_DAMAGE, popUpDamage);
        }

        
        public void Aid(int amount)
        {
            if (IsServer)
            {
                currentHealth.Value += amount;
                if (currentHealth.Value > maxHealth)
                {
                    currentHealth.Value = maxHealth;
                }
            }
        }

        public void TakeDamage(int amount, int playerConnectionId, Vector3 hitPosition, Vector3 hitDirection)
        {
            if(!IsServer)
            {
                return;
            }
            currentHealth.Value -= amount;
            
            if (carUserControl.IsAIPlayer)
            {
                OnHealthChanged(currentHealth.Value + amount, currentHealth.Value);
                HitImpact(hitPosition, hitDirection);
            }
            else
            {
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { (ulong)playerConnectionId }
                    }
                };
                TargetHitImpactClientRpc( hitPosition, hitDirection, clientRpcParams);
            }
            if (currentHealth.Value <= 0 && !isKilled)
            {
                isKilled = true;
                //TargetKilled(connectionToClient, 100);
                RpcKilledClientRpc( transform.position);
                NetworkObject.Despawn(true);
                //NetworkServer.UnSpawn(this.gameObject);
                //NetworkServer.Destroy(this.gameObject);
                int score = netPlayerManager.bcar.score;
                ScoreListManager.AddPlayerScore(playerConnectionId, score);
                using FastBufferWriter writer = new FastBufferWriter();
                ScoreListManager.ScoreListToWriter(writer);
                CustomMessagingManager.SendNamedMessageToAll("abc",writer );
                    
                NetworkServer.SendToAll(MessageTypes.MSG_SCORE_LIST, ScoreListManager.ScoreListMessage);
                if (playerConnectionId >= 0)
                {
                    NetworkServer.SendToClient(playerConnectionId, MessageTypes.MSG_SCORE, new IntegerMessage(score));
                }
                if (carUserControl.IsAIPlayer)
                {
                    ScoreListManager.RemovePlayer(netPlayerManager.AIConnectId);
                    netPlayerManager.RebrithAiPlayer();
                    NetworkObject.Despawn(gameObject);
                    //NetworkServer.Destroy(gameObject);
                }
                else
                {
                    
                    ScoreListManager.RemovePlayer(connectionToClient.connectionId);
                    
                    StartCoroutine(NetworkDestoryDelay(2.0f));

                }
                
                

                


                //Debug.LogFormat("SendToAll ScoreListMessage:{0}", ScoreListManager.ScoreListMessage);

                //Debug.LogFormat("SendToClient playerConnectionId:{0}, score:{1}", playerConnectionId, score);
            }
            
        }

        private IEnumerator NetworkDestoryDelay(float time)
        {
            yield return new WaitForSeconds(time);
            NetworkServer.Destroy(gameObject);
        }
        //[TargetRpc]
        //private void TargetKilled(NetworkConnection target, int score)
        //{
        //    Debug.LogFormat("killed, Score: {0}", score);
        //    NetworkGameController.singleton.StopClient();
        //    //Destroy(NetworkGameController.singleton.gameObject);
        //    //NetworkManager.Shutdown();
        //    //SceneManager.UnloadSceneAsync("Planet");
        //    MySceneManager.LoadSceneStatic("MainMenu2");
        //}
        [ClientRpc]
        private void RpcKilledClientRpc(Vector3 position)
        {
            ParticleSystem explosion = EffectPool.singleton.GetEffect(EffectPool.BIG_EXPLOSION_EFFECT);
            AudioSource audioSource = explosion.GetComponent<AudioSource>();
            explosion.transform.position = transform.position;
            explosion.transform.rotation = transform.rotation;
            explosion.gameObject.SetActive(true);
            explosion.Play();
            audioSource.Play();
            EffectPool.singleton.BackToPoolOnStop(EffectPool.BIG_EXPLOSION_EFFECT, explosion);
            if( IsLocalPlayer)
            {
                GameMenu.singleton.ShowGameOverPanel(explosion.main.duration);
                GameMenu.singleton.udpateGameOverScore(NetworkGameController.singleton.score);
                NetworkGameController.singleton.SavePlayer();
                
            }

        }
        [ClientRpc]
        private void TargetHitImpactClientRpc( Vector3 hitPosition, Vector3 hitDirection, ClientRpcParams clientRpcParams = default)
        {
            HitImpact(hitPosition, hitDirection);
        }

        private void HitImpact(Vector3 hitPosition, Vector3 hitDirection)
        {
            GetComponent<Rigidbody>().AddForceAtPosition(hitDirection * 10, hitPosition, ForceMode.Impulse);
        }




    }
}

