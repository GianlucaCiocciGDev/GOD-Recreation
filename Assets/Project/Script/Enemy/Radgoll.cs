using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GDev
{
    public class Radgoll : MonoBehaviour
    {
        Rigidbody[] rigidbodies;
        Animator enemyAnimator;
        public bool canLock = true;
        void Start()
        {
            rigidbodies = GetComponentsInChildren<Rigidbody>();
            enemyAnimator = GetComponent<Animator>();
            DeactivateRadgoll();
        }

        #region Radgoll
        private void DeactivateRadgoll()
        {
            foreach (Rigidbody currentRigidbody in rigidbodies)
            {
                currentRigidbody.isKinematic = true;
            }
            enemyAnimator.enabled = true;
        }
        public void ActivateRagdoll(Transform target, Transform point)
        {
            enemyAnimator.enabled = false;
            foreach (Rigidbody currentRigidbody in rigidbodies)
            {
                currentRigidbody.isKinematic = false;
            }
            point.GetComponent<Rigidbody>().AddForce(target.forward * 20, ForceMode.Impulse);
            canLock = false;
        }
        #endregion
    }
}
