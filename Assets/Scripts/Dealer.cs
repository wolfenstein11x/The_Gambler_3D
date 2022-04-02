using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum HandState { Begin, Flop, Turn, River }

public class Dealer : MonoBehaviour
{
    HandState handState;

    [SerializeField] Sprite[] cardImages;
    [SerializeField] Sprite cardBack;
    [SerializeField] Sprite dealerChip;
    [SerializeField] Transform[] tableCardPositions;
    [SerializeField] PlayerPosition[] playerPositions;
    [SerializeField] PokerPlayer[] NPCs;
    [SerializeField] PokerPlayer mainPlayer;

    ControlHub controlHub;
    WinnerCalculator winnerCalculator;
    PotManager potManager;
    
    public List<PokerPlayer> players = new List<PokerPlayer>();
    private List<PokerPlayer> usedNPCs = new List<PokerPlayer>();
    private List<PokerPlayer> eliminatedPlayers = new List<PokerPlayer>();
    
    private List<int> deck = Enumerable.Range(0, 52).ToList();
    public int dealerIdx;

    // Start is called before the first frame update
    void Start()
    {
        controlHub = FindObjectOfType<ControlHub>();
        winnerCalculator = FindObjectOfType<WinnerCalculator>();
        potManager = FindObjectOfType<PotManager>();
    }

    // NOT BEING USED
    private void NewGame()
    {
        ChoosePlayers();
        SeatPlayers();
        NewHand();

        // give every player their starting money
        //FindObjectOfType<PotManager>().InitMoney();

        // main player starts game off as dealer
        SetDealer(0);

        handState = HandState.Begin;
    }

    public void SetDealer(int idx)
    {
        // make sure no other player displays dealer chip
        foreach(PokerPlayer player in players) { player.playerPosition.dealerChip.GetComponent<SpriteRenderer>().sprite = null; }

        // make selected player display dealer chip and record dealerPos for reference when rotating
        players[idx].playerPosition.dealerChip.GetComponent<SpriteRenderer>().sprite = dealerChip;
        dealerIdx = idx;
    }

    public void RotateDealer()
    {
        do
        {   // increment dealer chip one time
            IncrementDealer();
            
            // keep incrementing the dealer chip if currently on eliminated player 
        } while (players[dealerIdx].eliminated == true);
    }

    private void IncrementDealer()
    {
        if (dealerIdx >= players.Count - 1) { SetDealer(0); }
        else { SetDealer(dealerIdx + 1); }
    }

    public void ResetDeck()
    {
        deck = Enumerable.Range(0, 52).ToList();
    }

    // NOT BEING USED
    public void NewHand()
    {
        // put the cards (numbers) back in the deck (list)
        ResetDeck();

        // move dealer chip to the left (clockwise)
        RotateDealer();

        // clear all images of player cards and table cards
        foreach (Transform tableCardPosition in tableCardPositions) { tableCardPosition.GetComponent<SpriteRenderer>().sprite = null; }
        foreach (PokerPlayer activePlayer in players)
        {
            activePlayer.playerPosition.cardImg1.GetComponent<SpriteRenderer>().sprite = null;
            activePlayer.playerPosition.cardImg2.GetComponent<SpriteRenderer>().sprite = null;
        }
        
        // clear all current hands data
        foreach(PokerPlayer activePlayer in players)
        {
            activePlayer.cards.Clear();
            activePlayer.hand.Clear();
            activePlayer.optimizedHand.Clear();
        }

        // set main state machine to deal mode, and set dealer state machine to deal-new-hands mode
       
        handState = HandState.Begin;



    }

    public void ClearTable()
    {
        // clear all images of table cards
        foreach (Transform tableCardPosition in tableCardPositions) 
        { 
            tableCardPosition.GetComponent<SpriteRenderer>().sprite = null; 
        }
    }

    public void ClearHands()
    {
        foreach (PokerPlayer player in players)
        {
            // clear all current hands data
            player.cards.Clear();
            player.hand.Clear();
            player.optimizedHand.Clear();

            // clear all images of player cards
            player.playerPosition.cardImg1.GetComponent<SpriteRenderer>().sprite = null;
            player.playerPosition.cardImg2.GetComponent<SpriteRenderer>().sprite = null;

            // clear all player fold flags
            player.folded = false;
        }
    }

