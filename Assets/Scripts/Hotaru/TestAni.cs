using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    public Animator bodyAnimator, shirtAnimator, pantsAnimator, hairAnimator;

    private float moveX;

    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");

        if (moveX == 0)
        {
            PlayState("Idle");
        }
        else if (Mathf.Abs(moveX) > 0 && Mathf.Abs(moveX) < 2)
        {
            PlayState("Walk");
        }
        else
        {
            PlayState("Run");
        }
    }

    void PlayState(string stateName)
    {
        bodyAnimator.Play(stateName);
        shirtAnimator.Play(stateName);
        pantsAnimator.Play(stateName);
        hairAnimator.Play(stateName);
    }
}
