using pdfsharp_online_backend.Domain;

namespace pdfsharp_online_backend.Services.Abstract
{
    public interface IImageItemService
    {
        IEnumerable<ImageItem> GetAll();
        ImageItem GetById(int id);
        void Create(ImageItem imageItem);
        bool Update(ImageItem imageItem);
        bool Delete(int id);
    }

}
