using System;
using System.Collections.Generic;
using System.IO;
using NodeCanvas;
using UnityEditor;
using UnityEngine;

namespace Dajiagame.NonlinearEvent.Editor
{
    public class EventEditor : EditorWindow
    {
        public static EventEditor Instance;

        private float _sliderValue = 1;

        private Rect _canvasRect = new Rect(0, 0, 20480, 20480);

        private int _gridSize = 64;

        private Vector2 _nodeSize = new Vector2(256, 192);

        private Vector2 _rootNodePositon = new Vector2(256, 128);

        private int _widthRight = 256;

        private Vector2 _offset = Vector2.zero;

        private GUISkin _skin;

        public NonlinearEventGroup EventGroup;

        private string _eventGroupFilePath;

        private EventNode _selectedNode;

        private int _lastEventID;

        private List<Transition> _listTransitions;

        /// <summary>
        /// 正在连线时的当前连接编号
        /// </summary>
        private int _currentTransitionType;

        private State _state;

        private enum State
        {
            Idle,
            ConnectTransition
        }

        private string[] _popUpCharacterNames;

        [MenuItem("DajiaGame/非线性事件编辑器")]
        static void OnInit()
        {
             GetWindow<EventEditor>("非线性事件编辑器");
        }

        void OnEnable()
        {
            Instance = this;
            _skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/Dajiagame/NonlinearEvent/Editor/UI/NonlinearEventEditor.guiskin");
            ReloadTransitions();
        }

        void OnGUI()
        {
            Background();
            GUI.skin = _skin;
            ShowRightMouseMenu();
            Transitions();
            DrawEventNodes();
            DrawRightPanel();
            GUI.skin = null;
            Repaint();
        }

        private void DrawEventNodes()
        {
            if (EventGroup == null)
            {
                return;
            }
            if (EventGroup.ListNodes == null) {
                return;
            }
            foreach (var eventNode in EventGroup.ListNodes)
            {
                Node(eventNode);
            }
        }

        private void ShowRightMouseMenu()
        {
            if (_state != State.Idle)
            {
                return;
            }
            Event currentEvent = Event.current;
            switch (currentEvent.type)
            {
                case EventType.MouseUp:
                    if (currentEvent.button == 1)
                    {
                        if (_selectedNode != null && PointInNodeDrawRect(_selectedNode, Event.current.mousePosition))
                        {
                            ShowNodeMenu();
                        }
                        else
                        {
                            ShowSystemMenu(currentEvent);
                        }
                    }
                    break;
            }
        }

        private void DrawRightPanel()
        {
            if (_selectedNode == null)
            {
                return;
            }
            Rect rightRect = new Rect(Screen.width - _widthRight, 0, _widthRight, Screen.height);
            GUILayout.BeginArea(rightRect, _skin.GetStyle("Right"));
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label("设置节点数据",_skin.GetStyle("Title"));
                _selectedNode.CharacterID = EditorGUILayout.Popup("说话者", _selectedNode.CharacterID, _popUpCharacterNames);
                GUILayout.Label("简介文本");
                _selectedNode.PreviewText = EditorGUILayout.TextField(_selectedNode.PreviewText);
                GUILayout.Label("主文本");
                _selectedNode.Text = EditorGUILayout.TextField(_selectedNode.Text, _skin.GetStyle("Talk"), GUILayout.Height(128));
            }
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
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

        private Rect GetNodeDrawRect(EventNode node)
        {
            Rect controlRect = new Rect(node.Position, _nodeSize);
            return new Rect(controlRect.position + _offset, controlRect.size);
        }

        private bool PointInNodeDrawRect(EventNode node, Vector2 point)
        {
            return GetNodeDrawRect(node).Contains(point);
        }

        private void Node(EventNode node)
        {
            Rect controlRect = new Rect(node.Position, _nodeSize);
            Rect drawRect = GetNodeDrawRect(node);

            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (Event.current.GetTypeForControl(controlID)) {
                case EventType.Repaint:
                    DrawNode(node, drawRect);
                    break;

                case EventType.MouseDown:
                    if (PointInNodeDrawRect(node, Event.current.mousePosition) && Event.current.button == 0) {
                        GUIUtility.hotControl = controlID;
                    }
                    break;
                case EventType.MouseUp:
                    if (PointInNodeDrawRect(node, Event.current.mousePosition))
                    {
                        if (Event.current.button == 0) {
                            if (GUIUtility.hotControl == controlID)
                            {
                                //左键抬起
                                //拖拽完成
                                GUIUtility.hotControl = 0;
                                node.Position = new Vector2((int) (controlRect.x + _gridSize/2)/_gridSize*_gridSize,
                                    (int) (controlRect.y + _gridSize/2)/_gridSize*_gridSize);
                                EditorUtility.SetDirty(EventGroup);
                                //选中Node
                                _selectedNode = node;
                            }
                        }
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID) {
                        node.Position += Event.current.delta;
                    }
                    break;
            }
        }

