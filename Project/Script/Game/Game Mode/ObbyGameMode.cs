using MEC;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class ObbyGameMode : GameMode
    {
        public static ObbyGameMode ChildInstance { get; private set; }

        [SerializeField] private TeamData teamData;
        [SerializeField] private Transform[] levels;
        [SerializeField] private Dictionary<PlayerConnection, Transform> players = new Dictionary<PlayerConnection, Transform>(5);

        protected override void Awake()
        {
            base.Awake();

            ChildInstance = this;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ChildInstance = null;
        }

        protected override void Start()
        {
            base.Start();

            if (BoltNetwork.IsServer)
            {
                HostPlayerAction.Instance.OnDeath = OnPlayerDeath;
                Game.Instance.MatchStatus = Game.MatchState.MidRound;
            }
        }

        public void GoToLevel(int level, PlayerConnection connection)
        {
            players[connection] = levels[level];
        }

        public void Teleport(PlayerConnection connection)
        {
            connection.Player.transform.position = players[connection].position;
        }

        public override void ConnectedDuringPlay(PlayerConnection connection)
        {
            players.Add(connection, levels[0]);
            Spawn(connection);
        }

        protected override void OnPlayerDeath(PlayerConnection connection, Vector3 position, float yaw)
        {
            if (BoltNetwork.IsClient)
            {
                return;
            }

            Timing.RunCoroutine(SpawnWithDelay(connection), "Respawn");
        }

        public void Spawn(PlayerConnection connection)
        {
            HostPlayerRegistry.Instance.DestroyPlayer(connection);
            var playerInfoState = connection.PlayerInfo.state;

            var playerToken = new PlayerToken
            {
                PenName = playerInfoState.PenName,
                Creature = "Civilian",
                Gadget1 = "",
                Armor = "",
                TeamID = teamData.ID,
                TeamLayer = teamData.Layer,
                TeamColor = teamData.Color,
                Yaw = 0.0f,
                Face = playerInfoState.Face,
                HeadGarb = playerInfoState.Garb.Head,
                FaceGarb = playerInfoState.Garb.Face,
                NeckGarb = playerInfoState.Garb.Neck,
                HeadColor = playerInfoState.SkinColor.Head,
                TorsoColor = playerInfoState.SkinColor.Torso,
                LeftHandColor = playerInfoState.SkinColor.LeftHand,
                RightHandColor = playerInfoState.SkinColor.RightHand,
                LeftFootColor = playerInfoState.SkinColor.LeftFoot,
                RightFootColor = playerInfoState.SkinColor.RightFoot,
                HasCommunismLicense = playerInfoState.License.HasCommunismLicense,
                HasScralloweenLicense = playerInfoState.License.HasScralloweenLicense,
                HasSchristmasLicense = playerInfoState.License.HasSchristmasLicense
            };

            Player player = HostPlayerRegistry.Instance.InstantiatePlayer(playerToken, players[connection].position, Quaternion.identity, connection);

            if (player == null)
            {
                return;
            }

            if (connection.BoltConnection != null)
            {
                player.entity.AssignControl(connection.BoltConnection, playerToken);
            }
            else
            {
                player.entity.TakeControl(playerToken);
            }

            if (HostPlayerAction.Instance != null && HostPlayerAction.Instance.OnSpawn != null)
            {
                HostPlayerAction.Instance.OnSpawn(player);
            }
        }

        public IEnumerator<float> SpawnWithDelay(PlayerConnection connection, Vector3 position = default(Vector3), float yaw = 0.0f)
        {
            double currentTime = TimeController.GetTime();

            yield return Timing.WaitUntilTrue(() =>
            {
                if (TimeController.GetTime(currentTime) >= 2.0f)
                {
                    return true;
                }

                return false;
            });

            if (this == null || connection == null)
            {
                yield break;
            }

            Spawn(connection);
        }
    }
}