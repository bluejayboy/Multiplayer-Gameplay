using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetVisualEffect : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private GameObject[] muzzleFlashes = null;
        [SerializeField] private Transform muzzle = null;
        [SerializeField] private Transform ejector = null;
        [SerializeField] private GameObject mag = null;

        private HitscanGadgetData data = null;
        private Pooler pooler = null;

        private void Awake()
        {
#if ISDEDICATED
                return;
#endif

            string view = " FP";

            if (gameObject.name.Contains(" TP"))
            {
                view = " TP";
            }

            var split = gameObject.name.Split(new string[] { view }, System.StringSplitOptions.None);
            var id = split[0];

            data = GetComponentInParent<PlayerLoadout>().Gadgets[id].Data as HitscanGadgetData;
            pooler = Pooler.Instance;
        }

        private void OnEnable()
        {
#if ISDEDICATED
                return;
#endif

            ToggleMag();

            Transform muzzleFlashParent = muzzleFlashes[0].transform.parent;

            muzzleFlashParent.SetParent(muzzle.transform, true);
            muzzleFlashParent.localPosition = default(Vector3);
            muzzleFlashParent.localRotation = Quaternion.identity;
        }

        public void FlashMuzzle()
        {
#if ISDEDICATED
                return;
#endif

            muzzleFlashes[Random.Range(0, muzzleFlashes.Length)].SetActive(true);
        }

        public void SpawnBulletShell()
        {
#if ISDEDICATED
                return;
#endif

            if (ejector == null || data.BulletShell == null)
            {
                return;
            }

            var bulletShell = pooler.Pooliate(data.BulletShell.name, ejector.position, ejector.rotation).GetComponent<Rigidbody>();

            //bulletShell.GetComponent<TrailRenderer>().Clear();
            bulletShell.AddForce(new Vector3(ejector.forward.x * Random.Range(200.0f, 250.0f), ejector.forward.y * Random.Range(200.0f, 250.0f), ejector.forward.z * Random.Range(200.0f, 250.0f)));
            bulletShell.AddTorque(new Vector3(Random.Range(-data.BulletShellTorque, data.BulletShellTorque), Random.Range(-data.BulletShellTorque, data.BulletShellTorque), Random.Range(-data.BulletShellTorque, data.BulletShellTorque)));
        }

        public void SpawnTracer(Vector3 direction)
        {
#if ISDEDICATED
                return;
#endif

            if (data.BulletTracer == null)
            {
                return;
            }

            pooler.Pooliate(data.BulletTracer.name, muzzle.position, Quaternion.LookRotation(direction));
        }

        public void DropMag()
        {
#if ISDEDICATED
                return;
#endif

            if (data.Mag == null)
            {
                return;
            }

            var mag = pooler.Pooliate(data.Mag.name, this.mag.transform.position, this.mag.transform.rotation).GetComponent<Rigidbody>();

            mag.AddForceAtPosition(new Vector3(data.MagForce * transform.forward.x, data.MagForce * transform.forward.y, data.MagForce * transform.forward.z), mag.transform.position);
        }

        public void ToggleMag()
        {
#if ISDEDICATED
                return;
#endif

            if (mag != null && entity.IsAttached)
            {
                mag.SetActive(state.OwnedGadgets[data.Slot].ActiveAmmo > 0);
            }
        }
    }
}