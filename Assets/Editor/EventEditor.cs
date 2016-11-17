using UnityEngine;
using UnityEditor;

public class EventEditor : EditorWindow
{

    private Event _currentEvent;

    private float _sliderValue = 1;

    private Rect _nodeRect = new Rect(64, 64, 128, 128);

    [MenuItem("DajiaGame/EventEditor")]
    static void OnInit()
    {
        GetWindow<EventEditor>();
    }

    void OnGUI()
    {

        _currentEvent = Event.current;
        if (_currentEvent.type == EventType.Repaint) {
            GUIStyle canvasBackground = "flow background";
            canvasBackground.Draw(new Rect(0, 0, 512, 512), false, false, false, false);
            Utils.DrawGrid();

        }

        GUIStyle style = "flow node 0";

        Utils.Node(ref _nodeRect, style);

        Repaint();
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
