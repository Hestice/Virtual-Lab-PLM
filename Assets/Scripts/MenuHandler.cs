using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    private Animator hamburgerAnimator;
    private Animator ClassroomTabAnimator;

    // Start is called before the first frame update
    void Start()
    {
        hamburgerAnimator = GameObject.Find("/Canvas").GetComponent<Animator>();

        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "EquipmentList")
            ClassroomTabAnimator = GameObject.Find("/Canvas/BG/ClassroomTab").GetComponent <Animator>();
    }

    public void hamburgerButton()
    {
        hamburgerAnimator.SetTrigger("MenuHam");
    }

    public void pullHamburger()
    {
        hamburgerAnimator.SetTrigger("MenuReturn");
    }
    public void SettingsShow()
    {

        hamburgerAnimator.SetTrigger("Settings");
    }
    public void ScoreShow()
    {

        hamburgerAnimator.SetTrigger("Scores");
    }
    public void ReturnMenu()
    {

        hamburgerAnimator.SetTrigger("ReturnM");
    }
    public void ScoresMenu()
    {

        hamburgerAnimator.SetTrigger("ScoreM");
    }
    public void UsageShow()
    {

        hamburgerAnimator.SetTrigger("Usage");
    }
    public void UsageBack()
    {

        hamburgerAnimator.SetTrigger("UsageM");
    }
    public void ClassroomShow()
    {

        hamburgerAnimator.SetTrigger("Class");
    }
    public void ClassroomBack()
    {

        hamburgerAnimator.SetTrigger("ClassM");
    }

    public void showClassCodeAnimation()
    {

        ClassroomTabAnimator.SetTrigger("showclass");
    }
    public void showFacultyCodeAnimation()
    {

        ClassroomTabAnimator.SetTrigger("showfaculty");
    }

}