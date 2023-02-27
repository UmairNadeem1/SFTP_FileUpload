using SFTP_FileUpload.Configurations;
using SFTP_FileUpload.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace SFTP_FileUpload
{
    public class Proc
    {


        public static SFTPData GetSchedulerConfig(string Key)
        {
            string METHOD = "Proc/GetFTPCofig";
            string PKG_PRC = "PRC_GET_SCHEDULER_CONFIG";

            SFTPData SftpData = new SFTPData();

            MySqlConnection conn = new MySqlConnection(DBConfig.DefaultConnectionString);
            MySqlCommand cmd = new MySqlCommand(PKG_PRC, conn);


            cmd.CommandType = CommandType.StoredProcedure;

            cmd.CommandTimeout = 9999;

            cmd.Parameters.Add(new MySqlParameter { ParameterName = "PKEY", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Input, Value = Key });

            cmd.Parameters.Add(new MySqlParameter { ParameterName = "PHOST", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new MySqlParameter { ParameterName = "PPORT", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new MySqlParameter { ParameterName = "PUSERNAME", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new MySqlParameter { ParameterName = "PPASSWORD", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new MySqlParameter { ParameterName = "PEMAIL_TO", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new MySqlParameter { ParameterName = "PEMAIL_FROM", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new MySqlParameter { ParameterName = "PFTP_WORKING_DIRECTORY", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output });

            cmd.Parameters.Add(new MySqlParameter { ParameterName = "PCODE", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new MySqlParameter { ParameterName = "PDESC", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new MySqlParameter { ParameterName = "PMSG", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output });

            try
            {
                conn.Close();
                conn.Open();

                cmd.ExecuteNonQuery();


                string sCODE = cmd.Parameters["PCODE"].Value != null ? cmd.Parameters["PCODE"].Value.ToString() : "";
                string sDESC = cmd.Parameters["PDESC"].Value != null ? cmd.Parameters["PDESC"].Value.ToString() : "";
                string sMSG = cmd.Parameters["PMSG"].Value != null ? cmd.Parameters["PMSG"].Value.ToString() : "";




                if (sMSG == "Y")
                {
                    SftpData.host = cmd.Parameters["PHOST"].Value != null ? (cmd.Parameters["PHOST"]).Value.ToString() : "";
                    SftpData.port = Convert.ToInt32(cmd.Parameters["PPORT"].Value);
                    SftpData.username = cmd.Parameters["PUSERNAME"].Value != null ? (cmd.Parameters["PUSERNAME"]).Value.ToString() : "";
                    SftpData.password = cmd.Parameters["PPASSWORD"].Value != null ? (cmd.Parameters["PPASSWORD"]).Value.ToString() : "";
                    SftpData.email_to = cmd.Parameters["PEMAIL_TO"].Value != null ? (cmd.Parameters["PEMAIL_TO"]).Value.ToString() : "";
                    SftpData.email_from = cmd.Parameters["PEMAIL_FROM"].Value != null ? (cmd.Parameters["PEMAIL_FROM"]).Value.ToString() : "";
                    SftpData.workingdirectory = cmd.Parameters["PFTP_WORKING_DIRECTORY"].Value != null ? (cmd.Parameters["PFTP_WORKING_DIRECTORY"]).Value.ToString() : "";

                }
                else
                {



                }
                conn.Close();

            }
            catch (Exception ex)
            {




            }
            finally
            {

                cmd.Dispose();
                cmd = null;
                conn.Dispose();
                conn = null;

            }
            return SftpData;


        }


        public static int IsPendingFiles(ref DataTable dataTable)
        {
            string localGuid = Guid.NewGuid().ToString();
            string PKG_PRC = "PRC_CHECK_PENDING_FILES";

            int count = 0;
            try
            {
                using MySqlConnection conn = new MySqlConnection(DBConfig.DefaultConnectionString);
                conn.Open();
                using MySqlCommand cmd = new MySqlCommand(PKG_PRC, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 9999;

                cmd.Parameters.Add("PCODE", MySqlDbType.VarChar, 250).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("PDESC", MySqlDbType.VarChar, 4000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("PMSG", MySqlDbType.VarChar, 250).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                dataTable = new DataTable();
                da.Fill(dataTable);
                count = dataTable.Rows.Count;

                
                string sCODE = cmd.Parameters["PCODE"].Value?.ToString() ?? "";
                string sDESC = cmd.Parameters["PDESC"].Value?.ToString() ?? "";
                string sMSG = cmd.Parameters["PMSG"].Value?.ToString() ?? "";
                
                if (sMSG != "Y" || sCODE != "00")
                {
                    // _logger.LogDebug($"Error occured in {PKG_PRC}-{sCODE}-{sMSG}-{sDESC}");
                }
            }
            catch (Exception ex)
            {
                // _logger.LogDebug(ex, $"Error occured in CheckServiceStatus");
            }
            //DebugJobLog(MainGuid, localGuid, PKG_PRC, JobName, "End  Checking Scheduler Run Time", "");

            return count;

        }


        public static string GetFTPReportData(string report, out DataTable reportData)
        {
            string localGuid = Guid.NewGuid().ToString();
            string PKG_PRC = "PRC_GET_FTP_REPORT";
            string fileType = string.Empty;
            reportData = new DataTable();
            try
            {

           
                using MySqlConnection conn = new MySqlConnection(DBConfig.DefaultConnectionString);
                conn.Open();
                using MySqlCommand cmd = new MySqlCommand(PKG_PRC, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 9999;

                cmd.Parameters.Add(new MySqlParameter { ParameterName = "PREPORT", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Input, Value = report });              
                cmd.Parameters.Add(new MySqlParameter { ParameterName = "PSYSTEM", MySqlDbType = MySqlDbType.Int64, Direction = ParameterDirection.Input, Value = "" });
                cmd.Parameters.Add(new MySqlParameter { ParameterName = "PFILE_TYPE", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output });
                cmd.Parameters.Add("PSYSTEM", MySqlDbType.Int64, 250).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("PCODE", MySqlDbType.VarChar, 250).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("PDESC", MySqlDbType.VarChar, 4000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("PMSG", MySqlDbType.VarChar, 250).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                string sCODE = cmd.Parameters["PCODE"].Value?.ToString() ?? "";
                string sDESC = cmd.Parameters["PDESC"].Value?.ToString() ?? "";
                string sMSG = cmd.Parameters["PMSG"].Value?.ToString() ?? "";
                

                if (sMSG == "Y" )
                {
                    fileType = cmd.Parameters["PFILE_TYPE"].Value != null ? (cmd.Parameters["PFILE_TYPE"]).Value.ToString() : "";
                }
            }
            catch (Exception ex)
            {
                // _logger.LogDebug(ex, $"Error occured in CheckServiceStatus");
            }
            //DebugJobLog(MainGuid, localGuid, PKG_PRC, JobName, "End  Checking Scheduler Run Time", "");


            return fileType;
        }

        public static string FTPStatusUpdate(string generalDto)
        {
            string response = string.Empty;
           string  Method = "Proc/FTPStatusUpdate";
           string  PKG_PRC = "PRC_FTP_STATUS_UPD";

            using MySqlConnection conn = new MySqlConnection(DBConfig.DefaultConnectionString);
            conn.Open();
            using MySqlCommand cmd = new MySqlCommand(PKG_PRC, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 9999;
            try
            {
                cmd.Parameters.Add(new MySqlParameter { ParameterName = "PAPP_KEY", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Input});
                cmd.Parameters.Add(new MySqlParameter { ParameterName = "PSYSTEM_ID", MySqlDbType = MySqlDbType.Int64, Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new MySqlParameter { ParameterName = "PWSCODE", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new MySqlParameter { ParameterName = "PWSDESC", MySqlDbType = MySqlDbType.VarChar, Direction = ParameterDirection.Output });
                cmd.Parameters.Add("PCODE", MySqlDbType.VarChar, 250).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("PDESC", MySqlDbType.VarChar, 4000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("PMSG", MySqlDbType.VarChar, 250).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                conn.Dispose();
                cmd.Dispose();
            }
            return response;
        }
    }

}
