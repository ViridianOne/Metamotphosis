using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darkness : MonoBehaviour
{
    public SpriteRenderer fullDarknessObject;
    public SpriteRenderer darknessWithLight;
    private bool isDark = false;
    public static Darkness instance;
    public MecroSelectManager index;
    //public bool lightsOn = false;
    [SerializeField] private bool isRoomDark;

    public void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        //if (Input.GetButtonDown("Fire1"))
        //    lightsOn = !lightsOn;
        if (/*index.GetIndex() == 0 && */isDark 
            && index.instantiatedMecros[/*index.GetIndex()*/(int)MecroStates.form161].isAbilityActivated/* && lightsOn == true*/) 
        {
            darknessWithLight.transform.position = Player.instance.transform.position;
            darknessWithLight.enabled = true;
            fullDarknessObject.enabled = false;
        }
        else if (isDark && !index.instantiatedMecros[/*index.GetIndex()*/(int)MecroStates.form161].isAbilityActivated/* && lightsOn==false*/)
        {
            darknessWithLight.enabled = false;
            fullDarknessObject.enabled = true;
        }
        else if(!isDark)
        {
            fullDarknessObject.enabled = false;
            darknessWithLight.enabled = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isRoomDark)
        {
            fullDarknessObject.enabled = true;
            isDark = !isDark;
        }

        //if(other.CompareTag("Enemy"))
        //{
        //    if(other.GetComponent<Enemy>().type == EnemyType.Robot)
        //        other.GetComponent<Robot>().canSwitchTheLights = true;
        //}
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
            fullDarknessObject.enabled = false;
            darknessWithLight.enabled = false;
            isDark = !isDark;
        }
    }

    public void TurnOn(bool canTurnOn)
    {
        isDark = canTurnOn;
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
