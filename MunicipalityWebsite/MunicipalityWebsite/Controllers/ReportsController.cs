using Microsoft.AspNetCore.Mvc;
using MunicipalityWebsite.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace MunicipalityWebsite.Controllers
{
	public class ReportsController : Controller
	{
		private IConfiguration _configuration;
		public ReportsController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public IActionResult Index()
		{
			List<Reports> reports = new List<Reports>
			{

			};
			reports = GetReports();
			return View(reports);
		}
		private List<Reports> GetReports()
		{
			List<Reports> Reports = new List<Reports>
			{

			};
			string connectionString = _configuration.GetConnectionString("DefaultConnection");

			using (System.Data.SqlClient.SqlConnection connection = new SqlConnection(connectionString))
			{
				try
				{
					connection.Open();

					string sqlQuery = "SELECT * FROM Reports";

					using (SqlCommand command = new SqlCommand(sqlQuery, connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							// Map data to Report objects
							while (reader.Read())
							{
								Reports reports = new Reports
								{
									ReportID = reader.GetInt32(0),
									CitizenID = reader.GetInt32(1),
									ReportType = reader.GetString(2),
									Details = reader.GetString(3),
									SubmissionDate = reader.GetDateTime(4),
									Status = reader.GetString(5),
								};
								Reports.Add(reports);
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
			return Reports;
		}
		[HttpPost]
		public IActionResult CreateReport(Reports reports)
		{

			string connectionString = _configuration.GetConnectionString("DefaultConnection");
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				const string query = "INSERT INTO Reports (CitizenID, ReportType, Details, SubmissionDate, Status) VALUES (@CitizenID, @ReportType, @Details, GETDATE(), 'Under Review');";
				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@CitizenID", reports.CitizenID);
					command.Parameters.AddWithValue("@ReportType", reports.ReportType);
					command.Parameters.AddWithValue("@Details", reports.Details);
					command.Parameters.AddWithValue("@SubmissionDate", reports.SubmissionDate);


					try
					{
						connection.Open();
						int rowsAffected = command.ExecuteNonQuery();

						if (rowsAffected > 0)
						{
							Console.WriteLine("Report added successfully!");
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
			Console.WriteLine($"Received: {reports.CitizenID}, {reports.ReportType}, {reports.Details}, {reports.SubmissionDate}, {reports.Status}");
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult ReviewReport(int ReportID)
		{
			if (ModelState.IsValid)
			{
				Reports report = null;
				string connectionString = _configuration.GetConnectionString("DefaultConnection");
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					try
					{
						connection.Open();
						string sqlQuery = "SELECT * FROM Reports WHERE ReportID = @ReportID";
						using (SqlCommand command = new SqlCommand(sqlQuery, connection))
						{
							command.Parameters.AddWithValue("@ReportID", ReportID);
							using (SqlDataReader reader = command.ExecuteReader())
							{
								if (reader.Read())
								{
									report = new Reports
									{
										ReportID = reader.GetInt32(0),
										CitizenID = reader.GetInt32(1),
										ReportType = reader.GetString(2),
										Details = reader.GetString(3),
										SubmissionDate = reader.GetDateTime(4),
										Status = reader.GetString(5),
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
				if (report == null)
				{
					return NotFound(); // No citizen found with this ID
				}
				
				return View("GetDetailsReport", report);
				
			}
			return RedirectToAction("Index"); // If model is not valid, return to the Edit form with errors
		}

		[HttpPost]
		public IActionResult UpdateReport(Reports report)
		{
            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    const string query = "UPDATE Reports SET ReportType = @ReportType, Details = @Details, SubmissionDate = @SubmissionDate, Status = @Status WHERE ReportID = @ReportID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReportID", report.ReportID);
						command.Parameters.AddWithValue("@CitizenID", report.CitizenID);
						command.Parameters.AddWithValue("@ReportType", report.ReportType);
                        command.Parameters.AddWithValue("@Details", report.Details);
                        command.Parameters.AddWithValue("@SubmissionDate", report.SubmissionDate);
                        command.Parameters.AddWithValue("@Status", report.Status);

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

    public IActionResult DeleteReport(int CitizenID)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                const string query = "DELETE FROM Reports WHERE CitizenID = @CitizenID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CitizenID", CitizenID);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return RedirectToAction("Index"); // Redirect to report list after deletion
                        }
                        else
                        {
                            ModelState.AddModelError("", "No citizen found with this ID.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error occurred while deleting report.");
                        Console.WriteLine(ex.Message); // Log error
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult GetDetailsCitizen(int CitizenID)
        {
            Reports report = null;
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
                                report = new Reports
                                {
                                    ReportID = reader.GetInt32(0),
                                    CitizenID = reader.GetInt32(1),
                                    ReportType = reader.GetString(2),
                                    Details = reader.GetString(3),
                                    SubmissionDate = reader.GetDateTime(4),
                                    Status = reader.GetString(5)
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

            if (report == null)
            {
                return NotFound(); // No citizen found with this ID
            }

            return View("GetDetailsReport", report);  // Pass the report object to the view
        }

        public IActionResult Staff()
		{
			return View();
		}
		public IActionResult Home()
		{
			return View();
		}
		public IActionResult Contact()
		{
			return View();
		}
	}
}
