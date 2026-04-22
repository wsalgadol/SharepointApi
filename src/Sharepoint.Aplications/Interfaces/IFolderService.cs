
namespace Sharepoint.Aplications.Interfaces
{
    public interface IFolderService
    {
        void EnsureFolder(string absoluteUrl);

        (string siteCollectionUrl,  string serverRelativeFolder) ParseFolderUrl(string fileUrl);

    }
}
