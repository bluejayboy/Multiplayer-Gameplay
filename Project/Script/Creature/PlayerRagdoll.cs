using Photon.Bolt;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerRagdoll : MonoBehaviour
    {
        [SerializeField] private CreatureListData list;
        [SerializeField] private CosmeticData cosmeticData = null;
        [SerializeField] private Rigidbody rb = null;
        [SerializeField] private GameObject view = null;

        [SerializeField] private Transform head = null;
        [SerializeField] private Transform chest = null;
        [SerializeField] private Transform hip = null;
        [SerializeField] private SkinnedMeshRenderer avatar = null;
        [SerializeField] private GameObject avatarDissolve = null;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            if (ClientPlayerAction.Instance != null)
            {
                ClientPlayerAction.Instance.OnSpawn += Disable;
            }

            Stage.Instance.StageViewer.SetActive(false);
        }

        private void OnDestroy()
        {
            if (ClientPlayerAction.Instance != null)
            {
                ClientPlayerAction.Instance.OnSpawn -= Disable;
            }
        }

        public void Initialize(Vector3 direction, IProtocolToken token, bool isDissolve)
        {
            if (isDissolve)
            {
                avatar.enabled = false;
                avatarDissolve.SetActive(true);
            }
            else
            {
                avatar.enabled = true;
                avatarDissolve.SetActive(false);
            }

            rb.AddForce(10000.0f * direction);

            var ragdollToken = token as RagdollToken;

            if (!isDissolve)
            {
                ApplyClothing(ragdollToken);
                ApplySkinColors(ragdollToken);
            }

            PlayEffect(ragdollToken);

            if (ragdollToken.IsController)
            {
                view.SetActive(true);
            }
        }

        private void Disable()
        {
            view.SetActive(false);
        }

        private void PlayEffect(RagdollToken token)
        {
            CreatureData creature = list.Datas[token.Creature];
            audioSource.pitch = token.VoicePitch;

            if (Random.value < creature.ScreamChance)
            {
                audioSource.PlayAudioClip(creature.Screams[Random.Range(0, creature.Screams.Length)]);
            }
        }

        private void ApplyClothing(RagdollToken token)
        {
            CreatureData creature = list.Datas[token.Creature];

            for (var i = 0; i < creature.Hats.Length; i++)
            {
                if (cosmeticData.InGameCosmetics.ContainsKey(creature.Hats[i].name))
                {
                    var hat = Instantiate(cosmeticData.InGameCosmetics[creature.Hats[i].name], head);

                    hat.transform.localScale *= 5.5f;
                    hat.transform.localPosition = new Vector3(hat.transform.localPosition.x, hat.transform.localPosition.y + 0.004f, hat.transform.localPosition.z + 0.0035f);
                }
            }

            for (var i = 0; i < creature.Scarves.Length; i++)
            {
                if (cosmeticData.InGameCosmetics.ContainsKey(creature.Scarves[i].name))
                {
                    var hat = Instantiate(cosmeticData.InGameCosmetics[creature.Scarves[i].name], chest);

                    hat.transform.localScale *= 5.5f;
                    hat.transform.localPosition = new Vector3(hat.transform.localPosition.x, hat.transform.localPosition.y + 0.004f, hat.transform.localPosition.z + 0.0035f);
                }
            }

            for (var i = 0; i < creature.Vests.Length; i++)
            {
                if (cosmeticData.InGameCosmetics.ContainsKey(creature.Vests[i].name))
                {
                    var hat = Instantiate(cosmeticData.InGameCosmetics[creature.Vests[i].name], hip);

                    hat.transform.localScale *= 5.5f;
                    hat.transform.localPosition = new Vector3(hat.transform.localPosition.x, hat.transform.localPosition.y + 0.004f, hat.transform.localPosition.z + 0.0035f);
                }
            }

            CheckHats(cosmeticData.DefaultCosmetics, token);
            CheckHats(cosmeticData.ScrammunismCosmetics, token);
            CheckHats(cosmeticData.ScralloweenCosmetics, token);
            CheckHats(cosmeticData.SchristmasCosmetics, token);
            CheckHats(cosmeticData.ScracticalCosmetics, token);

            CheckFace(cosmeticData.DefaultFaces, token);
            CheckFace(cosmeticData.ScralloweenFaces, token);
            CheckFace(cosmeticData.SchristmasFaces, token);
        }

        private void CheckHats(Dictionary<int, CosmeticData.Item> dict, RagdollToken token)
        {
            int head;
            bool isValidHead = int.TryParse(token.HeadGarb, out head);

            int face;
            bool isValidFace = int.TryParse(token.FaceGarb, out face);

            int neck;
            bool isValidNeck = int.TryParse(token.NeckGarb, out neck);

            CreatureData creature = list.Datas[token.Creature];

            if (creature.Hats.Length <= 0 && isValidHead && dict.ContainsKey(head))
            {
                if (dict.ContainsKey(head))
                {
                    var hat = Instantiate(dict[head].Prefab, this.head);

                    hat.transform.localScale *= 5.5f;
                    hat.transform.localPosition = new Vector3(hat.transform.localPosition.x, hat.transform.localPosition.y + 0.004f, hat.transform.localPosition.z + 0.0035f);
                }
            }

            if (creature.Hats.Length <= 0 && isValidFace && dict.ContainsKey(face))
            {
                if (dict.ContainsKey(face))
                {
                    var hat = Instantiate(dict[face].Prefab, this.head);

                    hat.transform.localScale *= 5.5f;
                    hat.transform.localPosition = new Vector3(hat.transform.localPosition.x, hat.transform.localPosition.y + 0.004f, hat.transform.localPosition.z + 0.0035f);
                }
            }

            if (creature.Scarves.Length <= 0 && isValidNeck && dict.ContainsKey(neck))
            {
                if (dict.ContainsKey(neck))
                {
                    var hat = Instantiate(dict[neck].Prefab, this.head);

                    hat.transform.localScale *= 5.5f;
                    hat.transform.localPosition = new Vector3(hat.transform.localPosition.x, hat.transform.localPosition.y + 0.004f, hat.transform.localPosition.z + 0.0035f);
                }
            }
        }

        private void CheckHats(Dictionary<string, GameObject> dict, RagdollToken token)
        {
            CreatureData creature = list.Datas[token.Creature];

            if (creature.Hats.Length <= 0 && !string.IsNullOrEmpty(token.HeadGarb) && dict.ContainsKey(token.HeadGarb))
            {
                var hat = Instantiate(dict[token.HeadGarb], head);

                hat.transform.localScale *= 5.5f;
                hat.transform.localPosition = new Vector3(hat.transform.localPosition.x, hat.transform.localPosition.y + 0.004f, hat.transform.localPosition.z + 0.0035f);
            }

            if (creature.Hats.Length <= 0 && !string.IsNullOrEmpty(token.FaceGarb) && dict.ContainsKey(token.FaceGarb))
            {
                var hat = Instantiate(dict[token.FaceGarb], head);

                hat.transform.localScale *= 5.5f;
                hat.transform.localPosition = new Vector3(hat.transform.localPosition.x, hat.transform.localPosition.y + 0.004f, hat.transform.localPosition.z + 0.0035f);
            }

            if (creature.Scarves.Length <= 0 && !string.IsNullOrEmpty(token.NeckGarb) && dict.ContainsKey(token.NeckGarb))
            {
                var hat = Instantiate(dict[token.NeckGarb], chest);

                hat.transform.localScale *= 5.5f;
                hat.transform.localPosition = new Vector3(hat.transform.localPosition.x, hat.transform.localPosition.y + 0.004f, hat.transform.localPosition.z + 0.0035f);
            }
        }

        private void CheckFace(Dictionary<int, Texture> dict, RagdollToken token)
        {
            int face;
            bool isValidFace = int.TryParse(token.Face, out face);

            if (isValidFace)
            {
                if (dict.ContainsKey(face))
                {
                    avatar.materials[2].mainTexture = dict[face];
                }
            }
        }

        private void CheckFace(Dictionary<string, Texture> dict, RagdollToken token)
        {
            if (!string.IsNullOrEmpty(token.Face))
            {
                if (dict.ContainsKey(token.Face))
                {
                    avatar.materials[2].mainTexture = dict[token.Face];
                }
            }
        }

        private void ApplySkinColors(RagdollToken token)
        {
            SetColor(6, token.HeadColor);
            SetColor(4, token.TorsoColor);
            SetColor(1, token.LeftHandColor);
            SetColor(0, token.RightHandColor);
            SetColor(3, token.LeftFootColor);
            SetColor(5, token.RightFootColor);
        }

        private Color32 SetColor(int index, Color32 color)
        {
            avatar.materials[index].SetColor("_Color", color);

            return color;
        }
    }
}