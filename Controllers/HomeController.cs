using Microsoft.AspNetCore.Mvc;
using ST10369044Semerster2WebApp.Models;
using ST10369044Semerster2WebApp.Services;
using System.Diagnostics;

namespace ST10369044Semerster2WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlobService _blobService;
        private readonly CustomerServices _customerService;
        private readonly QueueService _queueService;
        private readonly FileService _fileService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger, IConfiguration configuration, CustomerServices customerService, BlobService blobService, QueueService queueService, FileService fileService)
        {
            _blobService = blobService;
            _customerService = customerService;
            _queueService = queueService;
            _fileService = fileService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var model = new CustomerProfile();
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
                try
                {
                    using var httpClient = _httpClientFactory.CreateClient();
                    var baseUrl = _configuration["AzureFunction:StoreTableInfo"];
                    var requestUrl = $"{baseUrl}&tableName=CustomerProfiles&partitionKey={profile.PartitionKey}&rowKey={profile.RowKey}&firstName={profile.FirstName}&lastName={profile.LastName}&phoneNumber={profile.PhoneNumber}&Email={profile.Email}";
                    var response = await httpClient.PostAsync(requestUrl, null);
                    if (response.IsSuccessStatusCode)
                    {
                        await _customerService.InsertCustomerAsync(profile);
                        return RedirectToAction("Index");
                    }
                    else 
                    {
                        _logger.LogError($"Error submitting client info: {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error submitting client info: {ex.Message}");
                }
                return View("Index", profile);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
