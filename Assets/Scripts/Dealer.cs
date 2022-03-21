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
    [SerializeField] Transform[] playerPositions;
    [SerializeField] PokerPlayer[] NPCs;
    [SerializeField] PokerPlayer player;
    
    List<PokerPlayer> activePlayers = new List<PokerPlayer>();
    
    private List<int> deck = Enumerable.Range(0, 52).ToList();

    // Start is called before the first frame update
    void Start()
    {
        InitializeActivePlayers();
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
        foreach (Transform playerPosition in playerPositions)
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

    
    private void InitializeActivePlayers()
    {
        // make the main player the first in the list of active players and display headshot
        activePlayers.Add(player);
        Headshot headshot = playerPositions[0].GetComponentInChildren<Headshot>();
        headshot.GetComponent<SpriteRenderer>().sprite = player.headShot;

        // add each NPC to list of active players and display their images
        InitializeNPCs();
    }

    public void Deal()
    {
        if (state == HandState.Begin) { DealHands(); }
        else if (state == HandState.Flop) { DealFlop(); }
        else if (state == HandState.Turn) { DealTurn(); }
        else if (state == HandState.River) { DealRiver(); }
        else if (state == HandState.Reveal) { Reveal(); }
        else if (state == HandState.NewHand) { NewHand(); }
    }

    private void DealHands()
    {
        // first deal player hand cards face up
        DealPlayerHand();

        // deal opponent cards face down (start at i=1 because main player is at i=0)
        for (int i= 1; i < playerPositions.Length; i++)
        {
            // select card and store data
            int card1 = Random.Range(0, deck.Count);
            activePlayers[i].cards.Add(card1);
            activePlayers[i].hand.Add(cardImages[card1].name);
            deck.RemoveAt(card1);

            // display image of (back of) card
            CardImg1 cardImg1 = playerPositions[i].GetComponentInChildren<CardImg1>();
            cardImg1.GetComponent<SpriteRenderer>().sprite = cardBack;

            // select card and store data
            int card2 = Random.Range(0, deck.Count);
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
        int card1 = Random.Range(0, deck.Count);
        player.cards.Add(card1);
        player.hand.Add(cardImages[card1].name);
        deck.RemoveAt(card1);

        // display image of card
        CardImg1 cardImg1 = playerPositions[0].GetComponentInChildren<CardImg1>();
        cardImg1.GetComponent<SpriteRenderer>().sprite = cardImages[card1];

        // select card and store data
        int card2 = Random.Range(0, deck.Count);
        player.cards.Add(card2);
        player.hand.Add(cardImages[card2].name);
        deck.RemoveAt(card2);

        // display image of card
        CardImg2 cardImg2 = playerPositions[0].GetComponentInChildren<CardImg2>();
        cardImg2.GetComponent<SpriteRenderer>().sprite = cardImages[card2];
    }

    private void DealFlop()
    {
        // get random card from deck, display image, store card in each players hand, then remove from deck
        int card1 = Random.Range(0, deck.Count);
        tableCardPositions[0].GetComponent<SpriteRenderer>().sprite = cardImages[card1];
        foreach (PokerPlayer activePlayer in activePlayers) { activePlayer.hand.Add(cardImages[card1].name); }
        deck.RemoveAt(card1);

        // get random card from deck, display image, store card in each players hand, then remove from deck
        int card2 = Random.Range(0, deck.Count);
        tableCardPositions[1].GetComponent<SpriteRenderer>().sprite = cardImages[card2];
        foreach (PokerPlayer activePlayer in activePlayers) { activePlayer.hand.Add(cardImages[card2].name); }
        deck.RemoveAt(card2);

        // get random card from deck, display image, store card in each players hand, then remove from deck
        int card3 = Random.Range(0, deck.Count);
        tableCardPositions[2].GetComponent<SpriteRenderer>().sprite = cardImages[card3];
        foreach (PokerPlayer activePlayer in activePlayers) { activePlayer.hand.Add(cardImages[card3].name); }
        deck.RemoveAt(card3);

        // move state machine to next state
        state = HandState.Turn;
    }

    private void DealTurn()
    {
        // get random card from deck, display image, store card in each players hand, then remove from deck
        int card = Random.Range(0, deck.Count);
        tableCardPositions[3].GetComponent<SpriteRenderer>().sprite = cardImages[card];
        foreach (PokerPlayer activePlayer in activePlayers) { activePlayer.hand.Add(cardImages[card].name); }
        deck.RemoveAt(card);

        // move state machine to next state
        state = HandState.River;
    }

    private void DealRiver()
    {
        // get random card from deck, display image, store card in each players hand, then remove from deck
        int card = Random.Range(0, deck.Count);
        tableCardPositions[4].GetComponent<SpriteRenderer>().sprite = cardImages[card];
        foreach (PokerPlayer activePlayer in activePlayers) { activePlayer.hand.Add(cardImages[card].name); }
        deck.RemoveAt(card);

        // move state machine to next state
        state = HandState.Reveal;
    }

    private void Reveal()
    {
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
    
    


}
