using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json; // Ensure you have Newtonsoft.Json package installed

namespace DeliverooAPI.Controllers
{
    [Route("deliveroo/[controller]")]
    [ApiController]
    public class logsController : ControllerBase
    {
        //private const string SharedSecret = "abc123";
        private const string SharedSecret = "yzhnEmDy5krDa34kgZkN6G2pU9hotZLA28NdPqg6Bn-cyVLuJp9nUtLLh2-uxwuBrXAYPXtGb9xrMCAXyx91Lg";
        private const string FilePath = "log.txt";
        [HttpGet]
        public IActionResult GetData()
        {
            if (System.IO.File.Exists(FilePath))
            {
                var data = System.IO.File.ReadAllText(FilePath);
                return Ok(data);
            }
            return NotFound("No data available.");
        }

        private void Log(string message)
        {
            var logMessage = $"{System.DateTime.Now}: {message}\n";
            System.IO.File.AppendAllText("log.txt", logMessage);
        }
    }
}
