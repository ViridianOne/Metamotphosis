using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityManager : MonoBehaviour
{
    public static VisibilityManager instance;

    [SerializeField] private SpriteRenderer invisibilityEffect;
    private bool isVisibilityActivated = false;
    private List<GameObject> invisibleObjects = new List<GameObject>();

    public void Awake()
    {
        instance = this;
    }

    private void Update()
    {

    }

    public void MonitorInvisibleObject(GameObject obj)
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
        invisibilityEffect.GetComponent<SpriteRenderer>().enabled = type;
        foreach (var obj in invisibleObjects)
        {
            if (obj == null)
            {
                invisibleObjects.Remove(obj);
            }
            else
            {
                var objRenderer = obj.GetComponent<SpriteRenderer>();
                if (objRenderer != null)
                {
                    objRenderer.sortingLayerID = SortingLayer.NameToID(isVisibilityActivated ? "Visible" : "Invisible");
                }
            }
        }
    }
}
