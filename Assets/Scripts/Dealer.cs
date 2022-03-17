using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Dealer : MonoBehaviour
{
    
    [SerializeField] List<string> players;
    
    List<string> activePlayers = new List<string>();
    Dictionary<string, List<int>> playerHands = new Dictionary<string, List<int>>();

    private List<int> deck = Enumerable.Range(0, 52).ToList();

    // Start is called before the first frame update
    void Start()
    {
        DealHands();
        Deal(3);
        Deal();
        Deal();
        PrintHands();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DealHands()
    {
        foreach(string player in players)
        {
            List<int> cards = new List<int>();

            int card1 = Random.Range(0, deck.Count);
            cards.Add(deck[card1]);
            deck.RemoveAt(card1);

            int card2 = Random.Range(0, deck.Count);
            cards.Add(deck[card2]);
            deck.RemoveAt(card2);

            playerHands.Add(player, cards);
        }
    }

    private void Deal(int times = 1)
    {
        int n = 0;
        while (n < times)
        {
            int card = Random.Range(0, deck.Count);

            foreach (string player in players)
            {
                playerHands[player].Add(deck[card]);
            }

            deck.RemoveAt(card);

            n++;
        }
    }

    void PrintHands()
    {
        foreach (KeyValuePair<string, List<int>> entry in playerHands)
        {
            Debug.Log(entry.Key + ": " + "[" + entry.Value[0] + "," + entry.Value[1] + "," + entry.Value[2] + "," + entry.Value[3] + "," + entry.Value[4] + "," + entry.Value[5] + "," + entry.Value[6] + "]");
        }
    }

    


}
