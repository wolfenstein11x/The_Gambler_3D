using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Init, DealHands, DealFlop, DealTurn, DealRiver, Reveal, NewHand,  }

public class ControlHub : MonoBehaviour
{
    public GameState gameState;

    private Dealer dealer;
    private CanvasController canvasController;
    private PotManager potManager;
    private HandCalculator handCalculator;
    private WinnerCalculator winnerCalculator;
    private BetTracker betTracker;

    // Start is called before the first frame update
    void Start()
    {
        dealer = FindObjectOfType<Dealer>();
        canvasController = FindObjectOfType<CanvasController>();
        handCalculator = FindObjectOfType<HandCalculator>();
        winnerCalculator = FindObjectOfType<WinnerCalculator>();
        potManager = FindObjectOfType<PotManager>();
        betTracker = FindObjectOfType<BetTracker>();

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
            potManager.InitMoney(dealer.players);
            
            // start main player off with dealer chip
            dealer.SetDealer(0);

            // show deal button, which will move us to DealHands state when pressed
            canvasController.HandleDeal();
        }

        else if (gameState == GameState.DealHands)
        {
            // deal cards to all players
            dealer.DealToPlayers();

            // collect blinds
            potManager.CollectBlinds(dealer.players);

            // run pre-flop bet sequence
            betTracker.BetRoundType1();

            // bet sequence has concluded, so show deal button, which will move us to DealFlop state when pressed
            canvasController.HandleDeal();
        }

        else if (gameState == GameState.DealFlop)
        {
            // deal flop
            dealer.DealFlop();

            // run post-flop bet sequence
            betTracker.BetRoundType2();

            // bet sequence has concluded, so show deal button, which will move us to DealTurn state when pressed
            canvasController.HandleDeal();
        }

        else if (gameState == GameState.DealTurn)
        {
            // deal turn
            dealer.DealTurn();

            // run post-flop bet sequence
            betTracker.BetRoundType2();

            // bet sequence has concluded, so show deal button, which will move us to DealTurn state when pressed
            canvasController.HandleDeal();
        }

        else if (gameState == GameState.DealRiver)
        {
            // deal turn
            dealer.DealRiver();

            // run post-flop bet sequence
            betTracker.BetRoundType2();

            // bet sequence has concluded, so show reveal button, which will move us to Reveal state when pressed
            canvasController.HandleReveal();
        }

        else if (gameState == GameState.Reveal)
        {
            // reveal NPC cards (unless they are folded or eliminated)
            dealer.Reveal();
        }

    }

    public void DealButton()
    {
        // hide button after it is pressed
        canvasController.HideAllCanvases();

        // move to particular deal state depending on current state
        if (gameState == GameState.Init) { gameState = GameState.DealHands; }
        else if (gameState == GameState.DealHands) { gameState = GameState.DealFlop; }
        else if (gameState == GameState.DealFlop) { gameState = GameState.DealTurn; }
        else if (gameState == GameState.DealTurn) { gameState = GameState.DealRiver; }

        // run the state machine now that it is in updated state
        RunStateMachine();
    }

    public void RevealButton()
    {
        // hide button after it is pressed
        canvasController.HideAllCanvases();

        // move to Reveal state
        gameState = GameState.Reveal;
        
        // run the state machine now that it is in updated state
        RunStateMachine();
    }

    public void SetState(GameState state)
    {
        gameState = state;
    }
}
