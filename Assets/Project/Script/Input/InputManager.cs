using UnityEngine;
using UnityEngine.InputSystem;

namespace GDev
{
    public class InputManager : MonoBehaviour
    {
        MapInput inputMap;
        PlayerManager playerManager;

        public Vector2 movementAxis;
        public Vector2 lookAxis;

        public bool holdingRun;
        public bool holdingAim;
        public bool fireTrigger;

        private void Awake()
        {
            inputMap = new MapInput();

            inputMap.Player.Look.started += OnLook;
            inputMap.Player.Look.performed += OnLook;
            inputMap.Player.Look.canceled += OnLook;

            inputMap.Player.Movement.started += OnMove;
            inputMap.Player.Movement.performed += OnMove;
            inputMap.Player.Movement.canceled += OnMove;

            inputMap.Player.Run.started += OnRun;
            inputMap.Player.Movement.performed += OnRun;
            inputMap.Player.Run.canceled += OnRun;

            inputMap.Player.Aim.started += OnAim;
            inputMap.Player.Aim.canceled += OnAim;

            inputMap.Player.Attack.started += OnAttack;
            inputMap.Player.Attack.canceled += OnAttack;

            inputMap.Player.LockOnTarget.started += OnLockOnTarget;
            inputMap.Player.LockOnTarget.canceled += OnLockOnTarget;

            inputMap.Player.Quit.started += OnQuitGame;
            ToggleMouseCursor(false);

        }
        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();
        }

        #region Custom Input Events
        private void OnMove(InputAction.CallbackContext context)
        {
            movementAxis = context.ReadValue<Vector2>();
        }
        public void OnLook(InputAction.CallbackContext context)
        {
            lookAxis = context.ReadValue<Vector2>();
        }
        public void OnAttack(InputAction.CallbackContext context)
        {
            switch (context)
            {
                case { phase: InputActionPhase.Started }:
                    fireTrigger = true;
                    playerManager.combatManager.HandleAttack();
                    break;
                case { phase: InputActionPhase.Performed }:
                    break;
                case { phase: InputActionPhase.Canceled }:
                    fireTrigger = false;
                    break;
            }
        }
        public void OnAim(InputAction.CallbackContext context)
        {
            if (!playerManager.hasAxe)
                return;
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    holdingAim = true;
                    playerManager.combatManager.HandleAiming(true);
                    break;
                case InputActionPhase.Canceled:
                    playerManager.combatManager.HandleAiming(false);
                    holdingAim = false;
                    break;
            }
        }
        public void OnRun(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    holdingRun = true;
                    break;
                case InputActionPhase.Canceled:
                    holdingRun = false;
                    break;
            }
        }
        private void OnLockOnTarget(InputAction.CallbackContext context)
        {
            switch (context)
            {
                case { phase: InputActionPhase.Started }:
                    if (!playerManager.lockTarget.isTargeting)
                        playerManager.lockTarget.AssignTarget();
                    else
                        playerManager.lockTarget.ResetTarget();
                    break;
                case { phase: InputActionPhase.Performed }:
                    break;
                case { phase: InputActionPhase.Canceled }:
                    break;
            }
        }
        public void OnQuitGame(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Application.Quit();break;
            }
        }
        #endregion

        #region Utility Methods
        private void ToggleMouseCursor(bool value)
        {
            Cursor.visible = value;
            Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        }
        #endregion

        private void OnEnable()
        {
            inputMap.Enable();
        }
        private void OnDisable()
        {
            inputMap.Enable();
        }
    }
}
