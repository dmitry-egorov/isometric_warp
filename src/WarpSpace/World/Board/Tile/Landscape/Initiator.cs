using System;
using Lanski.Maths;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.World.Board.Tile.Descriptions;

namespace WarpSpace.World.Board.Tile.Landscape
{
    public class Initiator
    {
        private readonly TileSettings _mountain;
        private readonly TileSettings _hill;
        private readonly TileSettings _flatland;
        private readonly TileSettings _water;

        public Initiator(TileSettings mountain, TileSettings hill, TileSettings flatland, TileSettings water)
        {
            _mountain = mountain;
            _hill = hill;
            _flatland = flatland;
            _water = water;
        }

        public ComponentSpec Init(Index2D index, FullNeighbourhood2D<LandscapeType> neighbourhood)
        {
            var generatedMesh = GenerateMesh();
            
            return new ComponentSpec(generatedMesh, neighbourhood.Center);
            
            Mesh GenerateMesh()
            {
                var n = GetTypeAndSpecs();
                var spec = n.Center.Spec;
                var mesh = SelectMesh();
                var elevations = CalculateElevations();
                var orientation = TileCreationHelper.GetOrientation(index);
                
                return MeshTransformer.Transform(mesh, orientation, elevations, spec.Falloff);

                FullNeighbourhood2D<TypeAndSpec> GetTypeAndSpecs() => neighbourhood.Map(t => new TypeAndSpec(t, GetDataFor(t)));
                Mesh SelectMesh() => TileCreationHelper.SelectMesh(index, spec.Meshes);

                FullNeighbourhood2D<float> CalculateElevations()
                {
                    return FullNeighbourhood2D.Create(
                        center    : 0.0f,
                        
                        left      : CalculateAdjacentOffset(n.Center, n.Left),
                        up        : CalculateAdjacentOffset(n.Center, n.Up),
                        right     : CalculateAdjacentOffset(n.Center, n.Right),
                        down      : CalculateAdjacentOffset(n.Center, n.Down),
                        
                        leftUp    : CalculateCrossOffset(n.Left, n.Up, n.LeftUp, n.Center),
                        rightUp   : CalculateCrossOffset(n.Right, n.Up, n.RightUp, n.Center),
                        rightDown : CalculateCrossOffset(n.Right, n.Down, n.RightDown, n.Center),
                        leftDown  : CalculateCrossOffset(n.Left, n.Down, n.LeftDown, n.Center)
                    );

                    float CalculateAdjacentOffset(TypeAndSpec t1, TypeAndSpec t2)
                    {
                        if (t1 == t2)
                            return t1.Spec.SameTypeHeight;

                        var s1 = t1.Spec.OwnHeight;
                        var s2 = t2.Spec.OwnHeight;

                        return Mathf.Min(s1, s2);
                    }

                    float CalculateCrossOffset(TypeAndSpec ld, TypeAndSpec ru, TypeAndSpec lu, TypeAndSpec rd)
                    {
                        if (ld == ru && ru == lu && lu == rd)
                            return ld.Spec.SameTypeHeightCross;

                        var ldh = ld.Spec.OwnHeight;
                        var ruh = ru.Spec.OwnHeight;
                        var luh = lu.Spec.OwnHeight;
                        var rdh = rd.Spec.OwnHeight;

                        return Mathe.Avg(Mathf.Min(ldh, ruh), Mathf.Min(luh, rdh));
                    }
                }
            }
            
            
        }
        
        public TileSettings GetDataFor(LandscapeType type)
        {
            switch (type)
            {
                case LandscapeType.Mountain:
                    return _mountain;
                case LandscapeType.Hill:
                    return _hill;
                case LandscapeType.Flatland:
                    return _flatland;
                case LandscapeType.Water:
                    return _water;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private struct TypeAndSpec
        {
            public readonly LandscapeType Type;
            public readonly TileSettings Spec;

            public TypeAndSpec(LandscapeType type, TileSettings spec)
            {
                Type = type;
                Spec = spec;
            }

            public static bool operator ==(TypeAndSpec t1, TypeAndSpec t2)
            {
                return t1.Type == t2.Type;
            }

            public static bool operator !=(TypeAndSpec t1, TypeAndSpec t2)
            {
                return !(t1 == t2);
            }
        }
    }
}