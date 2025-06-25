
using TypicalTechTools.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TypicalTechTools.Models.Repository;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Ganss.Xss;


namespace TypicalTools.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        HtmlSanitizer _sanitizer = new HtmlSanitizer();
        public CommentController(ICommentRepository commentRepository, HtmlSanitizer sanitizer)
        {
            _commentRepository = commentRepository;
            _sanitizer = sanitizer;

        }

        [HttpGet] 
        public IActionResult CommentList(string productCode)
        {
            
            var comments = _commentRepository.GetCommentsForProduct(productCode);

            // Passes the product code to the view using ViewBag
            ViewBag.productCode = productCode;

            // Returns the list of comments to the view
            return View(comments);
        }

        // Show a form to add a new comment
        [HttpGet]
        [Authorize(Roles = "USER")]
        public IActionResult AddComment(string productCode)
        {
            

            var comment = new Comment
            {
                product_code = productCode
            };
            return View(comment);
        }

        // Receive and handle the newly created comment data
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(Comment comment)
        {
            ModelState.Remove("User");
            // Check if the model is valid
            if (ModelState.IsValid)
                {
                    // Set the current date for the comment and save the session ID
                    comment.created_date = DateTime.Now;
                    comment.session_id = HttpContext.Session.Id;
                   comment.comment_text = _sanitizer.Sanitize(comment.comment_text);
                



                // Check if the comment is empty after sanitization
                if (string.IsNullOrWhiteSpace(comment.comment_text))
                {
                    ModelState.AddModelError("comment_text", "Comment cannot be empty after sanitization.");
                    return View(comment);
                }

                // Save the comment to the repository
                _commentRepository.CreateComment(comment);

                // Redirect to the comment list for the specific product
                return RedirectToAction("CommentList", new { productCode = comment.product_code });
                }

            return View(comment);
        }

        // Handle a request to delete a comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveComment(int commentId)
        {

            var comment = _commentRepository.GetComment(commentId);

            // If the comment exists delete it from the repository
            if (comment != null)
            {
                _commentRepository.DeleteComment(commentId);
            }

            // Redirect to the comment list for the product
            return RedirectToAction("CommentList", new { productCode = comment?.product_code });
        }

        // Display the edit form for an existing comment
        [HttpGet]
        [Authorize(Roles = "USER")]
        public IActionResult EditComment(int commentId)
        {
            // Retrieve the comment by ID
            var comment = _commentRepository.GetComment(commentId);

            // If the comment is not found redirect to the product index page
            if (comment == null)
            {
                return RedirectToAction("Index", "Product");
            }

            // Return the view for editing the comment
            return View(comment);
        }

        // Handle the submission of the edited comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="USER")]
        public IActionResult EditComment(Comment comment)
        {
            // Check if the user is authenticated and an admin
            ModelState.Remove("User");
            // Check if the model is valid
            if (ModelState.IsValid)
                {
                    // Update the session ID and the created date for the comment
                    comment.session_id = HttpContext.Session.Id;
                    comment.created_date = DateTime.Now;

                comment.comment_text = _sanitizer.Sanitize(comment.comment_text);





                // Check if the comment is empty after sanitization
                if (string.IsNullOrWhiteSpace(comment.comment_text))
                {
                    ModelState.AddModelError("comment_text", "Comment cannot be empty after sanitization.");
                    return View(comment);
                }

           

                _commentRepository.UpdateComment(comment);

                   

                  
                    return RedirectToAction("CommentList", new { productCode = comment.product_code });
                }
            
     
            return View(comment);
        }
    }
}