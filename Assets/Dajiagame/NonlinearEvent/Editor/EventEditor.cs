using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dajiagame.NonlinearEvent.Editor
{
    public class EventEditor : EditorWindow
    {
        public static EventEditor Instance;

        private float _sliderValue = 1;

        private Rect _canvasRect = new Rect(0, 0, 2048, 2048);

        private Rect _nodeRect = new Rect(64, 64, 256, 192);

        private int _gridSize = 64;

        private Vector2 _offset = Vector2.zero;

        private GUISkin _skin;

        public Config Config;

        private string _configFilePath;

        private NonlinearEventGroup _eventGroup;

        /// <summary>
        /// 选择的类型(基于鼠标点击)
        /// </summary>
        private SelectType _selectType;

        private enum SelectType
        {
            None,   //什么都没选
            Event   //选中事件节点
        }

        [MenuItem("DajiaGame/非线性事件编辑器")]
        static void OnInit()
        {
             GetWindow<EventEditor>("非线性事件编辑器");
        }

        void OnEnable()
        {
            Instance = this;
            _skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Dajiagame/NonlinearEvent/Editor/UI/NonlinearEventEditor.guiskin");
        }

        void OnGUI()
        {
            Background();
            GUI.skin = _skin;
            ShowRightMouseMenu();
            Node(ref _nodeRect);
            GUI.skin = null;
            Repaint();
        }

        private void ShowRightMouseMenu()
        {
            Event currentEvent = Event.current;
            switch (currentEvent.type)
            {
                case EventType.MouseUp:
                    if (currentEvent.button == 1)
                    {
                        switch (_selectType) {
                            case SelectType.None:
                                ShowSystemMenu();
                                break;
                            case SelectType.Event:
                                ShowNodeMenu();
                                break;
                        }
                    }
                    break;
            }
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
                    if (Event.current.button == 2) {
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
            //if (Config == null || _eventGroup == null)
            //{
            //    return;
            //}

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
                    if (GUIUtility.hotControl == controlID && Event.current.button == 0) {
                        //左键抬起
                        GUIUtility.hotControl = 0;
                        controlRect = new Rect((int)(controlRect.x + _gridSize / 2) / _gridSize * _gridSize,
                            (int)(controlRect.y + _gridSize / 2) / _gridSize * _gridSize, controlRect.width, controlRect.height);
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

        private void ShowSystemMenu()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("新建配置"), false, CreateNewConfigFile);
            menu.AddItem(new GUIContent("读取配置"), false, LoadConfigFile);

            if (Config != null)
            {
                menu.AddItem(new GUIContent("编辑配置"), false, EditConfigFile);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("新建事件组"), false, delegate {

                });
                menu.AddItem(new GUIContent("读取事件组"), false, delegate {

                });
            }

            menu.ShowAsContext();
        }

        private void CreateNewConfigFile()
        {
            string path = EditorUtility.SaveFilePanel(
                    "选择保存位置", "Assets", "config.asset", "asset");
            if (String.IsNullOrEmpty(path))
            {
                return;
            }
            Config = CreateInstance<Config>();
            _configFilePath = Utils.AbsolutePathToAssetDataBasePath(path);
            AssetDatabase.CreateAsset(Config, _configFilePath);
            AssetDatabase.SaveAssets();
            EditConfigFile();
        }

        private void LoadConfigFile()
        {
            string path = EditorUtility.OpenFilePanel("选择配置文件", "Assets", "asset");
            if (String.IsNullOrEmpty(path)) {
                return;
            }
            _configFilePath = Utils.AbsolutePathToAssetDataBasePath(path);
            Config = AssetDatabase.LoadAssetAtPath<Config>(_configFilePath);
            EditConfigFile();
        }

        private void EditConfigFile()
        {
            GetWindow<ConfigEditor>("编辑配置");
        }
    }

}
