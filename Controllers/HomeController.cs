using HealthcareApp1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

namespace HealthcareApp1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            int patientCount = 0;

            string connStr = "Server=localhost\\SQLEXPRESS;Database=HealthcareDB;Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT COUNT(*) FROM Patients";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                patientCount = (int)cmd.ExecuteScalar();
            }

            ViewBag.PatientCount = patientCount;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
