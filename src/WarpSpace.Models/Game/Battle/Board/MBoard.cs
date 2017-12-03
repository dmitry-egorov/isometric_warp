using System.Collections.Generic;
using Lanski.Reactive;
using Lanski.Structures;
using Lanski.SwiftLinq;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Structure;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Models.Game.Battle.Board.Unit;

namespace WarpSpace.Models.Game.Battle.Board
{
    public class MBoard
    {
        public MTile[,] s_Tiles => its_tiles;

        public IStream<MUnit> Created_a_Unit => its_unit_factory.Created_a_Unit;

        public MBoard(DBoard the_description, DUnit the_mothership_desc, SignalGuard the_signal_guard, MGame the_game)
        {
            this.the_mothership_desc = the_mothership_desc;
            its_entrances_spacial = the_description.EntranceSpacial;
            its_turn_ends_stream = new GuardedStream<TheVoid>(the_signal_guard);
            its_unit_factory = new MUnitFactory(this, the_game, the_signal_guard);

            its_tiles = creates_the_tiles();

            MTile[,] creates_the_tiles()
            {
                var tiles = the_description.Tiles.Map((tile_desc, position) => it_creates_a_tile(position, tile_desc));
                foreach (var i in tiles.EnumerateIndex())
                {
                    var neighbours = tiles.GetFitNeighbours(i);
                    tiles.Get(i).Init(neighbours);
                }
                return tiles;

                MTile it_creates_a_tile(Index2D position, DTile desc)
                {
                    var the_tile = new MTile(position, desc, the_signal_guard);
                    the_tile.s_Occupant_Becomes(it_creates_the_occupant(desc.s_Initial_Occupant, the_tile));
                    return the_tile;
                }

                MTileOccupant it_creates_the_occupant(DTileOccupant site, MTile the_tile)
                {
                    if (site.is_a_Structure(out var structure_description))
                        return new MStructure(structure_description, the_tile);

                    if (site.is_Empty())
                        return MTileOccupant.Empty;

                    var unit_desc = site.must_be_a_Unit();
                    return its_unit_factory.Creates_a_Unit(unit_desc, the_tile);
                }
            }
        }

        public void Warps_In_the_Mothership()
        {
            var position = its_entrances_spacial.Position;
            var orientation = its_entrances_spacial.Orientation;

            creates_a_unit(the_mothership_desc, position + orientation);
        }

        public void Ends_the_Turn()
        {
            var iterator = its_units_list.s_New_Iterator();
            while (iterator.has_a_Value(out var unit))
            {
                unit.Handles_a_Turn_Ending();
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
                the_unit.Been_Destroyed()
                    .Subscribe(destructed =>
                    {
                        its_units_hashset.Remove(the_unit);
                        its_units_list.Remove(the_unit);
                    });
            }
        }

        private void creates_a_unit(DUnit desc, Index2D position)
        {
            var the_tile = its_tiles.Get(position);
            the_tile.is_Empty().Otherwise_Throw("Can't add a unit to an occupied tile");

            its_unit_factory.Creates_a_Unit(desc, the_tile);
        }

        private void signals_the_turns_end()
        {
            its_turn_ends_stream.Next();
        }

        private readonly GuardedStream<TheVoid> its_turn_ends_stream;
        private readonly Spacial2D its_entrances_spacial;
        private readonly MUnitFactory its_unit_factory;
        private readonly DUnit the_mothership_desc;
        
        private readonly MTile[,] its_tiles;
        private readonly HashSet<MUnit> its_units_hashset = new HashSet<MUnit>();
        private readonly List<MUnit> its_units_list = new List<MUnit>();
    }
}