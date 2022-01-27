using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.ahyim.planet
{
    public class WeaponAControl : NetworkBehaviour
    {

        public GameObject WeaponA;
        public GameObject WeaponAContainer;
        [SyncVar]
        public string bulletPrefabName;
        [SyncVar]
        public string effectPrefabName;
        [SyncVar]
        public int bulletDamage;
        [SyncVar]
        public float reloadTime;
        public Transform launcher;
        
       

        private float lastFireTime = 0f;
        private List<GameObject> nearByPlayer = new List<GameObject>();
        
         
        private Vector3 testFirePosition;

        private int fireCount = 0;

        private NetPlayerManager netPlayerManager;

        private string bulletEffectPrefabName;
        private CarUserControl carUserControl;

        // Use this for initialization
        void Start()
        {
            netPlayerManager = GetComponent<NetPlayerManager>();
            carUserControl = GetComponent<CarUserControl>();
            testFirePosition = transform.TransformPoint(new Vector3(0, 0, 6));

            if (isLocalPlayer)
            {
                bulletEffectPrefabName = Player.CurrentPlayer.CurrWeaponA.bulletEffectPrefeb;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            if (isLocalPlayer || (isServer && carUserControl.IsAIPlayer))
            {
                RotateWeaponAToNearestPlayer();
                
                //Fire(testFirePosition);
            }
        }

       
        

        private void RotateWeaponAToNearestPlayer()
        {
            GameObject targetPlayer = netPlayerManager.targetPlayer;

            Transform weaponATransform = WeaponA.transform;
            float localYAngle = weaponATransform.localEulerAngles.y;
            if (targetPlayer != null)
            {
                Vector3 nearestPostion = WeaponAContainer.transform.InverseTransformPoint(targetPlayer.transform.position);
                //Debug.LogFormat("NearestPlayer-x:{0},y:{1},z{2}", nearestPostion.x, nearestPostion.y, nearestPostion.z);
                float degree = Mathf.Atan2(nearestPostion.x, nearestPostion.z) * Mathf.Rad2Deg;
                //Debug.LogFormat("Atan2 of {0}/{1} = {2}", nearestPostion.x, nearestPostion.z, degree);
                //Quaternion toRotation = Quaternion.FromToRotation(weaponATransform.up, nearestPlayer.transform.position);
                weaponATransform.localEulerAngles = new Vector3(0, Mathf.LerpAngle(localYAngle, degree, 5 * Time.deltaTime), 0);
                //Debug.Log(toRotation);
                //weaponATransform.rotation = toRotation;// Quaternion.Slerp(transform.rotation, toRotation, 50 * Time.delta

                //weaponATransform.transform.Rotate(0, toRotation.eulerAngles.y,0, Space.Self);
                float angleDiff = Mathf.Abs(localYAngle - degree);
                if (angleDiff < 1.0f || angleDiff > 359.0f)
                {
                    Fire(targetPlayer.transform.position);
                }
                
            }
            else
            {
                weaponATransform.localEulerAngles = new Vector3(0, Mathf.LerpAngle(localYAngle, 0, 5 * Time.deltaTime), 0);
            }

        }
        public void Fire(Vector3 position)
        {
            float thisTime = Time.time;

            if ( isLocalPlayer && thisTime - lastFireTime > reloadTime)
            {
                
                
                    //Debug.LogFormat("Invoke CmdFire, thisTime:{0}, lastFireTime:{1}, launcher pos:{2}, target pos:{3}", thisTime, lastFireTime, launcher.position, position);
                    lastFireTime = thisTime;
                    CmdFire(bulletPrefabName, launcher.position, position, bulletDamage, bulletEffectPrefabName);

                
            }
            else if (isServer && carUserControl.IsAIPlayer && thisTime - lastFireTime > reloadTime)
            {
                Debug.LogFormat("{0} Fire", netPlayerManager.playerName);
                lastFireTime = thisTime;
                GameObject bulletObj = BulletPool.singleton.GetBullet(bulletPrefabName);
                Bullet bullet = bulletObj.GetComponent<Bullet>();
                bullet.Fire(bulletDamage, launcher.position, position, netPlayerManager.AIConnectId, bulletEffectPrefabName);
            }
            

            
        }
        [Command]
        void CmdFire(string prefebName, Vector3 launcherPosition, Vector3 targetPosition, int damage, string bulletEffectPrefabName)
        {
            GameObject bulletObj = BulletPool.singleton.GetBullet(prefebName);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.Fire(damage, launcherPosition, targetPosition, connectionToClient.connectionId, bulletEffectPrefabName);
            //Debug.LogFormat("Server fireCount:{0}, Time:{1}, launcherPosition:{2}, targetPosition:{3}, damage:{4}, firePlayerUid:{5}, connectionToClient.connectionId{6}", fireCount++, Time.time, launcherPosition, targetPosition, damage, firePlayerUid, connectionToClient.connectionId);
            RpcFireEffect();


        }
        [ClientRpc]
        void RpcFireEffect()
        {
            ParticleSystem fireEffect = EffectPool.singleton.GetEffect(EffectPool.FIRE_EFFECT);
            fireEffect.transform.parent = launcher;
            fireEffect.transform.localPosition = Vector3.zero;
            fireEffect.transform.localRotation = Quaternion.identity;
            fireEffect.Play();
            EffectPool.singleton.BackToPoolOnStop(EffectPool.FIRE_EFFECT, fireEffect);


        }

        

    }

}