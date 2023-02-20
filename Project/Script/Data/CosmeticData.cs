using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Cosmetic Data")]
    public sealed class CosmeticData : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<int, Item> defaultCosmetics = null;
        [SerializeField] private Dictionary<string, GameObject> inGameCosmetics = null;
        [SerializeField] private Dictionary<string, GameObject> scrammunismCosmetics = null;
        [SerializeField] private Dictionary<string, GameObject> scralloweenCosmetics = null;
        [SerializeField] private Dictionary<string, GameObject> schristmasCosmetics = null;
        [SerializeField] private Dictionary<string, GameObject> scracticalCosmetics = null;

        [SerializeField] private Dictionary<int, Texture> defaultFaces = null;
        [SerializeField] private Dictionary<string, Texture> scralloweenFaces = null;
        [SerializeField] private Dictionary<string, Texture> schristmasFaces = null;

        [SerializeField] private Dictionary<int, Voice> voices = null;

        public Dictionary<int, Item> DefaultCosmetics { get { return defaultCosmetics; } }
        public Dictionary<string, GameObject> InGameCosmetics { get { return inGameCosmetics; } }
        public Dictionary<string, GameObject> ScrammunismCosmetics { get { return scrammunismCosmetics; } }
        public Dictionary<string, GameObject> ScralloweenCosmetics { get { return scralloweenCosmetics; } }
        public Dictionary<string, GameObject> SchristmasCosmetics { get { return schristmasCosmetics; } }
        public Dictionary<string, GameObject> ScracticalCosmetics { get { return scracticalCosmetics; } }

        public Dictionary<int, Texture> DefaultFaces { get { return defaultFaces; } }
        public Dictionary<string, Texture> ScralloweenFaces { get { return scralloweenFaces; } }
        public Dictionary<string, Texture> SchristmasFaces { get { return schristmasFaces; } }

        public Dictionary<int, Voice> Voices { get { return voices; } }

        [Serializable]
        public class Item
        {
            [SerializeField] private GameObject prefab;
            [SerializeField] private BackpackSnippet.BodyAreaState bodyPart;

            public GameObject Prefab { get { return prefab; } }
            public BackpackSnippet.BodyAreaState BodyPart { get { return bodyPart; } }
            public string link = "https://developers.google.com/maps/documentation/maps-static/images/error-image-generic.png";
        }

        [Serializable]
        public class Voice
        {
            [SerializeField] private string display;
            [SerializeField] private float pitch;

            public string Display { get { return display; } }
            public float Pitch { get { return pitch; } }
        }
    }
}