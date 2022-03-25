using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
straight flush = 9
quads = 8
full house = 7
flush = 6
straight = 5
trips = 4
two pair = 3
pair = 2
high card = 1
*/

public class HandCalculator : MonoBehaviour
{
    //[SerializeField] List<string> testHand;

    private Dictionary<int, string> handNames = new Dictionary<int, string>()
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
        //Debug.Log(ScoreHand(testHand));
    }

    public int ScoreHand(List<string> hand)
    {
        // first sort cards from high to low to make it easier later
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
        if (CheckSpecialCaseStraight(hand)) { return true; }

        return false;
    }

    private List<string> MakeSublist(List<string> original, int start, int finish)
    {
        List<string> newList = new List<string>();

        for (int i=start; i <= finish; i++)
        {
            newList.Add(original[i]);
        }

        return newList;
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
        straightHand = GetStraightHand(hand, 0);

        if (straightHand.Count == 5) { return true; }

        // scenario #2, start at index 1
        straightHand = GetStraightHand(hand, 1);

        if (straightHand.Count == 5) { return true; }

        // scenario #3, start at index 2
        straightHand = GetStraightHand(hand, 2);

        if (straightHand.Count == 5) { return true; }

        // scenario #4, check special case (a-2-3-4-5)
        if (CheckSpecialCaseStraight(hand)) { return true; }

        return false;
    }

    private List<string> GetStraightHand(List<string> hand, int startIdx)
    {
        List<string> straightHand = new List<string>();

        straightHand.Add(hand[startIdx]);
        for (int i = startIdx; i < hand.Count - 1; i++)
        {
            if (ranks[hand[i][0]] - ranks[hand[i + 1][0]] == 1) { straightHand.Add(hand[i + 1]); }
            else if (ranks[hand[i][0]] - ranks[hand[i + 1][0]] == 0) { continue; }
            else break;
        }

        return straightHand;
    }

    // helper function to determine if 5 card subset is a straight
    private bool FiveConsecutive(List<string> fiveCards)
    {
        // make sure each card is exactly +1 greater than next card
        for (int i=0; i<fiveCards.Count-1; i++)
        {
            if (ranks[fiveCards[i][0]] - ranks[fiveCards[i+1][0]] != 1) return false; 
        }

        return true;
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

    /*
    private bool CheckAllStraights(List<string> hand)
    {
        // loop through all 10 straight combos
        for (int i = 0; i < straightCombos.GetLength(0); i++)
        {
            bool straightFound = CheckStraight(straightCombos[i, 0], straightCombos[i, 1], straightCombos[i, 2],
                                               straightCombos[i, 3], straightCombos[i, 4], hand);

            // exit the function and return true if you find a straight
            if (straightFound) { return true; }
        }

        // if you get to this point, there is no straight
        return false;
    }


    // helper function to check for a particular straight combo
    private bool CheckStraight(char r1, char r2, char r3, char r4, char r5, List<string> hand)
    {
        // take input chars and put the into array
        char[] straightArr = new char[] { r1, r2, r3, r4, r5 };

        // take first letters (ranks) from player hand and put into array
        char[] handRanksArr = new char[] {hand[0][0], hand[1][0], hand[2][0], hand[3][0],
                                          hand[4][0], hand[5][0], hand[6][0]};

        // check if player hand contains straight combo
        if (IsSubset(handRanksArr, straightArr, 5, 7)) { return true; }
        else { return false; }
    }

    // helper function to check if a 7 card array contains a certain 5 card array
    private bool IsSubset(char[] arr1, char[] arr2, int arr2Size, int arr1Size)
    {
        int i = 0;
        int j = 0;

        for (i = 0; i < arr2Size; i++)
        {
            for (j = 0; j < arr1Size; j++)
            {
                if (arr2[i] == arr1[j]) { break; }
            }

            // if the above loop goes once through without breaking, then its false
            if (j == arr1Size) { return false; }
        }

        // if we make it here, then arr2 is subset of arr1
        return true;
    }
    */

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

    // helper function to be used to resolve special case when hand has 3 pairs
    private bool Check3Pair(List<string> hand)
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

        return (numPairs == 3);
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

    private int CheckForPairs(List<string> hand)
    {
        // initialize pair counts to zero
        int numPairs = 0;
        bool trips = false;
        bool fullHouse = false;
        bool quads = false;

        foreach (string card in hand)
        {
            // count matches in letter at idx 0 (which is rank)
            int dups = CountDups(card[0], 0, hand);

            if (dups == 3) { quads = true; }
            else if (dups == 2) { trips = true; }
            else if (dups == 1) { numPairs++; }

        }

        // pairs are double counted, so divide by 2
        numPairs /= 2;

        // TODO write RemoveLowestPair function 
        // if (numPairs == 3) { RemoveLowestPair(); }

        // check for full house
        if (numPairs >= 1 && trips) { fullHouse = true; }

        if (quads) { return 8; }
        else if (fullHouse) { return 7; }
        else if (trips) { return 4; }
        else if (numPairs >= 2) { return 3; }
        else if (numPairs == 1) { return 2; }
        else { return 1; }

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
        // case statement with case being score of hand
            // in pair, 2 pair, or trips case:
                // sort list in order of rank, highest to lowest
                // start at lowest, and remove element if its not a dup, until list only has 5 items
            // in flush case:
                // remove every element that doesn't have a suit dup count of 5
                // sort list in order of rank, highest to lowest
                // remove lowest elements until list only has 5 items

        return hand;
    }
}
