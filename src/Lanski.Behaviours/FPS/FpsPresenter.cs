using Lanski.Reactive;
using UnityEngine;
using UnityEngine.UI;

namespace Lanski.Behaviours.FPS
{
    [RequireComponent(typeof(Text))]
    public class FpsPresenter : MonoBehaviour
    {
        void Start()
        {
            var cache = FindObjectOfType<IntStringsCache>();
            
            var text = GetComponent<Text>();
            FindObjectOfType<FpsCounter>()
                .FPS
                .DistinctSequential()
                .Subscribe(amount => text.text = cache.GetText(amount));
        }
    }
}