        private void DrawNode(EventNode node, Rect controlRect)
        {
            Rect drawRect = new Rect(controlRect.x + 16, controlRect.y + 16, controlRect.width - 32, controlRect.height - 32);           
            GUI.Label(drawRect, "", _skin.GetStyle("Node"));
            if (node == _selectedNode) {
                GUI.color = Color.cyan;
            }
            GUI.Label(new Rect(drawRect.x, drawRect.y, 24, 24), ""+node.ID, _skin.GetStyle("ID"));
            GUI.Label(new Rect(drawRect.x+23, drawRect.y, drawRect.width-23, 24), node.PreviewText, _skin.GetStyle("Title"));
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(drawRect.x, drawRect.y + 23, 48, 48), EventGroup.Config.Characters[node.CharacterID].Icon);
            GUI.Label(new Rect(drawRect.x, drawRect.y + 23 + 47, 48, 16), EventGroup.Config.Characters[node.CharacterID].Name, _skin.GetStyle("CharName"));
            GUI.Label(new Rect(drawRect.x + 47, drawRect.y + 23, drawRect.width-47, 64),
                node.Text, _skin.GetStyle("Talk"));
        }

        private void ShowNodeMenu()
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < EventGroup.Config.Transitions.Count; i++)
            {
                var selection = EventGroup.Config.Transitions[i];
                var currentTransitionType = i;
                menu.AddItem(new GUIContent(selection.Name), false, delegate
                {
                    _state = State.ConnectTransition;
                    _currentTransitionType = currentTransitionType;
                });
            }
            menu.ShowAsContext();
        }

        private void CreateNewNode(Vector2 createAt)
        {
            if (EventGroup.ListNodes == null)
            {
                EventGroup.ListNodes = new List<EventNode>();
                _lastEventID = 1;
            }
            EventNode newNode = new EventNode
            {
                ID =  _lastEventID,
                Position = createAt
            };
            EventGroup.ListNodes.Add(newNode);
            _lastEventID++;
        }

        #endregion

        private void ShowSystemMenu(Event currentEvent)
        {
            GenericMenu menu = new GenericMenu();

            if (EventGroup != null) {
                menu.AddItem(new GUIContent("新建节点"), false, delegate
                {
                    CreateNewNode(currentEvent.mousePosition);
                });
                menu.AddItem(new GUIContent("调整设置"), false, EditConfigFile);
                menu.AddSeparator("");
            }

            menu.AddItem(new GUIContent("新建文件"), false, CreateNewFile);
            menu.AddItem(new GUIContent("读取文件"), false, LoadFile);

            if (EventGroup != null) {
                menu.AddItem(new GUIContent("保存文件"), false, delegate { });
                menu.AddItem(new GUIContent("另存为…"), false, delegate { });
            }

            menu.ShowAsContext();
        }

#region ConfigFile

        private void EditConfigFile()
        {
            GetWindow<ConfigEditor>("编辑配置");
        }

#endregion

