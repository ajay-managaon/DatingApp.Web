using Azure.Storage.Blobs;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;

namespace Web.DatingApp.API.Web.DatingApp.Interfaces.Repositories
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
