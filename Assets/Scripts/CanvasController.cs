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

    [SerializeField] Canvas betRoundCanvas;
    [SerializeField] Text decisionText;
    [SerializeField] Image betterHeadshot;

    public Canvas raisePanelCanvas;
    public Text raiseAmountText;
    public Text callAmountText;

    [SerializeField] Canvas dealCanvas;
    [SerializeField] Canvas revealCanvas;
    [SerializeField] Canvas newHandCanvas;
    public Canvas checkRaiseCanvas;
    public Canvas callFoldRaiseCanvas;
    [SerializeField] Canvas raiseBlindsCanvas;
    public Canvas gameOverCanvas;
    public Canvas matchWonCanvas;
    
    HandCalculator handCalculator;
    WinnerCalculator winnerCalculator;
    Dealer dealer;
    BetTracker betTracker;

    // Start is called before the first frame update
    void Start()
    {
        handCalculator = FindObjectOfType<HandCalculator>();
        winnerCalculator = FindObjectOfType<WinnerCalculator>();
        dealer = FindObjectOfType<Dealer>();
        betTracker = FindObjectOfType<BetTracker>();

        HideAllCanvases();
    }


    public void HideAllCanvases()
    {
        dealCanvas.enabled = false;
        revealCanvas.enabled = false;
        newHandCanvas.enabled = false;
        checkRaiseCanvas.enabled = false;
        callFoldRaiseCanvas.enabled = false;
        raiseBlindsCanvas.enabled = false;
        betRoundCanvas.enabled = false;
        raisePanelCanvas.enabled = false;
        gameOverCanvas.enabled = false;
        matchWonCanvas.enabled = false;

        handWonCanvas.enabled = false;
        
    }

    // show only specified canvas (alone) or show it on top of currently displayed canvases (alone=false)
    public void ShowCanvas(Canvas canvas, bool alone=true)
    {
        if (alone) { HideAllCanvases(); }
        canvas.enabled = true;
    }

    public void HandleDeal()
    {
        ShowCanvas(dealCanvas);
    }

    public void HandleOptionToPlayer()
    {
        ShowCanvas(checkRaiseCanvas);
    }

    public void HandleReveal()
    {
        ShowCanvas(revealCanvas);
    }

    public void HandleNewHand()
    {
        ShowCanvas(newHandCanvas, false);
    }

    public void HandleHandWon()
    {
        ShowCanvas(handWonCanvas);
        
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

    public void HandleHandWonPremature()
    {
        ShowCanvas(handWonCanvas);

        // display winner catchphrase
        catchPhraseText.text = winnerCalculator.winners[0].catchPhrase;

        // display winner headshot
        winnerHeadshot.sprite = winnerCalculator.winners[0].headShot;

        // display winner info text
        winnerInfoText.text = winnerCalculator.winners[0].nickName + " WINS!";
       
    }

    public void ShowBetter()
    {
        ShowCanvas(betRoundCanvas);

        // clear text from previous NPC
        decisionText.text = "";

        // display NPC headshot
        betterHeadshot.sprite = dealer.players[betTracker.currentBetterIdx].headShot;
    }

    public void ShowBetterDecision(string decision)
    {
        decisionText.text = decision;
    }

    public void ShowRaiseBlindsCanvas()
    {
        ShowCanvas(raiseBlindsCanvas);
    }

    public void HideRaiseBlindsCanvas()
    {
        raiseBlindsCanvas.enabled = false;
    }

    
}
