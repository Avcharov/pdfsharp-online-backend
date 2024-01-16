namespace pdfsharp_online_backend.Domain
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<User>? Users { get; set; }
    }
}
