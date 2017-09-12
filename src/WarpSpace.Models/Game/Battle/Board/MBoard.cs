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
        public MTile[,] s_Tiles => its_tiles;
        public IReadOnlyList<MUnit> s_Units => its_units_list;

        public IStream<MUnit> s_Stream_Of_Unit_Creations => its_unit_factory.s_Unit_Creations_Stream;
        public IStream<MUnit> s_Unit_Destructions_Stream => its_unit_destructions_stream;
        public IStream<TheVoid> s_Turn_Ends_Stream => its_turn_ends_stream;

        public MBoard(DBoard description, SignalGuard the_signal_guard, MGame the_game)
        {
            this.the_signal_guard = the_signal_guard;
            its_entrances_spacial = description.EntranceSpacial;
            its_unit_destructions_stream = new GuardedStream<MUnit>(the_signal_guard);
            its_turn_ends_stream = new GuardedStream<TheVoid>(the_signal_guard);
            its_unit_factory = new MUnitFactory(this, the_game, the_signal_guard);

            its_tiles = creates_the_tiles();

            MTile[,] creates_the_tiles()
            {
                var tiles = description.Tiles.Map((tile_desc, position) => CreateTile(position, tile_desc));
                foreach (var i in tiles.EnumerateIndex())
                {
                    var adjacent = tiles.GetAdjacent(i);
                    var neighbours = tiles.GetFitNeighbours(i);
                    tiles.Get(i).Init(neighbours);
                }
                return tiles;

                MTile CreateTile(Index2D position, DTile desc) =>
                    new MTile(position, desc, its_unit_factory, the_signal_guard)
                ;
            }
        }

        public void Warps_In_the_Mothership()
        {
            var position = its_entrances_spacial.Position;
            var orientation = its_entrances_spacial.Orientation;

            var tank1 = new DUnit(UnitType.a_Tank, Faction.the_Player_Faction, Possible.Empty<DStuff>(), Possible.Empty<IReadOnlyList<Possible<DUnit>>>());
            var tank2 = new DUnit(UnitType.a_Tank, Faction.the_Player_Faction, Possible.Empty<DStuff>(), Possible.Empty<IReadOnlyList<Possible<DUnit>>>());
            var bay_units = new List<Possible<DUnit>> {tank1, tank2};
            var desc = new DUnit(UnitType.a_Mothership, Faction.the_Player_Faction, Possible.Empty<DStuff>(), bay_units);

            creates_a_unit(desc, position + orientation);
        }

        public void Ends_the_Turn()
        {
            var iterator = its_units_list.s_new_iterator();
            while (iterator.has_a_Value(out var unit))
            {
                unit.Finishes_the_Turn();
            }

            signals_the_turns_end();
        }

        internal void Adds_a_Unit(MUnit the_unit)
        {
            wires_the_units_destruction();
            its_units_hashset.Add(the_unit);
            its_units_list.Add(the_unit);

            void wires_the_units_destruction()
            {
                the_unit.s_Destruction_Signal
                    .Subscribe(destroyed =>
                    {
                        its_units_hashset.Remove(the_unit);
                        its_units_list.Remove(the_unit);
                        its_unit_destructions_stream.Next(the_unit);
                    });
            }
        }

        private void creates_a_unit(DUnit desc, Index2D position)
        {
            its_tiles.Get(position).has_a_Location(out var location).Otherwise_Throw("Can't add a unit to a tile occupied by a structure");

            its_unit_factory.Creates_a_Unit(desc, location);
        }

        private void signals_the_turns_end()
        {
            its_turn_ends_stream.Next();
        }

        private readonly GuardedStream<TheVoid> its_turn_ends_stream;
        private readonly GuardedStream<MUnit> its_unit_destructions_stream;
        private readonly Spacial2D its_entrances_spacial;
        private readonly MUnitFactory its_unit_factory;
        private readonly SignalGuard the_signal_guard;
        
        private readonly MTile[,] its_tiles;
        private readonly HashSet<MUnit> its_units_hashset = new HashSet<MUnit>();
        private readonly List<MUnit> its_units_list = new List<MUnit>();
    }
}