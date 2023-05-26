using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjectsManager : MonoBehaviour
{
    private List<PoolObjectData> objectsData;
    private Stack<PoolObject> placedObjects;
    private bool isObjectsPlaced = false;

    void Start()
    {
        objectsData = new List<PoolObjectData>(16);
        placedObjects = new Stack<PoolObject>();
        SaveObjectsData();
    }

    private void SaveObjectsData()
    {
        foreach (var poolObject in GetComponentsInChildren<PoolObject>())
        {
            objectsData.Add(GetObjectData(poolObject));
            Destroy(poolObject.gameObject);
        }
    }

    private PoolObjectData GetObjectData(PoolObject poolObject)
    {
        IPoolObject component;
        PoolObjectData data;
        switch (poolObject.type)
        {
            case ObjectType.Saw:
                component = poolObject.GetComponentInChildren<SawMoving>();
                break;

            case ObjectType.PlatformLarge:
            case ObjectType.PlatformSmall:
                component = poolObject.GetComponentInChildren<Platform_new>();
                break;

            case ObjectType.Trampoline:
                component = poolObject.GetComponentInChildren<Trampoline>();
                break;

            case ObjectType.Electricity:
                component = poolObject.GetComponentInChildren<ElectricityTileChange>();
                break;

            case ObjectType.PlatformsWheel2:
            case ObjectType.PlatformsWheel3:
                component = poolObject.GetComponentInChildren<PlatformsWheel>();
                break;

            case ObjectType.Bomb:
                component = poolObject.GetComponentInChildren<Bomb>();
                break;

            case ObjectType.LaserBot:
                component = poolObject.GetComponentInChildren<LaserBot>();
                break;

            case ObjectType.DestroyingPlatform:
                component = poolObject.GetComponentInChildren<DestroyingPlatform>();
                break;

            case ObjectType.PressingBlock:
                component = poolObject.GetComponentInChildren<PressingBlock>();
                break;

            case ObjectType.Fittings:
            case ObjectType.FallingPlatform:
                data = new PoolObjectData();
                data.SetDefaultData(poolObject.type, poolObject.transform);
                return data;

            default:
                return null;
        }
        data = component.GetObjectData();
        data.SetDefaultData(poolObject.type, poolObject.transform);
        return data;
    }

    public void PlaceObjects()
    {
        if (isObjectsPlaced)
        {
            return;
        }

        isObjectsPlaced = true;
        foreach (var objectData in objectsData)
        {
            var poolObject = PoolManager.GetObject(objectData.type, objectData.position, objectData.rotation);
            SetObjectData(poolObject, objectData);
            placedObjects.Push(poolObject);
        }
    }

    private void SetObjectData(PoolObject poolObject, PoolObjectData objectData)
    {
        IPoolObject component;
        switch (objectData.type)
        {
            case ObjectType.Saw:
                component = poolObject.GetComponentInChildren<SawMoving>();
                break;

            case ObjectType.PlatformLarge:
            case ObjectType.PlatformSmall:
                component = poolObject.GetComponentInChildren<Platform_new>();
                break;

            case ObjectType.Trampoline:
                component = poolObject.GetComponentInChildren<Trampoline>();
                break;

            case ObjectType.Electricity:
                component = poolObject.GetComponentInChildren<ElectricityTileChange>();
                break;

            case ObjectType.PlatformsWheel2:
            case ObjectType.PlatformsWheel3:
                component = poolObject.GetComponentInChildren<PlatformsWheel>();
                break;

            case ObjectType.Bomb:
                component = poolObject.GetComponentInChildren<Bomb>();
                break;

            case ObjectType.LaserBot:
                component = poolObject.GetComponentInChildren<LaserBot>();
                break;

            case ObjectType.DestroyingPlatform:
                component = poolObject.GetComponentInChildren<DestroyingPlatform>();
                break;

            case ObjectType.PressingBlock:
                component = poolObject.GetComponentInChildren<PressingBlock>();
                break;

            default:
                return;
        }
        component.SetObjectData(objectData);
    }

    public void RecycleObjects()
    {
        isObjectsPlaced = false;
        while (placedObjects.Count > 0)
        {
            placedObjects.Pop().Recycle();
        }
    }
}
