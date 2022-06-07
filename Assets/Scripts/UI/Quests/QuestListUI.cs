using RPG.Quests;
using UnityEngine;

namespace RGP.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] QuestItemUI questPrefab;
        QuestList questList;

        void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.onUpdate += Redraw;
            Redraw();
        }

        private void Redraw()
        {
            foreach (Transform item in transform)
            {
                Destroy(item.gameObject);
            }

            foreach (var status in questList.GetStatuses())
            {
                var uiInstance = Instantiate(questPrefab, transform);
                uiInstance.Setup(status);
            }
        }
    }

}
