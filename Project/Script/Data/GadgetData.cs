using System;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public abstract class GadgetData : ScriptableObject
    {
        public float MoveSpeedMultiplier = 1;

        [SerializeField] private string id = string.Empty;
        public string ID { get { return id; } }

        [SerializeField] private string display = string.Empty;
        public string Display { get { return display; } }

        [SerializeField] private bool visibleLeftHand = false;
        public bool VisibleLeftHand { get { return visibleLeftHand; } }

        [SerializeField] private bool enableCrosshair = true;
        public bool EnableCrosshair { get { return enableCrosshair; } }

        [SerializeField] private int slot = 0;
        public int Slot { get { return slot; } }

        [SerializeField] private float drawTime = 0.5f;
        public float DrawTime { get { return drawTime; } }

        [SerializeField] private BoltEntity pickup = null;
        public BoltEntity Pickup { get { return pickup; } }

        [SerializeField] private bool isFullAuto = false;
        public bool IsFullAuto { get { return isFullAuto; } }

        public enum SecondaryFireState { None, Attack, Aim }

        [SerializeField] private SecondaryFireState secondaryFire = SecondaryFireState.Attack;
        public SecondaryFireState SecondaryFire { get { return secondaryFire; } }

        [SerializeField] private AudioClip deploy = null;
        public AudioClip Deploy { get { return deploy; } }

        [SerializeField] private AudioClip primaryFire = null;
        public AudioClip PrimaryFire { get { return primaryFire; } }

        [SerializeField] private AudioClip primaryHold = null;
        public AudioClip PrimaryHold { get { return primaryHold; } }

        [SerializeField] private AudioClip primaryUp = null;
        public AudioClip PrimaryUp { get { return primaryUp; } }

        [SerializeField] private AudioClip secondaryFireSound = null;
        public AudioClip SecondaryFireSound { get { return secondaryFireSound; } }

        [SerializeField] private int damage = 0;
        public int Damage { get { return damage; } }

        [SerializeField] private float primaryFireRate = 5.0f;
        public float PrimaryFireRate { get { return primaryFireRate; } }

        [SerializeField] private float secondaryFireRate = 50.0f;
        public float SecondaryFireRate { get { return secondaryFireRate; } }

        [SerializeField] private float impactRadius = 1.0f;
        public float ImpactRadius { get { return impactRadius; } }

        [SerializeField] private float impactForce = 100.0f;
        public float ImpactForce { get { return impactForce; } }

        [SerializeField] private Pose idlePose = default(Pose);
        public Pose IdlePose { get { return idlePose; } }

        [SerializeField] private float poseSpeed = 10.0f;
        public float PoseSpeed { get { return poseSpeed; } }

        [Serializable]
        public struct Pose
        {
            [SerializeField] private Vector3 position;
            [SerializeField] public Vector3 Position { get { return position; } }

            [SerializeField] private Vector3 rotation;
            [SerializeField] public Vector3 Rotation { get { return rotation; } }

            [SerializeField] private float fovDivisor;
            [SerializeField] public float FovDivisor { get { return fovDivisor; } }
        }
    }
}