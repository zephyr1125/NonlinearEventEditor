using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dajiagame.NonlinearEvent
{
    public class NonlinearEventGroup : ScriptableObject
    {
        public Config Config;

        public List<EventNode> ListNodes;

        public EventNode GetNode(int ID)
        {
            return ListNodes.Find(_ => _.ID == ID);
        }

        public int GetLastEventID()
        {
            return ListNodes.Max(_ => _.ID);
        }
    }
}