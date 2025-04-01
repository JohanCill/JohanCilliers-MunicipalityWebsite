using Microsoft.AspNetCore.Mvc;
using MunicipalityWebsite.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace MunicipalityWebsite.Controllers
{

    public class CitizenController : Controller
    {
        private  IConfiguration _configuration;
		public CitizenController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public IActionResult Index()
        {
			List<Citizen> citizens = new List<Citizen>
			{

			};
			citizens = GetCitizens();
			return View(citizens);
        }
        private List<Citizen> GetCitizens() 
        {
			List<Citizen> citizens = new List<Citizen>
			{

			};
			string connectionString = _configuration.GetConnectionString("DefaultConnection");

			using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
			{
				try
				{
					connection.Open();

					string sqlQuery = "SELECT * FROM Citizens"; 

					using (SqlCommand command = new SqlCommand(sqlQuery, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							// Map data to Citizen objects
							while (reader.Read())
							{
								Citizen citizen = new Citizen
								{
									CitizenID = reader.GetInt32(0),
									FullName = reader.GetString(1),
									Address = reader.GetString(2),
									PhoneNumber = reader.GetString(3),
									Email = reader.GetString(4),
									DateOfBirth = reader.GetDateTime(5),
									RegistrationDate = reader.GetDateTime(6)
								};
								citizens.Add(citizen);
							}
						}
					}
				}
				catch (Exception ex)
				{
					// Handle exceptions appropriately
					throw new Exception("Error retrieving citizens", ex);
				}
			}
			return citizens;
		}
		[HttpPost]
		public IActionResult CreateCitizen(Citizen citizen)
		{
			if (CitizenExist(citizen.Email)) 
			{
				return Content(
					"<script>" +
                    "if (confirm('This user already exists.')) {" +
					"   window.history.back();" +
					"} else {" +
					"   window.history.back();" +
					"}" +
                    "</script>",
					"text/html"
				);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				const string query = "INSERT INTO Citizens (FullName, Address, PhoneNumber, Email, DateOfBirth, RegistrationDate) VALUES (@FullName, @Address, @PhoneNumber, @Email, @DateOfBirth, GETDATE()) SELECT SCOPE_IDENTITY();";
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@FullName", citizen.FullName);
					command.Parameters.AddWithValue("@Address", citizen.Address);
					command.Parameters.AddWithValue("@PhoneNumber", citizen.PhoneNumber);
					command.Parameters.AddWithValue("@Email", citizen.Email);
					command.Parameters.AddWithValue("@DateOfBirth", (object?)citizen.DateOfBirth ?? DBNull.Value);


					try
					{
						connection.Open();
						int rowsAffected = command.ExecuteNonQuery();

						if (rowsAffected > 0)
						{
							Console.WriteLine("Citizen added successfully!"); 
							return RedirectToAction("Index");
						}
						else
						{
							Console.WriteLine("No rows affected.");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("SQL Error: " + ex.Message);
					}
					
				}
			}
			Console.WriteLine($"Received: {citizen.FullName}, {citizen.Address}, {citizen.PhoneNumber}, {citizen.Email}, {citizen.DateOfBirth}");
			return RedirectToAction("Index");
		}

		private bool CitizenExist(string Email) 
		{
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                const string query = "SELECT * FROM Citizens WHERE Email = @Email;";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    
                    command.Parameters.AddWithValue("@Email", Email);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.HasRows; // true if rows exist, false otherwise
                    }

                }
            }
        }
		
		[HttpPost]
		public IActionResult EditCitizen(Citizen citizen)
		{
			if (ModelState.IsValid)
			{
				string connectionString = _configuration.GetConnectionString("DefaultConnection");

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					const string query = "UPDATE Citizens SET FullName = @FullName, Address = @Address, PhoneNumber = @PhoneNumber, Email = @Email, DateOfBirth = @DateOfBirth WHERE CitizenID = @CitizenID";

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@CitizenID", citizen.CitizenID);
						command.Parameters.AddWithValue("@FullName", citizen.FullName);
						command.Parameters.AddWithValue("@Address", citizen.Address);
						command.Parameters.AddWithValue("@PhoneNumber", citizen.PhoneNumber);
						command.Parameters.AddWithValue("@Email", citizen.Email);
						command.Parameters.AddWithValue("@DateOfBirth", citizen.DateOfBirth ?? (object)DBNull.Value);

						try
						{
							connection.Open();
							int rowsAffected = command.ExecuteNonQuery();

							if (rowsAffected > 0)
							{
								return RedirectToAction("Index");
							}
							else
							{
								ModelState.AddModelError("", "No citizen found to update.");
							}
						}
						catch (Exception ex)
						{
							ModelState.AddModelError("", "Error occurred while updating citizen.");
							Console.WriteLine(ex.Message); // Log the error 
						}
					}
				}
			}
			return RedirectToAction("Index"); // If model is not valid, return to the Edit form with errors
		}
		public IActionResult DeleteCitizen(int CitizenID)
		{
			string connectionString = _configuration.GetConnectionString("DefaultConnection");

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				const string query = "DELETE FROM Citizens WHERE CitizenID = @CitizenID";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@CitizenID", CitizenID);

					try
					{
						connection.Open();
						int rowsAffected = command.ExecuteNonQuery();

						if (rowsAffected > 0)
						{
							return RedirectToAction("Index"); // Redirect to citizen list after deletion
						}
						else
						{
							ModelState.AddModelError("", "No citizen found with this ID.");
						}
					}
					catch (Exception ex)
					{
						ModelState.AddModelError("", "Error occurred while deleting citizen.");
						Console.WriteLine(ex.Message); // Log error
					}
				}
			}
			return RedirectToAction("Index");
		}

        public IActionResult GetDetailsCitizen(int CitizenID)
        {
            Citizen citizen = null;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string sqlQuery = "SELECT * FROM Citizens WHERE CitizenID = @CitizenID";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CitizenID", CitizenID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                citizen = new Citizen
                                {
                                    CitizenID = reader.GetInt32(0),
                                    FullName = reader.GetString(1),
                                    Address = reader.GetString(2),
                                    PhoneNumber = reader.GetString(3),
                                    Email = reader.GetString(4),
                                    DateOfBirth = reader.GetDateTime(5),
                                    RegistrationDate = reader.GetDateTime(6)
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return NotFound(); 
                }
            }

            if (citizen == null)
            {
                return NotFound(); // No citizen found with this ID
            }

            return View("GetDetailsCitizen", citizen);  // Pass the citizen object to the view
        }

    }
}
