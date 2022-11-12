using DMNRestaurant.Services.Repository.IRepository;

namespace DMNRestaurant.Services.Repository
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment webHostEnv;

        public PhotoRepository(IConfiguration config, IWebHostEnvironment webHostEnv)
        {
            this.config = config;
            this.webHostEnv = webHostEnv;
        }

        /************************************************************************
         *                        Check File Upload
         ***********************************************************************/
        public bool IsUpdaload(IFormFile? file) => file != null && file.Length > 0;

        /************************************************************************
         *                      Validate Extension File
         ***********************************************************************/
        public bool ValidateExtension(string fileName)
        {
            string[] permittedExtension = { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(fileName);

            if (String.IsNullOrEmpty(ext) && !permittedExtension.Contains(ext))
                return false;

            return true;
        }

        /************************************************************************
         *                      Validate Size of File
         ***********************************************************************/
        public bool ValidateFileSize(long fileSize)
        {
            return fileSize > config.GetValue<long>("FileSettings:FileSizeLimit");
        }

        /************************************************************************
         *                  Validate Size & Extension Of File
         ***********************************************************************/
        public string? Validation(IFormFile file)
        {
            if (ValidateFileSize(file.Length))
                return "The file is to large";
            if (!ValidateExtension(file.FileName))
                return "Invalid file extension";

            return null;
        }

        /************************************************************************
         *                    Remove Old File From Path
         ***********************************************************************/
        public bool Remove(string fileName, string fromPath)
        {
            var rootPath = $"{webHostEnv.WebRootPath}/images/{fromPath}/";
            var fullPath = Path.Combine(rootPath, fileName);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                return true;
            }

            return false;
        }

        /************************************************************************
         *                 Uploading The File And Return File Name
         ***********************************************************************/
        public async Task<string> UploadAsync(IFormFile file, string toPath)
        {
            var originFileName = Path.GetFileName(file.FileName);
            var fileExts = Path.GetExtension(originFileName);
            var newFileName = Guid.NewGuid().ToString() + fileExts;
            var fullPath = $"{webHostEnv.WebRootPath}/images/{toPath}/{newFileName}";

            using (FileStream fs = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(fs);
                await fs.FlushAsync();
            }

            return newFileName;
        }

    }
}
