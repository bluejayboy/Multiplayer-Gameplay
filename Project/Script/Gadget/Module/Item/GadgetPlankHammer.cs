using MEC;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetPlankHammer : GadgetDraw, IPrimaryDown, IPrimaryFireHold, IPrimaryFireUp, ISecondaryFire, IEnable
    {
        [SerializeField] private AudioSource audioSource = null;
        [SerializeField] private Transform playerRebound = null;
        [SerializeField] private LayerMask impactLayerMask = default(LayerMask);
        [SerializeField] private PlankHammerGadgetData data = null;

        private Vector3 startPoint = default(Vector3);
        private Vector3 endPoint = default(Vector3);
        private Vector3 plankNormal = default(Vector3);
        private Transform plankPreview = null;
        private Vector3 previewPosition;
        private Quaternion previewRotation;
        private Vector3 previewScale;
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
            if (!state.ActiveGadget.IsDrawn)
            {
                return;
            }

            if (!state.unlimitedAmmo && !data.HasInfiniteAmmo && state.OwnedGadgets[data.Slot].ActiveAmmo <= 0 && entity.IsOwner || !state.unlimitedAmmo && !data.HasInfiniteAmmo && state.OwnedGadgets[data.Slot].PredictedAmmo <= 0 && !entity.IsOwner)
            {
                return;
            }

            if (timer > BoltNetwork.ServerFrame)
            {
                return;
            }

            var hit = default(RaycastHit);

            if (Physics.Raycast(playerRebound.position, playerRebound.forward, out hit, data.Range, impactLayerMask))
            {
                state.ActiveGadget.PrimaryDown();

                canDrag = true;
                startPoint = hit.point;
                plankNormal = hit.normal;
                previewPosition = startPoint;
                previewRotation = Quaternion.FromToRotation(Vector3.up, plankNormal);

                if (entity.HasControl)
                {
                    plankPreview = pooler.Pooliate(data.Preview.name, previewPosition, previewRotation).transform;
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
                endPoint = hit.point;
                plankNormal = hit.normal;
            }
            else
            {
                endPoint = data.Range * playerRebound.forward + playerRebound.position;
            }

            var lookTarget = endPoint - previewPosition;

            if (lookTarget != default(Vector3))
            {
                previewRotation = Quaternion.LookRotation(lookTarget, plankNormal);
            }

            float plankLength = Vector3.Distance(startPoint, endPoint);

            previewScale = new Vector3(data.Height, data.Width, plankLength);

            if (plankLength >= data.MinimumLength && plankLength <= data.MaximumLength && endPoint == hit.point)
            {
                canBuild = true;
            }
            else
            {
                canBuild = false;
            }

            if (plankPreview == null)
            {
                return;
            }

            plankPreview.localRotation = previewRotation;
            plankPreview.localScale = previewScale;

            Material plankMaterial = plankPreview.GetComponentInChildren<Renderer>().material;

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

                if (plankPreview != null)
                {
                    plankPreview.gameObject.SetActive(false);
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
                        Length = previewScale.z,
                        StartPosition = startPoint,
                        EndPosition = endPoint,
                        Normal = plankNormal,
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

            if (plankPreview == null)
            {
                return;
            }

            plankPreview.gameObject.SetActive(false);
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