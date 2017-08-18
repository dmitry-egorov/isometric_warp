using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.World.Board
{
    public class Tile : MonoBehaviour
    {
        public Index2D Index { get; private set; }
        public TileSpec Spec { get; private set; }
    
        public Landscape.Element LandscapeElement { get; private set; }
        public Water.Element WaterElement { get; private set; }

        public void Init(Index2D index, TileSpec spec)
        {
            LandscapeElement = GetComponentInChildren<Landscape.Element>();
            WaterElement = GetComponentInChildren<Water.Element>();
        
            Index = index;
            Spec = spec;
        }
    }
    
}