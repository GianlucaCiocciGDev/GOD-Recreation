using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDev
{
    public abstract class AnimatorManager : MonoBehaviour
    {
        public Animator baseAnimator;
        protected virtual void Start()
        {
            baseAnimator = GetComponent<Animator>();
        }
        public abstract void UpdateAnimatorValue(float verticalValue, float? horizontaValue = null);
        public abstract void PlayTargetAnimation(string animationName);
        public abstract void SetBoolState(string stateName, bool stateValue);
        public abstract void EnableCombo();
        public abstract void DisableCombo();
    }
}
