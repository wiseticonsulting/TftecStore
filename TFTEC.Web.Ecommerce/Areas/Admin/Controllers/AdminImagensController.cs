using TFTEC.Web.Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Configuration;

namespace TFTEC.Web.Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminImagensController : Controller
    {
        private readonly ConfigurationImagens _myConfig;

        private readonly IWebHostEnvironment _hostingEnvironment;

        const string blobContainerName = "blob-tftec-container";
        private readonly BlobServiceClient blobServiceClient;
        static BlobContainerClient blobContainer;

        public AdminImagensController(IWebHostEnvironment hostingEnvironment,
            IOptions<ConfigurationImagens> myConfiguration, BlobServiceClient blobServiceClient)
        {
            _hostingEnvironment = hostingEnvironment;
            _myConfig = myConfiguration.Value;
            this.blobServiceClient = blobServiceClient;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                // Retrieve storage account information from connection string
                // How to create a storage connection string - http://msdn.microsoft.com/en-us/library/azure/ee758697.aspx

                blobContainer = blobServiceClient.GetBlobContainerClient(blobContainerName);
                await blobContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);

                // To view the uploaded blob in a browser, you have two options. The first option is to use a Shared Access Signature (SAS) token to delegate  
                // access to the resource. See the documentation links at the top for more information on SAS. The second approach is to set permissions  
                // to allow public access to blobs in this container. Comment the line below to not use this approach and to use SAS. Then you can view the image  
                // using: https://[InsertYourStorageAccountNameHere].blob.core.windows.net/webappstoragedotnet-imagecontainer/FileName 

                // Gets all Block Blobs in the blobContainerName and passes them to the view
                List<Uri> allBlobs = new List<Uri>();
                foreach (BlobItem blob in blobContainer.GetBlobs())
                {
                    if (blob.Properties.BlobType == BlobType.Block)
                        allBlobs.Add(blobContainer.GetBlobClient(blob.Name).Uri);
                }

                return View(allBlobs);
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                ViewData["trace"] = ex.StackTrace;
                return View("Error");
            }
        }

        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                ViewData["Erro"] = "Error: Arquivo(s) não selecionado(s)";
                return View(ViewData);
            }

            if (files.Count > 10)
            {
                ViewData["Erro"] = "Error: Quantidade de arquivos excedeu o limite";
                return View(ViewData);
            }

            foreach (var formFile in files)
            {
                if (formFile.FileName.Contains(".jpg") || formFile.FileName.Contains(".gif") ||
                         formFile.FileName.Contains(".png"))
                {
                    var fileName = Path.GetFileName(formFile.FileName);
                    var fileType = Path.GetExtension(fileName);
                    var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileType);

                    Stream stream = formFile.OpenReadStream();

                    BlobClient blob = blobContainer.GetBlobClient(formFile.FileName);

                    blob.Upload(stream);
                    var fileUrl = blob.Uri.AbsoluteUri;
                }
            }

            //monta a ViewData que será exibida na view como resultado do envio 
            ViewData["Resultado"] = $"{files.Count} arquivos foram enviados ao blobStorage!";

            //ViewBag.Arquivos = filePathsName;

            //retorna a viewdata
            return View(ViewData);
        }

        public async Task<ActionResult> GetImagens()
        {
            //FileManagerModel model = new FileManagerModel();
            //var userImagesPath = Path.Combine(_hostingEnvironment.WebRootPath,
            //     _myConfig.NomePastaImagensProdutos);


            blobContainer = blobServiceClient.GetBlobContainerClient(blobContainerName);
            await blobContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Gets all Block Blobs in the blobContainerName and passes them to the view
            List<Uri> allBlobs = new List<Uri>();
            foreach (BlobItem blob in blobContainer.GetBlobs())
            {
                if (blob.Properties.BlobType == BlobType.Block)
                    allBlobs.Add(blobContainer.GetBlobClient(blob.Name).Uri);
            }

            //DirectoryInfo dir = new DirectoryInfo(userImagesPath);
            //FileInfo[] files = dir.GetFiles();
            //model.PathImagesProduto = _myConfig.NomePastaImagensProdutos;

            if (allBlobs.Count() == 0)
            {
                ViewData["Erro"] = $"Nenhum arquivo encontrado na pasta do blob";
            }

            //model.Files = files;
            //return View(model);

            return View(allBlobs);
        }

        public IActionResult Deletefile(string fname)
        {
            string _imagemDeleta = Path.Combine(_hostingEnvironment.WebRootPath,
                _myConfig.NomePastaImagensProdutos + "\\", fname);

            if ((System.IO.File.Exists(_imagemDeleta)))
            {
                System.IO.File.Delete(_imagemDeleta);
                ViewData["Deletado"] = $"Arquivo(s) {_imagemDeleta} deletado com sucesso";
            }
            return View("index");
        }
    }
}
