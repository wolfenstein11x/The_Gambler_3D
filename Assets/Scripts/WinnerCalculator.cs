using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerCalculator : MonoBehaviour
{
    //public List<PokerPlayer> finalists = new List<PokerPlayer>();
    public List<PokerPlayer> winners = new List<PokerPlayer>();
    public int highScore;

    HandCalculator handCalculator;

    // Start is called before the first frame update
    void Start()
    {
        handCalculator = FindObjectOfType<HandCalculator>();
    }

    public bool CheckForPrematureWinner(List<PokerPlayer> players)
    {
        int activePlayersCount = 0;

        foreach (PokerPlayer player in players)
        {
            // skip over folded or eliminated players
            if (player.folded || player.eliminated) { continue; }

            else { activePlayersCount++; }
        }

        // if everyone folded except one person, then we have premature winner
        return (activePlayersCount == 1);
    }

    // only call this function if we know there is a premature winner
    public void DeterminePrematureWinner(List<PokerPlayer> players)
    {
        foreach (PokerPlayer player in players)
        {
            // skip over folded or eliminated players
            if (player.folded || player.eliminated) { continue; }

            else 
            {
                winners.Clear();
                winners.Add(player);
            }
        }
    }

    // make a list of all players who have the highest hand
    public List<PokerPlayer> DetermineFinalists(List<PokerPlayer> players)
    {
        
        int highScore = GetHighScore(players);
        
        List<PokerPlayer> finalists = new List<PokerPlayer>();

        foreach (PokerPlayer player in players)
        {
            // skip over folded or eliminated players
            if (player.folded || player.eliminated) { continue; }

            if (handCalculator.ScoreHand(player.hand) == highScore) { finalists.Add(player); }
        }
        //Debug.Log("Finalists: ");
        //PrintList(finalists);
        return finalists;
    }

    // helper function for DetermineFinalists
    private int GetHighScore(List<PokerPlayer> players)
    {
        highScore = 0;

        foreach(PokerPlayer player in players)
        {
            // skip over folded or eliminated players
            if (player.folded || player.eliminated) { continue; }
            
            int handScore = handCalculator.ScoreHand(player.hand);
            
            if (handScore > highScore) { highScore = handScore; }
        }
        //Debug.Log("high score: " + highScore);
        return highScore;
    }

    // make a list of players with the highest hand and highest cards (usually will only be one... if multiple, it is a split pot)
    public List<PokerPlayer> FindWinners(List<PokerPlayer> finalists)
    {
        // sort all hands because preceding functions depend on hands being sorted high to low
        OptimizeAllHands(finalists);

        // clear winners from last hand
        winners.Clear();
        
        // break the tie until there is only one player left... if you go 5 rounds (for all 5 cards) and still have multiple players, it is a split plot
        // NOTE: this would be a good time to use recursion
        List<PokerPlayer> survivorsRound1 = BreakTieAtIdx(finalists, 0);
        List<PokerPlayer> survivorsRound2 = BreakTieAtIdx(survivorsRound1, 1);
        List<PokerPlayer> survivorsRound3 = BreakTieAtIdx(survivorsRound2, 2);
        List<PokerPlayer> survivorsRound4 = BreakTieAtIdx(survivorsRound3, 3);
        winners = BreakTieAtIdx(survivorsRound4, 4);

        //Debug.Log("Winners: ");
        //PrintList(winners);
        return winners;
    }

    // helper function for FindWinners
    private List<PokerPlayer> BreakTieAtIdx(List<PokerPlayer> finalists, int idx)
    {
        List<PokerPlayer> survivors = new List<PokerPlayer>();

        int highRank = HighestRankAtIdx(finalists, idx);


        foreach(PokerPlayer finalist in finalists)
        {
            if (handCalculator.ranks[finalist.hand[idx][0]] == highRank) { survivors.Add(finalist);  }
        }

        return survivors;
    }

    // helper function for BreakTie
    private int HighestRankAtIdx(List<PokerPlayer> finalists, int idx)
    {
        int highestRank = 0;

        foreach(PokerPlayer finalist in finalists)
        {
            if (handCalculator.ranks[finalist.hand[idx][0]] > highestRank) { highestRank = handCalculator.ranks[finalist.hand[idx][0]]; }
        }
        
        return highestRank;
    }

    private void OptimizeAllHands(List<PokerPlayer> activePlayers)
    {
        foreach(PokerPlayer activePlayer in activePlayers)
        {
            activePlayer.hand = handCalculator.OptimizeHand(activePlayer.hand);
        }
    }

    // for debugging
    private void PrintList(List<PokerPlayer> players)
    {
        foreach(PokerPlayer player in players)
        {
            Debug.Log(player.nickName);
        }
    }

    private void PrintHand(List<string> hand)
    {
        foreach (string card in hand)
        {
            Debug.Log(card);
        }
    }

}
