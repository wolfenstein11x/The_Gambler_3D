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

    [SerializeField] Canvas dealCanvas;
    [SerializeField] Canvas revealCanvas;
    [SerializeField] Canvas newHandCanvas;
    [SerializeField] Canvas checkRaiseCanvas;
    [SerializeField] Canvas betCanvas;
    [SerializeField] Canvas callFoldRaiseCanvas;
    [SerializeField] Canvas raiseCanvas;
    [SerializeField] Canvas raiseBlindsCanvas;
    
    HandCalculator handCalculator;
    WinnerCalculator winnerCalculator;

    // Start is called before the first frame update
    void Start()
    {
        handCalculator = FindObjectOfType<HandCalculator>();
        winnerCalculator = FindObjectOfType<WinnerCalculator>();

        HideAllCanvases();
    }


    public void HideAllCanvases()
    {
        dealCanvas.enabled = false;
        revealCanvas.enabled = false;
        newHandCanvas.enabled = false;
        checkRaiseCanvas.enabled = false;
        betCanvas.enabled = false;
        callFoldRaiseCanvas.enabled = false;
        raiseCanvas.enabled = false;
        raiseBlindsCanvas.enabled = false;

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

    public void ShowRaiseBlindsCanvas()
    {
        ShowCanvas(raiseBlindsCanvas);
    }

    public void HideRaiseBlindsCanvas()
    {
        raiseBlindsCanvas.enabled = false;
    }

    
}
