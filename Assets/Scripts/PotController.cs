using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotController : MonoBehaviour
{
    [SerializeField] int startingMoney;

    public int smallBlind = 1;
    public int bigBlind = 2;

    public Text potMoneyText;
    public int potMoney;

    public int bigBlindIdx;
    public int smallBlindIdx;

    Dealer dealer;
    WinnerCalculator winnerCalculator;

    // Start is called before the first frame update
    void Start()
    {
        dealer = FindObjectOfType<Dealer>();
        winnerCalculator = FindObjectOfType<WinnerCalculator>();
    }

    public void Test(List<PokerPlayer> players)
    {
        Debug.Log(players[0].nickName);
    }

    public void InitMoney()
    {
        foreach (PokerPlayer player in dealer.players)
        {
            player.money = startingMoney;
            player.playerPosition.moneyText.text = "$" + player.money.ToString();
        }

        ResetPot();
    }

    private void ResetPot()
    {
        potMoney = 0;
        potMoneyText.text = "$" + potMoney.ToString();
    }

    public void AddToPot(int amount)
    {
        potMoney += amount;
        potMoneyText.text = potMoneyText.text = "$" + potMoney.ToString();
    }

    public void CollectMoneyFromPlayer(PokerPlayer player, int amount)
    {
        AddToPot(amount);

        player.money -= amount;
        player.playerPosition.moneyText.text = "$" + player.money.ToString();
    }

    public void GiveMoneyToPlayer(PokerPlayer player, int amount)
    {
        AddToPot(-1 * amount);

        player.money += amount;
        player.playerPosition.moneyText.text = "$" + player.money.ToString();
    }

    public void DistributeWinnings()
    {
        foreach (PokerPlayer winner in winnerCalculator.winners)
        {
            // divide winnings among all winners in case of split pot
            GiveMoneyToPlayer(winner, potMoney / winnerCalculator.winners.Count);
        }

        // set pot to zero in case there is left over from split pot (due to integer division)
        ResetPot();
    }

    public void CollectBlinds()
    {
        DetermineBlindIndexes(dealer.players);
        CollectMoneyFromPlayer(dealer.players[smallBlindIdx], smallBlind);
        CollectMoneyFromPlayer(dealer.players[bigBlindIdx], bigBlind);

    }

    private void DetermineBlindIndexes(List<PokerPlayer> players)
    {
        // normal case: more than 2 players
        if (players.Count > 2)
        {
            // small blind is one space to left of dealer
            smallBlindIdx = dealer.dealerIdx + 1;

            // correct for any mistake caused by wrap-around
            if (smallBlindIdx >= players.Count) { smallBlindIdx -= players.Count; }

            // big blind is one space to left of small blind
            bigBlindIdx = smallBlindIdx + 1;

            // correct for any mistake caused by wrap-around
            if (bigBlindIdx >= players.Count) { bigBlindIdx -= players.Count; }
        }

        // heads up case: only 2 players
        else if (players.Count == 2)
        {
            // small blind is the non-dealer
            smallBlindIdx = dealer.dealerIdx + 1;

            // correct for any mistake caused by wrap-around
            if (smallBlindIdx >= players.Count) { smallBlindIdx -= players.Count; }

            // big blind is dealer
            bigBlindIdx = dealer.dealerIdx;
        }

        else return;

    }
}
