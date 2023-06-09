using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecroSelectManager : MonoBehaviour, IDataPersistance
{
    [HideInInspector] public static MecroSelectManager instance;
    private bool canSelect;
    [SerializeField] private MecroStates startMecro = MecroStates.form161;
    private MecroStates currentMecro;
    public Transform respawnPoint;
    [SerializeField] float respawnTime;
    [SerializeField] private float selectTime;

    //private Player currentMecro;
    [SerializeField] private Player[] mecros;
    [HideInInspector] public Player[] instantiatedMecros;
    public bool[] isMecroUnlocked = { true, false, false, false, false, false, false, false };

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
        instantiatedMecros = new Player[mecros.Length];
        for (int i = 0; i < mecros.Length; i++)
        {
            instantiatedMecros[i] = Instantiate(mecros[i], mecroPos, mecroRot);
            instantiatedMecros[i].gameObject.SetActive(false);
            instantiatedMecros[i].respawnPoint = respawnPoint;
            instantiatedMecros[i].transform.position = respawnPoint.position;
        }
        Player.instance = instantiatedMecros[(int)startMecro];
        Physics2D.IgnoreLayerCollision(7, 9, false);
        Player.instance.ceilCoef = 1;
        Player.instance.isVertical = false;
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
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SelectMecro(MecroStates.form206);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                SelectMecro(MecroStates.form341);
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                SelectMecro(MecroStates.form116);
            }
            else if(Input.GetKeyDown(KeyCode.L))
            {
                SelectMecro(MecroStates.form251);
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                SelectMecro(MecroStates.form26);
            }
        }
    }

    public void SelectMecro(MecroStates mecroState)
    {
        if (isMecroUnlocked[(int)mecroState] && currentMecro != mecroState)
        {
            Player.instance.DisableAbility();
            isChanged = true;
            if(Player.instance.transform.parent != null && Player.instance.transform.parent.tag == "Platform")
            {
                Player.instance.transform.SetParent(null);
            }
            Player.instance.gameObject.SetActive(false);
            instantiatedMecros[(int)mecroState].respawnPoint = respawnPoint;
            instantiatedMecros[(int)mecroState].transform.position = Player.instance.transform.position + new Vector3(0.2f, 0.2f, 0f);
            instantiatedMecros[(int)mecroState].transform.localRotation = Player.instance.transform.localRotation;
            if (Player.instance.IsGravityInverted)
                instantiatedMecros[(int)mecroState].InvertGravity();
            instantiatedMecros[(int)mecroState].InvertMovement(Player.instance.IsMovementInverted);
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

    public float GetRespawnTime() => respawnTime;

    public bool IsPlayerInvisible { get => instantiatedMecros[(int)MecroStates.form206].isAbilityActivated; }

    public void SetMecroUnlock(MecroStates mecro, bool isUnlocked)
    {
        isMecroUnlocked[(int)mecro] = isUnlocked;
        FormSwitchIconChanger.instance.SetPossibleState(mecro, isUnlocked);
    }

    public void LoadData(GameData data)
    {
        for (var i = 0; i < data.mecroFormsAvailabilty.Length; i++)
            SetMecroUnlock((MecroStates)i, data.mecroFormsAvailabilty[i]);
        respawnPoint.position = data.lastCheckpoint;
        Player.instance.transform.position = respawnPoint.position;
    }

    public void SaveData(ref GameData data)
    {
        data.mecroFormsAvailabilty = isMecroUnlocked;
        data.lastCheckpoint = respawnPoint.position;
    }
}
