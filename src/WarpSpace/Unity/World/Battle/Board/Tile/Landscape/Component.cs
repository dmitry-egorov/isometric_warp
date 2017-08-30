using System;
using Lanski.Maths;
using Lanski.Structures;
using UnityEngine;
using WarpSpace.Common.MeshTransformation;
using WarpSpace.Descriptions;

namespace WarpSpace.Unity.World.Battle.Board.Tile.Landscape
{
    public class Component: MonoBehaviour
    {
        public OwnSettings Settings;
        
        public MeshFilter LandscapeMeshFilter;
        public MeshRenderer Renderer;
        public Material Normal;
        public Material MoveHighlight;
        public Material UnitHighlight;
        public Material InteractionHighlight;
        public Material UseWeaponHighlight;

        public void Init(Index2D position, FullNeighbourhood2D<LandscapeType> neighbourhood)
        {
            LandscapeMeshFilter.sharedMesh = GenerateMesh(position, neighbourhood);
        }

        public bool set_highlight(HighlightType type)
        {
            Renderer.sharedMaterial = SelectMaterial();
            return true;

            Material SelectMaterial()
            {
                switch (type)
                {
                    case HighlightType.None:
                        return Normal;
                    case HighlightType.Move:
                        return MoveHighlight;
                    case HighlightType.Unit:
                        return UnitHighlight;
                    case HighlightType.Interaction:
                        return InteractionHighlight;
                    case HighlightType.UseWeapon:
                        return UseWeaponHighlight;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        private Mesh GenerateMesh(Index2D index, FullNeighbourhood2D<LandscapeType> neighbourhood)
        {
            var n = GetTypeAndSpecs();
            var spec = n.Center.Spec;
            var mesh = SelectMesh();
            var elevations = CalculateElevations();
            var orientation = TileCreationHelper.GetOrientation(index);
            
            return MeshTransformer.Transform(mesh, orientation, elevations, spec.Falloff);

            FullNeighbourhood2D<TypeAndSettings> GetTypeAndSpecs() => neighbourhood.Map(t => new TypeAndSettings(t, GetDataFor(t)));
            Mesh SelectMesh() => spec.Meshes.SelectBy(index);

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

            OwnSettings.Type GetDataFor(LandscapeType type)
            {
                switch (type)
                {
                    case LandscapeType.Mountain:
                        return Settings.Mountain;
                    case LandscapeType.Hill:
                        return Settings.Hill;
                    case LandscapeType.Flatland:
                        return Settings.Flatland;
                    case LandscapeType.Water:
                        return Settings.Water;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
            
        }

        private struct TypeAndSettings
        {
            public readonly LandscapeType Type;
            public readonly OwnSettings.Type Spec;

            public TypeAndSettings(LandscapeType type, OwnSettings.Type spec)
            {
                Type = type;
                Spec = spec;
            }

            public static bool operator ==(TypeAndSettings t1, TypeAndSettings t2)
            {
                return t1.Type == t2.Type;
            }

            public static bool operator !=(TypeAndSettings t1, TypeAndSettings t2)
            {
                return !(t1 == t2);
            }
        }

        [Serializable]
        public struct OwnSettings
        {
            public Type Mountain;
            public Type Hill;
            public Type Flatland;
            public Type Water;
            
            [Serializable]
            public struct Type
            {
                public float OwnHeight;
                public float SameTypeHeight;
                public float SameTypeHeightCross;
                public float Falloff;
                public Mesh[] Meshes;
            }
        }
    }
}