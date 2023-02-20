using Photon.Bolt;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Scram
{
    [DisallowMultipleComponent]
    [ShowOdinSerializedPropertiesInInspector]
    public sealed class PlayerEquipment : EntityBehaviour<IPlayerState>, ISerializationCallbackReceiver, ISupportsPrefabSerialization
    {
        [SerializeField] private Transform[] parents;
        [SerializeField] private Dictionary<string, GameObject> equipments = null;
        [SerializeField] private GameObject[] equipment1 = null;
        [SerializeField] private GameObject[] equipment2 = null;

        private List<Renderer> renderers = new List<Renderer>();

        private void Awake()
        {
            for (var i = 0; i < parents.Length; i++)
            {
                var children = parents[i].GetComponentsInChildren<Renderer>(true);

                for (var j = 0; j < children.Length; j++)
                {
                    renderers.Add(children[j]);
                }
            }
        }

        public override void Attached()
        {
            state.AddCallback("OwnedGadgets[].ID", EditOwnedGadget);
            state.AddCallback("ActiveGadget.Slot", EditActiveGadget);
        }

        public override void Detached()
        {
            for (var i = 0; i < parents.Length; i++)
            {
                for (var j = 0; j < parents[i].childCount; j++)
                {
                    parents[i].GetChild(j).gameObject.SetActive(false);
                }
            }
        }

        public override void ControlGained()
        {
            for (var i = 0; i < renderers.Count; i++)
            {
                renderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }

        public override void ControlLost()
        {
            for (var i = 0; i < renderers.Count; i++)
            {
                renderers[i].shadowCastingMode = ShadowCastingMode.On;
            }
        }

        private void EditOwnedGadget(IState state, string path, ArrayIndices indices)
        {
            var playerState = state as IPlayerState;
            int index = indices[0];
            string key = playerState.OwnedGadgets[index].ID;

            if (!string.IsNullOrEmpty(key))
            {
                if (equipments.ContainsKey(key) && playerState.ActiveGadget.Slot != index)
                {
                    var eq = index == 1 ? equipment1 : equipment2;

                    for (int i = 0; i < eq.Length; i++)
                    {
                        eq[i].SetActive(false);
                    }

                    equipments[key].SetActive(true);
                }
            }
            else
            {
                if (equipments.ContainsKey(key))
                {
                    equipments[key].SetActive(false);
                }
            }
        }

        private void EditActiveGadget()
        {
            for (int i = 0; i < state.OwnedGadgets.Length; i++)
            {
                string key = state.OwnedGadgets[i].ID;

                if (!string.IsNullOrEmpty(key) && equipments.ContainsKey(key))
                {
                    if (i == state.ActiveGadget.Slot)
                    {
                        equipments[key].SetActive(false);
                    }
                    else
                    {
                        equipments[key].SetActive(true);
                    }
                }
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