using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Quests;

public class GameData : MonoBehaviour
{
    [SerializeField] Quest gregQuest;
    static bool gregQuestInProgress;
    static bool gregQuestComplete;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("in progress: " + gregQuestInProgress);
        Debug.Log("complete: " + gregQuestComplete);
    }

    public void UpdateGregQuestStatus()
    {
        gregQuestInProgress = gregQuest.GetQuestInProgress();
        gregQuestComplete = gregQuest.GetQuestCompleted();
    }

    
}
