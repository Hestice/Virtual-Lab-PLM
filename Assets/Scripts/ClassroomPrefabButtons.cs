using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;
using Proyecto26;

public class ClassroomPrefabButtons : MonoBehaviour
{
    private Animator classroomAnimations;
    private GameObject classroomTab;
    private TMP_Text classCodeSubmittedPrompt;
    
    public User LoggedInUser { get; set; }
    public GameObject classCodePrefab;
    public GameObject facultyCodePrefab;

    public TMP_InputField classCodeInput;
    public TMP_InputField facultyCodeInput;

    private string adminCode = "plmnursing";

    private string databaseURL = "https://virtual-lab-65d18-default-rtdb.asia-southeast1.firebasedatabase.app/";

    void Awake()
    {
        classroomTab = GameObject.FindGameObjectWithTag("ClassroomTab");
        if (classroomTab != null)
        {
            classroomAnimations = classroomTab.GetComponent<Animator>();
            classCodeSubmittedPrompt = GameObject.FindGameObjectWithTag("ClassCodeSubmittedPrompt").GetComponent<TMP_Text>();
        }

        LoggedInUser = new User(
            email: "",
            username: "",
            firstName: "",
            middleName: "",
            lastName: "",
            age: 0,
            sex: "",
            isFaculty: false,
            classCode: ""
        );
        RefreshData();
    }

    public void classBack()
    {
        FadeAndDeleteExistingInstance("CodeInstance");
        classroomAnimations.SetTrigger("hideclass");
    }

    public void changeClass()
    {
        ResetClassCode();
        ShowClass();
    }

    public void ShowClass()
    {
        FadeAndDeleteExistingInstance("CodeInstance");

        GameObject classCodeInstance = Instantiate(classCodePrefab, classroomTab.transform);
        classCodeInstance.name = "Class code";
        classCodeInstance.tag = "CodeInstance";
    }

    private async Task ResetClassCode()
    {
        RefreshData();
        LoggedInUser.ClassCode = "";
        StudentGenerator.ClassCode = "";
        RestClient.Put(databaseURL + "users/" + UserInfoManager.localUser + ".json", LoggedInUser);
        RefreshData();
    }


    public void FacultyDashboard()
    {
        StartCoroutine(GoToFacultyDashboard());
    }

    IEnumerator GoToFacultyDashboard()
    {
        classroomAnimations.SetTrigger("Dashboard");
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene("FacultyScreen");
    }

    public async void changeFaculty()
    {
        await SetIsFaculty(false);
        ShowFaculty();
    }

    public void ShowFaculty()
    {
        FadeAndDeleteExistingInstance("CodeInstance");
        classroomAnimations.SetTrigger("showfaculty");

        GameObject facultyCodeInstance = Instantiate(facultyCodePrefab, classroomTab.transform);
        facultyCodeInstance.name = "Faculty code";
        facultyCodeInstance.tag = "CodeInstance";
    }


    private void FadeAndDeleteExistingInstance(string tag)
    {
        GameObject[] existingInstances = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject existingInstance in existingInstances)
        {
            Destroy(existingInstance, 0.1f);
        }
    }

    public async void submitClassCode()
    {
        string classCode = classCodeInput.text;

        if (string.IsNullOrEmpty(classCode))
        {
            Debug.Log("Class code is empty.");
            return;
        }
        RefreshData();
        LoggedInUser.ClassCode = classCode;
        RestClient.Put(databaseURL + "users/" + UserInfoManager.localUser + ".json", LoggedInUser);
        Debug.Log("Class code submitted and database updated.");
        classroomAnimations.SetTrigger("ClassCodeSubmittedPrompt");
        FadeAndDeleteExistingInstance("CodeInstance");

        RefreshData();
        StudentGenerator.ClassCode = classCode;
        classCodeSubmittedPrompt.text = "You are now part of class " + classCode + "!";            
    }

    public async void submitFacultyCode()
    {
        Debug.Log(facultyCodeInput.text + " comparing " + adminCode);
        if (facultyCodeInput.text == adminCode)
        {
            await SetIsFaculty(true);
            FadeAndDeleteExistingInstance("CodeInstance");
        }
    }

    private async Task SetIsFaculty(bool isFaculty)
    {
        RefreshData();
        LoggedInUser.IsFaculty = isFaculty;
        RestClient.Put(databaseURL + "users/" + UserInfoManager.localUser + ".json", LoggedInUser);
        if(LoggedInUser.IsFaculty){
            classroomAnimations.SetTrigger("FacultyCodeAccepted");
            classroomAnimations.SetTrigger("showfaculty");
        } else {
            LoggedInUser.IsFaculty = false;
            classroomAnimations.SetTrigger("showfaculty");
        }
        RefreshData();
    }

    public void Debugger()
    {
        Debug.Log("ButtonClicked");
    }

    private void RefreshData(){
        LoggedInUser.Email = UserInfoManager.localUser_Email;
        LoggedInUser.Username = UserInfoManager.localUser_Username;
        LoggedInUser.FirstName = UserInfoManager.localUser_FirstName;
        LoggedInUser.MiddleName = UserInfoManager.localUser_MiddleName;
        LoggedInUser.LastName = UserInfoManager.localUser_LastName;
        LoggedInUser.Age = UserInfoManager.localUser_Age;
        LoggedInUser.Sex = UserInfoManager.localUser_Sex;
        LoggedInUser.IsFaculty = UserInfoManager.localUser_IsFaculty;
        LoggedInUser.ClassCode = UserInfoManager.localUser_ClassCode;
    }
}


