using System.Collections.Generic;
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

        private Vector2 _effectDrawSize = new Vector2(38, 18);

        private int _widthRight = 256;

        private Vector2 Offset
        {
            get { return _offset; }
            set
            {
                _offset = value;
                float rightMax = 0 - (_canvasRect.width - (position.width - _widthRight));
                float downMax = 0 - (_canvasRect.height - position.height);
                if (_offset.x > 0) _offset.x = 0;
                if (_offset.x < rightMax) _offset.x = rightMax;
                if (_offset.y > 0) _offset.y = 0;
                if (_offset.y < downMax) _offset.y = downMax;
            }
        }
        private Vector2 _offset = Vector2.zero;

        private GUISkin _skin;

        public NonlinearEventGroup EventGroup;

        private string _eventGroupFilePath;

        private EventNode _selectedNode;

        /// <summary>
        /// 当前选中的Node的各个Selection的Rect,为右键菜单的定位缓存下来
        /// </summary>
        private List<Rect> _selectedNodeSelectionRects; 

        private int _lastEventID;

        private List<Transition> _listTransitions;

        /// <summary>
        /// 正在连线时的当前连接编号
        /// </summary>
        private int _currentTransitionType;

        private State _state;

        private Color[] SelectionColors = {
            new Color(1,0.7f,0.7f), 
            Color.green
        };

        private enum State
        {
            Idle,
            ConnectTransition
        }

        [SerializeField]
        private string[] _popUpCharacterNames;

        [MenuItem("DajiaGame/非线性事件编辑器")]
        static void OnInit()
        {
             GetWindow<EventEditor>("非线性事件编辑器");
        }

        void Awake()
        {
            if (EventGroup != null && EventGroup.ListNodes.Count > 0)
            {
                OffsetBackToRoot();
            }
            else
            {
                Offset = -new Vector2(position.width / 2, 0);
            }
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
            OffsetBackToRoot();
            GUI.skin = _skin;
            ShowRightMouseMenu();
            Transitions();
            DrawEventNodes();
            DrawRightPanel();
            GUI.skin = null;
            Repaint();
        }

        private void OffsetBackToRoot()
        {
            Event currentEvent = Event.current;
            if (currentEvent == null) return;
            switch (currentEvent.type) {
                case EventType.KeyUp:
                    if (currentEvent.keyCode == KeyCode.Home)
                    {
                        Debug.Log("Home");
                        if (EventGroup == null) return;
                        if(EventGroup.ListNodes.Count==0)return;
                        Vector2 rootPos = EventGroup.ListNodes[0].Position;
                        float winWidth = position.width - _widthRight;
                        Offset = -new Vector2(rootPos.x- winWidth/2, rootPos.y);
                    }
                    break;
            }
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
                DrawRightPanelRequires();
                DrawRightPanelTalk();
                for (int i = 0; i < _selectedNode.Selections.Count; i++)
                {
                    DrawRightPanelSelection(i);
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DrawRightPanelRequires()
        {
            GUILayout.Label("解锁条件", _skin.GetStyle("Title"));
            DrawEffectGroup(_selectedNode.Requires, -1);
        }

        private void DrawRightPanelTalk()
        {
            GUILayout.Label("设置文本", _skin.GetStyle("Title"));
            _selectedNode.CharacterID = EditorGUILayout.Popup("说话者", _selectedNode.CharacterID, _popUpCharacterNames);
            GUILayout.Label("文本内容");
            _selectedNode.Text = EditorGUILayout.TextField(_selectedNode.Text, _skin.GetStyle("Talk"), GUILayout.Height(128));
        }

        private void DrawRightPanelSelection(int selectionID)
        {
            GUILayout.Label("选项"+(selectionID+1)+"设置", _skin.GetStyle("Title"));
            GUILayout.Label("选项文本");
            _selectedNode.Selections[selectionID].Text = GUILayout.TextField(
                _selectedNode.Selections[selectionID].Text, _skin.GetStyle("Talk"));
            DrawEffectGroup(_selectedNode.Selections[selectionID].Effects, 0);
        }

        /// <summary>
        /// 绘制一组Effect数据
        /// </summary>
        /// <param name="effects"></param>
        /// <param name="defaultNum">如果节点的Effect数组不够长，以defaultNum填充</param>
        private void DrawEffectGroup(List<int> effects, int defaultNum)
        {
            for (int i = 0; i < EventGroup.Config.Effects.Count; i++) {
                if (i % 2 == 0) EditorGUILayout.BeginHorizontal();
                while (effects.Count <= i) {
                    effects.Add(defaultNum);
                };
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent(EventGroup.Config.Effects[i].Name, EventGroup.Config.Effects[i].Icon), GUILayout.Width(48));
                effects[i] = EditorGUILayout.IntField(effects[i], GUILayout.Width(48));
                EditorGUILayout.EndHorizontal();
                if (i % 2 == 1 || i == EventGroup.Config.Effects.Count - 1) EditorGUILayout.EndHorizontal();
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
                        Offset += Event.current.delta;
                    }
                    break;

            }
        }

        private void DrawBackground()
        {
            Rect drawRect = new Rect(_canvasRect.position + Offset, _canvasRect.size);
            GUIStyle canvasBackground = "flow background";
            canvasBackground.Draw(drawRect, false, false, false, false);
            Utils.DrawGrid(drawRect, _gridSize);
        }

        #endregion

        #region node

        private Rect GetNodeDrawRect(EventNode node)
        {
            Rect controlRect = new Rect(node.Position, _nodeSize);
            return new Rect(controlRect.position + Offset, controlRect.size);
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
            GUIStyle guiStyle;
            if (node == _selectedNode)
            {
                guiStyle = _skin.GetStyle("NodeSelected");
            }
            else
            {
                guiStyle = _skin.GetStyle("Node");
            }

            Rect drawRect = new Rect(controlRect.x + 16, controlRect.y + 16, controlRect.width - 32, controlRect.height - 32);       
            GUI.Label(drawRect, "", guiStyle);

            int height = 18;
            int y = (int)drawRect.y;
            
            GUI.Label(new Rect(drawRect.x, y, 24, height), ""+node.ID, _skin.GetStyle("ID"));

            DrawRequiresInNode(node, new Rect(drawRect.x + 23, y, drawRect.width-24, height));

            y += height - 1;
            height = 64;
            GUI.DrawTexture(new Rect(drawRect.x, y+1, 48, 48), EventGroup.Config.Characters[node.CharacterID].Icon);
            GUI.Label(new Rect(drawRect.x, y+1 + 47, 48, 16), EventGroup.Config.Characters[node.CharacterID].Name, _skin.GetStyle("CharName"));
            GUI.Label(new Rect(drawRect.x + 47, y, drawRect.width - 47, height), node.Text, _skin.GetStyle("Talk"));

            y += height - 1;
            height = (int)(drawRect.height-(y-drawRect.y));

            int width = (int)drawRect.width/ node.Selections.Count;
            for (int i = 0; i < node.Selections.Count; i++)
            {
                Rect rect = new Rect(drawRect.x + width*i - i, y, width, height);
                DrawNodeSelection(rect, node.Selections[i], i);
                if (node == _selectedNode)
                {
                    UpdateSelectedNodeSelectionRect(i, rect);
                }
            }
        }

        private void DrawRequiresInNode(EventNode node, Rect drawRect)
        {
            int p = 1;
            for (int i = EventGroup.Config.Effects.Count - 1; i >= 0; i--)
            {
                if (node.Requires[i] == -1) continue;

                DrawEffect(new Vector2(drawRect.x+drawRect.width-(_effectDrawSize.x-1)* p, drawRect.y), i, node.Requires[i]);
                p++;
            }
        }

        private Color GetSelectionColor(int selectionID)
        {
            if (selectionID < SelectionColors.Length)
            {
                return SelectionColors[selectionID];
            }
            else
            {
                return Color.white;
            }
            
        }

        /// <summary>
        /// 更新缓存的Node的Selection的绘制Rect
        /// </summary>
        /// <param name="seletionID"></param>
        /// <param name="rect"></param>
        private void UpdateSelectedNodeSelectionRect(int seletionID, Rect rect)
        {
            if (_selectedNodeSelectionRects == null)
            {
                _selectedNodeSelectionRects = new List<Rect>();
            }
            while (_selectedNodeSelectionRects.Count<= seletionID)
            {
                _selectedNodeSelectionRects.Add(new Rect(0,0,0,0));
            }
            _selectedNodeSelectionRects[seletionID] = rect;
        }

        private void DrawNodeSelection(Rect drawRect, EventNode.Selection selection, int selectionID)
        {
            GUI.Label(drawRect, "", _skin.GetStyle("Node"));
            //文字
            Color prevContentColor = GUI.contentColor;
            GUI.contentColor = GetSelectionColor(selectionID);
            GUI.Label(new Rect(drawRect.x, drawRect.y, drawRect.width, 18), selection.Text, _skin.GetStyle("SelectionText"));
            GUI.contentColor = prevContentColor;
            //属性效果
            int drawID = 0;
            for (int i = 0; i < selection.Effects.Count; i++)
            {
                int effect = selection.Effects[i];
                if (effect == 0) continue;
                Vector2 pos = new Vector2(drawRect.x, drawRect.y + 17);
                pos+=new Vector2((drawID % 3) * (_effectDrawSize.x-1), drawID / 3 * (_effectDrawSize.y - 1));
                DrawEffect(pos, i, effect);
                drawID++;
            }
        }

        /// <summary>
        /// 绘制一组Effect数据
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="effectID"></param>
        /// <param name="effectValue"></param>
        /// <returns>返回绘制的Rect</returns>
        private void DrawEffect(Vector2 pos, int effectID, int effectValue)
        {
            var effectRect = new Rect(pos, _effectDrawSize);
            GUI.Label(effectRect, "", _skin.GetStyle("Node"));
            GUI.DrawTexture(new Rect(effectRect.x + 1, effectRect.y + 1, 16, 16), EventGroup.Config.Effects[effectID].Icon);
            GUI.Label(new Rect(effectRect.x + 17, effectRect.y, _effectDrawSize.x-17, _effectDrawSize.y), "" + effectValue, _skin.GetStyle("EffectNum"));
        }

        private void ShowNodeMenu()
        {
            GenericMenu menu = new GenericMenu();

            //在各个选项的绘制范围内的话，右键菜单是连接，否则是删除
            bool inSelection = false;
            for (int i = 0; i < _selectedNodeSelectionRects.Count; i++)
            {
                var selectedNodeSelectionRect = _selectedNodeSelectionRects[i];
                if (!selectedNodeSelectionRect.Contains(Event.current.mousePosition)) continue;
                var currentTransitionType = i;
                menu.AddItem(new GUIContent("连接"), false, delegate
                {
                    _state = State.ConnectTransition;
                    _currentTransitionType = currentTransitionType;
                });
                inSelection = true;
                break;
            }

            if (!inSelection)
            {
                menu.AddItem(new GUIContent("删除节点"), false, DeleteNode);
            }
            
            menu.ShowAsContext();
        }

        private void DeleteNode()
        {
            //所有现有node断开与其的连接
            EventGroup.ListNodes.ForEach(node =>
            {
                node.Selections.ForEach(selection =>
                {
                    if (selection.NextEventID == _selectedNode.ID)
                    {
                        selection.NextEventID = 0;
                    }
                });
            });
            //所有暂存Transition移除相关连接
            _listTransitions.RemoveAll(transition => transition.StartNode == _selectedNode.ID || transition.EndNode == _selectedNode.ID);
            //删除node
            EventGroup.ListNodes.Remove(_selectedNode);
            _selectedNode = null;

        }

        private void CreateNewNode(Vector2 createAt)
        {
            if (EventGroup.ListNodes == null)
            {
                EventGroup.ListNodes = new List<EventNode>();
                _lastEventID = 1;
            }
            EventNode newNode = new EventNode(EventGroup.Config.Effects.Count)
            {
                ID = _lastEventID,
                Position = createAt
            };
            for (int i = 0; i < EventGroup.Config.DefaultSelectionCount; i++)
            {
                AddNodeSelection(newNode);
            }
            EventGroup.ListNodes.Add(newNode);
            _lastEventID++;
        }

        private void AddNodeSelection(EventNode node)
        {
            node.Selections.Add(new EventNode.Selection(EventGroup.Config.Effects.Count));
        }

        #endregion

        private void ShowSystemMenu(Event currentEvent)
        {
            GenericMenu menu = new GenericMenu();

            if (EventGroup != null) {
                menu.AddItem(new GUIContent("新建节点"), false, delegate
                {
                    CreateNewNode(currentEvent.mousePosition- Offset);
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
                for (int i = 0; i < eventNode.Selections.Count; i++)
                {
                    var nextEventID = eventNode.Selections[i].NextEventID;
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
            if (_listTransitions == null) {
                return;
            }
            foreach (var transition in _listTransitions) {
                Color color = GetSelectionColor(transition.TransitionType);
                DrawTransition(transition, color);
            }
        }

        private void DrawMouseTransition()
        {
            if (_state != State.ConnectTransition) {
                return;
            }
            if (_selectedNode == null) {
                return;
            }
            Color color = Color.white;
            DrawTransitionToMouse();
        }

        private void DrawTransition(Transition transition, Color color)
        {
            Handles.color = color;
            EventNode startNode = EventGroup.GetNode(transition.StartNode);
            int selectionCount = startNode.Selections.Count;
            int selectionRectWidth = (int)_nodeSize.x / selectionCount;
            Vector2 posStart = startNode.Position + new Vector2(selectionRectWidth * transition.TransitionType + selectionRectWidth / 2, _nodeSize.y * 3 / 4);
            Vector2 posEnd = EventGroup.GetNode(transition.EndNode).Position + new Vector2(_nodeSize.x/2, _nodeSize.y/4);
            Handles.DrawBezier(posStart + Offset, posEnd + Offset, posStart + Offset, posEnd + Offset, color, NodeStyles.connectionTexture, 3f);
            DrawArrow(posStart+ Offset, posEnd+ Offset);
        }

        private void DrawTransitionToMouse()
        {
            Color color = GetSelectionColor(_currentTransitionType);
            Handles.color = color;
            Vector2 posMouse = Event.current.mousePosition;
            int selectionCount = _selectedNode.Selections.Count;
            int selectionRectWidth = (int)_nodeSize.x/selectionCount;
            Vector2 posStart = _selectedNode.Position+new Vector2(selectionRectWidth*_currentTransitionType+selectionRectWidth/2, _nodeSize.y*3/4);
            Handles.DrawBezier(posStart + Offset, posMouse, posStart + Offset, posMouse, color, NodeStyles.connectionTexture, 3f);
            DrawArrow(posStart + Offset, posMouse);
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
