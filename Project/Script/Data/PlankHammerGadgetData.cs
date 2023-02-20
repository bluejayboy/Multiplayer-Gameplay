using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Plank Gadget")]
    public sealed class PlankHammerGadgetData : HitscanGadgetData
    {
        [SerializeField] private Transform preview = null;
        public Transform Preview { get { return preview; } }

        [SerializeField] private BoltEntity plank = null;
        public BoltEntity Plank { get { return plank; } }

        [SerializeField] private float width = 0.1f;
        public float Width { get { return width; } }

        [SerializeField] private float height = 0.5f;
        public float Height { get { return height; } }

        [SerializeField] private float minimumLength = 0.5f;
        public float MinimumLength { get { return minimumLength; } }

        [SerializeField] private float maximumLength = 5.0f;
        public float MaximumLength { get { return maximumLength; } }

        [SerializeField] private Pose holdPose = default(Pose);
        public Pose HoldPose { get { return holdPose; } }

        [SerializeField] private Pose upPose = default(Pose);
        public Pose UpPose { get { return upPose; } }

        [SerializeField] private Color32 validColor;
        public Color32 ValidColor { get { return validColor; } }

        [SerializeField] private Color32 invalidColor;
        public Color32 InvalidColor { get { return invalidColor; } }

        [SerializeField] private Color32 aliveColor;
        public Color32 AliveColor { get { return aliveColor; } }

        [SerializeField] private Color32 deadColor;
        public Color32 DeadColor { get { return deadColor; } }
    }
}