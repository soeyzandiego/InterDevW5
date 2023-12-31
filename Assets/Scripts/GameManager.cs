using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] float delayTime = 0.5f;
    [SerializeField] int playerHandCount;

    public enum GameState
    {
        PAUSE,
        DEAL,
        OPPONENT,
        PLAYER,
        RESOLVE,
        DISCARD, 
        RESHUFFLE
    };

    [System.NonSerialized] public static GameState state = GameState.PAUSE; // does not actually get serialized anyways, but for consistency

    // stored cards
    [System.NonSerialized] public List<Card> playerHand = new List<Card>();
    [System.NonSerialized] public List<Card> opponentHand = new List<Card>();
    Card pOpponentCard; // played opponent card
    Card pPlayerCard;   // played player card

    [Header("Scoring")]
    [SerializeField] TMP_Text opponentText;
    [SerializeField] TMP_Text playerText;
    [System.NonSerialized] public static int opponentScore;
    [System.NonSerialized] public static int playerScore;

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
    bool hovering = false;

    AudioPlayer audioPlayer;
    Resolver resolver;
    ScreenShake screenShake;

    private void Start()
    {
        state = GameState.PAUSE;
        audioPlayer = FindObjectOfType<AudioPlayer>();
        resolver = FindObjectOfType<Resolver>();
        screenShake = FindObjectOfType<ScreenShake>();
        StartCoroutine(WaitAndChangeState(GameState.DEAL, 1f));
    }

    private void Update()
    {
        opponentText.text = opponentScore.ToString();
        playerText.text = playerScore.ToString();

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
                    cardToPlay.SetTargetPos(cardPlayPos.position + new Vector3(0, 1.2f, 0), false);
                    opponentHand.Remove(cardToPlay);
                    pOpponentCard = cardToPlay;
                    StartCoroutine(WaitAndChangeState(GameState.PLAYER, delayTime));
                }
                
            break;

            case GameState.PLAYER:
                foreach (Card card in playerHand) 
                {
                    { card.Flip(true); }

                    if (!hovering) { card.SetTargetAlpha(1f); }

                    if (card.IsHovered())
                    {
                        hovering = true;

                        card.SetTargetPos(new Vector3(card.transform.position.x, playerPos.position.y + hoverAmount), false);
                        foreach (Card nonHover in playerHand) { nonHover.SetTargetAlpha(0.5f); }
                        card.SetTargetAlpha(1f);
                        // select card
                        if (Input.GetMouseButtonDown(0))
                        {
                            active = true;
                            Card cardToPlay = card;
                            cardToPlay.SetTargetPos(cardPlayPos.position + new Vector3(0, -1.2f, 0), false);
                            playerHand.Remove(cardToPlay);
                            pPlayerCard = cardToPlay;

                            pOpponentCard.Flip(true);
                            StartCoroutine(WaitAndChangeState(GameState.RESOLVE, 1f));
                            break;
                        }

                    }
                    else
                    {
                        hovering = false;
                        card.SetTargetPos(new Vector3(card.transform.position.x, playerPos.position.y), false);
                    }
                }
            break;

            case GameState.RESOLVE:
                if (!active)
                {
                    active = true;
                    resolver.Resolve(pOpponentCard, pPlayerCard);
                    StartCoroutine(WaitAndChangeState(GameState.DISCARD, 1.25f));
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

            case GameState.RESHUFFLE:
                if (!active) 
                { 
                    if (DeckManager.discardPile.Count > 0) { StartCoroutine(MoveBackToDeck()); }
                    else if (!DeckManager.shuffled) 
                    { 
                        StartCoroutine(ShuffleAnim()); 
                    }
                    else { StartCoroutine(WaitAndChangeState(GameState.DEAL, 1f)); }
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

            nextCard.SetTargetPos(newPos, true);
            handToDeal.Add(nextCard);
            if (handToDeal == playerHand) { nextCard.inHand = true; }
            DeckManager.deck.Remove(nextCard);

            audioPlayer.PlayDrawSound();
            yield return new WaitForSeconds(delayTime);
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
            discardCard.SetTargetPos(discardPos.position + new Vector3(0, stackSpacing * DeckManager.discardPile.Count), true);
            discardCard.GetComponent<SpriteRenderer>().sortingOrder = DeckManager.discardPile.Count;
            DeckManager.discardPile.Add(discardCard);
            targetList.Remove(discardCard);

            audioPlayer.PlayDrawSound();
            yield return new WaitForSeconds(delayTime);
        }

        active = false;
    }

    IEnumerator DiscardCard(Card targetCard)
    {
        active = true;

        targetCard.SetTargetPos(discardPos.position + new Vector3(0, stackSpacing * DeckManager.discardPile.Count), true);
        targetCard.GetComponent<SpriteRenderer>().sortingOrder = DeckManager.discardPile.Count;
        DeckManager.discardPile.Add(targetCard);

        audioPlayer.PlayDrawSound();
        yield return new WaitForSeconds(delayTime);

        active = false;
    }

    IEnumerator MoveBackToDeck()
    {
        active = true;
        DeckManager.shuffled = false;

        Vector3 deckPos = FindObjectOfType<DeckManager>().transform.position;

        if (DeckManager.discardPile.Count > 0)
        {           
            Card cardToMove = DeckManager.discardPile[DeckManager.discardPile.Count - 1];
            cardToMove.SetTargetPos(deckPos + new Vector3(0, stackSpacing * DeckManager.deck.Count), false);
            cardToMove.GetComponent<SpriteRenderer>().sortingOrder = DeckManager.deck.Count;
            cardToMove.Flip(false);
            DeckManager.deck.Add(cardToMove);
            DeckManager.discardPile.Remove(cardToMove);
            audioPlayer.PlayDrawSound();
            screenShake.ShakeScreen(0.06f, 0.02f);

            yield return new WaitForSeconds(0.05f);
        }

        active = false;
    }

    IEnumerator ShuffleAnim()
    {
        active = true;

        Vector3 deckPos = FindObjectOfType<DeckManager>().transform.position;

        // shuffle animation
        for (int i = 0; i < DeckManager.deck.Count; i++)
        {
            Card topCard = DeckManager.deck[DeckManager.deck.Count - 1];
            topCard.SetTargetPos(deckPos + new Vector3(0, stackSpacing * DeckManager.deck.Count), false);
            topCard.GetComponent<SpriteRenderer>().sortingOrder = DeckManager.deck.Count;
            DeckManager.deck.Remove(topCard);
            DeckManager.deck.Insert(0, topCard);
            audioPlayer.PlayDrawSound();

            yield return new WaitForSeconds(0.035f);
        }

        Shuffler.ShuffleCards(DeckManager.deck);
        DeckManager.shuffled = true;

        for (int i = 0; i < DeckManager.deck.Count; i++)
        {
            Card card = DeckManager.deck[i];
            card.SetTargetPos(deckPos + new Vector3(0, stackSpacing * i), false);
            card.GetComponent<SpriteRenderer>().sortingOrder = i;
        }

        active = false;
    }

    IEnumerator WaitAndChangeState(GameState newState, float delay)
    {
        active = true;
        yield return new WaitForSeconds(delay);
        state = newState;
        active = false;
    }
}
