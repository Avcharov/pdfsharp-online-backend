using Microsoft.AspNetCore.Mvc;
using pdfsharp_online_backend.Controllers;

[Route("api/image")]
[ApiController]
public class ImageItemsController : ControllerBase
{
    private readonly IImageItemService _imageItemService;

    public ImageItemsController(IImageItemService imageItemService)
    {
        _imageItemService = imageItemService;
    }

   // [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ImageItem>>> GetImageItems([FromQuery] long projectId)
    {
        return Ok(await _imageItemService.GetImagesByProjectId(projectId));
    }

    // GET: api/ImageItems/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ImageItem>> GetImageItem(long id)
    {
        var imageItem = await _imageItemService.GetImageById(id);

        if (imageItem == null)
        {
            return NotFound();
        }

        return imageItem;
    }

    // POST: api/ImageItems
    [HttpPost]
    public async Task<ActionResult<ImageItem>> PostImageItem(ImageItem imageItem)
    {
        await _imageItemService.AddImage(imageItem);
        return CreatedAtAction(nameof(GetImageItem), new { id = imageItem.Id }, imageItem);
    }

    // PUT: api/ImageItems/5
    [HttpPut]
    public async Task<ActionResult<IEnumerable<ImageItem>>> PutImageItem(IEnumerable<ImageItem> imageItems)
    {
        await _imageItemService.Update(imageItems);

        return Ok(imageItems);
    }

    // DELETE: api/ImageItems/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteImageItem(int id)
    {
        var imageItem = await _imageItemService.GetImageById(id);

        if (imageItem == null)
        {
            return NotFound();
        }

        await _imageItemService.Delete(id);

        return Ok(imageItem);
    }
}
