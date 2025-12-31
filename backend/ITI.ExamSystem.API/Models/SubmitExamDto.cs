namespace ITI.ExamSystem.API.Models
{
    public class ExamAnswerDto
    {
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
    }

    public class SubmitExamDto
    {
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public List<ExamAnswerDto> Answers { get; set; } = new List<ExamAnswerDto>();
    }
}
