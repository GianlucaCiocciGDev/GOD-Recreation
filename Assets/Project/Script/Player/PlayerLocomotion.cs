using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace GDev
{
    public class PlayerLocomotion : LocomotionManager
    {
        CharacterController characterController;

        [Header("Movement Settings")]
        [SerializeField] float walkSpeed = 2.0f;
        [SerializeField] float sprintSpeed = 4.0f;
        [SerializeField] float rotationSpeed;
        public float desideredRotation;

        [Header("Gravity Settings")]
        public float verticalSpeed;
        [SerializeField] float Gravity = -15.0f;

        [Header("Cinemachine Settings")]
        [SerializeField] GameObject CinemachineCameraTarget;
        [SerializeField] float TopClamp = 70.0f;
        [SerializeField] float BottomClamp = -30.0f;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        protected override void Start()
        {
            base.Start();
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            characterController = GetComponent<CharacterController>();
        }
        void Update()
        {
            if (playerManager.canMove)
                HandleLocomotion();
            HandleGravity();
        }
        private void LateUpdate()
        {
            HandleCamera();
        }

        #region Implement Class
        protected override void HandleLocomotion()
        {
            if (playerManager.isAction)
                return;

            if (playerManager.lockTarget.isTargeting)
                HandleLockOnTargetMovement();
            else
                HandleNormalMovement();
        }
        protected override void HandleNormalMovement()
        {
            Vector3 movement = new Vector3(playerManager.inputManager.movementAxis.x, 0, playerManager.inputManager.movementAxis.y).normalized;
            Vector3 movementWithRotation = Quaternion.Euler(0, _cinemachineTargetYaw, 0) * movement;

            bool isSprintig = playerManager.inputManager.holdingRun;
            float nextSpeed = isSprintig ? sprintSpeed : walkSpeed;
            Vector3 verticalmovement = new Vector3(0.0f, verticalSpeed, 0.0f);
            characterController.Move((verticalmovement + (movementWithRotation * nextSpeed)) * Time.deltaTime);

            if (playerManager.canRotate)
            {
                if (movementWithRotation.magnitude > 0)
                {
                    desideredRotation = Mathf.Atan2(movementWithRotation.x, movementWithRotation.z) * Mathf.Rad2Deg;
                }
                else
                    nextSpeed = 0;
                Quaternion currentRotation = transform.rotation;
                Quaternion targetRotation = Quaternion.Euler(0, desideredRotation, 0);
                HandleRotation(currentRotation, targetRotation);
            }

            float defSpeed = Mathf.Clamp(nextSpeed, 0.0f, isSprintig ? 2.0f : 1.0f);
            float defSpeedY = Mathf.Lerp(playerManager.animatorManager.GetAnimatorVerticalValue(), defSpeed, 6 * Time.deltaTime);
            playerManager.animatorManager.UpdateAnimatorValue(defSpeedY);
        }
        protected override void HandleLockOnTargetMovement()
        {
            Vector3 movement = new Vector3(playerManager.inputManager.movementAxis.x, 0, playerManager.inputManager.movementAxis.y).normalized;
            float defSpeedX = Mathf.Lerp(playerManager.animatorManager.GetAnimatorHorizontalValue(), movement.x, Time.deltaTime *6);
            float defSpeedY = Mathf.Lerp(playerManager.animatorManager.GetAnimatorVerticalValue(), movement.z, Time.deltaTime * 6);

            float speed = 0;
            if (playerManager.inputManager.movementAxis != Vector2.zero)
            {
                movement = transform.right * playerManager.inputManager.movementAxis.x + transform.forward * playerManager.inputManager.movementAxis.y;
                movement.y = 0;
                speed = walkSpeed;
            }

            characterController.Move(movement.normalized * (speed * Time.deltaTime));
            playerManager.animatorManager.UpdateAnimatorValue(defSpeedY, defSpeedX);

            var direction = (playerManager.lockTarget.currentTarget.position - transform.position).normalized;
            direction.y = 0;
            HandleAimRotation(transform.rotation, Quaternion.LookRotation(direction));
        }
        protected override void HandleRotation(Quaternion current, Quaternion target)
        {
            transform.rotation = Quaternion.Lerp(current, target, rotationSpeed * Time.deltaTime);
        }
        public override void HandleAimRotation(Quaternion current, Quaternion target)
        {
            transform.rotation = Quaternion.Lerp(current, target, rotationSpeed * Time.deltaTime);
            desideredRotation = transform.rotation.eulerAngles.y;
        }
        protected override void HandleCamera()
        {
            if (playerManager.lockTarget.isTargeting)
                return;
            if (playerManager.inputManager.lookAxis.sqrMagnitude >= 0f)
            {
                _cinemachineTargetYaw += playerManager.inputManager.lookAxis.x * 1.0f;
                _cinemachineTargetPitch += playerManager.inputManager.lookAxis.y * 1.0f;
            }
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,
                _cinemachineTargetYaw, 0.0f);
        }
        protected override void HandleGravity()
        {
            if (Physics.Raycast(transform.position, Vector3.down, .5f, LayerMask.GetMask("Ground")))
            {
                verticalSpeed = 0.01f;
            }
            else
            {
                verticalSpeed += Gravity * Time.deltaTime;
            }
        }
        #endregion

        #region Utils
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        #endregion
    }
}
