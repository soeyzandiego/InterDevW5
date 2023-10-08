using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float delayTime = 0.5f;

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

    [System.NonSerialized] public List<Card> playerHand = new List<Card>();
    [System.NonSerialized] public List<Card> opponentHand = new List<Card>();

    public int playerHandCount;
    [Header("Card Positions")]
    [SerializeField] Transform playerPos;
    [SerializeField] Transform opponentPos;

    bool active = false;

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
                    if (!active) { StartCoroutine(DealCard(playerHand)); }
                }
                else if (opponentHand.Count < playerHandCount)
                {
                    if (!active) { StartCoroutine(DealCard(opponentHand)); }
                }
                break;
            case GameState.PLAYER:
                break;
            case GameState.RESOLVE:
                break;
        }
    }

    IEnumerator DealCard(List<Card> handToDeal)
    {
        active = true;

        for (int i = 0; i < playerHandCount; i++)
        {
            Card nextCard = DeckManager.deck[DeckManager.deck.Count - 1];

            Vector3 newPos;
            if (handToDeal == playerHand) { newPos = playerPos.position; }
            else { newPos = opponentPos.position; }
            newPos.x += 2f * handToDeal.Count;

            nextCard.SetTargetPos(newPos);
            handToDeal.Add(nextCard);
            DeckManager.deck.Remove(nextCard);
            yield return new WaitForSecondsRealtime(delayTime);
        }
        active = false;
    }
}
