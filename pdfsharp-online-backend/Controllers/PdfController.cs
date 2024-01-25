using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using pdfsharp_online_backend.Services;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;


namespace pdfsharp_online_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PdfController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPdfService _pdfService;
        private readonly IImageItemService _imageItemService;

        public PdfController(AppDbContext context, IPdfService pdfService, IImageItemService imageItemService)
        {
            _context = context;
            _pdfService = pdfService;
            _imageItemService = imageItemService;
        }

        [Authorize]
        [HttpGet]
        [Route("{projectId}")]
        public async Task<ActionResult<byte[]>> PrintPdf(long projectId)
        {
            EditView project = await _context.EditViews.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId);
            string pdfBase64 = project.Base64AttachmentCode;
            IEnumerable<ImageItem> imageObjects = await _imageItemService.GetImagesByProjectId(project.Id);

            foreach (var imageObj in imageObjects)
            {
                imageObj.ImageData = imageObj.ImageData.Substring(imageObj.ImageData.IndexOf(',') + 1);
            }

            byte[] modifiedPdf = await _pdfService.PrintDocument(pdfBase64, imageObjects);

            return File(modifiedPdf, "application/pdf", project.Name + ".pdf");

        }        
        
  
        [HttpGet]
        [Route("code/{projectId}")]
        public async Task<ActionResult<byte[]>>ProjectCode(long projectId)
        {
            EditView project = await _context.EditViews.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId);
            string pdfBase64 = project.Base64AttachmentCode;
            IEnumerable<ImageItem> imageObjects = await _imageItemService.GetImagesByProjectId(project.Id);

            foreach (var imageObj in imageObjects)
            {
                imageObj.ImageData = imageObj.ImageData.Substring(imageObj.ImageData.IndexOf(',') + 1);
            }

            byte[] fileTxt = await _pdfService.GetPdfSharpCode(project, imageObjects);

            return File(fileTxt, "text/plain", $@"PdfSharpCode_{project.Name}.txt");

        }


    }
}
