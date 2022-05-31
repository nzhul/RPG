using RPG.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant _playerConversant;
        [SerializeField] TextMeshProUGUI AIText;
        [SerializeField] Button nextBtn;
        [SerializeField] GameObject AIResponse;
        [SerializeField] Transform choiseRoot;
        [SerializeField] GameObject choisePrefab;
        [SerializeField] Button quitButton;
        [SerializeField] TextMeshProUGUI conversantName;

        private void Start()
        {
            _playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            _playerConversant.onConversationUpdated += UpdateUI;
            nextBtn.onClick.AddListener(() => _playerConversant.Next());
            quitButton.onClick.AddListener(() => _playerConversant.Quit());
            UpdateUI();
        }

        private void UpdateUI()
        {
            gameObject.SetActive(_playerConversant.IsActive());

            if (!_playerConversant.IsActive())
            {
                return;
            }

            conversantName.text = _playerConversant.GetCurrentConversantName();

            AIResponse.SetActive(!_playerConversant.IsChoosing());
            choiseRoot.gameObject.SetActive(_playerConversant.IsChoosing());

            if (_playerConversant.IsChoosing())
            {
                BuildChoiseList();
            }
            else
            {
                AIText.text = _playerConversant.GetText();
                nextBtn.gameObject.SetActive(_playerConversant.HasNext());
            }
        }

        private void BuildChoiseList()
        {
            foreach (Transform item in choiseRoot)
            {
                Destroy(item.gameObject);
            }

            foreach (var choise in _playerConversant.GetChoises())
            {
                var choiseInstance = Instantiate(choisePrefab, choiseRoot);
                choiseInstance.GetComponentInChildren<TextMeshProUGUI>().text = choise.GetText();
                var button = choiseInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    _playerConversant.SelectChoise(choise);
                });
            }
        }
    }
}
