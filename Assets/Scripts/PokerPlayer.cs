using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerPlayer : MonoBehaviour
{
    public string nickName;
    public string catchPhrase;
    public Sprite headShot;
    public GameObject cardImage1, cardImage2;
    public PlayerPosition playerPosition;

    public List<int> cards;
    public List<string> hand;
    public List<string> optimizedHand;
    public int money;
    public bool eliminated;
    public bool folded;

    HandCalculator handCalculator;
    Dealer dealer;
    BetTracker betTracker;
    ControlHub controlHub;
    CanvasController canvasController;
    PotManager potManager;

    // Start is called before the first frame update
    void Start()
    {
        eliminated = false;
        folded = false;
        
        betTracker = FindObjectOfType<BetTracker>();
        handCalculator = FindObjectOfType<HandCalculator>();
        dealer = FindObjectOfType<Dealer>();
        controlHub = FindObjectOfType<ControlHub>();
        canvasController = FindObjectOfType<CanvasController>();
        potManager = FindObjectOfType<PotManager>();
    }

    public void CheckOrRaise()
    {
        // show player's headshot on canvas
        canvasController.ShowBetter();

        // bring up check or raise canvas (but don't clear currently displayed canvases, hence the 'false' parameter)
        canvasController.ShowCanvas(canvasController.checkRaiseCanvas, false);

        // now that Check button and Raise button are visible, pushing those will call necessary functions and send us to next state
    }

    public void Check()
    {
        string decision = "I check!";
        canvasController.ShowBetterDecision(decision);

        betTracker.IncrementCurrentBetter();

        // go to BetRound state if not reached end of bet sequence
        if (betTracker.currentBetterIdx != betTracker.betStarterIdx)
        {
            // go to correct bet round depending on previous state
            controlHub.SetState(controlHub.prevState);
            controlHub.RunStateMachine();
        }

        // go to correct BetRoundDone state if reached end of bet sequence
        else
        {
            if (controlHub.prevState == GameState.BetRound1) { controlHub.gameState = GameState.BetRound1Done; }
            else if (controlHub.prevState == GameState.BetRound2) { controlHub.gameState = GameState.BetRound2Done; }
            else if (controlHub.prevState == GameState.BetRound3) { controlHub.gameState = GameState.BetRound3Done; }
            else if (controlHub.prevState == GameState.BetRound4) { controlHub.gameState = GameState.BetRound4Done; }

            controlHub.RunStateMachine();
        }
    }

    public void Raise()
    {
        // get amount from text box
        // for now just make it $4
        int raise = 4;

        string decision = "I raise $" + raise;
        canvasController.ShowBetterDecision(decision);

        potManager.CollectMoneyFromPlayer(this, raise);

        // player bet, so player is new betStarter
        betTracker.betStarterIdx = betTracker.currentBetterIdx;

        // now that we've stored the new bet starter, we can increment the current better
        betTracker.IncrementCurrentBetter();

        // go back to bet round state no matter what, since can't end round on a raise
        controlHub.SetState(controlHub.prevState);
        controlHub.RunStateMachine();
    }

    


}
