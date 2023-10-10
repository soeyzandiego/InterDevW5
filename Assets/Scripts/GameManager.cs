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
    [System.NonSerialized] public static int opponentScore;
    [System.NonSerialized] public static int playerScore;

    // stored cards
    [System.NonSerialized] public List<Card> playerHand = new List<Card>();
    [System.NonSerialized] public List<Card> opponentHand = new List<Card>();
    Card pOpponentCard; // played opponent card
    Card pPlayerCard;   // played player card

    [Header("Card Position References")]
    [SerializeField] Transform playerPos;
    [SerializeField] Transform opponentPos;
    [SerializeField] Transform cardPlayPos;
    [SerializeField] Transform discardPos;

    [Header("Positioning Parameters")]
    [SerializeField][Range(0.1f, 3)] float hoverAmount = 1f;
    [SerializeField][Range(0.1f, 3)] float handSpacing = 1f;
    [SerializeField][Range(0.025f, 0.8f)] float stackSpacing = 0.05f;

    bool active = false;

    AudioPlayer audioPlayer;
    Resolver resolver;

    private void Start()
    {
        state = GameState.DEAL;
        audioPlayer = FindObjectOfType<AudioPlayer>();
        resolver = FindObjectOfType<Resolver>();
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
                    else { StartCoroutine(WaitAndChangeState(GameState.OPPONENT, delayTime)); }
                }
            break;

            case GameState.OPPONENT:
                // randomly choose one card to play
                if (!active)
                {
                    active = true;
                    int cardIndex = Random.Range(0, playerHandCount);
                    Card cardToPlay = opponentHand[cardIndex];
                    cardToPlay.SetTargetPos(cardPlayPos.position + new Vector3(0, 1.2f, 0));
                    opponentHand.Remove(cardToPlay);
                    pOpponentCard = cardToPlay;
                    StartCoroutine(WaitAndChangeState(GameState.PLAYER, delayTime));
                }
                
            break;

            case GameState.PLAYER:
                foreach (Card card in playerHand) 
                {
                    { card.Flip(true); }

                    if (card.IsHovered())
                    {
                        card.SetTargetPos(new Vector3(card.transform.position.x, playerPos.position.y + hoverAmount));
                        // select card
                        if (Input.GetMouseButtonDown(0))
                        {
                            active = true;
                            Card cardToPlay = card;
                            cardToPlay.SetTargetPos(cardPlayPos.position + new Vector3(0, -1.2f, 0));
                            playerHand.Remove(cardToPlay);
                            pPlayerCard = cardToPlay;
                            StartCoroutine(WaitAndChangeState(GameState.RESOLVE, delayTime));
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
                if (!active)
                {
                    active = true;
                    pOpponentCard.Flip(true);
                    resolver.Resolve(pOpponentCard, pPlayerCard);
                    StartCoroutine(WaitAndChangeState(GameState.DISCARD, 1f));
                }
            break;

            case GameState.DISCARD:
                if (pOpponentCard != null)
                {
                    // opponent chosen card
                    if (!active)
                    { 
                        StartCoroutine(DiscardCard(pOpponentCard));
                        pOpponentCard = null;
                    }
                }
                else if (pPlayerCard != null)
                {
                    if (!active) 
                    { 
                        StartCoroutine(DiscardCard(pPlayerCard));
                        pPlayerCard = null;
                    }
                }
                else if (opponentHand.Count > 0)
                {
                    // opponent hand
                   if (!active) { StartCoroutine(DiscardCard(opponentHand)); }
                }
                else if (playerHand.Count > 0)
                {
                    if (!active) { StartCoroutine(DiscardCard(playerHand)); }
                }
                else
                {
                    if (!active)
                    {
                        if (DeckManager.deck.Count > 0) { StartCoroutine(WaitAndChangeState(GameState.DEAL, delayTime)); }
                        else { StartCoroutine(WaitAndChangeState(GameState.RESHUFFLE, delayTime)); }
                    }
                }
                
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
            newPos.x += handSpacing * handToDeal.Count;

            nextCard.SetTargetPos(newPos);
            handToDeal.Add(nextCard);
            if (handToDeal == playerHand) { nextCard.inHand = true; }
            DeckManager.deck.Remove(nextCard);

            audioPlayer.PlayDrawSound();
            yield return new WaitForSecondsRealtime(delayTime);
        }
        active = false;
    }

    IEnumerator DiscardCard(List<Card> targetList)
    {
        active = true;

        if (targetList.Count > 0)
        {
            Card discardCard = targetList[targetList.Count - 1]; // grab top card
            discardCard.Flip(true);
            discardCard.SetTargetPos(discardPos.position + new Vector3(0, stackSpacing * DeckManager.discardPile.Count));
            discardCard.GetComponent<SpriteRenderer>().sortingOrder = DeckManager.discardPile.Count;
            DeckManager.discardPile.Add(discardCard);
            targetList.Remove(discardCard);

            audioPlayer.PlayDrawSound();
            yield return new WaitForSecondsRealtime(delayTime);
        }

        active = false;
    }

    IEnumerator DiscardCard(Card targetCard)
    {
        active = true;

        targetCard.SetTargetPos(discardPos.position + new Vector3(0, stackSpacing * DeckManager.discardPile.Count));
        targetCard.GetComponent<SpriteRenderer>().sortingOrder = DeckManager.discardPile.Count;
        DeckManager.discardPile.Add(targetCard);

        audioPlayer.PlayDrawSound();
        yield return new WaitForSecondsRealtime(delayTime);

        active = false;
    }

    IEnumerator WaitAndChangeState(GameState newState, float delay)
    {
        active = true;
        yield return new WaitForSecondsRealtime(delay);
        state = newState;
        active = false;
    }
}
