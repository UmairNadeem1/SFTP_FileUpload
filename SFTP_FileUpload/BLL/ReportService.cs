using SFTP_FileUpload.BLL.Interfaces;
using SFTP_FileUpload.Models;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;



namespace SFTP_FileUpload.BLL
{
    public class ReportService : IReportService
    {

        public async Task UplaodFile(string MainGuid)
        {
            string JobName = "UploadFilesToFTP";
            try
            {
                List<FileNames> listoffiles = new List<FileNames>();
                DataTable dataTable = new DataTable();
                int IsPendingFiles = Proc.IsPendingFiles(ref dataTable);
                if (IsPendingFiles > 0)
                {
                    listoffiles = new List<FileNames>();

                    foreach (DataRow dr in dataTable.Rows)
                    {
                        FileNames fileNames = new FileNames()
                        {
                            Name = dr.ItemArray[0].ToString()
                        };
                        listoffiles.Add(fileNames);
                    }

                    SFTPData SfTPData = new SFTPData();
                    SfTPData = Proc.GetSchedulerConfig("FTP");


                    foreach (var file in listoffiles)
                    {

                        file.fileType = Proc.GetFTPReportData(file.Name, out DataTable reportData);

                        ///Get Path where file will be uploaded.
                        if (!Directory.Exists(SfTPData.workingdirectory))  // if it doesn't exist, create
                            Directory.CreateDirectory(SfTPData.workingdirectory);


                        StreamWriter sw = null;
                        string fileName = file.Name + "_" + DateTime.Now.ToString("yyyy-MM-dd HH:mm ss") + file.fileType;
                        string path = System.IO.Path.Combine(SfTPData.workingdirectory, fileName);
                        sw = new StreamWriter(path, false);
                        CreateFile(reportData, sw, SfTPData, fileName, path);
                    }

                }
            }
            catch (Exception ex) { }
        }


        public void CreateFile(DataTable reportData, StreamWriter sw, SFTPData SfTPData, string fileName, string path)
        {
            try
            {
                string localGuid = Guid.NewGuid().ToString();
                int i = 0;
                for (i = 0; i < reportData.Columns.Count; i++)
                {

                    sw.Write(reportData.Columns[i].ColumnName + ",");
                }
                sw.WriteLine("\n");
                foreach (DataRow row in reportData.Rows)
                {
                    object[] array = row.ItemArray;

                    for (i = 0; i < array.Length - 1; i++)
                    {
                        sw.Write(array[i].ToString() + ",");
                    }
                    sw.Write(array[i].ToString() + ",");
                    sw.WriteLine();
                }
                sw.Flush();
                sw.Close();

                UploadFileToFTP(SfTPData, path, fileName);
            }
            catch (Exception ex)
            {

            }
        }


        public void UploadFileToFTP(SFTPData SfTPData, string filepath, string fileName)
        {

            bool isUploaded = false;
            string localGuid = Guid.NewGuid().ToString();
            using (var client = new SftpClient(SfTPData.host, SfTPData.port, SfTPData.username, SfTPData.password))
            {
                try
                {
                    client.Connect();
                    client.ChangeDirectory(SfTPData.workingdirectory);

                    var listDirectory = client.ListDirectory(SfTPData.workingdirectory);
                    using (var fileStream = new FileStream(filepath, FileMode.Open))
                    {
                        Console.WriteLine("Uploading {0} ({1:N0} bytes)", filepath, fileStream.Length);
                        client.BufferSize = 4 * 1024;
                        client.UploadFile(fileStream, Path.GetFileName(filepath), (o) =>
                        {
                            isUploaded = true;


                        });
                    }
                }
                catch (Exception ex) { }



                string emailBody = string.Empty;

                if (isUploaded == true)
                {
                    emailBody = fileName + " " + "Uploading Successful to Server.";
                }
                else
                {
                    emailBody = fileName + " " + "Uploading Unsuccessful to Server.";
                }

                try
                {
                    MailMessage message = new MailMessage(SfTPData.email_from, SfTPData.email_to, "SFTP Report Status", emailBody);

                    SmtpClient smtp = new SmtpClient();

                    smtp.Host = SfTPData.host;
                    smtp.Port = SfTPData.port;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(SfTPData.username, SfTPData.password);
                    smtp.EnableSsl = true;
                    message.IsBodyHtml = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(message);
                }
                catch (Exception ex)
                { //Log.LogError(ex.Message.ToString(), ex.StackTrace.ToString(), METHOD, "2035", "", "");
                }


            }


        }

    }

}   

