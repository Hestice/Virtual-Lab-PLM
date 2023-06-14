using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceedButtonAnimation : MonoBehaviour
{
    private Animator assessmentsAnimator;

    void Awake()
    {
        assessmentsAnimator = GameObject.Find("/Canvas/BG").GetComponent<Animator>();
    }

    public void proceedToTest(){
        assessmentsAnimator.SetTrigger("Proceed");
    }
}

