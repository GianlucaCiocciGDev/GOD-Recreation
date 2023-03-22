using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GDev
{
    public class OpenDoor : InteractManager
    {
        [SerializeField] Transform door;
        public bool canOpen = false;

        private void Update()
        {
            if (canOpen)
                DoInteract();
        }
        private async void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.CompareTag("Axe"))
            {
                canOpen = true;
                await Task.Run(() => Wait());
            }
        }

        public override void DoInteract() => door.position = Vector3.Lerp(door.position, new Vector3(6f, door.position.y, door.position.z), Time.deltaTime);

        async void Wait()
        {
            await Task.Delay(3000);
            canOpen = false;
            this.enabled = false;
        }
    }
}
