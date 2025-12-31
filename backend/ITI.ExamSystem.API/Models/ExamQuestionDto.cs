namespace ITI.ExamSystem.API.Models
{
    public class ExamQuestionDto
    {
        public int QuestionId { get; set; }
        public string QuestionContent { get; set; }
        public string QuestionType { get; set; }
        public int Points { get; set; }

        public int? ChoiceId { get; set; }
        public string? ChoiceContent { get; set; }
        public bool? IsCorrect { get; set; }
    }
}
