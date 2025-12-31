namespace ITI.ExamSystem.API.Models
{
    public class ExamQuestionViewDto
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
        public int Points { get; set; }

        public List<ChoiceDto> Choices { get; set; } = new();
    }
}
