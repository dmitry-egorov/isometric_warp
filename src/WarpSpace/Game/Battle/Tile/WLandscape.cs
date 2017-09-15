using Lanski.Maths;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common.MeshTransformation;
using WarpSpace.Game.Battle.Board;
using WarpSpace.Models.Descriptions;
using WarpSpace.Models.Game.Battle.Board.Tile;
using WarpSpace.Settings;

namespace WarpSpace.Game.Battle.Tile
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class WLandscape: MonoBehaviour
    {
        public void Start()
        {
            its_mesh_filter = GetComponent<MeshFilter>();
            
            var the_tile = GetComponentInParent<WTile>().s_Tile_Model;
            
            its_mesh_filter.sharedMesh = GenerateMesh(the_tile.s_Position, the_tile.s_Neighbors.Map(x => x.s_Landscape_Type));
        }

        private Mesh GenerateMesh(Index2D index, FullNeighbourhood2D<MLandscapeType> neighbourhood)
        {
            var n = GetTypeAndSpecs();
            var spec = n.Center.Spec;
            var mesh = spec.Meshes.SelectBy(index);
            var elevations = CalculateElevations();
            var orientation = TileHelper.GetOrientation(index);
            
            return MeshTransformer.Transform(mesh, orientation, elevations, spec.Falloff);

            FullNeighbourhood2D<TypeAndSettings> GetTypeAndSpecs()
            {
                return neighbourhood.Map(t => new TypeAndSettings(t, GetDataFor(t)));
                
                LandscapeTypeSettings.WorldSettings GetDataFor(MLandscapeType type) => LandscapeTypeSettings.Of(type).World;
            }

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

                float CalculateAdjacentOffset(TypeAndSettings t1, TypeAndSettings t2)
                {
                    if (t1 == t2)
                        return t1.Spec.SameTypeHeight;

                    var s1 = t1.Spec.OwnHeight;
                    var s2 = t2.Spec.OwnHeight;

                    return Mathf.Min(s1, s2);
                }

                float CalculateCrossOffset(TypeAndSettings ld, TypeAndSettings ru, TypeAndSettings lu, TypeAndSettings rd)
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

        private MeshFilter its_mesh_filter;

        private struct TypeAndSettings
        {
            public readonly MLandscapeType Type;
            public readonly LandscapeTypeSettings.WorldSettings Spec;

            public TypeAndSettings(MLandscapeType type, LandscapeTypeSettings.WorldSettings spec)
            {
                Type = type;
                Spec = spec;
            }

            public static bool operator ==(TypeAndSettings t1, TypeAndSettings t2) => t1.Type == t2.Type;
            public static bool operator !=(TypeAndSettings t1, TypeAndSettings t2) => !(t1 == t2);

            public bool Equals(TypeAndSettings other) => Type == other.Type && Spec.Equals(other.Spec);

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is TypeAndSettings && Equals((TypeAndSettings) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Type.GetHashCode() * 397) ^ Spec.GetHashCode();
                }
            }
        }
    }
}