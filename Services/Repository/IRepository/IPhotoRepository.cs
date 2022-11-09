namespace DMNRestaurant.Services.Repository.IRepository
{
    public interface IPhotoRepository
    {
        public bool IsUpdaload(IFormFile file);
        public bool ValidateFileSize(long fileSize);
        public bool ValidateExtension(string fileName);
        public string? Validation(IFormFile file);
        public Task<string> Upload(IFormFile file);
        public bool Remove(string fileName);
    }
}
