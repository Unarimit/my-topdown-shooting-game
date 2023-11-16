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
                    if (arr[i][j] == 1) prefab = Resources.Load<GameObject>("Terrains/Road");
                    else if(arr[i][j] == 2) prefab = Resources.Load<GameObject>("Terrains/Mountain");
                    else if (arr[i][j] == 3) prefab = Resources.Load<GameObject>("Terrains/Water");
                    else prefab = Resources.Load<GameObject>("Terrains/Grass");
                    var trans = context.CreateGO(prefab, TerrainTrans);
                    trans.localPosition = new Vector3(i, 0, j);
                    
                }
            }
        }

        /// <summary>
        /// return 64*64 random terrain
        /// </summary>
        /// <returns></returns>
        public static int[][] RandomTerrain()
        {
            var res = new int[64][];
            for (int i = 0; i < 64; i++) res[i] = new int[64];

            // road
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);

            // mountain
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 2);
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 2);
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 2);
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 6, 4, 2);
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 6, 4, 2);

            // water
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);

            return res;
        }

        private static void SimpleRandomWalk(int[][] map, Vector2Int startPos, int walkLength, int iterations, int aim)
        {
            HashSet<Vector2Int> path = new HashSet<Vector2Int>();
            path.Add(startPos);

            List<Vector2Int> directions = new List<Vector2Int> { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };
            for(int _ = 0; _ < iterations; _++)
            {
                var prevPos = path.ElementAt(Random.Range(0, path.Count));
                for (int i = 0; i < walkLength; i++)
                {
                    var newPos = prevPos + directions[Random.Range(0, directions.Count)];
                    path.Add(newPos);
                    prevPos = newPos;
                }
            }
            foreach(var pos in path)
            {
                if (pos.x < 0 || pos.x >= map.Length || pos.y < 0 || pos.y >= map[pos.x].Length) continue;

                map[pos.x][pos.y] = aim;
            }
        }


    }
}
