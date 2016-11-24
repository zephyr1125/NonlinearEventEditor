using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dajiagame.NonlinearEvent
{
    [Serializable]
    public class EventNode
    {
        public int ID;

        /// <summary>
        /// 各个后续事件ID
        /// </summary>
        public List<int> NextEventIDs = new List<int>();

        public string Text = "文本";
        public string PreviewText = "简介文本";

        /// <summary>
        /// 对数据的影响
        /// </summary>
        public List<int> Effects = new List<int>();

        /// <summary>
        /// 编辑器使用的，绘制节点的位置
        /// </summary>
        public Vector2 Position;

        public void AddNextEventNode(int connectionID, int nextNodeID)
        {
            if (NextEventIDs == null)
            {
                NextEventIDs = new List<int>();
            }
            while (NextEventIDs.Count <= connectionID)
            {
                NextEventIDs.Add(0);
            }
            NextEventIDs[connectionID] = nextNodeID;
        }

    }
}