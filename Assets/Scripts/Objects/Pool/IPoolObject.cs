using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
    public PoolObjectData GetObjectData();
    public void SetObjectData(PoolObjectData objectData);
}
