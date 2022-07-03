using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class SaveLoadUI : MonoBehaviour
    {
        [SerializeField] Transform contentRoot;
        [SerializeField] GameObject buttonPrefab;

        private void OnEnable()
        {
            var savingWrapper = FindObjectOfType<SavingWrapper>();
            if (savingWrapper == null) return;

            foreach (Transform child in contentRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (var save in savingWrapper.ListSaves())
            {
                var buttonInstance = Instantiate(buttonPrefab, contentRoot);
                var textComp = buttonInstance.GetComponentInChildren<TMP_Text>();
                textComp.text = save;
                var button = buttonInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    savingWrapper.LoadGame(save);
                });
            }

        }
    }
}
