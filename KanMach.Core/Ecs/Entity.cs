﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.Ecs
{
    public struct Entity : IEquatable<Entity>
    {

        public int Id;

        public int Gen;

        public EcsWorld World;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Entity other) => Id == other.Id && Gen == other.Gen;
    }
}
