using Benz.log;
using Call_Details_API.Helpers;
using Call_Details_API.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PushAPI.Model;
using Renci.SshNet;
using RestSharp;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static PushAPI.Model.TBL_CALLDETAILS;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace PushAPIContractNumber
{
    [ApiController]

    [Route("api/v1/[controller]")]
    public class ivrController : Controller
    {
        private readonly IRestClient _restClient;
        private readonly string _dbConnection;
        private readonly string UploadFolder = Path.Combine("/uploads"); // Server uploads folder
        private Log lg;
        private readonly string _host = "192.168.5.61";
        private readonly int _port = 22;
        private readonly string _username = "root";
        private readonly string _password = "Kaizen%$#@!";

        #region
        //[HttpPost("GetEnrolDetailsByContractID")]
        //public IActionResult GetEnrolData([FromForm] string Contract_ID)
        //{
        //	string response = string.Empty;
        //          string Query = "";
        //          var results = new List<Enrollment>();
        //          Log lg = new Log();
        //          string status = "";

        //          try
        //          {



        //              if ((Contract_ID != ""))
        //              {

        //                  lg.lodwrite("----entry----");



        //                      Query = "SELECT top 1 * FROM TBL_ENROLLMENT WHERE var_contractno = '" + Contract_ID + "'ORDER BY var_called_date DESC";




        //                  using (SqlConnection connection = new SqlConnection(_dbConnection))
        //                  {
        //                      connection.Open();

        //                      lg.lodwrite("ConnectionOpen");

        //                          using (SqlCommand selectCommand = new SqlCommand(Query, connection))
        //                          {
        //                              // Set the parameters
        //                              selectCommand.Parameters.AddWithValue("@var_contractno", Contract_ID);
        //                          //selectCommand.Parameters.AddWithValue("@VAR_ContractNumber", ContractNumber);

        //                          // Execute the select command
        //                          using (SqlDataReader reader = selectCommand.ExecuteReader())
        //                              {
        //                                  if (reader.HasRows)
        //                                  {
        //                                  lg.lodwrite("--ifcondtionentry--");
        //                                  for (int i = 0; i < reader.FieldCount; i++)
        //                                      {
        //                                          while (reader.Read())
        //                                          {

        //                                          status = reader["var_BVPstatus"].ToString();
        //                                          lg.lodwrite("--Enter into while Loop--");

        //                                          Enrollment data = new Enrollment
        //                                              {
        //                                                  //var_called_date = reader["var_called_date"].ToString(),
        //                                                  //var_ContractNumber = reader["var_ContractNumber"].ToString(),
        //                                                  var_contract_id = reader["var_contractno"].ToString(),
        //                                              var_conversation_id = reader["var_conversationID"].ToString(),

        //                                              var_registered_status = reader["var_BVPstatus"].ToString(),


        //                                              };

        //                                          if(status== "Not Sufficient Voice data")
        //                                          {
        //                                              data.var_registered_status = "Not Sufficient Voice data";
        //                                          }
        //                                          else if(status == "Already Enrolled")
        //                                          {
        //                                              data.var_registered_status = "Already Enrolled";
        //                                          }
        //                                          else if(status== "Failed")
        //                                          {
        //                                              data.var_registered_status = "Failed";
        //                                          }
        //                                          else
        //                                          {
        //                                              data.var_registered_status = "Success";
        //                                          }

        //                                              results.Add(data);
        //                                              // Process each row
        //                                              // For example, retrieve data:

        //                                              // Do something with ContractNumber, like logging or further processing
        //                                          }
        //                                      }


        //                                  }
        //                                  else
        //                                  {
        //                                  //response = "No data";
        //                                  Enrollment ed = new Enrollment();
        //                                  ed.var_contract_id ="";
        //                                  ed.var_conversation_id = "";
        //                                  ed.var_registered_status ="";
        //                                      results.Add(ed);
        //                                      return Ok(results);
        //                                  }
        //                              }
        //                          }

        //                          connection.Close();


        //                  }



        //              }
        //              else
        //              {
        //                  response = "ContractNumber is Empty";
        //              return Ok(response);
        //              }
        //          }
        //          catch (Exception ex) {
        //              lg.lodwrite("Exception---"+ex.Message.ToString()+"---");
        //              return(Ok(ex.Message));
        //          }



        //          return Ok(results);





        //}








        //    return Ok(results);





        //}
        #endregion

        public ivrController(IConfiguration configuration)
        {
            //string dbcon=""
            Log lg = new Log();
            lg.lodwrite("databaseconnection");
            _dbConnection = configuration.GetConnectionString("dbcon");
        }
        private void UpdateRowscalldetails(string data)
        {
            using (SqlConnection sqlcon = new SqlConnection(_dbConnection))
            {
                sqlcon.Open();
                using (SqlCommand updateCmd = new SqlCommand("UPDATE TBL_CALLDETAILS SET VAR_STATUS = '2' WHERE VAR_UNIQUE_ID = '" + data + "'", sqlcon))
                {
                    updateCmd.Parameters.AddWithValue("@VAR_STATUS", "2");
                    updateCmd.Parameters.AddWithValue("@VAR_UNIQUE_ID", data ?? string.Empty);
                    updateCmd.ExecuteNonQuery();
                }
                sqlcon.Close();
            }
        }

        // GetBy Callerhistory
        [HttpPost("callerhistory")]
        public IActionResult GetCallDetailsbycallerid([FromForm] string Caller_id)
        {
            string response = string.Empty;
            Int64 score = 0;
            Log lg = new Log();

            var results = new List<TBL_CALLDETAILS>();
            try
            {
                if (Caller_id != "")
                {

                    lg.lodwrite("---Entrymethod----");

                    string insetQuery = "SELECT  * FROM TBL_CALLDETAILS WHERE VAR_CALLER_ID = '" + Caller_id + "' and var_status='1' and VAR_IVR_END_TIME is not null ORDER BY var_called_date DESC";

                    using (SqlConnection connection = new SqlConnection(_dbConnection))
                    {
                        connection.Open();

                        using (SqlCommand selectCommand = new SqlCommand(insetQuery, connection))
                        {
                            // Set the parameters
                            selectCommand.Parameters.AddWithValue("@VAR_CALLER_ID", Caller_id);

                            // Execute the select command
                            using (SqlDataReader reader = selectCommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        lg.lodwrite(reader.FieldCount.ToString());
                                        while (reader.Read())
                                        {
                                            TBL_CALLDETAILS data = new TBL_CALLDETAILS
                                            {
                                                //SNO = Convert.ToInt32(reader["SNO"]),
                                                called_date = reader["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_CALLED_DATE"]),
                                                caller_id = reader["VAR_CALLER_ID"]?.ToString(),
                                                unique_id = reader["VAR_UNIQUE_ID"]?.ToString(),
                                                //ivr_starttime = reader["VAR_PATCH_START_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_PATCH_START_TIME"]),
                                                //ivr_endtime = reader["VAR_PATCH_END_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_PATCH_END_TIME"]),
                                                ivr_starttime = reader["VAR_IVR_START_TIME"]?.ToString(),
                                                ivr_endtime = reader["VAR_IVR_END_TIME"]?.ToString(),
                                                duration = reader["VAR_DURATION"]?.ToString(),
                                                dnis = reader["VAR_DNIS"]?.ToString(),
                                                Call_Type = reader["VAR_CALL_TYPE"]?.ToString(),


                                                // transfer_status = reader["var_transferstatus"]?.ToString(),

                                            };
                                            // UpdateRows(data.unique_id);
                                            results.Add(data);

                                        }
                                    }
                                }
                                else
                                {
                                    return Ok("Nodata");
                                }
                            }
                        }
                        connection.Close();
                    }
                }
                else
                {
                    lg.lodwrite("Empty");
                    return Ok("ContractNumber or conversationID is Empty");
                }
            }
            catch (Exception ex)
            {
                lg.lodwrite(ex.Message.ToString());
            }

            return Ok(results);
        }


        /// GetBy AgentId
        [HttpPost("agenthistory")]
        public IActionResult getcalltranferdetailsbyagentid([FromForm] string agent_id)
        {
            string response = string.Empty;
            Int64 score = 0;
            Log lg = new Log();

            var results = new List<TBL_CALL_TRANSFER>();
            try
            {
                if (agent_id != "")
                {
                    lg.lodwrite("---Entrymethod----");

                    string insetQuery = "SELECT * FROM TBL_CALL_TRANSFER WHERE VAR_TRANSFERVDN = '" + agent_id + "' and var_status='1' and VAR_PATCH_END_TIME is not null ORDER BY var_called_date DESC";

                    using (SqlConnection connection = new SqlConnection(_dbConnection))
                    {
                        connection.Open();

                        using (SqlCommand selectCommand = new SqlCommand(insetQuery, connection))
                        {
                            // Set the parameters
                            selectCommand.Parameters.AddWithValue("@VAR_CALLER_ID", agent_id);

                            // Execute the select command
                            using (SqlDataReader reader = selectCommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        lg.lodwrite(reader.FieldCount.ToString());
                                        while (reader.Read())
                                        {
                                            TBL_CALL_TRANSFER data_ = new TBL_CALL_TRANSFER
                                            {
                                                //SNO = Convert.ToInt32(reader["SNO"]),
                                                called_date = reader["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_CALLED_DATE"]),
                                                caller_id = reader["VAR_CALLER_ID"]?.ToString(),
                                                unique_id = reader["VAR_UNIQUE_ID"]?.ToString(),
                                                patch_starttime = reader["VAR_PATCH_START_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_PATCH_START_TIME"]),
                                                patch_endtime = reader["VAR_PATCH_END_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_PATCH_END_TIME"]),
                                                duration = reader["VAR_PATCH_DURATION"]?.ToString(),
                                                transfer_vdn = reader["VAR_TRANSFERVDN"]?.ToString(),
                                                transfer_status = reader["var_transferstatus"]?.ToString(),

                                            };
                                          // UpdateRows(data_.unique_id.ToString());
                                            results.Add(data_);

                                        }
                                    }
                                }
                                else
                                {
                                    return Ok("Nodata");
                                }
                            }
                        }

                        connection.Close();
                    }
                }
                else
                {
                    lg.lodwrite("Empty");
                    return Ok("ContractNumber or conversationID is Empty");
                }
            }
            catch (Exception ex)
            {
                lg.lodwrite(ex.Message.ToString());
            }

            return Ok(results);
        }

       


        [HttpPost("getcalldetails")]
        public IActionResult GetCallDetails([FromForm] string Caller_id)
        {
            string response = string.Empty;
            Int64 score = 0;
            Log lg = new Log();

            var results = new List<TBL_CALLDETAILS>();
            try
            {
                if (Caller_id != "")
                {
                    lg.lodwrite("---Entrymethod----");

                    string insetQuery = "SELECT  * FROM TBL_CALLDETAILS WHERE VAR_CALLER_ID = '" + Caller_id + "' and var_Status='1' ORDER BY var_called_date DESC";

                    using (SqlConnection connection = new SqlConnection(_dbConnection))
                    {
                        connection.Open();

                        using (SqlCommand selectCommand = new SqlCommand(insetQuery, connection))
                        {
                            // Set the parameters
                            selectCommand.Parameters.AddWithValue("@VAR_CALLER_ID", Caller_id);

                            // Execute the select command
                            using (SqlDataReader reader = selectCommand.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        lg.lodwrite(reader.FieldCount.ToString());
                                        while (reader.Read())
                                        {
                                            TBL_CALLDETAILS data = new TBL_CALLDETAILS
                                            {
                                                //    SNO = Convert.ToInt32(reader["SNO"]),
                                                called_date = reader["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_CALLED_DATE"]),
                                                caller_id = reader["VAR_CALLER_ID"]?.ToString(),
                                                unique_id = reader["VAR_UNIQUE_ID"]?.ToString(),
                                                ivr_starttime = reader["VAR_IVR_START_TIME"]?.ToString(),
                                                ivr_endtime = reader["VAR_IVR_END_TIME"]?.ToString(),
                                                duration = reader["VAR_DURATION"]?.ToString(),
                                                dnis = reader["VAR_DNIS"]?.ToString(),
                                                Call_Type = reader["VAR_CALL_TYPE"]?.ToString(),
                                            };
                                         //   UpdateRowscalldetails(data.unique_id);
                                            results.Add(data);

                                        }
                                    }
                                }
                                else
                                {
                                    return Ok("Nodata");
                                }
                            }
                        }
                        connection.Close();
                    }
                }
                else
                {

                    //Verification verify = new Verification();
                    //verify.var_verification_status = "ContractNumber or conversationID is Empty";
                    //results.Add(verify);
                    lg.lodwrite("Empty");
                    return Ok("CallerID is Empty");
                }
            }
            catch (Exception ex)
            {
                lg.lodwrite(ex.Message.ToString());
            }

            return Ok(results);
        }    

        

        [HttpPost("savefile")]
        public IActionResult FileDownload([FromForm] string Unique_id)
        {
            Log lg = new Log();
            lg.lodwrite("--- Entry FileDownload method ---");

            try
            {
                if (string.IsNullOrWhiteSpace(Unique_id))
                {
                    lg.lodwrite("UniqueID is empty.");
                    return BadRequest("UniqueID is empty.");
                }

                lg.lodwrite($"Unique ID: {Unique_id}");

                string query = @"SELECT VAR_RECORDINGPATH FROM TBL_CALL_TRANSFER WHERE VAR_UNIQUE_ID = @VAR_UNIQUE_ID";

                using (SqlConnection connection = new SqlConnection(_dbConnection))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@VAR_UNIQUE_ID", Unique_id);
                        object result = command.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                        {
                            lg.lodwrite("No file path found in the database.");
                            return NotFound("No file path found for this Unique ID.");
                        }

                        string FilePath = Path.Combine(result.ToString(), $"{Unique_id}.wav").Replace("\\", "/");
                        lg.lodwrite($"SFTP File Path: {FilePath}");

                        using (var client = new SftpClient(_host, _port, _username, _password))
                        {
                            client.Connect();

                            if (!client.Exists(FilePath))
                            {
                                lg.lodwrite($"File not found on SFTP: {FilePath}");
                                return NotFound($"File not found on SFTP: {FilePath}");
                            }

                            using (var memoryStream = new MemoryStream())
                            {
                                client.DownloadFile(FilePath, memoryStream);
                                client.Disconnect();

                                memoryStream.Position = 0;
                                string fileName = Path.GetFileName(FilePath);
                                lg.lodwrite($"Successfully downloaded file: {fileName}");

                                return File(memoryStream.ToArray(), "audio/wav", fileName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lg.lodwrite($"Exception occurred: {ex}");
                return StatusCode(500, "Internal server error occurred while processing the file.");
            }
        }

        [HttpPost("outdial")]
        public IActionResult UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded!");

            // 1. Save Excel file to server uploads folder
            Directory.CreateDirectory(UploadFolder);
            string filePath = Path.Combine(UploadFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // 2. Read Excel rows
            var rows = FileDataReader.ReadTable(filePath).ToList();
            if (!rows.Any())
                return BadRequest("No data found in the Excel file.");

            // 3. Generate .call files
            var callFiles = new List<string>();
            foreach (var arr in rows.Skip(1)) // Skip header
            {
                if (arr.Length < 2) continue;

                string callFilePath = Path.Combine(UploadFolder, arr[0] + ".call");
                System.IO.File.WriteAllLines(callFilePath, new[]
                {
                    $"Channel:PJSIP/{arr[0]}@out",
                    "WaitTime:30",
                    "Maxretries:0",
                    "RetryTime:0",
                    "Context:from-interval",
                    $"Extension:{arr[1]}",
                    $"setvar:caller_id=out{arr[0]}",
                    "Priority:1",
                    "Archive:yes"
                });

                callFiles.Add(callFilePath);
            }

            // 4. Upload call files to Linux server
            var successFiles = new List<string>();
            var failedFiles = new List<string>();

            foreach (var callFile in callFiles)
            {
                if (UploadToSftp(callFile, "/var/spool/asterisk/outgoing"))
                    successFiles.Add(Path.GetFileName(callFile));
                else
                    failedFiles.Add(Path.GetFileName(callFile));
            }

            return Ok(new
            {
                SuccessCount = successFiles.Count,
                FailedCount = failedFiles.Count,
                //UploadedFiles = successFiles,
                //FailedFiles = failedFiles
            });
        }

        private bool UploadToSftp(string filePath, string remoteDir)
        {
            const string host = "192.168.5.61";
            const int port = 22;
            const string username = "root";
            const string password = "Kaizen%$#@!";

            try
            {
                using var client = new SftpClient(host, port, username, password);
                client.Connect();

                string remotePath = $"{remoteDir}/{Path.GetFileName(filePath)}";
                Console.WriteLine($"Uploading {filePath} to {remotePath}");

                using var fileStream = new FileStream(filePath, FileMode.Open);
                client.UploadFile(fileStream, remotePath);

                client.Disconnect();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload failed for {filePath}: {ex.Message}");
                return false;
            }
        }

        [HttpPost("updateuniqueidbycalldetails")]
        public IActionResult UpdateUniqueIds([FromBody] calldetails request)
        {
            Log lg = new Log();
            if (request?.UniqueIds == null || !request.UniqueIds.Any())
                return BadRequest("UniqueIds list is empty or missing.");

            var tBL_CALLDETAILs = new List<TBL_CALLDETAILS>();

            using (var connection = new SqlConnection(_dbConnection))
            {
                connection.Open();

                var parameters = string.Join(",", request.UniqueIds.Select((id, index) => $"@id{index}"));

                // New query using multiple unique IDs
                var query = $@"SELECT * FROM TBL_CALLDETAILS 
               WHERE VAR_UNIQUE_ID IN ({parameters}) 
               AND VAR_STATUS = '1' and VAR_IVR_END_TIME IS NOT NULL
               ORDER BY VAR_CALLED_DATE DESC";
                using (var command = new SqlCommand(query, connection))
                {
                    // Add parameters
                    for (int i = 0; i < request.UniqueIds.Count; i++)
                    {
                        command.Parameters.AddWithValue($"@id{i}", request.UniqueIds[i]);
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                lg.lodwrite(reader.FieldCount.ToString());
                                while (reader.Read())
                                {
                                    TBL_CALLDETAILS data = new TBL_CALLDETAILS
                                    {
                                        //    SNO = Convert.ToInt32(reader["SNO"]),
                                        called_date = reader["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_CALLED_DATE"]),
                                        caller_id = reader["VAR_CALLER_ID"]?.ToString(),
                                        unique_id = reader["VAR_UNIQUE_ID"]?.ToString(),
                                        ivr_starttime = reader["VAR_IVR_START_TIME"]?.ToString(),
                                        ivr_endtime = reader["VAR_IVR_END_TIME"]?.ToString(),
                                        duration = reader["VAR_DURATION"]?.ToString(),
                                        dnis = reader["VAR_DNIS"]?.ToString(),
                                    };
                                    //UpdateRowscalldetails(data.unique_id);
                                    tBL_CALLDETAILs.Add(data);

                                }
                            }
                        }
                        else
                        {
                            return Ok("Nodata");
                        }
                    }
                }
            }
            return Ok("Success");
        }


        private void UpdateRowscalltransferdetails(string Tdata)
        {
            using (SqlConnection sqlcon = new SqlConnection(_dbConnection))
            {
                sqlcon.Open();
                using (SqlCommand updateCmd = new SqlCommand("UPDATE TBL_CALL_TRANSFER SET VAR_STATUS = '2' WHERE VAR_UNIQUE_ID = '" + Tdata + "'", sqlcon))
                {
                    updateCmd.Parameters.AddWithValue("@VAR_STATUS", "2");
                    updateCmd.Parameters.AddWithValue("@VAR_UNIQUE_ID", Tdata ?? string.Empty);
                    updateCmd.ExecuteNonQuery();
                }
                sqlcon.Close();
            }
        }


        [HttpPost("updateuniqueidbycalltransferdetails")]
        public IActionResult UpdateUniqueIdbycalltransferdetials([FromBody] calldetails request)
        {
            Log lg = new Log();
            if (request?.UniqueIds == null || !request.UniqueIds.Any())
                return BadRequest("UniqueIds list is empty or missing.");

            var tBL_CALL_TRANSFERs = new List<TBL_CALL_TRANSFER>();

            using (var connection = new SqlConnection(_dbConnection))
            {
                connection.Open();

                var parameters = string.Join(",", request.UniqueIds.Select((id, index) => $"@id{index}"));

                var query = $@"SELECT * FROM TBL_CALL_TRANSFER WHERE VAR_UNIQUE_ID IN ({parameters}) 
                               AND VAR_STATUS = '1' and VAR_PATCH_END_TIME IS NOT NULL ORDER BY VAR_CALLED_DATE DESC";
                using (var command = new SqlCommand(query, connection))
                {

                    for (int i = 0; i < request.UniqueIds.Count; i++)
                    {
                        command.Parameters.AddWithValue($"@id{i}", request.UniqueIds[i]);
                    }
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                lg.lodwrite(reader.FieldCount.ToString());
                                while (reader.Read())
                                {
                                    TBL_CALL_TRANSFER data = new TBL_CALL_TRANSFER
                                    {
                                        called_date = reader["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_CALLED_DATE"]),
                                        caller_id = reader["VAR_CALLER_ID"]?.ToString(),
                                        unique_id = reader["VAR_UNIQUE_ID"]?.ToString(),
                                        patch_starttime = reader["VAR_PATCH_START_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_PATCH_START_TIME"]),
                                        patch_endtime = reader["VAR_PATCH_END_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_PATCH_END_TIME"]),
                                        duration = reader["VAR_DURATION"]?.ToString(),
                                        transfer_vdn = reader["VAR_TRANSFERVRN"]?.ToString(),
                                    };
                                    //UpdateRowscalltransferdetails(data.unique_id);
                                    tBL_CALL_TRANSFERs.Add(data);
                                }
                            }
                        }
                        else
                        {
                            return Ok("Nodata");
                        }
                    }
                }
            }
            return Ok("Success");
        }

        #region
        [HttpGet("allcalls")]

        public IActionResult GetAllCallDetail()
        {
            string response = string.Empty;
            Int64 score = 0;
            Log lg = new Log();

            var results = new List<TBL_CALLDETAILS>();
            try
            {
                lg.lodwrite("---Entrymethod----");


                string insetQuery = @"SELECT VAR_CALLED_DATE,VAR_CALLER_ID,
           VAR_UNIQUE_ID,VAR_IVR_START_TIME,
           VAR_IVR_END_TIME,VAR_DURATION,
           VAR_DNIS,VAR_CALLER_ID,VAR_CALL_TYPE,VAR_BLACK_LIST,VAR_RMNIN_STATUS
           FROM TBL_CALLDETAILS WHERE VAR_STATUS = '1' 
            AND VAR_IVR_END_TIME IS NOT NULL
            ORDER BY VAR_CALLED_DATE DESC";

                using (SqlConnection connection = new SqlConnection(_dbConnection))
                {
                    connection.Open();

                    using (SqlCommand selectCommand = new SqlCommand(insetQuery, connection))
                    {
                        using (SqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    lg.lodwrite(reader.FieldCount.ToString());
                                    while (reader.Read())
                                    {
                                        TBL_CALLDETAILS data = new TBL_CALLDETAILS
                                        {
                                            called_date = reader["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_CALLED_DATE"]),
                                            caller_id = reader["VAR_CALLER_ID"]?.ToString(),
                                            unique_id = reader["VAR_UNIQUE_ID"]?.ToString(),
                                            ivr_starttime = reader["VAR_IVR_START_TIME"]?.ToString(),
                                            ivr_endtime = reader["VAR_IVR_END_TIME"]?.ToString(),
                                            duration = reader["VAR_DURATION"]?.ToString(),
                                            dnis = reader["VAR_DNIS"]?.ToString(),
                                            Call_Type = reader["VAR_CALL_TYPE"]?.ToString(),
                                        };
                                        //UpdateRowscalldetails(data.unique_id);
                                        results.Add(data);
                                    }
                                }
                            }
                            else
                            {
                                return Ok("Nodata");
                            }
                        }
                    }

                    connection.Close();
                }

                lg.lodwrite("Empty");
                //return Ok(results);
                var csv = new StringBuilder();

                // Header row
                csv.AppendLine("Called Date,Caller Id,Unique Id,IVR Start,IVR End,Duration,DNIS,Call Type,Black list,rmnin status");

                // Data rows
                foreach (var item in results)
                {
                    csv.AppendLine(
                        $"{item.called_date?.ToString("dd-MM-yyyy HH:mm:ss")},"
                        + $"{item.caller_id},"
                        + $"{item.unique_id},"
                        + $"{item.ivr_starttime},"
                        + $"{item.ivr_endtime},"
                        + $"{item.duration},"
                        + $"{item.dnis},"
                        + $"{item.Call_Type}"
                        + $"{item.Black_list}"
                        + $"{item.rmnin_status}"
                    );
                }

                byte[] fileBytes = Encoding.UTF8.GetBytes(csv.ToString());
                return File(fileBytes, "text/csv", "allcalls.csv");

            }
            catch (Exception ex)
            {
                lg.lodwrite(ex.Message.ToString());
            }

            return Ok(results);
        }



        [HttpGet("TodaywisecallDetails")]
        public IActionResult TodayCALLDETAILS()
        {
            var result = new List<TBL_CALLDETAILS>();
            try
            {
                DateTime today = DateTime.Today;

                string selectquery = @"SELECT * FROM TBL_CALLDETAILS 
            WHERE CAST(VAR_CALLED_DATE AS DATE) = @TodayDate 
            ORDER BY VAR_CALLED_DATE DESC";

                using (SqlConnection con = new SqlConnection(_dbConnection))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(selectquery, con))
                    {
                        cmd.Parameters.AddWithValue("@TodayDate", today);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var tBL_CALLDETAILS = new TBL_CALLDETAILS
                                    {
                                        called_date = reader["VAR_CALLED_DATE"] == DBNull.Value
                                            ? (DateTime?)null
                                            : Convert.ToDateTime(reader["VAR_CALLED_DATE"]),
                                        caller_id = reader["VAR_CALLER_ID"]?.ToString(),
                                        unique_id = reader["VAR_UNIQUE_ID"]?.ToString(),
                                        ivr_starttime = reader["VAR_IVR_START_TIME"]?.ToString(),
                                        ivr_endtime = reader["VAR_IVR_END_TIME"]?.ToString(),
                                        duration = reader["VAR_DURATION"]?.ToString(),
                                        dnis = reader["VAR_DNIS"]?.ToString(),
                                        Call_Type = reader["VAR_CALL_TYPE"]?.ToString(),
                                    };
                                    result.Add(tBL_CALLDETAILS);
                                }
                            }
                            else
                            {
                                return Ok("NO DATA FOR TODAY");
                            }
                        }
                    }
                }

                // ✅ Generate CSV
                var csv = new StringBuilder();

                // Header row
                csv.AppendLine("Called Date,Caller Id,Unique Id,IVR Start,IVR End,Duration,DNIS,Call Type");

                // Data rows
                foreach (var item in result)
                {
                    csv.AppendLine(
                        $"{item.called_date?.ToString("dd-MM-yyyy HH:mm:ss")},"
                        + $"{item.caller_id},"
                        + $"{item.unique_id},"
                        + $"{item.ivr_starttime},"
                        + $"{item.ivr_endtime},"
                        + $"{item.duration},"
                        + $"{item.dnis},"
                        + $"{item.Call_Type}"
                    );
                }

                byte[] fileBytes = Encoding.UTF8.GetBytes(csv.ToString());
                return File(fileBytes, "text/csv", "Calldetails.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion
        #region

        //[HttpGet("allcalls")]
        //public IActionResult csvGetCallDetails(string? calledDate)
        //{
        //    var result = new List<TBL_CALLDETAILS>();

        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(_dbConnection))
        //        {
        //            con.Open();

        //            string query;

        //            if (!string.IsNullOrEmpty(calledDate))
        //            {
        //                // If parameter is provided, filter by date
        //                query = @"SELECT VAR_CALLED_DATE, VAR_CALLER_ID, VAR_UNIQUE_ID, VAR_IVR_START_TIME, VAR_IVR_END_TIME, VAR_DURATION, 
        //                          VAR_DNIS, VAR_CALLER_ID, VAR_CALL_TYPE, VAR_BLACK_LIST, VAR_RMNIN_STATUS, VAR_STATUS FROM TBL_CALLDETAILS 
        //                          WHERE CAST(VAR_CALLED_DATE AS DATE) = @CalledDate 
        //                          ORDER BY VAR_CALLED_DATE DESC";
        //            }
        //            else
        //            {
        //                // If parameter is not provided, return all records
        //                query = @"SELECT VAR_CALLED_DATE, VAR_CALLER_ID, VAR_UNIQUE_ID, VAR_IVR_START_TIME, VAR_IVR_END_TIME, VAR_DURATION, 
        //                          VAR_DNIS, VAR_CALLER_ID, VAR_CALL_TYPE, VAR_BLACK_LIST, VAR_RMNIN_STATUS, VAR_STATUS FROM TBL_CALLDETAILS 
        //                          ORDER BY VAR_CALLED_DATE DESC";
        //            }

        //            using (SqlCommand cmd = new SqlCommand(query, con))
        //            {
        //                if (!string.IsNullOrEmpty(calledDate))
        //                {
        //                    cmd.Parameters.AddWithValue("@CalledDate", DateTime.Parse(calledDate));
        //                }

        //                using (SqlDataReader reader = cmd.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        result.Add(new TBL_CALLDETAILS
        //                        {
        //                            called_date = reader["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_CALLED_DATE"]),
        //                            caller_id = reader["VAR_CALLER_ID"]?.ToString(),
        //                            unique_id = reader["VAR_UNIQUE_ID"]?.ToString(),
        //                            ivr_starttime = reader["VAR_IVR_START_TIME"]?.ToString(),
        //                            ivr_endtime = reader["VAR_IVR_END_TIME"]?.ToString(),
        //                            duration = reader["VAR_DURATION"]?.ToString(),
        //                            dnis = reader["VAR_DNIS"]?.ToString(),
        //                            Call_Type = reader["VAR_CALL_TYPE"]?.ToString(),
        //                            rmnin_status = reader["VAR_RMNIN_STATUS"]?.ToString(),
        //                            Black_list = reader["VAR_BLACK_LIST"]?.ToString(),
        //                            // Add other columns as needed
        //                        });
        //                    }
        //                }
        //            }
        //        }

        //        // ✅ Generate CSV
        //        var csv = new StringBuilder();

        //        // Header row
        //        csv.AppendLine("Called Date,Caller Id,Unique Id,IVR Start,IVR End,Duration,DNIS,Call Type,Rmnin_Status,Black_List");

        //        // Data rows
        //        foreach (var item in result)
        //        {
        //            csv.AppendLine(
        //                $"{item.called_date?.ToString("dd-MM-yyyy HH:mm:ss")},"
        //                + $"{item.caller_id},"
        //                + $"{item.unique_id},"
        //                + $"{item.ivr_starttime},"
        //                + $"{item.ivr_endtime},"
        //                + $"{item.duration},"
        //                + $"{item.dnis},"
        //                + $"{item.Call_Type},"
        //                + $"{item.Black_list},"
        //                + $"{item.rmnin_status}"
        //            );
        //        }

        //        byte[] fileBytes = Encoding.UTF8.GetBytes(csv.ToString());
        //        return File(fileBytes, "text/csv", "Calldetails.csv");
        //        //   return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
        #endregion
        [HttpGet("alltransfer")]
        public IActionResult GetAll_CallTransferDetails()
        {
            Log lg = new Log();
            var results = new List<TBL_CALL_TRANSFER>();

            try
            {
                lg.lodwrite("--- Entry method: GetAll_CallTransferDetails ---");

                string query = @"
            SELECT VAR_CALLED_DATE, VAR_CALLER_ID, VAR_UNIQUE_ID, VAR_PATCH_START_TIME, 
                   VAR_PATCH_END_TIME, VAR_PATCH_DURATION, VAR_TRANSFERVDN, VAR_TRANSFERSTATUS
            FROM TBL_CALL_TRANSFER
            WHERE VAR_STATUS = '1' AND VAR_PATCH_END_TIME IS NOT NULL
            ORDER BY VAR_CALLED_DATE DESC";

                using (SqlConnection connection = new SqlConnection(_dbConnection))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var data = new TBL_CALL_TRANSFER
                                {
                                    called_date = reader["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_CALLED_DATE"]),
                                    caller_id = reader["VAR_CALLER_ID"]?.ToString(),
                                    unique_id = reader["VAR_UNIQUE_ID"]?.ToString(),
                                    patch_starttime = reader["VAR_PATCH_START_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_PATCH_START_TIME"]),
                                    patch_endtime = reader["VAR_PATCH_END_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_PATCH_END_TIME"]),
                                    duration = reader["VAR_PATCH_DURATION"]?.ToString(),
                                    transfer_vdn = reader["VAR_TRANSFERVDN"]?.ToString(),
                                    transfer_status = reader["VAR_TRANSFERSTATUS"]?.ToString()
                                };
                                results.Add(data);
                            }
                        }
                        else
                        {
                            lg.lodwrite("No data found.");
                            return Ok("Nodata");
                        }
                    }

                    //    connection.Close();
                }

                //return Ok(results);

                var csv = new StringBuilder();

                // Header row
                csv.AppendLine("Called Date,Caller Id,Unique Id,patch Start,patch End,Duration,transfer_vdn,transfer_status");

                // Data rows
                foreach (var item in results)
                {
                    csv.AppendLine(
                        $"{item.called_date?.ToString("dd-MM-yyyy HH:mm:ss")},"
                        + $"{item.caller_id},"
                        + $"{item.unique_id},"
                        + $"{item.patch_starttime},"
                        + $"{item.patch_endtime},"
                        + $"{item.duration},"
                        + $"{item.transfer_vdn},"
                        + $"{item.transfer_status},"
                    );
                }

                byte[] fileBytes = Encoding.UTF8.GetBytes(csv.ToString());
                return File(fileBytes, "text/csv", "CallTransfer.csv");
            }
            catch (Exception ex)
            {
                lg.lodwrite("Exception: " + ex.Message);
                lg.lodwrite("StackTrace: " + ex.StackTrace);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("alltransaction")]
        public IActionResult GetallTransactionDetail()
        {
            var result = new List<TBL_TRANSACTION_DETAILS>();
            try
            {
                string selectquery = "SELECT * FROM TBL_TRANSACTION_DETAILS";
                using (SqlConnection con = new SqlConnection(_dbConnection))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(selectquery, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var tBL_TRANSACTION_DETAILS = new TBL_TRANSACTION_DETAILS
                                    {
                                        //sno = Convert.ToInt32(reader["VAR_SNO"]),
                                        calleddate = reader["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_CALLED_DATE"]),
                                        callerid = reader["VAR_CALLER_ID"]?.ToString(),
                                        uniqueid = reader["VAR_UNIQUE_ID"]?.ToString(),
                                        flow = reader["VAR_FLOW"]?.ToString(),
                                        level_1 = reader["VAR_LEVEL_1"]?.ToString(),
                                        level_2 = reader["VAR_LEVEL_2"]?.ToString(),
                                        level_3 = reader["VAR_LEVEL_3"]?.ToString(),
                                        level_4 = reader["VAR_LEVEL_4"]?.ToString(),
                                        level_5 = reader["VAR_LEVEL_5"]?.ToString(),
                                        level_6 = reader["VAR_LEVEL_6"]?.ToString(),
                                        level_7 = reader["VAR_LEVEL_7"]?.ToString(),
                                        level_8 = reader["VAR_LEVEL_8"]?.ToString(),
                                        level_9 = reader["VAR_LEVEL_9"]?.ToString(),
                                        level_10 = reader["VAR_LEVEL_10"]?.ToString(),
                                        level_11 = reader["VAR_LEVEL_11"]?.ToString(),
                                        level_12 = reader["VAR_LEVEL_12"]?.ToString(),
                                        level_13 = reader["VAR_LEVEL_13"]?.ToString(),
                                        level_14 = reader["VAR_LEVEL_14"]?.ToString(),
                                        level_15 = reader["VAR_LEVEL_15"]?.ToString(),
                                        disconnecttree = reader["VAR_DISCONNECT_TREE"]?.ToString()
                                    };
                                    result.Add(tBL_TRANSACTION_DETAILS);
                                }
                            }
                            else
                            {
                                return Ok("NO DATA");
                            }
                        }
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        

        [HttpGet("csvtransactiondetail")]
        public IActionResult csvTransactionDetail()
        {
            DateTime today = DateTime.Today;
            var result = new List<TBL_TRANSACTION_DETAILS>();
            try
            {
                string selectquery = "SELECT * FROM TBL_TRANSACTION_DETAILS  WHERE CAST(VAR_CALLED_DATE AS DATE) = @TodayDate ORDER BY VAR_CALLED_DATE DESC ";
                using (SqlConnection con = new SqlConnection(_dbConnection))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(selectquery, con))
                    {
                        cmd.Parameters.AddWithValue("@TodayDate", today);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var tBL_TRANSACTION_DETAILS = new TBL_TRANSACTION_DETAILS
                                    {
                                        //sno = Convert.ToInt32(reader["VAR_SNO"]),
                                        calleddate = reader["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VAR_CALLED_DATE"]),
                                        callerid = reader["VAR_CALLER_ID"]?.ToString(),
                                        uniqueid = reader["VAR_UNIQUE_ID"]?.ToString(),
                                        flow = reader["VAR_FLOW"]?.ToString(),
                                        level_1 = reader["VAR_LEVEL_1"]?.ToString(),
                                        level_2 = reader["VAR_LEVEL_2"]?.ToString(),
                                        level_3 = reader["VAR_LEVEL_3"]?.ToString(),
                                        level_4 = reader["VAR_LEVEL_4"]?.ToString(),
                                        level_5 = reader["VAR_LEVEL_5"]?.ToString(),
                                        level_6 = reader["VAR_LEVEL_6"]?.ToString(),
                                        level_7 = reader["VAR_LEVEL_7"]?.ToString(),
                                        level_8 = reader["VAR_LEVEL_8"]?.ToString(),
                                        level_9 = reader["VAR_LEVEL_9"]?.ToString(),
                                        level_10 = reader["VAR_LEVEL_10"]?.ToString(),
                                        level_11 = reader["VAR_LEVEL_11"]?.ToString(),
                                        level_12 = reader["VAR_LEVEL_12"]?.ToString(),
                                        level_13 = reader["VAR_LEVEL_13"]?.ToString(),
                                        level_14 = reader["VAR_LEVEL_14"]?.ToString(),
                                        level_15 = reader["VAR_LEVEL_15"]?.ToString(),
                                        disconnecttree = reader["VAR_DISCONNECT_TREE"]?.ToString()
                                    };
                                    result.Add(tBL_TRANSACTION_DETAILS);
                                }
                            }
                            else
                            {
                                return Ok("NO DATA");
                            }
                        }
                    }
                }

                var csv = new StringBuilder();

                // Header row
                csv.AppendLine("Called Date,CallerId,UniqueId,Flow,Level1,Level2,Level3,Level4,Level5,Level6,Level7,Level8,Level9,Level10,Level11,Level12,Level13,Level14,Level15,Disconnecttree");

                // Data rows
                foreach (var item in result)
                {
                    csv.AppendLine(
                        $"{item.calleddate?.ToString("dd-MM-yyyy HH:mm:ss")},"
                        + $"{item.callerid},"
                        + $"{item.uniqueid},"
                        + $"{item.flow},"
                        + $"{item.level_1},"
                        + $"{item.level_2},"
                        + $"{item.level_3},"
                        + $"{item.level_4}"
                        + $"{item.level_5}"
                        + $"{item.level_6}"
                        + $"{item.level_7}"
                        + $"{item.level_8}"
                        + $"{item.level_9}"
                        + $"{item.level_10}"
                        + $"{item.level_11}"
                        + $"{item.level_12}"
                        + $"{item.level_13}"
                        + $"{item.level_14}"
                        + $"{item.level_15}"
                        + $"{item.disconnecttree}"
                    );
                }

                byte[] fileBytes = Encoding.UTF8.GetBytes(csv.ToString());
                return File(fileBytes, "text/csv", "TransactionDetails.csv");
                //  return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}