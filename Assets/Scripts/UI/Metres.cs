using System.Globalization;
using Runner;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Metres : MonoBehaviour
    {
        [SerializeField] private TargetObject target;
        [SerializeField] private TMP_Text text;
    

        private void Update()
        {
            text.text = Mathf.Floor(target.DistanceToPlayer).ToString(CultureInfo.InvariantCulture);
        }
    }
}
