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
            Utils.DrawGrid();
            DrawNode(0);
            DrawNode(1);
        }
        GUILayout.EndArea();
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
