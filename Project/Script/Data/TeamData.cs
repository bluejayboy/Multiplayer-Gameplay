using System;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Team")]
    public sealed class TeamData : ScriptableObject
    {
        [SerializeField] private string id = "Civilians";
        public string ID { get { return id; } }

        [SerializeField] private string layer = "Team A";
        public string Layer { get { return layer; } }

        [SerializeField] private string cue = "You are a Civilian.";
        public string Cue { get { return cue; } }

        [SerializeField] private Color32 color = default(Color32);
        public Color32 Color { get { return color; } }

        [SerializeField] [Range(0.0f, 1.0f)] private float chance = 0.5f;
        public float Chance { get { return chance; } }

        [SerializeField] private Creature[] creatures = null;
        public Creature[] Creatures { get { return creatures; } }

        [Serializable]
        public struct Creature
        {
            [SerializeField] private string id;
            public string ID { get { return id; } }

            [SerializeField] [Range(0.0f, 1.0f)] private float chance;
            public float Chance { get { return chance; } }
        }
    }
}