using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electricity : MonoBehaviour
{
    [SerializeField] private float duration;

    void Start()
    {
        StartCoroutine(Existing());
    }

    void Update()
    {
        
    }

    private IEnumerator Existing()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
