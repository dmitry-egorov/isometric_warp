using Lanski.Reactive;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lanski.Behaviours
{
    public class ClickSource : MonoBehaviour, IPointerClickHandler
    {
        private bool _initialized;
        private Stream<PointerEventData> _clicks;
        public IStream<PointerEventData> Clicks => _clicks;

        public void Awake()
        {
            Init();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Init();
            
            _clicks.Next(eventData);
        }

        private void Init()
        {
            if (_initialized)
                return;
            _initialized = true;

            _clicks = new Stream<PointerEventData>();
        }
    }
}