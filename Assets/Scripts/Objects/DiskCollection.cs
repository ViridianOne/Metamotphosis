using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskCollection : MonoBehaviour
{
    public int DiskCount;

    public void DiskCollected()
    {
        DiskCount++;
    }
    public int getDiskCount()
    {
        return DiskCount;
    }
}
