using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Rescue")]
    public sealed class RescueData : ScriptableObject
    {
        [SerializeField] private int arrivalTime = 60;
        [SerializeField] private int standbyTime = 45;
        [SerializeField] private int departureTime = 10;

        public int ArrivalTime { get { return arrivalTime; } }
        public int StandbyTime { get { return standbyTime; } }
        public int DepartureTime { get { return departureTime; } }
    }
}