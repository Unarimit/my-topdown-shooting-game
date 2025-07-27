
using UnityEngine;

public static class TransformExtend
{
    public static T AddComponent<T>(this Transform trans) where T : Component
    {
        return trans.gameObject.AddComponent<T>();   
    }
}