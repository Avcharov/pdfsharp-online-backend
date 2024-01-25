using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using PdfSharp.UniversalAccessibility.Drawing;
using pdfsharp_online_backend.Domain;

namespace pdfsharp_online_backend.Services
{
    public class PdfService : IPdfService
    {

        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public PdfService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public Task<byte[]> PrintDocument(string pdfBase64, IEnumerable<ImageItem> imageItems)
        {
            byte[] pdfBytes = Convert.FromBase64String(pdfBase64);

            using (MemoryStream pdfStream = new MemoryStream(pdfBytes))
            {
                // Load PDF document
                PdfDocument document = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Modify);

                var groupedItems = imageItems.GroupBy(i => i.PdfPage);

                foreach (var group in groupedItems)
                {
                    int pageNumber = group.Key;
                    if (pageNumber < 0 || pageNumber > document.Pages.Count)
                    {
                        // Handle invalid page number
                        continue; // Skip this group or throw an exception
                    }

                    PdfPage page = document.Pages[pageNumber - 1];

                    // Create XGraphics for the current page
                    using (XGraphics gfx = XGraphics.FromPdfPage(page))
                    {
                        foreach (var imageItem in group)
                        {
                            byte[] imageBytes = Convert.FromBase64String(imageItem.ImageData);
                            using (MemoryStream imageStream = new MemoryStream(imageBytes, 0, imageBytes.Length, true, true))
                            {
                                imageStream.Position = 0;

                                // Load image
                                XImage image = XImage.FromStream(imageStream);

                                // Draw the image
                                gfx.DrawImage(image, imageItem.XPos, imageItem.YPos, imageItem.ImageWidth, imageItem.ImageHeight);
                            }
                        }
                    }
                }

                // Save the document to a new stream and return it
                using (MemoryStream outputPdfStream = new MemoryStream())
                {
                    document.Save(outputPdfStream);
                    return Task.FromResult(outputPdfStream.ToArray());
                }
            }
        }

        public Task<byte[]> GetPdfSharpCode(EditView project, IEnumerable<ImageItem> imageItems)
        {
            var body = "";

            var groupedItems = imageItems.GroupBy(i => i.PdfPage);

            foreach (var group in groupedItems)
            {
                int pageNumber = group.Key;

                var pageString = $@"
                //Get Page of document by getting it by index
                PdfPage page{pageNumber} = document.Pages[{pageNumber} - 1];
                ";

                foreach (var imageItem in group)
                {
                    var middleTemplate = $@"

                    using (XGraphics gfx = XGraphics.FromPdfPage(page))
                    {{
                            byte[] imageBytes = Convert.FromBase64String('Put here Base64 string of image');
                            using (MemoryStream imageStream = new MemoryStream(imageBytes, 0, imageBytes.Length, true, true))
                            {{
                                imageStream.Position = 0;

                                // Load image
                                XImage image = XImage.FromStream(imageStream);

                                // Draw the image
                                gfx.DrawImage(image, {imageItem.XPos}, {imageItem.YPos}, {imageItem.ImageWidth}, {imageItem.ImageHeight});
                            }}
                    }}";

                    pageString += middleTemplate;
                }

                body += pageString;

            }

            string startTemplate = $@"
            public Task<byte[]> PrintDocument(string pdfBase64, string imageBase64, ImageItem imageObj)
            {{
            byte[] pdfBytes = Convert.FromBase64String(pdfBase64);
            byte[] imageBytes = Convert.FromBase64String(imageBase64);
            {body}
            }}";

            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(startTemplate);

            return Task.FromResult(byteArray);
        }

    }
}
