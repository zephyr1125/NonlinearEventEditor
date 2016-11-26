using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dajiagame.NonlinearEvent
{
    [Serializable]
    public class EventNode
    {
        public int ID;

        public int CharacterID;

        public string Text = "文本";

        /// <summary>
        /// 选项
        /// </summary>
        public List<Selection> Selections = new List<Selection>();

        /// <summary>
        /// 选项子类
        /// </summary>
        [Serializable]
        public class Selection
        {
            /// <summary>
            /// 选项文本
            /// </summary>
            public string Text;
            /// <summary>
            /// 对数据的影响
            /// </summary>
            public List<int> Effects;
            /// <summary>
            /// 后续事件ID
            /// </summary>
            public int NextEventID;

            public Selection(int effectCount)
            {
                Effects = new List<int>(effectCount);
                while (Effects.Count<effectCount)
                {
                    Effects.Add(0);
                }
            }
        }

        /// <summary>
        /// 编辑器使用的，绘制节点的位置
        /// </summary>
        public Vector2 Position;

        public void AddNextEventNode(int transitionType, int nextEventID)
        {
            Selections[transitionType].NextEventID = nextEventID;
        }

        public void RemoveNextEventNode(int transitionType)
        {
            Selections[transitionType].NextEventID = 0;
        }
    }
}