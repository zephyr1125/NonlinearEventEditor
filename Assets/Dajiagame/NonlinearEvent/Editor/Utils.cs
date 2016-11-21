using UnityEngine;
using UnityEngine.Assertions;

namespace Dajiagame.NonlinearEvent.Editor
{
    public class Utils
    {
        public static void DrawGrid(Rect rect, int grid)
        {
            GL.PushMatrix();
            GL.Begin(1);
            DrawGridLines(rect, grid, new Vector2(0, 0), Color.white);
            DrawGridLines(rect, grid, new Vector2(0, 0), Color.white);
            GL.End();
            GL.PopMatrix();
        }

        public static void DrawGridLines(Rect rect, float gridSize, Vector2 _offset, Color gridColor)
        {
            GL.Color(gridColor);
            for (float i = rect.x + (_offset.x < 0f ? gridSize : 0f) + _offset.x % gridSize; i < rect.x + rect.width; i = i + gridSize) {
                DrawLine(new Vector2(i, rect.y), new Vector2(i, rect.y + rect.height));
            }
            for (float j = rect.y + (_offset.y < 0f ? gridSize : 0f) + _offset.y % gridSize; j < rect.y + rect.height; j = j + gridSize) {
                DrawLine(new Vector2(rect.x, j), new Vector2(rect.x + rect.width, j));
            }
        }

        public static void DrawLine(Vector2 p1, Vector2 p2)
        {
            GL.Vertex(p1);
            GL.Vertex(p2);
        }

        /// <summary>
        /// 从绝对路径转换为AssetDataBase可用的相对路径
        /// WARNING: 目前要求必须文件存于Assets文件夹内
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public static string AbsolutePathToAssetDataBasePath(string absolutePath)
        {
            Assert.IsTrue(absolutePath.Contains(Application.dataPath),"文件不能放在Assets之外:"+ absolutePath+", "+Application.dataPath);
            return "Assets"+absolutePath.Substring(Application.dataPath.Length);
        }
    }
}
