using System.Collections.Generic;
using UnityEngine;

namespace Dajiagame.NonlinearEvent
{
    public class NonlinearEventGroup : ScriptableObject
    {
        public Config Config;

        public List<EventNode> ListNodes;
    }
}