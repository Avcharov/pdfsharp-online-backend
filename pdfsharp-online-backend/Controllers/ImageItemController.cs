using Microsoft.AspNetCore.Mvc;
using pdfsharp_online_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImageItemsController : ControllerBase
{
    private readonly IImageItemService _imageItemService;

    public ImageItemsController(IImageItemService imageItemService)
    {
        _imageItemService = imageItemService;
    }

    // GET: api/ImageItems
    [HttpGet]
    public ActionResult<IEnumerable<ImageItem>> GetImageItems()
    {
        return Ok(_imageItemService.GetAll());
    }

    // GET: api/ImageItems/5
    [HttpGet("{id}")]
    public ActionResult<ImageItem> GetImageItem(int id)
    {
        var imageItem = _imageItemService.GetById(id);

        if (imageItem == null)
        {
            return NotFound();
        }

        return imageItem;
    }

    // POST: api/ImageItems
    [HttpPost]
    public ActionResult<ImageItem> PostImageItem(ImageItem imageItem)
    {
        _imageItemService.Create(imageItem);
        return CreatedAtAction(nameof(GetImageItem), new { id = imageItem.Id }, imageItem);
    }

    // PUT: api/ImageItems/5
    [HttpPut("{id}")]
    public IActionResult PutImageItem(int id, ImageItem imageItem)
    {
        if (id != imageItem.Id)
        {
            return BadRequest();
        }

        if (!_imageItemService.Update(imageItem))
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: api/ImageItems/5
    [HttpDelete("{id}")]
    public IActionResult DeleteImageItem(int id)
    {
        if (!_imageItemService.Delete(id))
        {
            return NotFound();
        }

        return NoContent();
    }
}
