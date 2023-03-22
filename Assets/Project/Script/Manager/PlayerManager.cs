using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDev
{
    public class PlayerManager : MonoBehaviour
    {
        public Camera mainCamera;
        public InputManager inputManager;
        public PlayerLocomotion locomotionManager;
        public PlayerCombatManager combatManager;
        public PlayerAnimatorManager animatorManager;
        public LockTarget lockTarget;

        [Header("Animator States")]
        public bool canDoCombo;
        public bool isAction;
        public bool isAiming;

        public bool canRotate = true;
        public bool canMove = true;
        public bool hasAxe = true;

        private void Awake()
        {
            mainCamera = Camera.main;
            inputManager = GetComponent<InputManager>();
            locomotionManager = GetComponent<PlayerLocomotion>();
            combatManager = GetComponent<PlayerCombatManager>();
            animatorManager = GetComponent<PlayerAnimatorManager>();
            lockTarget = GetComponent<LockTarget>();
        }
        private void LateUpdate()
        {
            isAction = animatorManager.baseAnimator.GetBool("IsAction");
            canDoCombo = animatorManager.baseAnimator.GetBool("CanDoCombo");
        }
    }
}
