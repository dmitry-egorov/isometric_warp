using UnityEngine;

namespace WarpSpace.Common
{
    [ExecuteInEditMode]
    public class ShaderGlobalsWirer : MonoBehaviour
    {
        private static bool _initialized;

        void Start()
        {
            Init();
        }

        void Update()
        {
            Init();
        }

        private static void Init()
        {
            if (_initialized)
                return;
            _initialized = true;

            var reference = FindObjectOfType<ReferencePixels>();
            
            reference
                .PixelPerfectScaleCell
                .Subscribe(scale => Shader.SetGlobalFloat("_PixelPerfectScale", scale));

            reference
                .PixelScaleCell
                .Subscribe(scale => Shader.SetGlobalFloat("_PixelScale", scale));
        }
    }
}