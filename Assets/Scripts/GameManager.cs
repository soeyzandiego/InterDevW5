using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float delayTime = 0.5f;
    [SerializeField] int playerHandCount;

    public enum GameState
    {
        DEAL,
        OPPONENT,
        PLAYER,
        RESOLVE,
        DISCARD, 
        RESHUFFLE
    };

    [System.NonSerialized] public static GameState state; // does not actually get serialized anyways, but for consistency

    [System.NonSerialized] public List<Card> playerHand = new List<Card>();
    [System.NonSerialized] public List<Card> opponentHand = new List<Card>();
    Card pOpponentCard; // played opponent card
    Card pPlayerCard;   // played player card

    [Header("Card Position References")]
    [SerializeField] Transform playerPos;
    [SerializeField] Transform opponentPos;
    [SerializeField] Transform cardPlayPos;

    [Header("Positioning Parameters")]
    [SerializeField][Range(0.1f, 3)] public float hoverAmount = 1f;

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
                if (!active)
                {
                    // deal opponent
                    if (opponentHand.Count < playerHandCount) { StartCoroutine(DealCard(opponentHand)); }
                    // deal player
                    else if (playerHand.Count < playerHandCount) { StartCoroutine(DealCard(playerHand)); }
                    else { StartCoroutine(WaitAndChangeState(GameState.OPPONENT)); }
                }
            break;

            case GameState.OPPONENT:
                // randomly choose one card to play
                if (!active)
                {
                    active = true;
                    int cardIndex = Random.Range(0, playerHandCount);
                    Card cardToPlay = opponentHand[cardIndex];
                    cardToPlay.SetTargetPos(cardPlayPos.position);
                    opponentHand.Remove(cardToPlay);
                    pOpponentCard = cardToPlay;
                    StartCoroutine(WaitAndChangeState(GameState.PLAYER));
                }
                
            break;

            case GameState.PLAYER:
                foreach (Card card in playerHand) 
                {
                    card.Flip();

                    if (card.IsHovered())
                    {
                        card.SetTargetPos(new Vector3(card.transform.position.x, playerPos.position.y + hoverAmount));
                        // select card
                        if (Input.GetMouseButtonDown(0))
                        {
                            active = true;
                            Card cardToPlay = card;
                            cardToPlay.SetTargetPos(cardPlayPos.position);
                            playerHand.Remove(cardToPlay);
                            pPlayerCard = cardToPlay;
                            StartCoroutine(WaitAndChangeState(GameState.RESOLVE));
                            break;
                        }
                    }
                    else
                    {
                        card.SetTargetPos(new Vector3(card.transform.position.x, playerPos.position.y));
                    }
                }

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
            if (handToDeal == playerHand) { nextCard.inHand = true; }
            DeckManager.deck.Remove(nextCard);
            yield return new WaitForSecondsRealtime(delayTime);
        }
        active = false;
    }

    IEnumerator WaitAndChangeState(GameState newState)
    {
        //active = true;
        yield return new WaitForSecondsRealtime(delayTime);
        state = newState;
        //active = false;
    }
}
