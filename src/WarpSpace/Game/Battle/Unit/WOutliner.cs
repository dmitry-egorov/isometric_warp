﻿using Lanski.Structures;
using Lanski.UnityExtensions;
using UnityEngine;
using WarpSpace.Overlay.Units;

namespace WarpSpace.Game.Battle.Unit
{
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(MeshFilter))]
    public class WOutliner : MonoBehaviour
    {
        public float Offset;

        public void Start() => it_inits();
        public void Update() => it_updates();

        public void Shows() => gameObject.Shows();
        public void Hides() => gameObject.Hides();

        private void it_inits()
        {
            its_transform = GetComponent<Transform>();
            its_parents_transform = transform.parent;
            its_parents_mesh_filter = its_parents_transform.gameObject.GetComponent<MeshFilter>();
            its_mesh_filter = GetComponent<MeshFilter>();
            
            it_builds_the_mesh();

            it_is_initialized = true;
        }

        private void it_builds_the_mesh()
        {
            var the_mesh = its_parents_mesh_filter.sharedMesh;
            if (the_mesh == its_parents_last_mesh)
                return;
            
            its_mesh_filter.sharedMesh = MeshSimplifier.Removes_Duplicate_Vertices_From(the_mesh);
            its_parents_last_mesh = the_mesh;
        }

        private void it_updates()
        {
            it_is_initialized.Must_Be_True_Otherwise_Throw("The OOutliner must be initialized before the first update");
            
            it_builds_the_mesh();
            
            its_transform.rotation = its_parents_transform.rotation;
            its_transform.position = its_parents_transform.position + Quaternion.Euler(-45, 0, 0) * Vector3.up * Offset;
        }

        private bool it_is_initialized;
        private Transform its_transform;
        private Transform its_parents_transform;
        private MeshFilter its_parents_mesh_filter;
        private MeshFilter its_mesh_filter;
        
        private Mesh its_parents_last_mesh;

    }
}