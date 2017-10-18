using Lanski.Reactive;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;

namespace WarpSpace.Game.Battle.Unit
{
    public class WTeleporter
    {
        public WTeleporter(WSpacial the_spacial, WMover the_mover, WRotator the_rotator)
        {
            this.the_spacial = the_spacial;

            this.the_mover = the_mover;
            this.the_rotator = the_rotator;
            
            its_movements = new Stream<TheVoid>();
        }
        
        public void Teleports_To(Possible<Index2D> the_position, Direction2D the_orientation)
        {
            the_mover.Resets();
            the_rotator.Resets();
            the_spacial.s_Position = the_position;
            the_spacial.s_Local_Position = Vector3.zero;
            the_spacial.s_Orientation = the_orientation;
            the_spacial.s_Local_Rotation = the_orientation.s_Rotation();

            its_movements.Next();
        }
        
        private readonly WSpacial the_spacial;
        private readonly WMover the_mover;
        private readonly WRotator the_rotator;
        
        private readonly Stream<TheVoid> its_movements;
    }
}