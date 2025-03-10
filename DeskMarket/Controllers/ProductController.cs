using DeskMarket.Data;
using DeskMarket.Data.Models;
using DeskMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DeskMarket.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Where(p => !p.IsDeleted)
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .ToListAsync();

            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            var categories = await _context.Categories.ToListAsync();
            var model = new ProductFormViewModel
            {
                Categories = categories
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.ToListAsync();
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = new Product
            {
                ProductName = model.ProductName,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                CategoryId = model.CategoryId,
                SellerId = userId,
                AddedOn = DateTime.Now
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Where(p => !p.IsDeleted && p.Id == id)
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            bool isInCart = false;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                isInCart = await _context.ProductsClients
                    .AnyAsync(pc => pc.ProductId == id && pc.ClientId == userId);
            }

            var model = new ProductDetailsViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category.Name,
                SellerName = product.Seller.UserName,
                SellerId = product.SellerId,
                AddedOn = product.AddedOn,
                IsInCart = isInCart
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products
                .Where(p => !p.IsDeleted && p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.SellerId != userId)
            {
                return Unauthorized();
            }

            var categories = await _context.Categories.ToListAsync();

            var model = new ProductFormViewModel
            {
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                AddedOn = product.AddedOn,
                Categories = categories
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
        {
            var product = await _context.Products
                .Where(p => !p.IsDeleted && p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.SellerId != userId)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.ToListAsync();
                return View(model);
            }

            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.Price = model.Price;
            product.ImageUrl = model.ImageUrl;
            product.CategoryId = model.CategoryId;
            product.AddedOn = model.AddedOn;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = product.Id });
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Where(p => !p.IsDeleted && p.Id == id)
                .Include(p => p.Category)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.SellerId != userId)
            {
                return Unauthorized();
            }

            var model = new ProductDeleteViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                CategoryName = product.Category.Name
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Where(p => !p.IsDeleted && p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.SellerId != userId)
            {
                return Unauthorized();
            }

            product.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _context.Products
                .Where(p => !p.IsDeleted && p.Id == id)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (product.SellerId == userId)
            {
                return BadRequest("You cannot buy your own product.");
            }

            var isInCart = await _context.ProductsClients
                .AnyAsync(pc => pc.ProductId == id && pc.ClientId == userId);

            if (isInCart)
            {
                return RedirectToAction(nameof(Cart));
            }

            var productClient = new ProductClient
            {
                ProductId = id,
                ClientId = userId
            };

            await _context.ProductsClients.AddAsync(productClient);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Cart));
        }

        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var productClient = await _context.ProductsClients
                .FirstOrDefaultAsync(pc => pc.ProductId == id && pc.ClientId == userId);

            if (productClient == null)
            {
                return NotFound();
            }

            _context.ProductsClients.Remove(productClient);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Cart));
        }

        [Authorize]
        public async Task<IActionResult> Cart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var products = await _context.ProductsClients
                .Where(pc => pc.ClientId == userId)
                .Select(pc => new ProductCartViewModel
                {
                    Id = pc.Product.Id,
                    ProductName = pc.Product.ProductName,
                    Description = pc.Product.Description,
                    Price = pc.Product.Price,
                    ImageUrl = pc.Product.ImageUrl,
                    CategoryName = pc.Product.Category.Name
                })
                .ToListAsync();

            return View(products);
        }
    }
} 