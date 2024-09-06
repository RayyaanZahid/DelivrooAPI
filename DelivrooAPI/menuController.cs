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
    public class menuController : ControllerBase
    {
        //private const string SharedSecret = "abc123";
        private const string SharedSecret = "shared_secret_webhook";
        private const string FilePath = "menu_data.json";

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            // Get headers
            if (!Request.Headers.TryGetValue("x-deliveroo-sequence-guid", out var sequenceHeader) ||
                !Request.Headers.TryGetValue("x-deliveroo-hmac-sha256", out var expectedHeader))
            {
                Log("400 Bad Request - Missing required headers.");

                return BadRequest("Missing required headers.");
            }

            string sequence = sequenceHeader.ToString();
            string expected = expectedHeader.ToString();

            // Read the request body
            string requestBody;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SharedSecret)))
            {
                // Update HMAC with sequence
                byte[] sequenceBytes = Encoding.UTF8.GetBytes(sequence);
                hmac.TransformBlock(sequenceBytes, 0, sequenceBytes.Length, sequenceBytes, 0);

                // Update HMAC with space
                byte[] spaceByte = Encoding.UTF8.GetBytes(" ");
                hmac.TransformBlock(spaceByte, 0, spaceByte.Length, spaceByte, 0);

                // Update HMAC with body
                byte[] bodyBytes = Encoding.UTF8.GetBytes(requestBody);
                hmac.TransformFinalBlock(bodyBytes, 0, bodyBytes.Length);

                // Get the HMAC as a hexadecimal string
                string calculatedHmac = BitConverter.ToString(hmac.Hash).Replace("-", "").ToLower();
                Console.WriteLine($"Calculated HMAC: {calculatedHmac}");
                if (expected != calculatedHmac)
                {
                    Log($"400 Bad Request - Invalid HMAC. Expected: {expected}, Calculated: {calculatedHmac}");

                    return BadRequest($"Invalid HMAC. Expected: {expected}, Calculated: {calculatedHmac}");
                }
            }

            // Compute HMAC

            try
            {
                JObject json = JObject.Parse(requestBody);
                await System.IO.File.WriteAllTextAsync(FilePath, json.ToString());
                Log("200 OK - Webhook received and saved successfully.");

            }
            catch (JsonReaderException ex)
            {
                Log($"500 Internal Server Error - {ex.Message}");
                Log($"{requestBody}");
                return BadRequest("Invalid JSON data.");

            }

            return Ok("Webhook received and saved successfully.");
        }

        [HttpGet("data")]
        public IActionResult GetData()
        {
            try
            {
                if (System.IO.File.Exists(FilePath))
                {
                    var data = System.IO.File.ReadAllText(FilePath);
                    Log("200 OK - Data retrieved successfully.");
                    return Content(data, "application/json");
                }
                Log("404 Not Found - No data available.");
                return NotFound("No data available.");
            }
            catch (Exception ex)
            {
                Log($"500 Internal Server Error - {ex.Message}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        private void Log(string message)
        {
            var logMessage = $"{System.DateTime.Now}: Deliveroo/Menu : {message}\n";
            System.IO.File.AppendAllText("log.txt", logMessage);
        }
    }
}
