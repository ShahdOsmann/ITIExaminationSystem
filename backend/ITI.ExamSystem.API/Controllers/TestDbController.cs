using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ITI.ExamSystem.API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestDbController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TestDbController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("db")]
        public IActionResult TestDatabase()
        {
            try
            {
                using SqlConnection con = new SqlConnection(
                    _config.GetConnectionString("DefaultConnection"));

                con.Open(); 
                SqlCommand cmd = new SqlCommand("SELECT TOP 1 S_Id, S_FName FROM Student", con);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string firstStudent = reader["S_FName"].ToString();
                    return Ok($"Database connected  First student: {firstStudent}");
                }

                return Ok("Database connected  But no students found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
