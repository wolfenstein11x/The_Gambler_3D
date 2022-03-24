using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum HandState { Begin, Flop, Turn, River, Reveal, NewHand }

public class Dealer : MonoBehaviour
{
    HandState state;

    [SerializeField] Sprite[] cardImages;
    [SerializeField] Sprite cardBack;
    [SerializeField] Transform[] tableCardPositions;
    [SerializeField] PlayerPosition[] playerPositions;
    [SerializeField] PokerPlayer[] NPCs;
    [SerializeField] PokerPlayer mainPlayer;
    
    List<PokerPlayer> activePlayers = new List<PokerPlayer>();
    
    private List<int> deck = Enumerable.Range(0, 52).ToList();

    // Start is called before the first frame update
    void Start()
    {
        ChoosePlayers();
        SeatPlayers();
        NewHand();
        
        state = HandState.Begin;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ResetDeck()
    {
        deck = Enumerable.Range(0, 52).ToList();
    }

    private void NewHand()
    {
        // put the cards (numbers) back in the deck (list)
        ResetDeck();

        // clear all images of player cards and table cards
        foreach (Transform tableCardPosition in tableCardPositions) { tableCardPosition.GetComponent<SpriteRenderer>().sprite = null; }
        foreach (PlayerPosition playerPosition in playerPositions)
        {
            CardImg1 cardImg1 = playerPosition.GetComponentInChildren<CardImg1>();
            cardImg1.GetComponent<SpriteRenderer>().sprite = null;

            CardImg2 cardImg2 = playerPosition.GetComponentInChildren<CardImg2>();
            cardImg2.GetComponent<SpriteRenderer>().sprite = null;
        }

        // clear all current hands data
        foreach(PokerPlayer activePlayer in activePlayers)
        {
            activePlayer.cards.Clear();
            activePlayer.hand.Clear();
        }

        // dealer ready to deal hands next time deal button pressed
        state = HandState.Begin;
    }

    private void InitializeNPCs()
    {
        List<int> usedNPCs = new List<int>();

        // start at i=1, because main player is i=0
        for (int i=1; i < playerPositions.Length; i++)
        {
            int npcIdx;
            
            // pick random NPC to fill each position (but keep picking until you don't pick a repeat)
            do
            {
                npcIdx = Random.Range(0, NPCs.Length);
            } while (usedNPCs.Contains(npcIdx));
            
            // keep track of NPCs already picked, so you don't duplicate
            usedNPCs.Add(npcIdx);
            
            // put NPC headshot in position
            Headshot headshot = playerPositions[i].GetComponentInChildren<Headshot>();
            headshot.GetComponent<SpriteRenderer>().sprite = NPCs[npcIdx].headShot;

            // add NPC to list of active players
            activePlayers.Add(NPCs[npcIdx]);
        }
    }

    private void ChoosePlayers()
    {
        // the main player is always the first one added
        activePlayers.Add(mainPlayer);

        // choose random NPCs
        List<int> usedNPCs = new List<int>();

        // start at i=1, because main player is i=0
        for (int i = 1; i < playerPositions.Length; i++)
        {
            int npcIdx;

            // pick random NPC to fill each position (but keep picking until you don't pick a repeat)
            do
            {
                npcIdx = Random.Range(0, NPCs.Length);
            } while (usedNPCs.Contains(npcIdx));

            // keep track of NPCs already picked, so you don't duplicate
            usedNPCs.Add(npcIdx);

            // add NPC to list of active players
            activePlayers.Add(NPCs[npcIdx]);
        }
    }

    public void SeatPlayers()
    {
        for (int i=0; i < playerPositions.Length; i++)
        {
            SeatPlayer(activePlayers[i], i);
        }
    }

    private void SeatPlayer(PokerPlayer pokerPlayer, int seat)
    {
        // give player one of the playerPositions
        pokerPlayer.playerPosition = playerPositions[seat];

        // display player headshot in assigned playerPosition
        Headshot headshot = pokerPlayer.playerPosition.GetComponentInChildren<Headshot>();
        headshot.GetComponent<SpriteRenderer>().sprite = pokerPlayer.headShot;
    }

    private void InitializeActivePlayers()
    {
        // make the main player the first in the list of active players and display headshot
        activePlayers.Add(mainPlayer);
        Headshot headshot = playerPositions[0].GetComponentInChildren<Headshot>();
        headshot.GetComponent<SpriteRenderer>().sprite = mainPlayer.headShot;

        // add each NPC to list of active players and display their images
        //InitializeNPCs();
    }

    public void Deal()
    {
        if (state == HandState.Begin) { DealToPlayers(); }
        else if (state == HandState.Flop) { DealFlop(); }
        else if (state == HandState.Turn) { DealTurn(); }
        else if (state == HandState.River) { DealRiver(); }
        else if (state == HandState.Reveal) { Reveal(); }
        else if (state == HandState.NewHand) { NewHand(); }
    }

    private void DealToPlayers()
    {
        // deal main player face up
        DealToPlayer(activePlayers[0], true);
        DealToPlayer(activePlayers[0], true);

        // deal to NPCs face down
        for (int i=1; i < activePlayers.Count; i++)
        {
            DealToPlayer(activePlayers[i]);
            DealToPlayer(activePlayers[i]);
        }

        // move state machine to next state
        state = HandState.Flop;
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
            CardImg1 cardImg1 = player.playerPosition.GetComponentInChildren<CardImg1>();
            if (faceUp) { cardImg1.GetComponent<SpriteRenderer>().sprite = cardImages[card]; }
            else { cardImg1.GetComponent<SpriteRenderer>().sprite = cardBack; }
        }

        // display image of card in second empty position if first spot not empty (face up only if for main player)
        else if (player.playerPosition.cardImg2.GetComponent<SpriteRenderer>().sprite == null)
        {
            CardImg2 cardImg2 = player.playerPosition.GetComponentInChildren<CardImg2>();
            if (faceUp) { cardImg2.GetComponent<SpriteRenderer>().sprite = cardImages[card]; }
            else { cardImg2.GetComponent<SpriteRenderer>().sprite = cardBack; }
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
        foreach (PokerPlayer activePlayer in activePlayers) { activePlayer.hand.Add(cardImages[card1].name); }

        // remove card from deck
        deck.RemoveAt(card1Idx);
    }

    private void DealHands()
    {
        // first deal player hand cards face up
        DealPlayerHand();

        // deal opponent cards face down (start at i=1 because main player is at i=0)
        for (int i= 1; i < playerPositions.Length; i++)
        {
            // select card and store data
            int card1 = deck[Random.Range(0, deck.Count)];
            activePlayers[i].cards.Add(card1);
            activePlayers[i].hand.Add(cardImages[card1].name);
            deck.RemoveAt(card1);

            // display image of (back of) card
            CardImg1 cardImg1 = playerPositions[i].GetComponentInChildren<CardImg1>();
            cardImg1.GetComponent<SpriteRenderer>().sprite = cardBack;

            // select card and store data
            int card2 = deck[Random.Range(0, deck.Count)];
            activePlayers[i].cards.Add(card2);
            activePlayers[i].hand.Add(cardImages[card2].name);
            deck.RemoveAt(card2);

            // display image of (back of) card
            CardImg2 cardImg2 = playerPositions[i].GetComponentInChildren<CardImg2>();
            cardImg2.GetComponent<SpriteRenderer>().sprite = cardBack;

        }

        // move state machine to next state
        state = HandState.Flop;
    }

    private void DealPlayerHand()
    {
        // select card and store data
        int card1 = deck[Random.Range(0, deck.Count)];
        mainPlayer.cards.Add(card1);
        mainPlayer.hand.Add(cardImages[card1].name);
        deck.RemoveAt(card1);

        // display image of card
        CardImg1 cardImg1 = playerPositions[0].GetComponentInChildren<CardImg1>();
        cardImg1.GetComponent<SpriteRenderer>().sprite = cardImages[card1];

        // select card and store data
        int card2 = deck[Random.Range(0, deck.Count)];
        mainPlayer.cards.Add(card2);
        mainPlayer.hand.Add(cardImages[card2].name);
        deck.RemoveAt(card2);

        // display image of card
        CardImg2 cardImg2 = playerPositions[0].GetComponentInChildren<CardImg2>();
        cardImg2.GetComponent<SpriteRenderer>().sprite = cardImages[card2];
    }

    private void DealFlop()
    {
        // deal to first 3 spots on table
        DealToTable(0);
        DealToTable(1);
        DealToTable(2);

        // move state machine to next state
        state = HandState.Turn;
    }

    private void DealTurn()
    {
        // deal to 4th spot on table
        DealToTable(3);

        // move state machine to next state
        state = HandState.River;
    }

    private void DealRiver()
    {
        // deal to 5th spot on table
        DealToTable(4);

        // move state machine to next state
        state = HandState.Reveal;
    }

    private void Reveal()
    {   /*
        // TODO: skip over inactive players
        for (int i=1; i < playerPositions.Length; i++)
        {
            // display image of first card
            CardImg1 cardImg1 = playerPositions[i].GetComponentInChildren<CardImg1>();
            cardImg1.GetComponent<SpriteRenderer>().sprite = cardImages[activePlayers[i].cards[0]];

            // display image of second card
            CardImg2 cardImg2 = playerPositions[i].GetComponentInChildren<CardImg2>();
            cardImg2.GetComponent<SpriteRenderer>().sprite = cardImages[activePlayers[i].cards[1]];
        }
        */

        foreach(PokerPlayer player in activePlayers)
        {
            // skip main player because we already see his cards
            if (player.tag == "mainPlayer") { continue; }

            // display image of first card
            CardImg1 cardImg1 = player.playerPosition.GetComponentInChildren<CardImg1>();
            cardImg1.GetComponent<SpriteRenderer>().sprite = cardImages[player.cards[0]];

            // display image of second card
            CardImg2 cardImg2 = player.playerPosition.GetComponentInChildren<CardImg2>();
            cardImg2.GetComponent<SpriteRenderer>().sprite = cardImages[player.cards[1]];
        }

        // for debugging only
        PrintHands();

        // move state machine to next state
        state = HandState.NewHand;

    }
    
    void PrintHands()
    {
        foreach(PokerPlayer activePlayer in activePlayers)
        {
            activePlayer.PrintHand();
        }
    }

    public void TestDeck()
    {
        int card = Random.Range(0, deck.Count);
        Debug.Log(cardImages[deck[card]].name);
        deck.RemoveAt(card);
    }
    
    


}
