using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerPlayer : MonoBehaviour
{
    public string nickName;
    public string catchPhrase;
    public Sprite headShot;
    public GameObject cardImage1, cardImage2;
    public PlayerPosition playerPosition;

    public List<int> cards;
    public List<string> hand;
    public List<string> optimizedHand;

    HandCalculator handCalculator;

    // Start is called before the first frame update
    void Start()
    {
        handCalculator = FindObjectOfType<HandCalculator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    public void PrintHand()
    {
        int handScore = handCalculator.ScoreHand(hand);
        string handName = handCalculator.handNames[handScore];

        optimizedHand = handCalculator.OptimizeHand(hand);
        
        Debug.Log(gameObject.name + ": [" + optimizedHand[0] + "," + optimizedHand[1] + "," + optimizedHand[2] + "," + optimizedHand[3] + "," + optimizedHand[4] + "]" + " = " + handName);
        
    }
    */
    
}
