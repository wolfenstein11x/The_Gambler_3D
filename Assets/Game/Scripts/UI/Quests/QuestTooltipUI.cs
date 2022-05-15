using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Quests;
using UnityEngine.UI;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] Text title;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject objectivePrefab;

        public void Setup(Quest quest)
        {
            title.text = quest.GetTitle();
            
            //objectiveContainer.DetachChildren();
            /*foreach(string objective in quest.GetObjectives())
            {
                GameObject objectiveInstance = Instantiate(objectivePrefab, objectiveContainer);
                Text objectiveText = objectiveInstance.GetComponentInChildren<Text>();
                objectiveText.text = objective;
            }*/
        }
    }
}
