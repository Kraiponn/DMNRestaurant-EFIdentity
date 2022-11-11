namespace DMNRestaurant.Services.Repository.IRepository
{
    public interface IPhotoRepository
    {
        public bool IsUpdaload(IFormFile file);
        public bool ValidateFileSize(long fileSize);
        public bool ValidateExtension(string fileName);
        public string? Validation(IFormFile file);
        public Task<string> UploadAsync(IFormFile file, string toPath);
        public bool Remove(string fileName, string fromPath);
    }
}
