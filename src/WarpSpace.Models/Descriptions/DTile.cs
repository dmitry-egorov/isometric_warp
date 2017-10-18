namespace WarpSpace.Models.Descriptions
{
    public struct DTile
    {
        public readonly MLandscapeType s_Type;
        public readonly DTileOccupant s_Initial_Occupant;

        public DTile(MLandscapeType type, DTileOccupant initial_occupant)
        {
            s_Type = type;
            s_Initial_Occupant = initial_occupant;
        }
    }
}