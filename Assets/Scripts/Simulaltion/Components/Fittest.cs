using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Components
{
    public struct Fittest : ISharedComponentData
    {
        public NetData net;
        public float fitness;
        public int tag;
    }
}
