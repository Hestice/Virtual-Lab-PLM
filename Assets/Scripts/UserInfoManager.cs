using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Proyecto26; 
using UnityEngine.UI;
using System.Threading.Tasks;

public class UserInfoManager : MonoBehaviour
{
    [Header("Local User Info")]
    private string thisUserHeader;
    public TMP_Text userHeader;
    public User LoggedInUser { get; set; }

    [Space]
    [Header("Class Code Field")]
    public TMP_InputField classCodeInput;
    public GameObject classCodePrefab;
    public GameObject existingClassPrefab;

    [Space]
    [Header("Faculty Code Field")]
    public TMP_InputField facultyCodeInput;
    public GameObject facultyCodePrefab;
    public GameObject existingFacultyPrefab;

    [Space]
    [Header("Classroom Tab")]
    public GameObject classroomTab;

    [Space]
    [Header("ScoresTab")]
    public GameObject scoresTabManager;

    [Space]
    [Header("Profile")]
    public GameObject userIcon;
    public TMP_Text greeting;
    public Sprite maleUser;
    public Sprite femaleUser;
    public Sprite maleFaculty;
    public Sprite femaleFaculty;

    [Header("Personal Info")]
    public TMP_Text profileFullName;
    public TMP_Text profileEmail;
    public TMP_Text profileUserID;

    [Space]
    [Header("Bar")]
    public GameObject progressBar;
    public TMP_Text progressPercentage;
    public GameObject scoreBar;
    public TMP_Text gradeWeight;
    public TMP_Text gradePercentage;

    [Space]
    [Header("Class Info")]
    public TMP_Text profileClassCode;
    public TMP_Text profileRole;

    [Space]
    [Header("Class Info")]
    public TMP_Text facultyName;
    // public TMP_Text facultyEmail;

    public static string localUser;
    public static string localUser_Email;
    public static string localUser_Username;
    public static string localUser_FirstName;
    public static string localUser_MiddleName;
    public static string localUser_LastName;
    public static int localUser_Age;
    public static string localUser_Sex;
    public static bool localUser_IsFaculty;
    public static string localUser_ClassCode;
    private string localUser_Progress;
    private string localUser_Score;
    private string localUser_PercentageScore;
    private int takenAssessment;
    private int totalAssessment;
    

    private string databaseURL = "https://virtual-lab-65d18-default-rtdb.asia-southeast1.firebasedatabase.app/";

    // Start is called before the first frame update
    void Awake()
    {
        //initialized Local User Info. Freshly Created user
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

        // Store tempUserHeader to thisUserHeader
        thisUserHeader = FirebaseDB.tempUserHeader;
        if(thisUserHeader==null)
            thisUserHeader = "custillano";
        LoggedInUser.Username = thisUserHeader;
        GetUserID(thisUserHeader, (userId) =>
        {
            if (userId != null)
            {
                localUser = userId;
                Debug.Log("localUser: "+localUser);
                RetrieveUserData();
                // Store the retrieved user ID in LoggedInUser or use it as needed
            }
            else
            {
                Debug.Log("User ID not found.");
                // Handle the case when the user ID is not found
            }
        });
    }

    void Start()
    {
        if (userHeader != null)
        {
            //fallback for debugger mode
            userHeader.text = "Hi, " + LoggedInUser.Username + "!";
            UpdateProfile();
        }
    }

