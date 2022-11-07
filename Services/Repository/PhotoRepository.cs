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

        public bool IsUpdaload(IFormFile file)
            => file != null && file.Length > 0;

        public bool Remove(string fileName)
        {
            var rootPath = $"{webHostEnv.WebRootPath}/images/";
            var fullPath = Path.Combine(rootPath, fileName);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                return true;
            }

            return false;
        }

        public Task<bool> ReplaceImage(string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Upload(IFormFile file)
        {
            var originFileName = Path.GetFileName(file.FileName);
            var fileExts = Path.GetExtension(originFileName);
            var newFileName = Guid.NewGuid().ToString() + fileExts;
            var fullPath = $"{webHostEnv.WebRootPath}/images/{newFileName}";

            using (FileStream fs = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(fs);
                await fs.FlushAsync();
            }

            return newFileName;
        }

        public bool ValidateExtension(string fileName)
        {
            string[] permittedExtension = { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(fileName);

            if (String.IsNullOrEmpty(ext) && !permittedExtension.Contains(ext))
                return false;

            return true;
        }

        public bool ValidateFileSize(long fileSize)
        {
            return fileSize > config.GetValue<long>("FileSettings:FileSizeLimit");
        }

        public string? Validation(IFormFile file)
        {
            if (ValidateFileSize(file.Length))
                return "The file is to large";
            if (!ValidateExtension(file.FileName))
                return "Invalid file extension";

            return null;
        }
    }
}
