using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] int shakeSpeed = 2; // how many frames will a single shake last for

    float shakeAmount;
    bool shaking = false;
    int shakeFrame = 0;

    Vector3 defaultPos;

    // Start is called before the first frame update
    void Start()
    {
        defaultPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (shaking)
        {
            shakeFrame++;
            if (shakeFrame % shakeSpeed == 0)
            {
                float _x = Random.Range(-shakeAmount, shakeAmount);
                float _y = Random.Range(-shakeAmount, shakeAmount);
                transform.position = new Vector3(_x, _y, defaultPos.z);
            }
        }
        else
        {
            transform.position = defaultPos;
        }
    }

    public void ShakeScreen(float amount, float time)
    {
        shakeAmount = amount;
        shaking = true;
        StartCoroutine(WaitAndStopShake(time));
    }

    IEnumerator WaitAndStopShake(float time)
    {
        yield return new WaitForSeconds(time);
        shaking = false;
        shakeFrame = 0;
    }
}
