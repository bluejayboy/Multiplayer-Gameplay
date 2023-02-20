using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class TurretVisual : EntityEventListener<ITurretState>
    {
        [SerializeField] private HitscanGadgetData data = null;
        [SerializeField] private Transform root = null;
        [SerializeField] private GameObject impact = null;
        [SerializeField] private GameObject fpFlash = null;
        [SerializeField] private GameObject tpFlash = null;
        [SerializeField] private Transform muzzle = null;
        [SerializeField] private Transform ejector = null;

        [SerializeField] private float recoil = 1.0f;
        [SerializeField] private float kickSpeed = 100.0f;
        [SerializeField] private float returnSpeed = 10.0f;

        private Transform cachedTransform = null;
        private AudioSource audioSource;
        private Pooler pooler = null;

        private void Awake()
        {
            cachedTransform = transform;
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            pooler = Pooler.Instance;
        }

        private void FixedUpdate()
        {
            ApplyUpdate(Time.fixedDeltaTime);
        }

        public override void Attached()
        {
            state.OnFire = ApplyEffect;
        }

        public override void OnEvent(TracerEvent evnt)
        {
            SpawnTracer(evnt.Direction);
        }

        public override void OnEvent(ImpactEvent evnt)
        {
            SpawnImpact(evnt.Position, evnt.Rotation, evnt.Tag);
        }

        public void FlashMuzzle()
        {
            if (entity.HasControl)
            {
                fpFlash.SetActive(true);
            }
            else
            {
                tpFlash.SetActive(true);
            }
        }

        public void SpawnBulletShell()
        {
#if ISDEDICATED
                return;
#endif

            if (data.BulletShell == null)
            {
                return;
            }

            var bulletShell = pooler.Pooliate(data.BulletShell.name, ejector.position, ejector.rotation).GetComponent<Rigidbody>();

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

        public void SpawnImpact(Vector3 position, Quaternion rotation, string tag)
        {
            var bulletImpact = pooler.Pooliate(impact.name, position, rotation).GetComponent<BulletImpact>();

            bulletImpact.SpawnBulletImpact(tag);
        }

        public void ApplyEffect()
        {
            FlashMuzzle();
            SpawnBulletShell();
            Recoil();
            audioSource.PlayAudioClip(data.PrimaryFire);
        }

        private void ApplyUpdate(float time)
        {
            if (!entity.IsAttached)
            {
                return;
            }

            root.localPosition = Vector3.Lerp(root.localPosition, default(Vector3), returnSpeed * time);
            root.localRotation = Quaternion.Lerp(root.localRotation, Quaternion.identity, returnSpeed * time);
        }

        public void Recoil()
        {
            if (entity.IsAttached)
            {
                root.localPosition = Vector3.Lerp(root.localPosition, new Vector3(0.0f, 0.0f, -recoil), kickSpeed * Time.smoothDeltaTime);
            }
        }
    }
}