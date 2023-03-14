using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotation : MonoBehaviour
{
    private float moveInput;
    [SerializeField] private Vector3 rotationDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput > 0f)
        {
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 90f), 30f);
            rotationDirection.z = -90;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, rotationDirection.z), 1);
        }
        else if (moveInput < 0f)
        {
            rotationDirection.z = 0;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, rotationDirection.z), 1);
        }
    }
}
