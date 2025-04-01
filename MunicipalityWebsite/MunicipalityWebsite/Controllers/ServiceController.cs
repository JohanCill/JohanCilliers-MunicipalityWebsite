using Microsoft.AspNetCore.Mvc;
using MunicipalityWebsite.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System;
using Azure.Core;

namespace MunicipalityWebsite.Controllers
{
    public class ServiceController : Controller
    {
        private IConfiguration _configuration;
        public ServiceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            List<Service> Requests = new List<Service>
            {

            };
            Requests = GetRequests();
            return View(Requests);
        }
        private List<Service> GetRequests()
        {
            List<Service> Requests = new List<Service>
            {

            };
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string sqlQuery = "SELECT * FROM ServiceRequests";

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Map data to Citizen objects
                            while (reader.Read())
                            {
                                Service service = new Service
                                {
                                    RequestID = reader.GetInt32(0),
                                    CitizenID = reader.GetInt32(1),
                                    ServiceType = reader.GetString(2),
                                    RequestDate = reader.GetDateTime(3),
                                    Status = reader.GetString(4),

                                };
                                Requests.Add(service);
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
            return Requests;
        }

		[HttpPost]
		public IActionResult CreateRequest(Service request)
		{

			string connectionString = _configuration.GetConnectionString("DefaultConnection");

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				const string query = "INSERT INTO ServiceRequests (CitizenID, ServiceType, RequestDate, Status) VALUES (@CitizenID, @ServiceType, GETDATE(), 'Pending');";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@CitizenID", request.CitizenID);
					command.Parameters.AddWithValue("@ServiceType", request.ServiceType);

					try
					{
						connection.Open();
						int rowsAffected = command.ExecuteNonQuery();

						if (rowsAffected > 0)
						{
							Console.WriteLine("Request added successfully!");
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
			Console.WriteLine($"Received: {request.CitizenID}, {request.ServiceType}, {request.RequestDate}, {request.Status}");
			return RedirectToAction("Index");
		}
		[HttpPost]
		public IActionResult EditStatus(int CitizenID, string Status)
		{
			if (ModelState.IsValid)
			{

				string connectionString = _configuration.GetConnectionString("DefaultConnection");

				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					const string query = "UPDATE ServiceRequests SET Status = @Status WHERE CitizenID = @CitizenID";

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@CitizenID", CitizenID);
						command.Parameters.AddWithValue("@Status", Status);

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

		public IActionResult DeleteRequest(int CitizenID)
		{
			string connectionString = _configuration.GetConnectionString("DefaultConnection");

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				const string query = "DELETE FROM ServiceRequests WHERE CitizenID = @CitizenID";

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
		public IActionResult GetDetailsService(int CitizenID)
		{
			Service request = null;
			string connectionString = _configuration.GetConnectionString("DefaultConnection");

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				try
				{
					connection.Open();
					string sqlQuery = "SELECT * FROM ServiceRequests WHERE CitizenID = @CitizenID";
					using (SqlCommand command = new SqlCommand(sqlQuery, connection))
					{
						command.Parameters.AddWithValue("@CitizenID", CitizenID);
						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								request = new Service
								{
									RequestID = reader.GetInt32(0),
									CitizenID = reader.GetInt32(1),
									ServiceType = reader.GetString(2),
									RequestDate = reader.GetDateTime(3),
									Status = reader.GetString(4),
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

			if (request == null)
			{
				return NotFound(); // No citizen found with this ID
			}

			return View("GetDetailsService", request);  // Pass the citizen object to the view
		}

	}
}
