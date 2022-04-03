using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokerAI : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] float bluffRating; 

    BetTracker betTracker;
    ControlHub controlHub;
    CanvasController canvasController;
    PotManager potManager;
    WinnerCalculator winnerCalculator;
    HandCalculator handCalculator;
    Dealer dealer;

    private void Start()
    {
        betTracker = FindObjectOfType<BetTracker>();
        controlHub = FindObjectOfType<ControlHub>();
        canvasController = FindObjectOfType<CanvasController>();
        potManager = FindObjectOfType<PotManager>();
        winnerCalculator = FindObjectOfType<WinnerCalculator>();
        dealer = FindObjectOfType<Dealer>();
        handCalculator = FindObjectOfType<HandCalculator>();
    }

    public void CheckOrRaise()
    {
        // show player's headshot on canvas
        canvasController.ShowBetter();

        StartCoroutine(CheckOrRaiseCoroutine(2f, 1f));
    }

    IEnumerator CheckOrRaiseCoroutine(float thinkTime, float transitionTime)
    {
        yield return new WaitForSeconds(thinkTime);

        // run AI algorithm to determine check or raise
        int decision = DecideCheckOrRaise();

        // return 1 for check, 2 for raise, 3 for all-in
        switch (decision)
        {
            case 1:
                Check();
                break;
            case 2:
                Check();
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
        // show player's headshot on canvas
        canvasController.ShowBetter();

        StartCoroutine(CallRaiseOrFoldCoroutine(2f, 1f));
    }

    IEnumerator CallRaiseOrFoldCoroutine(float thinkTime, float transitionTime)
    {
        yield return new WaitForSeconds(thinkTime);

        // run AI algorithm to determine call raise or fold
        int decision = DecideCallRaiseFold();

        // get raise amount if decision is raise

        // 0 for fold, 1 for call, 2 for raise, 3 for all-in
        switch (decision)
        {
            case 0:
                Fold();
                break;
            case 1:
                Call();
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
        int toCall = potManager.highestBet - GetComponent<PokerPlayer>().currentBet;

        // put call amount into pot, but no need to update bet starter
        potManager.CollectMoneyFromPlayer(GetComponent<PokerPlayer>(), toCall);
    }

    private void Fold()
    {
        // display NPC decision
        string decision = "I fold!";
        canvasController.ShowBetterDecision(decision);

        // mark player as folded
        GetComponent<PokerPlayer>().folded = true;

        // clear all images of player cards
        GetComponent<PokerPlayer>().playerPosition.cardImg1.GetComponent<SpriteRenderer>().sprite = null;
        GetComponent<PokerPlayer>().playerPosition.cardImg2.GetComponent<SpriteRenderer>().sprite = null;

    }

    private void Check()
    {
        string decision = "I check!";
        canvasController.ShowBetterDecision(decision);
    }

    private int DecideCallRaiseFold()
    {
        // return 0 for fold, 1 for call, 2 for raise, 3 for all-in

        List<string> hand = GetComponent<PokerPlayer>().hand;

        // pre-flop logic
        if (hand.Count <= 2)
        {
            // get weights
            float handStrength = handCalculator.ScorePocket(hand) / 43f;
            float bluff = Random.Range(0, bluffRating);

            // combine weights to get total weight
            float weightedSum = handStrength + bluff;
            Debug.Log("handStrength: " + handStrength);
            Debug.Log("bluff: " + bluff);
            Debug.Log("weighted sum: " + weightedSum);

            // compare weight with thresholds
            return PreFlopAnalysisCallRaiseOrFold(weightedSum);
        }

        // post-flop logic
        else 
        {
            // get weights
            float handStrength = (handCalculator.ScoreHand(hand) - 1f) / 9f;
            float bluff = Random.Range(0, bluffRating);

            // combine weights to get total weight
            float totalWeight = handStrength + bluff;
            Debug.Log("handStrength: " + handStrength);
            Debug.Log("bluff: " + bluff);
            Debug.Log("totalWeight: " + totalWeight);

            // compare weight with thresholds
            return PostFlopAnalysisCallRaiseOrFold(totalWeight);
        }
    }

    private int DecideCheckOrRaise()
    {
        // return 1 for call, 2 for raise, 3 for all-in

        List<string> hand = GetComponent<PokerPlayer>().hand;

        // pre-flop logic
        if (hand.Count <= 2)
        {
            // get weights
            float handStrength = handCalculator.ScorePocket(hand) / 43f;
            float bluff = Random.Range(0, bluffRating);

            // combine weights to get total weight
            float weightedSum = handStrength + bluff;
            Debug.Log("handStrength: " + handStrength);
            Debug.Log("bluff: " + bluff);
            Debug.Log("weighted sum: " + weightedSum);

            // compare weight with thresholds
            return AnalysisCheckOrRaise(weightedSum);
        }

        // post-flop logic
        else
        {
            // get weights
            float handStrength = (handCalculator.ScoreHand(hand) - 1f) / 9f;
            float bluff = Random.Range(0, bluffRating);

            // combine weights to get total weight
            float weightedSum = handStrength + bluff;
            Debug.Log("handStrength: " + handStrength);
            Debug.Log("bluff: " + bluff);
            Debug.Log("totalWeight: " + weightedSum);

            // compare weight with thresholds
            return AnalysisCheckOrRaise(weightedSum);
        }

    }

    private int PreFlopAnalysisCallRaiseOrFold(float weightedSum)
    {
        // return 0 for fold, 1 for call, 2 for raise, 3 for all-in

        // compare weighted sum with thresholds
        if (weightedSum >= 0.5) { return 2; }
        else { return 0; }
    }

    private int PostFlopAnalysisCallRaiseOrFold(float weightedSum)
    {
        // return 0 for fold, 1 for call, 2 for raise, 3 for all-in

        // compare weighted sum with thresholds
        if (weightedSum >= 0.5) { return 1; }
        else { return 0; }
    }

    private int AnalysisCheckOrRaise(float weightedSum)
    {
        // return 1 for call, 2 for raise, 3 for all-in

        // compare weighted sum with thresholds
        if (weightedSum >= 0.5) { return 2; }
        else { return 1; }
    }
}
