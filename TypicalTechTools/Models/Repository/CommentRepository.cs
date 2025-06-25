
using Ganss.Xss;
using TypicalTechTools.Models.Data;

namespace TypicalTechTools.Models.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly TypicalTechToolsDBContext _context;
       
            // Request the context class by naming it as a parameter in the constructor
            public CommentRepository(TypicalTechToolsDBContext context)
        { 
           
            _context = context;
        }
        public void CreateComment(Comment comment)
        {
            // Add the comment to the context class to be added to the DbSet
            _context.Comments.Add(comment);
            // Save all DbSet changes to the database
            _context.SaveChanges();
        }

        public void DeleteComment(int commentId)
        {
            // Find the entry that matches the provided commentId and delete it
            var comment = _context.Comments.FirstOrDefault(c => c.commentId == commentId);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }
        }

        public List<Comment> GetAllComments()
        {
            // Retrieve all comments from the database
            return _context.Comments.OrderBy(c => c.created_date).ToList();
        }

        public Comment GetComment(int commentId)
        {
            // Retrieve a specific comment by its ID
            return _context.Comments.FirstOrDefault(c => c.commentId == commentId);
        }

        public void UpdateComment(Comment comment)
        {
            // Update the comment in the context class
            _context.Comments.Update(comment);
            // Save changes to the database
            _context.SaveChanges();
        }

        public List<Comment> GetCommentsForProduct(string productCode)
        {
            // Fetch comments related to the specified product code
           
            return _context.Comments
                           .Where(c => c.product_code == productCode)
                           .OrderBy(c => c.created_date)
                           .ToList();
        }
    }
}
