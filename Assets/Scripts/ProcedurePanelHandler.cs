using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ProcedurePanelHandler : MonoBehaviour
{
    private Animator procedureAnimator;
    private Animator ViewModeSwitch;
    private Button procedureButton;
    

    // Check which Scene is loaded, 
    // Initializes the Animator Controller
    void Start(){
        CheckLoadedScene();
    }

    void CheckLoadedScene(){
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "BlankVR")
        {
            Debug.Log("BlankVR scene is loaded.");
            ViewModeSwitch = GameObject.Find("Canvas/BG/VRMain/VRScreenCover/ViewModeSelection").GetComponent<Animator>();
        }

        procedureButton = GameObject.Find("Canvas/BG/VRMain/Procedure").GetComponent<Button>();
        //This directory should be different for AR and VR, but it will be implemented with the same VRMain for both because no time to create a separate.
        procedureAnimator = GameObject.Find("Canvas/BG").GetComponent<Animator>();
    }

    //This function Displays the other buttons for the procedure panel.
    public void ProcedureButtons(){
        procedureAnimator.SetTrigger("Procedures");
        procedureButton.enabled = false;
    }

    public void ViewMode(){
       if (CineTouch._isFirstPersonView==false)
            ViewModeSwitch.SetTrigger("FirstPersonView");
        else
            ViewModeSwitch.SetTrigger("OrbitView");

    }
}
