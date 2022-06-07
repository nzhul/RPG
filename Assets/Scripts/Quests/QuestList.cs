using GameDevTV.Inventories;
using RPG.Core;
using RPG.Saving;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        List<QuestStatus> statuses = new List<QuestStatus>();
        public event Action onUpdate;

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest))
            {
                return;
            }

            var questStatus = new QuestStatus(quest);
            statuses.Add(questStatus);
            onUpdate?.Invoke();
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            var status = GetQuestStatus(quest);
            status.CompleteObjective(objective);

            if (status.IsComplete())
            {
                GiveReward(quest);
            }

            onUpdate?.Invoke();
        }

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        private bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            foreach (var status in statuses)
            {
                if (status.GetQuest() == quest)
                {
                    return status;
                }
            }

            return null;
        }

        private void GiveReward(Quest quest)
        {
            foreach (var reward in quest.GetRewards())
            {
                var success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);

                if (!success)
                {
                    GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
                }
            }
        }

        public object CaptureState()
        {
            var state = new List<object>();
            foreach (var status in statuses)
            {
                state.Add(status.CaptureState());
            }

            return state;
        }

        public void RestoreState(object state)
        {
            var stateList = state as List<object>;
            if (stateList == null) return;

            statuses.Clear();
            foreach (var objectState in stateList)
            {
                statuses.Add(new QuestStatus(objectState));
            }
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {

            switch (predicate)
            {
                case "HasQuest":
                    return HasQuest(Quest.GetByName(parameters[0]));
                case "CompletedQuest":
                    return GetQuestStatus(Quest.GetByName(parameters[0])).IsComplete();
            }

            return null;

        }
    }

}