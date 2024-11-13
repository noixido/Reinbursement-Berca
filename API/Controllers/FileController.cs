using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class FileController : ControllerBase
    {

        [HttpGet("uploads/evidence/{accountId}/{fileName}")]
        public IActionResult GetEvidence(string accountId, string fileName)
        {
            // Menentukan path file
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "evidence", accountId, fileName);

            if (System.IO.File.Exists(filePath))
            {
                // Mengembalikan file gambar
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileExtension = Path.GetExtension(filePath).ToLower();

                // Menentukan MIME type berdasarkan ekstensi file
                string mimeType = fileExtension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".pdf" => "application/pdf",
                    _ => "application/octet-stream",
                };

                // Menambahkan Content-Disposition sebagai inline untuk tampilan di browser
                Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");

                return File(fileBytes, mimeType);
            }
            else
            {
                return NotFound(new {
                    StatusCode = 400,
                    Message = "File tidak ditemukan"
                });
            }
        }
    }
}
