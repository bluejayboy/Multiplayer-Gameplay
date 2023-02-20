using Photon.Bolt;

namespace Scram
{
    public sealed class PlayerConnection
    {
        public BoltConnection BoltConnection { get; set; }
        public PlayerInfo PlayerInfo { get; set; }
        public Player Player { get; set; }
    }
}