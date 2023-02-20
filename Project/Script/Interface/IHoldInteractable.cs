namespace Scram
{
    public interface IHoldInteractable : IInteractable
    {
        Player ActivePlayer { get; set; }
        void Uninteract();
    }
}