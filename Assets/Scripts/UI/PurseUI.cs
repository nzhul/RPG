using RPG.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI balanceField;

        Purse _playerPurse = null;

        private void Start()
        {
            _playerPurse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>();

            if (_playerPurse != null)
            {
                _playerPurse.onChange += RefreshUI;
            }

            RefreshUI();
        }

        public void RefreshUI()
        {
            balanceField.text = $"${_playerPurse.GetBalance():N2}";
        }
    }
}