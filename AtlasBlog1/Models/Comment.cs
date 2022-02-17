using AtlasBlog1.Enums;

namespace AtlasBlog1.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }

        //Need to refrence the comment author
        public string? AuthorId { get; set; }
        public string? ModeratorId { get; set; }
        public string CommentBody { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }

        //Moderator Properties
        public DateTime? ModDate { get; set; }
        public ModerationReason ModType { get; set; }
        public string? ModBody { get; set; }



        //Navigation property that is "lazy loaded"
        public virtual Post? Post { get; set; }
        public virtual AppUser? Author { get; set; }
        public virtual AppUser? Moderator { get; set; }


    }
}
