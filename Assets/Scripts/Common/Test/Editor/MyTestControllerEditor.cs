
using Assets.Scripts.Common.Test;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MyTestController))]
internal class MyTestControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var controller = (MyTestController)target;

        GUILayout.Label($"{controller.MyTestInvokeInfos.Count} method find, only runtime clearly");
        foreach (var x in controller.MyTestInvokeInfos)
        {
            if (GUILayout.Button($"{x.Instance.GetType().Name}::{x.Method.Name}"))
            {
                x.Method.Invoke(x.Instance, null);
            }
        }
        
        base.OnInspectorGUI();
    }
}
