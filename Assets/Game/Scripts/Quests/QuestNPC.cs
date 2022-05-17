using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestNPC : MonoBehaviour
{
    [SerializeField] string pokerScene;

    public void StartPokerMatch()
    {
        SceneManager.LoadScene(pokerScene);
    }

    
}
