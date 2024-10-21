using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Make sure to include this for handling UI Images

public class ParallaxBackground : MonoBehaviour
{
    [Header("Sky Settings")]
    [SerializeField] private Image skyImage;  // Reference to the Image component of the Sky object
    [SerializeField] private Sprite[] skySprites;  // Array to hold the different sky images

    [Header("Temple Settings")]
    [SerializeField] private Image templeImage;  // Reference to the Image component of the Temple object
    [SerializeField] private Sprite[] templeSprites;  // Array to hold the different temple images

    void Start()
    {
        // Randomly select and assign an image for the Sky
        int randomSkyIndex = Random.Range(0, skySprites.Length);
        skyImage.sprite = skySprites[randomSkyIndex];

        // Randomly select and assign an image for the Temple
        int randomTempleIndex = Random.Range(0, templeSprites.Length);
        templeImage.sprite = templeSprites[randomTempleIndex];
    }
}
