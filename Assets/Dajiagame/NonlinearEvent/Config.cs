using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dajiagame.NonlinearEvent
{
    public class Config : ScriptableObject
    {
        /// <summary>
        /// 事件影响效果设置：数量、图标
        /// </summary>
        public List<Effect> Effects = new List<Effect>();

        /// <summary>
        /// 人物设置：数量、名称、图标
        /// </summary>
        public List<Character> Characters;

        /// <summary>
        /// 选项分支设置：数量、颜色、名称
        /// </summary>
        public List<Selection> Selections;

        [Serializable]
        public class Effect
        {
            public string Name;
            public Texture2D Icon;
        }

        [Serializable]
        public class Character
        {
            public string Name;
            public Texture2D Icon;
        }

        [Serializable]
        public class Selection
        {
            public string Name;
            public Color Color;
        }
    }
}