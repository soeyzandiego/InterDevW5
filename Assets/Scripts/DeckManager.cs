using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] CardType[] cardTypes;
    [SerializeField] int deckCount;

    public static List<Card> deck = new List<Card>();

    void Start()
    {
        for (int i = 0; i < deckCount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, gameObject.transform);
            Card newCardScript = newCard.GetComponent<Card>();
            newCardScript.SetCardType(cardTypes[i % 3]);
            deck.Add(newCardScript);
        }   
    }

    public List<Card> GetDeck()
    {
        return deck;
    }
}
