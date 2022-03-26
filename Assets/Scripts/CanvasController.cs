using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] Canvas handWonCanvas;
    [SerializeField] Text winnerInfoText;

    HandCalculator handCalculator;
    WinnerCalculator winnerCalculator;

    // Start is called before the first frame update
    void Start()
    {
        handCalculator = FindObjectOfType<HandCalculator>();
        winnerCalculator = FindObjectOfType<WinnerCalculator>();

        HideAllCanvases();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideAllCanvases()
    {
        handWonCanvas.enabled = false;
    }

    public void HandleHandWon()
    {
        handWonCanvas.enabled = true;

        List<PokerPlayer> winners = winnerCalculator.winners;

        int winningHand = winnerCalculator.highScore;
        string winningHandName = handCalculator.handNames[winningHand];

        if (winners.Count == 1) 
        {
            winnerInfoText.text = winners[0].gameObject.name + " wins with " + winningHandName;
        }
        
        else 
        {
            winnerInfoText.text = "Split pot! High hand is " + winningHandName;
        }
    }
}
