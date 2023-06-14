using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentTriggers : MonoBehaviour
{
    private Animator ActiveEquipment_Animator;
    private int equipmentToPlace = ButtonGenerator.activeButtonIndex;
    public AudioManager audioManager;
    private string equipmentName;
    private int index = 1;

    void Awake()
    {
        if (equipmentToPlace == -1)
            equipmentToPlace = 0;
        Debug.Log("equipmentToPlace == "+equipmentToPlace);
        ActiveEquipment_Animator = GameObject.Find("ObjectToPlace").transform.GetChild(equipmentToPlace).GetComponent<Animator>();
        Transform equipmentTransform = GameObject.Find("ObjectToPlace").transform.GetChild(equipmentToPlace);
        equipmentName = equipmentTransform.gameObject.name;
        DeleteOtherChildren();
        audioManager = AudioManager.instance;
        audioManager.Play(equipmentName+"_Description");
    }

    // This function deletes all children except the one referenced by equipmentToPlace index
    void DeleteOtherChildren()
    {
        Transform parent = transform;
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            if (i != equipmentToPlace)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }
    }

    // This Function plays the next step in the procedures
    public void NextStepTrigger()
    {
        // Check to make sure that AnimationCounter is less than the number of animations
        if (index < ActiveEquipment_Animator.parameters.Length)
        {
            // Play the next animation
            Debug.Log("Step: " + index);
            ActiveEquipment_Animator.SetTrigger("Step" + index);

            if (audioManager)//audioManager !=null, .stop();
            audioManager.Play( equipmentName+"_" + index);
            index++;
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
        if (index >= 1)
        {
            Debug.Log("Step: " + index);
            if (index == 1)
            {
                index--;
                ActiveEquipment_Animator.SetTrigger("Step" + index);
                audioManager.Play(equipmentName+"_Description");
                index++;
            }
            else
            {   
                index-=2;
                ActiveEquipment_Animator.SetTrigger("Step" + index);
                if (index == 0){
                    audioManager.Play( equipmentName+"_Description");
                } else {
                    audioManager.Play( equipmentName+"_" + index);
                } 
                index++;
            }
        }
    }
    
    // This function plays all the animations one after another
    public void SkipToEnd()
    {
        if (DescriptionsHandler.currentIndex < ActiveEquipment_Animator.parameters.Length)
        {
            // Play all the animations one after another
            for (int i = DescriptionsHandler.currentIndex; i < ActiveEquipment_Animator.parameters.Length; i++)
            {
                // Play the animation
                ActiveEquipment_Animator.SetTrigger("Step" + i);
                Debug.Log("Step" + i);

                // Start a coroutine that will wait for the animation to finish.
                StartCoroutine(WaitForAnimationToFinish());
            }
        audioManager.Play( equipmentName+"_" + DescriptionsHandler.currentIndex);
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
        if (DescriptionsHandler.currentIndex >= 1)
        {
            // Play all the animations in reverse order
            for (int i = DescriptionsHandler.currentIndex; i > 0; i--)
            {
                // Play the animation
                ActiveEquipment_Animator.SetTrigger("Step" + i);
                Debug.Log("Step" + i);

                // Start a coroutine that will wait for the animation to finish.
                StartCoroutine(WaitForAnimationToFinish());
            }
            DescriptionsHandler.currentIndex = 0;
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

}
