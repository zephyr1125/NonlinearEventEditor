
using UnityEngine;

public class Utils
{
    public static void DrawGrid()
    {
        GL.PushMatrix();
        GL.Begin(1);
        DrawGridLines(new Rect(0, 0, 512, 512), 64, new Vector2(0, 0), Color.white);
        DrawGridLines(new Rect(0, 0, 512, 512), 64, new Vector2(0, 0), Color.white);
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

    public static float MyCustomSlider(Rect controlRect, float value, GUIStyle style)
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.Repaint:
                Debug.Log("Repaint");
                int pixelWidth = (int) Mathf.Lerp(8f, controlRect.width, value);
                Rect targetRect = new Rect(controlRect) {width = pixelWidth};
                GUI.color = Color.Lerp(Color.red, Color.green, value);
                GUI.DrawTexture(targetRect, style.normal.background);
                GUI.color = Color.white;
                break;

            case EventType.mouseDown:
                if (controlRect.Contains(Event.current.mousePosition) && Event.current.button == 0)
                {
                    Debug.Log("Start Control");
                    GUIUtility.hotControl = controlID;
                }
                break;

            case EventType.mouseUp:
                if (GUIUtility.hotControl == controlID)
                {
                    Debug.Log("End Control");
                    GUIUtility.hotControl = 0;
                }
                break;
        }

        if (Event.current.isMouse && GUIUtility.hotControl == controlID)
        {
            float relativeX = Event.current.mousePosition.x - controlRect.x;
            value = Mathf.Clamp01(relativeX/controlRect.width);
            Debug.Log("value to"+ value);
            GUI.changed = true;
            Event.current.Use();
        }
        return value;
    }
}