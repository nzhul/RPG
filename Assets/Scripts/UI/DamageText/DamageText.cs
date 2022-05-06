using TMPro;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI damageText = null;

        public void SetValue(float amount)
        {
            damageText.text = string.Format("{0:0}", amount);
        }
    }
}