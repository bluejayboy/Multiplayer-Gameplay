using UnityEngine;

namespace Photon.Bolt
{
    public static class BoltGlobalEvent
    {
        public static void SendIgnoreCollider(bool isIgnore, NetworkId player, NetworkId collider, BoltConnection target)
        {
            if (target == null)
            {
                return;
            }

            var evnt = IgnoreColliderEvent.Create(target, ReliabilityModes.ReliableOrdered);

            evnt.IsIgnore = isIgnore;
            evnt.Player = player;
            evnt.Collider = collider;
            evnt.Send();
        }

        public static void SendSuicideExplosion(Vector3 position, Quaternion rotation)
        {
            var evnt = SuicideExplosionEvent.Create(GlobalTargets.Everyone, ReliabilityModes.Unreliable);

            evnt.Position = position;
            evnt.Rotation = rotation;
            evnt.Send();
        }

        public static void SendRagdoll(Vector3 position, Quaternion rotation, Vector3 direction, bool isDissolve, IProtocolToken token, BoltConnection target)
        {
            var evnt = (target != null) ? RagdollEvent.Create(target, ReliabilityModes.ReliableOrdered) : RagdollEvent.Create(GlobalTargets.OnlySelf, ReliabilityModes.ReliableOrdered);

            evnt.Position = position;
            evnt.Rotation = rotation;
            evnt.Direction = direction;
            evnt.IsDissolve = isDissolve;
            evnt.Token = token;
            evnt.Send();
        }

        public static void SendGameItem(string id)
        {
            var evnt = IngameItemEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered);

            evnt.ID = id;
            evnt.Send();
        }

        public static void SendBread(int bread, BoltConnection target)
        {
            var evnt = (target != null) ? BreadEvent.Create(target, ReliabilityModes.Unreliable) : BreadEvent.Create(GlobalTargets.OnlySelf, ReliabilityModes.Unreliable);

            evnt.Bread = bread;
            evnt.Send();
        }

        public static void SendCue(string text, Color32 color)
        {
            var evnt = CueEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);

            evnt.Text = text;
            evnt.Color = color;
            evnt.Send();
        }

        public static void SendCue(string text, Color32 color, BoltConnection target)
        {
            var evnt = (target != null) ? CueEvent.Create(target, ReliabilityModes.ReliableOrdered) : CueEvent.Create(GlobalTargets.OnlySelf, ReliabilityModes.ReliableOrdered);

            evnt.Text = text;
            evnt.Color = color;
            evnt.Send();
        }

        public static void SendInteract(NetworkId networkID)
        {
            var evnt = InteractEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered);

            evnt.NetworkID = networkID;
            evnt.Send();
        }

        public static void SendPickup(string name, int slot, int activeAmmo, int maxAmmo, BoltEntity entity, bool willEquip)
        {
            if (entity.Controller == null)
            {
                return;
            }

            var evnt = PickupEvent.Create(entity.Controller, ReliabilityModes.ReliableOrdered);

            evnt.ID = name;
            evnt.Slot = slot;
            evnt.ActiveAmmo = activeAmmo;
            evnt.MaxAmmo = maxAmmo;
            evnt.NetworkID = entity.NetworkId;
            evnt.WillEquip = willEquip;
            evnt.Send();
        }

        public static void SendDrop(int slot, NetworkId networkID)
        {
            var evnt = DropEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered);

            evnt.Slot = slot;
            evnt.NetworkID = networkID;
            evnt.Send();
        }

        public static void SendChat(string text, Color32 color, bool isTeam)
        {
            var evnt = ChatEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered);

            evnt.Text = text;
            evnt.Color = color;
            evnt.IsTeam = isTeam;
            evnt.Send();
        }

        public static void SendMessage(string text, Color32 color)
        {
            var evnt = MessageEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);

            evnt.Text = text;
            evnt.Color = color;
            evnt.Send();
        }

        public static void SendPrivateMessage(string text, Color32 color, BoltConnection connection)
        {
            MessageEvent evnt;

            if (connection == null)
            {
                evnt = MessageEvent.Create(GlobalTargets.OnlySelf, ReliabilityModes.ReliableOrdered);
            }
            else
            {
                evnt = MessageEvent.Create(connection, ReliabilityModes.ReliableOrdered);
            }

            evnt.Text = text;
            evnt.Color = color;
            evnt.Send();
        }

        public static void SendMessage(string text, Color32 color, BoltConnection target)
        {
            var evnt = (target != null) ? MessageEvent.Create(target, ReliabilityModes.ReliableOrdered) : MessageEvent.Create(GlobalTargets.OnlySelf, ReliabilityModes.ReliableOrdered);

            evnt.Text = text;
            evnt.Color = color;
            evnt.Send();
        }

        public static void SendObjectiveEvent(string text, string audioClip, Color32 color)
        {
            var evnt = ObjectiveEvent.Create(GlobalTargets.Everyone, ReliabilityModes.ReliableOrdered);

            evnt.Text = text;
            evnt.AudioClip = audioClip;
            evnt.Color = color;
            evnt.Send();
        }

        public static void SendObjectiveEvent(string text, string audioClip, Color32 color, BoltConnection target)
        {
            var evnt = (target != null) ? ObjectiveEvent.Create(target, ReliabilityModes.ReliableOrdered) : ObjectiveEvent.Create(GlobalTargets.OnlySelf, ReliabilityModes.ReliableOrdered);

            evnt.Text = text;
            evnt.AudioClip = audioClip;
            evnt.Color = color;
            evnt.Send();
        }

        public static void SendSpectate(int playerIndex, NetworkId networkID, BoltConnection target)
        {
            var evnt = (target != null) ? SpectateEvent.Create(target, ReliabilityModes.ReliableOrdered) : SpectateEvent.Create(GlobalTargets.OnlySelf, ReliabilityModes.ReliableOrdered);

            evnt.PlayerIndex = playerIndex;
            evnt.NetworkID = networkID;
            evnt.Send();
        }

        public static void SendSpectateRequest(int index, bool hasDelay)
        {
            var evnt = SpectateRequestEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered);

            evnt.PlayerIndex = index;
            evnt.HasDelay = hasDelay;
            evnt.Send();
        }

        public static void SendSuicide()
        {
            SuicideEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered).Send();
        }
    }
}