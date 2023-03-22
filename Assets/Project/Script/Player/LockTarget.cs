using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using System.Linq;

namespace GDev
{
    public class LockTarget : MonoBehaviour
    {
        PlayerManager playerManager;
        [Header("Camera Lock On Target")]
        [SerializeField] private CinemachineVirtualCamera aimCamera; 

        private string enemyTag => "Enemy";
        [Header("Lock Settings")]
        [SerializeField] private float minDistance; 
        [SerializeField] private float maxDistance;
        [SerializeField] private float maxAngle = 90;

        public bool isTargeting;
        [HideInInspector]public Transform currentTarget;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
        }

        #region Manage Lock Target
        public void AssignTarget()
        {
            if (ClosestTarget())
            {
                currentTarget = ClosestTarget().transform;
                isTargeting = true;
                aimCamera.LookAt=currentTarget;
                aimCamera.gameObject.SetActive(true);
            }
        }
        public void ResetTarget()
        {
            aimCamera.gameObject.SetActive(false);
            currentTarget = null;
            aimCamera.LookAt = currentTarget;
            isTargeting = false;
        }
        #endregion

        #region Utils
        private GameObject ClosestTarget()
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag(enemyTag).Where(x=>x.transform.GetComponent<Radgoll>().canLock).ToArray();
            GameObject closest = null;
            float distance = maxDistance;
            float currAngle = maxAngle;
            Vector3 position = transform.position;
            foreach (GameObject go in gos)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.magnitude;
                if (curDistance < distance)
                {
                    Vector3 viewPos = playerManager.mainCamera.WorldToViewportPoint(go.transform.position);
                    Vector2 newPos = new Vector3(viewPos.x - 0.5f, viewPos.y - 0.5f);
                    if (Vector3.Angle(diff.normalized, playerManager.mainCamera.transform.forward) < maxAngle)
                    {
                        closest = go;
                        currAngle = Vector3.Angle(diff.normalized, playerManager.mainCamera.transform.forward.normalized);
                        distance = curDistance;
                    }
                }
            }
            return closest;
        }
        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawWireSphere(transform.position, maxDistance);
        //}
        #endregion
    }
}
