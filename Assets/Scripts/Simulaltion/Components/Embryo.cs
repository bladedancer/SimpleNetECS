using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Components
{
    struct Embryo : ISharedComponentData
    {
        public NetData netdata;
    }
}