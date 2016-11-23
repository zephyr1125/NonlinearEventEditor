using System;
using UnityEngine;

namespace Dajiagame.NonlinearEvent
{
    [Serializable]
    public class EventNode
    {
        public int ID;

        /// <summary>
        /// 前置事件ID
        /// </summary>
        public int PrevID;

        /// <summary>
        /// 确认的后续事件ID
        /// </summary>
        public int ConfirmNextID;

        /// <summary>
        /// 驳回的后置事件ID
        /// </summary>
        public int RejectNextID;

        public string Text = "文本";
        public string PreviewText = "预览文本";

        public int[] Effects;

        /// <summary>
        /// 编辑器使用的，绘制节点的位置
        /// </summary>
        public Vector2 Position;

    }
}