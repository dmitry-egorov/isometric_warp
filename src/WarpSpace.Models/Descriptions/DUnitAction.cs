using System;
using JetBrains.Annotations;
using Lanski.Structures;

namespace WarpSpace.Models.Descriptions
{
    public struct DUnitAction: IEquatable<DUnitAction>
    {
        public static class Create
        {
            public static DUnitAction Fire() => new DUnitAction(new Fire());
            public static DUnitAction Move() => new DUnitAction(new Move());
            public static DUnitAction Interact() => new DUnitAction(new Interact());
        }
        
        public DUnitAction(Or<Fire, Move, Interact> the_variant)
        {
            its_variant = the_variant;
        }
        
        [Pure] public bool is_a_Fire_Action() => its_variant.is_a_T1();
        [Pure] public bool is_a_Move_Action() => its_variant.is_a_T2();
        [Pure] public bool is_an_Interact_Action() => its_variant.is_a_T3();
        
        public struct Fire {}
        public struct Move {}
        public struct Interact {}
        
        private readonly Or<Fire, Move, Interact> its_variant;

        public bool Equals(DUnitAction other) => its_variant.Equals(other.its_variant);
    }
}