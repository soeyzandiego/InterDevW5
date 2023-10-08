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

    static float moveSpeed = 0.04f; 

    Sprite faceSprite;
    Sprite backSprite;

    SpriteRenderer sr;
    Animator anim;

    bool mouseOver = false;
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
        if (mouseOver) { sr.sprite = faceSprite; }

        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed);
    }

    void OnMouseDown()
    {
        mouseOver = true;   
    }

    void Flip()
    {

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
}
