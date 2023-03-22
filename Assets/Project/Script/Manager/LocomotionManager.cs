using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDev
{
    public abstract class LocomotionManager : MonoBehaviour
    {
        protected PlayerManager playerManager;
        protected virtual void Start()
        {
            playerManager = GetComponent<PlayerManager>();
        }
        protected abstract void HandleLocomotion();
        protected abstract void HandleNormalMovement();
        protected abstract void HandleLockOnTargetMovement();
        protected abstract void HandleRotation(Quaternion current, Quaternion target);
        public abstract void HandleAimRotation(Quaternion current, Quaternion target);
        protected abstract void HandleGravity();
        protected abstract void HandleCamera();
    }
}
