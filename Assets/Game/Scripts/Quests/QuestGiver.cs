using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;

namespace RPG.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] Quest quest;
        

        public void GiveQuest()
        {
            GetComponent<AIConversant>().SetDialogueToMidQuest();
        }
    }
}
