using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class TraitUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI unassignedPointsText;
        [SerializeField] Button commitButton;

        TraitStore _traitStore = null;

        private void Start()
        {
            _traitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
            commitButton.onClick.AddListener(_traitStore.Commit);
        }

        private void Update()
        {
            unassignedPointsText.text = _traitStore.GetUnassignedPoints().ToString();
        }
    }
}
