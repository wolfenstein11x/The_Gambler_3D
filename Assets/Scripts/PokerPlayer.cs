using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerPlayer : MonoBehaviour
{
    public Sprite headShot;
    public GameObject cardImage1, cardImage2;
    public PlayerPosition playerPosition;

    public List<int> cards;
    public List<string> hand;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintHand()
    {
        hand = FindObjectOfType<HandCalculator>().SortHandHighLow(hand);

        int handScore = FindObjectOfType<HandCalculator>().CheckHand(hand);

        Debug.Log(gameObject.name + ": [" + hand[0] + "," + hand[1] + "," + hand[2] + "," + hand[3] + "," + hand[4] + "," + hand[5] + "," + hand[6] + "]" + " = " + handScore);
    }

    
}
