using ITI.ExamSystem.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace ITI.ExamSystem.API.Controllers
{
    [ApiController]
    [Route("api/student")]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentController(IConfiguration config)
        {
            _config = config;
        } 
        private string HashPassword(string password)
        {
            using SHA256 sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
 
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] SignupDto dto)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                con.Open();

                int nextId;
                using (SqlCommand cmdId = new SqlCommand("SELECT ISNULL(MAX(S_Id), 0) + 1 FROM Student", con))
                {
                    nextId = (int)cmdId.ExecuteScalar();
                }

                using SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Student 
                    (S_Id, S_FName, S_LName, S_Email, S_Password, S_Age, St_Pocket_Money, S_GPA, B_Id, Track_Id) 
                    VALUES 
                    (@SId, @FName, @LName, @Email, @Password, @Age, @PocketMoney, @GPA, @BId, @TrackId)", con);

                cmd.Parameters.AddWithValue("@SId", nextId);
                cmd.Parameters.AddWithValue("@FName", dto.FName);
                cmd.Parameters.AddWithValue("@LName", dto.LName);
                cmd.Parameters.AddWithValue("@Email", dto.Email);
                cmd.Parameters.AddWithValue("@Password", HashPassword(dto.Password));  
                cmd.Parameters.AddWithValue("@Age", dto.Age);
                cmd.Parameters.AddWithValue("@PocketMoney", dto.PocketMoney);
                cmd.Parameters.AddWithValue("@GPA", dto.GPA);
                cmd.Parameters.AddWithValue("@BId", dto.BId);
                cmd.Parameters.AddWithValue("@TrackId", dto.TrackId);

                cmd.ExecuteNonQuery();
                return Ok(new { message = "Student registered successfully", studentId = nextId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        } 
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                con.Open();

                string hashedPassword = HashPassword(dto.Password);

                using SqlCommand cmd = new SqlCommand(@"
                    SELECT S_Id, S_FName, S_LName, S_Email 
                    FROM Student 
                    WHERE S_Email=@Email AND S_Password=@Password", con);

                cmd.Parameters.AddWithValue("@Email", dto.Email);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var student = new
{
    id = reader.GetInt32(0),
    fName = reader.GetString(1),
    lName = reader.GetString(2),
    email = reader.GetString(3)
};
                    return Ok(student);
                }

                return Unauthorized(new { error = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
