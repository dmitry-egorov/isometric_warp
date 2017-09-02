using System;
using Lanski.Structures;

namespace WarpSpace.Common
{
    [Serializable]
    public struct Spacial2DData
    {
        public Index2DData Position;
        public Direction2D Orientation;

        public override string ToString()
        {
            return $"{Position}, {Orientation}";
        }
    }
}