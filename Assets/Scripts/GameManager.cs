using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        DEAL,
        OPPONENT,
        PLAYER,
        RESOLVE,
        DISCARD, 
        RESHUFFLE
    };

    public static GameState state;

    [System.NonSerialized] public List<GameObject> playerHand = new List<GameObject>();

    public int playerHandCount;
    public Transform playerPos;

    private void Start()
    {
        state = GameState.DEAL;
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.DEAL:
                if (playerHand.Count < playerHandCount)
                {
                    DealCard();
                }
                else
                {
                    state = GameState.OPPONENT;
                }
                break;
            case GameState.PLAYER:
                break;
            case GameState.RESOLVE:
                break;
        }
    }

    void DealCard()
    {
        GameObject nextCard = DeckManager.deck[DeckManager.deck.Count - 1];
        Vector3 newPos = playerPos.transform.position;
        newPos.x += 2f * playerHand.Count;
        nextCard.GetComponent<Card>().SetTargetPos(newPos);
        playerHand.Add(nextCard);
        DeckManager.deck.Remove(nextCard);
    }
}
