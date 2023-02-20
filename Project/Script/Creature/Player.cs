using Photon.Bolt;
using Hitbox;
using MEC;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scram
{
    [DisallowMultipleComponent]
    [ShowOdinSerializedPropertiesInInspector]
    public sealed class Player : EntityBehaviour<IPlayerState>, IDamageable, ISerializationCallbackReceiver, ISupportsPrefabSerialization
    {
        [SerializeField] private CreatureListData list;
        public CreatureListData List { get { return list; } }

        [SerializeField] private Dictionary<string, GameObject> abilities = null;

        [SerializeField] private GameObject[] localGameObjects = null;
        [SerializeField] private GameObject[] remoteGameObjects = null;

        [SerializeField] private GameObject view;
        [SerializeField] private Camera worldViewer = null;
        [SerializeField] private GameObject identity = null;
        [SerializeField] private GameObject spectateCamera = null;
        [SerializeField] private LayerMask originalWorldViewerLayerMask = default(LayerMask);
        [SerializeField] private LayerMask originalImpactLayerMask = default(LayerMask);
        [SerializeField] private LayerMask originalWallLayerMask = default(LayerMask);
        [SerializeField] private LayerMask impactLayerMask = default(LayerMask);
        [SerializeField] private LayerMask wallLayerMask = default(LayerMask);

        private string dataName = string.Empty;
        private CreatureData data = null;

        private AudioSource audioSource = null;
        private CharacterController characterController = null;
        private PlayerController playerController = null;
        private PlayerLoadout playerLoadout = null;
        private PlayerMaterial playerVisual = null;
        private PlayerInteract interact = null;
        private Transform[] hitboxes = null;
        private PlayerCustomization playerCustomization = null;
        private Collider ccCollider;
        private EvacuationGameMode evac;
        private PlayerCommander commander;

        public LayerMask ImpactLayerMask { get { return impactLayerMask; } }
        public LayerMask WallLayerMask { get { return wallLayerMask; } }

        private CoroutineHandle briefHealthHandle = default(CoroutineHandle);
        private List<Collider> ignoredColliders = new List<Collider>();

        private float voicePitch;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            characterController = GetComponent<CharacterController>();
            playerController = GetComponent<PlayerController>();
            playerLoadout = GetComponent<PlayerLoadout>();
            playerVisual = GetComponent<PlayerMaterial>();
            interact = GetComponentInChildren<PlayerInteract>(true);
            hitboxes = GetComponent<HitboxBody>().Transforms;
            playerCustomization = GetComponent<PlayerCustomization>();
            ccCollider = GetComponent<Collider>();
            evac = FindObjectOfType<EvacuationGameMode>();
            commander = GetComponent<PlayerCommander>();

#if ISDEDICATED
                return;
#endif
        }

        public override void Attached()
        {
            var token = entity.AttachToken as PlayerToken;

            if (token == null)
            {
                return;
            }

            dataName = token.Creature;
            data = list.Datas[dataName];
            transform.localScale = data.Scale;
            voicePitch = token.VoicePitch;

            if (GameMode.Instance.Data.ShowTeamIdentity)
            {
                worldViewer.cullingMask |= (1 << LayerMask.NameToLayer(token.TeamLayer + " Render"));
            }

            identity.layer = LayerMask.NameToLayer(token.TeamLayer + " Render");

            if (!GameMode.Instance.Data.AllowTeamKill)
            {
                impactLayerMask &= ~(1 << LayerMask.NameToLayer(token.TeamLayer + " Hitbox"));
            }

            for (var i = 0; i < hitboxes.Length; i++)
            {
                hitboxes[i].gameObject.layer = LayerMask.NameToLayer(token.TeamLayer + " Hitbox");
            }

            commander.Ability = null;

            for (var i = 0; i < data.Abilities.Length; i++)
            {
                abilities[data.Abilities[i]].SetActive(false);
            }

            for (var i = 0; i < data.Abilities.Length; i++)
            {
                var ability = abilities[data.Abilities[i]].GetComponent<Ability>();

                abilities[data.Abilities[i]].SetActive(true);
                commander.Ability = ability;

                if (entity.IsOwner)
                {
                    state.ActiveAbility.AbilityTimer = state.ActiveAbility.AbilityRate = ability.AbilityRate;
                }
            }

            state.AddCallback("playerScale", OnScaleChange);

            if (!entity.IsOwner)
            {
                return;
            }

            gameObject.name = "Creature: " + token.PenName;

            state.CanInteract = data.AllowInteract;
            state.PenName = token.PenName;
            state.Color = token.TeamColor;
            state.Team = token.TeamID;
            state.TeamLayer = token.TeamLayer;
            state.MaxHealth = data.Health;
            state.ActiveHealth = state.MaxHealth;
            state.BriefHealth = 0;

            interact.enabled = true;
        }

        public override void Detached()
        {
            if (!entity.IsOwner)
            {
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            }

            if (entity.IsOwner)
            {
                gameObject.name = "Creature: None";
            }

            transform.SetParent(null, true);
            ResetIgnoredColliders();

            state.SetTransforms(state.Transform, null);
            state.Horizontal = 0;
            state.Vertical = 0;
            state.Pitch = 0;
            state.IsGrounded = false;
            state.IsCrouching = false;
            state.ActiveHealth = 0;
            state.MaxHealth = 0;
            state.BriefHealth = 0;
            state.ServerFrame = 0;
            state.CanInteract = false;
            state.InteractTimer = 0;
            state.IsInvincible = false;
            state.IsImmobilized = false;
            state.isFrozen = false;
            state.unlimitedAmmo = false;
            state.playerScale = 1;
            state.playerGravity = 1;
            state.MainMoveSpeed = 1;
            state.AltMoveSpeed = 1;
            state.DisableArmLook = false;
            state.ActiveAbility.IsUsingAbility = false;
            state.ActiveAbility.AbilityTimer = 0;
            state.ActiveAbility.AbilityRate = 0;
            state.ActiveGadget.Slot = -1;
            state.ActiveGadget.IsSafe = false;
            state.ActiveGadget.IsAiming = false;
            state.ActiveGadget.IsSlapping = false;
            state.ActiveGadget.IsDrawn = false;
            state.ActiveGadget.HealTimer = 0;
            state.ActiveGadget.MaxHealTime = 0;

            for (int i = 0; i < state.OwnedGadgets.Length; i++)
            {
                state.OwnedGadgets[i].ID = string.Empty;
                state.OwnedGadgets[i].Display = string.Empty;
                state.OwnedGadgets[i].ActiveAmmo = 0;
                state.OwnedGadgets[i].MaxAmmo = 0;
                state.OwnedGadgets[i].PrefabID = default(PrefabId);
                state.OwnedGadgets[i].PredictedAmmo = 0;
                state.OwnedGadgets[i].ServerToken = null;
            }

            state.Movement.LocalPosition = Vector3.zero;
            state.Movement.Position = Vector3.zero;
            state.Movement.Velocity = Vector3.zero;
            state.Movement.CharacterVelocity = Vector3.zero;
            state.Movement.JumpFrames = 0;
            state.Movement.ForceFrames = 0;
            state.Movement.IsGrounded = false;
            state.Movement.IsCrouching = false;
            state.Movement.StepOffset = 0;
            state.Movement.ColliderCenter = Vector3.zero;
            state.Movement.ColliderHeight = 0;

            state.ActiveAbility.OnStartAbility = null;
            state.ActiveAbility.OnEndAbility = null;
            state.ActiveGadget.OnPrimaryDown = null;
            state.ActiveGadget.OnPrimaryUp = null;
            state.ActiveGadget.OnSecondaryDown = null;
            state.ActiveGadget.OnDryFire = null;
            state.ActiveGadget.OnEmptyMag = null;
            state.ActiveGadget.OnHit = null;
            state.Effect.OnJump = null;
            state.Effect.OnCrouch = null;
            state.Effect.OnUncrouch = null;
            state.Effect.OnLandSoft = null;
            state.Effect.OnLandHard = null;
            state.Effect.OnTakeFallDamage = null;
            state.Effect.OnTakeDamage = null;
            state.Effect.OnStepFoot = null;
            state.Effect.OnMeleeHit = null;
            state.Effect.OnShakeScreen = null;
            state.Effect.OnPickUp = null;
            state.Effect.OnDrop = null;
            state.Effect.OnFart = null;

            commander.Ability = null;

            if (data != null)
            {
                for (var i = 0; i < data.Abilities.Length; i++)
                {
                    abilities[data.Abilities[i]].SetActive(false);
                }
            }

            if (GameMode.Instance != null)
            {
                if (GameMode.Instance.Data.ShowTeamIdentity)
                {
                    worldViewer.cullingMask = originalWorldViewerLayerMask;
                }

                if (!GameMode.Instance.Data.AllowTeamKill)
                {
                    impactLayerMask = originalImpactLayerMask;
                    wallLayerMask = originalWallLayerMask;
                }
            }

            data = null;
            transform.localScale = Vector3.one;

            state.RemoveAllCallbacks();

            Timing.KillCoroutines(briefHealthHandle);
        }

        public override void ControlGained()
        {
            if (PlayerInfo.Instance != null)
            {
                PlayerInfo.Instance.MyPlayer = this;
            }

            interact.enabled = true;
            playerVisual.SetBodyInvisible(true);

            for (var i = 0; i < localGameObjects.Length; i++)
            {
                localGameObjects[i].SetActive(true);
            }

            for (var i = 0; i < remoteGameObjects.Length; i++)
            {
                remoteGameObjects[i].SetActive(false);
            }

            FloatingText.instance.DisableAll();
            PlayerHud.Instance.Attach(entity, state, true);
            DisableRendererOnOwn.playerInstance = this;

            if (!BoltNetwork.IsSinglePlayer)
            {
                ClientPlayerAction.Instance.OnSpawn.Invoke();
            }

            PlayerHud.Instance.Ability.SetActive(false);

            if (data.Abilities.Length > 0)
            {
                PlayerHud.Instance.Ability.SetActive(true);
            }

            Stage.Instance.StageViewer.SetActive(false);
        }

        public override void ControlLost()
        {
            if (Camera.main == null)
            {
                Stage.Instance.StageViewer.SetActive(true);
            }

            if (PlayerInfo.Instance != null)
            {
                PlayerInfo.Instance.MyPlayer = null;
            }

            interact.enabled = false;
            playerVisual.SetBodyInvisible(false);

            for (var i = 0; i < localGameObjects.Length; i++)
            {
                localGameObjects[i].SetActive(false);
            }

            for (var i = 0; i < remoteGameObjects.Length; i++)
            {
                remoteGameObjects[i].SetActive(true);
            }

            FloatingText.instance.DisableAll();
            PlayerHud.Instance.Detach();
        }

        public void Spectate(bool isTrue)
        {
            if (isTrue)
            {
                spectateCamera.SetActive(true);
                Stage.Instance.StageViewer.SetActive(false);
                PlayerHud.Instance.Attach(entity, state, false);
            }
            else
            {
                spectateCamera.SetActive(false);

                if (Camera.main == null)
                {
                    Stage.Instance.StageViewer.SetActive(true);
                }
            }
        }

        public void IgnoreCollider(Collider collider, bool isIgnore)
        {
            Physics.IgnoreCollision(ccCollider, collider, isIgnore);

            if (isIgnore)
            {
                if (!ignoredColliders.Contains(collider))
                {
                    playerController.EnvironmentLayerMask &= ~(1 << collider.gameObject.layer);
                    ignoredColliders.Add(collider);
                }
            }
            else
            {
                if (ignoredColliders.Contains(collider))
                {
                    playerController.EnvironmentLayerMask |= (1 << collider.gameObject.layer);
                    ignoredColliders.Remove(collider);
                }
            }
        }

        private void ResetIgnoredColliders()
        {
            for (var i = 0; i < ignoredColliders.Count; i++)
            {
                Physics.IgnoreCollision(ccCollider, ignoredColliders[i], false);
            }

            ignoredColliders.Clear();
        }

        public void TakeDamage(int damage, Vector3 direction, string enemyName = "", PlayerConnection enemyConnection = null, bool isFall = false)
        {
            if (state.IsInvincible || !entity.IsAttached)
            {
                return;
            }

            var remainingDamage = 0;
            int bread = damage;

            if (bread > state.ActiveHealth + state.BriefHealth)
            {
                bread = state.ActiveHealth + state.BriefHealth;
            }

            if (state.BriefHealth > 0)
            {
                remainingDamage = damage - state.BriefHealth;
                state.BriefHealth -= damage;
            }
            else
            {
                state.ActiveHealth -= damage;
            }

            if (remainingDamage > 0)
            {
                state.ActiveHealth -= remainingDamage;
            }

            if (enemyConnection != null && enemyConnection != entity.Controller.GetPlayerConnection())
            {
                enemyConnection.PlayerInfo.state.Bread += bread;
                BoltGlobalEvent.SendBread(bread, enemyConnection.BoltConnection);
            }

            if (state.ActiveHealth > 0)
            {
                if (isFall)
                {
                    return;
                }

                state.Effect.TakeDamage();
                BoltEntityEvent.SendDamageReceive(damage, entity);
            }
            else
            {
                Die(direction, enemyName, enemyConnection);
            }
        }

        public void Die(Vector3 direction, string enemyName = "", PlayerConnection enemyConnection = null)
        {
            if (!entity.IsAttached || entity.Controller.GetPlayerConnection() == null || entity.Controller.GetPlayerConnection().PlayerInfo == null)
            {
                return;
            }

            if (playerLoadout != null && state.ActiveGadget.Slot > -1)
            {
                playerLoadout.DropGadget(state.ActiveGadget.Slot, true, false);
            }

            if (enemyConnection != null && enemyConnection.PlayerInfo != null)
            {
                var enemyInfoState = enemyConnection.PlayerInfo.state;

                enemyInfoState.Kills++;
                enemyInfoState.Score++;

                //StartCoroutine(ItemController.AddLeaderboardScore((ulong)enemyConnection.SteamID, 3058973));
            }

            if (HostPlayerAction.Instance.OnDeath != null)
            {
                HostPlayerAction.Instance.OnDeath.Invoke(entity.Controller.GetPlayerConnection(), transform.position, transform.rotation.eulerAngles.y);
            }

            if (HostPlayerAction.Instance.TurretDeath != null)
            {
                HostPlayerAction.Instance.TurretDeath.Invoke(entity);
            }

            if (playerCustomization != null)
            {
                var clientToken = new RagdollToken
                {
                    IsController = false,
                    Creature = dataName,
                    HeadGarb = playerCustomization.HeadGarb,
                    FaceGarb = playerCustomization.FaceGarb,
                    NeckGarb = playerCustomization.NeckGarb,
                    HipGarb = playerCustomization.HipGarb,
                    Face = playerCustomization.Face,
                    HeadColor = playerCustomization.HeadColor,
                    TorsoColor = playerCustomization.TorsoColor,
                    LeftHandColor = playerCustomization.LeftHandColor,
                    RightHandColor = playerCustomization.RightHandColor,
                    LeftFootColor = playerCustomization.LeftFootColor,
                    RightFootColor = playerCustomization.RightFootColor,
                    VoicePitch = voicePitch
                };

                var controllerToken = new RagdollToken
                {
                    IsController = true,
                    Creature = dataName,
                    HeadGarb = playerCustomization.HeadGarb,
                    FaceGarb = playerCustomization.FaceGarb,
                    NeckGarb = playerCustomization.NeckGarb,
                    HipGarb = playerCustomization.HipGarb,
                    Face = playerCustomization.Face,
                    HeadColor = playerCustomization.HeadColor,
                    TorsoColor = playerCustomization.TorsoColor,
                    LeftHandColor = playerCustomization.LeftHandColor,
                    RightHandColor = playerCustomization.RightHandColor,
                    LeftFootColor = playerCustomization.LeftFootColor,
                    RightFootColor = playerCustomization.RightFootColor,
                    VoicePitch = voicePitch
                };

                bool isDissolve = false;

                if (enemyName == "the nuke")
                {
                    isDissolve = true;
                }

                for (int i = 0; i < HostPlayerRegistry.Instance.PlayerConnections.Count; i++)
                {
                    if (HostPlayerRegistry.Instance.PlayerConnections[i] == null)
                    {
                        continue;
                    }

                    if (HostPlayerRegistry.Instance.PlayerConnections[i].BoltConnection != entity.Controller)
                    {
                        BoltGlobalEvent.SendRagdoll(transform.position, transform.rotation, direction, isDissolve, clientToken, HostPlayerRegistry.Instance.PlayerConnections[i].BoltConnection);
                    }
                    else
                    {
                        BoltGlobalEvent.SendRagdoll(transform.position, transform.rotation, direction, isDissolve, controllerToken, HostPlayerRegistry.Instance.PlayerConnections[i].BoltConnection);
                    }
                }
            }

            entity.Controller.GetPlayerConnection().PlayerInfo.state.Deaths++;
            SendKillfeed(enemyName);
            HostPlayerRegistry.Instance.DestroyPlayer(entity.Controller.GetPlayerConnection());
        }

        private void SendKillfeed(string enemyName)
        {
            string finalMessage = (string.IsNullOrEmpty(enemyName)) ? state.PenName + " committed suicide." : enemyName + " killed " + state.PenName + ".";

            BoltGlobalEvent.SendMessage(finalMessage, Color.cyan);
        }

        public void ToggleTurretView(bool isActive)
        {
            view.SetActive(isActive);
        }

        private void OnScaleChange()
        {
            transform.localScale = new Vector3(state.playerScale, state.playerScale, state.playerScale);
        }

        public void StartBriefHealth()
        {
            Timing.KillCoroutines(briefHealthHandle);
            briefHealthHandle = Timing.RunCoroutine(DecreaseBriefHealth());
        }

        private IEnumerator<float> DecreaseBriefHealth()
        {
            while (state.BriefHealth > 0)
            {
                yield return Timing.WaitForSeconds(1.0f);

                if (this == null)
                {
                    yield break;
                }

                if (state.BriefHealth > 0)
                {
                    state.BriefHealth--;
                }
            }
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