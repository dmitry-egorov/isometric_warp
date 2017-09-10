using System.Collections.Generic;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.SwiftLinq;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public class MBoard
    {
        public readonly MTile[,] Tiles;
        public IReadOnlyList<MUnit> Units => its_units_list;

        public IStream<MUnit> s_Stream_Of_Unit_Creations => its_unit_factory.s_Unit_Creations_Stream;
        public IStream<MUnit> s_Unit_Destructions_Stream => its_unit_destructions_stream;
        public IStream<TheVoid> s_Turn_Ends_Stream => its_turn_ends_stream;
        public IStream<TheVoid> s_Stream_Of_Exits { get; }

        public MBoard(BoardDescription description, SignalGuard the_signal_guard)
        {
            this.the_signal_guard = the_signal_guard;
            its_entrances_spacial = description.EntranceSpacial;
            its_unit_destructions_stream = new GuardedStream<MUnit>(the_signal_guard);
            its_turn_ends_stream = new GuardedStream<TheVoid>(the_signal_guard);
            its_unit_factory = new UnitFactory(the_signal_guard);

            s_Stream_Of_Exits = Create_Exits_Stream();
            wires_unit_creation();

            Tiles = creates_tiles();

            IStream<TheVoid> Create_Exits_Stream() =>
                s_Stream_Of_Unit_Creations
                    .Select(x => x.s_Exit_Signal)
                    .Merge()
            ;

            MTile[,] creates_tiles()
            {
                var tiles = description.Tiles.Map((tile_desc, position) => CreateTile(position, tile_desc));
                foreach (var i in tiles.EnumerateIndex())
                {
                    var adjacent = tiles.GetAdjacent(i);
                    tiles.Get(i).Init(adjacent);
                }
                return tiles;

                MTile CreateTile(Index2D position, TileDescription desc) =>
                    new MTile(position, desc, its_unit_factory, the_signal_guard)
                ;
            }

            void wires_unit_creation()
            {
                s_Stream_Of_Unit_Creations
                    .Subscribe(unit =>
                    {
                        wires_the_units_destruction(unit);
                        its_units_hashset.Add(unit);
                        its_units_list.Add(unit);
                    });

                void wires_the_units_destruction(MUnit unit)
                {
                    unit.s_Destruction_Signal
                        .Subscribe(destroyed =>
                        {
                            its_units_hashset.Remove(unit);
                            its_units_list.Remove(unit);
                            its_unit_destructions_stream.Next(unit);
                        });
                }
            }
        }

        public void Warps_In_the_Mothership()
        {
            var position = its_entrances_spacial.Position;
            var orientation = its_entrances_spacial.Orientation;

            var tank1 = new UnitDescription(UnitType.a_Tank, Faction.Player, Possible.Empty<Stuff>(), Possible.Empty<IReadOnlyList<Possible<UnitDescription>>>());
            var tank2 = new UnitDescription(UnitType.a_Tank, Faction.Player, Possible.Empty<Stuff>(), Possible.Empty<IReadOnlyList<Possible<UnitDescription>>>());
            var bay_units = new List<Possible<UnitDescription>> {tank1, tank2};
            var desc = new UnitDescription(UnitType.a_Mothership, Faction.Player, Possible.Empty<Stuff>(), bay_units);

            creates_a_unit(desc, position + orientation);
        }

        public void Ends_the_Turn()
        {
            using (the_signal_guard.Holds_All_Events())
            {
                var iterator = its_units_list.SIterate();
                while (iterator.has_a_Value(out var unit))
                {
                    unit.Finishes_the_Turn();
                }
                
                signals_the_turns_end();
            }
        }

        private void creates_a_unit(UnitDescription desc, Index2D position)
        {
            Tiles.Get(position).has_a_Location(out var location).Otherwise_Throw("Can't add a unit to a tile occupied by a structure");

            its_unit_factory.Creates_a_Unit(desc, location);
        }

        private void signals_the_turns_end()
        {
            its_turn_ends_stream.Next();
        }

        private readonly GuardedStream<TheVoid> its_turn_ends_stream;
        private readonly GuardedStream<MUnit> its_unit_destructions_stream;
        private readonly Spacial2D its_entrances_spacial;
        private readonly UnitFactory its_unit_factory;
        private readonly SignalGuard the_signal_guard;
        
        private readonly HashSet<MUnit> its_units_hashset = new HashSet<MUnit>();
        private readonly List<MUnit> its_units_list = new List<MUnit>();
    }
}