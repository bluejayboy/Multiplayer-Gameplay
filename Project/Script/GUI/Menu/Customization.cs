using CodeStage.AntiCheat.ObscuredTypes;
using Sirenix.OdinInspector;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Customization : SerializedMonoBehaviour
    {
        [SerializeField] private BackpackSnippet bpSnippet = null;
        [SerializeField] private SkinnedMeshRenderer avatar = null;
        [SerializeField] private Texture defaultFace = null;
        [SerializeField] private Transform head = null;
        [SerializeField] private Transform chest = null;
        [SerializeField] private Transform content = null;
        [SerializeField] private Transform contentHolder = null;

        [SerializeField] private CosmeticData cosmeticData = null;
        [SerializeField] private List<Item> scrammunismItems = null;
        [SerializeField] private List<Item> scralloweenItems = null;
        [SerializeField] private List<Item> schristmasItems = null;
        [SerializeField] private List<Item> scracticalItems = null;
        [SerializeField] private BodyPartColor[] bodyParts = null;
        [SerializeField] private InputField inventorySearch;
        [SerializeField] private List<GameObject> inventory = new List<GameObject>();
        [SerializeField] private Texture errorTexture;

        private GameObject activeHeadGarb = null;
        private GameObject activeFaceGarb = null;
        private GameObject activeNeckGarb = null;
        private Texture activeFace = null;

        private byte[] m_SerializedBuffer;

        [Serializable]
        private class Item
        {
            public BackpackSnippet.BodyAreaState area;
            public string ID;
            public string display;
            public string Link = "error";
        }

        IEnumerator GetItemIcon(int itemID, RawImage iconImage){
            if(!Directory.Exists(Application.persistentDataPath + "/inventoryCache"))
                Directory.CreateDirectory(Application.persistentDataPath + "/inventoryCache");

            if(File.Exists(Application.persistentDataPath + "/inventoryCache/item" + itemID + ".scramicon")){
                StartCoroutine(UpdateItemIcon("", iconImage, true, itemID));
            }else{
                UnityWebRequest request = UnityWebRequest.Get("https://scramgame.com/api/game/items/" + itemID + "/icon");
                yield return request.Send();
                
                if(request.isHttpError || request.isNetworkError){
                    StartCoroutine(UpdateItemIcon("error", iconImage));
                }else{
                    try{
                        //InventoryIcon icon = JsonConvert.DeserializeObject<InventoryIcon>(request.downloadHandler.text.Replace("\\", ""));
                        if(activeFace ){
                            //StartCoroutine(UpdateItemIcon(icon.response, iconImage, false, itemID));
                        }else{
                            StartCoroutine(UpdateItemIcon("error", iconImage));
                        }
                    }catch{
                        StartCoroutine(UpdateItemIcon("error", iconImage));
                    }
                }
            }
        }

        IEnumerator UpdateItemIcon(string url, RawImage icon, bool isCache = false, int itemID = 0){
            if(isCache){
                string data = File.ReadAllText(Application.persistentDataPath + "/inventoryCache/item" + itemID + ".scramdata");
                Texture2D tex = new Texture2D(int.Parse(data.Split('|')[0]), int.Parse(data.Split('|')[1]));
                bool ableToLoad = tex.LoadImage(File.ReadAllBytes(Application.persistentDataPath + "/inventoryCache/item" + itemID + ".scramicon"));
                icon.texture = tex;
            }else{
                if(url == "error"){
                    icon.texture = errorTexture;
                    yield break;
                }

                UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
                yield return request.Send();
                
                if(!request.isHttpError && !request.isNetworkError){
                    icon.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    File.WriteAllBytes(Application.persistentDataPath + "/inventoryCache/item" + itemID + ".scramicon", ((DownloadHandlerTexture)request.downloadHandler).texture.EncodeToPNG());
                    File.WriteAllText(Application.persistentDataPath + "/inventoryCache/item" + itemID + ".scramdata", ((DownloadHandlerTexture)request.downloadHandler).texture.width + "|" + ((DownloadHandlerTexture)request.downloadHandler).texture.height);
                }
            }
        }

        public void SearchUpdate(){
            foreach(Transform t in content){
                if(!inventory.Contains(t.gameObject))
                    inventory.Add(t.gameObject);
                t.SetParent(contentHolder);
            }

            inventory.Sort((x, y) => x.name.CompareTo(y.name));

            for(int i = 0; i < inventory.Count; i++){
                bool enable = inventory[i].GetComponent<BackpackSnippet>().Display.text.ToLower().Contains(inventorySearch.text.ToLower());

                inventory[i].SetActive(enable);

                if(enable){
                    inventory[i].transform.SetParent(content);
                }
            }
        }

        private void Awake()
        {
            if (!ObscuredPrefs.GetBool("IsReset"))
            {
                for (var i = 0; i < bodyParts.Length; i++)
                {
                    ObscuredPrefs.SetColor(bodyParts[i].name, bodyParts[i].defaultColor);
                }

                ObscuredPrefs.SetString("Face", string.Empty);
                ObscuredPrefs.SetString("Head Garb", string.Empty);
                ObscuredPrefs.SetString("Face Garb", string.Empty);
                ObscuredPrefs.SetString("Neck Garb", string.Empty);

                ObscuredPrefs.SetBool("IsReset", true);
            }

            if (!ObscuredPrefs.HasKey("Voice Pitch"))
            {
                ObscuredPrefs.SetFloat("Voice Pitch", 1.5f);
            }
        }

        private void Start()
        {
            InitializePrefs();
            ApplyCustomization();
        }

        private void InitializePrefs()
        {
            for (var i = 0; i < bodyParts.Length; i++)
            {
                if (!ObscuredPrefs.HasKey(bodyParts[i].name))
                {
                    ObscuredPrefs.SetColor(bodyParts[i].name, bodyParts[i].defaultColor);
                }
            }
        }

        public void ApplyCustomization()
        {
            SetColor(6, ObscuredPrefs.GetColor("Head Color"));
            SetColor(4, ObscuredPrefs.GetColor("Torso Color"));
            SetColor(1, ObscuredPrefs.GetColor("Left Hand Color"));
            SetColor(0, ObscuredPrefs.GetColor("Right Hand Color"));
            SetColor(3, ObscuredPrefs.GetColor("Left Foot Color"));
            SetColor(5, ObscuredPrefs.GetColor("Right Foot Color"));

            if (activeHeadGarb != null)
            {
                Destroy(activeHeadGarb);
            }

            if (activeFaceGarb != null)
            {
                Destroy(activeFaceGarb);
            }

            if (activeNeckGarb != null)
            {
                Destroy(activeNeckGarb);
            }

            avatar.GetComponent<FaceAnimator>().Run();
            avatar.materials[2].mainTexture = defaultFace;

            int face;
            bool isValidFace = int.TryParse(ObscuredPrefs.GetString("Face"), out face);

            if (isValidFace && cosmeticData.DefaultFaces.ContainsKey(face))
            {
                avatar.GetComponent<FaceAnimator>().Stop();
                avatar.materials[2].mainTexture = cosmeticData.DefaultFaces[face];
            }

            if (cosmeticData.ScralloweenFaces.ContainsKey(ObscuredPrefs.GetString("Face")))
            {
                avatar.GetComponent<FaceAnimator>().Stop();
                avatar.materials[2].mainTexture = cosmeticData.ScralloweenFaces[ObscuredPrefs.GetString("Face")];
            }

            if (cosmeticData.SchristmasFaces.ContainsKey(ObscuredPrefs.GetString("Face")))
            {
                avatar.GetComponent<FaceAnimator>().Stop();
                avatar.materials[2].mainTexture = cosmeticData.SchristmasFaces[ObscuredPrefs.GetString("Face")];
            }

            CheckCosmetic(cosmeticData.DefaultCosmetics);
            CheckCosmetic(cosmeticData.ScrammunismCosmetics);
            CheckCosmetic(cosmeticData.ScralloweenCosmetics);
            CheckCosmetic(cosmeticData.SchristmasCosmetics);
            CheckCosmetic(cosmeticData.ScracticalCosmetics);
        }

        private void CheckCosmetic(Dictionary<int, CosmeticData.Item> dict)
        {
            int head;
            bool isValidHead = int.TryParse(ObscuredPrefs.GetString("Head Garb"), out head);

            if (isValidHead && dict.ContainsKey(head))
            {
                activeHeadGarb = Instantiate(dict[head].Prefab, this.head);
            }

            int face;
            bool isValidFace = int.TryParse(ObscuredPrefs.GetString("Face Garb"), out face);

            if (isValidFace && dict.ContainsKey(face))
            {
                activeFaceGarb = Instantiate(dict[face].Prefab, this.head);
            }

            int neck;
            bool isValidNeck = int.TryParse(ObscuredPrefs.GetString("Neck Garb"), out neck);

            if (isValidNeck && dict.ContainsKey(neck))
            {
                activeNeckGarb = Instantiate(dict[neck].Prefab, chest);
            }
        }

        private void CheckCosmetic(Dictionary<string, GameObject> dict)
        {
            if (dict.ContainsKey(ObscuredPrefs.GetString("Head Garb")))
            {
                activeHeadGarb = Instantiate(dict[ObscuredPrefs.GetString("Head Garb")], head);
            }

            if (dict.ContainsKey(ObscuredPrefs.GetString("Face Garb")))
            {
                activeFaceGarb = Instantiate(dict[ObscuredPrefs.GetString("Face Garb")], head);
            }

            if (dict.ContainsKey(ObscuredPrefs.GetString("Neck Garb")))
            {
                activeNeckGarb = Instantiate(dict[ObscuredPrefs.GetString("Neck Garb")], chest);
            }
        }

        private void SetColor(int index, Color32 color)
        {
            avatar.materials[index].SetColor("_Color", color);
        }

        [Serializable]
        private class BodyPartColor
        {
            public string name;
            public Color32 defaultColor;
        }
    }
}

[SerializeField]
public class InventoryIcon {
    public string response = "";
    public string status = "NOT FOUND";
}