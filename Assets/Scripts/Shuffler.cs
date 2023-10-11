using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuffler : MonoBehaviour
{
    public static void ShuffleCards(List<Card> _list)
    {
        List<Card> tempList = new List<Card>();
        for (int i = 0; i < _list.Count; i++)
        {
            Card temp = _list[i];
            int randomIndex = Random.Range(i, _list.Count);
            _list[i] = _list[randomIndex];
            _list[randomIndex] = temp;
        }
    }
}
