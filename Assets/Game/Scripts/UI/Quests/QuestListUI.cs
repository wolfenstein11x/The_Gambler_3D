using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Quests;

public class QuestListUI : MonoBehaviour
{
    [SerializeField] Quest[] tempQuests;
    [SerializeField] QuestItemUI questPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // clear all quests to clean it up
        transform.DetachChildren();

        // show all quests
        foreach(Quest quest in tempQuests)
        {
            QuestItemUI uiInstance = Instantiate<QuestItemUI>(questPrefab, transform);
            uiInstance.Setup(quest);
        }
    }




}
