using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using System.Linq;
using System;

public static class GameTools {

    private static System.Random rnd = new System.Random();

    /// <summary>
    /// Returns whether the given <see cref="GameObject"/> has a <see cref="Component"/> of Type <see cref="{T}"/> attached to it, 
    /// if so the <see cref="Component"/> is returned via the out-parameter
    /// </summary>
    public static bool TryGetComponent<T>(this GameObject obj, out T output)
    {
        output = obj.GetComponent<T>();
        return output != null;
    }

    /// <summary>
    /// Returns whether one of the children of the given <see cref="GameObject"/> has a <see cref="Component"/> of Type <see cref="{T}"/> attached to it, 
    /// if so the <see cref="Component"/> is returned via the out-parameter
    /// </summary>
    public static bool TryGetComponentInChildren<T>(this GameObject obj, out T output)
    {
        output = obj.GetComponentInChildren<T>();
        return output != null;
    }

    /// <summary>
    /// Returns whether the given <see cref="GameObject"/> has a <see cref="Component"/> of the given <see cref="Type"/> attached to it or not
    /// </summary>
    public static bool HasComponent(this GameObject obj, Type type)
    {
        return obj.GetComponents(type).Count() > 0;
    }

    /// <summary>
    /// Returns a random element of a <see cref="IEnumerable"/> of Type <see cref="{T}"/>s
    /// </summary>
    public static T GetRandom<T>(this IEnumerable<T> collection)
    {
        return collection.ElementAt(rnd.Next(0, collection.Count()));
    }

    /// <summary>
    /// Trys to read the value with the specified name out of a <see cref="SerializationInfo"/>-<see cref="object"/>
    /// </summary>
    public static bool TryGetValue<T>(this SerializationInfo info, string name, out T output)
    {
        try
        {
            output = (T)info.GetValue(name, typeof(T));
            return true;
        }
        catch
        {
            output = default(T);
            return false;
        }
    }

    /// <summary>
    /// Returns a Coroutine for fading <see cref="Transform"/>s to another location
    /// </summary>
    public static IEnumerator Fade(Transform toFade, Vector3 targetPos, float smoothTime)
    {
        Vector3 fadeVelocity = Vector3.zero;

        while (toFade.position != targetPos)
        {
            toFade.position = Vector3.SmoothDamp(toFade.position, targetPos, ref fadeVelocity, smoothTime);
            yield return new WaitForEndOfFrame();
            if ((toFade.position - targetPos).magnitude < 0.1f)
                toFade.position = targetPos;
        }
    }
}
