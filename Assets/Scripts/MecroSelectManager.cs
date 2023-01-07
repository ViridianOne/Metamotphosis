using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecroSelectManager : MonoBehaviour
{
    [HideInInspector] public static MecroSelectManager instance;
    private bool canSelect;
    [SerializeField] private MecroStates startMecro = MecroStates.form161;
    private MecroStates currentMecro;
    [SerializeField] public Transform respawnPoint;
    [SerializeField] private float selectTime;

    //private Player currentMecro;
    [SerializeField] private Player[] mecros;
    [HideInInspector] public Player[] instantiatedMecros = new Player[3];
    [SerializeField] public bool[] isMecroUnlocked = { true, false, false };

    public bool isChanged;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        /*currentMecro = Player.instance;
        currentMecro.respawnPoint = respawnPoint;*/
        //mecros[0].respawnPoint = respawnPoint;
        //mecros[1].respawnPoint = respawnPoint;
        //mecros[2].respawnPoint = respawnPoint;
        /*currentMecro = instantiatedMecros[0];
        currentMecro.gameObject.SetActive(true);*/
        //instantiatedMecros[startMecroIndex].respawnPoint = respawnPoint;
        canSelect = true;
        currentMecro = startMecro;
        Vector3 mecroPos = Player.instance.transform.position;
        Quaternion mecroRot = Player.instance.transform.localRotation;
        Destroy(Player.instance.gameObject);
        for(int i = 0; i < mecros.Length; i++)
        {
            instantiatedMecros[i] = Instantiate(mecros[i], mecroPos, mecroRot);
            instantiatedMecros[i].gameObject.SetActive(false);
            instantiatedMecros[i].respawnPoint = respawnPoint;
        }
        Player.instance = instantiatedMecros[(int)startMecro];
        Player.instance.gameObject.SetActive(true);
    }

    void Update()
    {
        //Player.instance.isActive = canSelect;
        if (canSelect)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SelectMecro(MecroStates.form161);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SelectMecro(MecroStates.form296);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SelectMecro(MecroStates.form71);
            }
        }
    }

    private void SelectMecro(MecroStates mecroState)
    {
        if (isMecroUnlocked[(int)mecroState] && currentMecro != mecroState)
        {
            //Vector3 mecroPos = Player.instance.transform.position;
            //Quaternion mecroRot = Player.instance.transform.localRotation;
            //Destroy(Player.instance.gameObject);
            //currentMecro = Instantiate(mecros[mecroListIndex], mecroPos, mecroRot);
            /*currentMecro = mecros[mecroListIndex];
            currentMecro.gameObject.SetActive(true);
            currentMecro.respawnPoint = respawnPoint;*/
            //instantiatedMecros[mecroListIndex].respawnPoint = respawnPoint;
            isChanged = true;
            Player.instance.DisableAbility();
            Player.instance.gameObject.SetActive(false);
            instantiatedMecros[(int)mecroState].transform.position = Player.instance.transform.position;
            instantiatedMecros[(int)mecroState].transform.localRotation = Player.instance.transform.localRotation;
            Player.instance = instantiatedMecros[(int)mecroState];
            Player.instance.gameObject.SetActive(true);
            currentMecro = mecroState;
            StartCoroutine(WaitAfterSelect());
        }
    }

    private IEnumerator WaitAfterSelect()
    {
        Player.instance.ToggleActive(false);
        canSelect = false;
        yield return new WaitForSeconds(selectTime);
        Player.instance.ToggleActive(true);
        canSelect = true;
        isChanged = false;
    }

    public int GetIndex()
    {
        return (int)currentMecro;
    }
}
