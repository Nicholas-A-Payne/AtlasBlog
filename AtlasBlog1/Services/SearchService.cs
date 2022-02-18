using AtlasBlog1.Data;
using AtlasBlog1.Models;

namespace AtlasBlog1.Services
{
    public class SearchService
    {
        private readonly ApplicationDbContext _dbContext;

        public SearchService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IOrderedQueryable<Post> ItemSearch(string? SearchItem)
        {
            SearchItem = SearchItem?.ToLower();

            var resultSet = _dbContext
                            .Posts
                            .Where(b => b.PostState == Enums.PostState.ProducationReady && 
                                  !b.IsDeleted).AsQueryable();

            //If they supplied a Search Item, I will look for it insdie the resultset
            if (!string.IsNullOrEmpty(SearchItem))
            {
                resultSet = resultSet.Where(p =>
                    p.Title.ToLower().Contains(SearchItem) ||
                    p.Abstract.ToLower().Contains(SearchItem) ||
                    p.Body.ToLower().Contains(SearchItem) ||
                    p.Comments.Any(c =>
                        c.CommentBody.ToLower().Contains(SearchItem) ||
                        c.ModBody!.ToLower().Contains(SearchItem) ||
                        c.Author!.FirstName.ToLower().Contains(SearchItem) ||
                        c.Author.LastName.ToLower().Contains(SearchItem) ||
                        c.Author.Email.ToLower().Contains(SearchItem) ||
                        c.Author.UserName.ToLower().Contains(SearchItem))); 
            }

            return resultSet.OrderByDescending(r => r.Created);
        }


    }
}
