using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fittings : MonoBehaviour
{
    [SerializeField] private bool setRandomSprite;
    [SerializeField] private Sprite[] randomSprites;

    void Start()
    {
        if (setRandomSprite)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            var randInt = Random.Range(0, randomSprites.Length);
            spriteRenderer.sprite = randomSprites[randInt];
        }
    }

    void Update()
    {
        
    }
}
