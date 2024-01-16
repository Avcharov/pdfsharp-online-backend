using pdfsharp_online_backend.Domain;

namespace pdfsharp_online_backend.Services
{
    public class ImageItemService : IImageItemService
    {
        private readonly List<ImageItem> _imageItems = new List<ImageItem>();

        public IEnumerable<ImageItem> GetAll()
        {
            return _imageItems;
        }

        public ImageItem GetById(int id)
        {
            return _imageItems.FirstOrDefault(i => i.Id == id);
        }

        public void Create(ImageItem imageItem)
        {
            // In a real application, you would also handle ID assignment and validation
            _imageItems.Add(imageItem);
        }

        public bool Update(ImageItem imageItem)
        {
            var existingItem = GetById(imageItem.Id);
            if (existingItem == null)
            {
                return false;
            }

            // Update properties of the existing item
            existingItem.Name = imageItem.Name;
            existingItem.XPos = imageItem.XPos;
            existingItem.YPos = imageItem.YPos;
            existingItem.Rotation = imageItem.Rotation;
            existingItem.ViewId = imageItem.ViewId;
            existingItem.Opacity = imageItem.Opacity;
            existingItem.ImageWidth = imageItem.ImageWidth;
            existingItem.ImageHeight = imageItem.ImageHeight;
            existingItem.ImageRight = imageItem.ImageRight;
            existingItem.ImageBottom = imageItem.ImageBottom;
            existingItem.ImageUrl = imageItem.ImageUrl; // Or ImageData, depending on your design
            existingItem.ImageName = imageItem.ImageName;
            existingItem.PdfPage = imageItem.PdfPage;

            return true;
        }

        public bool Delete(int id)
        {
            var item = GetById(id);
            if (item == null)
            {
                return false;
            }

            _imageItems.Remove(item);
            return true;
        }
    }

}
