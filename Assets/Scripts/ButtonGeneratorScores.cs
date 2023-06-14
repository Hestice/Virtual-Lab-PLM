using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

using Proyecto26;
using FullSerializer;
using Newtonsoft.Json;


public class ButtonGeneratorScores : MonoBehaviour
{
    [Serializable]
    private struct Equipment
    {
        public string Name;
        public string Description;
        public string ClassCode;
    }
    private ButtonGenerator buttonGenerator;

    private string databaseURL = "https://virtual-lab-65d18-default-rtdb.asia-southeast1.firebasedatabase.app/";

    public static string equipmentAssessment;
    private string AssessmentClassCode = "CSC0123";

    private List<Equipment> allEquipments;
    

    void Awake()
    {
        GenerateButtonScores();
    }

    public void GenerateButtonScores()
    {
        // AssessmentClassCode = UserInfoManager.localUser_ClassCode;
        Debug.Log("classCode is " + AssessmentClassCode);
        if (transform.childCount == 0)
        {
            Debug.LogError("ButtonGeneratorScores has no child objects!");
            return;
        }

        if(AssessmentClassCode==null)
            return;

        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        // if (!buttonTemplate.activeSelf)
        //     buttonTemplate.SetActive(true);

        RestClient.Get(databaseURL + "equipmentAssessments.json").Then(response =>
        {
            var takeAssessments = JsonConvert.DeserializeObject<Dictionary<string, Equipment>>(response.Text);
            if (takeAssessments == null)
            {
                Debug.LogError("Failed to fetch equipment assessments");
                return;
            }

            allEquipments = new List<Equipment>();

            foreach (var assessment in takeAssessments)
            {
                // Check if the assessment belongs to the desired class code
                if (assessment.Value.ClassCode == AssessmentClassCode)
                {
                    Equipment equipment = new Equipment
                    {
                        Name = assessment.Key,
                        Description = assessment.Value.Description,
                        ClassCode = assessment.Value.ClassCode
                    };

                    allEquipments.Add(equipment);
                }
                
                GameObject buttonInstance = Instantiate(buttonTemplate, transform);
                int lastIndex = allEquipments.Count -1;
                buttonInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = allEquipments[lastIndex].Name;
                buttonInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = allEquipments[lastIndex].Description;

                int indexA = lastIndex;
                buttonInstance.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => OnChildButton1Click(indexA));

                int indexB = lastIndex;
                buttonInstance.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => OnChildButton2Click(indexB));
            }

           Destroy(buttonTemplate);
        }).Catch(error =>
        {
            Debug.LogError("Failed to fetch equipment assessments: " + error.Message);
        });
        StartCoroutine(CheckTakenAssessments());
    }


    IEnumerator CheckTakenAssessments()
{
    yield return new WaitForSeconds(2.0f);
    RestClient.Get(databaseURL + "assessmentScores/" + UserInfoManager.localUser + "/equipmentAssessments.json").Then(response =>
    {
        Debug.Log("Assessments Taken: " + response.Text);

        Dictionary<string, Dictionary<string, int>> assessmentData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(response.Text);

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform assessment = transform.GetChild(i);
            string assessmentName = assessment.GetChild(0).GetComponent<TextMeshProUGUI>().text;

            if (assessmentData.ContainsKey(assessmentName))
            {
                int maxScore = assessmentData[assessmentName]["maxScore"];
                int rawScore = assessmentData[assessmentName]["rawScore"];
                int newMaxScore;
                int newRawScore;

                float percentage = (float)rawScore / maxScore * 100;

                // Check if new scores exist and assign them accordingly
                if (assessmentData[assessmentName].ContainsKey("newMaxScore") && assessmentData[assessmentName].ContainsKey("newRawScore"))
                {
                    newMaxScore = assessmentData[assessmentName]["newMaxScore"];
                    newRawScore = assessmentData[assessmentName]["newRawScore"];
                }
                else
                {
                    newMaxScore = maxScore;
                    newRawScore = 0;
                }

                float newPercentage = (float)newRawScore / newMaxScore * 100;
                Debug.Log("Assessment Name: " + assessmentName);
                Debug.Log("Max Score: " + maxScore);
                Debug.Log("Raw Score: " + rawScore);
                Debug.Log("Percentage: " + percentage + "%");

                // Perform additional calculations or actions with the assessment data

                // Example: Modify child components based on assessment data
                Transform percentageText = assessment.GetChild(2).GetChild(1); // Assuming the desired TextMeshProUGUI component is the third child of the second child of the assessment
                percentageText.GetComponent<TextMeshProUGUI>().text = percentage.ToString() + "%";

                if (newRawScore > rawScore)
                    percentageText.GetComponent<TextMeshProUGUI>().text = percentage.ToString() + "%\n(" + newPercentage.ToString() + "%)";

                Transform retakeAssessment = assessment.GetChild(4);
                if (maxScore == rawScore || newMaxScore == newRawScore)
                    Destroy(retakeAssessment.gameObject);

                Transform firstAssessment = assessment.GetChild(5); // Assuming you want to remove the fifth child component of the assessment
                Destroy(firstAssessment.gameObject);
            }
        }
    });
}


    


    private void OnChildButton1Click(int index)
    {
        Debug.Log("Child button clicked at index: " + index);
        equipmentAssessment = allEquipments[index].Name;
    }

    private void OnChildButton2Click(int index)
    {
        Debug.Log("Child button clicked at index: " + index);
        equipmentAssessment = allEquipments[index].Name;
    }
}