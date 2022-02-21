namespace AtlasBlog1.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string TagItem { get; set; } = string.Empty;
        public string TagDescript { get; set; } = string.Empty;


        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    }
}
