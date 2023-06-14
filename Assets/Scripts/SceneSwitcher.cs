using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneSwitcher : MonoBehaviour
{
    private Animator canvasAnimator;
    public float transitionTime = 1f;

    private bool fromAssessment;

    void Awake(){
        canvasAnimator = GameObject.Find("/Canvas").GetComponent<Animator>();
    }
    
    public void Login(){
        StartCoroutine(SkipToLogin());
    }
    public void EqList(){
        SceneManager.LoadScene("EquipmentList");
    }
    //Load The AR on debug
    public void ARStart(){
        StartCoroutine(ARPageAnim());
    }
    public void VRStart(){
        StartCoroutine(VRPageAnim());
    }
    public void VRBack(){
        StartCoroutine(BackToEqList());
    }
    public void SignOut()
    {
        StartCoroutine(SignOutAnim());
    }
    public void SwitchToVR()
    {
        StartCoroutine(gotoVR());
    }
    public void SwitchToAR()
    {
        StartCoroutine(gotoAR());
    }
    public void ToAssessments(){
        SceneManager.LoadScene("Assessments");
    }

    public void fromAssessmentToEqList(){
        fromAssessment = true;
        EqList();
    }
 

//,LoadSceneMode.Additive makes it so the assets are added

    IEnumerator SkipToLogin(){
        canvasAnimator.SetTrigger("End");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("Login");
    }

    IEnumerator ARPageAnim(){
        canvasAnimator.SetTrigger("End");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("ARCamera");
    }

    IEnumerator VRPageAnim(){
        canvasAnimator.SetTrigger("End");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("BlankVR");    
    }

    IEnumerator BackToEqList(){
        canvasAnimator.SetTrigger("Back");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("EquipmentList");    
    }
    IEnumerator SignOutAnim()
    {
        canvasAnimator.SetTrigger("End");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("Login");
    }
    IEnumerator gotoVR()
    {
        canvasAnimator.SetTrigger("SwitchF");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("BlankVR");
    }
    IEnumerator gotoAR()
    {
        canvasAnimator.SetTrigger("SwitchF");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("ARCamera");
    }
}

