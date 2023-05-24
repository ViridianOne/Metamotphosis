using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension
{
    public static Vector3 Add(this Vector3 v1, Vector2 v2) => new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z);

    public static Vector2 AsVector2(this Vector3 v) => new(v.x, v.y);
}

public static class Vector2Extension
{
    public static Vector2 Add(this Vector2 v1, Vector3 v2) => new Vector2(v1.x + v2.x, v1.y + v2.y);

    public static Vector3 AsVector3(this Vector2 v) => new(v.x, v.y, 0);
}
