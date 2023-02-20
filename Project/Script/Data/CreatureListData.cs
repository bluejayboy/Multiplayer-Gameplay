using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Creature List Data")]
    public sealed class CreatureListData : SerializedScriptableObject
    {
        [SerializeField] private Dictionary<string, CreatureData> datas = null;
        public Dictionary<string, CreatureData> Datas { get { return datas; } }
    }
}