using Microsoft.AspNetCore.Mvc;
using MunicipalityWebsite.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace MunicipalityWebsite.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Staff()
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
