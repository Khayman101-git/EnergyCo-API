using Microsoft.AspNetCore.Mvc;
using EnergyCo.Models;
using EnergyCo.Services;

[ApiController]
[Route("meter-reading-uploads")]
public class MeterReadingUploadsController : ControllerBase
{
    private readonly IMeterReadingService _service;

    public MeterReadingUploadsController(IMeterReadingService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<MeterReadingResult>> Upload([FromForm] IFormFile file)
    {
        var result = await _service.ProcessCsvAsync(file);
        return Ok(result);
    }
}
