using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using Proyecto26;
using FullSerializer;
using Newtonsoft.Json;

public class AssessmentScoringManager : MonoBehaviour
{
    public Transform AssessmentContent;
    public GameObject circleTimer;
    public TMP_Text TimerText;
    private string equipmentId;
    private int localScore;

    private string userId;
    private string classCode;
    private float timerDuration = 300f; // 5 minutes
    private float timer;
    private string databaseURL = "https://virtual-lab-65d18-default-rtdb.asia-southeast1.firebasedatabase.app/";


    public GameObject TestSubmitted;
    public GameObject TestTimeUp;
    public Transform parentTransform;
    private Animator BGanimator;

    private float fadeDuration = 0.2f; 

    void Awake()
    {
        equipmentId = ButtonGeneratorScores.equipmentAssessment;
        userId = UserInfoManager.localUser;
        classCode = StudentGenerator.ClassCode;
        BGanimator = parentTransform.GetComponent<Animator>();
    }

    void Start()
    {
    }

    public void StartTimer()
    {
        timer = timerDuration;
        InvokeRepeating("UpdateTimer", 0f, 1f);
    }


    void UpdateTimer()
    {
        timer -= 1f;

        // Calculate minutes and seconds
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        // Update TimerText with the remaining time
        if (minutes > 0)
        {
            TimerText.text = string.Format("{0}:{1:00}", minutes, seconds);
        }
        else
        {
            TimerText.text = string.Format("{0}", seconds);
        }


        // Calculate the fill amount for the circleTimer image
        float fillAmount = timer / timerDuration;
        circleTimer.GetComponent<Image>().fillAmount = fillAmount;

        // Determine the color for pulsing based on the remaining time
        Color startColor;
        Color endColor;

        if (timer <= 60f) // Last minute: pulse white and red
        {
            startColor = Color.white;
            endColor = Color.red;
        }
        else if (timer <= 180f) // After 3 minutes: pulse white and orange-like color
        {
            startColor = Color.white;
            endColor = new Color(1f, 0.5f, 0f); // Orange-like color (combination of red and yellow)
        }
        else // First 3 minutes: pulse white and green
        {
            startColor = Color.white;
            endColor = Color.green;
        }

        // Apply the pulsing color effect to the TimerText
        float t = Mathf.PingPong(Time.time, 1f) / 1f; // Pulsing duration of 1 second
        TimerText.color = Color.Lerp(startColor, endColor, t);
        circleTimer.GetComponent<Image>().color = Color.Lerp(startColor, endColor, t);

        if (timer <= 0f)
        {
            GameObject testTimeUpGO = Instantiate(TestTimeUp, parentTransform); // Instantiate the TestTimeUp prefab as a child of the parent transform (BG)
            CanvasGroup canvasGroup = testTimeUpGO.GetComponent<CanvasGroup>();
            CalculateScore();
            StartCoroutine(PostScoreData());
            CancelInvoke("UpdateTimer");
            BGanimator.SetTrigger("TestDone");
            FadeIn(canvasGroup, fadeDuration); // Fade in the TestTimeUp GameObject

        }
    }


    
    async Task FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float normalizedTime = (Time.time - startTime) / duration;
            canvasGroup.alpha = normalizedTime; // Update the alpha value of the CanvasGroup to create the fade-in effect
            await Task.Yield();
        }

        canvasGroup.alpha = 1f; // Ensure the alpha value is fully faded in at the end
    }

    public void PostResults()
    {
        GameObject testSubmitted = Instantiate(TestSubmitted, parentTransform); // Instantiate the TestTimeUp prefab as a child of the parent transform (BG)
        CanvasGroup canvasGroup = testSubmitted.GetComponent<CanvasGroup>();
        CalculateScore();
        StartCoroutine(PostScoreData());
        BGanimator.SetTrigger("TestDone");
        FadeIn(canvasGroup, fadeDuration);
        CancelInvoke("UpdateTimer");
    }

    IEnumerator PostScoreData(){
        yield return new WaitForSeconds(1.0f);
         // Post user's class code
        RestClient.Put(databaseURL + "assessmentScores/" + UserInfoManager.localUser + "/classCode.json", classCode);

        // dbReference.Child("assessmentScores").Child(userId).Child("classCode").SetValueAsync(classCode);

        // Post equipment assessment scores

        RestClient.Get(databaseURL + "assessmentScores/" + UserInfoManager.localUser + "/equipmentAssessments.json").Then(response =>
        {
            Debug.Log("Assessments Taken: " + response.Text);

            Dictionary<string, Dictionary<string, int>> assessmentData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(response.Text);

            int maxScore;
            int rawScore;

            if (assessmentData.ContainsKey(equipmentId))
            {
                maxScore = assessmentData[equipmentId]["maxScore"];
                rawScore = assessmentData[equipmentId]["rawScore"];

                if (rawScore<localScore){
                    // Add new keys and values
                    assessmentData[equipmentId]["newMaxScore"] = 5;
                    assessmentData[equipmentId]["newRawScore"] = localScore;
                }
            }
            else
            {
                maxScore = 5; // Set default maxScore value if no record exists
                rawScore = localScore; // Set default rawScore value if no record exists

                // Create a new entry with new keys and values
                Dictionary<string, int> newEntry = new Dictionary<string, int>();
                newEntry["maxScore"] = 5;
                newEntry["rawScore"] = localScore;                
                assessmentData[equipmentId] = newEntry;
            }

            // Perform additional calculations or actions with the assessment data

            // Update the assessment scores in the database
            string assessmentDataJson = JsonConvert.SerializeObject(assessmentData);
            RestClient.Put(databaseURL + "assessmentScores/" + UserInfoManager.localUser + "/equipmentAssessments.json", assessmentDataJson);
        });

        // RestClient.Put(databaseURL + "assessmentScores/" + UserInfoManager.localUser + "/equipmentAssessments/"+equipmentId+"/maxScore.json", "5");
        // RestClient.Put(databaseURL + "assessmentScores/" + UserInfoManager.localUser + "/equipmentAssessments/"+equipmentId+"/rawScore.json", localScore.ToString());
        // dbReference.Child("assessmentScores").Child(userId).Child("equipmentAssessments").Child(equipmentId).Child("maxScore").SetValueAsync(5);
        // dbReference.Child("assessmentScores").Child(userId).Child("equipmentAssessments").Child(equipmentId).Child("rawScore").SetValueAsync(localScore); 
    }

    async Task CalculateScore()
    {
        localScore = 0;

        for (int i = 0; i < AssessmentContent.childCount; i++)
        {
            Transform child = AssessmentContent.GetChild(i);

            if (child.name == "AssessmentTab_TrueOrFalse(Clone)")
            {
                string userAnswer = GetTrueOrFalseAnswer(child);
                RestClient.Get(databaseURL + "equipmentAssessments/"+equipmentId+"/questions/" + i.ToString() + "/trueOrFalse/correctAnswer.json").Then(response =>
                {
                    if (response.Text==null)
                    {
                        Debug.LogError("Failed to fetch answer");
                        return;
                    }
                    Debug.Log(response.Text);

                    var trueOrFalseCorrectAnswer = JsonConvert.DeserializeObject<string>(response.Text);
                    Debug.Log(trueOrFalseCorrectAnswer.ToString());
                    if (userAnswer == trueOrFalseCorrectAnswer.ToString())
                    {
                        localScore++;
                    }
                }).Catch(error =>
                {
                    Debug.LogError("Error during the request: " + error.Message);
                });

            }
            else if (child.name == "AssessmentTab_MultipleChoice(Clone)")
            {
                int userChoice = GetMultipleChoiceAnswer(child);

                RestClient.Get(databaseURL + "equipmentAssessments/"+equipmentId+"/questions/" + i.ToString() + "/multipleChoice/correctAnswer.json").Then(response =>
                {
                    if (response.Text==null)
                    {
                        Debug.LogError("Failed to fetch answer");
                        return;
                    }
                    Debug.Log(response.Text);
                    var multipleChoiceAnswer = JsonConvert.DeserializeObject<string>(response.Text);
                    Debug.Log(multipleChoiceAnswer.ToString());
                    if (userChoice.ToString() == multipleChoiceAnswer.ToString())
                    {
                        localScore++;
                    }
                }).Catch(error =>
                {
                    Debug.LogError("Error during the request: " + error.Message);
                });
            }
        }
    }

    string GetTrueOrFalseAnswer(Transform assessmentTab)
    {
        Toggle trueToggle = assessmentTab.Find("Container/TrueOrFalse/True").GetComponent<Toggle>();
        Toggle falseToggle = assessmentTab.Find("Container/TrueOrFalse/False").GetComponent<Toggle>();
             

        if (trueToggle.isOn)
        {
            return "true";
        }
        else if (falseToggle.isOn)
        {
            return "false";
        }

        // Return a default value if no toggle is selected
        return "";
    }

    // Task<bool> GetTrueOrFalseCorrectAnswer(int questionIndex)
    // {
    //     var taskCompletionSource = new TaskCompletionSource<bool>();
    //     bool correctAnswer;
    //     RestClient.Get(databaseURL + "equipmentAssessments/questions/" + questionIndex.ToString() + "trueOrFalse/correctAnswer.json").Then(response =>
    //     {
    //         if (response.Text==null)
    //         {
    //             Debug.LogError("Failed to fetch answer");
    //             taskCompletionSource.SetResult(false);
    //             return;
    //         }

    //         correctAnswer = JsonConvert.DeserializeObject<bool>(response.Value);

    //         taskCompletionSource.SetResult(true);
    //     }).Catch(error =>
    //     {
    //         Debug.LogError("Error during the request: " + error.Message);
    //         taskCompletionSource.SetResult(false);
    //     });

    //     return correctAnswer;
    // }




    int GetMultipleChoiceAnswer(Transform assessmentTab)
    {
        ToggleGroup toggleGroup = assessmentTab.Find("Container/MultipleChoice").GetComponent<ToggleGroup>();
        Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();

        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                return i; // Return the index of the selected toggle
            }
        }

        // Return a default value if no toggle is selected
        return -1;
    }

    // async Task<int> GetMultipleChoiceCorrectAnswer(int questionIndex)
    // {   
    //     int correctAnswer;
    //     var taskCompletionSource = new TaskCompletionSource<bool>();
    //     RestClient.Get(databaseURL + "equipmentAssessments/questions/" + questionIndex.ToString() + "multipleChoice/correctAnswer.json").Then(response =>
    //     {
    //         var taskCompletionSource = new TaskCompletionSource<bool>();
    //         if (response.Text==null)
    //         {
    //             Debug.LogError("Failed to fetch answer");
    //             taskCompletionSource.SetResult(false);
    //             return;
    //         }

    //         correctAnswer = JsonConvert.DeserializeObject<int>(response.Text);

    //         taskCompletionSource.SetResult(true);
    //     });

    //     return correctAnswer;
    // }

}
