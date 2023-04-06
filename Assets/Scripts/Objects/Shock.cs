using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : MonoBehaviour
{
    [SerializeField] private SpriteRenderer holderSprite;
    [SerializeField] private Collider2D shockCollider;
    private Color spriteColor;
    private bool isFaded;
    [SerializeField] private float fadingTime;
    [SerializeField] private float fadingFrameTime;
    private float fadingTimer;
    private float fadingFrameTimer;
    [SerializeField] private float turnOffTimer;
    [SerializeField] private float turnOnTimer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (turnOffTimer > 0)
        {
            turnOffTimer -= Time.deltaTime;
        }
        if (turnOffTimer < 0)
        {
            StartCoroutine(TurnOff());
            turnOnTimer = 15;
            turnOffTimer = 0;
        }
        if (turnOnTimer > 0)
        {
            turnOnTimer -= Time.deltaTime;
        }
        if (turnOnTimer < 0)
        {
            StartCoroutine(TurnOn());
            turnOffTimer = 10;
            turnOnTimer = 0;
        }
    }
    protected IEnumerator TurnOff()
    {
        //gameObject.GetComponent<Collider2D>().enabled = false;
        shockCollider.enabled = false;
        fadingTimer = fadingTime;
        fadingFrameTimer = 0;
        while (fadingTimer > 0)
        {
            spriteColor = holderSprite.color;
            if (fadingFrameTimer <= 0)
            {
                isFaded = !isFaded;
                fadingFrameTimer = fadingFrameTime;
            }
            spriteColor.a = isFaded ? 0 : 1;
            holderSprite.color = spriteColor;
            fadingTimer -= Time.deltaTime;
            fadingFrameTimer -= Time.deltaTime;
            yield return null;
        }
        //gameObject.SetActive(false);
        holderSprite.enabled = false;
    }

    protected IEnumerator TurnOn()
    {
        //shockCollider.enabled = true;
        holderSprite.enabled = true;
        fadingTimer = fadingTime;
        fadingFrameTimer = 0;
        while (fadingTimer > 0)
        {
            if (fadingFrameTimer <= 0)
            {
                isFaded = !isFaded;
                fadingFrameTimer = fadingFrameTime;
            }
            spriteColor.a = isFaded ? 0 : 1;
            holderSprite.color = spriteColor;
            fadingTimer -= Time.deltaTime;
            fadingFrameTimer -= Time.deltaTime;
            yield return null;
        }
        //gameObject.SetActive(true);
        //holderSprite.enabled = true;
        shockCollider.enabled = true;
    }
}
