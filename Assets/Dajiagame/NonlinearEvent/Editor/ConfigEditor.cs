using UnityEditor;
using UnityEngine;

namespace Dajiagame.NonlinearEvent.Editor
{
    public class ConfigEditor : EditorWindow
    {

        public static ConfigEditor Instance;

        private NonlinearEventGroup _eventGroup;
        private Config _config;
        private Texture2D _newEffectIcon;

        void Awake()
        {
            Instance = this;
            _eventGroup = EventEditor.Instance.EventGroup;
            _config = _eventGroup.Config;
            position = new Rect(160, 160, 360, 640);
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("事件会产生的数据效果");
                EditorGUILayout.BeginHorizontal();
                {
                    DrawEffects();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("参与的角色");
                EditorGUILayout.BeginHorizontal();
                {
                    DrawCharacters();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                _config.DefaultSelectionCount = EditorGUILayout.IntField("默认选项数量", _config.DefaultSelectionCount);
            }
            EditorGUILayout.EndVertical();
            EditorUtility.SetDirty(EventEditor.Instance.EventGroup);
        }

        private void DrawEffects()
        {
            for (int i = 0; i < _config.Effects.Count; i++)
            {
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    //名称
                    _config.Effects[i].Name = EditorGUILayout.TextField(_config.Effects[i].Name, GUILayout.Width(48));
                    //图标
                    _config.Effects[i].Icon = (Texture2D)EditorGUILayout.ObjectField(_config.Effects[i].Icon, typeof(Texture2D), false, GUILayout.Width(48), GUILayout.Height(48));
                    //移除按钮
                    if (GUILayout.Button("-", GUILayout.Width(48), GUILayout.Height(16))) {
                        RemoveAEffect(i);
                    }
                }
            }
            if (GUILayout.Button("+", GUILayout.Width(32), GUILayout.Height(91)))
            {
                _config.Effects.Add(new Config.Effect());
            }
            
        }

        private void DrawCharacters()
        {
            for (int i = 0; i < _config.Characters.Count; i++) {
                using (new EditorGUILayout.VerticalScope("box")) {
                    //名称
                    _config.Characters[i].Name = EditorGUILayout.TextField(_config.Characters[i].Name, GUILayout.Width(64));
                    //图标
                    _config.Characters[i].Icon = (Texture2D)EditorGUILayout.ObjectField(_config.Characters[i].Icon, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64));
                    //移除按钮
                    if (GUILayout.Button("-", GUILayout.Width(64), GUILayout.Height(16))) {
                        _config.Characters.RemoveAt(i);                        
                    }
                }
            }
            if (GUILayout.Button("+", GUILayout.Width(32), GUILayout.Height(107))) {
                _config.Characters.Add(new Config.Character());
            }
            EventEditor.Instance.UpdatePopUpCharacterNames();
        }

        private void RemoveAEffect(int ID)
        {
            _config.Effects.RemoveAt(ID);
            //所有现有节点对应调整Require和Effect数据
            if (_eventGroup.ListNodes == null) return;
            foreach (var eventNode in _eventGroup.ListNodes)
            {
                if(eventNode.Requires.Count>ID)eventNode.Requires.RemoveAt(ID);
                foreach (var selection in eventNode.Selections)
                {
                    if (selection.Effects.Count > ID) selection.Effects.RemoveAt(ID);  
                }
            }
        }

        public static void CloseWindow()
        {
            if (Instance != null)
            {
                Instance.Close();
                Instance = null;
            }
        }
    }
}