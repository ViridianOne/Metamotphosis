using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityManager : MonoBehaviour
{
    public static VisibilityManager instance;

    [SerializeField] private SpriteRenderer invisibilityEffect;
    private bool isVisibilityActivated = false;
    private List<SpriteRenderer> invisibleObjects = new List<SpriteRenderer>();

    public void Awake()
    {
        instance = this;
    }

    public void MonitorInvisibleObject(SpriteRenderer obj)
    {
        invisibleObjects.Add(obj);
    }

    public void ToggleVisibility(bool type)
    {
        if (type == isVisibilityActivated)
        {
            return;
        }
        isVisibilityActivated = type;
        invisibilityEffect.transform.position = Player.instance.transform.position;
        //invisibilityEffect.GetComponent<SpriteRenderer>().enabled = type;
        invisibilityEffect.enabled = type;
        foreach (var obj in invisibleObjects)
        {
            if (obj.gameObject == null)
            {
                invisibleObjects.Remove(obj);
            }
            else
            {
                //var objRenderer = obj.GetComponent<SpriteRenderer>();
                if (obj != null)
                {
                    obj.sortingLayerID = SortingLayer.NameToID(isVisibilityActivated ? "Visible" : "Invisible");
                    obj.enabled = isVisibilityActivated;
                }
            }
        }
    }
}
