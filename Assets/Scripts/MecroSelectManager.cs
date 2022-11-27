using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecroSelectManager : MonoBehaviour
{
    private bool canSelect;
    [SerializeField] private int startMecroIndex;
    private int currentMecroIndex;
    [SerializeField] public Transform respawnPoint;
    [SerializeField] private float selectTime;

    private Player currentMecro;
    [SerializeField] private Player[] mecros;
    
    void Start()
    {
        canSelect = true;
        currentMecro = Player.instance;
        currentMecro.respawnPoint = respawnPoint;
        currentMecroIndex = startMecroIndex;
    }

    void Update()
    {
        Player.instance.isActive = canSelect;
        if (canSelect)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CreateNewMecro(0);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                CreateNewMecro(1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                CreateNewMecro(2);
            }
        }
    }

    private void CreateNewMecro(int mecroListIndex)
    {
        if (currentMecroIndex != mecroListIndex)
        {
            Vector3 mecroPos = Player.instance.transform.position;
            Destroy(Player.instance.gameObject);
            currentMecro = Instantiate(mecros[mecroListIndex], mecroPos, mecros[mecroListIndex].transform.rotation);
            currentMecro.respawnPoint = respawnPoint;
            Player.instance = currentMecro;
            currentMecroIndex = mecroListIndex;
            StartCoroutine(WaitAfterSelect());
            //CameraController.instance.target = currentMecro.transform;
        }
    }

    private IEnumerator WaitAfterSelect()
    {
        canSelect = false;
        yield return new WaitForSeconds(selectTime);
        canSelect = true;
    }
}
