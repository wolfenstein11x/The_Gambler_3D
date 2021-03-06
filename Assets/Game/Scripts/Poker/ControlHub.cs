using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Init, DealHands, BetRound1, BetRound1Done, BetRound2, BetRound2Done, BetRound3, BetRound3Done, BetRound4, BetRound4Done, NPCoption, PlayerOption, DealFlop, DealTurn, DealRiver, PrematureWin, Reveal, NewHand, }

public class ControlHub : MonoBehaviour
{
    [SerializeField] int maxTableCombineCount = 2;

    public GameState gameState;
    public GameState prevState;

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


    public void RunStateMachine()
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

            // reset all current bet amounts to zero
            potManager.highestBet = 0;
            potManager.ClearBets(dealer.players);

            // show deal button, which will move us to DealHands state when pressed
            canvasController.HandleDeal();
        }

        else if (gameState == GameState.DealHands)
        {
            // deal cards to all players
            dealer.DealToPlayers();

            // collect blinds
            potManager.CollectBlinds(dealer.players);
            
            // set bet starter to first active player left of big blind
            betTracker.DetermineBetStarterPreFlop(dealer.players);
            
            // move state machine to start betting round
            gameState = GameState.BetRound1;
            RunStateMachine();
        }

        else if (gameState == GameState.BetRound1)
        {
   
            // go to PlayerOption if bet is to main player
            if (dealer.players[betTracker.currentBetterIdx].tag == "mainPlayer")
            {
                gameState = GameState.PlayerOption;

                // save state so we can come back here when needed
                prevState = GameState.BetRound1;

                RunStateMachine();
            }

            // go to NPCoption if bet is to NPC
            else
            {
                gameState = GameState.NPCoption;

                // save state so we can come back here when needed
                prevState = GameState.BetRound1;

                RunStateMachine();
            }
        }

        else if (gameState == GameState.BetRound2)
        {
            
            // go to PlayerOption if bet is to main player
            if (dealer.players[betTracker.currentBetterIdx].tag == "mainPlayer")
            {
                gameState = GameState.PlayerOption;

                // save state so we can come back here when needed
                prevState = GameState.BetRound2;

                RunStateMachine();
            }

            // go to NPCoption if bet is to NPC
            else
            {
                gameState = GameState.NPCoption;

                // save state so we can come back here when needed
                prevState = GameState.BetRound2;

                RunStateMachine();
            }
        }

        else if (gameState == GameState.BetRound3)
        {

            // go to PlayerOption if bet is to main player
            if (dealer.players[betTracker.currentBetterIdx].tag == "mainPlayer")
            {
                gameState = GameState.PlayerOption;

                // save state so we can come back here when needed
                prevState = GameState.BetRound3;

                RunStateMachine();
            }

            // go to NPCoption if bet is to NPC
            else
            {
                gameState = GameState.NPCoption;

                // save state so we can come back here when needed
                prevState = GameState.BetRound3;

                RunStateMachine();
            }
        }

        else if (gameState == GameState.BetRound4)
        {

            // go to PlayerOption if bet is to main player
            if (dealer.players[betTracker.currentBetterIdx].tag == "mainPlayer")
            {
                gameState = GameState.PlayerOption;

                // save state so we can come back here when needed
                prevState = GameState.BetRound4;

                RunStateMachine();
            }

            // go to NPCoption if bet is to NPC
            else
            {
                gameState = GameState.NPCoption;

                // save state so we can come back here when needed
                prevState = GameState.BetRound4;

                RunStateMachine();
            }
        }

        else if (gameState == GameState.BetRound1Done)
        {
            // show deal button, which will move us to correct deal state based on current state when pressed
            canvasController.HandleDeal();
        }

        else if (gameState == GameState.BetRound2Done)
        {
            // show deal button, which will move us to correct deal state based on current state when pressed
            canvasController.HandleDeal();   
        }

        else if (gameState == GameState.BetRound3Done)
        {
            // show deal button, which will move us to correct deal state based on current state when pressed
            canvasController.HandleDeal();
            
        }

        else if (gameState == GameState.BetRound4Done)
        {
            // show reveal button, which will move us to reveal state based on current state when pressed
            canvasController.HandleReveal();
        }

        else if (gameState == GameState.PlayerOption)
        {
            // Check if toPlay amount is 0 or not
            if (dealer.players[betTracker.currentBetterIdx].currentBet < potManager.highestBet)
            {
                dealer.players[betTracker.currentBetterIdx].CallRaiseOrFold();
            }

            else
            {
                dealer.players[betTracker.currentBetterIdx].CheckOrRaise();
            }
        }

        else if (gameState == GameState.NPCoption)
        {
            // Check if toPlay amount is 0 or not
            if (dealer.players[betTracker.currentBetterIdx].currentBet < potManager.highestBet)
            {
                dealer.players[betTracker.currentBetterIdx].GetComponent<PokerAI>().CallRaiseOrFold();
            }

            else
            {
                dealer.players[betTracker.currentBetterIdx].GetComponent<PokerAI>().CheckOrRaise();
            }
  
        }

        else if (gameState == GameState.DealFlop)
        {
            // deal flop
            dealer.DealFlop();

            // set bet starter to first active player left of dealer
            betTracker.DetermineBetStarterPostFlop(dealer.players);

            // reset all current bet amounts to zero
            potManager.highestBet = 0;
            potManager.ClearBets(dealer.players);

            // move state machine to start betting round
            gameState = GameState.BetRound2;
            RunStateMachine();
        }

        else if (gameState == GameState.DealTurn)
        {
            // deal turn
            dealer.DealTurn();

            // set bet starter to first active player left of dealer
            betTracker.DetermineBetStarterPostFlop(dealer.players);

            // reset all current bet amounts to zero
            potManager.highestBet = 0;
            potManager.ClearBets(dealer.players);

            // move state machine to start betting round
            gameState = GameState.BetRound3;
            RunStateMachine();
        }

        else if (gameState == GameState.DealRiver)
        {
            // deal turn
            dealer.DealRiver();

            // set bet starter to first active player left of dealer
            betTracker.DetermineBetStarterPostFlop(dealer.players);

            // reset all current bet amounts to zero
            potManager.highestBet = 0;
            potManager.ClearBets(dealer.players);

            // move state machine to start betting round
            gameState = GameState.BetRound4;
            RunStateMachine();
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

        else if (gameState == GameState.PrematureWin)
        {
            // only player left is the winner
            winnerCalculator.DeterminePrematureWinner(dealer.players);

            // give winner all the money in the pot
            potManager.DistributeWinnings();

            // display hand won premature canvas
            canvasController.HandleHandWonPremature();

            // show NewHand button, which will move us to NewHand state when pressed
            canvasController.HandleNewHand();
        }

        else if (gameState == GameState.NewHand)
        {
            // clear cards from table
            dealer.ClearTable();

            // clear player hands (clears fold flags too)
            dealer.ClearHands();

            // reset the deck
            dealer.ResetDeck();

            // reset all current bet amounts to zero
            potManager.highestBet = 0;
            potManager.ClearBets(dealer.players);

            // eliminate players with $0
            foreach (PokerPlayer player in dealer.players)
            {
                if (player.money <= 0) { dealer.EliminatePlayer(player); }
            }

            // game is over if main player is eliminated
            if (dealer.players[0].eliminated)
            {
                canvasController.ShowCanvas(canvasController.gameOverCanvas);
            }

            // check if main player won match
            else if (winnerCalculator.CheckForMatchWonByMainPlayer(dealer.players))
            {
                canvasController.ShowCanvas(canvasController.matchWonCanvas);
            }

            // combine tables and raise blinds if down to 3 players (and haven't already combined tables twice)
            else if (dealer.CountActivePlayers() <= 3 && tableCombineCount < maxTableCombineCount)
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




    // buttons

    public void DealButton()
    {
        // hide button after it is pressed
        canvasController.HideAllCanvases();

        // move to particular deal state depending on current state
        if (gameState == GameState.Init)           { gameState = GameState.DealHands; }
        else if (gameState == GameState.BetRound1Done) { gameState = GameState.DealFlop; }
        else if (gameState == GameState.BetRound2Done)  { gameState = GameState.DealTurn; }
        else if (gameState == GameState.BetRound3Done)  { gameState = GameState.DealRiver; }
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

    public int ParseRaiseAmountText()
    {
        string numericString = string.Empty;

        // filter number characters (grab everything but '$' sign)
        foreach(char c in canvasController.raiseAmountText.text)
        {
            if (c >= '0' && c <= '9')
            {
                numericString = string.Concat(numericString, c.ToString());
            }
        }

        if (int.TryParse(numericString, out int raiseAmount))
        {
            return raiseAmount;  
        }

        else return 0;

    }

    public void UpArrowButton()
    {
        // get integer value from string in UI
        int raiseAmount = ParseRaiseAmountText();

        // toggle up in increments of big blind
        raiseAmount += potManager.bigBlind;

        // don't go higher than highest raise player can afford (which would be player's current bet + player's remaining money)
        int highestPossibleRaise = dealer.players[betTracker.currentBetterIdx].currentBet + dealer.players[betTracker.currentBetterIdx].money;
        if (raiseAmount > highestPossibleRaise) { raiseAmount -= potManager.bigBlind; }

        // update UI with new raise amount
        canvasController.raiseAmountText.text = "$" + raiseAmount.ToString();
    }

    public void DownArrowButton()
    {
        // get integer value from string in UI
        int raiseAmount = ParseRaiseAmountText();

        // toggle down in decrements of big blind
        raiseAmount -= potManager.bigBlind;

        // don't go below minimum raise amount
        int minRaiseAmount = potManager.highestBet + potManager.bigBlind;
        if (raiseAmount < minRaiseAmount) { raiseAmount = minRaiseAmount; }

        // update UI with new raise amount
        canvasController.raiseAmountText.text = "$" + raiseAmount.ToString();
    }
}
