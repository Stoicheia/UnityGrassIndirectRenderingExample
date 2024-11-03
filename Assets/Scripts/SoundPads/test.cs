using System;
using UnityEngine;
using UnityEngine.UI;

namespace MagicGrass.SoundPads
{
    public class test : MonoBehaviour
    {
        public void ChangeImage(Sprite newSprite)
        {
            Image image = GetComponent<Image>();
            image.sprite = newSprite;
        }
    }
}