using Photon.Bolt;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [ShowOdinSerializedPropertiesInInspector]
    public sealed class PlayerArmor : EntityBehaviour<IPlayerState>, ISerializationCallbackReceiver, ISupportsPrefabSerialization
    {
        [SerializeField] private Dictionary<string, GameObject> armors = null;

        private GameObject armor;

        public override void Attached()
        {
            var token = entity.AttachToken as PlayerToken;

            if (token == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(token.Armor))
            {
                armor = armors[token.Armor];
                armor.SetActive(true);
            }
        }

        public override void Detached()
        {
            if (armor != null)
            {
                armor.SetActive(false);
            }
        }

        #region Odin
        [SerializeField, HideInInspector]
        private SerializationData serializationData = default(SerializationData);

        SerializationData ISupportsPrefabSerialization.SerializationData { get { return serializationData; } set { serializationData = value; } }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            UnitySerializationUtility.SerializeUnityObject(this, ref serializationData);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnitySerializationUtility.DeserializeUnityObject(this, ref serializationData);
        }
        #endregion
    }
}