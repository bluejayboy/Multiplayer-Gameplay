using System;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Creature")]
    public sealed class CreatureData : ScriptableObject
    {
        [Header("General")] [SerializeField] private Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
        public Vector3 Scale { get { return scale; } }

        [SerializeField] private string[] abilities = null;
        public string[] Abilities { get { return abilities; } }

        [SerializeField] private int health = 100;
        public int Health { get { return health; } }

        [SerializeField] private float lifetime = 10.0f;
        public float Lifetime { get { return lifetime; } }

        [SerializeField] private bool allowInteract = true;
        public bool AllowInteract { get { return allowInteract; } }

        [Header("Control")] [SerializeField] private Pose stand = default(Pose);
        public Pose Stand { get { return stand; } }

        [SerializeField] private Pose crouch = default(Pose);
        public Pose Crouch { get { return crouch; } }

        [Header("Loadout")] [SerializeField] private string defaultGadget = string.Empty;
        public string DefaultGadget { get { return defaultGadget; } }

        [Header("Audio")]

        [SerializeField] private AudioClip[] grunts = null;
        public AudioClip[] Grunts { get { return grunts; } }

        [SerializeField] private AudioClip[] screams = null;
        public AudioClip[] Screams { get { return screams; } }

        [SerializeField] private float gruntChance = 0.5f;
        public float GruntChance { get { return gruntChance; } }

        [SerializeField] private float screamChance = 1.0f;
        public float ScreamChance { get { return screamChance; } }

        [Header("Outfit")] [SerializeField] private GameObject[] hats = null;
        public GameObject[] Hats { get { return hats; } }

        [SerializeField] private GameObject[] scarves = null;
        public GameObject[] Scarves { get { return scarves; } }

        [SerializeField] private GameObject[] vests = null;
        public GameObject[] Vests { get { return vests; } }

        [SerializeField] private string face = null;
        public string Face { get { return face; } }

        [Header("Skin Color")] [SerializeField] private bool isFixedSkinColor = false;
        public bool IsFixedSkinColor { get { return isFixedSkinColor; } }

        [SerializeField] private Color32 head = Color.white;
        public Color32 Head { get { return head; } }

        [SerializeField] private Color32 torso = Color.white;
        public Color32 Torso { get { return torso; } }

        [SerializeField] private Color32 leftHand = Color.white;
        public Color32 LeftHand { get { return leftHand; } }

        [SerializeField] private Color32 rightHand = Color.white;
        public Color32 RightHand { get { return rightHand; } }

        [SerializeField] private Color32 leftFoot = Color.white;
        public Color32 LeftFoot { get { return leftFoot; } }

        [SerializeField] private Color32 rightFoot = Color.white;
        public Color32 RightFoot { get { return rightFoot; } }

        [Serializable]
        public struct Pose
        {
            [SerializeField] private float moveSpeed;
            public float MoveSpeed { get { return moveSpeed; } }

            [SerializeField] private float footstepRate;
            public float FootstepRate { get { return footstepRate; } }
        }
    }
}