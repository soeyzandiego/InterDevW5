using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resolver : MonoBehaviour
{
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip loseSound;
    [SerializeField] AudioClip tieSound;

    AudioPlayer audioPlayer;

    void Start()
    {
        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    public void Resolve(Card opponentCard, Card playerCard)
    {
        if (opponentCard.GetValue() == Card.CardValue.ROCK)
        {
            switch (playerCard.GetValue())
            {
                case Card.CardValue.ROCK:
                    Tie();
                    break;
                case Card.CardValue.PAPER:
                    Win();
                    break;
                case Card.CardValue.SCISSORS:
                    Lose();
                    break;
            }
        }
        else if (opponentCard.GetValue() == Card.CardValue.PAPER)
        {
            switch (playerCard.GetValue())
            {
                case Card.CardValue.ROCK:
                    Lose();
                    break;
                case Card.CardValue.PAPER:
                    Tie();
                    break;
                case Card.CardValue.SCISSORS:
                    Win();
                    break;
            }
        }
        else if (opponentCard.GetValue() == Card.CardValue.SCISSORS)
        {
            switch (playerCard.GetValue())
            {
                case Card.CardValue.ROCK:
                    Win();
                    break;
                case Card.CardValue.PAPER:
                    Lose();
                    break;
                case Card.CardValue.SCISSORS:
                    Tie();
                    break;
            }
        }
    }

    void Tie()
    {
        audioPlayer.PlaySound(tieSound);
    }

    void Win()
    {
        GameManager.playerScore++;
        audioPlayer.PlaySound(winSound);
    }

    void Lose()
    {
        GameManager.opponentScore++;
        audioPlayer.PlaySound(loseSound);
    }
}
