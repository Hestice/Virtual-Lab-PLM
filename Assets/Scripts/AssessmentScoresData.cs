using System.Collections.Generic;

public class AssessmentScoresData
{
    public Dictionary<string, AssessmentScore> assessmentScores;

    public class AssessmentScore
    {
        public string classCode;
        public Dictionary<string, EquipmentAssessment> equipmentAssessments;
    }

    public class EquipmentAssessment
    {
        public int maxScore;
        public int rawScore;
    }
}

