using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Components
{
    struct Embryo : IComponentData
    {
        public int Generation;
    }
}