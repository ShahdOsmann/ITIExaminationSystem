using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ITI.ExamSystem.API.Models;  
using System.Data;

namespace ITI.ExamSystem.API.Controllers
{
    [ApiController]
    [Route("api/exam")]
    public class ExamController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ExamController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("generate")]
        public IActionResult GenerateExam([FromQuery] string courseName, [FromQuery] int numMCQ, [FromQuery] int numTF)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using SqlCommand cmd = new SqlCommand("GenerateExam", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CourseName", courseName);
                cmd.Parameters.AddWithValue("@NumMCQ", numMCQ);
                cmd.Parameters.AddWithValue("@NumTF", numTF);

                con.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                var questions = new List<object>();
                int currentQuestionId = -1;
                dynamic currentQuestion = null;

                while (reader.Read())
                {
                    int qId = Convert.ToInt32(reader["Q_ID"]);

                    if (qId != currentQuestionId)
                    {
                        currentQuestionId = qId;
                        currentQuestion = new
                        {
                            ExamId = Convert.ToInt32(reader["ExamId"]),
                            QuestionId = qId,
                            Content = reader["Q_Content"].ToString(),
                            Type = reader["Q_Type"].ToString(),
                            Points = Convert.ToInt32(reader["Q_Points"]),
                            Choices = new List<object>()
                        };
                        questions.Add(currentQuestion);
                    }

                    if (reader["Choice_ID"] != DBNull.Value)
                    {
                        ((List<object>)currentQuestion.Choices).Add(new
                        {
                            ChoiceId = Convert.ToInt32(reader["Choice_ID"]),
                            ChoiceContent = reader["Choice_Content"].ToString(),
                            IsCorrect = Convert.ToBoolean(reader["Is_Correct"])
                        });
                    }
                }

                return Ok(questions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

[HttpPost("submit")]
public IActionResult SubmitExam([FromBody] SubmitExamDto dto)
{
    try
    {
        using SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        con.Open();

        foreach (var answer in dto.Answers)
        {
            using SqlCommand cmd = new SqlCommand("SubmitExamAnswers", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StudentId", dto.StudentId);
            cmd.Parameters.AddWithValue("@ExamId", dto.ExamId);
            cmd.Parameters.AddWithValue("@QuestionId", answer.QuestionId);
            cmd.Parameters.AddWithValue("@ChoiceId", answer.ChoiceId);
            cmd.ExecuteNonQuery();
        }

        using SqlCommand cmdGrade = new SqlCommand("CorrectExam", con);
        cmdGrade.CommandType = CommandType.StoredProcedure;
        cmdGrade.Parameters.AddWithValue("@StudentId", dto.StudentId);
        cmdGrade.Parameters.AddWithValue("@ExamId", dto.ExamId);

        var grade = cmdGrade.ExecuteScalar();  

        return Ok(new { message = "Exam submitted successfully", grade });
    }
    catch (Exception ex)
    {
        return BadRequest(new { error = ex.Message });
    }
}
[HttpGet("correct")]
public IActionResult CorrectExam([FromQuery] int studentId, [FromQuery] int examId)
{
    try
    {
        using SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        con.Open();

        using SqlCommand cmd = new SqlCommand("CorrectExam", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@StudentId", studentId);
        cmd.Parameters.AddWithValue("@ExamId", examId);

        var grade = cmd.ExecuteScalar();  

        return Ok(new { grade });
    }
    catch (Exception ex)
    {
        return BadRequest(new { error = ex.Message });
    }
}[HttpGet("review")]
public IActionResult ReviewExam(int studentId, int examId)
{
    using SqlConnection con = new SqlConnection(
        _config.GetConnectionString("DefaultConnection"));

    using SqlCommand cmd = new SqlCommand(
        "GetExamReviewWithChoices", con);

    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("@StudentId", studentId);
    cmd.Parameters.AddWithValue("@ExamId", examId);

    con.Open();
    SqlDataReader reader = cmd.ExecuteReader();

    var questions = new Dictionary<int, dynamic>();

    while (reader.Read())
    {
        int qId = (int)reader["Q_ID"];

        if (!questions.ContainsKey(qId))
        {
            questions[qId] = new
            {
                question = reader["Q_Content"].ToString(),
                studentChoiceId = reader["StudentChoiceId"] == DBNull.Value
                    ? null
                    : (int?)reader["StudentChoiceId"],
                choices = new List<object>()
            };
        }

        questions[qId].choices.Add(new
        {
            choiceId = (int)reader["Choice_Id"],
            text = reader["Choice_Content"].ToString(),
            isCorrect = (bool)reader["Is_Correct"]
        });
    }

    return Ok(questions.Values);
}



    }
}
