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

    static float moveSpeed = 0.045f;
    static float tweenAmount = 1.2f;

    Animator anim;

    bool hovered = false;
    bool faceUp = false;
    bool tweening = false;
    [System.NonSerialized] public bool inHand = false;

    Vector3 tweenStartPos;
    Vector3 targetPos;
    float targetAlpha = 1;

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

        Color tempColor = GetComponent<SpriteRenderer>().color;
        tempColor.a = Mathf.Lerp(tempColor.a, targetAlpha, 0.1f);
        GetComponent<SpriteRenderer>().color = tempColor;

        if (Vector3.Distance(transform.position, targetPos) < 0.01f) { tweening = false; }
        if (tweening)
        {
            float halfwayPoint = (tweenStartPos.x + targetPos.x) / 2;
            float xScaleAdd;
            if (transform.position.x < halfwayPoint) { xScaleAdd = Mathf.Abs(transform.position.x - tweenStartPos.x) * tweenAmount; }
            else { xScaleAdd = Mathf.Abs(targetPos.x - transform.position.x) * tweenAmount; }

            Vector3 tempScale = transform.localScale;
            tempScale.x = 1 + xScaleAdd;
            tempScale.y = 1 - (xScaleAdd / 4);
            transform.localScale = tempScale;
        }
    }

    void OnMouseEnter() { hovered = true; }
    void OnMouseExit() { hovered = false; }

    public void Flip(bool _faceUp)
    {
        faceUp = _faceUp;
        anim.SetBool("faceUp", faceUp);
    }

    public void SetTargetPos(Vector3 newPos, bool tween)
    {
        tweening = tween;
        tweenStartPos = transform.position;

        targetPos = newPos;
        targetPos.z = 0;
    }

    public void SetCardType(CardType _cardType)
    {
        value = _cardType.value;
        anim.runtimeAnimatorController = _cardType.animController;
    }

    public void SetTargetAlpha(float newAlpha)
    {
        targetAlpha = newAlpha;
    }

    public CardValue GetValue() { return value; }

    public bool IsHovered() { return hovered; }
}
