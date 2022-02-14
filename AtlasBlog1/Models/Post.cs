using AtlasBlog1.Enums;
using System.ComponentModel.DataAnnotations;

namespace AtlasBlog1.Models
{
    public class Post
    {

        public int Id { get; set; }

        [Display(Name = "Blog Id")]
        public int BlogId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage ="The {0} must be atleast {2} and at most {1} characters long", MinimumLength = 5)]
        public string Title { get; set; } = "";


        public string Slug { get; set; } = "";

        public bool IsDeleted { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be atleast {2} and at most {1} characters long", MinimumLength = 5)]
        public string Abstract { get; set; } = "";

        [Required]
        [Display(Name = "Post State" )]
        public PostState PostState { get; set; }


        [Required]
        public string Body { get; set; } = "";

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        //Navigation Properties
        public Blog? Blog { get; set; }

        //Comments
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();


    }
}