    public void InitializePlayers()
    {
        ChoosePlayers();
        SeatPlayers();
    }

    private void ChoosePlayers()
    {
        // the main player is always the first one added
        players.Add(mainPlayer);

        // start at i=1, because main player is i=0
        for (int i = 1; i < playerPositions.Length; i++)
        {
            int randomIdx;
            PokerPlayer randomNPC;

            // pick random NPC to fill each position (but keep picking until you don't pick a repeat)
            do
            {
                randomIdx = Random.Range(0, NPCs.Length);
                randomNPC = NPCs[randomIdx];
            } while (usedNPCs.Contains(randomNPC));

            // keep track of NPCs already picked, so you don't duplicate
            usedNPCs.Add(randomNPC);

            // add NPC to list of active players
            players.Add(randomNPC);
        }
    }

    public void SeatPlayers()
    {
        for (int i=0; i < players.Count; i++)
        {
            SeatPlayer(players[i], i);
        }
    }

    private void SeatPlayer(PokerPlayer pokerPlayer, int seat)
    {
        // give player one of the playerPositions
        pokerPlayer.playerPosition = playerPositions[seat];

        // display player headshot in assigned playerPosition
        pokerPlayer.playerPosition.headShot.GetComponent<SpriteRenderer>().sprite = pokerPlayer.headShot;

    }

    public void CombineTables()
    {
        // list of players from current table who aren't eliminated
        List<PokerPlayer> remainingPlayers = new List<PokerPlayer>();
       
        foreach(PokerPlayer player in players)
        {
            if (!player.eliminated) { remainingPlayers.Add(player); }
        }

        // list of NPCs to be added to updated list of players
        List<PokerPlayer> newPlayers = new List<PokerPlayer>();

        // choose players to fill table
        for (int i = remainingPlayers.Count; i < playerPositions.Length; i++)
        {
            int randomIdx;
            PokerPlayer randomNPC;

            // quit if we've run out of unused NPCs
            if (usedNPCs.Count == NPCs.Length){ break; }

            // pick random NPC to fill each position (but keep picking until you don't pick a repeat)
            do
            {
                randomIdx = Random.Range(0, NPCs.Length);
                randomNPC = NPCs[randomIdx];
            } while (usedNPCs.Contains(randomNPC));

            // keep track of NPCs already picked, so you don't duplicate
            usedNPCs.Add(randomNPC);

            // add NPC to list of new players
            newPlayers.Add(randomNPC);
        }

        // randomize money that new players have, between high and low stack of remaining players
        int low = potManager.GetLowStack(remainingPlayers);
        int high = potManager.GetHighStack(remainingPlayers);

        foreach (PokerPlayer player in newPlayers)
        {
            player.money = Random.Range(low, high);
        }

        // reset players list with old and new players
        players.Clear();

        foreach(PokerPlayer player in remainingPlayers) { players.Add(player); }
        foreach(PokerPlayer player in newPlayers) { players.Add(player); }

        // reseat players at new table
        SeatPlayers();

        // make sure all players money is showing (particularly new players)
        foreach(PokerPlayer player in players) { potManager.RefreshPlayerMoney(player); }



    }

    public void EliminatePlayer(PokerPlayer player)
    {
        player.eliminated = true;

        // clear NPC headshot
        player.playerPosition.headShot.GetComponent<SpriteRenderer>().sprite = null;

        // clear player money display
        player.playerPosition.moneyText.text = "";

    }

    public int CountActivePlayers()
    {
        int activePlayerCount = 0;

        foreach(PokerPlayer player in players)
        {
            if (!player.eliminated) { activePlayerCount++; }
        }

        return activePlayerCount;
    }


    // NOT BEING USED
    public void Deal()
    {
        if (controlHub.gameState == GameState.DealHands) 
        { 
            DealToPlayers();

            // hide deal button after it is pressed
            FindObjectOfType<CanvasController>().HideAllCanvases();
        }

        /*
        if (handState == HandState.Begin) { DealToPlayers(); }
        else if (handState == HandState.Flop) { DealFlop(); }
        else if (handState == HandState.Turn) { DealTurn(); }
        else if (handState == HandState.River) { DealRiver(); }
        */
        
    }

