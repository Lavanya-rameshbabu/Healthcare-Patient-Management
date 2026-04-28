using HealthcareApp1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
public class PatientController : Controller
{
    string connStr = "Server=localhost\\SQLEXPRESS;Database=HealthcareDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Patient p)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string query = "INSERT INTO Patients (FirstName, LastName, DOB, Gender, Phone, Email, Address) VALUES (@FirstName, @LastName, @DOB, @Gender, @Phone, @Email, @Address)";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@FirstName", p.FirstName);
            cmd.Parameters.AddWithValue("@LastName", p.LastName);
            cmd.Parameters.AddWithValue("@DOB", p.DOB);
            cmd.Parameters.AddWithValue("@Gender", p.Gender);
            cmd.Parameters.AddWithValue("@Phone", p.Phone);
            cmd.Parameters.AddWithValue("@Email", p.Email);
            cmd.Parameters.AddWithValue("@Address", p.Address);

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        TempData["Success"] = "Patient added successfully!";
        return RedirectToAction("Create");
    }
    public IActionResult Index(string search, string gender, int page = 1)
    {
        int pageSize = 5;
        int offset = (page - 1) * pageSize;
        int totalRecords = 0;

        using (SqlConnection con = new SqlConnection(connStr))
        {
            string countQuery = @"SELECT COUNT(*) FROM Patients 
                          WHERE (FirstName LIKE @search OR LastName LIKE @search)" +
                                  (string.IsNullOrEmpty(gender) ? "" : " AND Gender = @gender");

            SqlCommand countCmd = new SqlCommand(countQuery, con);
            countCmd.Parameters.AddWithValue("@search", "%" + (search ?? "") + "%");

            if (!string.IsNullOrEmpty(gender))
            {
                countCmd.Parameters.AddWithValue("@gender", gender);
            }

            con.Open();
            totalRecords = (int)countCmd.ExecuteScalar();
            con.Close();
        }
        List<Patient> patients = new List<Patient>();

        using (SqlConnection con = new SqlConnection(connStr))
        {
            string query = @"SELECT * FROM Patients 
                         WHERE (FirstName LIKE @search OR LastName LIKE @search)
                         " + (string.IsNullOrEmpty(gender) ? "" : " AND Gender = @gender") +
                             " ORDER BY PatientID OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@search", "%" + (search ?? "") + "%");
            cmd.Parameters.AddWithValue("@offset", offset);
            cmd.Parameters.AddWithValue("@pageSize", pageSize);

            if (!string.IsNullOrEmpty(gender))
            {
                cmd.Parameters.AddWithValue("@gender", gender);
            }

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                patients.Add(new Patient
                {
                    PatientID = (int)reader["PatientID"],
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    Gender = reader["Gender"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    Email = reader["Email"].ToString(),
                    Address = reader["Address"].ToString()
                });
            }
        }

        ViewBag.TotalRecords = totalRecords;
        ViewBag.PageSize = pageSize;
        ViewBag.Page = page;
        return View(patients);
    }
    public IActionResult Edit(int id)
    {
        Patient p = new Patient();

        using (SqlConnection con = new SqlConnection(connStr))
        {
            string query = "SELECT * FROM Patients WHERE PatientID=@id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                p.PatientID = (int)reader["PatientID"];
                p.FirstName = reader["FirstName"].ToString();
                p.LastName = reader["LastName"].ToString();
                p.Gender = reader["Gender"].ToString();
                p.Phone = reader["Phone"].ToString();
                p.Email = reader["Email"].ToString();
                p.Address = reader["Address"].ToString();
            }
        }

        return View(p);
    }
    [HttpPost]
    public IActionResult Edit(Patient p)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string query = "UPDATE Patients SET FirstName=@FirstName, LastName=@LastName, Gender=@Gender, Phone=@Phone, Email=@Email, Address=@Address WHERE PatientID=@PatientID";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@FirstName", p.FirstName);
            cmd.Parameters.AddWithValue("@LastName", p.LastName);
            cmd.Parameters.AddWithValue("@Gender", p.Gender);
            cmd.Parameters.AddWithValue("@Phone", p.Phone);
            cmd.Parameters.AddWithValue("@Email", p.Email);
            cmd.Parameters.AddWithValue("@Address", p.Address);
            cmd.Parameters.AddWithValue("@PatientID", p.PatientID);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        return RedirectToAction("Index");
    }
    public IActionResult Delete(int id)
    {
        using (SqlConnection con = new SqlConnection(connStr))
        {
            string query = "DELETE FROM Patients WHERE PatientID=@id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", id);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        return RedirectToAction("Index");
    }
}

