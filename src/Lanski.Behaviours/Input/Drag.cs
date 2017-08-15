using Lanski.Reactive;
using UnityEngine;

namespace Lanski.Behaviours.Input
{
    public class Drag: MonoBehaviour
    {
        private bool _initialized;
        private Vector2 _currentOrigin;
        private Vector2 _lastFrameOrigin;

        private Stream<DragEvent> _dragEvents;
        private ICell<bool> _dragIsInProgressCell;

        public IStream<DragEvent> DragEvents
        {
            get
            {
                Initialize();
                return _dragEvents;
            }
        }

        public ICell<bool> DragIsInProgress
        {
            get
            {
                Initialize();
                return _dragIsInProgressCell;
            }
        }

        void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_initialized)
                return;
            _initialized = true;
            
            _dragEvents = new Stream<DragEvent>();
            _dragIsInProgressCell = _dragEvents.IsInProgress(e => e.Type == DragEventType.Started, e => e.Type == DragEventType.Finished);
        }

        void Update()
        {
            Initialize();
            
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                _currentOrigin = UnityEngine.Input.mousePosition;
                _lastFrameOrigin = _currentOrigin;
                SendEvent(DragEventType.Started);
            }

            if (UnityEngine.Input.GetMouseButton(0))
            {
                SendEvent(DragEventType.Moved);
            }

            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                SendEvent(DragEventType.Finished);
            }
        }

        private void SendEvent(DragEventType type)
        {
            var destination = (Vector2) UnityEngine.Input.mousePosition;
            var delta = destination - _lastFrameOrigin;
            _dragEvents.Next(new DragEvent(type, _currentOrigin, destination, delta));

            _lastFrameOrigin = destination;
        }
    }

    public struct DragEvent
    {
        public DragEvent(DragEventType type, Vector2 origin, Vector2 destination, Vector2 currentFrameDelta)
        {
            Type = type;
            Origin = origin;
            Destination = destination;
            CurrentFrameDelta = currentFrameDelta;
        }

        public DragEventType Type { get; }
        public Vector2 Origin { get; }
        public Vector2 Destination { get; }
        public Vector2 CurrentFrameDelta { get; }

        public Vector2 TotalDelta => Destination - Origin;
    }

    public enum DragEventType
    {
        Started,
        Moved,
        Finished
    }
}