    public void DealToPlayers()
    {
        foreach(PokerPlayer player in players)
        {
            // skip over eliminated players
            if (player.eliminated) 
            {
                continue; 
            }

            // deal to main player face-up
            else if (player.tag == "mainPlayer")
            {
                DealToPlayer(player, true);
                DealToPlayer(player, true);
            }

            // deal to NPCs face down
            else
            {
                DealToPlayer(player);
                DealToPlayer(player);
            }
        }
    }

    private void DealToPlayer(PokerPlayer player, bool faceUp=false)
    {
        // select card and store data
        int cardIdx = Random.Range(0, deck.Count);
        int card = deck[cardIdx];

        // add card to player's hand (and to player cards, so we can reveal later if necessary)
        player.hand.Add(cardImages[card].name);
        player.cards.Add(card);
        
        // remove card from deck so not dealt again
        deck.RemoveAt(cardIdx);

        // display image of card in first empty card position (face up only if for main player)
        if (player.playerPosition.cardImg1.GetComponent<SpriteRenderer>().sprite == null)
        {
            if (faceUp) { player.playerPosition.cardImg1.GetComponent<SpriteRenderer>().sprite = cardImages[card]; }
            else { player.playerPosition.cardImg1.GetComponent<SpriteRenderer>().sprite = cardBack; }
        }

        // display image of card in second empty position if first spot not empty (face up only if for main player)
        else if (player.playerPosition.cardImg2.GetComponent<SpriteRenderer>().sprite == null)
        {
            if (faceUp) { player.playerPosition.cardImg2.GetComponent<SpriteRenderer>().sprite = cardImages[card]; }
            else { player.playerPosition.cardImg2.GetComponent<SpriteRenderer>().sprite = cardBack; }
        }
                
    }

    public void DealToTable(int position)
    {
        
        // get random card from deck
        int card1Idx = Random.Range(0, deck.Count);
        int card1 = deck[card1Idx];
       
        // display image of card on table 
        tableCardPositions[position].GetComponent<SpriteRenderer>().sprite = cardImages[card1];

        // store card in each players hand
        foreach (PokerPlayer player in players) 
        {
            // skip over inactive players
            if (player.folded || player.eliminated) { continue; }

            else { player.hand.Add(cardImages[card1].name); } 
        }

        // remove card from deck
        deck.RemoveAt(card1Idx);
    }

    public void DealFlop()
    {
        // deal to first 3 spots on table
        DealToTable(0);
        DealToTable(1);
        DealToTable(2);
    }

    public void DealTurn()
    {
        // deal to 4th spot on table
        DealToTable(3);
    }

    public void DealRiver()
    {
        // deal to 5th spot on table
        DealToTable(4);
    }

    public void Reveal()
    {   
        foreach(PokerPlayer player in players)
        {
            // skip inactive players
            if (player.folded || player.eliminated) { continue; }

            // skip main player because we already see his cards
            if (player.tag == "mainPlayer") { continue; }

            // display image of first card
            player.playerPosition.cardImg1.GetComponent<SpriteRenderer>().sprite = cardImages[player.cards[0]];

            // display image of second card
            player.playerPosition.cardImg2.GetComponent<SpriteRenderer>().sprite = cardImages[player.cards[1]];
        }

        // for debugging only
        //PrintHands();
        
        //List<PokerPlayer> finalists = winnerCalculator.DetermineFinalists(players);
        //winnerCalculator.FindWinners(finalists);

    }
    
    void PrintHands()
    {
        
        foreach(PokerPlayer activePlayer in players)
        {
            PrintHand(activePlayer, FindObjectOfType<HandCalculator>().OptimizeHand(activePlayer.hand));
        }
        
    }

    void PrintHand(PokerPlayer pokerPlayer, List<string> hand)
    {
        int handScore = FindObjectOfType<HandCalculator>().ScoreHand(hand);
        string handName = FindObjectOfType<HandCalculator>().handNames[handScore];

        Debug.Log(pokerPlayer.nickName + ": " + "[" + hand[0] + ", " + hand[1] + ", " + hand[2] + ", " + hand[3] + ", " + hand[4] + "] = " + handName);
    }

    
    


}
