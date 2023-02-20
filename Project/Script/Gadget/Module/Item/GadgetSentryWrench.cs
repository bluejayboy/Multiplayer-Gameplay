using MEC;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetSentryWrench : GadgetDraw, IPrimaryDown, IPrimaryFireHold, IPrimaryFireUp, ISecondaryFire, IEnable
    {
        [SerializeField] private AudioSource audioSource = null;
        [SerializeField] private Transform playerRebound = null;
        [SerializeField] private LayerMask impactLayerMask = default(LayerMask);
        [SerializeField] private PlankHammerGadgetData data = null;

        private Transform preview = null;
        private Vector3 previewNormal = default(Vector3);
        private Vector3 previewPosition;
        private Quaternion previewRotation;
        private Pooler pooler = null;

        private bool canDrag = false;
        private bool canBuild = false;
        private bool isDragging = false;
        private float timer = 0.0f;

        public GadgetData Data { get { return data; } }
        private GadgetPose pose;
        private CoroutineHandle handle;

        private List<int> clientQueue = new List<int>();

        private void Awake()
        {
            pooler = Pooler.Instance;
        }

        public override void Equip()
        {
            base.Equip();

            pose = pose ?? GetComponent<GadgetPose>();

            Cancel();
        }

        public override void Dequip()
        {
            base.Dequip();

            Cancel();

            Timing.KillCoroutines(handle);
        }

        private void Update()
        {
            if (!entity.IsAttached)
            {
                return;
            }

            if (!entity.IsOwner)
            {
                for (int i = 0; i < clientQueue.Count; i++)
                {
                    if (clientQueue[i] <= (state.OwnedGadgets[data.Slot].ServerToken as AmmoPredictToken).frame)
                    {
                        clientQueue.RemoveAt(i);
                        i--;
                    }
                }

                if (state.OwnedGadgets[data.Slot].ServerToken != null)
                {
                    state.OwnedGadgets[data.Slot].PredictedAmmo = (state.OwnedGadgets[data.Slot].ServerToken as AmmoPredictToken).ammo - clientQueue.Count;
                }
            }
        }

        private void FixedUpdate()
        {
            if (!entity.IsAttached || !pose.IsDisabled)
            {
                return;
            }

            GadgetData.Pose finalPose = isDragging ? data.HoldPose : data.UpPose;
            float time = isDragging ? 10 : 30;

            transform.localPosition = Vector3.Slerp(transform.localPosition, finalPose.Position, time * Time.fixedDeltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(finalPose.Rotation), time * Time.fixedDeltaTime);
        }

        public void PrimaryDown()
        {
            Preview();
        }

        public void PrimaryHold()
        {
            Drag();
        }

        public void PrimaryUp()
        {
            Build();
        }

        public void SecondaryDown()
        {
            Cancel();
        }

        public void PlayPrimaryDown()
        {
            audioSource.PlayAudioClip(data.PrimaryFire);
        }

        public void PlayPrimaryUp()
        {
            audioSource.PlayAudioClip(data.PrimaryUp);
        }

        public void PlaySecondaryDown()
        {
            audioSource.PlayAudioClip(data.SecondaryFireSound);
        }

        private void Preview()
        {
            var hit = default(RaycastHit);

            if (Physics.Raycast(playerRebound.position, playerRebound.forward, out hit, data.Range, impactLayerMask))
            {
                state.ActiveGadget.PrimaryDown();

                canDrag = true;
                previewPosition = hit.point;
                previewNormal = hit.normal;
                previewRotation = Quaternion.FromToRotation(Vector3.up, previewNormal);

                if (entity.HasControl)
                {
                    preview = pooler.Pooliate(data.Preview.name, previewPosition, previewRotation).transform;
                }
            }
        }

        private void Drag()
        {
            if (!canDrag)
            {
                return;
            }

            pose.IsDisabled = true;
            isDragging = true;

            var hit = default(RaycastHit);

            if (Physics.Raycast(playerRebound.position, playerRebound.forward, out hit, data.Range, impactLayerMask))
            {
                previewNormal = hit.normal;
                previewPosition = hit.point;
            }
            else
            {
                previewNormal = Vector3.down;
                previewPosition = data.Range * playerRebound.forward + playerRebound.position;
            }

            preview.localPosition = previewPosition;
            preview.localRotation = Quaternion.Euler(0, transform.rotation.y,0);
            canBuild = previewNormal == Vector3.up;

            if (preview == null)
            {
                return;
            }

            Material plankMaterial = preview.GetComponentInChildren<Renderer>().material;

            if (canBuild)
            {
                plankMaterial.color = data.ValidColor;
            }
            else
            {
                plankMaterial.color = data.InvalidColor;
            }
        }

        private void Build()
        {
            if (!canDrag)
            {
                return;
            }

            if (canBuild)
            {
                state.ActiveGadget.PrimaryUp();

                timer = data.PrimaryFireRate + BoltNetwork.ServerFrame;
                handle = Timing.RunCoroutine(ResetPose());

                isDragging = false;
                canDrag = false;
                canBuild = false;

                if (preview != null)
                {
                    preview.gameObject.SetActive(false);
                }

                if (!entity.IsOwner && !data.HasInfiniteAmmo && !state.unlimitedAmmo)
                {
                    clientQueue.Add(BoltNetwork.ServerFrame);
                }

                if (entity.IsOwner && !data.HasInfiniteAmmo && !state.unlimitedAmmo)
                {
                    state.OwnedGadgets[data.Slot].ActiveAmmo--;
                    state.OwnedGadgets[data.Slot].ServerToken = null;
                    state.OwnedGadgets[data.Slot].ServerToken = new AmmoPredictToken(BoltNetwork.ServerFrame, state.OwnedGadgets[data.Slot].ActiveAmmo);

                    var token = new PlankToken
                    {
                        StartPosition = previewPosition,
                        Normal = previewNormal,
                        NetworkID = entity.NetworkId
                    };

                    var plank = BoltPooler.Instance.Instantiate(BoltPrefabs.Plank, token, previewPosition, previewRotation, true);

                    if (entity.Controller != null)
                    {
                        plank.AssignControl(entity.Controller);
                    }
                    else
                    {
                        plank.TakeControl();
                    }
                }
            }
            else
            {
                Cancel();
            }
        }

        private void Cancel()
        {
            pose.IsDisabled = false;
            isDragging = false;
            canDrag = false;
            canBuild = false;

            if (preview == null)
            {
                return;
            }

            preview.gameObject.SetActive(false);
        }

        private IEnumerator<float> ResetPose()
        {
            yield return Timing.WaitForSeconds(0.5f);

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            pose.IsDisabled = false;
        }
    }
}