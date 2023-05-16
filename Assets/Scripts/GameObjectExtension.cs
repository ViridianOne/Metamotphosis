using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension
{
    public static IEnumerator MoveObjectSmoothly(this GameObject obj, Vector2 targetPos, float duration)
    {
        var timeElapsed = 0f;
        var startPos = obj.transform.localPosition;
        while (timeElapsed < duration)
        {
            obj.transform.localPosition = Vector3.Lerp(startPos, targetPos, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.localPosition = targetPos;
    }
}
