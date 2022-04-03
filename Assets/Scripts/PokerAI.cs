using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokerAI : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] float bluffRating;
    [SerializeField] [Range(0, 1)] float slowPlayRating;

    [SerializeField] float thinkTimeMin = 1f;
    [SerializeField] float thinkTimeMax = 2f;

    BetTracker betTracker;
    ControlHub controlHub;
    CanvasController canvasController;
    PotManager potManager;
    WinnerCalculator winnerCalculator;
    HandCalculator handCalculator;
    Dealer dealer;
    PokerPlayer pokerPlayer;

    private void Start()
    {
        betTracker = FindObjectOfType<BetTracker>();
        controlHub = FindObjectOfType<ControlHub>();
        canvasController = FindObjectOfType<CanvasController>();
        potManager = FindObjectOfType<PotManager>();
        winnerCalculator = FindObjectOfType<WinnerCalculator>();
        dealer = FindObjectOfType<Dealer>();
        handCalculator = FindObjectOfType<HandCalculator>();
        pokerPlayer = GetComponent<PokerPlayer>();
    }

    public void CheckOrRaise()
    {
        Debug.Log(dealer.players[3].nickName);

        // show player's headshot on canvas
        canvasController.ShowBetter();

        // skip to the end when player has zero money
        if (pokerPlayer.money <= 0)
        {
            pokerPlayer.HandleOptionToMoneylessPlayer();
            return;
        }

        StartCoroutine(CheckOrRaiseCoroutine(2f, 1f));
    }

    IEnumerator CheckOrRaiseCoroutine(float thinkTime, float transitionTime)
    {
        yield return new WaitForSeconds(thinkTime);

        // run AI algorithm to determine check or raise
        int decision = DecideCheckOrRaise();

        // return 1 for check, 3 for raise, 4 for big raise
        switch (decision)
        {
            case 1:
                Check();
                break;
            case 3:
                Raise();
                break;
            case 4:
                Raise(true);
                break;
            default:
                Check();
                break;
        }


        yield return new WaitForSeconds(transitionTime);

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

    public void CallRaiseOrFold()
    {
        Debug.Log(dealer.players[3].nickName);

        // show player's headshot on canvas
        canvasController.ShowBetter();

        // skip to the end when player has zero money
        if (pokerPlayer.money <= 0)
        {
            pokerPlayer.HandleOptionToMoneylessPlayer();
            return;
        }

        float thinkTime = Random.Range(thinkTimeMin, thinkTimeMax);

        StartCoroutine(CallRaiseOrFoldCoroutine(thinkTime, 1f));
    }

    IEnumerator CallRaiseOrFoldCoroutine(float thinkTime, float transitionTime)
    {
        yield return new WaitForSeconds(thinkTime);

        // run AI algorithm to determine call raise or fold
        int decision = DecideCallRaiseFold();

        // 0 for fold, 2 for call, 3 for raise, 4 for big raise
        switch (decision)
        {
            case 0:
                Fold();
                break;
            case 2:
                Call();
                break;
            case 3:
                Raise();
                break;
            case 4:
                Raise(true);
                break;
            default:
                Call();
                break;
        }

        yield return new WaitForSeconds(transitionTime);

        betTracker.IncrementCurrentBetter();

        // if we only have one player left, hand is over in premature win
        if (winnerCalculator.CheckForPrematureWinner(dealer.players))
        {
            controlHub.gameState = GameState.PrematureWin;
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

    private void Call()
    {
        // display NPC decision
        string decision = "I call!";
        canvasController.ShowBetterDecision(decision);

        // calculate amount player owes
        int toCall = potManager.highestBet - pokerPlayer.currentBet;

        // put call amount into pot, but no need to update bet starter
        potManager.CollectMoneyFromPlayer(pokerPlayer, toCall);
    }

    private void Fold()
    {
        // display NPC decision
        string decision = "I fold!";
        canvasController.ShowBetterDecision(decision);

        // mark player as folded
        GetComponent<PokerPlayer>().folded = true;

        // clear all images of player cards
        pokerPlayer.playerPosition.cardImg1.GetComponent<SpriteRenderer>().sprite = null;
        pokerPlayer.playerPosition.cardImg2.GetComponent<SpriteRenderer>().sprite = null;
    }

    private void Raise(bool bigRaise=false)
    {
        // Note: raise equals current highest bet plus amount you are adding... so think of the word being used as "I raise to this amount" instead of "I raise by this much"

        // initialize raise to minimum raise
        int raiseAmount = potManager.highestBet + potManager.bigBlind; 
        
        // standard increase will be between big blind and 3 times big blind
        if (!bigRaise) 
        {
            // get random increase amount between big blind and 3 times big blind
            int raiseMin = potManager.highestBet + potManager.bigBlind;
            int raiseMax = potManager.highestBet + 3 * potManager.bigBlind;
            raiseAmount = Random.Range(raiseMin, raiseMax+1);

            // make raise a multiple of the big blind
            raiseAmount -= (raiseAmount % potManager.bigBlind);        
        }

        // big increase will be between 3 times big blind and all-in
        else
        {
            // get random increase amount between 3 times big blind and all-in
            int raiseMin = potManager.highestBet + 3 * potManager.bigBlind;
            int raiseMax = potManager.highestBet + pokerPlayer.money;
            raiseAmount = Random.Range(raiseMin, raiseMax + 1);

            // make raise a multiple of the big blind
            raiseAmount -= (raiseAmount % potManager.bigBlind);
        }

        
        //int raiseAmount = 4;//potManager.highestBet + potManager.bigBlind;
        string decision = "I raise to $" + raiseAmount;

        // if can't afford raise amount, that means going all in
        if (raiseAmount >= pokerPlayer.money + pokerPlayer.currentBet)
        {
            raiseAmount = pokerPlayer.money + pokerPlayer.currentBet;
            decision = "I'm all in!";
        }

        canvasController.ShowBetterDecision(decision);

        // amount player puts in pot is the raise amount minus the money they already have put in
        int amountOwed = raiseAmount - GetComponent<PokerPlayer>().currentBet;

        // player puts money in pot and raise become new amount required to keep playing
        potManager.CollectMoneyFromPlayer(GetComponent<PokerPlayer>(), amountOwed);
        potManager.highestBet = raiseAmount;

        // player bet, so player is new betStarter
        betTracker.betStarterIdx = betTracker.currentBetterIdx;
    }


    private void Check()
    {
        string decision = "I check!";
        canvasController.ShowBetterDecision(decision);
    }

    private int DecideCallRaiseFold()
    {
        // return 0 for fold, 2 for call, 3 for raise, 4 for big raise

        List<string> hand = GetComponent<PokerPlayer>().hand;

        // pre-flop logic
        if (hand.Count <= 2)
        {
            // get weights
            float handStrength = handCalculator.ScorePocket(hand) / 43f;
            float bluff = Random.Range(0, bluffRating);

            // scale weights so they add up to 1.0 max
            handStrength *= 0.5f;
            bluff *= 0.5f;

            // combine weights to get total weight
            float weightedSum = handStrength + bluff;
            //Debug.Log("handStrength: " + handStrength);
            //Debug.Log("bluff: " + bluff);
            //Debug.Log("weighted sum: " + weightedSum);

            // compare weight with thresholds
            return AnalysisCallRaiseOrFold(weightedSum);
        }

        // post-flop logic
        else 
        {
            // get weights
            float handStrength = (handCalculator.ScoreHand(hand) - 1f) / 9f;
            float bluff = Random.Range(0, bluffRating);

            // scale weights so they add up to 1.0 max
            handStrength *= 0.5f;
            bluff *= 0.5f;

            // combine weights to get total weight
            float weightedSum = handStrength + bluff;
            //Debug.Log("handStrength: " + handStrength);
            //Debug.Log("bluff: " + bluff);
            //Debug.Log("weightedSum: " + weightedSum);

            // compare weight with thresholds
            return AnalysisCallRaiseOrFold(weightedSum);
        }
    }

    private int DecideCheckOrRaise()
    {
        // return 1 for call, 2 for raise, 3 for big raise

        List<string> hand = GetComponent<PokerPlayer>().hand;

        // pre-flop logic
        if (hand.Count <= 2)
        {
            // get weights
            float handStrength = handCalculator.ScorePocket(hand) / 43f;
            float bluff = Random.Range(0, bluffRating);
            float slowPlay = Random.Range(0, slowPlayRating);

            // scale weights so they add up to 1.0 max
            handStrength /= 3f;
            bluff /= 3f;
            slowPlay /= 3f;

            // combine weights to get total weight
            float weightedSum = handStrength + bluff - slowPlay;
            //Debug.Log("handStrength: " + handStrength);
            //Debug.Log("bluff: " + bluff);
            //Debug.Log("weightedSum: " + weightedSum);

            // compare weight with thresholds
            return AnalysisCheckOrRaise(weightedSum);
        }

        // post-flop logic
        else
        {
            // get weights
            float handStrength = (handCalculator.ScoreHand(hand) - 1f) / 9f;
            float bluff = Random.Range(0, bluffRating);
            float slowPlay = Random.Range(0, slowPlayRating);

            // scale weights so they add up to 1.0 max
            handStrength /= 3f;
            bluff /= 3f;
            slowPlay /= 3f;

            // combine weights to get total weight
            float weightedSum = handStrength + bluff - slowPlay;
            //Debug.Log("handStrength: " + handStrength);
            //Debug.Log("bluff: " + bluff);
            //Debug.Log("weightedSum: " + weightedSum);

            // compare weight with thresholds
            return AnalysisCheckOrRaise(weightedSum);
        }

    }

    private int AnalysisCallRaiseOrFold(float weightedSum)
    {
        // return 0 for fold, 2 for call, 3 for raise, 4 for big raise

        // compare weighted sum with thresholds
        if (weightedSum >= 0.6) { return 4; }
        else if (weightedSum >= 0.4) { return 3; }
        else if (weightedSum >= 0.15) { return 2; }
        else { return 0; }
    }

    private int AnalysisCheckOrRaise(float weightedSum)
    {
        // return 1 for check, 3 for raise, 4 for big raise

        // compare weighted sum with thresholds
        if (weightedSum >= 0.6) { return 4; }
        if (weightedSum >= 0.2) { return 3; }
        else { return 1; }
    }

    private void HandleOptionToMoneylessAI()
    {

    }

    
}
