using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground26 : MonoBehaviour
{
    [SerializeField] private Collider2D colliderGround26;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (MecroSelectManager.instance.GetIndex() == 7)
        {
            colliderGround26.gameObject.SetActive(true);
        }
        else
        {
            colliderGround26.gameObject.SetActive(false);
        }
    }
}
