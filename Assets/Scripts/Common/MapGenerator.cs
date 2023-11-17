using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Common
{
    internal class MapGenerator
    {
        /// <summary>
        /// return 64*64 random map
        /// </summary>
        /// <returns></returns>
        public static int[][] RandomMap()
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

            // tree
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 4);
            SimpleRandomWalk(res, new Vector2Int(Random.Range(0, res.Length), Random.Range(0, res[0].Length)), 4, 4, 4);

            return res;
        }

        /// <summary>
        /// return 40*40 random map
        /// </summary>
        /// <returns></returns>
        public static int[][] RandomMiddleMap()
        {
            var res = new int[40][];
            for (int i = 0; i < res.Length; i++) res[i] = new int[40];

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
