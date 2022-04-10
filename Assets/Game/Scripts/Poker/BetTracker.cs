using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetTracker : MonoBehaviour
{
    public int betStarterIdx;
    public int currentBetterIdx;
    //public PokerPlayer betStarter;
    //public PokerPlayer currentBetter;



    private Dealer dealer;
    private CanvasController canvasController;
    private PotManager potManager;
    private ControlHub controlHub;
   

   

    // Start is called before the first frame update
    void Start()
    {
        dealer = FindObjectOfType<Dealer>();
        canvasController = FindObjectOfType<CanvasController>();
        potManager = FindObjectOfType<PotManager>();
        controlHub = FindObjectOfType<ControlHub>();

    }


   

    public void DetermineBetStarterPreFlop(List<PokerPlayer> players)
    {
        // set bet starter to left of big blind, adjusting for wrap-around
        betStarterIdx = potManager.bigBlindIdx + 1;
        
        if (betStarterIdx >= players.Count) { betStarterIdx -= players.Count; }

        // keep rotating bet starter if it lands on inactive player
        while(players[betStarterIdx].folded || players[betStarterIdx].eliminated)
        {
            betStarterIdx++;
            if (betStarterIdx >= players.Count) { betStarterIdx -= players.Count; }
        }

        currentBetterIdx = betStarterIdx;
        //Debug.Log("betStarterIdx: " + betStarterIdx);
    }

    public void DetermineBetStarterPostFlop(List<PokerPlayer> players)
    {
        // set bet starter to left of dealer, adjusting for wrap-around
        betStarterIdx = dealer.dealerIdx + 1;
        
        if (betStarterIdx >= players.Count) { betStarterIdx -= players.Count; }

        // keep rotating bet starter if it lands on inactive player
        while (players[betStarterIdx].folded || players[betStarterIdx].eliminated)
        {
            betStarterIdx++;
            if (betStarterIdx >= players.Count) { betStarterIdx -= players.Count; }
        }

        currentBetterIdx = betStarterIdx;
        
    }

    public void IncrementCurrentBetter()
    {
        // increment bet position, adjusting for wrap-around
        currentBetterIdx++;
        if (currentBetterIdx >= dealer.players.Count) { currentBetterIdx -= dealer.players.Count; }

        // stop if back at betStarter, even if betStarter is folded
        if (currentBetterIdx == betStarterIdx) { return; }

        // otherwise, keep rotating bet position if it lands on inactive player
        while (dealer.players[currentBetterIdx].folded || dealer.players[currentBetterIdx].eliminated)
        {
            currentBetterIdx++;
            if (currentBetterIdx >= dealer.players.Count) { currentBetterIdx -= dealer.players.Count; }
        }
    }

    

    

}
