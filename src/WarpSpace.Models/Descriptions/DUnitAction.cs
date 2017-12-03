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
            public static DUnitAction Deploy(int slot_index) => new DUnitAction(new Deploy {s_bay_slot_index = slot_index});
            public static DUnitAction Dock() => new DUnitAction(new Dock());
            public static DUnitAction Move() => new DUnitAction(new Move());
            public static DUnitAction Interact() => new DUnitAction(new Interact());
        }
        
        public DUnitAction(Or<Fire, Deploy, Dock, Move, Interact> the_variant)
        {
            its_variant = the_variant;
        }
        
        [Pure] public bool is_a_Fire_Action() => its_variant.is_a_T1();
        [Pure] public bool is_a_Deploy_Action(out Deploy the_deploy_action) => its_variant.is_a_T2(out the_deploy_action);
        [Pure] public bool is_a_Dock_Action() => its_variant.is_a_T3();
        [Pure] public bool is_a_Move_Action() => its_variant.is_a_T4();
        [Pure] public bool is_an_Interact_Action() => its_variant.is_a_T5();
        
        public struct Fire {}
        public struct Deploy { public int s_bay_slot_index; }
        public struct Dock {}
        public struct Move {}
        public struct Interact {}
        
        private readonly Or<Fire, Deploy, Dock, Move, Interact> its_variant;

        public bool Equals(DUnitAction other) => its_variant.Equals(other.its_variant);
    }
}