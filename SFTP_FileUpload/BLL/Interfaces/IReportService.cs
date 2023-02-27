using SFTP_FileUpload.Models;
using System.Threading.Tasks;

namespace SFTP_FileUpload.BLL.Interfaces
{
    public interface IReportService
    {
        public Task UplaodFile(string MainGuid);

        public void UploadFileToFTP( SFTPData sFTPData, string filepath, string fileName);
    }
}