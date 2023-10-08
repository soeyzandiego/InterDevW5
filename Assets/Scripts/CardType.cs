using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Type", menuName = "New Card Type")]
public class CardType : ScriptableObject
{
    [SerializeField] public Card.CardValue value;
    [SerializeField] public Sprite faceSprite;
    [SerializeField] public RuntimeAnimatorController animController;
}
