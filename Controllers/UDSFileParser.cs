using ie_home_test_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ie_home_test_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UDSFileParser : ControllerBase
    {
		private readonly UDSFileProcessor udsFileProcessor = new UDSFileProcessor();

		[HttpPost]
		[Route("Process")]
		public IActionResult Process(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("File is empty or not provided.");
			}

			try
			{
				var writeBlock = udsFileProcessor.ExportWriteData(file);
				var fileName = file.FileName;
				fileName = fileName.Replace(".candata", ".transferdata.bin");
				return File(writeBlock.ToArray(), "application/octet-stream", fileName);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}
	}
}
