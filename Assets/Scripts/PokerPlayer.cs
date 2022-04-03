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
    
    // amount the player has put in... must pay (toCall - currentBet) to play
    public int currentBet;

    HandCalculator handCalculator;
    Dealer dealer;
    BetTracker betTracker;
    ControlHub controlHub;
    CanvasController canvasController;
    PotManager potManager;
    WinnerCalculator winnerCalculator;

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
        winnerCalculator = FindObjectOfType<WinnerCalculator>();
    }

    public void CheckOrRaise()
    {
        // show player's headshot on canvas
        canvasController.ShowBetter();

        // skip to the end when player has zero money
        if (money <= 0)
        {
            HandleOptionToMoneylessPlayer();
            return;
        }
        
        // bring up check or raise canvas and raise panel canvas (but don't clear currently displayed canvases, hence the 'false' parameters)
        canvasController.ShowCanvas(canvasController.checkRaiseCanvas, false);
        canvasController.ShowCanvas(canvasController.raisePanelCanvas, false);

        // display minimum raise, which will be one big blind over the current highest bet (so just big blind in this case)
        int minRaise = potManager.highestBet + potManager.bigBlind;
        canvasController.raiseAmountText.text = "$" + minRaise;

        // now that Check button and Raise button are visible, pushing those will call necessary functions and send us to next state
    }

    public void CallRaiseOrFold()
    {
        // show player's headshot on canvas
        canvasController.ShowBetter();

        // bring up check or raise canvas and riase panel canvas (but don't clear currently displayed canvases, hence the 'false' parameters)
        canvasController.ShowCanvas(canvasController.callFoldRaiseCanvas, false);
        canvasController.ShowCanvas(canvasController.raisePanelCanvas, false);

        // display amount needed to call
        canvasController.callAmountText.text = "$" + (potManager.highestBet - currentBet).ToString();

        // display minimum raise, which will be one big blind over the current highest bet
        int minRaise = potManager.highestBet + potManager.bigBlind;
        canvasController.raiseAmountText.text = "$" + minRaise;

        // now that Check button and Raise button are visible, pushing those will call necessary functions and send us to next state
    }

    public void CheckButton()
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

    public void RaiseButton()
    {
        // Note: raise equals current highest bet plus amount you are adding... so think of the word being used as "I raise to this amount" instead of "I raise by this much"

        // don't let player raise if player has zero money
        if (money <= 0) { return; }

        // get amount from text box
        int raiseAmount = controlHub.ParseRaiseAmountText();

        string decision = "I raise to $" + raiseAmount;
        canvasController.ShowBetterDecision(decision);

        // amount player puts in pot is the raise amount minus the money they already have put in
        int amountOwed = raiseAmount - currentBet;

        // player puts money in pot and raise become new amount required to keep playing
        potManager.CollectMoneyFromPlayer(this, amountOwed);
        potManager.highestBet = raiseAmount;

        // player bet, so player is new betStarter
        betTracker.betStarterIdx = betTracker.currentBetterIdx;

        // now that we've stored the new bet starter, we can increment the current better
        betTracker.IncrementCurrentBetter();

        // go back to bet round state no matter what, since can't end round on a raise
        controlHub.SetState(controlHub.prevState);
        controlHub.RunStateMachine();
    }

    public void AllInButton()
    {
        // don't let player raise if player has zero money
        if (money <= 0) { return; }

        int raiseAmount = currentBet + money;

        string decision = "I'm all in!";
        canvasController.ShowBetterDecision(decision);

        // amount player puts in pot is the raise amount minus the money they already have put in
        int amountOwed = raiseAmount - currentBet;

        // player puts money in pot and raise become new amount required to keep playing
        potManager.CollectMoneyFromPlayer(this, amountOwed);
        potManager.highestBet = raiseAmount;

        // player bet, so player is new betStarter
        betTracker.betStarterIdx = betTracker.currentBetterIdx;

        // now that we've stored the new bet starter, we can increment the current better
        betTracker.IncrementCurrentBetter();

        // go back to bet round state no matter what, since can't end round on a raise
        controlHub.SetState(controlHub.prevState);
        controlHub.RunStateMachine();
    }

    public void CallButton()
    {
        // display NPC decision
        string decision = "I call!";
        canvasController.ShowBetterDecision(decision);

        // calculate amount player owes
        int toCall = potManager.highestBet - GetComponent<PokerPlayer>().currentBet;

        // put call amount into pot, but no need to update bet starter
        potManager.CollectMoneyFromPlayer(this, toCall);

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

    public void FoldButton()
    {
        // display NPC decision
        string decision = "I fold!";
        canvasController.ShowBetterDecision(decision);

        // mark player as folded
        folded = true;

        // clear all images of player cards
        playerPosition.cardImg1.GetComponent<SpriteRenderer>().sprite = null;
        playerPosition.cardImg2.GetComponent<SpriteRenderer>().sprite = null;

        betTracker.IncrementCurrentBetter();

        // if we only have one player left, hand is over
        if (winnerCalculator.CheckForPrematureWinner(dealer.players))
        {
            controlHub.gameState = GameState.Reveal;
            controlHub.RunStateMachine();
        }

        // go to BetRound state if not reached end of bet sequence
        else if (betTracker.currentBetterIdx != betTracker.betStarterIdx)
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

    public void HandleOptionToMoneylessPlayer()
    {
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

    


}
