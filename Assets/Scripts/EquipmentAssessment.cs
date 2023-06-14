public class EquipmentAssessmentData
{
    public class EquipmentAssessment
    {
        public string classCode;
        public long closingDateTime;
        public long datePosted;
        public string description;
        public long dueDateTime;
        public string equipment;
        public string faculty;
        public string instructions;
        public Question[] questions;
    }

    public class Question
    {
        public string question;
        public int questionType;
        public MultipleChoice multipleChoice;
        public Identification identification;
        public Essay essay;
        public TrueOrFalse trueOrFalse;
    }

    public class MultipleChoice
    {
        public string[] options;
        public int correctOptionIndex;
    }

    public class Identification
    {
        public string correctAnswer;
    }

    public class Essay
    {
        public int maxPoints;
    }

    public class TrueOrFalse
    {
        public bool correctAnswer;
    }
    

}
