using UnityEditor.Expose;
using UnityEngine;

namespace WarpSpace.Common.Outline
{
    public class OutlinesInitializer : MonoBehaviour
    {
        [ExposeMethodInEditor]
        void Init()
        {
            var builders = GetComponentsInChildren<OutlineMeshBuilder>();

            foreach (var builder in builders)
            {
                builder.Build();
            }
        }
    }
}