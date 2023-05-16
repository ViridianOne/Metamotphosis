using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension
{
    public static Vector3 Add(this Vector3 v1, Vector3 v2) => new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
}

public static class Vector2Extension
{
    public static Vector2 Add(this Vector2 v1, Vector2 v2) => new Vector2(v1.x + v2.x, v1.y + v2.y);
}