    private void UpdateProfile(){

        //if sex is male, change userIcon image component to maleUser.
        //female, change userIcon image component to femaleUser.
        //if male and faculty is true, change to maleFaculty,
        //if female and faculty is true, change to femaleFaculty.
        if ((localUser_Sex == "Male")&&(localUser_IsFaculty)){
            userIcon.GetComponent<Image>().sprite = maleFaculty;

        } else if ((localUser_Sex == "Female")&&(localUser_IsFaculty)){
            userIcon.GetComponent<Image>().sprite = femaleFaculty;

        } else if ((localUser_Sex == "Male")&&(!localUser_IsFaculty)){
            userIcon.GetComponent<Image>().sprite = maleUser;

        } else if ((localUser_Sex == "Female")&&(!localUser_IsFaculty)){
            userIcon.GetComponent<Image>().sprite = femaleUser;
        } 
        
        //update greeting to "Hi, [username]!" 
        greeting.text = "Hi, "+localUser_Username+"!";

        //update profileFullName to "[Last name], [First name Middle Name (but get the initial letter only)]."
       profileFullName.text = localUser_LastName + ", " + localUser_FirstName + " " + localUser_MiddleName[0].ToString() + ".";

        //update profileEmail to email
        profileEmail.text = localUser_Email;
        //udpate profileUserID to userID
        profileUserID.text = localUser;

        CalculateProgress();
        //update progressBar, find out the total equipmentAssessments associated with the classCode, divide that to how many equipmentAssessment under assessmentScores
        float progress = float.Parse(localUser_Progress.TrimEnd('%')) / 100f;
            Debug.Log("progress parsed");
        float gradePercent = float.Parse(localUser_PercentageScore.TrimEnd('%'));
            progressBar.GetComponent<Image>().fillAmount = progress;
            scoreBar.GetComponent<Image>().fillAmount = gradePercent;
            
            Debug.Log("progress bar");

            // Update the progress text
            progressPercentage.text = localUser_Progress;
            Debug.Log("got progress text");

            float percentTemp = float.Parse(localUser_PercentageScore) * 100f;
            gradePercentage.text = percentTemp.ToString("0") + "%";
            string grade = localUser_Score;
            Debug.Log("got grade");
        
            // Set the color based on the grade
            switch (localUser_Score)
            {
                case "1":
                case "1.25":
                case "1.5":
                case "1.75":
                    Color greenColor;
                    ColorUtility.TryParseHtmlString("#04A915", out greenColor); // Custom green color
                    gradeWeight.color = greenColor;
                    break;
                case "2":
                case "2.25":
                case "2.5":
                case "2.75":
                case "3":
                    Color yellowColor;
                    ColorUtility.TryParseHtmlString("#D2C549", out yellowColor); // Custom yellow color
                    gradeWeight.color = yellowColor;
                    break;
                case "4":
                    Color orangeColor;
                    ColorUtility.TryParseHtmlString("#EC9C24", out orangeColor); // Custom orange color
                    gradeWeight.color = orangeColor;
                    break;
                case "5":
                    Color redColor;
                    ColorUtility.TryParseHtmlString("#BB3B3B", out redColor); // Custom red color
                    gradeWeight.color = redColor;
                    break;
                default:
                    gradeWeight.color = Color.black;
                    break;
            }

            gradeWeight.text = grade;

        //use the progress to fill the image's fill amount

        //also update the scoreBar, under assessmentScores/[userid]/equipmentAssessments/maxScore and rawScore
            //total the maxScores and total the rawScores
            //divide the total of rawScore to the total maxScore to get the average.
            

        profileClassCode.text = localUser_ClassCode;
        //update the ClassCode text and UserID text
        //update the   Role if IsFaculty is true, set the role to faculty. Student if not
        if(localUser_IsFaculty)
            profileRole.text = "Faculty";
            else
            profileRole.text = "Student";

        GetAllFaculty();
         //For these, there might be multiple values, so just format with enter for each.
            //Update the FullName by finding how many IsFaculty is true on the users that have the localUSer_ClassCode, Form the Full Name by Concatenating First Name + Middle Name (Only get the first letter). + LAst Name
            //Also Update the Email the same way

    
    }

    private void GetUserID(string username, Action<string> callback)
    {
        RestClient.Get(databaseURL + "users.json").Then(response =>
        {
            var users = JsonConvert.DeserializeObject<Dictionary<string, User>>(response.Text);
            foreach (var user in users)
            {
                if (user.Value.Username == username)
                {
                    callback(user.Key); // Pass the user ID to the callback
                    return;
                }
            }
            // No user found with the given username
            callback(null);
        });
    }

