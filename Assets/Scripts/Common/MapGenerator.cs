using Assets.Scripts.Entities.Level;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Common
{
    internal class MapGenerator
    {

        public static int[][] RandomMap(MapType mapSize)
        {
            // TODO：finish small map and big map generation
            if (mapSize == MapType.Big) return randomBigMap();
            else if (mapSize == MapType.Middle) return randomMiddleMap();
            else return randomBigMap();
        }

        public static int[][] GetInvasionMap()
        {
            int rseed = Random.Range(1, 1000000000);
            Random.InitState(25);
            var res = new int[80][];
            for (int i = 0; i < 80; i++) res[i] = new int[80];
            // road
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);

            // mountain
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 2);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 2);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 2);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 6, 4, 2);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 6, 4, 2);

            // water
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);

            // tree
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 4);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 4);

            Random.InitState(rseed);
            return res;
        }

        /// <summary>
        /// return 64*64 random map
        /// </summary>
        /// <returns></returns>
        private static int[][] randomBigMap()
        {
            var res = new int[64][];
            for (int i = 0; i < 64; i++) res[i] = new int[64];

            // road
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);

            // mountain
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 2);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 2);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 2);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 6, 4, 2);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 6, 4, 2);

            // water
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);

            // tree
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 4);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 4);

            return res;
        }
        /// <summary>
        /// return 40*40 random map
        /// </summary>
        /// <returns></returns>
        private static int[][] randomMiddleMap()
        {
            var res = new int[40][];
            for (int i = 0; i < res.Length; i++) res[i] = new int[40];

            // road
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 8, 1, 1);

            // mountain
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 2);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 2);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 2);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 6, 4, 2);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 6, 4, 2);

            // water
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);
            simpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 3);

            return res;
        }

        private static void simpleRandomWalk(int[][] map, Vector2Int startPos, int walkLength, int iterations, int aim)
        {
            HashSet<Vector2Int> path = new HashSet<Vector2Int>();
            path.Add(startPos);

            List<Vector2Int> directions = new List<Vector2Int> { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };
            for (int _ = 0; _ < iterations; _++)
            {
                var prevPos = path.ElementAt(Random.Range(0, path.Count));
                for (int i = 0; i < walkLength; i++)
                {
                    var newPos = prevPos + directions[Random.Range(0, directions.Count)];
                    path.Add(newPos);
                    prevPos = newPos;
                }
            }
            foreach (var pos in path)
            {
                if (pos.x < 0 || pos.x >= map.Length || pos.y < 0 || pos.y >= map[pos.x].Length) continue;

                map[pos.x][pos.y] = aim;
            }
        }
    }
}
