using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetTracker : MonoBehaviour
{
    //public int lastBetterIdx;
    public PokerPlayer lastBetter;

    private PotManager potManager;
    private Dealer dealer;
    public int consecutiveNonRaises;

    // Start is called before the first frame update
    void Start()
    {
        potManager = FindObjectOfType<PotManager>();
        dealer = FindObjectOfType<Dealer>();
    }

    public void PlaceBet(PokerPlayer player, int amount)
    {
        potManager.CollectMoneyFromPlayer(player, amount);

        player = lastBetter;

    }

    // initial betting round, where first better is to left of big blind
    public void BetRoundType1()
    {
        lastBetter = dealer.players[potManager.bigBlindIdx + 1];
    }

    // post-flop betting round, where first better is to left of dealer
    public void BetRoundType2()
    {

    }
}
