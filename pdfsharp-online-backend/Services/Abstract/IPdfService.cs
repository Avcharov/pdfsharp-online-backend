namespace pdfsharp_online_backend.Services.Abstract
{
    public interface IPdfService
    {
        Task<byte[]> PrintDocument(string pdfBase64, IEnumerable<ImageItem> imageItems);
    }
}
