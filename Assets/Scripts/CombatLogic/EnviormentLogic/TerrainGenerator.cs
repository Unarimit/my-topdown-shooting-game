using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.CombatLogic.EnviormentLogic
{
    internal static class TerrainGenerator
    {
        static Transform TerrainTrans;
        public static void GenerateTerrain(this CombatContextManager context, int[][] arr)
        {
            if (TerrainTrans == null) TerrainTrans = context.transform.Find("Terrain");
            for(int i = 0; i < arr.Length; i++)
            {
                for(int j = 0; j < arr[i].Length; j++)
                {
                    GameObject prefab;
                    if (arr[i][j] == 1) prefab = ResourceManager.Load<GameObject>("Terrains/Road");
                    else if (arr[i][j] == 2) prefab = ResourceManager.Load<GameObject>("Terrains/Mountain");
                    else if (arr[i][j] == 3) prefab = ResourceManager.Load<GameObject>("Terrains/Water");
                    else
                    {
                        prefab = ResourceManager.Load<GameObject>("Terrains/Grass");
                        GenerateGrassDecoration(i, j, context);
                    }
                    var trans = context.CreateGO(prefab, TerrainTrans);
                    trans.localPosition = new Vector3(i, 0, j);
                    
                }
            }

            // tree
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr[i].Length; j++)
                {
                    if (arr[i][j] != 4) continue;
                    GameObject prefab = ResourceManager.Load<GameObject>("Terrains/Tree");
                    var trans = context.CreateGO(prefab, TerrainTrans);
                    trans.localPosition = new Vector3(i, 0, j);
                    trans.localEulerAngles = new Vector3(0, Random.Range(0, 30f), 0);

                }
            }

            // collider
            var colliderRoot = context.transform.Find("Terrain").Find("collider");
            {
                var nCollider = colliderRoot.Find("NCollider");
                nCollider.localScale = new Vector3(arr[0].Length, nCollider.localScale.y, nCollider.localScale.z);
                nCollider.localPosition = new Vector3(arr[0].Length / 2, nCollider.localScale.y, arr.Length);
            }
            {
                var sCollider = colliderRoot.Find("SCollider");
                sCollider.localScale = new Vector3(arr[0].Length, sCollider.localScale.y, sCollider.localScale.z);
                sCollider.localPosition = new Vector3(arr[0].Length / 2, sCollider.localScale.y, 0);
            }
            {
                var eCollider = colliderRoot.Find("ECollider");
                eCollider.localScale = new Vector3(arr.Length, eCollider.localScale.y, eCollider.localScale.z);
                eCollider.localPosition = new Vector3(arr.Length, eCollider.localScale.y, arr[0].Length/2);
            }
            {
                var wCollider = colliderRoot.Find("WCollider");
                wCollider.localScale = new Vector3(arr.Length, wCollider.localScale.y, wCollider.localScale.z);
                wCollider.localPosition = new Vector3(0, wCollider.localScale.y, arr[0].Length/2);
            }



        }

        public static void GenerateGrassDecoration(int i, int j, CombatContextManager context)
        {
            if (Random.Range(0, 15) != 1) return;
            var prefab = ResourceManager.Load<GameObject>("Terrains/GrassProp" + Random.Range(1, 8)); //1-7
            var trans = context.CreateGO(prefab, TerrainTrans);
            trans.localPosition = new Vector3(i, 0, j);
            trans.localEulerAngles = new Vector3(0, Random.Range(0, 90f), 0);
        }

       


    }
}
