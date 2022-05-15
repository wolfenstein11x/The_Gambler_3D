using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Quests;
using UnityEngine.UI;

public class QuestItemUI : MonoBehaviour
{
    [SerializeField] Text title;
    [SerializeField] Text progress;

    public void Setup(Quest quest)
    {
        title.text = quest.GetTitle();
        progress.text = "0/" + quest.GetObjectiveCount();
    }
}
