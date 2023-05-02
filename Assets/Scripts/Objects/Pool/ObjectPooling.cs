using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling
{
	private PoolObject sample;
	private Transform objects_parent;
	private Stack<PoolObject> objects;

	public ObjectPooling(PoolObject sample, int count, Transform objects_parent)
    {
		objects = new Stack<PoolObject>();
		this.sample = sample;
		this.objects_parent = objects_parent;
		for (int i = 0; i < count; i++)
		{
			CreateObject();
		}
	}

	private void CreateObject()
	{
		var clone = Object.Instantiate(sample);
		clone.name = sample.name;
		clone.transform.SetParent(objects_parent);
		clone.SetPool(this);
		clone.SetActive(false);
		objects.Push(clone);
		//if (clone.GetComponent<Animator>())
		//	clone.GetComponent<Animator>().StartPlayback();
	}

	public PoolObject GetObject()
    {
        if (objects.Count == 0)
        {
			CreateObject();
        }
        var obj = objects.Pop();
        obj.SetActive(true);
        return obj;
    }

    public void RecycleObject(PoolObject obj)
    {
        if (obj != null && obj.Pool == this)
        {
            obj.SetActive(false);
            if (!objects.Contains(obj))
            {
				objects.Push(obj);
            }
        }
    }

	public void Clear()
    {
		objects.Clear();
    }
}
