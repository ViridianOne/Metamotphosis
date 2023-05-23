using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PoolObjectData
{
    public ObjectType type { get; private set; }
    public Vector3 position { get; private set; }
    public Quaternion rotation { get; private set; }

    public void SetDefaultData(ObjectType type, Transform transform)
    {
        this.type = type;
        position = transform.position;
        rotation = transform.rotation;
    }
}


public class PoolSawData : PoolObjectData
{
    public readonly Vector3 position1, position2, startPosition;
    public readonly bool isMoving, isSmall;

    public PoolSawData(Vector3 position1, Vector3 position2, Vector3 startPosition, 
        bool isMoving, bool isSmall)
    {
        this.position1 = position1;
        this.position2 = position2;
        this.startPosition = startPosition;
        this.isMoving = isMoving;
        this.isSmall = isSmall;
    }
}


public class PoolPlatformData : PoolObjectData
{
    public readonly Vector3 position1, position2, startPosition;
    public readonly bool isMoving, isSleeping;
    public readonly int animationLayer;
    public readonly Vector2 activeZonePos, activeZoneSize;

    public PoolPlatformData(Vector3 position1, Vector3 position2, Vector3 startPosition,
        bool isMoving, bool isSleeping, int animationLayer, 
        Vector2 activeZonePos, Vector2 activeZoneSize)
    {
        this.position1 = position1;
        this.position2 = position2;
        this.startPosition = startPosition;
        this.isMoving = isMoving;
        this.isSleeping = isSleeping;
        this.animationLayer = animationLayer;
        this.activeZonePos = activeZonePos;
        this.activeZoneSize = activeZoneSize;
    }
}


public class PoolTrampolineData : PoolObjectData
{
    public readonly bool isSleeping;
    public readonly float jumpAdditionForce;
    public readonly Vector2 activeZonePos, activeZoneSize;
    public readonly int animationLayer;

    public PoolTrampolineData(bool isSleeping, float jumpAdditionForce, 
        Vector2 activeZonePos, Vector2 activeZoneSize, int animationLayer)
    {
        this.isSleeping = isSleeping;
        this.jumpAdditionForce = jumpAdditionForce;
        this.activeZonePos = activeZonePos;
        this.activeZoneSize = activeZoneSize;
        this.animationLayer = animationLayer;
    }
}


public class PoolElectricityData : PoolObjectData
{
    public readonly bool isVertical;
    public readonly int firstTilePos, lastTilePos, constTilePos;
    public readonly AnimatedTile middleTile, firstTile, lastTile;
    public readonly int animationLayer;

    public PoolElectricityData(bool isVertical,
        AnimatedTile middleTile, AnimatedTile firstTile, AnimatedTile lastTile,
        int firstTilePos, int lastTilePos, int constTilePos, int animationLayer)
    {
        this.isVertical = isVertical;
        this.firstTilePos = firstTilePos;
        this.lastTilePos = lastTilePos;
        this.constTilePos = constTilePos;
        this.middleTile = middleTile;
        this.firstTile = firstTile;
        this.lastTile = lastTile;
        this.animationLayer = animationLayer;
    }
}


public class PoolPlatformsWheelData : PoolObjectData
{
    public readonly bool isMoving, isSleeping;
    public readonly int clockwiseCoef;
    public readonly float rotationSpeed, movementSpeed;
    public readonly Vector3 position1, position2, startPosition;
    public readonly Vector3[] platformsPositions;
    public readonly int animationLayer;

    public PoolPlatformsWheelData(bool isMoving, bool isSleeping, int clockwiseCoef, 
        float rotationSpeed, float movementSpeed, Vector3 position1, Vector3 position2, 
        Vector3 startPosition, Vector3[] platformsPositions, int animationLayer)
    {
        this.isMoving = isMoving;
        this.isSleeping = isSleeping;
        this.clockwiseCoef = clockwiseCoef;
        this.rotationSpeed = rotationSpeed;
        this.movementSpeed = movementSpeed;
        this.position1 = position1;
        this.position2 = position2;
        this.startPosition = startPosition;
        this.platformsPositions = platformsPositions;
        this.animationLayer = animationLayer;
    }
}


public class PoolBombData : PoolObjectData
{
    public readonly int animationLayer;
    public readonly float timeBeforeExplode;
    public readonly float explosionReclainForce;
    public readonly float lethalExplosionRadius, explosionReclainRadius;
    public readonly Vector2 triggerAreaPos, triggerAreaSize;

    public PoolBombData(int animationLayer, float timeBeforeExplode, float explosionReclainForce, 
        float lethalExplosionRadius, float explosionReclainRadius, 
        Vector2 triggerAreaPos, Vector2 triggerAreaSize)
    {
        this.animationLayer = animationLayer;
        this.timeBeforeExplode = timeBeforeExplode;
        this.explosionReclainForce = explosionReclainForce;
        this.lethalExplosionRadius = lethalExplosionRadius;
        this.explosionReclainRadius = explosionReclainRadius;
        this.triggerAreaPos = triggerAreaPos;
        this.triggerAreaSize = triggerAreaSize;
    }
}


public class PoolLaserBotData : PoolObjectData
{
    public readonly bool isMoving;
    public readonly Vector3 position1, position2, startPosition;
    public readonly float movementSpeed;
    public readonly Vector3 laserEndLocPosition;
    public readonly float activeTime, disableTime;
    public readonly int animationLayer;

    public PoolLaserBotData(bool isMoving, Vector3 position1, Vector3 position2, Vector3 startPosition, 
        float movementSpeed, Vector3 laserEndLocPosition, float activeTime, float disableTime, int animationLayer)
    {
        this.isMoving = isMoving;
        this.position1 = position1;
        this.position2 = position2;
        this.startPosition = startPosition;
        this.movementSpeed = movementSpeed;
        this.laserEndLocPosition = laserEndLocPosition;
        this.activeTime = activeTime;
        this.disableTime = disableTime;
        this.animationLayer = animationLayer;
    }
}


public class PoolDestroyingPlatformData : PoolObjectData
{
    public readonly float phaseTime;
    public readonly float timeToRecover;
    public readonly int animationLayer;

    public PoolDestroyingPlatformData(float phaseTime, float timeToRecover, int animationLayer)
    {
        this.phaseTime = phaseTime;
        this.timeToRecover = timeToRecover;
        this.animationLayer = animationLayer;
    }
}
