using CG.Web.MegaApiClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClassRoomClone_App.Server.Services.Implements
{
    public class MegaStorageService
    {
        private readonly string _megaEmail;
        private readonly string _megaPassword;

        public MegaStorageService(IConfiguration configuration)
        {
            _megaEmail = configuration["Mega:Email"] ?? throw new ArgumentNullException("Mega:Email");
            _megaPassword = configuration["Mega:Password"] ?? throw new ArgumentNullException("Mega:Password");
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var client = new MegaApiClient();
            await client.LoginAsync(_megaEmail, _megaPassword);

            // Get root node manually
            var nodes = await client.GetNodesAsync();
            var root = nodes.FirstOrDefault(n => n.Type == NodeType.Root);

            if (root == null)
                throw new Exception("Root node not found.");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            // Upload to root
            var uploadedNode = await client.UploadAsync(ms, file.FileName, root);

            // Generate public link
            var publicNode = await client.GetDownloadLinkAsync(uploadedNode);

            await client.LogoutAsync();

            return publicNode.ToString(); // returns URL
        }
    }
}