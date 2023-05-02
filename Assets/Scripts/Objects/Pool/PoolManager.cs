using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoolManager
{
	private static PoolData[] poolsSetup;
	private static GameObject objectsParent;

    public static void SetPoolsSetup(PoolData[] newPoolsSetup)
	{
        if (poolsSetup == newPoolsSetup)
            return;

        if (poolsSetup != null)
            ClearSetup();

        objectsParent = new GameObject();
        objectsParent.name = "Location pool";
        poolsSetup = newPoolsSetup;
        for (int i = 0; i < poolsSetup.Length; i++)
        {
            if (poolsSetup[i].prefab != null)
            {
                poolsSetup[i].pool = new ObjectPooling(poolsSetup[i].prefab, poolsSetup[i].count, objectsParent.transform);
            }
        }
    }

    public static void ClearSetup()
    {
        UnityEngine.Object.Destroy(objectsParent);
        foreach (var poolData in poolsSetup)
        {
            if (poolData.pool != null)
            {
                poolData.pool.Clear();
            }
        }
    }

    public static PoolObject GetObject(ObjectType type, Vector3 position, Quaternion rotation)
	{
		if (poolsSetup != null)
		{
            for (int i = 0; i < poolsSetup.Length; i++)
            {
                if (poolsSetup[i].type == type && poolsSetup[i].prefab != null)
                {
                    var result = poolsSetup[i].pool.GetObject();
                    result.transform.position = position;
                    result.transform.rotation = rotation;
                    return result;
                }
            }
        }
		return null;
	}

}
