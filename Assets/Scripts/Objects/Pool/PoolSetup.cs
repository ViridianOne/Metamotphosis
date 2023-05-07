using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[AddComponentMenu("Pool/PoolSetup")]
public class PoolSetup : MonoBehaviour
{
	[SerializeField] private bool setAsDefault = false;
	[SerializeField] private PoolData[] pools;

    private void Awake()
    {
        if (setAsDefault)
        {
            PoolManager.SetPoolsSetup(pools);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            PoolManager.SetPoolsSetup(pools);
        }
    }

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player") && !other.isTrigger)
    //    {
    //        PoolManager.ClearSetup();
    //    }
    //}
}
