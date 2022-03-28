using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Deal, PlayerBet, NPCsBet, PreReveal, PostReveal, PostHand }

public class ControlHub : MonoBehaviour
{
    GameState gameState;

    private Dealer dealer;
    private CanvasController canvasController;
    private HandCalculator handCalculator;
    private WinnerCalculator winnerCalculator;

    // Start is called before the first frame update
    void Start()
    {
        dealer = FindObjectOfType<Dealer>();
        canvasController = FindObjectOfType<CanvasController>();
        handCalculator = FindObjectOfType<HandCalculator>();
        winnerCalculator = FindObjectOfType<WinnerCalculator>();

        gameState = GameState.Deal;
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine();
    }

    private void StateMachine()
    { 
        if (gameState == GameState.Deal) 
        { 
            canvasController.HandleDeal(); 
        }
        else if (gameState == GameState.PreReveal) 
        { 
            canvasController.HandlePreReveal(); 
        }
        else if (gameState == GameState.PostReveal)
        {
            canvasController.HandlePostReveal();
        }
        else if (gameState == GameState.PostHand) 
        { 
            canvasController.HandleNewHand();
        }

        
    }

    public void SetState(GameState state)
    {
        gameState = state;
    }
}
