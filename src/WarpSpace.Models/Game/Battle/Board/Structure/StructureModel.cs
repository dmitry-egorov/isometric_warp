using Lanski.Reactive;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Structure
{
    public class StructureModel
    {
        public readonly StructureDescription Description;
        public readonly TileModel Location;

        public IStream<StructureDestroyed> Signal_Of_the_Destruction => _signal_of_the_destruction;
        
        internal StructureModel(StructureDescription description, TileModel location)
        {
            Description = description;
            Location = location;
        }

        public void Destroy()
        {
            Location.Reset_Structure();
            _signal_of_the_destruction.Next(new StructureDestroyed(this));
        }
        
        public bool Is_an_Entrance() => Description.Is_An_Entrance();
        public bool Is_an_Exit() => Description.Is_An_Exit();
        public bool Is_a_Debris() => Description.Is_A_Debris();
        public bool Is_a_Debris(out StructureDescription.Debris debris) => Description.Is_A_Debris(out debris);

        private readonly Signal<StructureDestroyed> _signal_of_the_destruction = new Signal<StructureDestroyed>();
    }
}