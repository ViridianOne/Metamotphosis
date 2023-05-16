using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Сhemical : MonoBehaviour
{
    public Color newColor;
    private float timerUntilDamage;
    private float timeUntilDamage = 2f;
    private bool isActive;

    void Start()
    {
        timerUntilDamage = 0;
    }
    private void Update()
    {
        if (timerUntilDamage>0f)
        {
            timerUntilDamage -= Time.deltaTime;
            isActive = true;
        }
        else if (timerUntilDamage<=0f && isActive)
        {
            Player.instance.DamagePlayer();
            isActive = false;
        }
        if (!isActive && timerUntilDamage<0f)
        {
            timerUntilDamage = 0f;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var sprite = Player.instance.GetHolder().GetComponent<SpriteRenderer>();
            sprite.color = newColor;
            if (timerUntilDamage == 0f)
            {
                timerUntilDamage = timeUntilDamage;
                //isActive = true;
            }
        }
    }
}
