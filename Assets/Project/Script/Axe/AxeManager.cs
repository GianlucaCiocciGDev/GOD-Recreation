using UnityEngine;

namespace GDev
{
    public class AxeManager : MonoBehaviour
    {
        PlayerManager playerManager;

        public bool active;
        public float rotationSpeed;

        BoxCollider axeCollider;

        [Header("Weapon FX")]
        [SerializeField] private ParticleSystem trailFX;
        [SerializeField] private ParticleSystem grabFX;
        [SerializeField] private ParticleSystem bloodFX;

        public bool enableRaycast = false;
        [SerializeField] Transform raycastStartPoint;
        [SerializeField] Transform raycastEndPoint;
        int layerMask = 1 << 8;

        private void Start()
        {
            axeCollider = GetComponentInChildren<BoxCollider>();
            playerManager = GetComponentInParent<PlayerManager>();
        }
        void Update()
        {
            if (active)
            {
                transform.localEulerAngles += Vector3.forward * rotationSpeed * Time.deltaTime;

            }
            else if (enableRaycast)
            {
                if (Physics.Linecast(raycastStartPoint.position, raycastEndPoint.position, out RaycastHit hit, layerMask))
                    HandleTakeDamage(hit);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                GetComponent<Rigidbody>().Sleep();
                GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                GetComponent<Rigidbody>().isKinematic = true;
                active = false;
                if (other.gameObject.layer == 8)
                    HandleTakeDamage(other.transform);
            }
        }

        #region Take Damage
        private void HandleTakeDamage(RaycastHit hit)
        {
            Radgoll damage = hit.transform.GetComponentInParent<Radgoll>();
            if (damage != null)
                damage.transform.GetComponent<Animator>().CrossFade("React", .2f);
            Instantiate(bloodFX, hit.point, Quaternion.identity);
        }
        private void HandleTakeDamage(Transform hit)
        {
            this.transform.SetParent(hit.transform);
            Radgoll damage = hit.transform.GetComponentInParent<Radgoll>();
            if (damage != null)
                damage.ActivateRagdoll(playerManager.transform, hit.transform);
            Instantiate(bloodFX, hit.position, Quaternion.identity);
        }
        #endregion

        #region FX
        public void PlayWeaponFX()
        {
            trailFX.Stop();
            if (trailFX.isStopped)
            {
                trailFX.Play();
            }
        }
        public void PlayWeaponFXCatch()
        {
            grabFX.Stop();
            if (grabFX.isStopped)
            {
                grabFX.Play();
            }
        }
        #endregion

        #region Manage Damage Action
        public void OpenCollider()
        {
            axeCollider.enabled = true;
        }
        public void CloseCollider()
        {
            axeCollider.enabled = false;
        }
        public void OpenRaycast()
        {
            enableRaycast = true;
        }
        public void CloseRaycast()
        {
            enableRaycast = false;
        }
        #endregion
    }
}
