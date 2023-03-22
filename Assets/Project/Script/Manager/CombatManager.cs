using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDev
{
    public abstract class CombatManager : MonoBehaviour
    {
        protected PlayerManager playerManager;
        protected virtual void Start()
        {
            playerManager = GetComponent<PlayerManager>();
        }
        public abstract void HandleAttack();
        public abstract void HandleAiming(bool isAIming);
        protected abstract void HandleComboAttack();
        protected abstract void HandleLightAttack();
        protected abstract void HandleThrowAttack();
    }
}
