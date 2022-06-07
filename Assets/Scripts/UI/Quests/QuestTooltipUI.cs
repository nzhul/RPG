using RPG.Quests;
using System;
using TMPro;
using UnityEngine;

namespace RGP.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject objectivePrefab;
        [SerializeField] GameObject objectiveIncompletePrefab;
        [SerializeField] TextMeshProUGUI rewardText;

        public void Setup(QuestStatus status)
        {
            var quest = status.GetQuest();
            title.text = quest.GetTitle();
            foreach (Transform item in objectiveContainer)
            {
                Destroy(item.gameObject);
            }

            foreach (var objective in quest.GetObjectives())
            {
                var prefab = status.IsObjectiveComplete(objective.reference) ? objectivePrefab : objectiveIncompletePrefab;
                var objectiveInstance = Instantiate(prefab, objectiveContainer);
                var objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();
                objectiveText.text = objective.description;
            }

            rewardText.text = GetRewardText(quest);
        }

        private string GetRewardText(Quest quest)
        {
            var rewardText = string.Empty;
            foreach (var reward in quest.GetRewards())
            {
                if (rewardText != string.Empty)
                {
                    rewardText += ", ";
                }

                if (reward.number > 1)
                {
                    rewardText += reward.number + " ";
                }

                rewardText += reward.item.GetDisplayName();
            }

            if (rewardText == string.Empty)
            {
                rewardText = "No reward";
            }

            rewardText += ".";

            return rewardText;
        }
    }

}
