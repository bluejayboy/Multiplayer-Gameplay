namespace Scram
{
    public interface IPrimaryDown
    {
        GadgetData Data { get; }
        void PrimaryDown();
        void PlayPrimaryDown();
    }
}