using System.ComponentModel.DataAnnotations;

namespace AtlasBlog1.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Blog Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at most {1} and at least {2} characters long", MinimumLength = 10)]
        public string BlogName { get; set; } = "";

        [Required]
        [StringLength(300, ErrorMessage = "The {0} must be at most {1} and at least {2} characters long", MinimumLength = 10)]
        public string Description { get; set; } = "";

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        public DateTime Update { get; set; }

        //This model should have a list pf Posts children
        public ICollection<Post> Post { get; set; } = new HashSet<Post>();


    }
}
