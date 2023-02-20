namespace Scram
{
    public interface IInteractable
    {
        string Display { get; }
        void Interact(Player player);
    }
}