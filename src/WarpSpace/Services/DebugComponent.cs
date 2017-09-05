using UnityEngine;

namespace WarpSpace.Services
{
    public class DebugComponent: MonoBehaviour
    {
        void Awake()
        {
            Models.Debug.TheLog.Subscribe(Debug.Log);
        }
    }
}