using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using nguyennhatquang_2280618967.Models;
using nguyennhatquang_2280618967.Repository;

namespace nguyennhatquang_2280618967.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductVideoRepository _productVideoRepository; // Thêm video repository

        public ProductController(IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IProductImageRepository productImageRepository,
            IProductVideoRepository productVideoRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _productImageRepository = productImageRepository;
            _productVideoRepository = productVideoRepository;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // Xử lý thêm sản phẩm mới với video
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl,
            List<IFormFile> additionalImages, List<IFormFile> productVideos)
        {
            if (ModelState.IsValid)
            {
                // Lưu hình ảnh đại diện
                if (imageUrl != null)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                // Thêm sản phẩm trước
                await _productRepository.AddAsync(product);

                // Lưu các hình ảnh bổ sung
                if (additionalImages != null && additionalImages.Count > 0)
                {
                    await SaveAdditionalImages(product.Id, additionalImages);
                }

                // Lưu các video
                if (productVideos != null && productVideos.Count > 0)
                {
                    await SaveProductVideos(product.Id, productVideos);
                }

                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Hàm SaveImage (giữ nguyên)
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName;
        }

        // Hàm lưu nhiều hình ảnh (giữ nguyên)
        private async Task SaveAdditionalImages(int productId, List<IFormFile> images)
        {
            foreach (var image in images)
            {
                if (image != null && image.Length > 0)
                {
                    var imageUrl = await SaveImage(image);
                    var productImage = new ProductImage
                    {
                        ProductId = productId,
                        Url = imageUrl
                    };
                    await _productImageRepository.AddAsync(productImage);
                }
            }
        }

        // Hàm lưu video
        private async Task SaveProductVideos(int productId, List<IFormFile> videos)
        {
            foreach (var video in videos)
            {
                if (video != null && video.Length > 0 && IsVideoFile(video))
                {
                    var videoUrl = await SaveVideo(video);
                    var productVideo = new ProductVideo
                    {
                        ProductId = productId,
                        Url = videoUrl,
                        ContentType = video.ContentType,
                        FileSize = video.Length,
                        Title = Path.GetFileNameWithoutExtension(video.FileName)
                    };
                    await _productVideoRepository.AddAsync(productVideo);
                }
            }
        }

        // Hàm lưu video file
        private async Task<string> SaveVideo(IFormFile video)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(video.FileName);
            var savePath = Path.Combine("wwwroot/videos", fileName);

            // Tạo thư mục videos nếu chưa có
            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await video.CopyToAsync(fileStream);
            }
            return "/videos/" + fileName;
        }

        // Kiểm tra file có phải video không
        private bool IsVideoFile(IFormFile file)
        {
            var videoMimeTypes = new[] { "video/mp4", "video/webm", "video/ogg", "video/avi", "video/mov" };
            return videoMimeTypes.Contains(file.ContentType.ToLower());
        }

        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Lấy tất cả hình ảnh và video của sản phẩm
            var productImages = await _productImageRepository.GetByProductIdAsync(id);
            var productVideos = await _productVideoRepository.GetByProductIdAsync(id);

            ViewBag.ProductImages = productImages;
            ViewBag.ProductVideos = productVideos;

            return View(product);
        }

        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            // Lấy hình ảnh và video hiện có
            var existingImages = await _productImageRepository.GetByProductIdAsync(id);
            var existingVideos = await _productVideoRepository.GetByProductIdAsync(id);

            ViewBag.ExistingImages = existingImages;
            ViewBag.ExistingVideos = existingVideos;

            return View(product);
        }

        // Xử lý cập nhật sản phẩm với video
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product, IFormFile imageUrl,
            List<IFormFile> additionalImages, List<IFormFile> productVideos,
            List<int> deleteImageIds, List<int> deleteVideoIds)
        {
            ModelState.Remove("ImageUrl");

            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);

                // Cập nhật hình ảnh đại diện
                if (imageUrl != null)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                else
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }

                // Cập nhật thông tin sản phẩm
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;

                await _productRepository.UpdateAsync(existingProduct);

                // Xóa hình ảnh được chọn
                if (deleteImageIds != null && deleteImageIds.Count > 0)
                {
                    foreach (var imageId in deleteImageIds)
                    {
                        await _productImageRepository.DeleteAsync(imageId);
                    }
                }

                // Xóa video được chọn
                if (deleteVideoIds != null && deleteVideoIds.Count > 0)
                {
                    foreach (var videoId in deleteVideoIds)
                    {
                        await _productVideoRepository.DeleteAsync(videoId);
                    }
                }

                // Thêm hình ảnh mới
                if (additionalImages != null && additionalImages.Count > 0)
                {
                    await SaveAdditionalImages(id, additionalImages);
                }

                // Thêm video mới
                if (productVideos != null && productVideos.Count > 0)
                {
                    await SaveProductVideos(id, productVideos);
                }

                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            // Lấy lại media hiện có khi có lỗi
            var existingImages = await _productImageRepository.GetByProductIdAsync(id);
            var existingVideos = await _productVideoRepository.GetByProductIdAsync(id);

            ViewBag.ExistingImages = existingImages;
            ViewBag.ExistingVideos = existingVideos;

            return View(product);
        }

        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Xóa tất cả hình ảnh của sản phẩm
            var productImages = await _productImageRepository.GetByProductIdAsync(id);
            foreach (var image in productImages)
            {
                await _productImageRepository.DeleteAsync(image.Id);
            }

            // Xóa tất cả video của sản phẩm
            var productVideos = await _productVideoRepository.GetByProductIdAsync(id);
            foreach (var video in productVideos)
            {
                await _productVideoRepository.DeleteAsync(video.Id);
            }

            // Xóa sản phẩm
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Action để xóa một video cụ thể
        [HttpPost]
        public async Task<IActionResult> DeleteVideo(int videoId, int productId)
        {
            await _productVideoRepository.DeleteAsync(videoId);
            return RedirectToAction("Update", new { id = productId });
        }

        // Action để xóa một hình ảnh cụ thể (giữ nguyên)
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId, int productId)
        {
            await _productImageRepository.DeleteAsync(imageId);
            return RedirectToAction("Update", new { id = productId });
        }


    }
}