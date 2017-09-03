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
                .PixelPixelPerfectScaleCell
                .Subscribe(scale => Shader.SetGlobalFloat("_PixelPerfectScale", scale));

            reference
                .PixelPixelScaleCell
                .Subscribe(scale => Shader.SetGlobalFloat("_PixelScale", scale));
        }
    }
}