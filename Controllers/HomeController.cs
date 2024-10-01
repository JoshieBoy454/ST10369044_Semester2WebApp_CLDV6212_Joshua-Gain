using Microsoft.AspNetCore.Mvc;
using ST10369044Semerster2WebApp.Models;
using ST10369044Semerster2WebApp.Services;
using System.Diagnostics;

namespace ST10369044Semerster2WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlobService _blobService;
        private readonly TableService _tableService;
        private readonly QueueService _queueService;
        private readonly FileService _fileService;

        public HomeController(BlobService blobService, TableService tableService, QueueService queueService, FileService fileService)
        {
            _blobService = blobService;
            _tableService = tableService;
            _queueService = queueService;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SubmitAllForms(IFormFile ImageFile, IFormFile ContractFile, CustomerProfile profile, string OrderId)
        {
            // Handle Image Upload
            if (ImageFile != null)
            {
                using var stream = ImageFile.OpenReadStream();
                await _blobService.UploadBlobAsync("product-images", ImageFile.FileName, stream);
            }

            // Handle Customer Profile
            if (ModelState.IsValid)
            {
                await _tableService.AddEntityAsync(profile);
            }

            // Handle Order Processing
            if (!string.IsNullOrEmpty(OrderId))
            {
                await _queueService.SendMessageAsync("order-processing", $"Processing order {OrderId}");
            }

            // Handle Contract Upload
            if (ContractFile != null)
            {
                using var stream = ContractFile.OpenReadStream();
                await _fileService.UploadFileAsync("contracts-logs", ContractFile.FileName, stream);
            }

            return RedirectToAction("Index");
        }

        //[HttpPost]
        //public async Task<IActionResult> UploadImage(IFormFile file)
        //{
        //    if (file != null)
        //    {
        //        using var stream = file.OpenReadStream();
        //        await _blobService.UploadBlobAsync("product-images", file.FileName, stream);
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public async Task<IActionResult> AddCustomerProfile(CustomerProfile profile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _tableService.AddEntityAsync(profile);
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public async Task<IActionResult> ProcessOrder(string orderId)
        //{
        //    await _queueService.SendMessageAsync("order-processing", $"Processing order {orderId}");
        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //public async Task<IActionResult> UploadContract(IFormFile file)
        //{
        //    if (file != null)
        //    {
        //        using var stream = file.OpenReadStream();
        //        await _fileService.UploadFileAsync("contracts-logs", file.FileName, stream);
        //    }
        //    return RedirectToAction("Index");
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
