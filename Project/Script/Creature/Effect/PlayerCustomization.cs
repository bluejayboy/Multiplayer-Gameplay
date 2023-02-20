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
    public sealed class PlayerCustomization : EntityBehaviour<IPlayerState>, ISerializationCallbackReceiver, ISupportsPrefabSerialization
    {
        [SerializeField] private CosmeticData cosmeticData = null;
        [SerializeField] private Transform head = null;
        [SerializeField] private Transform chest = null;
        [SerializeField] private Transform hip = null;
        [SerializeField] private SkinnedMeshRenderer avatar = null;

        private Player player = null;
        private CreatureData creatureData = null;
        private FaceAnimator faceAnimator = null;

        private List<GameObject> activeCosmetics = new List<GameObject>();

        public string HeadGarb { get; set; }
        public string FaceGarb { get; set; }
        public string NeckGarb { get; set; }
        public string HipGarb { get; set; }
        public string Face { get; set; }

        public Color32 HeadColor { get; set; }
        public Color32 TorsoColor { get; set; }
        public Color32 LeftHandColor { get; set; }
        public Color32 RightHandColor { get; set; }
        public Color32 LeftFootColor { get; set; }
        public Color32 RightFootColor { get; set; }

        private void Awake()
        {
            player = GetComponent<Player>();
            faceAnimator = avatar.GetComponent<FaceAnimator>();
        }

        public override void Attached()
        {
            var token = entity.AttachToken as PlayerToken;

            if (token == null)
            {
                return;
            }

            creatureData = player.List.Datas[token.Creature];

            ApplyClothing(token);
            ApplySkinColors(token);
        }

        public override void Detached()
        {
            for (var i = 0; i < activeCosmetics.Count; i++)
            {
                Destroy(activeCosmetics[i]);
            }

            faceAnimator.Run();
        }

        public override void ControlGained()
        {
            var headRenderers = head.GetComponentsInChildren<Renderer>();
            var chestRenderers = chest.GetComponentsInChildren<Renderer>();
            var hipRenderers = hip.GetComponentsInChildren<Renderer>();
            var lights = head.GetComponentsInChildren<Light>();

            for (var i = 0; i < headRenderers.Length; i++)
            {
                headRenderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }

            for (var i = 0; i < chestRenderers.Length; i++)
            {
                chestRenderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }

            for (var i = 0; i < hipRenderers.Length; i++)
            {
                hipRenderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }

            for (var i = 0; i < lights.Length; i++)
            {
                lights[i].enabled = false;
            }
        }

        private void ApplyClothing(PlayerToken token)
        {
            HeadGarb = FaceGarb = NeckGarb = HipGarb = Face = string.Empty;

            for (var i = 0; i < creatureData.Hats.Length; i++)
            {
                if (cosmeticData.InGameCosmetics.ContainsKey(creatureData.Hats[i].name))
                {
#if !ISDEDICATED
                    activeCosmetics.Add(Instantiate(cosmeticData.InGameCosmetics[creatureData.Hats[i].name], head));
#endif

                    HeadGarb = creatureData.Hats[i].name;
                }
            }

            for (var i = 0; i < creatureData.Scarves.Length; i++)
            {
                if (cosmeticData.InGameCosmetics.ContainsKey(creatureData.Scarves[i].name))
                {
#if !ISDEDICATED
                    activeCosmetics.Add(Instantiate(cosmeticData.InGameCosmetics[creatureData.Scarves[i].name], chest));
#endif

                    NeckGarb = creatureData.Scarves[i].name;
                }
            }

            for (var i = 0; i < creatureData.Vests.Length; i++)
            {
                if (cosmeticData.InGameCosmetics.ContainsKey(creatureData.Vests[i].name))
                {
#if !ISDEDICATED
                    activeCosmetics.Add(Instantiate(cosmeticData.InGameCosmetics[creatureData.Vests[i].name], hip));
#endif

                    NeckGarb = creatureData.Vests[i].name;
                }
            }

            int face;
            bool isValidFace = int.TryParse(creatureData.Face, out face);

            if (isValidFace)
            {
                if (cosmeticData.DefaultFaces.ContainsKey(face))
                {
#if !ISDEDICATED
                    faceAnimator.Stop();
                    avatar.materials[2].mainTexture = cosmeticData.DefaultFaces[face];
#endif

                    Face = creatureData.Face;
                }
            }

            CheckHats(cosmeticData.DefaultCosmetics, token);
            CheckFace(cosmeticData.DefaultFaces, token);

            if (token.HasCommunismLicense)
            {
                CheckHats(cosmeticData.ScrammunismCosmetics, token);
            }

            if (token.HasScralloweenLicense)
            {
                CheckHats(cosmeticData.ScralloweenCosmetics, token);
                CheckFace(cosmeticData.ScralloweenFaces, token);
            }

            if (token.HasSchristmasLicense)
            {
                CheckHats(cosmeticData.SchristmasCosmetics, token);
                CheckFace(cosmeticData.SchristmasFaces, token);
            }

            if (token.HasScracticalLicense)
            {
                CheckHats(cosmeticData.ScracticalCosmetics, token);
            }
        }

        private void CheckHats(Dictionary<int, CosmeticData.Item> dict, PlayerToken token)
        {
            int head;
            bool isValidHead = int.TryParse(token.HeadGarb, out head);

            int face;
            bool isValidFace = int.TryParse(token.FaceGarb, out face);

            int neck;
            bool isValidNeck = int.TryParse(token.NeckGarb, out neck);

            if (creatureData.Hats.Length <= 0 && isValidHead && dict.ContainsKey(head))
            {
#if !ISDEDICATED
                activeCosmetics.Add(Instantiate(dict[head].Prefab, this.head));
#endif

                HeadGarb = token.HeadGarb;
            }

            if (creatureData.Hats.Length <= 0 && isValidFace && dict.ContainsKey(face))
            {
#if !ISDEDICATED
                activeCosmetics.Add(Instantiate(dict[face].Prefab, this.head));
#endif

                FaceGarb = token.FaceGarb;
            }

            if (creatureData.Scarves.Length <= 0 && isValidNeck && dict.ContainsKey(neck))
            {
#if !ISDEDICATED
                activeCosmetics.Add(Instantiate(dict[neck].Prefab, chest));
#endif

                NeckGarb = token.NeckGarb;
            }
        }

        private void CheckHats(Dictionary<string, GameObject> dict, PlayerToken token)
        {
            if (creatureData.Hats.Length > 0 || creatureData.Scarves.Length > 0)
            {
                return;
            }

            if (creatureData.Hats.Length <= 0 && !string.IsNullOrEmpty(token.HeadGarb) && dict.ContainsKey(token.HeadGarb))
            {
#if !ISDEDICATED
                activeCosmetics.Add(Instantiate(dict[token.HeadGarb], head));
#endif

                HeadGarb = token.HeadGarb;
            }

            if (creatureData.Hats.Length <= 0 && !string.IsNullOrEmpty(token.FaceGarb) && dict.ContainsKey(token.FaceGarb))
            {
#if !ISDEDICATED
                activeCosmetics.Add(Instantiate(dict[token.FaceGarb], head));
#endif

                FaceGarb = token.FaceGarb;
            }

            if (creatureData.Scarves.Length <= 0 && !string.IsNullOrEmpty(token.NeckGarb) && dict.ContainsKey(token.NeckGarb))
            {
#if !ISDEDICATED
                activeCosmetics.Add(Instantiate(dict[token.NeckGarb], chest));
#endif

                NeckGarb = token.NeckGarb;
            }
        }

        private void CheckFace(Dictionary<int, Texture> dict, PlayerToken token)
        {
            if (!string.IsNullOrEmpty(creatureData.Face))
            {
                return;
            }

            int face;
            bool isValidFace = int.TryParse(token.Face, out face);

            if (isValidFace && dict.ContainsKey(face))
            {
#if !ISDEDICATED
                faceAnimator.Stop();
                avatar.materials[2].mainTexture = dict[face];
#endif

                Face = token.Face;
            }
        }

        private void CheckFace(Dictionary<string, Texture> dict, PlayerToken token)
        {
            if (!string.IsNullOrEmpty(creatureData.Face))
            {
                return;
            }

            if (!string.IsNullOrEmpty(token.Face))
            {
                if (dict.ContainsKey(token.Face))
                {
#if !ISDEDICATED
                    faceAnimator.Stop();
                    avatar.materials[2].mainTexture = dict[token.Face];
#endif

                    Face = token.Face;
                }
            }
        }

        private void ApplySkinColors(PlayerToken token)
        {
            if (creatureData.IsFixedSkinColor)
            {
                HeadColor = SetColor(6, creatureData.Head);
                TorsoColor = SetColor(4, creatureData.Torso);
                LeftHandColor = SetColor(1, creatureData.LeftHand);
                RightHandColor = SetColor(0, creatureData.RightHand);
                LeftFootColor = SetColor(3, creatureData.LeftFoot);
                RightFootColor = SetColor(5, creatureData.RightFoot);
            }
            else
            {
                HeadColor = SetColor(6, token.HeadColor);
                TorsoColor = SetColor(4, token.TorsoColor);
                LeftHandColor = SetColor(1, token.LeftHandColor);
                RightHandColor = SetColor(0, token.RightHandColor);
                LeftFootColor = SetColor(3, token.LeftFootColor);
                RightFootColor = SetColor(5, token.RightFootColor);
            }
        }

        private Color32 SetColor(int index, Color32 color)
        {
            if (!creatureData.IsFixedSkinColor && !ColorCheck.Instance.DefaultColors.Contains(color))
            {
                color = ColorCheck.Instance.DefaultColors[0];
            }

#if !ISDEDICATED
            avatar.materials[index].SetColor("_Color", color);
#endif

            return color;
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