using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;

namespace RPG.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] Quest quest;
        [SerializeField] GameObject[] QuestNPCs;

        public void GiveQuest()
        {
            GetComponent<AIConversant>().SetDialogueToMidQuest();
            ActivateQuestNPCs();
        }

        private void ActivateQuestNPCs()
        {
            foreach(GameObject NPC in QuestNPCs)
            {
                NPC.SetActive(true);
            }
        }
    }
}
