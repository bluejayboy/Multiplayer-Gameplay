using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class BulletImpact : SerializedMonoBehaviour
    {
        [SerializeField] private Dictionary<string, BulletImpactInfo> bulletImpactInfos = null;
        [SerializeField] private GameObject[] disableObjects;

        public GameObject BloodAttach;
        public GameObject[] BloodFX;

        private AudioSource audioSource = null;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void SpawnBulletImpact(string tag)
        {
            ResetAll();

            if (!bulletImpactInfos.ContainsKey(tag))
            {
                tag = "Untagged";
            }

            var bulletImpact = bulletImpactInfos[tag];

            if (bulletImpact.Hole != null)
            {
                bulletImpact.Hole.SetActive(true);
            }

            bulletImpact.Particle.SetActive(true);
            audioSource.PlayAudioClip(bulletImpact.AudioClip);

            if (tag == "Flesh")
            {
                SpawnBlood();
            }
        }

        private void ResetAll()
        {
            for (int i = 0; i < disableObjects.Length; i++)
            {
                disableObjects[i].SetActive(false);
            }

            for (int i = 0; i < BloodFX.Length; i++)
            {
                BloodFX[i].SetActive(false);
            }
        }

        private void SpawnBlood()
        {
            var instance = BloodFX[UnityEngine.Random.Range(0, BloodFX.Length)];
            instance.SetActive(true);

            /*Ray ray = new Ray(transform.position + transform.right, -transform.right);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var nearestBone = GetNearestObject(hit.transform.root, hit.point);
                if (nearestBone != null)
                {
                    var attachBloodInstance = Instantiate(BloodAttach);
                    var bloodT = attachBloodInstance.transform;
                    bloodT.position = hit.point;
                    bloodT.localRotation = Quaternion.identity;
                    bloodT.localScale = Vector3.one * UnityEngine. Random.Range(0.75f, 1.2f);
                    bloodT.LookAt(hit.point + hit.normal, Vector3.zero);
                    bloodT.Rotate(90, 0, 0);
                    bloodT.transform.parent = nearestBone;
                    Destroy(attachBloodInstance, 10);
                }
            }*/
        }

        Transform GetNearestObject(Transform hit, Vector3 hitPos)
        {
            var closestPos = 100f;
            Transform closestBone = null;
            var childs = hit.GetComponentsInChildren<Transform>();

            foreach (var child in childs)
            {
                var dist = Vector3.Distance(child.position, hitPos);
                if (dist < closestPos)
                {
                    closestPos = dist;
                    closestBone = child;
                }
            }

            var distRoot = Vector3.Distance(hit.position, hitPos);
            if (distRoot < closestPos)
            {
                closestPos = distRoot;
                closestBone = hit;
            }
            return closestBone;
        }

        [Serializable]
        private struct BulletImpactInfo
        {
            [SerializeField] private GameObject hole;
            public GameObject Hole { get { return hole; } }

            [SerializeField] private GameObject particle;
            public GameObject Particle { get { return particle; } }

            [SerializeField] private AudioClip audioClip;
            public AudioClip AudioClip { get { return audioClip; } }
        }
    }
}