    private async Task CalculateProgress()
{

    RestClient.Get(databaseURL + "equipmentAssessments.json").Then(response =>
    {
        var equipmentAssessmentsJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Text);
        totalAssessment = equipmentAssessmentsJson.Count;
        
        var fetchTaskCompletionSource2 = new TaskCompletionSource<bool>();

        RestClient.Get(databaseURL + "assessmentScores/" +localUser+ "/equipmentAssessments.json").Then(assessmentResponse =>
        {
            // Debug.Log("Assessment Scores Response Text: " + assessmentResponse.Text);
            var assessmentScoresJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(assessmentResponse.Text);
            if (assessmentScoresJson != null)
            {
                takenAssessment = assessmentScoresJson.Count;
            }
            float progress = (float)takenAssessment / totalAssessment;

            // Calculate GenAvg
            int totalRawScore = 0;
            int totalMaxScore = 0;

            if (assessmentScoresJson != null)
            {
                foreach (var equipmentScore in assessmentScoresJson.Values)
                {
                    if (equipmentScore is JObject equipment)
                    {
                        int rawScore = (int)equipment["rawScore"];
                        int maxScore = (int)equipment["maxScore"];

                        totalRawScore += rawScore;
                        totalMaxScore += maxScore;
                    }
                }
            }

            float genAvg = totalMaxScore > 0 ? (float)totalRawScore / totalMaxScore : 0;
            localUser_PercentageScore = genAvg.ToString();
            string grade = GetGradeFromGenAvg(genAvg);
            
            localUser_Progress = progress.ToString("P");
            localUser_Score = grade;
            Debug.Log(progress+"|"+grade); 
        });

    });
}

