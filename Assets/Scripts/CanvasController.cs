using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] Canvas handWonCanvas;
    [SerializeField] Text catchPhraseText;
    [SerializeField] Image winnerHeadshot;
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

        // display winner catchphrase
        catchPhraseText.text = winners[0].catchPhrase;

        // display winner headshot
        winnerHeadshot.sprite = winners[0].headShot;

        // display winner info text
        if (winners.Count == 1) 
        {
            winnerInfoText.text = winners[0].nickName + " WINS WITH " + winningHandName.ToUpper();
        }
        
        else 
        {
            winnerInfoText.text = "SPLIT POT! HIGH HAND IS " + winningHandName.ToUpper();
        }
    }
}
