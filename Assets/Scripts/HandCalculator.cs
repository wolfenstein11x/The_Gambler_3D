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
    private char[,] straightCombos = new char[,] { {'a','2','3','4','5'}, {'2','3','4','5','6'}, {'3','4','5','6','7'},
                                          {'4','5','6','7','8' },{'5','6','7','8','9'}, {'6','7','8','9','t'},
                                          {'7','8','9','t','j' },{'8','9','t','j','q'}, {'9','t','j','q','k'},
                                          {'t','j','q','k','a' } };

    private Dictionary<char, int> ranks = new Dictionary<char, int>()
        {
            {'2',2},{'3',3},{'4',4},{'5',5},{'6',6},{'7',7},{'8',8},{'9',9},{'t',10},{'j',11},{'q',12},{'k',13},{'a',14}
        };

    void Start()
    {
        /*
        List<string> myHand = new List<string> { "2s", "3d", "7h", "kc", "jd", "as", "4c" };
        myHand = SortHandHighLow(myHand);

        foreach(string card in myHand) { Debug.Log(card); }
        */
    }

    public int CheckHand(List<string> hand)
    {
        // first sort cards frmo high to low to make it easier later
        hand = SortHandHighLow(hand);

        int pairScore = CheckForPairs(hand);
        bool flush = CheckForFlush(hand);
        bool straight = CheckAllStraights(hand);

        // full house/quads
        if (pairScore > 6) { return pairScore; }

        // flush
        else if (flush) { return 6; }

        // straight
        else if (straight) { return 5; }

        // trips, pairs, high card
        else { return pairScore; }
    }

    private bool CheckForFlush(List<string> hand)
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
