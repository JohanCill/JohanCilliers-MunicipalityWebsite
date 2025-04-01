using Microsoft.AspNetCore.Mvc;
using MunicipalityWebsite.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace MunicipalityWebsite.Controllers
{
	public class StaffController : Controller
	{
		private IConfiguration _configuration;
		public StaffController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public IActionResult Index()
		{
			List<Staff> Staff = new List<Staff>
			{

			};
			Staff = GetStaff();
			return View(Staff); 
		}
		private List<Staff> GetStaff()
		{
			List<Staff> Staff = new List<Staff>
			{

			};
			string connectionString = _configuration.GetConnectionString("DefaultConnection");

			using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
			{
				try
				{
					connection.Open();

					string sqlQuery = "SELECT * FROM Staff";

					using (SqlCommand command = new SqlCommand(sqlQuery, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							// Map data to Staff objects
							while (reader.Read())
							{
								Staff staff = new Staff
								{
									StaffID = reader.GetInt32(0),
									FullName = reader.GetString(1),
									Position = reader.GetString(2),
									Department = reader.GetString(3),
									Email = reader.GetString(4),
									PhoneNumber = reader.GetString(5),
									HireDate = reader.GetDateTime(6)
								};
								Staff.Add(staff);
							}
						}
					}
				}
				catch (Exception ex)
				{
					// Handle exceptions appropriately
					throw new Exception("Error retrieving Staff", ex);
				}
			}
			return Staff;
		}
        private bool StaffExist(string Email)
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
        public IActionResult CreateStaff(Staff staff)
        {
            if (StaffExist(staff.Email))
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
                const string query = "INSERT INTO Staff (FullName, Position, Department, Email, PhoneNumber, HireDate) VALUES (@FullName, @Position, @Department, @Email, @PhoneNumber, GETDATE()) SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", staff.FullName);
                    command.Parameters.AddWithValue("@Position", staff.Position);
                    command.Parameters.AddWithValue("@Department", staff.Department);
                    command.Parameters.AddWithValue("@Email", staff.Email);
                    command.Parameters.AddWithValue("@PhoneNumber", staff.PhoneNumber);
                    
                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Staff added successfully!");
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
            Console.WriteLine($"Received: {staff.FullName}, {staff.Position}, {staff.Department}, {staff.Email}, {staff.PhoneNumber}");
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult EditStaff(Staff staff)
		{
			if (ModelState.IsValid)
			{
				string connectionString = _configuration.GetConnectionString("DefaultConnection");

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					const string query = "UPDATE Staff SET FullName = @FullName, Position = @Position, Department = @Department, Email = @Email, PhoneNumber = @PhoneNumber WHERE StaffID = @StaffID";

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@StaffID", staff.StaffID);
						command.Parameters.AddWithValue("@FullName", staff.FullName);
						command.Parameters.AddWithValue("@Position", staff.Position);
						command.Parameters.AddWithValue("@Department", staff.Department);
						command.Parameters.AddWithValue("@Email", staff.Email);
						command.Parameters.AddWithValue("@PhoneNumber", staff.PhoneNumber);

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
								ModelState.AddModelError("", "No staff found to update.");
							}
						}
						catch (Exception ex)
						{
							ModelState.AddModelError("", "Error occurred while updating staff.");
							Console.WriteLine(ex.Message); // Log the error 
						}
					}
				}
			}
			return RedirectToAction("Index");  // If model is not valid, return to the Edit form with errors
		}

		public IActionResult DeleteStaff(int StaffID)
		{
			string connectionString = _configuration.GetConnectionString("DefaultConnection");

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				const string query = "DELETE FROM Staff WHERE StaffID = @StaffID";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@StaffID", StaffID);

					try
					{
						connection.Open();
						int rowsAffected = command.ExecuteNonQuery();

						if (rowsAffected > 0)
						{
							return RedirectToAction("Index"); // Redirect to Staff list after deletion
						}
						else
						{
							ModelState.AddModelError("", "No Staff found with this ID.");
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

		public IActionResult GetDetailsStaff(int StaffID)
		{
			Staff staff = null;
			string connectionString = _configuration.GetConnectionString("DefaultConnection");

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				try
				{
					connection.Open();
					string sqlQuery = "SELECT * FROM Staff WHERE StaffID = @StaffID";
					using (SqlCommand command = new SqlCommand(sqlQuery, connection))
					{
						command.Parameters.AddWithValue("@StaffID", StaffID);
						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								staff = new Staff
								{
									StaffID = reader.GetInt32(0),
									FullName = reader.GetString(1),
									Position = reader.GetString(2),
									Department = reader.GetString(3),
									Email = reader.GetString(4),
									PhoneNumber = reader.GetString(5),
									HireDate = reader.GetDateTime(6)
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

			if (staff == null)
			{
				return NotFound(); // No staff found with this ID
			}

			return View("GetDetailsStaff", staff);  // Pass the staff object to the view
		}
		public IActionResult Home()
		{
			return View();
		}
		public IActionResult Reports()
		{
			return View();
		}
		public IActionResult Contact()
		{
			return View();
		}
	}
}
