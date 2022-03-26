using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandCalculator : MonoBehaviour
{
    [SerializeField] List<string> testHand;

    public Dictionary<int, string> handNames = new Dictionary<int, string>()
    {
        {9, "Straight flush"}, {8, "Quads"}, {7, "Full house"}, {6, "Flush"}, {5, "Straight"}, {4, "Trips"}, {3, "Two-pair"}, {2, "Pair"}, {1, "High card"}
    };

    private string[,] straightFlushSpecialCases = new string[,] { {"as","2s","3s","4s","5s"}, {"ad", "2d", "3d", "4d", "5d" }, {"ac", "2c", "3c", "4c", "5c" }, 
                                                                  {"ah", "2h", "3h", "4h", "5h" } };

    private Dictionary<char, int> ranks = new Dictionary<char, int>()
        {
            {'2',2},{'3',3},{'4',4},{'5',5},{'6',6},{'7',7},{'8',8},{'9',9},{'t',10},{'j',11},{'q',12},{'k',13},{'a',14}
        };

    void Start()
    {
        List<string> sortedHand = SortHandHighLow(testHand);
        int handScore = ScoreHand(testHand);
        string handName = handNames[handScore];

        List<string> optimizedHand = OptimizeHand(sortedHand);
        Debug.Log("[" + sortedHand[0] + "," + sortedHand[1] + "," + sortedHand[2] + "," + sortedHand[3] + "," + sortedHand[4] + "," + sortedHand[5] + "," + sortedHand[6] + "]" + " = " + handName);
        Debug.Log("[" + optimizedHand[0] + "," + optimizedHand[1] + "," + optimizedHand[2] + "," + optimizedHand[3] + "," + optimizedHand[4] + "]" + " = " + handName);
    }

    public int ScoreHand(List<string> hand)
    {
        // sort hand from high to low because hand checker functions assume it is sorted
        hand = SortHandHighLow(hand);

        if (CheckStraightFlush(hand)) return 9;
        else if (CheckQuads(hand)) return 8;
        else if (CheckFullHouse(hand)) return 7;
        else if (CheckFlush(hand)) return 6;
        else if (CheckStraight(hand)) return 5;
        else if (CheckTrips(hand)) return 4;
        else if (Check2Pair(hand)) return 3;
        else if (CheckPair(hand)) return 2;
        else return 1;

    }

    public string GetHandName(List<string> hand)
    {
        return handNames[ScoreHand(hand)];
    }

    private bool CheckStraightFlush(List<string> hand)
    {
        // fill this up/ clear it as you go
        List<string> straightFlushHand = new List<string>();

        // scenario #1, start at index 0
        straightFlushHand = GetStraightHand(hand, 0);

        if (straightFlushHand.Count == 5 && CheckFlush(straightFlushHand)) { return true; }

        // scenario #2, start at index 1
        straightFlushHand = GetStraightHand(hand, 1);

        if (straightFlushHand.Count == 5 && CheckFlush(straightFlushHand)) { return true; }

        // scenario #3, start at index 2
        straightFlushHand = GetStraightHand(hand, 2);

        if (straightFlushHand.Count == 5 && CheckFlush(straightFlushHand)) { return true; }

        // scenario #4, check special case (a-2-3-4-5)
        if (CheckSpecialCaseStraightFlush(hand)) { return true; }

        return false;
    }

    
    private bool CheckQuads(List<string> hand)
    {
        foreach (string card in hand)
        {
            // count matches in letter at idx 0 (which is rank)
            int dups = CountDups(card[0], 0, hand);

            if (dups == 3) { return true; }
        }
        return false;
    }

    private bool CheckFullHouse(List<string> hand)
    {
        bool trips = false;
        bool pair = false;

        foreach (string card in hand)
        {
            // count matches in letter at idx 0 (which is rank)
            int dups = CountDups(card[0], 0, hand);

            if (dups == 2) { trips = true; }
            else if (dups == 1) { pair = true; }
        }

        return (trips && pair);
    }

    private bool CheckFlush(List<string> hand)
    {
        foreach (string card in hand)
        {
            // count matches in letter at idx 1 (which is suit)
            int suitDups = CountDups(card[1], 1, hand);

            // count 5 of the same suit, exit function, you have flush
            if (suitDups >= 4) { return true; }
        }

        // no flush if never counted 5 of same suit
        return false;
    }

    private bool CheckStraight(List<string> hand)
    {   
        // fill this up/ clear it as you go
        List<string> straightHand = new List<string>();

        // scenario #1, start at index 0
        if (GetStraightHand(hand, 0).Count == 5) { return true; }
        
        // scenario #2, start at index 1
        if (GetStraightHand(hand, 1).Count == 5) { return true; }
        
        // scenario #3, start at index 2
        if (GetStraightHand(hand, 2).Count == 5) { return true; }
        
        // scenario #4, check special case (a-2-3-4-5)
        if (CheckSpecialCaseStraight(hand)) { return true; }
        
        return false;
    }

    // helper function for CheckStraight()
    private List<string> GetStraightHand(List<string> hand, int startIdx)
    {
        List<string> straightHand = new List<string>();

        straightHand.Add(hand[startIdx]);
        for (int i = startIdx; i < hand.Count - 1; i++)
        {
            // add cards if they are consecutive cards 
            if (ranks[hand[i][0]] - ranks[hand[i + 1][0]] == 1) 
            { 
                straightHand.Add(hand[i + 1]);

                // quit if you get 5 connected cards (you will have a bug if you return 9-8-7-6-5-4-3, for example)
                if (straightHand.Count == 5) { return straightHand; }
            }

            // skip card if it is a pair
            else if (ranks[hand[i][0]] - ranks[hand[i + 1][0]] == 0) { continue; }
            
            // quit if cards not connected
            else break;    
        }

        return straightHand;
    }

    
    // helper function to check for special case of straight, a-2-3-4-5
    private bool CheckSpecialCaseStraight(List<string> hand)
    {
        List<char> specialStraight = new List<char>() { 'a', '2', '3', '4', '5' };
        List<char> handRanks = new List<char>();

        // make list that is just the ranks (no suits) of player hand
        foreach(string card in hand)
        {
            handRanks.Add(card[0]);
        }

        // check that each card in the special case straight is in the player hand
        foreach(char rank in specialStraight)
        {
            if (!handRanks.Contains(rank)) { return false; }
        }

        // if it makes it to here, player has the special case straight
        return true;
    }

    // helper function to check for special case of straight flush, ax-2x-3x-4x-5x
    private bool CheckSpecialCaseStraightFlush(List<string> hand)
    {
        for (int i=0; i < straightFlushSpecialCases.GetLength(0); i++)
        {
            int matches = 0;
            for (int j=0; j<hand.Count; j++)
            {
                //Debug.Log(straightFlushSpecialCases[i, j]);
                // move on to next straight flush special case combo if one of its cards is not contained in player hand
                if (!hand.Contains(straightFlushSpecialCases[i, j])) { break; }

                // if you make it here, it has one of the special case straight flush cards
                matches++;
                if (matches == 5) { return true; }
            }
        }

        // if you make it here, player does not have straight flush
        return false;
    }

   
    private bool CheckTrips(List<string> hand)
    {
        foreach (string card in hand)
        {
            // count matches in letter at idx 0 (which is rank)
            int dups = CountDups(card[0], 0, hand);

            if (dups == 2) { return true; }
        }
        return false;
    }

    private bool Check2Pair(List<string> hand)
    {
        int numPairs = 0;

        foreach (string card in hand)
        {
            // count matches in letter at idx 0 (which is rank)
            int dups = CountDups(card[0], 0, hand);

            if (dups == 1) { numPairs++; }
        }

        // pairs are double counted, so divide by 2
        numPairs /= 2;

        return (numPairs >= 2);
    }

    private bool CheckPair(List<string> hand)
    {
        int numPairs = 0;

        foreach (string card in hand)
        {
            // count matches in letter at idx 0 (which is rank)
            int dups = CountDups(card[0], 0, hand);

            if (dups == 1) { numPairs++; }
        }

        // pairs are double counted, so divide by 2
        numPairs /= 2;

        return (numPairs == 1);
    }

    
    // CountDups can be used to count duplicate ranks (idx=0) or duplicate suits (idx=1)
    private int CountDups(char letter, int idx, List<string> hand)
    {
        // start dup count at -1, since first dup counted will just be the original
        int dups = -1;

        // loop through cards and count dups
        foreach (string card in hand)
        {
            if (card[idx].Equals(letter)) { dups += 1; }
        }

        return dups;
    }

    
    public List<string> SortHandHighLow(List<string> hand, bool highToLow=true)
    {
        List<string> sortedHand = new List<string>();
        List<int> usedIndexes = new List<int>();

        int currentHighest = 0;
        int currentHighestIdx = 0;
        
        for (int i=0; i<hand.Count; i++)
        {

            for (int j=0; j<hand.Count; j++)
            {
                if (ranks[hand[j][0]] > currentHighest)
                {
                    if (usedIndexes.Contains(j)) { continue; }
                    currentHighest = ranks[hand[j][0]];
                    currentHighestIdx = j;
                    //Debug.Log("current highest is " + currentHighest);
                }
            }

            currentHighest = 0;

            sortedHand.Add(hand[currentHighestIdx]);
            usedIndexes.Add(currentHighestIdx);
        }

        return sortedHand;
    }

    public List<string> OptimizeHand(List<string> hand)
    {
        // sort hand first because optimization functions assume it is sorted
        hand = SortHandHighLow(hand);

        switch (ScoreHand(hand))
        {
            case 7:
                hand = OptimizeFullHouse(hand);
                break;
            case 6:
                hand = OptimizeFlush(hand);
                break;
            case 5:
                hand = OptimizeStraight(hand);
                break;
            case 4:
                hand = OptimizeTrips(hand);
                break;
            case 3:
                hand = Optimize2Pair(hand);
                break;
            case 2:
                hand = OptimizePair(hand);
                break;
            case 1:
                hand = OptimizeHighCard(hand);
                break;
            default:
                Debug.Log("Error: Unknown hand score");
                break;
        }
        

        return hand;
    }

    private List<string> OptimizeFullHouse(List<string> hand)
    {
        List<string> optimizedHand = new List<string>();

        // start at beginning of hand (highest card) and find trips and put trips at front of optimized hand
        for (int i = 0; i < hand.Count - 2; i++)
        {
            if (ranks[hand[i][0]] == ranks[hand[i + 2][0]])
            {
                optimizedHand.Add(hand[i]);
                optimizedHand.Add(hand[i + 1]);
                optimizedHand.Add(hand[i + 2]);

                // store trips values to reference in order to remove them
                string trip1 = hand[i];
                string trip1a = hand[i + 1];
                string trip1b = hand[i + 2];

                // remove trips from original hand
                hand.Remove(trip1);
                hand.Remove(trip1a);
                hand.Remove(trip1b);
            }
        }

        // repeat process for the pair
        for (int i = 0; i < hand.Count - 1; i++)
        {
            if (ranks[hand[i][0]] == ranks[hand[i + 1][0]])
            {
                optimizedHand.Add(hand[i]);
                optimizedHand.Add(hand[i + 1]);

                // store pair values to reference in order to remove them
                string pair1 = hand[i];
                string pair1a = hand[i + 1];

                // remove pair from original hand
                hand.Remove(pair1);
                hand.Remove(pair1a);
            }
        }

        // no more cards needed to put into optimized hand
        return optimizedHand;
    }

    private List<string> OptimizeFlush(List<string> hand)
    {
        return hand;
    }

    private List<string> OptimizeStraight(List<string> hand)
    {
        // fill this up/ clear it as you go
        List<string> straightHand = new List<string>();

        // scenario #1, start at index 0
        if (GetStraightHand(hand, 0).Count == 5)
        {
            straightHand = GetStraightHand(hand, 0);
            return straightHand;
        }

        // scenario #2, start at index 1
        else if (GetStraightHand(hand, 1).Count == 5)
        {
            straightHand = GetStraightHand(hand, 1);
            return straightHand;
        }

        // scenario #3, start at index 2
        else if (GetStraightHand(hand, 2).Count == 5)
        {
            straightHand = GetStraightHand(hand, 2);
            return straightHand;
        }

        // scenario #4, check special case (a-2-3-4-5)
        else if (CheckSpecialCaseStraight(hand)) 
        {
            List<string> specialCaseStraight = new List<string>();

            // must order hand 5-4-3-2-a, so use helper function to grab those cards from the hand
            specialCaseStraight.Add(CopyCardFromHand(hand, '5'));
            specialCaseStraight.Add(CopyCardFromHand(hand, '4'));
            specialCaseStraight.Add(CopyCardFromHand(hand, '3'));
            specialCaseStraight.Add(CopyCardFromHand(hand, '2'));
            specialCaseStraight.Add(CopyCardFromHand(hand, 'a'));

            return specialCaseStraight;   
        }

        // something is wrong if made it to here, so just return original hand
        return hand;   
    }

    private List<string> OptimizeTrips(List<string> hand)
    {
        List<string> optimizedHand = new List<string>();

        // start at beginning of hand (highest card) and find trips and put trips at front of optimized hand
        for (int i = 0; i < hand.Count - 2; i++)
        {
            if (ranks[hand[i][0]] == ranks[hand[i + 2][0]]) 
            {
                optimizedHand.Add(hand[i]);
                optimizedHand.Add(hand[i + 1]);
                optimizedHand.Add(hand[i + 2]);

                // store trips values to reference in order to remove them
                string trip1 = hand[i];
                string trip1a = hand[i + 1];
                string trip1b = hand[i + 2];

                // remove trips from original hand
                hand.Remove(trip1);
                hand.Remove(trip1a);
                hand.Remove(trip1b);

            }
        }

        // put the highest remaining cards (need 2 more) into the optimized hand after the pair, highest to lowest
        optimizedHand.Add(hand[0]);
        optimizedHand.Add(hand[1]);

        return optimizedHand;
    }

    private List<string> Optimize2Pair(List<string> hand)
    {
        List<string> optimizedHand = new List<string>();

        // start at beginning of hand (highest card) and find first pair (which is highest pair) and put pair at front of optimized hand
        for (int i = 0; i < hand.Count - 1; i++)
        {
            if (ranks[hand[i][0]] == ranks[hand[i + 1][0]])
            {
                optimizedHand.Add(hand[i]);
                optimizedHand.Add(hand[i + 1]);

                // store pair values to reference in order to remove them
                string pair1 = hand[i];
                string pair1a = hand[i + 1];

                // remove pair from original hand
                hand.Remove(pair1);
                hand.Remove(pair1a);

            }
        }

        // repeat process for 2nd highest pair
        for (int i = 0; i < hand.Count - 1; i++)
        {
            if (ranks[hand[i][0]] == ranks[hand[i + 1][0]])
            {
                optimizedHand.Add(hand[i]);
                optimizedHand.Add(hand[i + 1]);

                // store pair values to reference in order to remove them
                string pair2 = hand[i];
                string pair2a = hand[i + 1];

                // remove pair from original hand
                hand.Remove(pair2);
                hand.Remove(pair2a);

            }
        }

        // put the highest remaining card (only need 1 more) into the optimized hand after the pair
        optimizedHand.Add(hand[0]);

        return optimizedHand;
    }

    private List<string> OptimizePair(List<string> hand)
    {
        List<string> optimizedHand = new List<string>();

        // start at beginning of hand (highest card) and find first pair (which is highest pair) and put pair at front of optimized hand
        for (int i=0; i<hand.Count-1; i++)
        {
            if (ranks[hand[i][0]] == ranks[hand[i+1][0]]) 
            {
                optimizedHand.Add(hand[i]);
                optimizedHand.Add(hand[i + 1]);

                // store pair values to reference in order to remove them
                string pair1 = hand[i];
                string pair1a = hand[i + 1];

                // remove pair from original hand
                hand.Remove(pair1);
                hand.Remove(pair1a);
                
            }
        }

        // put the highest remaining cards (need 3 more) into the optimized hand after the pair, highest to lowest
        int j = 0;
        while (optimizedHand.Count < 5)
        {
            optimizedHand.Add(hand[j]);
            j++;
        }

        return optimizedHand;
    }

    private List<string> OptimizeHighCard(List<string> hand)
    {
        // remove lowest 2 cards
        hand.RemoveAt(hand.Count - 1);
        hand.RemoveAt(hand.Count - 1);

        return hand;
    }

    // UTILITY FUNCTIONS
    private List<string> CopyCardsFromHand(List<string> hand, char rank)
    {
        List<string> cards = new List<string>();

        foreach(string card in hand)
        {
            if (card[0] == rank) { cards.Add(card); }
        }

        return cards;
    }

    private string CopyCardFromHand(List<string> hand, char rank)
    {
        foreach (string card in hand)
        {
            if (card[0] == rank) { return card; }
        }

        return "";
    }


}
