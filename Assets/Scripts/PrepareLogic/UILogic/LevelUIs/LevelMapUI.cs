using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PrepareLogic.UILogic.LevelUIs
{
    internal class LevelMapUI : MonoBehaviour
    {
        Color grass = new Color(0, 0.5f, 0);
        Color waterBlue = new Color(0, 0.74f, 1);
        Color treeGreen = new Color(0.133f, 0.545f, 0.133f);
        public void DrawMap(int[][] map, RectInt teamSpawn, RectInt enemySpawn)
        {
            // 根据map的不同值，绘制不同颜色
            var ri = GetComponent<RawImage>();

            int width = map.Length;
            int height = map[0].Length;

            Texture2D texture = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color color;

                    if (map[x][y] == 0) 
                    {
                        color = grass;
                    }
                    else if (map[x][y] == 1)
                    {
                        color = grass;
                    }
                    else if (map[x][y] == 2)
                    {
                        color = Color.yellow;
                    }
                    else if (map[x][y] == 3)
                    {
                        color = waterBlue;
                    }
                    else
                    {
                        color = treeGreen;
                    }

                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();

            ri.texture = texture;

            // 出生点图示
            var tTrans = transform.Find("TSpawnRawImg");
            var eTrans = transform.Find("ESpawnRawImg");
            ((RectTransform)tTrans).anchoredPosition = teamSpawn.center * ((float)250 / map.Length);
            ((RectTransform)eTrans).anchoredPosition = enemySpawn.center * ((float)250 / map.Length);
        }
    }
}
