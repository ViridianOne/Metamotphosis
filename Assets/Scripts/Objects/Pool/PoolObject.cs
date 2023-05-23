using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public ObjectPooling Pool { get; private set; }
    public ObjectType type;

    public void SetPool(ObjectPooling pool)
    {
        Pool = pool;
    }

    public void Recycle()
    {
        if (Pool != null)
        {
            Pool.RecycleObject(this);
        }
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }
}
