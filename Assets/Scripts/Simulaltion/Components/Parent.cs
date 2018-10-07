using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Components
{
    struct Parent : IComponentData
    {
        public float CoolDown;
    }
}