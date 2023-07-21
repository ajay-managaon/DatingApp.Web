using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Web.DatingApp.API.Web.DatingApp.Interfaces.Repositories;

namespace Web.DatingApp.API.Web.DatingApp.Implenentations.Implementations
{
    public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient blobServiceClient;

        public ContainerService(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }
        public string GetContainer()
        {
            var container = new List<string>();
            foreach (BlobContainerItem item in blobServiceClient.GetBlobContainers())
            {
                container.Add(item.Name);
            }
            return container.FirstOrDefault();
        }
    }
}
