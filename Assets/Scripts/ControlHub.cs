using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Init, DealHands, DealFlop, DealTurn, DealRiver, Reveal, NewHand,  }

public class ControlHub : MonoBehaviour
{
    public GameState gameState;

    private Dealer dealer;
    private CanvasController canvasController;
    private PotController potController;
    private HandCalculator handCalculator;
    private WinnerCalculator winnerCalculator;

    // Start is called before the first frame update
    void Start()
    {
        dealer = FindObjectOfType<Dealer>();
        canvasController = FindObjectOfType<CanvasController>();
        handCalculator = FindObjectOfType<HandCalculator>();
        winnerCalculator = FindObjectOfType<WinnerCalculator>();
        potController = FindObjectOfType<PotController>();

        gameState = GameState.Init;

        RunStateMachine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RunStateMachine()
    { 
        if (gameState == GameState.Init)
        {
            // randomize players around table
            dealer.InitializePlayers();
            
            // give each player $100
            potController.InitMoney();
            
            // start main player off with dealer chip
            dealer.SetDealer(0);

            // move to next state
            gameState = GameState.DealHands;

            // continue running state machine
            RunStateMachine();
        }

        else if (gameState == GameState.DealHands)
        {
            // show deal button (will disappear after pressed)
            canvasController.HandleDeal();

            // collect blinds
            potController.CollectBlinds();

            // begin bet sequence
            Debug.Log("its bettin time");


        }
        
    }

    public void SetState(GameState state)
    {
        gameState = state;
    }
}
