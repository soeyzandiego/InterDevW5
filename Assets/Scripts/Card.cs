using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardValue
    {
        ROCK,
        PAPER,
        SCISSORS
    };

    CardValue value;

    static float moveSpeed = 0.025f; 

    Sprite faceSprite;
    Sprite backSprite;

    SpriteRenderer sr;
    Animator anim;

    bool hovered = false;
    bool faceUp = false;
    [System.NonSerialized] public bool inHand = false;
    Vector3 targetPos;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        backSprite = sr.sprite;
        targetPos = transform.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed);
    }

    void OnMouseEnter() { hovered = true; }
    void OnMouseExit() { hovered = false; }

    public void Flip()
    {
        sr.sprite = faceSprite;
        faceUp = !faceUp;
    }

    public void SetTargetPos(Vector3 newPos)
    {
        targetPos = newPos;
        targetPos.z = 0;
    }

    public void SetCardType(CardType _cardType)
    {
        value = _cardType.value;
        faceSprite = _cardType.faceSprite;
        //anim.runtimeAnimatorController = _cardType.animController;
    }

    public CardValue GetValue() { return value; }

    public bool IsHovered() { return hovered; }
}
