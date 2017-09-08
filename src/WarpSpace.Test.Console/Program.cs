using System;
using System.Collections.Generic;
using Lanski.Structures;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game;

namespace WarpSpace.Test.Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine("Start");
            var the_tile_site_description = new TileSiteDescription(TheVoid.Instance);
            var tile = new TileDescription(LandscapeType.Flatland, the_tile_site_description);
            var tiles = new TileDescription[8, 8];//.Map(_ => new TileDescription(LandscapeType.Flatland, new TileSiteDescription(TheVoid.Instance)));
            
            var desc = new BoardDescription(tiles, new Spacial2D(new Index2D(2, 2), Direction2D.Down));
            var game = new MGame(desc);
            
            game.Starts();
        }
    }
}