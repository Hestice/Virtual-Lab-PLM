using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Proyecto26;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class StudentGenerator : MonoBehaviour
{
    private struct Student
    {
        public string Number;
        public string Name;
        public string Progress;
        public string GenAvg;
        public string UserId;
        public string classCode;
    }

    private Student[] Allstudents;
    private int classCodeCounts;
    public static string ClassCode;
    private string databaseURL = "https://virtual-lab-65d18-default-rtdb.asia-southeast1.firebasedatabase.app/";
    public User LoggedInUser { get; set; }

    private int takenAssessment;
    private int totalAssessment;

    public TMP_Text facultyDetails;

    private void Awake()
    {

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
    }

    private async void Start()
    {
        // Generate buttons based on the class code counts
        await CountClassCodeOccurrencesAsync();
        // await CalculateProgress();
        // await CalculateGenAvg();
        StartCoroutine(GenerateLate());
    }


    public async void ReloadStudents(){
        GameObject[] Students = GameObject.FindGameObjectsWithTag("Students");
        foreach(GameObject Student in Students)
        GameObject.Destroy(Student);
        await CountClassCodeOccurrencesAsync();
        StartCoroutine(GenerateLate());
        facultyDetails.text = "Hey, "+UserInfoManager.localUser_Username+"! There are a total of "+Allstudents.Length+" students in the class "+ClassCode+"!";
    }

    IEnumerator GenerateLate(){
        yield return new WaitForSeconds(1f); 
        GenerateButtons();
        facultyDetails.text = "Hey, "+UserInfoManager.localUser_Username+"! There are a total of "+Allstudents.Length+" students in the class "+ClassCode+"!";

    }
    
    private async Task CountClassCodeOccurrencesAsync()
    {
        RestClient.Get(databaseURL + "assessmentScores.json").Then(response =>
        {         
            var users = JsonConvert.DeserializeObject<Dictionary<string, Student>>(response.Text);

            if (users==null){
                Debug.Log("No response");
                return;
            }

            List<Student> students = new List<Student>();
            foreach (var user in users)
            {
                if (user.Value.classCode == UserInfoManager.localUser_ClassCode)
                {
                    Student student = new Student();
                    student.Number = ""; // Set an initial value for the Number field
                    student.UserId = user.Key;
                    students.Add(student);
                }
            }
            students.Sort((x, y) => string.Compare(x.Name, y.Name));

            classCodeCounts = students.Count;
            Allstudents = students.ToArray();
            Debug.Log("Length of allStudents array = "+Allstudents.Length);

            FetchUserInformationAsync();
        });
    }

    private async Task FetchUserInformationAsync()
    {
        var fetchTasks = new List<Task>();

        for (int i = 0; i < Allstudents.Length; i++)
        {
            string userId = Allstudents[i].UserId; // Use UserId property instead of Name
            int currentIndex = i; // Capture the current value of i

            var fetchTaskCompletionSource = new TaskCompletionSource<bool>();

            RestClient.Get(databaseURL + "users/" + userId + ".json").Then(response =>
            {
                var studentJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Text);

                var student = new User(
                    email: studentJson["Email"],
                    username: studentJson["Username"],
                    firstName: studentJson["FirstName"],
                    middleName: studentJson["MiddleName"],
                    lastName: studentJson["LastName"],
                    age: int.Parse(studentJson["Age"]),
                    sex: studentJson["Sex"],
                    isFaculty: bool.Parse(studentJson["IsFaculty"]),
                    classCode: studentJson["ClassCode"]
                );

                string fullName = GenerateFullName(student);
                Allstudents[currentIndex].Name = fullName; // Use the captured currentIndex
                fetchTaskCompletionSource.SetResult(true);
            });

            fetchTasks.Add(fetchTaskCompletionSource.Task);
        }

        await Task.WhenAll(fetchTasks);
        CalculateProgress();
    }


    private string GenerateFullName(User student)
    {
        string firstName = student.FirstName;
        string middleName = student.MiddleName;
        string lastName = student.LastName;

        string middleInitial = middleName.Length > 0 ? middleName.Substring(0, 1) + "." : "";

        string fullName = firstName + " " + middleInitial + " " + lastName;
        fullName = fullName.Replace("  ", " ").Trim();
        return fullName;
    }


    private async Task CalculateProgress()
{
    var fetchTasks = new List<Task>();

    var fetchTaskCompletionSource1 = new TaskCompletionSource<bool>();

    RestClient.Get(databaseURL + "equipmentAssessments.json").Then(response =>
    {
        var equipmentAssessmentsJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Text);
        totalAssessment = equipmentAssessmentsJson.Count;

        for (int i = 0; i < Allstudents.Length; i++)
        {
            int currentIndex = i; // Capture the current value of i
            var fetchTaskCompletionSource2 = new TaskCompletionSource<bool>();

            RestClient.Get(databaseURL + "assessmentScores/" + Allstudents[currentIndex].UserId + "/equipmentAssessments.json").Then(assessmentResponse =>
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
                string grade = GetGradeFromGenAvg(genAvg);
                
                Allstudents[currentIndex].Number = Allstudents[currentIndex].Number;
                Allstudents[currentIndex].Name = Allstudents[currentIndex].Name;
                Allstudents[currentIndex].Progress = progress.ToString("P");
                Allstudents[currentIndex].GenAvg = grade;
                Allstudents[currentIndex].UserId = Allstudents[currentIndex].UserId;

                Debug.Log(progress+"|"+grade);

               
            });
            

        }


    });




    
    for (int i = 0; i < classCodeCounts; i++)
        {   
        Debug.Log("Allstudents[" + i + "]: Number=" + Allstudents[i].Number +
        ", Name=" + Allstudents[i].Name +
        ", Progress=" + Allstudents[i].Progress +
        ", GenAvg=" + Allstudents[i].GenAvg +
        ", UserId=" + Allstudents[i].UserId);
        }
    
    
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

    private void GenerateButtons()
    {

        Debug.Log("GeneratingButtons");

        if (transform.childCount == 0)
        {
            Debug.LogError("ButtonGeneratorScores has no child objects!");
            return;
        }

        GameObject studentTemplate = transform.GetChild(0).gameObject;
        GameObject studentInstance;
        studentTemplate.SetActive(true);
        
        int N = classCodeCounts;
        Debug.Log(N);

        // Sort the Allstudents array alphabetically by name
        Allstudents = Allstudents.OrderBy(student => student.Name).ToArray();

        for (int i = 0; i < N; i++)
        {   

            Debug.Log("Allstudents[" + i + "]: Number=" + Allstudents[i].Number +
        ", Name=" + Allstudents[i].Name +
        ", Progress=" + Allstudents[i].Progress +
        ", GenAvg=" + Allstudents[i].GenAvg +
        ", UserId=" + Allstudents[i].UserId);
            Allstudents[i].classCode = UserInfoManager.localUser_ClassCode;

            studentInstance = Instantiate(studentTemplate, transform);
            studentInstance.tag = "Students";
            studentInstance.SetActive(true); // Ensure the template is active
            Debug.Log("instantiated");
            // Set the student number as the button's text
            // Numbering
            studentInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (i + 1).ToString();
            Debug.Log("got number");

            // Student name
            studentInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Allstudents[i].Name;
            Debug.Log("got name");

            // Progress
            Image progressBar = studentInstance.transform.GetChild(2).GetChild(0).GetComponent<Image>();
            TextMeshProUGUI progressText = studentInstance.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
            Debug.Log(Allstudents[i].Progress);



            float progress = float.Parse(Allstudents[i].Progress.TrimEnd('%')) / 100f;
            Debug.Log("progress parsed");
            float maxWidth = 273f; // Maximum width of the progress bar

            // Adjust the width of the progress bar
            progressBar.rectTransform.sizeDelta = new Vector2(maxWidth * progress, progressBar.rectTransform.sizeDelta.y);
            Debug.Log("progress bar");

            // Update the progress text
            progressText.text = "Progress: "+ Allstudents[i].Progress;
            Debug.Log("got progress text");

            // Average
            TextMeshProUGUI genAvgText = studentInstance.transform.GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>();
            string grade = Allstudents[i].GenAvg;
            Debug.Log("got grade");

            // Set the color based on the grade
            switch (grade)
            {
                case "1":
                case "1.25":
                case "1.5":
                case "1.75":
                    Color greenColor;
                    ColorUtility.TryParseHtmlString("#04A915", out greenColor); // Custom green color
                    genAvgText.color = greenColor;
                    break;
                case "2":
                case "2.25":
                case "2.5":
                case "2.75":
                case "3":
                    Color yellowColor;
                    ColorUtility.TryParseHtmlString("#D2C549", out yellowColor); // Custom yellow color
                    genAvgText.color = yellowColor;
                    break;
                case "4":
                    Color orangeColor;
                    ColorUtility.TryParseHtmlString("#EC9C24", out orangeColor); // Custom orange color
                    genAvgText.color = orangeColor;
                    break;
                case "5":
                    Color redColor;
                    ColorUtility.TryParseHtmlString("#BB3B3B", out redColor); // Custom red color
                    genAvgText.color = redColor;
                    break;
                default:
                    genAvgText.color = Color.black;
                    break;
            }

            Debug.Log("Allstudents[" + i + "]: Number=" + Allstudents[i].Number +
        ", Name=" + Allstudents[i].Name +
        ", Progress=" + Allstudents[i].Progress +
        ", GenAvg=" + Allstudents[i].GenAvg +
        ", UserId=" + Allstudents[i].UserId);

            genAvgText.text = grade;
        }

        studentTemplate.SetActive(false);
    }
    

}
