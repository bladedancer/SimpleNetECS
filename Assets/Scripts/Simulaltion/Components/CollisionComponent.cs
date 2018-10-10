﻿using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Components
{
    struct CollisionComponent : IComponentData
    {
        public Entity source;
        public Entity target;
    }
}