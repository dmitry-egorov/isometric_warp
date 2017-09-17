using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common;
using WarpSpace.Game.Battle.Board;

namespace WarpSpace.Game.Battle.Unit
{
    public class WSpacial
    {
        public WSpacial(Possible<Index2D> initial_position, Transform the_transform, Transform the_limbo, WBoard the_board)
        {
            its_position = initial_position;
            its_orientation = Direction2D.Left;
            its_transform = the_transform;
            this.the_limbo = the_limbo;
            this.the_board = the_board;

            its_transform.localRotation = Direction2D.Left.s_Rotation();
        }

        public Vector3 s_Local_Position
        {
            get => its_transform.localPosition;
            set => its_transform.localPosition = value;
        }

        public Quaternion s_Local_Rotation
        {
            get => its_transform.localRotation;
            set => its_transform.localRotation = value;
        }

        public Possible<Direction2D> s_Orientation
        {
            get => its_orientation;
            set => its_orientation = value;
        }

        public Possible<Index2D> s_Position
        {
            get => its_position;
            set
            {
                its_position = value;
                its_transform.parent = value.has_a_Value(out var the_position) 
                    ? the_board[the_position].s_Unit_Slot.s_Transform 
                    : the_limbo
                ;
            }
        }

        private readonly Transform its_transform;
        private readonly Transform the_limbo;
        private readonly WBoard the_board;
        private Possible<Index2D> its_position;
        private Possible<Direction2D> its_orientation;
    }
}