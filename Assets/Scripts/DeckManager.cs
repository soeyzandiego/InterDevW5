using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] CardType[] cardTypes;
    [SerializeField] int deckCount;

    public static List<GameObject> deck = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < deckCount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, gameObject.transform);
            Card newCardScript = newCard.GetComponent<Card>();
            newCardScript.SetCardType(cardTypes[i % 3]);
            deck.Add(newCard);
        }   
    }

    public List<GameObject> GetDeck()
    {
        return deck;
    }
}
