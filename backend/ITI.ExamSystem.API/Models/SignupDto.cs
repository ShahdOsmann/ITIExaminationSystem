namespace ITI.ExamSystem.API.Models
{
    public class SignupDto
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public decimal PocketMoney { get; set; }
        public float GPA { get; set; }
        public int BId { get; set; }
        public int TrackId { get; set; }
    }
}
