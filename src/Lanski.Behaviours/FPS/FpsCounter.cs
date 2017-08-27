using JetBrains.Annotations;
using Lanski.Reactive;
using UnityEngine;

namespace Lanski.Behaviours.FPS
{
    public class FpsCounter : MonoBehaviour
    {
        public float RefreshRate = 1f;
                                      
        public IStream<int> FPS => _fps;

        private readonly Stream<int> _fps = new Stream<int>();
        private float _elapsedTime;
        private int _elapsedFrames;

        [UsedImplicitly]
        public void Update()
        {
            _elapsedTime += Time.deltaTime;
            _elapsedFrames++;

            if (_elapsedTime > RefreshRate)
            {
                var fps = Mathf.RoundToInt((_elapsedFrames - 1) / RefreshRate);
                _fps.Next(fps);
                _elapsedFrames = 1;
                _elapsedTime -= RefreshRate;
            }
        }
    }
}