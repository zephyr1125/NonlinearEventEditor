using UnityEngine;
using UnityEditor;

public class EventEditor : EditorWindow
{

    private Event _currentEvent;

    [MenuItem("DajiaGame/EventEditor")]
    static void OnInit()
    {
        GetWindow<EventEditor>();
    }

    void OnGUI()
    {
        _currentEvent = Event.current;
        GUILayout.BeginArea(new Rect(0, 0, 512, 512));
        if (_currentEvent.type == EventType.Repaint)
        {
            GUIStyle canvasBackground = "flow background";
            canvasBackground.Draw(new Rect(0, 0, 512, 512), false, false, false, false);
            DrawGrid();
            DrawNode(0);
            DrawNode(1);
        }
        GUILayout.EndArea();
    }

    private void DrawGrid()
    {
        GL.PushMatrix();
        GL.Begin(1);
        DrawGridLines(new Rect(0, 0, 512, 512), 64, new Vector2(0, 0), Color.white);
        DrawGridLines(new Rect(0, 0, 512, 512), 64, new Vector2(0, 0), Color.white);
        GL.End();
        GL.PopMatrix();
    }

    private void DrawGridLines(Rect rect, float gridSize, Vector2 _offset, Color gridColor)
    {
        GL.Color(gridColor);
        for (float i = rect.x + (_offset.x < 0f ? gridSize : 0f) + _offset.x % gridSize; i < rect.x + rect.width; i = i + gridSize) {
            DrawLine(new Vector2(i, rect.y), new Vector2(i, rect.y + rect.height));
        }
        for (float j = rect.y + (_offset.y < 0f ? gridSize : 0f) + _offset.y % gridSize; j < rect.y + rect.height; j = j + gridSize) {
            DrawLine(new Vector2(rect.x, j), new Vector2(rect.x + rect.width, j));
        }
    }

    private void DrawLine(Vector2 p1, Vector2 p2)
    {
        GL.Vertex(p1);
        GL.Vertex(p2);
    }

    private void DrawNode(int id)
    {
        string nodename = "测试";
        int nodecolor = 0;
        bool isGroupStyle;
        GUIStyle style = "flow node 0";
        Rect pos = new Rect(64+8+id*128, 64+8, 128-16, 64-16);
        EditorGUI.TextArea(pos, nodename, style);
        Rect inforect = pos;			
    }
}
