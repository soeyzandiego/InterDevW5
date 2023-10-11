using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] CardType[] cardTypes;
    [SerializeField] int deckCount;

    [System.NonSerialized] public static List<Card> deck = new List<Card>();
    [System.NonSerialized] public static List<Card> discardPile = new List<Card>();

    [System.NonSerialized] public static bool shuffled = false;

    void Start()
    {
        for (int i = 0; i < deckCount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, transform);
            newCard.name = i.ToString();
            Card newCardScript = newCard.GetComponent<Card>();
            newCardScript.SetCardType(cardTypes[i % 3]);
            deck.Add(newCardScript);
        }
        Shuffler.ShuffleCards(deck);
        shuffled = true;
        for (int i = 0; i < deck.Count; i++)
        {
            Card card = deck[i];
            card.transform.position += new Vector3(0, 0.05f * i);
            card.GetComponent<SpriteRenderer>().sortingOrder = i;
        }
    }
}
