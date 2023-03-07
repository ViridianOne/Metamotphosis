using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisible : MonoBehaviour
{
    private void Awake()
    {
        var renderer = gameObject.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sortingLayerID = SortingLayer.NameToID("Invisible");
            VisibilityManager.instance.MonitorInvisibleObject(renderer);
        }
    }
}
