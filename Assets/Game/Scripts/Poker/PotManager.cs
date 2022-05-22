using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotManager : MonoBehaviour
{
    [SerializeField] int startingMoney;

    public int smallBlind = 1;
    public int bigBlind = 2;

    public Text potMoneyText;
    public int potMoney;

    public int bigBlindIdx;
    public int smallBlindIdx;

    public int highestBet;

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
        Debug.Log(players[4].nickName);
    }

    public void RaiseBlinds()
    {
        smallBlind *= 2;
        bigBlind *= 2;
    }

    public void InitMoney(List<PokerPlayer> players)
    {
        foreach (PokerPlayer player in players)
        {
            player.money = startingMoney;
            player.playerPosition.moneyText.text = "$" + player.money.ToString();
        }

        ResetPot();

        ClearBets(players);
    }

    private void ResetPot()
    {
        potMoney = 0;
        potMoneyText.text = "$" + potMoney.ToString();

        highestBet = 0;
    }

    public void ClearBets(List<PokerPlayer> players)
    {
        foreach(PokerPlayer player in players)
        {
            player.currentBet = 0;
        }

        highestBet = 0;
    }

    public void AddToPot(int amount)
    {
        potMoney += amount;
        potMoneyText.text = potMoneyText.text = "$" + potMoney.ToString();
    }

    public void CollectMoneyFromPlayer(PokerPlayer player, int amount)
    {
        // if player has less than required amount, just give all the money the player has
        if (amount > player.money) { amount = player.money; }

        // take money from player and put it in pot
        player.money -= amount;
        AddToPot(amount);

        // keep track of money player paid, to find how much they owe to call raises
        player.currentBet += amount;

        // update player money display
        player.playerPosition.moneyText.text = "$" + player.money.ToString();
    }

    public void GiveMoneyToPlayer(PokerPlayer player, int amount)
    {
        // take money from pot and give it to player
        AddToPot(-1 * amount);
        player.money += amount;

        // update player money display
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

    public void CollectBlinds(List<PokerPlayer> players)
    {
        DetermineBlindIndexes(players);
        CollectMoneyFromPlayer(players[smallBlindIdx], smallBlind);
        CollectMoneyFromPlayer(players[bigBlindIdx], bigBlind);

        // players have to match big blind in order to play
        highestBet = bigBlind;
    }

    private void DetermineBlindIndexes(List<PokerPlayer> players)
    {
        // normal case: more than 2 players
        if (players.Count > 2)
        {
            DetermineSmallBlindIndex(players);
            DetermineBigBlindIndex(players);
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

    private void DetermineSmallBlindIndex(List<PokerPlayer> players)
    {
        // initially set small blind to left of dealer chip, while correcting for wrap-around
        smallBlindIdx = dealer.dealerIdx + 1;
        if (smallBlindIdx >= players.Count) { smallBlindIdx -= players.Count; }

        // if small blind is on eliminated player, keep rotating it until it is not
        while (players[smallBlindIdx].eliminated == true)
        {
            smallBlindIdx++;
            if (smallBlindIdx >= players.Count) { smallBlindIdx -= players.Count; }
        }
    }

    private void DetermineBigBlindIndex(List<PokerPlayer> players)
    {
        // initially set big blind to left of small blind, while correcting for wrap-around
        bigBlindIdx = smallBlindIdx + 1;
        if (bigBlindIdx >= players.Count) { bigBlindIdx -= players.Count; }

        // if big blind is on eliminated player, keep rotating it until it is not
        while (players[bigBlindIdx].eliminated == true)
        {
            bigBlindIdx++;
            if (bigBlindIdx >= players.Count) { bigBlindIdx -= players.Count; }
        }
    }

    public int GetLowStack(List<PokerPlayer> players)
    {
        int lowStack = players[0].money;

        foreach(PokerPlayer player in players)
        {
            if (player.money <= lowStack) { lowStack = player.money; }
        }

        return lowStack;
    }

    public int GetHighStack(List<PokerPlayer> players)
    {
        int highStack = players[0].money;

        foreach (PokerPlayer player in players)
        {
            if (player.money >= highStack) { highStack = player.money; }
        }

        return highStack;
    }

    // useful when combining tables and want to make sure new players money doesn't disappear
    public void RefreshPlayerMoney(PokerPlayer player)
    {
        player.playerPosition.moneyText.text = "$" + player.money.ToString();
    }

    public int GetSecondHighestActiveStack()
    {
        List<PokerPlayer> activePlayers = new List<PokerPlayer>();
        int i;
        int highStackIdx = 0;

        foreach(PokerPlayer player in dealer.players)
        {
            if (player.eliminated || player.folded) { continue; }
            else
            {
                activePlayers.Add(player);
            }
        }
  
        int highStack = activePlayers[0].money;
       
        // first find highest stack
        for (i=0; i<activePlayers.Count; i++)
        {
            if (activePlayers[i].eliminated || activePlayers[i].folded) { continue; }

            if (activePlayers[i].money >= highStack)
            {
                highStack = activePlayers[i].money;
                highStackIdx = i;
            }
        }

        // remove highest stack
        activePlayers.RemoveAt(highStackIdx);

        int secondHighestStack = activePlayers[0].money;

        // find highest remaining stack, which is second highest stack
        for (i = 0; i < activePlayers.Count; i++)
        {
            if (activePlayers[i].eliminated || activePlayers[i].folded) { continue; }

            if (activePlayers[i].money >= highStack)
            {
                secondHighestStack = activePlayers[i].money;
            }
        }

        return secondHighestStack;
    }

    // there are some bugs with LevelOffRaise function, so will have to sideline it for now
    public int LevelOffRaise(int amountOwed)
    {
        int secondHighestStack = GetSecondHighestActiveStack();

        if (amountOwed > secondHighestStack) { amountOwed = secondHighestStack; }

        return amountOwed;
    }

    public int CountPlayersWithMoney()
    {
        int playersWithMoney = 0;

        foreach (PokerPlayer player in dealer.players)
        {
            if (player.eliminated || player.folded) { continue; }
            else
            {
                if (player.money > 0) { playersWithMoney++; }
            }
        }

        return playersWithMoney;
    }
}
