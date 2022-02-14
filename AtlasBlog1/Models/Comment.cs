namespace AtlasBlog1.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }

        //Need to refrence the comment author
        public string AuthorId { get; set; }




        public string CommentBody { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }




        //Navigation property that is "lazy loaded"
        public virtual Post? Post { get; set; }
        public virtual AppUser? Author { get; set; }


    }
}
