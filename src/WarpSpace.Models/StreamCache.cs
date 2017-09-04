using Lanski.Reactive;
using WarpSpace.Models.Game.Battle.Board;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;

namespace WarpSpace.Models
{
    public static class StreamCache
    {
        public static readonly IStream<UnitMoved> Empty_Stream_of_Movements = Stream.Empty<UnitMoved>();
        public static readonly IStream<UnitDestroyed> Empty_Stream_of_Unit_Destruction = Stream.Empty<UnitDestroyed>();
        public static readonly IStream<StructureDestroyed> Empty_Stream_of_Structure_Destruction = Stream.Empty<StructureDestroyed>();
    }
}