using System;

namespace SFTP_FileUpload.Models
{
    public class SFTPData
    {
        public int port { get; set; }
        public String host { get; set; }
        public String username { get; set; }
        public String password { get; set; }
        public String email_to { get; set; }
        public String email_from { get; set; }
        public String workingdirectory { get; set; }
    }
}
