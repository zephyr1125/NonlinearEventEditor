using UnityEngine;

namespace Dajiagame.NonlinearEvent
{
    public class Config : ScriptableObject
    {
        /// <summary>
        /// 事件影响效果设置：数量、图标
        /// </summary>
        public Texture2D[] EventIcons;

        /// <summary>
        /// 人物设置：数量、名称、图标
        /// </summary>
        public Char[] Characters;

        /// <summary>
        /// 选项分支设置：数量、颜色、名称
        /// </summary>
        public Selection[] Selections;

        public class Char
        {
            public string Name;
            public Texture2D Icon;
        }

        public class Selection
        {
            public string NameInMenu;
            public Color Color;
        }
    }
}