#region EventGroupFile

        private void CreateNewFile()
        {
            string path = EditorUtility.SaveFilePanel(
                    "选择保存位置", "Assets", "event_group.asset", "asset");
            if (string.IsNullOrEmpty(path)) {
                return;
            }
            EventGroup = CreateInstance<NonlinearEventGroup>();
            _eventGroupFilePath = Utils.AbsolutePathToAssetDataBasePath(path);
            AssetDatabase.CreateAsset(EventGroup, _eventGroupFilePath);
            AssetDatabase.SaveAssets();
            EditConfigFile();
        }

        private void LoadFile()
        {
            string path = EditorUtility.OpenFilePanel("选择文件", "Assets", "asset");
            if (string.IsNullOrEmpty(path)) {
                return;
            }
            _eventGroupFilePath = Utils.AbsolutePathToAssetDataBasePath(path);
            EventGroup = AssetDatabase.LoadAssetAtPath<NonlinearEventGroup>(_eventGroupFilePath);
            _lastEventID = EventGroup.GetLastEventID() + 1;
            ReloadTransitions();
            UpdatePopUpCharacterNames();
        }

        public void UpdatePopUpCharacterNames()
        {
            Config config = EventGroup.Config;
            _popUpCharacterNames = new string[config.Characters.Count];
            for (int i = 0; i < config.Characters.Count; i++) {
                var character = config.Characters[i];
                _popUpCharacterNames[i] = character.Name;
            }
        }

        private void ReloadTransitions()
        {
            _listTransitions = new List<Transition>();
            if (EventGroup == null)
            {
                return;
            }

            if (EventGroup.ListNodes == null)
            {
                return;
            }

            foreach (var eventNode in EventGroup.ListNodes)
            {
                for (int i = 0; i < eventNode.NextEventIDs.Count; i++)
                {
                    var nextEventID = eventNode.NextEventIDs[i];
                    if (nextEventID > 0)
                    {
                        _listTransitions.Add(new Transition(eventNode.ID, nextEventID, i));
                    }
                }
            }
        }

        #endregion

        #region Transition

        private class Transition
        {
            public int StartNode;
            public int EndNode;
            public int TransitionType;

            public Transition(int startNode, int endNode, int transitionType)
            {
                StartNode = startNode;
                EndNode = endNode;
                TransitionType = transitionType;
            }
        }

        private void Transitions()
        {
            if (_state == State.ConnectTransition)
            {
                Event currentEvent = Event.current;
                switch (currentEvent.type) {
                    case EventType.MouseUp:
                        if (currentEvent.button == 0)
                        {
                            //遍历Node查找点中的
                            bool found = false;
                            foreach (var eventNode in EventGroup.ListNodes)
                            {
                                if (PointInNodeDrawRect(eventNode, currentEvent.mousePosition))
                                {
                                    _selectedNode.AddNextEventNode(_currentTransitionType, eventNode.ID);
                                    AddTransition(_selectedNode.ID, eventNode.ID, _currentTransitionType);
                                    found = true;
                                }
                            }
                            //如果没有点中的，表示删除原有连接
                            if (!found)
                            {
                                Transition exist =
                                    _listTransitions.Find(
                                        _ =>
                                            _.StartNode == _selectedNode.ID &&
                                            _.TransitionType == _currentTransitionType);
                                if (exist != null)
                                {
                                    _listTransitions.Remove(exist);
                                    _selectedNode.RemoveNextEventNode(_currentTransitionType);
                                }
                            }
                            _state = State.Idle;
                        }
                        if (currentEvent.button == 1)
                        {
                            //右键取消连接操作
                            _state = State.Idle;
                        }
                        break;
                }
            }

            DrawTransitions();

            DrawMouseTransition();
        }

        private void DrawTransitions()
        {
            if (_listTransitions == null)
            {
                return;
            }
            foreach (var transition in _listTransitions)
            {
                if (transition.TransitionType < EventGroup.Config.Transitions.Count)
                {
                    Color color = EventGroup.Config.Transitions[transition.TransitionType].Color;
                    DrawTransition(transition, color);
                }
                
            }
        }

        private void DrawMouseTransition()
        {
            if (_state != State.ConnectTransition)
            {
                return;
            }
            if (_selectedNode == null)
            {
                return;
            }
            Color color = EventGroup.Config.Transitions[_currentTransitionType].Color;
            DrawTransitionToMouse(_selectedNode.ID, Event.current.mousePosition, color);
        }

        private void DrawTransition(Transition transition, Color color)
        {
            Handles.color = color;
            Vector2 posStart = EventGroup.GetNode(transition.StartNode).Position+_nodeSize/2;
            Vector2 posEnd = EventGroup.GetNode(transition.EndNode).Position + _nodeSize / 2;
            Handles.DrawBezier(posStart + _offset, posEnd + _offset, posStart + _offset, posEnd + _offset, color, NodeStyles.connectionTexture, 3f);
            DrawArrow(posStart+_offset, posEnd+_offset);
        }

        private void DrawTransitionToMouse(int nodeID, Vector2 posMouse, Color color)
        {
            Handles.color = color;
            Vector2 posStart = EventGroup.GetNode(nodeID).Position + _nodeSize / 2;
            Handles.DrawBezier(posStart + _offset, posMouse, posStart + _offset, posMouse, color, NodeStyles.connectionTexture, 3f);
            DrawArrow(posStart + _offset, posMouse);
        }

        private void DrawArrow(Vector2 posStart, Vector2 posEnd)
        {
            float dis = Vector2.Distance(posEnd, posStart);
            Vector2 nor = (posEnd - posStart).normalized;
            Vector2 cross = nor * (dis * 0.2f) + posStart;
            Quaternion q = Quaternion.FromToRotation(Vector3.back, posStart - posEnd);
            //		Handles.Label (cross, arrows.ToString());
            cross = nor * (dis * 0.5f) + posStart;
            Handles.ConeCap(0, new Vector3(cross.x, cross.y, -100), q, 18);
        }

        private void AddTransition(int startNodeID, int endNodeID, int transitionType)
        {
            if (_listTransitions == null)
            {
                _listTransitions = new List<Transition>();
            }
            Transition exist =
                _listTransitions.Find(_ => _.StartNode == startNodeID && _.TransitionType == transitionType);
            if (exist!=null)
            {
                exist.EndNode = endNodeID;
            } else
            {
                _listTransitions.Add(new Transition(startNodeID, endNodeID, transitionType));
            }
            
        }

        #endregion

    }
}
