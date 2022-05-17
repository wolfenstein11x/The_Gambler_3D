using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "RPG Project/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] string[] objectives;
        [SerializeField] static bool inProgress = false;
        [SerializeField] static bool completed = false;

        public string GetTitle()
        {
            return name;
        }

        public int GetObjectiveCount()
        {
            return objectives.Length;
        }

        public IEnumerable<string> GetObjectives()
        {
            return objectives;
        }

        public bool GetQuestCompleted()
        {
            return completed;
        }

        public bool GetQuestInProgress()
        {
            return inProgress;
        }

        public void CompleteQuest()
        {
            completed = true;
        }

        public void StartQuest()
        {
            inProgress = true;
        }
    }

    
}
