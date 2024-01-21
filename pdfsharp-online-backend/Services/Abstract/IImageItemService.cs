using pdfsharp_online_backend.Domain;

namespace pdfsharp_online_backend.Services.Abstract
{
    public interface IImageItemService
    {
       // Task<ImageItem> GetAll();
        Task<ImageItem> GetImageById(long id);
        Task<IEnumerable<ImageItem>> GetImagesByProjectId(long id);
        Task<ImageItem> AddImage(ImageItem imageItem);
        Task<IEnumerable<ImageItem>> Update(IEnumerable<ImageItem> imageItems);
        Task<ImageItem> Delete(long id);
    }

}
