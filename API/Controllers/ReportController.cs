using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReportRenderingService _reportService;

        public ReportController(ReportRenderingService reportService)
        {
            _reportService = reportService;
        }
        [HttpGet("GenerateSingleTransactionPDF")]
        public ActionResult GeneratePDF()
        {
            var pdfBytes = _reportService.GeneratePDFReport();
            return File(pdfBytes, "application/pdf", "transaction-report.pdf");
        }
    }
}
