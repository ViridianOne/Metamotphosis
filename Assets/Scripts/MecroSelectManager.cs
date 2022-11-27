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

    //private Player currentMecro;
    [SerializeField] private Player[] mecros;
    private Player[] instantiatedMecros = new Player[3]; 
    
    void Start()
    {
        canSelect = true;
        /*currentMecro = Player.instance;
        currentMecro.respawnPoint = respawnPoint;*/
        currentMecroIndex = startMecroIndex;
        Vector3 mecroPos = Player.instance.transform.position;
        Quaternion mecroRot = Player.instance.transform.localRotation;
        Destroy(Player.instance.gameObject);
        for(int i = 0; i < mecros.Length; i++)
        {
            instantiatedMecros[i] = Instantiate(mecros[i], mecroPos, mecroRot);
            instantiatedMecros[i].gameObject.SetActive(false);
            instantiatedMecros[i].respawnPoint = respawnPoint;
        }
        /*currentMecro = instantiatedMecros[0];
        currentMecro.gameObject.SetActive(true);*/
        //instantiatedMecros[startMecroIndex].respawnPoint = respawnPoint;
        Player.instance = instantiatedMecros[startMecroIndex];
        Player.instance.gameObject.SetActive(true);
    }

    void Update()
    {
        //Player.instance.isActive = canSelect;
        if (canSelect)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CreateNewMecro(1);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                CreateNewMecro(0);
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
            //Vector3 mecroPos = Player.instance.transform.position;
            //Quaternion mecroRot = Player.instance.transform.localRotation;
            //Destroy(Player.instance.gameObject);
            //currentMecro = Instantiate(mecros[mecroListIndex], mecroPos, mecroRot);
            Player.instance.gameObject.SetActive(false);
            /*currentMecro = mecros[mecroListIndex];
            currentMecro.gameObject.SetActive(true);
            currentMecro.respawnPoint = respawnPoint;*/
            instantiatedMecros[mecroListIndex].transform.position = Player.instance.transform.position;
            instantiatedMecros[mecroListIndex].transform.localRotation = Player.instance.transform.localRotation;
            //instantiatedMecros[mecroListIndex].respawnPoint = respawnPoint;
            Player.instance = instantiatedMecros[mecroListIndex];
            Player.instance.gameObject.SetActive(true);
            currentMecroIndex = mecroListIndex;
            StartCoroutine(WaitAfterSelect());
            //CameraController.instance.target = currentMecro.transform;
        }
    }

    private IEnumerator WaitAfterSelect()
    {
        canSelect = false;
        Player.instance.isActive = false;
        yield return new WaitForSeconds(selectTime);
        canSelect = true;
        Player.instance.isActive = true;
    }
}
