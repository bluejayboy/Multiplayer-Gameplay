using System;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Hitscan Gadget")]
    public class HitscanGadgetData : GadgetData
    {
        [SerializeField] private bool hasInfiniteAmmo = false;
        public bool HasInfiniteAmmo { get { return hasInfiniteAmmo; } }

        [SerializeField] private AudioClip dryFire = null;
        public AudioClip DryFire { get { return dryFire; } }

        [SerializeField] private AudioClip magUnload = null;
        public AudioClip MagUnload { get { return magUnload; } }

        [Header("Hitscan")] [SerializeField] private int fragments = 1;
        public int Fragments { get { return fragments; } }

        [SerializeField] private float range = 500.0f;
        public float Range { get { return range; } }

        [SerializeField] private float spread = 0.1f;
        public float Spread { get { return spread; } }

        [SerializeField] private float aimSpread = 0.0f;
        public float ScopeSpread { get { return aimSpread; } }

        [SerializeField] private float spreadAddend = 0.01f;
        public float SpreadAddend { get { return spreadAddend; } }

        [SerializeField] private float spreadSubtrahend = 0.001f;
        public float SpreadSubtrahend { get { return spreadSubtrahend; } }

        [SerializeField] private float aimSensitivityDivisor = 2.0f;
        public float AimSensitivityDivisor { get { return aimSensitivityDivisor; } }

        [Header("Slap")] [SerializeField] private int slapDamage = 50;
        public int SlapDamage { get { return slapDamage; } }

        [SerializeField] private float preSlapDelay = 0.2f;
        public float PreSlapDelay { get { return preSlapDelay; } }

        [SerializeField] private float postSlapDelay = 0.1f;
        public float PostSlapDelay { get { return postSlapDelay; } }

        [SerializeField] private SecondaryAttack preSlap = default(SecondaryAttack);
        public SecondaryAttack PreSlap { get { return preSlap; } }

        [SerializeField] private SecondaryAttack postSlap = default(SecondaryAttack);
        public SecondaryAttack PostSlap { get { return postSlap; } }

        [Header("Ammo")] [SerializeField] private int maxAmmo = 30;
        public int MaxAmmo { get { return maxAmmo; } }

        [SerializeField] private Rigidbody mag = null;
        public Rigidbody Mag { get { return mag; } }

        [SerializeField] private float magForce = 200.0f;
        public float MagForce { get { return magForce; } }

        [Header("Pose")] [SerializeField] private Pose aimPose = default(Pose);
        public Pose AimPose { get { return aimPose; } }

        [Header("Kick")] [SerializeField] private float recoilPitch = 0.5f;
        public float RecoilPitch { get { return recoilPitch; } }

        [SerializeField] private float recoilYaw = 1.0f;
        public float RecoilYaw { get { return recoilYaw; } }

        [SerializeField] private float reboundPitch = 0.5f;
        public float ReboundPitch { get { return reboundPitch; } }

        [SerializeField] private float reboundYaw = 1.0f;
        public float ReboundYaw { get { return reboundYaw; } }

        [SerializeField] private float reboundRoll = 0.5f;
        public float ReboundRoll { get { return reboundRoll; } }

        [SerializeField] private float reboundDepth = 0.5f;
        public float ReboundDepth { get { return reboundDepth; } }

        [SerializeField] private float kickSpeed = 100.0f;
        public float KickSpeed { get { return kickSpeed; } }

        [SerializeField] private Kick idleKick = default(Kick);
        public Kick IdleKick { get { return idleKick; } }

        [Header("Visual")] [SerializeField] private GameObject bulletTracer = null;
        public GameObject BulletTracer { get { return bulletTracer; } }

        [SerializeField] private Rigidbody bulletShell = null;
        public Rigidbody BulletShell { get { return bulletShell; } }

        [SerializeField] private float bulletShellTorque = 30.0f;
        public float BulletShellTorque { get { return bulletShellTorque; } }

        [Serializable]
        public struct Kick
        {
            [SerializeField] private Vector3 position;
            [SerializeField] public Vector3 Position { get { return position; } }

            [SerializeField] private Vector3 rotation;
            [SerializeField] public Vector3 Rotation { get { return rotation; } }
        }

        [Serializable]
        public struct SecondaryAttack
        {
            [SerializeField] private Vector3 position;
            [SerializeField] public Vector3 Position { get { return position; } }

            [SerializeField] private Vector3 rotation;
            [SerializeField] public Vector3 Rotation { get { return rotation; } }

            [SerializeField] private float speed;
            public float Speed { get { return speed; } }
        }
    }
}