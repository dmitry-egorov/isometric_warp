using Lanski.Maths;
using Lanski.Structures;
using UnityEngine;

namespace WarpSpace.World.Board.Landscape
{
    public static class Initiator
    {
        public static void InitElement(Tile tile, FullNeighbourhood2D<TileSpec> neighbourhood, Data specs)
        {
            var generateMesh = GenerateMesh();
            
            tile
                .LandscapeElement
                .Init(generateMesh);

            Mesh GenerateMesh()
            {
                var index = tile.Index;
                var n = GetTypeAndSpecs();
                var spec = n.Center.Spec;
                var mesh = SelectMesh();
                var elevations = CalculateElevations();
                return MeshTransformer.Transform(mesh, TileCreation.GetDirection(index), elevations, spec.Falloff);

                FullNeighbourhood2D<TypeAndSpec> GetTypeAndSpecs()
                {
                    return neighbourhood.Select(x => x.Type).Select(t => new TypeAndSpec(t, specs.GetSpecFor(t)));
                }

                Mesh SelectMesh()
                {
                    return spec.TerrainMeshes[index.Row % spec.TerrainMeshes.Length];
                }

                FullNeighbourhood2D<float> CalculateElevations()
                {
                    return FullNeighbourhood2D.Create(
                        center: 0.0f,
                        left: CalculateAdjacentOffset(n.Center, n.Left),
                        up: CalculateAdjacentOffset(n.Center, n.Up),
                        right: CalculateAdjacentOffset(n.Center, n.Right),
                        down: CalculateAdjacentOffset(n.Center, n.Down),
                        leftUp: CalculateCrossOffset(n.Left, n.Up, n.LeftUp, n.Center),
                        rightUp: CalculateCrossOffset(n.Right, n.Up, n.RightUp, n.Center),
                        rightDown: CalculateCrossOffset(n.Right, n.Down, n.RightDown, n.Center),
                        leftDown: CalculateCrossOffset(n.Left, n.Down, n.LeftDown, n.Center)
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

        private struct TypeAndSpec
        {
            public readonly LandscapeType Type;
            public readonly TileData Spec;

            public TypeAndSpec(LandscapeType type, TileData spec)
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