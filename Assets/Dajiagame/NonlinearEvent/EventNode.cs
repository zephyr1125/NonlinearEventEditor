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
        /// 解锁本事件的数据需求
        /// WARNING 注意由于有死亡事件是以某数值到0为条件的，因此这里只能以-1为默认值
        /// </summary>
        public List<int> Requires;

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

        public EventNode(int effectCount)
        {
            Requires = new List<int>(effectCount);
            while (Requires.Count < effectCount) {
                //require的默认值填-1
                Requires.Add(-1);
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