
using TypicalTechTools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TypicalTechTools.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Ganss.Xss;

namespace TypicalTools.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        HtmlSanitizer _sanitizer=new HtmlSanitizer();
        public ProductController(IProductRepository productRepository,HtmlSanitizer sanitizer)
        {
            _productRepository = productRepository;
            _sanitizer = sanitizer;
        }

        // Show all products
        public IActionResult Index()
        {
            // Retrieve all products using the repository
            var products = _productRepository.GetAllProducts();
            return View(products);
        }
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public IActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                // Set the created date for the product
                product.UpdatedDate = DateTime.Now;
                product.ProductName = _sanitizer.Sanitize(product.ProductName);
                product.ProductDescription = _sanitizer.Sanitize(product.ProductDescription);
                product.ProductCode = _sanitizer.Sanitize(product.ProductCode);

                if (string.IsNullOrWhiteSpace(product.ProductName) || string.IsNullOrWhiteSpace(product.ProductDescription) || string.IsNullOrWhiteSpace(product.ProductCode))
                {
                    ModelState.AddModelError(string.Empty, "Invalid Field Data");
                    return View(product);
                }



                // Add the product to the database
                _productRepository.CreateProduct(product);

                // Redirect to the product details page or the list of products
                return RedirectToAction("Index");
            }

            // If the model state is invalid, return the view with the current product model to show validation errors
            return View(product);
        }
        // Show details of a specific product
        public IActionResult Details(string productCode)
        {
           
            // Retrieve a specific product using the repository
            var product = _productRepository.GetProduct(productCode);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Display form to create a new product
        [Authorize(Roles = "ADMIN")]
        public IActionResult Create()
        {
            return View();
        }

        // Handle POST request to create a new product
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Create(Product product)
        {
            
                if (ModelState.IsValid)
                {
                product.UpdatedDate = DateTime.Now;
                product.ProductName = _sanitizer.Sanitize(product.ProductName);
                product.ProductDescription = _sanitizer.Sanitize(product.ProductDescription);
                product.ProductCode = _sanitizer.Sanitize(product.ProductCode);

                if (string.IsNullOrWhiteSpace(product.ProductName) || string.IsNullOrWhiteSpace(product.ProductDescription) || string.IsNullOrWhiteSpace(product.ProductCode))
                {
                    ModelState.AddModelError(string.Empty, "Invalid Field Data");
                    return View(product);
                }




                _productRepository.CreateProduct(product);
                    return RedirectToAction(nameof(Index));
                }
            
            return View(product);
        }

        // Display form to edit an existing product
        public IActionResult Edit(string productCode)
        {
            
            var product = _productRepository.GetProduct(productCode);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Handle POST request to update an existing product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                product.UpdatedDate = DateTime.Now;
                product.ProductName = _sanitizer.Sanitize(product.ProductName);
                product.ProductDescription = _sanitizer.Sanitize(product.ProductDescription);
                product.ProductCode = _sanitizer.Sanitize(product.ProductCode);


                if (string.IsNullOrWhiteSpace(product.ProductName) || string.IsNullOrWhiteSpace(product.ProductDescription) || string.IsNullOrWhiteSpace(product.ProductCode))
                {
                    ModelState.AddModelError(string.Empty, "Invalid Field Data");
                    return View(product);
                }


                _productRepository.UpdateProduct(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

     
       

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public IActionResult UpdatePrice(string productCode)
        {
            
            var product =_productRepository.GetProduct(productCode);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePrice(Product product)
        {
            
                product.UpdatedDate = DateTime.UtcNow;

           


            _productRepository.UpdateProductPrice(product.ProductCode, product.ProductPrice);
                return RedirectToAction("Index", "Product");
           
        }

    }
}