using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetTracker : MonoBehaviour
{
    //public int lastBetterIdx;
    public PokerPlayer lastBetter;

    private PotManager potManager;

    // Start is called before the first frame update
    void Start()
    {
        potManager = FindObjectOfType<PotManager>();
    }

    public void PlaceBet(PokerPlayer player, int amount)
    {
        potManager.CollectMoneyFromPlayer(player, amount);

        player = lastBetter;

    }
}
