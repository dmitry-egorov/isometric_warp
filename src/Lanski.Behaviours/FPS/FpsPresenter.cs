using System.Linq;
using Lanski.Reactive;
using UnityEngine;
using UnityEngine.UI;

namespace Lanski.Behaviours.FPS
{
    [RequireComponent(typeof(Text))]
    public class FpsPresenter : MonoBehaviour
    {
        private static string[] _prebuiltStrings;
        
        void Start()
        {
            Init();
            
            var text = GetComponent<Text>();
            FindObjectOfType<FpsCounter>()
                .FPS
                .DistinctSequential()
                .Subscribe(amount => text.text = GetText(amount));
        }

        private void Init()
        {
            if (_prebuiltStrings != null)
                return;

            _prebuiltStrings = Enumerable
                .Range(0, 120)
                .Select(x => x.ToString())
                .ToArray();
        }

        private static string GetText(int amount)
        {
            return amount < _prebuiltStrings.Length 
                ? _prebuiltStrings[amount] 
                : amount.ToString();
        }
    }
}