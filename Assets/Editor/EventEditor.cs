using UnityEngine;
using UnityEditor;

public class EventEditor : EditorWindow
{

    private float _sliderValue = 1;

    private Rect _canvasRect = new Rect(0, 0, 2048, 2048);

    private Rect _nodeRect = new Rect(64, 64, 320, 192);

    private int _gridSize = 64;

    private Vector2 _offset = Vector2.zero;

    private GUISkin _skin;

    [MenuItem("DajiaGame/EventEditor")]
    static void OnInit()
    {
        GetWindow<EventEditor>();
    }

    void Awake()
    {
        _skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Editor/NonlinearEventEditor.guiskin");
    }

    void OnGUI()
    {
        Background();
        GUI.skin = _skin;
        Node(ref _nodeRect);
        GUI.skin = null;
        Repaint();
    }

#region background

    private void Background()
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (Event.current.GetTypeForControl(controlID)) {
            case EventType.Repaint:
                DrawBackground();
                break;
            case EventType.MouseDrag:
                if (Event.current.button == 2)
                {
                    //中键拖拽整个工作区
                    _offset += Event.current.delta;
                }
                break;

        }
    }

    private void DrawBackground()
    {
        Rect drawRect = new Rect(_canvasRect.position + _offset, _canvasRect.size);
        GUIStyle canvasBackground = "flow background";
        canvasBackground.Draw(drawRect, false, false, false, false);
        Utils.DrawGrid(drawRect, _gridSize);
    }

#endregion

#region node

    private void Node(ref Rect controlRect)
    {
        Rect drawRect = new Rect(controlRect.position + _offset, controlRect.size);
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (Event.current.GetTypeForControl(controlID)) {
            case EventType.Repaint:
                DrawNode(drawRect);
                break;

            case EventType.MouseDown:
                if (drawRect.Contains(Event.current.mousePosition) && Event.current.button == 0) {
                    GUIUtility.hotControl = controlID;
                }
                break;

            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID && Event.current.button == 0)
                {
                    //左键抬起
                    GUIUtility.hotControl = 0;
                    controlRect = new Rect((int)(controlRect.x+_gridSize/2)/_gridSize* _gridSize,
                        (int)(controlRect.y + _gridSize / 2) / _gridSize * _gridSize, controlRect.width, controlRect.height);
                }
                if (drawRect.Contains(Event.current.mousePosition) && Event.current.button == 1) {
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

    private void DrawNode(Rect controlRect)
    {
        Rect drawRect = new Rect(controlRect.x + 16, controlRect.y + 16, controlRect.width - 32, controlRect.height - 32);
        GUI.Label(drawRect, "", _skin.GetStyle("Node"));
        GUI.Label(new Rect(drawRect.x, drawRect.y, drawRect.width, 24), "这里是标题", _skin.GetStyle("Title"));
        GUI.TextArea(new Rect(drawRect.x, drawRect.y + 23, drawRect.width, 64),
            "本电子邮件为系统自动发送，请勿直接回复。如有问题，请回复邮箱至", _skin.GetStyle("Talk"));

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

#endregion

}
