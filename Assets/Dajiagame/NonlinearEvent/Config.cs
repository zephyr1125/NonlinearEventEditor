using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dajiagame.NonlinearEvent
{
    [Serializable]
    public class Config
    {
        /// <summary>
        /// 事件影响效果设置：数量、图标
        /// </summary>
        public List<Effect> Effects = new List<Effect>();

        /// <summary>
        /// 人物设置：数量、名称、图标
        /// </summary>
        public List<Character> Characters = new List<Character>();

        /// <summary>
        /// 选项默认数量
        /// </summary>
        public int DefaultSelectionCount;

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

    }
}