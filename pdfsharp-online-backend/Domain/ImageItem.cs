using System.ComponentModel.DataAnnotations.Schema;

namespace pdfsharp_online_backend.Domain
{
    public class ImageItem
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double XPos { get; set; }
        public double YPos { get; set; }
        public double Rotation { get; set; }
        public int ViewId { get; set; }
        [ForeignKey(nameof(ViewId))]
        public virtual EditView? EditView{ get; set; }

        public double Opacity { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public int ImageRight { get; set; }
        public int ImageBottom { get; set; }

        public string ImageUrl { get; set; } // Or byte[] ImageData { get; set; }
        public string ImageData { get; set; }
        public int PdfPage { get; set; }
    }

}
