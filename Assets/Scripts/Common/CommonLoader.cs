using Assets.Scripts;
using Assets.Scripts.CombatLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

internal class CommonLoader : MonoBehaviour
{
    public static CommonLoader Instance { get; private set; }

    public static void TryInitCommonLoader()
    {
        if (Instance != null) return;
        var go = ResourceManager.LoadGoAndInstantiate("Common/Common", null);
        DontDestroyOnLoad(go);
        Instance = go.GetComponent<CommonLoader>();

    }

    [NonSerialized]
    public FbxLoadManager FbxLoader;

    private void Awake()
    {
        FbxLoader = GetComponent<FbxLoadManager>();
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
