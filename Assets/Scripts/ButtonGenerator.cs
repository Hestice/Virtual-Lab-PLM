using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> onClick)
    {
        button.onClick.AddListener(delegate
        {
            onClick(param);
        });
    }
}

public class ButtonGenerator : MonoBehaviour
{
    [Serializable]
    public struct Equipment
    {
        public Sprite Icon;
        public string Name;
        public string Description;
        public Animator AnimatorController; // reference to the animator controller
    }

    [SerializeField] Equipment[] allEquipments;

    [SerializeField] AnimationClip buttonAnimationClip; // reference to the animation clip
    public static int activeButtonIndex = 0; // index of the currently active button, -1 if none
    //this is the reference to get from VR and AR Scenes

    void Start()
    {
        if (transform.childCount == 0)
        {
            Debug.LogError("ButtonGenerator has no child objects!");
            return;
        }
        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject buttonInstance;

        int N = allEquipments.Length;

        for (int i = 0; i < N; i++)
        {
            buttonInstance = Instantiate(buttonTemplate, transform);
            buttonInstance.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = allEquipments[i].Icon;
            buttonInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = allEquipments[i].Name;
            buttonInstance.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = allEquipments[i].Description;

            // Add a click event listener to the button
            ButtonExtension.AddEventListener(buttonInstance.GetComponent<Button>(), i, ItemClicked);

            allEquipments[i].AnimatorController = buttonInstance.GetComponentInChildren<Animator>();
        }

        Destroy(buttonTemplate);
    }

    public Equipment[] AllEquipments
    {
        get { return allEquipments; }
    }


    void ItemClicked(int itemIndex)
    {
        Debug.Log("------------item " + itemIndex + " clicked---------------");
        Debug.Log("name " + allEquipments[itemIndex].Name);
        Debug.Log("desc " + allEquipments[itemIndex].Description);

        // Hide the options of the active button if a new button is clicked
        if (activeButtonIndex != -1 && activeButtonIndex != itemIndex)
        {
            Animator activeButtonAnimator = allEquipments[activeButtonIndex].AnimatorController;
            activeButtonAnimator.SetBool("isShown", false);
            activeButtonAnimator.SetTrigger("hideOption");
        }

        // Toggle the animation trigger for the clicked button
        Animator clickedButtonAnimator = allEquipments[itemIndex].AnimatorController;
        if (clickedButtonAnimator)
        {
            if (clickedButtonAnimator.GetBool("isShown"))
            {
                clickedButtonAnimator.SetTrigger("hideOption");
                clickedButtonAnimator.SetBool("isShown", false);
            }
            else
            {
                clickedButtonAnimator.ResetTrigger("hideOption");
                clickedButtonAnimator.SetTrigger("buttonclick");
                clickedButtonAnimator.SetBool("isShown", true);
            }

            // Update the active button index
            activeButtonIndex = itemIndex; 
        }
    }
}
