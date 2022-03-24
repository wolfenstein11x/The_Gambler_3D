using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    public Headshot headShot;
    public CardImg1 cardImg1;
    public CardImg2 cardImg2;

    // Start is called before the first frame update
    void Start()
    {
        headShot = GetComponentInChildren<Headshot>();
        cardImg1 = GetComponentInChildren<CardImg1>();
        cardImg2 = GetComponentInChildren<CardImg2>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
