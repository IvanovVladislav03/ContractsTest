using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test_1.Models;
using Test_1.Data;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Globalization;

namespace Test_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;

        }
        public IActionResult UploadExcel()
        {
            var contracts = _context.Contracts.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file!=null && file.Length>0)
            {
                var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads";

                if(!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, file.FileName);

                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Choose one of either 1 or 2:

                        // 1. Use the reader methods
                        do
                        {
                            bool headSkip = false;
                            bool contOrStage = false;
                            while (reader.Read())
                            {
                                
                                if (!headSkip)
                                {
                                    if (reader.GetValue(0).ToString() == "ContractCode")
                                        contOrStage = true;
                                    headSkip = true;
                                    continue;
                                }

                                if (contOrStage)
                                {

                                    Contract c = new Contract();
                                    c.ContractCode = reader.GetValue(0).ToString();
                                    c.ContractName = reader.GetValue(1).ToString();
                                    c.Customer = reader.GetValue(2).ToString();

                                    _context.Add(c);
                                }
                                else
                                {


                                    Stages s = new Stages();
                                    s.StageName = reader.GetValue(0).ToString();
                                    string startDateString = reader.GetValue(1).ToString();
                                    string endDateString = reader.GetValue(2).ToString();

                                    if (DateTime.TryParseExact(startDateString, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
                                    {
                                        s.StartDate = startDate;
                                    }
                                    else
                                    {
                                        throw new FormatException("Некорректный формат даты.");
                                    }

                                    if (DateTime.TryParseExact(endDateString, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
                                    {
                                        s.EndDate = endDate;
                                    }
                                    else
                                    {
                                        throw new FormatException("Некорректный формат даты.");
                                    }


                                    s.ContractId = Convert.ToInt32(reader.GetValue(3));

                                    _context.Add(s);
                                }
                                await _context.SaveChangesAsync();
                            }
                        } while (reader.NextResult());


                    }
                }
            }
            return View();
        }
        public ActionResult Index()
        {

            return View();
        }


        public ActionResult GetContractsJson()
        {
            var contracts = _context.Contracts.ToList();
            return PartialView("_ContractsListPartial", contracts);
        }

        public IActionResult GetStagesByContractCode(string contractCode)
        {
            var contracts = _context.Contracts.ToList();
            var contract = contracts[Convert.ToInt32(contractCode)-1];

            if (contract != null)
            {
                var stages = _context.Stages.Where(s => s.ContractId == contract.ContractId).ToList();
                return PartialView("_StagesListPartial", stages);
            }

            return PartialView("_StagesListPartial", new List<Stages>()); // Возвращаем пустой список, если договор не найден
        }



    }
}
