using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstTimeAccessDetector : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake(){
        if (PlayerPrefs.HasKey("HasOpenedBefore"))
        {
            // User has opened the app before
            SceneManager.LoadScene("Login");
            
        }
        else
        {
            // User is opening the app for the first time
            PlayerPrefs.SetInt("HasOpenedBefore", 1);
            PlayerPrefs.Save();
        }
    }
}
