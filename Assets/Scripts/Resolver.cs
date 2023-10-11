using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resolver : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip loseSound;
    [SerializeField] AudioClip tieSound;

    [Header("Particles")]
    [SerializeField] ParticleSystem winPart;
    [SerializeField] ParticleSystem losePart;


    AudioPlayer audioPlayer;
    ScreenShake screenShake;

    void Start()
    {
        audioPlayer = FindObjectOfType<AudioPlayer>();
        screenShake = FindObjectOfType<ScreenShake>();
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
        winPart.Play();
    }

    void Lose()
    {
        GameManager.opponentScore++;
        audioPlayer.PlaySound(loseSound);
        screenShake.ShakeScreen(0.2f, 0.3f);
        losePart.Play();
    }
}
