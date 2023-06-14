using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginPageManager : MonoBehaviour
{

    private Animator canvasAnimator;
    public static bool state; //0 for Login; 1 for Sign Up

    // Start is called before the first frame update
    void Start(){
        canvasAnimator = GameObject.Find("/Canvas").GetComponent<Animator>();
    }

    public void loginButton(){
        state = false;
        canvasAnimator.SetTrigger("LoginPressed");
    }

    public void signUpButton(){
        state = true;
        canvasAnimator.SetTrigger("SignUpPressed");
    }

    public void loginPageEnd(){
        canvasAnimator.SetTrigger("LoginEnd");
        FirebaseDB.tempUserHeader = "arizz";

    }

    public void loginBackButton(){
        Debug.Log("Pressed " + state);
        if (state)
            canvasAnimator.SetTrigger("SignUpBackPressed");
            else 
                canvasAnimator.SetTrigger("LoginBackPressed");
        
    }



}
