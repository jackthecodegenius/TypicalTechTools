using TypicalTechTools.Models;

namespace TypicalTechTools.Models.Repository
{
    public interface ICommentRepository
    {
        List<Comment> GetAllComments();
        Comment GetComment(int commentId);
        void CreateComment(Comment comment);
        void UpdateComment(Comment comment);
        void DeleteComment(int commentId);
        List<Comment> GetCommentsForProduct(string productCode);
    }
}
