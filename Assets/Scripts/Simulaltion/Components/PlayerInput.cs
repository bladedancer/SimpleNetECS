using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Components
{
    public struct PlayerInput : IComponentData
    {
        public float Vertical;
        public float Horizontal;
    }
}
