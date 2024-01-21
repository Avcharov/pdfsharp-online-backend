using Microsoft.Extensions.Configuration;
using pdfsharp_online_backend.Domain;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace pdfsharp_online_backend.Services
{
    public class ImageItemService : IImageItemService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public ImageItemService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<ImageItem> GetImageById(long id)
        {
            return await _context.ImageItems.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<ImageItem>> GetImagesByProjectId(long projectId)
        {
            return await _context.ImageItems.AsNoTracking().Where(i => i.ViewId == projectId).ToListAsync();
        }

        public async Task<ImageItem> AddImage(ImageItem imageItem)
        {
            imageItem.Id = 0;
            // In a real application, you would also handle ID assignment and validation
            await _context.ImageItems.AddAsync(imageItem);
            await _context.SaveChangesAsync();

            return imageItem;
        }

        public async Task<IEnumerable<ImageItem>> Update(IEnumerable<ImageItem> imageItems)
        {

            foreach (var imageItem in imageItems)
            {
                var existingItem = await GetImageById(imageItem.Id);
                if (existingItem == null)
                {
                    throw new ValidationException("Given Image has is empty.");
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
                existingItem.ImageUrl = imageItem.ImageUrl;
                existingItem.ImageData = imageItem.ImageData;
                existingItem.PdfPage = imageItem.PdfPage;

                _context.ImageItems.Update(existingItem);
            }

            await _context.SaveChangesAsync();

            return imageItems;
        }

        public async Task<ImageItem> Delete(long id)
        {
            var item = await GetImageById(id);
            if (item == null)
            {
                throw new ValidationException("Given Image Id not found.");
            }

            _context.ImageItems.Remove(item);
            await _context.SaveChangesAsync();

            return item;
        }
    }

}
