
using Assets.Scripts.CombatLogic.GOAPs;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GOAPDebugger))]
internal class GOAPDebuggerEditor : Editor
{
    public override void OnInspectorGUI() {
        var controller = (GOAPDebugger)target;
        GUILayout.Label(controller.ActionsResult);
        base.OnInspectorGUI();
    }
}