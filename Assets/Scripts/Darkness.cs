using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darkness : MonoBehaviour
{
    public GameObject fullDarknessObject;
    public GameObject darknessWithLight;
    public bool isDark = false;
    public static Darkness instance;
    public MecroSelectManager index;
    public bool lightsOn = false;
    public void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        //if (Input.GetButtonDown("Fire1"))
        //    lightsOn = !lightsOn;
        if (/*index.GetIndex() == 0 && */isDark && index.instantiatedMecros[index.GetIndex()].lightSwitcher/* && lightsOn == true*/) 
        {
            darknessWithLight.transform.position = Player.instance.transform.position;
            darknessWithLight.SetActive(true);
            fullDarknessObject.SetActive(false);
        }
        else if (isDark && !index.instantiatedMecros[index.GetIndex()].lightSwitcher/* && lightsOn==false*/)
        {
            darknessWithLight.SetActive(false);
            fullDarknessObject.SetActive(true);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isDark)
        {
            fullDarknessObject.SetActive(true);
            isDark = !isDark;
        }
        /*(else if (other.CompareTag("Player") && isDark == true)
        {
            darknessObject.SetActive(false);
            isDark = !isDark;
        }*/
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isDark)
        {
            fullDarknessObject.SetActive(false);
            darknessWithLight.SetActive(false);
            isDark = !isDark;
        }
    }

    /*public void TurnOnLight()
    {
        if (index.GetIndex() == 0 && isDark == true)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                darknessWithLight.SetActive(true);
                darknessWithLight.transform.position = Player.instance.transform.position;
                if (transform.position == pos1.position || lightsOn == true)
                {
                    nextPos = pos2.position;
                }
                if (lightsOn == false)
                {
                    nextPos = transform.position;
                }
            }

        }*/
}
