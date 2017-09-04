using Lanski.Reactive;
using WarpSpace.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models.Game.Battle.Board.Structure
{
    public class StructureModel
    {
        public readonly StructureDescription Description;
        public readonly Interactor Interactor;
        public readonly TileModel Location;

        public IStream<StructureDestroyed> Signal_Of_the_Destruction => _signal_of_the_destruction;
        
        internal StructureModel(StructureDescription description, TileModel location, InteractorFactory factory)
        {
            Description = description;
            Interactor = factory.Create(description, this);
            Location = location;
        }

        public void Destroy()
        {
            Location.Reset_Structure();
            
            _signal_of_the_destruction.Next(new StructureDestroyed(this));
        }
        
        private readonly Signal<StructureDestroyed> _signal_of_the_destruction = new Signal<StructureDestroyed>();
    }
}