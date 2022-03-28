using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotManager : MonoBehaviour
{
    [SerializeField] int startingMoney;

    Dealer dealer;

    // Start is called before the first frame update
    void Start()
    {
        dealer = FindObjectOfType<Dealer>();

    }

    public void InitMoney()
    {
        foreach(PokerPlayer player in dealer.activePlayers)
        {
            player.money = startingMoney;
            player.playerPosition.moneyText.text = "$" + player.money.ToString(); 
        }
    }

    public void GiveMoney(PokerPlayer player)
    {

    }
}
