using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum HandState { Begin, Flop, Turn, River, Reveal }

public class Dealer : MonoBehaviour
{
    HandState state;

    [SerializeField] List<string> opponents;
    [SerializeField] Sprite[] cardImages;
    [SerializeField] Transform[] playerCardPositions;
    [SerializeField] Transform[] tableCardPositions;
    
    private GameObject playerCard1, playerCard2;

    List<string> activePlayers = new List<string>();
    Dictionary<string, List<int>> playerHands = new Dictionary<string, List<int>>();

    private List<int> deck = Enumerable.Range(0, 52).ToList();

    // Start is called before the first frame update
    void Start()
    {
        state = HandState.Begin;

        InitializeActivePlayers();
        
        //PrintHands();
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

        // clear all player cards and table cards
        foreach (Transform playerCard in playerCardPositions) { playerCard.GetComponent<SpriteRenderer>().sprite = null; }
        foreach (Transform tableCard in tableCardPositions) { tableCard.GetComponent<SpriteRenderer>().sprite = null; }

        // clear all current hands from dictionary
        playerHands.Clear();

        // dealer ready to deal hands next time deal button pressed
        state = HandState.Begin;
    }

    
    private void InitializeActivePlayers()
    {
        activePlayers.Add("Player");

        foreach(string opponent in opponents)
        {
            activePlayers.Add(opponent);
        }
    }

    public void Deal()
    {
        if (state == HandState.Begin) { DealHands(); }
        else if (state == HandState.Flop) { DealFlop(); }
        else if (state == HandState.Turn) { DealTurn(); }
        else if (state == HandState.River) { DealRiver(); }
        else if (state == HandState.Reveal) { NewHand(); }
    }

    private void DealHands()
    {
        DealPlayerHand();
        
        foreach(string opponent in opponents)
        {
            List<int> cards = new List<int>();

            int card1 = Random.Range(0, deck.Count);
            cards.Add(deck[card1]);
            deck.RemoveAt(card1);

            int card2 = Random.Range(0, deck.Count);
            cards.Add(deck[card2]);
            deck.RemoveAt(card2);

            playerHands.Add(opponent, cards);
        }

        // move state machine to next state
        state = HandState.Flop;
    }

    private void DealPlayerHand()
    {
        List<int> cards = new List<int>();

        int card1 = Random.Range(0, deck.Count);
        cards.Add(deck[card1]);
        playerCardPositions[0].GetComponent<SpriteRenderer>().sprite = cardImages[card1];
        deck.RemoveAt(card1);

        int card2 = Random.Range(0, deck.Count);
        cards.Add(deck[card2]);
        playerCardPositions[1].GetComponent<SpriteRenderer>().sprite = cardImages[card2];
        deck.RemoveAt(card2);

        playerHands.Add("Player", cards);
    }

    private void DealFlop()
    {
        // get random card from deck, display image, store card in each players hand, then remove from deck
        int card1 = Random.Range(0, deck.Count);
        tableCardPositions[0].GetComponent<SpriteRenderer>().sprite = cardImages[card1];
        foreach (string player in activePlayers) { playerHands[player].Add(deck[card1]); }
        deck.RemoveAt(card1);

        // get random card from deck, display image, store card in each players hand, then remove from deck
        int card2 = Random.Range(0, deck.Count);
        tableCardPositions[1].GetComponent<SpriteRenderer>().sprite = cardImages[card2];
        foreach (string player in activePlayers) { playerHands[player].Add(card2); }
        deck.RemoveAt(card2);

        // get random card from deck, display image, store card in each players hand, then remove from deck
        int card3 = Random.Range(0, deck.Count);
        tableCardPositions[2].GetComponent<SpriteRenderer>().sprite = cardImages[card3];
        foreach (string player in activePlayers) { playerHands[player].Add(card3); }
        deck.RemoveAt(card3);

        // move state machine to next state
        state = HandState.Turn;
    }

    private void DealTurn()
    {
        // get random card from deck, display image, store card in each players hand, then remove from deck
        int card = Random.Range(0, deck.Count);
        tableCardPositions[3].GetComponent<SpriteRenderer>().sprite = cardImages[card];
        foreach (string player in activePlayers) { playerHands[player].Add(deck[card]); }
        deck.RemoveAt(card);

        // move state machine to next state
        state = HandState.River;
    }

    private void DealRiver()
    {
        // get random card from deck, display image, store card in each players hand, then remove from deck
        int card = Random.Range(0, deck.Count);
        tableCardPositions[4].GetComponent<SpriteRenderer>().sprite = cardImages[card];
        foreach (string player in activePlayers) { playerHands[player].Add(deck[card]); }
        deck.RemoveAt(card);

        // move state machine to next state
        state = HandState.Reveal;
    }

    

    void PrintHands()
    {
        foreach (KeyValuePair<string, List<int>> entry in playerHands)
        {
            Debug.Log(entry.Key + ": " + "[" + entry.Value[0] + "," + entry.Value[1] + "," + entry.Value[2] + "," + entry.Value[3] + "," + entry.Value[4] + "," + entry.Value[5] + "," + entry.Value[6] + "]");
        }
    }

    


}
