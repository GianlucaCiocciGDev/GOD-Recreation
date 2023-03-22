using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDev
{
    public class PlayerAnimatorManager : AnimatorManager
    {
        private int VerticalID;
        private int HorizontalID;
        protected override void Start()
        {
            base.Start();
            VerticalID = Animator.StringToHash("Vertical");
            HorizontalID = Animator.StringToHash("Horizontal");
        }
        public override void UpdateAnimatorValue(float verticalValue,float? horizontaValue = null)
        {
            baseAnimator.SetFloat(VerticalID, verticalValue);
            if(horizontaValue.HasValue)
                baseAnimator.SetFloat(HorizontalID, horizontaValue.Value);
        }
        public override void PlayTargetAnimation(string animationName)
        {
            baseAnimator.CrossFade(animationName, .1f);
        }
        public float GetAnimatorVerticalValue()
        {
            return baseAnimator.GetFloat(VerticalID);
        }
        public float GetAnimatorHorizontalValue()
        {
            return baseAnimator.GetFloat(HorizontalID);
        }
        public override void SetBoolState(string stateName, bool stateValue)
        {
            baseAnimator.SetBool(stateName, stateValue);
        }

        #region Animation Events
        public override void EnableCombo()
        {
            baseAnimator.SetBool("CanDoCombo", true);
        }
        public override void DisableCombo()
        {
            baseAnimator.SetBool("CanDoCombo", false);
        }
        #endregion
    }
}
