using UnityEditor;
using UnityEngine;

namespace Dajiagame.NonlinearEvent.Editor
{
    public class ConfigEditor : EditorWindow
    {
        private Config _config;
        private Texture2D _newEffectIcon;

        void Awake()
        {
            _config = EventEditor.Instance.EventGroup.Config;
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
                EditorGUILayout.LabelField("选项分支的名称与颜色");
                EditorGUILayout.BeginHorizontal();
                {
                    DrawSelections();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
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
                        _config.Effects.RemoveAt(i);
                    }
                }
            }
            if (GUILayout.Button("+", GUILayout.Width(32), GUILayout.Height(91)))
            {
                _config.Effects.Add(new Config.Effect());
            }
            EditorUtility.SetDirty(EventEditor.Instance.EventGroup);
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
            EditorUtility.SetDirty(EventEditor.Instance.EventGroup);
        }

        private void DrawSelections()
        {
            for (int i = 0; i < _config.Transitions.Count; i++) {
                using (new EditorGUILayout.VerticalScope("box")) {
                    //名称
                    _config.Transitions[i].Name = EditorGUILayout.TextField(_config.Transitions[i].Name, GUILayout.Width(64));
                    //颜色
                    _config.Transitions[i].Color = EditorGUILayout.ColorField(_config.Transitions[i].Color, GUILayout.Width(64), GUILayout.Height(16));
                    //移除按钮
                    if (GUILayout.Button("-", GUILayout.Width(64), GUILayout.Height(16))) {
                        _config.Transitions.RemoveAt(i);
                    }
                }
            }
            if (GUILayout.Button("+", GUILayout.Width(32), GUILayout.Height(60))) {
                _config.Transitions.Add(new Config.Transition());
            }
            EditorUtility.SetDirty(EventEditor.Instance.EventGroup);
        }
    }
}