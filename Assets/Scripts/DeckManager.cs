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

    void Start()
    {
        for (int i = 0; i < deckCount; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, transform);
            newCard.transform.position += new Vector3(0, 0.05f * i);    
            newCard.GetComponent<SpriteRenderer>().sortingOrder = i;
            Card newCardScript = newCard.GetComponent<Card>();
            newCardScript.SetCardType(cardTypes[i % 3]);
            deck.Add(newCardScript);
        }   
    }
}
