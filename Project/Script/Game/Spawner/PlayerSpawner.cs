using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private TeamData teamData = null;
        public Transform[] spawns = null;
        [SerializeField] private int respawnRate = 3;

        private List<Transform> newSpawns = new List<Transform>(64);

        private void Awake()
        {
            ResetSpawns();
        }

        private void Start()
        {
            if (HostGameAction.Instance != null)
            {
                HostGameAction.Instance.OnIntermissionEnd += () => Timing.KillCoroutines("Respawn");
                HostGameAction.Instance.OnRoundEnd += () => Timing.KillCoroutines("Respawn");
            }
        }

        public void ResetSpawns()
        {
            newSpawns.Clear();

            for (var i = 0; i < spawns.Length; i++)
            {
                newSpawns.Add(spawns[i]);
            }
        }

        public Player Spawn(PlayerConnection connection, Vector3 position = default(Vector3), float yaw = 0.0f)
        {
            if (connection == null || connection.PlayerInfo == null)
            {
                Debug.LogError(connection);

                return null;
            }

            HostPlayerRegistry.Instance.DestroyPlayer(connection);

            bool isIntermission = Game.Instance.MatchStatus == Game.MatchState.Intermission;
            var playerInfoState = connection.PlayerInfo.state;
            var creature = isIntermission ? string.Empty : playerInfoState.Creature;
            var randomValue = Random.value;

            if (string.IsNullOrEmpty(creature))
            {
                for (var i = 0; i < teamData.Creatures.Length; i++)
                {
                    if (randomValue <= teamData.Creatures[i].Chance)
                    {
                        creature = teamData.Creatures[i].ID;

                        break;
                    }
                }
            }

            Transform randomSpawn = newSpawns[Random.Range(0, newSpawns.Count)];

            if (position == default(Vector3))
            {
                newSpawns.Remove(randomSpawn);

                if (newSpawns.Count <= 0)
                {
                    ResetSpawns();
                }
            }

            var spawnYaw = (yaw != 0.0f) ? yaw : randomSpawn.rotation.eulerAngles.y;
            var spawnPosition = (position != default(Vector3)) ? position : randomSpawn.position;

            var playerToken = new PlayerToken
            {
                PenName = playerInfoState.PenName,
                Creature = creature,
                Gadget1 = isIntermission ? string.Empty : playerInfoState.Gadget1,
                Gadget2 = isIntermission ? string.Empty : playerInfoState.Gadget2,
                Gadget3 = isIntermission ? string.Empty : playerInfoState.Gadget3,
                Armor = isIntermission ? string.Empty : playerInfoState.Armor,
                TeamID = teamData.ID,
                TeamLayer = teamData.Layer,
                TeamColor = teamData.Color,
                Yaw = spawnYaw,
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
                HasSchristmasLicense = playerInfoState.License.HasSchristmasLicense,
                HasScracticalLicense= playerInfoState.License.HasScracticalLicense,
                VoicePitch = playerInfoState.VoicePitch
            };

            Player player = HostPlayerRegistry.Instance.InstantiatePlayer(playerToken, spawnPosition, Quaternion.identity, connection);

            if (player == null)
            {
                return null;
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

            return player;
        }

        public IEnumerator<float> SpawnWithDelay(PlayerConnection connection, Vector3 position = default(Vector3), float yaw = 0.0f)
        {
            double currentTime = TimeController.GetTime();

            yield return Timing.WaitUntilTrue(() =>
            {
                if (TimeController.GetTime(currentTime) >= respawnRate)
                {
                    return true;
                }

                return false;
            });

            if (this == null || connection == null)
            {
                yield break;
            }

            Spawn(connection, position, yaw);
        }
    }
}