using UnityEngine;
using UnityEngine.UI;
using System;

public class ImageCycler : MonoBehaviour
{
    [Serializable]
    public struct ImageObject
    {
        public Sprite image;
        public string author;
    }

    public Image image;
    public Text author;
    public ImageObject[] imageObjects;

    public void Awake()
    {
        EnableImage(UnityEngine.Random.Range(0, imageObjects.Length));
    }

    private void EnableImage(int index)
    {
        image.sprite = imageObjects[index].image;
        author.text = "Made by: " + imageObjects[index].author;
    }
}