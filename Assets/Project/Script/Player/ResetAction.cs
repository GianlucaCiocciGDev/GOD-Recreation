using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDev
{
    public class ResetAction : StateMachineBehaviour
    {
        public string IsActionBool = "isAction";
        public bool IsActionStatus = false;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(IsActionBool, IsActionStatus);
        }
    }
}