private string GetGradeFromGenAvg(float genAvg)
    {
        if (genAvg >= 1)
            return "1";
        else if (genAvg >= 0.95)
            return "1.25";
        else if (genAvg >= 0.9)
            return "1.5";
        else if (genAvg >= 0.85)
            return "1.75";
        else if (genAvg >= 0.8)
            return "2";
        else if (genAvg >= 0.75)
            return "2.25";
        else if (genAvg >= 0.7)
            return "2.5";
        else if (genAvg >= 0.65)
            return "2.75";
        else if (genAvg >= 0.6)
            return "3";
        else if (genAvg >= 0)
            return "4";
        else
            return "5";
    }

    public void GetAllFaculty()
{
    RestClient.Get(databaseURL + "users.json").Then(response =>
    {
        // Clear the existing faculty names
        facultyName.text = "";

        // Parse the response data as a JSON object
        JObject jsonResponse = JObject.Parse(response.Text);

        // Iterate over each user entry in the JSON object
        foreach (KeyValuePair<string, JToken> userEntry in jsonResponse)
        {
            // Access the user data as a JSON object
            JObject userData = (JObject)userEntry.Value;

            // Check if the user is faculty (assuming the "IsFaculty" field is present)
            if (userData["IsFaculty"] != null && (bool)userData["IsFaculty"])
            {
                // Check if the user's class code matches the localUser_ClassCode
                if (userData["ClassCode"] != null && (string)userData["ClassCode"] == localUser_ClassCode)
                {
                    // Update the faculty name and email
                    string firstName = (string)userData["FirstName"];
                    string middleName = (string)userData["MiddleName"];
                    string lastName = (string)userData["LastName"];
                    string email = (string)userData["Email"];

                    // Form the full name by concatenating the first name, middle initial, and last name
                    string fullName = firstName + " " + (middleName.Length > 0 ? middleName[0] + ". " : "") + lastName;

                    // Append the faculty name and email to the facultyName text with appropriate formatting
                    facultyName.text += fullName + "\nEmail: " + email + "\n\n";
                }
            }
        }
    });
}


    public void RetrieveUserData()
    {
        RestClient.Get(databaseURL + "users/" + localUser + ".json").Then(response =>
        {
            if (!string.IsNullOrEmpty(response.Text))
            {
                // Deserialize the user data from the response
                var userData = JsonConvert.DeserializeObject<User>(response.Text);
                Debug.Log(response.Text);
                if (userData != null)
                {
                    // Update the LoggedInUser object with the retrieved data
                    localUser_Email = userData.Email;
                    localUser_Username = userData.Username;
                    localUser_FirstName = userData.FirstName;
                    localUser_MiddleName = userData.MiddleName;
                    localUser_LastName = userData.LastName;
                    localUser_Age = userData.Age;
                    localUser_Sex = userData.Sex;
                    localUser_IsFaculty = userData.IsFaculty;
                    localUser_ClassCode = userData.ClassCode;
                    StudentGenerator.ClassCode = localUser_ClassCode;
                    UpdateProfile();

                    // Debug logs
                    // Debug.Log("localUser_Email: " + localUser_Email);
                    // Debug.Log("localUser_Username: " + localUser_Username);
                    // Debug.Log("localUser_FirstName: " + localUser_FirstName);
                    // Debug.Log("localUser_MiddleName: " + localUser_MiddleName);
                    // Debug.Log("localUser_LastName: " + localUser_LastName);
                    // Debug.Log("localUser_Age: " + localUser_Age);
                    // Debug.Log("localUser_Sex: " + localUser_Sex);
                    // Debug.Log("localUser_ClassCode: " + localUser_ClassCode);

                    // ButtonGeneratorScores content = scoresTabManager.AddComponent<ButtonGeneratorScores>();
                    // content.GenerateButtonScores();
                    // Proceed with any additional logic or UI updates
                }
                else
                {
                    Debug.Log("Failed to deserialize user data.");
                }
            }
            else
            {
                Debug.Log("User data not found in the database.");
            }
        });
    }


    public void ShowClass()
    {
        RetrieveUserData();
        if (localUser_ClassCode == "")
        {
            Debug.Log("No class code found for the logged-in user.");

            // Fade out and delete existing faculty code instance if it exists
            FadeAndDeleteExistingInstance("CodeInstance");

            GameObject classCodeInstance = Instantiate(classCodePrefab, classroomTab.transform);
            classCodeInstance.name = "Class code";

            // Set the tag for the new class code instance
            classCodeInstance.tag = "CodeInstance";

            // Get the CanvasGroup component of the instantiated class code prefab
            CanvasGroup canvasGroup = classCodeInstance.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, 0.3f));
            }
        }
        else
        {
            Debug.Log("Class code of the logged-in user: " + localUser_ClassCode);

            // Fade out and delete existing class code instance if it exists
            FadeAndDeleteExistingInstance("CodeInstance");

            GameObject existingClassInstance = Instantiate(existingClassPrefab, classroomTab.transform);
            existingClassInstance.name = "Class code";
            CanvasGroup canvasGroup = existingClassInstance.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, 0.3f));
            }

            // Set the tag for the new existing class instance
            existingClassInstance.tag = "CodeInstance";
        }
    }

    public void ShowFaculty()
    {
        RetrieveUserData();
        if (localUser_IsFaculty)
        {
            Debug.Log("The logged-in user is a faculty member.");

            // Fade out and delete existing faculty code instance if it exists
            FadeAndDeleteExistingInstance("CodeInstance");

            GameObject existingFacultyInstance = Instantiate(existingFacultyPrefab, classroomTab.transform);
            existingFacultyInstance.name = "Faculty Code";
            CanvasGroup canvasGroup = existingFacultyInstance.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, 0.3f));
            }

            // Set the tag for the new existing faculty instance
            existingFacultyInstance.tag = "CodeInstance";
        }
        else
        {
            Debug.Log("The logged-in user is not a faculty member.");

            // Fade out and delete existing class code instance if it exists
            FadeAndDeleteExistingInstance("CodeInstance");

            GameObject facultyCodeInstance = Instantiate(facultyCodePrefab, classroomTab.transform);
            facultyCodeInstance.name = "Faculty Code";

            // Set the tag for the new faculty code instance
            facultyCodeInstance.tag = "CodeInstance";

            // Get the CanvasGroup component of the instantiated faculty code prefab
            CanvasGroup canvasGroup = facultyCodeInstance.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, 0.3f));
            }
        }
    }

    private void FadeAndDeleteExistingInstance(string tag)
    {
        GameObject[] existingInstances = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject existingInstance in existingInstances)
        {
            CanvasGroup canvasGroup = existingInstance.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, 0.3f));
            }

            Destroy(existingInstance, 0.1f);
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float targetAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float normalizedTime = elapsedTime / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the target alpha is reached
        canvasGroup.alpha = targetAlpha;
    }
}
