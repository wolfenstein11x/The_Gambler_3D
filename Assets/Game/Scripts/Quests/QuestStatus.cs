using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Quests;

public class QuestStatus : MonoBehaviour
{

    [SerializeField] Quest quest;
    
    QuestGiver questGiver;

    static bool questInProgress;
    static bool questComplete;

    // Start is called before the first frame update
    void Start()
    {
        questGiver = GetComponent<QuestGiver>();

        Debug.Log("In progress: " + questInProgress);
        Debug.Log("Completed: " + questComplete);
    }

    public void UpdateGregQuestStatus()
    {
        questInProgress = quest.GetQuestInProgress();
        questComplete = quest.GetQuestCompleted();

        Debug.Log("In progress: " + questInProgress);
        Debug.Log("Completed: " + questComplete);
    }
}
