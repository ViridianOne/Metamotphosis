using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChemicalDrop : MonoBehaviour
{
    [SerializeField] private float activeTime, disableTime, turningOnTime;
    private float activeTimer, disableTimer;
    private bool isActive;
    [SerializeField] private GameObject drop;
    [SerializeField] private Transform pos1, pos2;
    // Start is called before the first frame update
    void Start()
    {
        activeTimer = 0;
        disableTimer = disableTime;
        drop.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (drop.transform.position.y <= pos2.position.y)
        {
            //drop.transform.position = pos2.position;
            drop.SetActive(false);
        }
        if (disableTimer > 0f)
        {
            disableTimer -= Time.deltaTime;
            isActive = false;
        }
        else if (disableTimer <= 0f && !isActive)
        {
            StartCoroutine(TurnOn());
        }

        if (activeTimer > 0f)
        {
            activeTimer -= Time.deltaTime;
            isActive = true;
        }
        else if (activeTimer <= 0f && isActive)
        {
            //drop.SetActive(false);
            disableTimer = disableTime;
        }
    }

    private IEnumerator TurnOn()
    {
        drop.transform.position = pos1.position;
        yield return new WaitForSeconds(turningOnTime);
        drop.SetActive(true);
        activeTimer = activeTime;
    }
}
