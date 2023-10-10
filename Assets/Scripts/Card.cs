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

    static float moveSpeed = 0.035f; 

    Animator anim;

    bool hovered = false;
    bool faceUp = false;
    [System.NonSerialized] public bool inHand = false;
    Vector3 targetPos;

    void Awake()
    {
        // script execution order, anim returns null if assigned in Start
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed);
    }

    void OnMouseEnter() { hovered = true; }
    void OnMouseExit() { hovered = false; }

    public void Flip(bool _faceUp)
    {
        faceUp = _faceUp;
        anim.SetBool("faceUp", faceUp);
    }

    public void SetTargetPos(Vector3 newPos)
    {
        targetPos = newPos;
        targetPos.z = 0;
    }

    public void SetCardType(CardType _cardType)
    {
        value = _cardType.value;
        anim.runtimeAnimatorController = _cardType.animController;
    }

    public CardValue GetValue() { return value; }

    public bool IsHovered() { return hovered; }
}
