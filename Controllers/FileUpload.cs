using Microsoft.AspNetCore.Mvc;

namespace FileUpload.Controllers;

[Route("api/[controller]")]
[ApiController]

public class FileUploadController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    public FileUploadController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }
    [HttpPost]
    public async Task<IActionResult> UploadFile(List<IFormFile> files)
    {
        if (files.Count == 0)
        {
            return BadRequest("No files found");
        }
        string directoryName = Path.Combine(_webHostEnvironment.ContentRootPath, "UploadedFiles");

        foreach (var file in files)
        {

            string filePath = Path.Combine(directoryName, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
        return Ok("upload successful");
    }



}