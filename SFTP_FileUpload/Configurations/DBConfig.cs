using Microsoft.Extensions.Configuration;
using System;

namespace SFTP_FileUpload.Configurations
{
    public class DBConfig
    {
        public static string DefaultConnectionString
        {
            get
            {
                return Config.Configuration.GetConnectionString("DefaultConnection");
            }
        }
    }
}
