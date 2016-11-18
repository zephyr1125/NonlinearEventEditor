using UnityEngine;
using UnityEditor;

public class EventEditor : EditorWindow
{

    private Event _currentEvent;

    private float _sliderValue = 1;

    private Rect _nodeRect = new Rect(64, 64, 256, 128);

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

        Node(ref _nodeRect, style);

        Repaint();
    }

    private void Node(ref Rect controlRect, GUIStyle style)
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (Event.current.GetTypeForControl(controlID)) {
            case EventType.Repaint:
                DrawNode(controlRect, style);
                break;

            case EventType.MouseDown:
                if (controlRect.Contains(Event.current.mousePosition) && Event.current.button == 0) {
                    GUIUtility.hotControl = controlID;
                }
                break;

            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID && Event.current.button == 0)
                {
                    //左键抬起
                    GUIUtility.hotControl = 0;
                    controlRect.center = new Vector2(((int) controlRect.center.x/64)*64,
                        ((int) controlRect.center.y/64)*64);
                }
                if (controlRect.Contains(Event.current.mousePosition) && Event.current.button == 1) {
                    //右键抬起
                    ShowNodeMenu();
                }
                break;
            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlID) {
                    controlRect.center += Event.current.delta;
                }
                break;
        }
    }

    private void DrawNode(Rect controlRect, GUIStyle style)
    {
        //GUI.DrawTexture(controlRect, style.normal.background);
        GUI.Label(controlRect, "", style);
        GUI.Label(new Rect(controlRect.x + 8, controlRect.y + 8, controlRect.width - 16, 16), "这里是标题");
        GUI.TextArea(new Rect(controlRect.x + 8, controlRect.y + 24, controlRect.width - 16, 64),
            "你无法摆脱恐惧。它就像大自然，你赢不了也逃不了，但只要撑过去，你就会了解你的潜力。");

        GUI.color = Color.white;
    }

    private void ShowNodeMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("←连接"), false, delegate {
            
        });
        menu.AddItem(new GUIContent("→连接"), false, delegate {

        });
        menu.ShowAsContext();
    }
}
