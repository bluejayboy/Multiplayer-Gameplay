using Photon.Bolt;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Scram
{
    [DisallowMultipleComponent]
    [ShowOdinSerializedPropertiesInInspector]
    public sealed class PlayerLoadout : EntityBehaviour<IPlayerState>, ISerializationCallbackReceiver, ISupportsPrefabSerialization
    {
        [SerializeField] private Transform tpParent;
        [SerializeField] private Renderer claws;
        [SerializeField] private float dropForce = 100.0f;
        [SerializeField] private Dictionary<string, GadgetInfo> gadgets = null;

        private Transform cachedTransform = null;
        private AudioSource audioSource = null;
        private CreatureData creatureData = null;
        private PlayerMaterial playerVisual = null;
        private Transform playerRebound = null;
        private PlayerToken playerToken = null;
        private PlayerCommander commander = null;
        private Player player;
        private Renderer[] renderers;

        public Dictionary<string, GadgetInfo> Gadgets { get { return gadgets; } }
        public GadgetInfo ActiveGadget { get; private set; }

        private IPrimaryDown primaryDown;
        private IPrimaryFireHold primaryHold;
        private IPrimaryFireUp primaryUp;
        private ISecondaryFire secondaryDown;
        private IHitscan hitscan;

        private void Awake()
        {
            cachedTransform = transform;
            audioSource = GetComponentInParent<AudioSource>();
            playerVisual = GetComponent<PlayerMaterial>();
            playerRebound = GetComponentInChildren<PlayerRebound>(true).transform;
            commander = GetComponent<PlayerCommander>();
            player = GetComponent<Player>();
            renderers = tpParent.GetComponentsInChildren<Renderer>(true);
        }

        private void Update()
        {
            if (!entity.IsAttached)
            {
                return;
            }

            if (state.ActiveGadget.Slot < 0)
            {
                return;
            }

            if (BoltNetwork.IsServer && state.OwnedGadgets.Length > 0 && state.OwnedGadgets[state.ActiveGadget.Slot].ServerToken == null || BoltNetwork.IsServer && state.OwnedGadgets.Length > 0 && (state.OwnedGadgets[state.ActiveGadget.Slot].ServerToken as AmmoPredictToken).ammo != state.OwnedGadgets[state.ActiveGadget.Slot].ActiveAmmo)
            {
                state.OwnedGadgets[state.ActiveGadget.Slot].ServerToken = new AmmoPredictToken(BoltNetwork.ServerFrame, state.OwnedGadgets[state.ActiveGadget.Slot].ActiveAmmo);
            }
        }

        public override void Attached()
        {
            playerToken = entity.AttachToken as PlayerToken;

            if (playerToken == null)
            {
                return;
            }

            state.AddCallback("OwnedGadgets[].PredictedAmmo", DropMag);
            state.AddCallback("ActiveGadget.Slot", ApplyGadget);
            InitializeReplication();

            creatureData = player.List.Datas[playerToken.Creature];

            if (string.IsNullOrEmpty(playerToken.Gadget1))
            {
                playerToken.Gadget1 = creatureData.DefaultGadget;
            }

            if (!entity.IsOwner)
            {
                return;
            }

            state.OwnedGadgets[state.ActiveGadget.Slot].ActiveAmmo = -1;
            state.OwnedGadgets[state.ActiveGadget.Slot].MaxAmmo = -1;
            state.ActiveGadget.Slot = -1;

            if (!string.IsNullOrEmpty(playerToken.Gadget1))
            {
                InitializeLoadout(playerToken.Gadget1, gadgets[playerToken.Gadget1].Data.Slot, true);
            }

            if (!string.IsNullOrEmpty(playerToken.Gadget2))
            {
                InitializeLoadout(playerToken.Gadget2, gadgets[playerToken.Gadget2].Data.Slot, false);
            }

            if (!string.IsNullOrEmpty(playerToken.Gadget3))
            {
                InitializeLoadout(playerToken.Gadget3, gadgets[playerToken.Gadget3].Data.Slot, false);
            }
        }

        public override void ControlGained()
        {
            state.ActiveGadget.OnHit = PlayerHud.Instance.DisplayHit;

            for (var i = 0; i < renderers.Length; i++)
            {
                renderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }

            claws.shadowCastingMode = ShadowCastingMode.ShadowsOnly;

            if (entity.IsOwner)
            {
                return;
            }

            if (!string.IsNullOrEmpty(playerToken.Gadget1))
            {
                InitializeLoadout(playerToken.Gadget1, gadgets[playerToken.Gadget1].Data.Slot, true);
            }

            if (!string.IsNullOrEmpty(playerToken.Gadget2))
            {
                InitializeLoadout(playerToken.Gadget2, gadgets[playerToken.Gadget2].Data.Slot, false);
            }

            if (!string.IsNullOrEmpty(playerToken.Gadget3))
            {
                InitializeLoadout(playerToken.Gadget3, gadgets[playerToken.Gadget3].Data.Slot, false);
            }
        }

        public override void Detached()
        {
            UnequipGadget();
        }

        public override void ControlLost()
        {
            for (var i = 0; i < renderers.Length; i++)
            {
                renderers[i].shadowCastingMode = ShadowCastingMode.On;
            }

            claws.shadowCastingMode = ShadowCastingMode.On;
        }

        private void DropMag()
        {
            if (!entity.IsOwner && !state.unlimitedAmmo)
            {
                state.ActiveGadget.EmptyMag();
            }
        }

        private void InitializeReplication()
        {
            state.AddCallback("OwnedGadgets[].ActiveAmmo", ToggleMag);

            state.ActiveGadget.OnPrimaryDown = () =>
            {
                if (primaryDown != null)
                {
                    primaryDown.PlayPrimaryDown();
                }
            };

            state.ActiveGadget.OnPrimaryUp = () =>
            {
                if (primaryUp != null)
                {
                    primaryUp.PlayPrimaryUp();
                }
            };

            state.ActiveGadget.OnSecondaryDown = () =>
            {
                if (secondaryDown != null)
                {
                    secondaryDown.PlaySecondaryDown();
                }
            };

            state.ActiveGadget.OnDryFire = () =>
            {
                if (hitscan != null)
                {
                    hitscan.ApplyDryFireEffect();
                }
            };

            state.ActiveGadget.OnEmptyMag = () =>
            {
                if (hitscan != null)
                {
                    hitscan.PlayEmptyMagEffect();
                }
            };
        }

        private void InitializeLoadout(string id, int slot, bool willEquip)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var ammo = -1;

                if (gadgets[id].Data is HitscanGadgetData)
                {
                    var hitscan = gadgets[id].Data as HitscanGadgetData;

                    ammo = hitscan.MaxAmmo;
                }

                PickUpGadget(id, slot, ammo, ammo, willEquip);
            }
        }

        public void PickUpGadget(string id, int slot, int activeAmmo, int maxAmmo, bool willEquip)
        {
            DropGadget(slot, willEquip, true);

            if (entity.IsOwner)
            {
                state.OwnedGadgets[slot].ActiveAmmo = activeAmmo;
                state.OwnedGadgets[slot].MaxAmmo = maxAmmo;

                if (gadgets[id].Data.Pickup != null)
                {
                    state.OwnedGadgets[slot].PrefabID = gadgets[id].Data.Pickup.ModifySettings().prefabId;
                }
            }

            state.OwnedGadgets[slot].ID = id;
            state.OwnedGadgets[slot].Display = gadgets[id].Data.Display;
            state.Effect.PickUp();

            if (willEquip)
            {
                state.ActiveGadget.Slot = -1;
                state.ActiveGadget.Slot = slot;
            }
        }

        public void DropGadget(int slot, bool willUnequip, bool allowForce)
        {
            if (!state.CanInteract || string.IsNullOrEmpty(state.OwnedGadgets[slot].ID))
            {
                return;
            }

            if (willUnequip)
            {
                state.ActiveGadget.Slot = -1;
            }

            state.OwnedGadgets[slot].ID = string.Empty;
            state.Effect.Drop();

            if (!entity.IsOwner)
            {
                return;
            }

            SpawnDrop(slot, allowForce);

            state.OwnedGadgets[slot].ActiveAmmo = -1;
            state.OwnedGadgets[slot].MaxAmmo = -1;
            state.OwnedGadgets[slot].PrefabID = default(PrefabId);
        }

        private void ApplyGadget()
        {
            if (state.ActiveGadget.Slot > -1 && !string.IsNullOrEmpty(state.OwnedGadgets[state.ActiveGadget.Slot].ID))
            {
                EquipGadget();
            }
            else
            {
                UnequipGadget();
            }
        }

        private void EquipGadget()
        {
            UnequipGadget();

            ActiveGadget = gadgets[state.OwnedGadgets[state.ActiveGadget.Slot].ID];
            ActiveGadget.FirstPerson.SetActive(true);
            ActiveGadget.ThirdPerson.SetActive(true);

            primaryDown = commander.PrimaryDown = ActiveGadget.FirstPerson.GetComponent<IPrimaryDown>();
            primaryHold = commander.PrimaryHold = ActiveGadget.FirstPerson.GetComponent<IPrimaryFireHold>();
            primaryUp = commander.PrimaryUp = ActiveGadget.FirstPerson.GetComponent<IPrimaryFireUp>();
            secondaryDown = commander.SecondaryDown = ActiveGadget.FirstPerson.GetComponent<ISecondaryFire>();
            hitscan = ActiveGadget.FirstPerson.GetComponent<IHitscan>();

            var enables = ActiveGadget.FirstPerson.GetComponents<IEnable>();

            if (enables != null)
            {
                for (int i = 0; i < enables.Length; i++)
                {
                    enables[i].Equip();
                }
            }

            playerVisual.SetHandsInvisible(true, ActiveGadget.Data.VisibleLeftHand);
            audioSource.PlayAudioClip(ActiveGadget.Data.Deploy);
        }

        public void UnequipGadget()
        {
            if (ActiveGadget != null)
            {
                var enables = ActiveGadget.FirstPerson.GetComponents<IEnable>();

                if (enables != null)
                {
                    for (int i = 0; i < enables.Length; i++)
                    {
                        enables[i].Dequip();
                    }
                }

                primaryDown = commander.PrimaryDown = null;
                primaryHold = commander.PrimaryHold = null;
                primaryUp = commander.PrimaryUp = null;
                secondaryDown = commander.SecondaryDown = null;
                hitscan = null;

                ActiveGadget.FirstPerson.SetActive(false);
                ActiveGadget.ThirdPerson.SetActive(false);

                playerVisual.SetHandsInvisible(false, ActiveGadget.Data.VisibleLeftHand);

                ActiveGadget = null;
            }
        }

        private void SpawnDrop(int slot, bool allowForce)
        {
            if (!GameMode.Instance.Data.AllowGadgetDrop)
            {
                return;
            }

            var token = new PickupToken
            {
                Force = (allowForce) ? new Vector3(playerRebound.forward.x * dropForce, playerRebound.forward.y * dropForce, playerRebound.forward.z * dropForce) : default(Vector3),
                Ammo = state.OwnedGadgets[slot].ActiveAmmo,
                DropEntity = entity.IsAttached ? entity : null,
                Color = state.Color
            };

            var position = new Vector3(cachedTransform.position.x + cachedTransform.up.x, cachedTransform.position.y + cachedTransform.up.y, cachedTransform.position.z + cachedTransform.up.z);

            BoltPooler.Instance.Instantiate(state.OwnedGadgets[slot].PrefabID, token, position, default(Quaternion), true);
            state.AltMoveSpeed = 1;
        }

        private void ToggleMag()
        {
            if (ActiveGadget == null)
            {
                return;
            }

            var effect = (entity.HasControl) ? ActiveGadget.FirstPerson.GetComponent<GadgetVisualEffect>() : ActiveGadget.ThirdPerson.GetComponent<GadgetVisualEffect>();

            if (effect != null)
            {
                effect.ToggleMag();
            }
        }

        [Serializable]
        public sealed class GadgetInfo
        {
            [SerializeField] private GadgetData data = null;
            public GadgetData Data { get { return data; } }

            [SerializeField] private GameObject firstPerson = null;
            public GameObject FirstPerson { get { return firstPerson; } }

            [SerializeField] private GameObject thirdPerson = null;
            public GameObject ThirdPerson { get { return thirdPerson; } }
        }

        #region Odin
        [SerializeField, HideInInspector]
        private SerializationData serializationData = default(SerializationData);

        SerializationData ISupportsPrefabSerialization.SerializationData { get { return serializationData; } set { serializationData = value; } }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref serializationData);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref serializationData);
        }
        #endregion
    }
}