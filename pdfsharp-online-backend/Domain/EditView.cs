﻿using System.ComponentModel.DataAnnotations.Schema;
namespace pdfsharp_online_backend.Domain
{
    public class EditView
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }
        public string Base64AttachmentCode { get; set; }
    }
}
