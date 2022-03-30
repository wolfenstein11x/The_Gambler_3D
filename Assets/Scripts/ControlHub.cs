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

    private int tableCombineCount;

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
        tableCombineCount = 0;

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

            // make sure players don't have any cards
            dealer.ClearHands();
            
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

            // determine hand winner(s)
            List<PokerPlayer> finalists = winnerCalculator.DetermineFinalists(dealer.players);
            winnerCalculator.FindWinners(finalists);

            // distribute money in pot to winner(s)
            potManager.DistributeWinnings();

            // display hand won canvas
            canvasController.HandleHandWon();

            // show NewHand button, which will move us to NewHand state when pressed
            canvasController.HandleNewHand();
        }

        else if (gameState == GameState.NewHand)
        {
            // clear cards from table
            dealer.ClearTable();

            // clear player hands
            dealer.ClearHands();

            // reset the deck
            dealer.ResetDeck();

            // eliminate players with $0
            foreach(PokerPlayer player in dealer.players)
            {
                if (player.money <= 0) { dealer.EliminatePlayer(player); }
            }

            // game is over if main player is eliminated

            // combine tables and raise blinds if down to 3 players (and haven't already combined tables twice)
            if (dealer.CountActivePlayers() <= 3 && tableCombineCount < 2)
            {
                canvasController.ShowRaiseBlindsCanvas();
                potManager.RaiseBlinds();
                Invoke("InitiateCombineTables", 2);
            }

            else
            {
                // increment dealer chip
                dealer.RotateDealer();

                // show deal button, which will move us to DealHands state when pressed
                canvasController.HandleDeal();
            }
        }

    }

    public void DealButton()
    {
        // hide button after it is pressed
        canvasController.HideAllCanvases();

        // move to particular deal state depending on current state
        if (gameState == GameState.Init)           { gameState = GameState.DealHands; }
        else if (gameState == GameState.DealHands) { gameState = GameState.DealFlop; }
        else if (gameState == GameState.DealFlop)  { gameState = GameState.DealTurn; }
        else if (gameState == GameState.DealTurn)  { gameState = GameState.DealRiver; }
        else if (gameState == GameState.NewHand)   { gameState = GameState.DealHands; }

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

    public void NewHandButton()
    {
        // hide button after it is pressed
        canvasController.HideAllCanvases();

        // move to NewHand state
        gameState = GameState.NewHand;

        // run the state machine now that it is in updated state
        RunStateMachine();
    }

    public void SetState(GameState state)
    {
        gameState = state;
    }

    private void InitiateCombineTables()
    {
        dealer.CombineTables();
        tableCombineCount++;

        // increment dealer chip
        dealer.RotateDealer();

        // show deal button, which will move us to DealHands state when pressed
        canvasController.HandleDeal();
    }
}
