using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokerAI : MonoBehaviour
{ 
    BetTracker betTracker;
    ControlHub controlHub;
    CanvasController canvasController;
    PotManager potManager;

    private void Start()
    {
        betTracker = FindObjectOfType<BetTracker>();
        controlHub = FindObjectOfType<ControlHub>();
        canvasController = FindObjectOfType<CanvasController>();
        potManager = FindObjectOfType<PotManager>();
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

        string decision = "I check!";
        canvasController.ShowBetterDecision(decision);

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

        //Fold();
        Call();


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
}
