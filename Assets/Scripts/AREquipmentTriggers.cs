using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AREquipmentTriggers : MonoBehaviour
{
    private Animator ActiveEquipment_Animator;
    private GameObject ActiveEquipment;
    private int AnimationCounter = 0;
    public AudioManager audioManager;
    private int equipmentToPlace = ButtonGenerator.activeButtonIndex;
    private string equipmentName;

    void Start()
    {
        if (equipmentToPlace == -1)
            equipmentToPlace = 0;
        Debug.Log("equipmentToPlace == "+equipmentToPlace);
        audioManager = AudioManager.instance;
        audioManager.Play(equipmentName+"_Description");
    }

    void OnEnable(){
        //Find Object by spawn tag
        ActiveEquipment_Animator = GameObject.FindWithTag("SpawnedEquipment").transform.GetComponent<Animator>();
        ActiveEquipment = GameObject.FindWithTag("SpawnedEquipment");
        Transform equipmentTransform = GameObject.FindWithTag("SpawnedEquipment").transform;
        equipmentName = equipmentTransform.gameObject.name;
        equipmentName = equipmentName.Replace("(Clone)", "");
        Debug.Log(equipmentName);
    }

    // This Function plays the next step in the procedures
    public void NextStepTrigger()
    {
        // Check to make sure that AnimationCounter is less than the number of animations
        if (AnimationCounter < ActiveEquipment_Animator.parameters.Length)
        {
            // Play the next animation
            AnimationCounter += 1;
            Debug.Log("Step: " + AnimationCounter);
            ActiveEquipment_Animator.SetTrigger("Step" + AnimationCounter);

            if (audioManager)//audioManager !=null, .stop();
            audioManager.Play( equipmentName+"_" + AnimationCounter);
        }
        else
        {
            // There are no more animations to play
            Debug.Log("There are no more animations to play");
        }
    }

    // This function takes the procedure to the previous step
    public void PreviousStepTrigger()
    {
        if (AnimationCounter >= 1)
        {
            Debug.Log("Step: " + AnimationCounter);
            if (AnimationCounter == 1)
            {
                AnimationCounter -= 1;
                ActiveEquipment_Animator.SetTrigger("Step" + AnimationCounter);
                audioManager.Play(equipmentName+"_Description");
            }
            else
            {   
                AnimationCounter -= 1;
                ActiveEquipment_Animator.SetTrigger("Step" + AnimationCounter);
                audioManager.Play( equipmentName+"_" + AnimationCounter);
            }
        }
    }
    
    // This function plays all the animations one after another
    public void SkipToEnd()
    {
        if (AnimationCounter < ActiveEquipment_Animator.parameters.Length)
        {
            AnimationCounter+=1;
            // Play all the animations one after another
            for (int i = AnimationCounter; i < ActiveEquipment_Animator.parameters.Length; i++)
            {
                // Play the animation
                ActiveEquipment_Animator.SetTrigger("Step" + i);
                Debug.Log("Step" + i);

                // Start a coroutine that will wait for the animation to finish.
                StartCoroutine(WaitForAnimationToFinish());
                AnimationCounter = i;
            }
        audioManager.Play( equipmentName+"_" + AnimationCounter);
        }
        else
        {
            // There are no more animations to play
            Debug.Log("There are no more animations to play");
        }
    }

    // This function plays all the animations in reverse order
    public void BackToStart()
    {
        if (AnimationCounter >= 1)
        {
            AnimationCounter-=1;
            // Play all the animations in reverse order
            for (int i = AnimationCounter; i > 0; i--)
            {
                // Play the animation
                ActiveEquipment_Animator.SetTrigger("Step" + i);
                Debug.Log("Step" + i);

                // Start a coroutine that will wait for the animation to finish.
                StartCoroutine(WaitForAnimationToFinish());
            }
            AnimationCounter = 0;
            audioManager.Play( equipmentName+"_Description");
        }
        
    }


    IEnumerator WaitForAnimationToFinish()
    {
        while (ActiveEquipment_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
    }

    //Ending State
    //Open up the go back button
    //Activate the Take test link. or just show a redirecting button to the take test.
    //Also show if the user has already taken the test or not.
    // 
   
}
