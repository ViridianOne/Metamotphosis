using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolData
{
	public ObjectType type;
	public PoolObject prefab;
	public int count;
	public ObjectPooling pool;
}
