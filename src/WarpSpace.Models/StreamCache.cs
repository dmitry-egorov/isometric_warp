using Lanski.Reactive;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models
{
    public static class StreamCache
    {
        public static readonly IStream<UnitMoved> Empty_Stream_of_Movements = Stream.Empty<UnitMoved>();
        public static readonly IStream<MUnit> Empty_Stream_of_Unit_Destruction = Stream.Empty<MUnit>();
        public static readonly IStream<StructureDestroyed> Empty_Stream_of_Structure_Destruction = Stream.Empty<StructureDestroyed>();
    }
}