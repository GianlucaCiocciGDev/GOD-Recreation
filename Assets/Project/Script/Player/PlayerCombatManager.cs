using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using UnityEngine.XR;

namespace GDev
{
    public class PlayerCombatManager : CombatManager
    {
        Rig aimRig;

        [Header("Attack Settings")]
        private string OH_LIGHT_ATTACK_01 = "Attack1";
        private string OH_LIGHT_ATTACK_02 = "Attack2";
        private string OH_LIGHT_ATTACK_03 = "Attack3";
        private string lastAttack;

        [Header("current Weapon Settings")]
        public GameObject currentWeapon;
        AxeManager axeManager;
        private Vector3 originalLocalPosition;
        private Vector3 originalLocalRotation;
        private Vector3 pullPosition;
        bool pulling;
        private float returnTime;
        public Transform curvePoint;
        public Transform weaponHolder;
        [SerializeField] CinemachineVirtualCamera aimCamera;
        [SerializeField] Image reticle;
        [SerializeField] Transform aimSourceObject;

        [SerializeField] CinemachineImpulseSource impulse;
        
        protected override void Start()
        {
            base.Start();
            axeManager = currentWeapon.GetComponent<AxeManager>();
            originalLocalPosition = currentWeapon.transform.localPosition;
            originalLocalRotation = currentWeapon.transform.localEulerAngles;

            aimRig = GetComponentInChildren<Rig>();
            aimRig.weight = 0.0f;
        }
        private void Update()
        {
            if (playerManager.isAiming)
            {
                Vector3 mouseWorldPosition = Vector3.zero;
                Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
                Ray ray = playerManager.mainCamera.ScreenPointToRay(screenCenterPoint);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, float.MaxValue))
                {
                    if (hit.transform.root.gameObject == this.transform.gameObject)
                        return;

                    mouseWorldPosition = hit.point;
                    if (hit.transform?.gameObject?.layer == 8)
                    {
                        reticle.color = Color.red;
                    }
                }
                else
                {
                    mouseWorldPosition = ray.origin + ray.direction * 900f;
                    reticle.color = Color.white;
                }

                aimSourceObject.position = mouseWorldPosition;
                Vector3 aimDirection = (aimSourceObject.position - transform.position).normalized;
                aimDirection.y = 0.0f;
                playerManager.locomotionManager.HandleAimRotation(transform.rotation, Quaternion.LookRotation(aimDirection));
            }

            if (pulling && !playerManager.hasAxe)
            {
                if (returnTime < 1)
                {
                    if (returnTime > .9f && returnTime < .91f)
                    {
                        playerManager.animatorManager.PlayTargetAnimation("Grab");
                        axeManager.PlayWeaponFX();
                    }
                    currentWeapon.transform.position = GetQuadraticCurvePoint(returnTime, pullPosition, curvePoint.position, weaponHolder.position);
                    returnTime += Time.deltaTime * 1.5f;
                }
                else
                {
                    CatchAxe();
                }
            }
        }

        #region Implemet class
        public override void HandleAttack()
        {
            if (playerManager.isAiming)
            {
                HandleThrowAttack();
            }
            else
            {
                if (!playerManager.hasAxe)
                    PullAxe();
                else
                {
                    playerManager.animatorManager.SetBoolState("IsAction", true);
                    //await Task.Run(() => Wait());
                    if (playerManager.canDoCombo)
                    {
                        HandleComboAttack();
                    }
                    else
                        HandleLightAttack();
                }
            }
        }
        public override void HandleAiming(bool isAIming)
        {
            playerManager.canRotate = playerManager.canMove = !isAIming;

            playerManager.animatorManager.SetBoolState("IsAiming", isAIming);
            playerManager.isAiming = isAIming;
            reticle.gameObject.SetActive(isAIming ? true : false);
            aimCamera.gameObject.SetActive(isAIming ? true : false);
            aimRig.weight = isAIming ? 1.0f : 0.0f;
        }
        protected override void HandleComboAttack()
        {
            playerManager.animatorManager.SetBoolState("CanDoCombo", false);
            if (lastAttack == OH_LIGHT_ATTACK_01)
            {
                playerManager.animatorManager.PlayTargetAnimation(OH_LIGHT_ATTACK_02);
                lastAttack = OH_LIGHT_ATTACK_02;
            }
            else if (lastAttack == OH_LIGHT_ATTACK_02)
            {
                playerManager.animatorManager.PlayTargetAnimation(OH_LIGHT_ATTACK_03);
                lastAttack = OH_LIGHT_ATTACK_03;
            }
            else
            {
                playerManager.animatorManager.PlayTargetAnimation(OH_LIGHT_ATTACK_01);
                lastAttack = OH_LIGHT_ATTACK_01;
            }
        }
        protected override void HandleLightAttack()
        {
            if (playerManager.isAction)
                return;

            if (playerManager.canDoCombo)
                return;

            playerManager.animatorManager.PlayTargetAnimation(OH_LIGHT_ATTACK_01);
            lastAttack = OH_LIGHT_ATTACK_01;

            axeManager.PlayWeaponFX();
        }
        protected override void HandleThrowAttack()
        {
            if (!playerManager.hasAxe)
                return;
            axeManager.PlayWeaponFX();
            playerManager.animatorManager.PlayTargetAnimation("Throw");
            HandleAiming(false);
        }
        #endregion

        #region Axe Manage
        public void ThrowAxe()
        {
            Rigidbody weaponRb = currentWeapon.GetComponent<Rigidbody>();
            axeManager.active = true;
            weaponRb.isKinematic = false;
            weaponRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            currentWeapon.transform.SetParent(null);
            currentWeapon.transform.eulerAngles = new Vector3(0, -90 + transform.eulerAngles.y, 0);
            currentWeapon.transform.transform.position += transform.right / 5;
            weaponRb.AddForce(playerManager.mainCamera.transform.forward * 30 + transform.up * 2, ForceMode.Impulse);
            playerManager.hasAxe = false;
            axeManager.OpenCollider();
        }
        public void EnableAxeRaycast()
        {
            axeManager.OpenRaycast();
        }
        public void DisableAxeRaycast()
        {
            axeManager.CloseRaycast();
        }
        public void PullAxe()
        {
            axeManager.CloseCollider();
            pullPosition = currentWeapon.transform.position;
            Rigidbody weaponRb = currentWeapon.GetComponent<Rigidbody>();
            weaponRb.Sleep();
            weaponRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            weaponRb.isKinematic = true;
            currentWeapon.transform.eulerAngles = new Vector3(90, 90, 0);
            axeManager.active = true;
            pulling = true;
            playerManager.animatorManager.SetBoolState("IsPulling", pulling);
        }
        private void CatchAxe()
        {
            if (impulse)
                impulse.GenerateImpulse(Vector3.right);
            axeManager.PlayWeaponFXCatch();
            returnTime = 0;
            pulling = false;
            currentWeapon.transform.parent = weaponHolder;
            axeManager.active = false;
            currentWeapon.transform.localEulerAngles = originalLocalRotation;
            currentWeapon.transform.localPosition = originalLocalPosition;
            playerManager.hasAxe = true;
            playerManager.animatorManager.SetBoolState("IsPulling", pulling);
        }
        #endregion

        #region Utils
        public Vector3 GetQuadraticCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            return (uu * p0) + (2 * u * t * p1) + (tt * p2);
        }
        #endregion
    }
}
