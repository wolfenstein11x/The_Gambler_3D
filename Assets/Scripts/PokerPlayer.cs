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
    }

    public void CheckOrRaise()
    {
        // show picture of NPC

        // everything below here will be within coroutine, after coroutine delay
        //Debug.Log("betStarterIdx now:" + betTracker.betStarterIdx);
        //Debug.Log("currentBetterIdx:" + betTracker.currentBetterIdx);
        Debug.Log(nickName + " checks");

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
