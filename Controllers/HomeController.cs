using Benz.log;
using Call_Details_API.Helpers;
using Call_Details_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
//using OfficeOpenXml;
using PushAPI.Model;
using Renci.SshNet;
using Renci.SshNet.Sftp;
//using RestSharp;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
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
        //private readonly IRestClient _restClient;
        private readonly string _dbConnection;
        private readonly string UploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");
        private Log lg;
        const string host = "192.168.5.61";
        const int port = 22;
        const string username = "root";
        //const string password = "Kaizen%$#@!";
        const string password = "Kaizen^%$#@!";

        private readonly IConfiguration _configuration;
        public ivrController(IConfiguration configuration)
        {
            //string dbcon=""
            Log lg = new Log();
            lg.lodwrite("databaseconnection");
            _dbConnection = configuration.GetConnectionString("dbcon");
            //_configuration = configuration.GetConnectionString("dbcon");
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

                        using (var client = new SftpClient(host, port, username, password))
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

        [HttpPost("CreateCampaignMaster")]
        public IActionResult InsertCampaignMaster([FromBody] DNIS_TABLE dNIS_TABLE)
        {
            try
            {
                List<string> ErrorMessage = new List<string>();

                if (string.IsNullOrWhiteSpace(dNIS_TABLE.campaign_id))
                {
                    ErrorMessage.Add("Campaign_id is mandatory");
                }

                if (string.IsNullOrWhiteSpace(dNIS_TABLE.campaign_name))
                {
                    ErrorMessage.Add("Campaign_Name is mandatory");
                }
                if (string.IsNullOrWhiteSpace(dNIS_TABLE.Status))
                {
                    ErrorMessage.Add("Status is mandatory");
                }
                if (string.IsNullOrWhiteSpace(dNIS_TABLE.Time_Zone))
                {
                    ErrorMessage.Add("Time_Zone is mandatory");
                }
                if (string.IsNullOrWhiteSpace(dNIS_TABLE.Start_Date))
                {
                    ErrorMessage.Add("Start_date is mandatory");
                }
                if (string.IsNullOrWhiteSpace(dNIS_TABLE.End_Date))
                {
                    ErrorMessage.Add("end_date is mandatory");
                }

                if (string.IsNullOrWhiteSpace(dNIS_TABLE.Start_Time))
                {
                    ErrorMessage.Add("Start_time is mandatory");
                }

                if (string.IsNullOrWhiteSpace(dNIS_TABLE.End_Time))
                {
                    ErrorMessage.Add("End_time is mandatory");
                }
                if (string.IsNullOrWhiteSpace(dNIS_TABLE.Dialing_Mode))
                {
                    ErrorMessage.Add("Dialing_Mode is mandatory");
                }

                if (string.IsNullOrWhiteSpace(dNIS_TABLE.Retry_intervals))
                {
                    ErrorMessage.Add("Retry_intervals is mandatory");
                }
                if (string.IsNullOrWhiteSpace(dNIS_TABLE.Retry_attempts))
                {
                    ErrorMessage.Add("Retry_attempts is mandatory");
                }
                if (ErrorMessage.Count > 0)
                {
                    return Ok(new { ErrorMessage });
                }
                using (SqlConnection con = new SqlConnection(_dbConnection))
                {
                    con.Open();
                    string CMquery = "SELECT COUNT(*) FROM TBL_CAMPAIGN_MASTER_V2 where VAR_CAMPAIGN_ID=@VAR_CAMPAIGN_ID";
                    using (SqlCommand CMcmd = new SqlCommand(CMquery, con))
                    {
                        CMcmd.Parameters.AddWithValue("@VAR_CAMPAIGN_ID", dNIS_TABLE.campaign_id);
                        //cmd.ExecuteNonQuery();
                        int count = (int)CMcmd.ExecuteScalar();
                        if (count > 0)
                        {
                            return Content($"Campaign ID already exists. Please use a unique Campaign ID ");
                        }
                        else
                        {
                            string checkQuery = "SELECT COUNT(*) FROM TBL_DNIS WHERE VAR_MODE='NOTINUSE'";
                            using (SqlCommand cmdCheck = new SqlCommand(checkQuery, con))
                            {
                                int DNIScount = (int)cmdCheck.ExecuteScalar();

                                if (DNIScount > 0)
                                {
                                    string selectquery = "SELECT TOP(1) VAR_DNIS FROM TBL_DNIS WHERE VAR_MODE='NOTINUSE' ORDER BY VAR_DNIS";
                                    using (SqlCommand checkDnisCmd = new SqlCommand(selectquery, con))
                                    using (SqlDataReader reader = checkDnisCmd.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            string selectedDnis = reader["VAR_DNIS"].ToString();
                                            reader.Close();

                                            string DNISquery = @"UPDATE TBL_DNIS SET VAR_CAMPAIGN_NAME=@VAR_CAMPAIGN_NAME, 
                                                                 VAR_CAMPAIGN_ID=@VAR_CAMPAIGN_ID,VAR_MODE='INUSE' WHERE VAR_DNIS=@VAR_DNIS";

                                            using (SqlCommand cmd = new SqlCommand(DNISquery, con))
                                            {
                                                cmd.Parameters.AddWithValue("@VAR_CAMPAIGN_ID", dNIS_TABLE.campaign_id);
                                                cmd.Parameters.AddWithValue("@VAR_CAMPAIGN_NAME", dNIS_TABLE.campaign_name);
                                                cmd.Parameters.AddWithValue("@VAR_DNIS", selectedDnis);
                                                cmd.ExecuteNonQuery();

                                                string insertquery = @"INSERT INTO TBL_CAMPAIGN_MASTER_V2 
                                          (VAR_CAMPAIGN_ID, VAR_CAMPAIGN_NAME, VAR_STATUS, VAR_CAMPAIGN_DESCRIPTION,
                                          VAR_CAMPAIGN_TYPE, VAR_TIME_ZONE, VAR_CAMPAIN_CREATED_DATE, VAR_CAMPAIN_START_DATE, VAR_CAMPAIN_END_DATE, 
                                          VAR_CAMPAIGN_START_TIME, VAR_CAMPAIGN_END_TIME, VAR_DIALING_MODE,
                                          VAR_MAX_CONCURRENT_CALLS, VAR_CALL_DURATION_LIMIT, VAR_RETRY_ATTEMPTS, VAR_RETRY_INTERVALS, 
                                          VAR_TEAMS, VAR_MAX_LEADS, VAR_SKILL_TAGS, VAR_IS_RECORDING, VAR_SOURCE_FILR_PATH, VAR_DESTINATION_FILE_PATH)
                                          VALUES
                                          (@VAR_CAMPAIGN_ID, @VAR_CAMPAIGN_NAME, @VAR_STATUS, @VAR_CAMPAIGN_DESCRIPTION,
                                          @VAR_CAMPAIGN_TYPE, @VAR_TIME_ZONE, @VAR_CAMPAIN_CREATED_DATE, @VAR_CAMPAIN_START_DATE, @VAR_CAMPAIN_END_DATE,
                                          @VAR_CAMPAIGN_START_TIME, @VAR_CAMPAIGN_END_TIME, @VAR_DIALING_MODE,
                                          @VAR_MAX_CONCURRENT_CALLS, @VAR_CALL_DURATION_LIMIT, @VAR_RETRY_ATTEMPTS, @VAR_RETRY_INTERVALS, 
                                          @VAR_TEAMS, @VAR_MAX_LEADS, @VAR_SKILL_TAGS, @VAR_IS_RECORDING, NULL, NULL)";


                                                using (SqlCommand Icmd = new SqlCommand(insertquery, con))
                                                {
                                                    AddCampaignParameters(Icmd, dNIS_TABLE);
                                                    int rows = Icmd.ExecuteNonQuery();
                                                    #region
                                                    //if (rows > 0)
                                                    //{
                                                    //    return Content($"Campaign ID: {dNIS_TABLE.campaign_id}, DNIS Number: {selectedDnis}, Successfully Created!");

                                                    //    string selectquery = "SELECT * FROM TBL_DNIS WHERE VAR_CAMPAIGN_ID=Campaign_id";

                                                    //    string updateExtensionQuery = @"UPDATE TBL_CAMPAIGN_MASTER_V2 SET VAR_EXTENSION_NUMBER = @Extension_Number  WHERE VAR_CAMPAIGN_ID = @VAR_CAMPAIGN_ID";

                                                    //    using (SqlCommand updateCmd = new SqlCommand(updateExtensionQuery, con))
                                                    //    {
                                                    //        updateCmd.Parameters.AddWithValue(
                                                    //            "@EXTENSION_NUMBER",
                                                    //            (object?)dNIS_TABLE.Extension_Number ?? DBNull.Value
                                                    //        );

                                                    //        updateCmd.Parameters.AddWithValue(
                                                    //            "@VAR_CAMPAIGN_ID",
                                                    //            dNIS_TABLE.campaign_id
                                                    //        );

                                                    //        updateCmd.ExecuteNonQuery();
                                                    //    }
                                                    //    //return Content($"Campaign ID: {dNIS_TABLE.campaign_id}, DNIS Number: {selectedDnis}, Successfully Created!");
                                                    //}
                                                    //else
                                                    //{
                                                    //    return Content("Insert failed");
                                                    //}
                                                    #endregion
                                                    #region
                                                    //if (rows > 0)
                                                    //{

                                                    //    // =========================================================
                                                    //    // 1. SELECT DNIS AND EXTENSION NUMBER FROM TBL_DNIS
                                                    //    // =========================================================

                                                    //    string selectDnisQuery = @"SELECT VAR_DNIS  FROM TBL_DNIS  WHERE VAR_CAMPAIGN_ID = @VAR_CAMPAIGN_ID";

                                                    //    string? selectedDnisFromDb = null;
                                                    //    object extensionNumber = DBNull.Value;

                                                    //    using (SqlCommand selectCmd = new SqlCommand(selectDnisQuery, con))
                                                    //    {
                                                    //        selectCmd.Parameters.AddWithValue(
                                                    //            "@VAR_CAMPAIGN_ID",
                                                    //            dNIS_TABLE.campaign_id
                                                    //        );

                                                    //        using (SqlDataReader selectreader = selectCmd.ExecuteReader())
                                                    //        {
                                                    //            if (selectreader.Read())
                                                    //            {
                                                    //                selectedDnisFromDb = selectreader["VAR_DNIS"]?.ToString();

                                                    //                if (reader["VAR_EXTENSION_NUMBER"] != DBNull.Value)
                                                    //                {
                                                    //                    extensionNumber = reader["VAR_EXTENSION_NUMBER"];
                                                    //                }
                                                    //            }
                                                    //        }
                                                    //    }

                                                    //    //// =========================================================
                                                    //    //// 2. CHECK DNIS RECORD FOUND
                                                    //    //// =========================================================

                                                    //    //if (string.IsNullOrWhiteSpace(selectedDnisFromDb))
                                                    //    //{
                                                    //    //    return Content(
                                                    //    //        $"Campaign ID: {dNIS_TABLE.campaign_id} created, " +
                                                    //    //        "but DNIS record was not found in TBL_DNIS."
                                                    //    //    );
                                                    //    //}

                                                    //    // =========================================================
                                                    //    // 3. UPDATE EXTENSION NUMBER IN CAMPAIGN MASTER
                                                    //    // =========================================================

                                                    //    string updateExtensionQuery = @"UPDATE TBL_CAMPAIGN_MASTER_V2 SET VAR_EXTENSION_NUMBER = @EXTENSION_NUMBER WHERE VAR_CAMPAIGN_ID = @VAR_CAMPAIGN_ID";

                                                    //    using (SqlCommand updateCmd = new SqlCommand(updateExtensionQuery, con))
                                                    //    {
                                                    //        updateCmd.Parameters.AddWithValue(
                                                    //            "@EXTENSION_NUMBER",
                                                    //            extensionNumber
                                                    //        );

                                                    //        updateCmd.Parameters.AddWithValue(
                                                    //            "@VAR_CAMPAIGN_ID",
                                                    //            dNIS_TABLE.campaign_id
                                                    //        );

                                                    //        int updateRows = updateCmd.ExecuteNonQuery();

                                                    //        if (updateRows > 0)
                                                    //        {
                                                    //            //return Content(
                                                    //            //    $"Campaign ID: {dNIS_TABLE.campaign_id}, " +
                                                    //            //    $"DNIS Number: {selectedDnisFromDb}, " +
                                                    //            //    $"Extension Number: {extensionNumber}, " +
                                                    //            //    "Successfully Created!"
                                                    //            //);
                                                    //        }
                                                    //        else
                                                    //        {
                                                    //            return Content(
                                                    //                $"Campaign ID: {dNIS_TABLE.campaign_id} created, " +
                                                    //                "but VAR_EXTENSION_NUMBER update failed."
                                                    //            );
                                                    //        }
                                                    //    }
                                                    //    return Content($"Campaign ID: {dNIS_TABLE.campaign_id}, DNIS Number: {selectedDnis}, Successfully Created!");
                                                    //}
                                                    //else
                                                    //{
                                                    //    return Content("Insert failed");
                                                    //}
                                                    #endregion
                                                    #region
                                                    //if (rows > 0)
                                                    //{
                                                    //    // =========================================================
                                                    //    // 1. SELECT DNIS FROM TBL_DNIS USING CAMPAIGN ID
                                                    //    // =========================================================

                                                    //    string selectDnisQuery = @"SELECT TOP 1 VAR_DNIS   FROM TBL_DNIS WHERE VAR_CAMPAIGN_ID = @VAR_CAMPAIGN_ID";

                                                    //    string? selectedDnisFromDb = null;

                                                    //    using (SqlCommand selectCmd = new SqlCommand(selectDnisQuery, con))
                                                    //    {
                                                    //        selectCmd.Parameters.AddWithValue(
                                                    //            "@VAR_CAMPAIGN_ID",
                                                    //            dNIS_TABLE.campaign_id
                                                    //        );

                                                    //        using (SqlDataReader selectReader = selectCmd.ExecuteReader())
                                                    //        {
                                                    //            if (selectReader.Read())
                                                    //            {
                                                    //                selectedDnisFromDb =
                                                    //                    selectReader["VAR_DNIS"]?.ToString();
                                                    //            }
                                                    //        }
                                                    //    }

                                                    //    // =========================================================
                                                    //    // 2. CHECK DNIS
                                                    //    // =========================================================

                                                    //    if (string.IsNullOrWhiteSpace(selectedDnisFromDb))
                                                    //    {
                                                    //        return Content(
                                                    //            $"Campaign ID: {dNIS_TABLE.campaign_id} created, " +
                                                    //            "but DNIS was not found in TBL_DNIS."
                                                    //        );
                                                    //    }

                                                    //    // =========================================================
                                                    //    // 3. UPDATE EXTENSION NUMBER IN CAMPAIGN MASTER
                                                    //    // =========================================================

                                                    //    string updateExtensionQuery = @"UPDATE TBL_CAMPAIGN_MASTER_V2
                                                    //                                    SET VAR_EXTENSION_NUMBER = @EXTENSION_NUMBER 
                                                    //                                    WHERE VAR_CAMPAIGN_ID = @VAR_CAMPAIGN_ID";

                                                    //    using (SqlCommand updateCmd =
                                                    //           new SqlCommand(updateExtensionQuery, con))
                                                    //    {
                                                    //        updateCmd.Parameters.AddWithValue(
                                                    //            "@EXTENSION_NUMBER",
                                                    //            (object?)dNIS_TABLE.Extension_Number ?? DBNull.Value
                                                    //        );

                                                    //        updateCmd.Parameters.AddWithValue(
                                                    //            "@VAR_CAMPAIGN_ID",
                                                    //            dNIS_TABLE.campaign_id
                                                    //        );

                                                    //        int updateRows = updateCmd.ExecuteNonQuery();

                                                    //        if (updateRows <= 0)
                                                    //        {
                                                    //            return Content(
                                                    //                $"Campaign ID: {dNIS_TABLE.campaign_id} created, " +
                                                    //                "but VAR_EXTENSION_NUMBER update failed."
                                                    //            );
                                                    //        }
                                                    //    }

                                                    //    // =========================================================
                                                    //    // 4. SUCCESS RESPONSE
                                                    //    // =========================================================

                                                    //    return Content(
                                                    //        $"Campaign ID: {dNIS_TABLE.campaign_id}, " +
                                                    //        $"DNIS Number: {selectedDnisFromDb}, " +
                                                    //        $"Extension Number: {dNIS_TABLE.Extension_Number}, " +
                                                    //        "Successfully Created!"
                                                    //    );
                                                    //}
                                                    //else
                                                    //{
                                                    //    return Content("Insert failed");
                                                    //}
                                                    #endregion
                                                    // =========================================================
                                                    // 1. GET DNIS AND DNIS_EXTENSION FROM TBL_DNIS
                                                    // =========================================================

                                                    string selectDnisQuery = @"
    SELECT TOP 1 
           VAR_DNIS
    FROM TBL_DNIS
    WHERE VAR_CAMPAIGN_ID = @VAR_CAMPAIGN_ID
    ORDER BY VAR_DNIS";

                                                    string? selectedDnisFromDb = null;
                                                    object extensionNumberFromDnis = DBNull.Value;

                                                    using (SqlCommand selectCmd = new SqlCommand(selectDnisQuery, con))
                                                    {
                                                        selectCmd.Parameters.AddWithValue(
                                                            "@VAR_CAMPAIGN_ID",
                                                            dNIS_TABLE.campaign_id
                                                        );

                                                        using (SqlDataReader selectReader = selectCmd.ExecuteReader())
                                                        {
                                                            if (selectReader.Read())
                                                            {
                                                                // Get DNIS
                                                                selectedDnisFromDb =
                                                                    selectReader["VAR_DNIS"]?.ToString();

                                                                // Get DNIS_EXTENSION
                                                                //if (selectReader["VAR_DNIS"] != DBNull.Value)
                                                                //{
                                                                //    extensionNumberFromDnis =
                                                                //        selectReader["VAR_DNIS"];
                                                                //}
                                                            }
                                                        }
                                                    }

                                                    // =========================================================
                                                    // 2. CHECK DNIS RECORD
                                                    // =========================================================

                                                    if (string.IsNullOrWhiteSpace(selectedDnisFromDb))
                                                    {
                                                        return Content(
                                                            $"Campaign ID: {dNIS_TABLE.campaign_id} created, " +
                                                            "but DNIS was not found in TBL_DNIS."
                                                        );
                                                    }

                                                    // =========================================================
                                                    // 3. CHECK DNIS_EXTENSION
                                                    // =========================================================

                                                    //if (extensionNumberFromDnis == DBNull.Value ||
                                                    //    string.IsNullOrWhiteSpace(extensionNumberFromDnis.ToString()))
                                                    //{
                                                    //    return Content(
                                                    //        $"Campaign ID: {dNIS_TABLE.campaign_id} created, " +
                                                    //        $"DNIS Number: {selectedDnisFromDb}, " +
                                                    //        "but DNIS_EXTENSION was not found in TBL_DNIS."
                                                    //    );
                                                    //}

                                                    // =========================================================
                                                    // 4. UPDATE EXTENSION NUMBER IN CAMPAIGN MASTER
                                                    // =========================================================

                                                    string updateExtensionQuery = @"
    UPDATE TBL_CAMPAIGN_MASTER_V2
    SET VAR_EXTENSION_NUMBER = @EXTENSION_NUMBER
    WHERE VAR_CAMPAIGN_ID = @VAR_CAMPAIGN_ID";

                                                    using (SqlCommand updateCmd =
                                                           new SqlCommand(updateExtensionQuery, con))
                                                    {
                                                        updateCmd.Parameters.AddWithValue(
                                                            "@EXTENSION_NUMBER",
                                                            selectedDnisFromDb
                                                        );
                                                        //updateCmd.Parameters.AddWithValue(
                                                        //    "@EXTENSION_NUMBER",
                                                        //    extensionNumberFromDnis
                                                        //);

                                                        updateCmd.Parameters.AddWithValue(
                                                            "@VAR_CAMPAIGN_ID",
                                                            dNIS_TABLE.campaign_id
                                                        );

                                                        int updateRows = updateCmd.ExecuteNonQuery();

                                                        if (updateRows <= 0)
                                                        {
                                                            return Content(
                                                                $"Campaign ID: {dNIS_TABLE.campaign_id} created, " +
                                                                "but VAR_EXTENSION_NUMBER update failed."
                                                            );
                                                        }
                                                    }

                                                    // =========================================================
                                                    // 5. SUCCESS RESPONSE
                                                    // =========================================================

                                                    return Content(
                                                        $"Campaign ID: {dNIS_TABLE.campaign_id}, " +
                                                        $"DNIS Number: {selectedDnisFromDb}, " +
                                                        //$"Extension Number: {extensionNumberFromDnis}, " +
                                                        "Successfully Created!"
                                                    );

                                                }
                                            }
                                        }
                                        else
                                        {
                                            return Content($"{dNIS_TABLE.campaign_id} Campaign ID is already created");
                                        }
                                    }
                                }
                                else
                                {
                                    return Content("No Data Found in NOTINUSE");
                                }
                            }
                        }

                    }

                }
            }

            catch (Exception ex)
            {
                return Content("Unexpected Error: " + ex.Message);
            }
        }

       

        [HttpPost("UpdateCampaignMaster")]
        public IActionResult UpdateCampaignMaster([FromBody] CampaignMaster request)
        {
            try
            {
                // ============================================================
                // 1. VALIDATE REQUEST
                // ============================================================

                if (request == null)
                    return BadRequest("Request body is missing");

                if (string.IsNullOrWhiteSpace(request.Campaign_id))
                    return BadRequest("Campaign_id is mandatory");

                string query = string.Empty;
                string updatedField = string.Empty;

                // ============================================================
                // 2. CHECK WHICH FIELD NEEDS TO BE UPDATED
                // ============================================================

                if (!string.IsNullOrWhiteSpace(request.Campaign_Name))
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2 SET VAR_CAMPAIGN_NAME = @Value WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "Campaign_Name";
                }
                else if (!string.IsNullOrWhiteSpace(request.Status))
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2 SET VAR_STATUS = @Value WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "Status";
                }
                else if (!string.IsNullOrWhiteSpace(request.Campaign_Description))
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2
                SET VAR_CAMPAIGN_DESCRIPTION = @Value
                WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "Campaign_Description";
                }
                else if (!string.IsNullOrWhiteSpace(request.Campaign_Type))
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2
                SET VAR_CAMPAIGN_TYPE = @Value
                WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "Campaign_Type";
                }
                else if (!string.IsNullOrWhiteSpace(request.Time_Zone))
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2
                SET VAR_TIME_ZONE = @Value
                WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "Time_Zone";
                }
                else if (request.Start_Date.HasValue)
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2
                SET VAR_CAMPAIN_START_DATE = @Value
                WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "Start_Date";
                }
                else if (request.End_Date.HasValue)
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2
                SET VAR_CAMPAIN_END_DATE = @Value
                WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "End_Date";
                }
                else if (!string.IsNullOrWhiteSpace(request.Dialing_Mode))
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2
                SET VAR_DIALING_MODE = @Value
                WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "Dialing_Mode";
                }
                else if (!string.IsNullOrWhiteSpace(request.Max_Concurrent_Calls))
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2
                SET VAR_MAX_CONCURRENT_CALLS = @Value
                WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "Max_Concurrent_Calls";
                }
                else if (!string.IsNullOrWhiteSpace(request.Call_duration_Limit))
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2
                SET VAR_CALL_DURATION_LIMIT = @Value
                WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "Call_duration_Limit";
                }
                else if (!string.IsNullOrWhiteSpace(request.Teams))
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2
                SET VAR_TEAMS = @Value
                WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "Teams";
                }
                else if (!string.IsNullOrWhiteSpace(request.Max_Leads))
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2
                SET VAR_MAX_LEADS = @Value
                WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "Max_Leads";
                }
                else if (!string.IsNullOrWhiteSpace(request.Skill_Tags))
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2
                SET VAR_SKILL_TAGS = @Value
                WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "Skill_Tags";
                }
                else if (!string.IsNullOrWhiteSpace(request.Is_Recording))
                {
                    query = @"
                UPDATE TBL_CAMPAIGN_MASTER_V2
                SET VAR_IS_RECORDING = @Value
                WHERE VAR_CAMPAIGN_ID = @Campaign_id";

                    updatedField = "Is_Recording";
                }
                else
                {
                    return BadRequest("No field value provided for update");
                }

                // ============================================================
                // 3. GET VALUE BASED ON FIELD
                // ============================================================

                object value;

                if (updatedField == "Campaign_Name")
                    value = request.Campaign_Name!;
                else if (updatedField == "Status")
                    value = request.Status!;
                else if (updatedField == "Campaign_Description")
                    value = request.Campaign_Description!;
                else if (updatedField == "Campaign_Type")
                    value = request.Campaign_Type!;
                else if (updatedField == "Time_Zone")
                    value = request.Time_Zone!;
                else if (updatedField == "Start_Date")
                    value = request.Start_Date!.Value;
                else if (updatedField == "End_Date")
                    value = request.End_Date!.Value;
                else if (updatedField == "Dialing_Mode")
                    value = request.Dialing_Mode!;
                else if (updatedField == "Max_Concurrent_Calls")
                    value = request.Max_Concurrent_Calls!;
                else if (updatedField == "Call_duration_Limit")
                    value = request.Call_duration_Limit!;
                else if (updatedField == "Teams")
                    value = request.Teams!;
                else if (updatedField == "Max_Leads")
                    value = request.Max_Leads!;
                else if (updatedField == "Skill_Tags")
                    value = request.Skill_Tags!;
                else
                    value = request.Is_Recording!;

                // ============================================================
                // 4. EXECUTE UPDATE
                // ============================================================

                using SqlConnection con = new(_dbConnection);
                using SqlCommand cmd = new(query, con);

                cmd.Parameters.AddWithValue("@Campaign_id", request.Campaign_id);
                cmd.Parameters.AddWithValue("@Value", value);

                con.Open();

                int rows = cmd.ExecuteNonQuery();

                // ============================================================
                // 5. CHECK RESULT
                // ============================================================

                if (rows == 0)
                {
                    return NotFound(new
                    {
                        Message = "Campaign not found",
                        Campaign_id = request.Campaign_id
                    });
                }

                // ============================================================
                // 6. SUCCESS
                // ============================================================

                return Ok(new
                {
                    Message = $"{updatedField} updated successfully",
                    Campaign_id = request.Campaign_id,
                    Updated_Field = updatedField,
                    Updated_Value = value
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error while updating campaign",
                    Error = ex.Message
                });
            }
        }
        private void AddCampaignParameters(SqlCommand cmd, DNIS_TABLE dNIS_TABLE)
        {
            cmd.Parameters.AddWithValue("@VAR_CAMPAIGN_ID", dNIS_TABLE.campaign_id);
            cmd.Parameters.AddWithValue("@VAR_CAMPAIGN_NAME", dNIS_TABLE.campaign_name);
            cmd.Parameters.AddWithValue("@VAR_STATUS", dNIS_TABLE.Status);
            cmd.Parameters.AddWithValue("@VAR_CAMPAIGN_DESCRIPTION", dNIS_TABLE.Campaign_Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VAR_CAMPAIGN_TYPE", dNIS_TABLE.Campaign_Type ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VAR_TIME_ZONE", dNIS_TABLE.Time_Zone);
            cmd.Parameters.AddWithValue("@VAR_CAMPAIN_CREATED_DATE", DateTime.Now);
            cmd.Parameters.AddWithValue("@VAR_CAMPAIN_START_DATE", dNIS_TABLE.Start_Date ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VAR_CAMPAIN_END_DATE", dNIS_TABLE.End_Date ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VAR_CAMPAIGN_START_TIME", dNIS_TABLE.Start_Time ?? (object)DBNull.Value); // TimeSpan or DateTime
            cmd.Parameters.AddWithValue("@VAR_CAMPAIGN_END_TIME", dNIS_TABLE.End_Time ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VAR_DIALING_MODE", dNIS_TABLE.Dialing_Mode ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VAR_MAX_CONCURRENT_CALLS", dNIS_TABLE.Max_Concurrent_Calls ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VAR_CALL_DURATION_LIMIT", dNIS_TABLE.Call_duration_Limit ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VAR_RETRY_ATTEMPTS", dNIS_TABLE.Retry_attempts ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VAR_RETRY_INTERVALS", dNIS_TABLE.Retry_intervals ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VAR_TEAMS", dNIS_TABLE.Teams ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VAR_MAX_LEADS", dNIS_TABLE.Max_Leads ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VAR_SKILL_TAGS", dNIS_TABLE.Skill_Tags ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VAR_IS_RECORDING", dNIS_TABLE.Is_Recording ?? (object)DBNull.Value);
        }
      
        //new working
       
        [HttpPost("outdial")]
        public IActionResult UploadFile([FromForm] IFormFile file, [FromForm] string campaign_id)
        {
            // ============================================================
            // 1. VALIDATE FILE
            // ============================================================

            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    message = "No file uploaded!"
                });
            }

            // ============================================================
            // 2. VALIDATE CAMPAIGN ID
            // ============================================================

            if (!int.TryParse(campaign_id, out int campaignId))
            {
                return BadRequest(new
                {
                    message = "Invalid Campaign ID"
                });
            }

            // ============================================================
            // 3. VALIDATE DATABASE CONNECTION
            // ============================================================

            if (string.IsNullOrWhiteSpace(_dbConnection))
            {
                return StatusCode(500, new
                {
                    message = "Database connection string 'dbcon' not found."
                });
            }

            // ============================================================
            // 4. CAMPAIGN VALUES
            // ============================================================

            string campaignName = string.Empty;
            string dnisExtension = string.Empty;

            int callDurationLimit = 0;
            int retryAttempts = 0;
            int retryIntervals = 0;

            // ============================================================
            // 5. GET CAMPAIGN DETAILS
            // ============================================================

            try
            {
                using (SqlConnection con = new SqlConnection(_dbConnection))
                {
                    con.Open();

                    const string query = @"
        SELECT
            VAR_CAMPAIGN_NAME,
            VAR_CALL_DURATION_LIMIT,
            VAR_RETRY_ATTEMPTS,
            VAR_RETRY_INTERVALS
        FROM TBL_CAMPAIGN_MASTER_V2
        WHERE VAR_CAMPAIGN_ID = @CAMPAIGN_ID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(
                            "@CAMPAIGN_ID",
                            SqlDbType.Int).Value = campaignId;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                return BadRequest(new
                                {
                                    message = "Campaign ID not found.",
                                    campaignId = campaignId
                                });
                            }

                            // =================================================
                            // CAMPAIGN NAME
                            // =================================================

                            campaignName =
                                reader["VAR_CAMPAIGN_NAME"] == DBNull.Value
                                    ? string.Empty
                                    : reader["VAR_CAMPAIGN_NAME"]
                                        .ToString()
                                        ?.Trim() ?? "";

                            // =================================================
                            // CALL DURATION LIMIT
                            // =================================================

                            if (reader["VAR_CALL_DURATION_LIMIT"] != DBNull.Value)
                            {
                                int.TryParse(
                                    reader["VAR_CALL_DURATION_LIMIT"].ToString(),
                                    out callDurationLimit);
                            }

                            // =================================================
                            // RETRY ATTEMPTS
                            // =================================================

                            if (reader["VAR_RETRY_ATTEMPTS"] != DBNull.Value)
                            {
                                int.TryParse(
                                    reader["VAR_RETRY_ATTEMPTS"].ToString(),
                                    out retryAttempts);
                            }

                            // =================================================
                            // RETRY INTERVALS
                            // =================================================

                            if (reader["VAR_RETRY_INTERVALS"] != DBNull.Value)
                            {
                                int.TryParse(
                                    reader["VAR_RETRY_INTERVALS"].ToString(),
                                    out retryIntervals);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error while getting campaign details.",
                    error = ex.Message
                });
            }

            // ============================================================
            // 6. VALIDATE CAMPAIGN NAME
            // ============================================================

            if (string.IsNullOrWhiteSpace(campaignName))
            {
                return BadRequest(new
                {
                    message = "Campaign name is empty.",
                    campaignId = campaignId
                });
            }

            // ============================================================
            // 7. GET DNIS EXTENSION FROM tbl_dnis
            // ============================================================

            try
            {
                using (SqlConnection con = new SqlConnection(_dbConnection))
                {
                    con.Open();

                    const string dnisQuery = @"SELECT VAR_DNIS FROM tbl_dnis WHERE VAR_CAMPAIGN_ID = @CAMPAIGN_ID";

                    using (SqlCommand cmd = new SqlCommand(dnisQuery, con))
                    {
                        cmd.Parameters.Add(
                            "@CAMPAIGN_ID",
                            SqlDbType.Int).Value = campaignId;

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            dnisExtension = result.ToString()?.Trim() ?? "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error while getting DNIS extension.",
                    campaignId = campaignId,
                    //error = ex.Message
                    error = "DNIS extension column was not found in the 'tbl_dnis' table."
                });
            }

            // ============================================================
            // 8. VALIDATE DNIS EXTENSION
            // ============================================================

            if (string.IsNullOrWhiteSpace(dnisExtension))
            {
                return BadRequest(new
                {
                    message = "DNIS extension is not configured.",
                    campaignId = campaignId
                });
            }

            // ============================================================
            // 9. DEFAULT VALUES
            // ============================================================

            if (callDurationLimit <= 0)
                callDurationLimit = 60;

            if (retryAttempts < 0)
                retryAttempts = 0;

            if (retryIntervals < 0)
                retryIntervals = 0;

            // ============================================================
            // 10. CLEAN CAMPAIGN NAME
            // ============================================================

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                campaignName = campaignName.Replace(invalidChar, '_');
            }

            campaignName = campaignName.Trim();

            // ============================================================
            // 11. CREATE LOCAL UPLOAD FOLDER
            // ============================================================

            string uploadFolder =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Uploads");

            try
            {
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Unable to create Uploads folder.",
                    error = ex.Message
                });
            }

            // ============================================================
            // 12. SAVE EXCEL FILE
            // ============================================================

            string originalFileName = Path.GetFileName(file.FileName);

            string excelFileName =
                $"{Guid.NewGuid()}_{originalFileName}";

            string excelFilePath =
                Path.Combine(uploadFolder, excelFileName);

            try
            {
                using (FileStream stream = new FileStream(
                    excelFilePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    81920,
                    FileOptions.SequentialScan))
                {
                    file.CopyTo(stream);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Unable to save uploaded Excel file.",
                    error = ex.Message
                });
            }

            // ============================================================
            // 13. READ EXCEL
            // ============================================================

            List<string[]> rows;

            try
            {
                rows = FileDataReader
                    .ReadTable(excelFilePath)
                    .ToList();
            }
            catch (Exception ex)
            {
                TryDeleteFile(excelFilePath);

                return BadRequest(new
                {
                    message = "Unable to read Excel file.",
                    error = ex.Message
                });
            }

            // ============================================================
            // 14. VALIDATE EXCEL DATA
            // ============================================================

            if (rows == null || rows.Count <= 1)
            {
                TryDeleteFile(excelFilePath);

                return BadRequest(new
                {
                    message = "Excel file does not contain data."
                });
            }

            // ============================================================
            // 15. REMOTE DIRECTORY
            // ============================================================

            //string remoteDirectory =$"/var/spool/asterisk/CallFile/{campaignName}_{campaignId}";
            string remoteDirectory = $"/var/spool/asterisk/CallFile/Campaign-{campaignId}_{campaignId}";

            // ============================================================
            // 16. COUNTERS
            // ============================================================

            int successCount = 0;
            int failedCount = 0;

            var successFiles = new List<string>();
            var failedExcelFiles = new List<string>();
            var failedUploadFiles = new List<string>();

            // ============================================================
            // 17. DELETE DUPLICATE FILES
            // ============================================================

            try
            {
                DeleteDuplicateSftpFiles(
                    remoteDirectory,
                    rows);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Duplicate cleanup error: {ex.Message}");
            }

            // ============================================================
            // 18. UPLOAD CALL FILES
            // ============================================================

            try
            {
                UploadExcelRowsToSftp(
                    rows,
                    campaignId,
                    campaignName,
                    dnisExtension,
                    remoteDirectory,
                    callDurationLimit,
                    retryAttempts,
                    retryIntervals,
                    successFiles,
                    failedExcelFiles,
                    failedUploadFiles,
                    ref successCount,
                    ref failedCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"SFTP processing error: {ex.Message}");

                failedCount++;

                failedUploadFiles.Add(
                    $"SFTP ERROR: {ex.Message}");
            }
            #region
            // ============================================================
            // 19. UPDATE CAMPAIGN MASTER
            // ============================================================

            //    bool campaignUpdated = false;

            //    try
            //    {
            //        using (SqlConnection con =  new SqlConnection(_dbConnection))
            //        {
            //            con.Open();

            //            const string updateQuery = @"
            //UPDATE TBL_CAMPAIGN_MASTER_V2
            //SET
            //    VAR_DESTINATION_FILE_PATH = @DESTINATION_FILE_PATH,
            //    VAR_SOURCE_FILR_PATH      = @VAR_SOURCE_FILR_PATH,
            //    VAR_STATUS                = 'ACTIVE',
            //    VAR_SUCCESS_FILE_COUNT    = @SUCCESS_COUNT,
            //    VAR_FAILED_FILE_COUNT     = @FAILED_COUNT,
            //WHERE VAR_CAMPAIGN_ID = @CAMPAIGN_ID";

            //            using (SqlCommand cmd =
            //                   new SqlCommand(updateQuery, con))
            //            {
            //                cmd.Parameters.Add(
            //                    "@DESTINATION_FILE_PATH",
            //                    SqlDbType.NVarChar,
            //                    500).Value = remoteDirectory;

            //                cmd.Parameters.Add(
            //                    "@VAR_SOURCE_FILR_PATH",
            //                    SqlDbType.NVarChar,
            //                    500).Value = uploadFolder;

            //                cmd.Parameters.Add(
            //                    "@SUCCESS_COUNT",
            //                    SqlDbType.Int).Value = successCount;

            //                cmd.Parameters.Add(
            //                    "@FAILED_COUNT",
            //                    SqlDbType.Int).Value = failedCount;


            //                cmd.Parameters.Add(
            //                    "@CAMPAIGN_ID",
            //                    SqlDbType.Int).Value = campaignId;

            //                int rowsAffected =
            //                    cmd.ExecuteNonQuery();

            //                campaignUpdated =
            //                    rowsAffected > 0;
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(
            //            $"Campaign master update failed: {ex.Message}");
            //    }
            #endregion


            // ============================================================
            // 19. UPDATE CAMPAIGN MASTER
            // ============================================================

            bool campaignUpdated = false;

            try
            {
                using (SqlConnection con = new SqlConnection(_dbConnection))
                {
                    con.Open();

                    const string updateQuery = @"
UPDATE TBL_CAMPAIGN_MASTER_V2
SET
    VAR_DESTINATION_FILE_PATH = @DESTINATION_FILE_PATH,
    VAR_SOURCE_FILR_PATH      = @VAR_SOURCE_FILR_PATH,
    VAR_STATUS                = 'ACTIVE',
    VAR_SUCCESS_FILE_COUNT    = @SUCCESS_COUNT,
    VAR_FAILED_FILE_COUNT     = @FAILED_COUNT

WHERE VAR_CAMPAIGN_ID = @CAMPAIGN_ID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.Add(
                            "@DESTINATION_FILE_PATH",
                            SqlDbType.NVarChar, 500).Value = remoteDirectory;

                        cmd.Parameters.Add(
                            "@VAR_SOURCE_FILR_PATH",
                            SqlDbType.NVarChar, 500).Value = uploadFolder;

                        cmd.Parameters.Add(
                            "@SUCCESS_COUNT",
                            SqlDbType.Int).Value = successCount;

                        cmd.Parameters.Add(
                            "@FAILED_COUNT",
                            SqlDbType.Int).Value = failedCount;

                        cmd.Parameters.Add(
                            "@CAMPAIGN_ID",
                            SqlDbType.Int).Value = campaignId;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        campaignUpdated = rowsAffected > 0;

                        // ✅ Add this to debug if still 0 rows affected
                        Console.WriteLine($"Update rows affected: {rowsAffected}");
                    }
                }
            }
            catch (Exception ex)
            {
                // ✅ Previously this silently swallowed the error
                Console.WriteLine($"Campaign master update failed: {ex.Message}");
            }

            // ============================================================
            // 20. DELETE LOCAL EXCEL
            // ============================================================

            TryDeleteFile(excelFilePath);

            // ============================================================
            // 21. RESPONSE
            // ============================================================

            return Ok(new
            {
                campaignId = campaignId,
                campaignName = campaignName,
                dnisExtension = dnisExtension,
                remoteDirectory = remoteDirectory,
                campaignUpdated = campaignUpdated,
                successCount = successCount,
                failedCount = failedCount,
                successFiles = successFiles,
                failedFiles = failedExcelFiles,
                failedUploadFiles = failedUploadFiles
            });
        }

        private void DeleteDuplicateSftpFiles(string remoteDirectory, List<string[]> rows)
        {
            const string host = "192.168.5.61";
            const int port = 22;
            const string username = "root";
            const string password = "Kaizen^%$#@!";

            // ============================================================
            // BUILD PHONE NUMBER SET FROM EXCEL
            // ============================================================

            var incomingPhoneNumbers =
                new HashSet<string>(
                    rows.Skip(1)
                        .Where(arr =>
                            arr != null &&
                            arr.Length >= 1)
                        .Select(arr =>
                            (arr[0]?.Trim() ?? "")
                                .Replace(" ", "")
                                .Replace("-", "")
                                .Replace("(", "")
                                .Replace(")", ""))
                        .Where(p =>
                            !string.IsNullOrWhiteSpace(p)),
                    StringComparer.OrdinalIgnoreCase);

            if (incomingPhoneNumbers.Count == 0)
                return;

            // ============================================================
            // CONNECT SFTP
            // ============================================================

            using (var sftp =
                   new SftpClient(
                       host,
                       port,
                       username,
                       password))
            {
                sftp.Connect();

                if (!sftp.IsConnected)
                {
                    throw new Exception(
                        "Unable to connect to SFTP server for duplicate cleanup.");
                }

                // ========================================================
                // CHECK REMOTE DIRECTORY
                // ========================================================

                if (!sftp.Exists(remoteDirectory))
                {
                    sftp.Disconnect();
                    return;
                }

                // ========================================================
                // GET REMOTE CALL FILES
                // ========================================================

                var remoteFiles =
                    sftp.ListDirectory(remoteDirectory)
                        .Where(f =>
                            f.IsRegularFile &&
                            f.Name.EndsWith(
                                ".call",
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();

                if (remoteFiles.Count == 0)
                {
                    sftp.Disconnect();
                    return;
                }

                // ========================================================
                // DELETE DUPLICATES
                // ========================================================

                foreach (var remoteFile in remoteFiles)
                {
                    try
                    {
                        string fileNameWithoutExt =
                            Path.GetFileNameWithoutExtension(
                                remoteFile.Name);

                        // Format:
                        // phone_campaignId_random.call

                        string[] parts =
                            fileNameWithoutExt.Split('_');

                        if (parts.Length < 1)
                            continue;

                        string filePhoneNumber =
                            parts[0];

                        if (incomingPhoneNumbers.Contains(
                            filePhoneNumber))
                        {
                            string remoteFilePath =
                                $"{remoteDirectory}/{remoteFile.Name}";

                            sftp.DeleteFile(
                                remoteFilePath);

                            Console.WriteLine(
                                $"Deleted duplicate: {remoteFile.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Failed to delete {remoteFile.Name}: {ex.Message}");
                    }
                }

                // ========================================================
                // DISCONNECT
                // ========================================================

                if (sftp.IsConnected)
                {
                    sftp.Disconnect();
                }
            }
        }
        private void UploadExcelRowsToSftp(List<string[]> rows, int campaignId, string campaignName, string dnisExtension,
            string remoteDirectory, int callDurationLimit, int retryAttempts, int retryIntervals,
            List<string> successFiles, List<string> failedExcelFiles, List<string> failedUploadFiles,
            ref int successCount, ref int failedCount)
        {
            // ============================================================
            // VALIDATION
            // ============================================================

            if (rows == null || rows.Count <= 1)
                return;

            if (string.IsNullOrWhiteSpace(dnisExtension))
            {
                throw new Exception(
                    "DNIS extension is not configured.");
            }

            // ============================================================
            // SFTP DETAILS
            // ============================================================

            const string host = "192.168.5.61";
            const int port = 22;
            const string username = "root";
            const string password = "Kaizen^%$#@!";

            // ============================================================
            // CONNECT SFTP
            // ============================================================

            using (var sftp =
                   new SftpClient(
                       host,
                       port,
                       username,
                       password))
            {
                sftp.Connect();

                if (!sftp.IsConnected)
                {
                    throw new Exception(
                        "Unable to connect to SFTP server.");
                }

                // ========================================================
                // CREATE REMOTE DIRECTORY
                // ========================================================

                if (!sftp.Exists(remoteDirectory))
                {
                    sftp.CreateDirectory(
                        remoteDirectory);
                }

                // ========================================================
                // PROCESS EXCEL ROWS
                // ========================================================

                foreach (string[] arr in rows.Skip(1))
                {
                    string phoneNumber = "";
                    string extension = "";

                    try
                    {
                        // =================================================
                        // VALIDATE ROW
                        // =================================================

                        if (arr == null || arr.Length < 2)
                        {
                            failedCount++;

                            failedExcelFiles.Add(
                                "Invalid row: Phone number or Extension missing.");

                            continue;
                        }

                        // =================================================
                        // GET PHONE NUMBER
                        // =================================================

                        phoneNumber =
                            arr[0]?.Trim() ?? "";

                        phoneNumber =
                            phoneNumber
                                .Replace(" ", "")
                                .Replace("-", "")
                                .Replace("(", "")
                                .Replace(")", "");

                        // =================================================
                        // GET EXTENSION
                        // =================================================

                        extension =
                            arr[1]?.Trim() ?? "";

                        // =================================================
                        // PHONE NUMBER VALIDATION
                        // =================================================

                        if (string.IsNullOrWhiteSpace(phoneNumber))
                        {
                            failedCount++;

                            failedExcelFiles.Add(
                                "Empty phone number.");

                            continue;
                        }

                        if (!phoneNumber.All(char.IsDigit))
                        {
                            failedCount++;

                            failedExcelFiles.Add(
                                $"{phoneNumber} - Invalid phone number.");

                            continue;
                        }

                        if (phoneNumber.Length != 11)
                        {
                            failedCount++;

                            failedExcelFiles.Add(
                                $"{phoneNumber} - Phone number must be 11 digits.");

                            continue;
                        }

                        if (!phoneNumber.StartsWith("0"))
                        {
                            failedCount++;

                            failedExcelFiles.Add(
                                $"{phoneNumber} - Phone number must start with 0.");

                            continue;
                        }

                        // =================================================
                        // EXTENSION VALIDATION
                        // =================================================

                        if (string.IsNullOrWhiteSpace(extension))
                        {
                            failedCount++;

                            failedExcelFiles.Add(
                                $"{phoneNumber} - Extension is empty.");

                            continue;
                        }

                        // =================================================
                        // DNIS EXTENSION CHECK
                        // =================================================

                        if (!string.Equals(
                                extension.Trim(),
                                dnisExtension.Trim(),
                                StringComparison.OrdinalIgnoreCase))
                        {
                            failedCount++;

                            failedExcelFiles.Add(
                                $"{phoneNumber} - Extension mismatch. " +
                                $"Excel Extension: {extension}, " +
                                $"DNIS Extension: {dnisExtension}");

                            // IMPORTANT:
                            // No call file will be created.
                            continue;
                        }

                        // =================================================
                        // CREATE RANDOM NUMBER
                        // =================================================

                        string randomNumber =
                            Random.Shared
                                .Next(100000, 999999)
                                .ToString();

                        // =================================================
                        // CALL FILE NAME
                        // =================================================

                        string callFileName =
                            $"{phoneNumber}_{campaignId}_{randomNumber}.call";

                        // =================================================
                        // LOCAL TEMP FILE
                        // =================================================

                        string tempCallFile =
                            Path.Combine(
                                Path.GetTempPath(),
                                callFileName);

                        // =================================================
                        // CREATE CALL FILE CONTENT
                        // =================================================

                        string callFileContent =
                            $"Channel: PJSIP/{phoneNumber}@out{Environment.NewLine}" +
                            $"WaitTime: {callDurationLimit}{Environment.NewLine}" +
                            $"MaxRetries: {retryAttempts}{Environment.NewLine}" +
                            $"RetryTime: {retryIntervals}{Environment.NewLine}" +
                            $"Context: from-interval{Environment.NewLine}" +
                            $"Extension: {extension}{Environment.NewLine}" +
                            $"Priority: 1{Environment.NewLine}" +
                            $"Setvar: caller_id=out{phoneNumber}{Environment.NewLine}" +
                            $"Setvar: Campaign={campaignId}{Environment.NewLine}" +
                            $"Setvar: Campaign_Name={campaignName}{Environment.NewLine}" +
                            $"Archive: yes{Environment.NewLine}";

                        // =================================================
                        // WRITE LOCAL CALL FILE
                        // =================================================

                        System.IO.File.WriteAllText(
                            tempCallFile,
                            callFileContent);

                        // =================================================
                        // REMOTE FILE PATH
                        // =================================================

                        string remoteFilePath =
                            $"{remoteDirectory}/{callFileName}";

                        // =================================================
                        // UPLOAD TO SFTP
                        // =================================================

                        try
                        {
                            using (FileStream fs =
                                   new FileStream(
                                       tempCallFile,
                                       FileMode.Open,
                                       FileAccess.Read,
                                       FileShare.Read))
                            {
                                sftp.UploadFile(
                                    fs,
                                    remoteFilePath);
                            }

                            successCount++;

                            successFiles.Add(
                                callFileName);
                        }
                        catch (Exception uploadEx)
                        {
                            failedCount++;

                            failedUploadFiles.Add(
                                $"{callFileName} - {uploadEx.Message}");
                        }
                        finally
                        {
                            // =============================================
                            // DELETE TEMP FILE
                            // =============================================

                            TryDeleteFile(
                                tempCallFile);
                        }
                    }
                    catch (Exception rowEx)
                    {
                        failedCount++;

                        string errorFileName =
                            !string.IsNullOrWhiteSpace(phoneNumber)
                                ? phoneNumber
                                : "UNKNOWN";

                        failedUploadFiles.Add(
                            $"{errorFileName} - {rowEx.Message}");
                    }
                }

                // ========================================================
                // DISCONNECT
                // ========================================================

                if (sftp.IsConnected)
                {
                    sftp.Disconnect();
                }
            }
        }

        private bool UploadToSftp(string localFilePath, string remoteDirectory)
        {
            const string host = "192.168.5.61";
            const int port = 22;
            const string username = "root";
            const string password = "Kaizen^%$#@!";

            try
            {
                using (var sftp =
                       new SftpClient(
                           host,
                           port,
                           username,
                           password))
                {
                    sftp.Connect();

                    if (!sftp.IsConnected)
                        return false;

                    if (!sftp.Exists(remoteDirectory))
                    {
                        sftp.CreateDirectory(
                            remoteDirectory);
                    }

                    string fileName =
                        Path.GetFileName(
                            localFilePath);

                    string remoteFilePath =
                        $"{remoteDirectory}/{fileName}";

                    using (FileStream fs =
                           new FileStream(
                               localFilePath,
                               FileMode.Open,
                               FileAccess.Read))
                    {
                        sftp.UploadFile(
                            fs,
                            remoteFilePath);
                    }

                    sftp.Disconnect();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"SFTP Upload Error: {ex.Message}");

                return false;
            }
        }

        private void TryDeleteFile(string filePath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(filePath) &&
                    System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(
                        filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Unable to delete file: {ex.Message}");
            }
        }


        [HttpGet("callactivity")]
        public IActionResult CallActivity()
        {
            var result = new List<TBL_QUEUE_ACTIVITY>();

            try
            {
                using (SqlConnection con = new SqlConnection(_dbConnection))
                {
                    con.Open();
                    string query = @"SELECT VAR_CALLED_DATE, VAR_CALLER_ID, VAR_UNIQUE_ID, VAR_QUEUE_NAME,
                             VAR_AGENT_ID, VAR_STATUS, VAR_WAIT_START_TIME, VAR_WAIT_END_TIME, 
                             VAR_WAIT_DURATION 
                             FROM TBL_QUEUE_ACTIVITY order by VAR_CALLED_DATE desc";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new TBL_QUEUE_ACTIVITY
                            {
                                calleddate = dr["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_CALLED_DATE"]),
                                callerid = dr["VAR_CALLER_ID"]?.ToString(),
                                uniqueid = dr["VAR_UNIQUE_ID"]?.ToString(),
                                queuename = dr["VAR_QUEUE_NAME"]?.ToString(),

                                agentid = dr["VAR_AGENT_ID"] == DBNull.Value ? "" : dr["VAR_AGENT_ID"].ToString()
        .Replace("pjsip/", "", StringComparison.OrdinalIgnoreCase).Replace("pjsip", "", StringComparison.OrdinalIgnoreCase)
        .Trim().Equals("NOT CONNECTED", StringComparison.OrdinalIgnoreCase) ? "" : dr["VAR_AGENT_ID"].ToString()
            .Replace("pjsip/", "", StringComparison.OrdinalIgnoreCase).Replace("pjsip", "", StringComparison.OrdinalIgnoreCase).Trim(),

                                status = dr["VAR_STATUS"] == DBNull.Value ? null : dr["VAR_STATUS"].ToString().Trim().ToLower() == "continue"
        ? "connected" : dr["VAR_STATUS"].ToString(),

                                startdate = dr["VAR_WAIT_START_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_WAIT_START_TIME"]),
                                enddate = dr["VAR_WAIT_END_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_WAIT_END_TIME"]),
                                waitduration = dr["VAR_WAIT_DURATION"]?.ToString()
                            });
                        }
                    }
                }

                if (result.Count == 0)
                    return Ok("Nodata");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("callsinteracting")]
        public IActionResult CallInteracting()
        {
            var result = new List<TBL_CALL_TRANSFER_2>();
            try
            {
                using (SqlConnection Con = new SqlConnection(_dbConnection))
                {
                    Con.Open();
                    string Selectquery = @"
                    SELECT VAR_CALLED_DATE, VAR_CALLER_ID, VAR_UNIQUE_ID,
                           VAR_PATCH_START_TIME, VAR_PATCH_END_TIME,
                           VAR_PATCH_DURATION, VAR_TRANSFERSTATUS
                    FROM TBL_CALL_TRANSFER 
                    WHERE VAR_TRANSFERSTATUS = 'ANSWER'
                    ORDER BY VAR_CALLED_DATE DESC";



                    using (SqlCommand cmd = new SqlCommand(Selectquery, Con))
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DateTime? endtime = dr["VAR_PATCH_END_TIME"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(dr["VAR_PATCH_END_TIME"]);

                            // 👉 If End Time is null → Status = PROGRESS
                            string status = endtime == null
                                ? "LIVECALL"
                                : dr["VAR_TRANSFERSTATUS"]?.ToString();

                            result.Add(new TBL_CALL_TRANSFER_2
                            {
                                calleddate = dr["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_CALLED_DATE"]),
                                callerid = dr["VAR_CALLER_ID"]?.ToString(),
                                uniqueid = dr["VAR_UNIQUE_ID"]?.ToString(),
                                status = status,
                                startdate = dr["VAR_PATCH_START_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_PATCH_START_TIME"]),
                                enddate = endtime,
                                duration = dr["VAR_PATCH_DURATION"]?.ToString()
                            });
                        }
                    }

                    if (!result.Any())
                        return Ok("Nodata");

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("hold")]
        public IActionResult Hold()
        {
            var login = new List<TBL_QUEUE_ACTIVITY>();
            try
            {
                using (SqlConnection con = new SqlConnection(_dbConnection))
                {
                    con.Open();
                    string selectquery = "select * from TBL_QUEUE_ACTIVITY where VAR_CONVERSATION_HOLD > 0 order by VAR_CALLED_DATE desc";
                    using (SqlCommand cmd = new SqlCommand(selectquery, con))
                    {
                        //cmd.Parameters.AddWithValue("@VAR_STATUS", status);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                login.Add(new TBL_QUEUE_ACTIVITY
                                {

                                    calleddate = dr["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_CALLED_DATE"]),
                                    callerid = dr["VAR_CALLER_ID"]?.ToString(),
                                    uniqueid = dr["VAR_UNIQUE_ID"]?.ToString(),
                                    queuename = dr["VAR_QUEUE_NAME"]?.ToString(),

                                    agentid = dr["VAR_AGENT_ID"] == DBNull.Value ? "" : dr["VAR_AGENT_ID"].ToString()
                                              .Replace("pjsip/", "", StringComparison.OrdinalIgnoreCase).Replace("pjsip", "", StringComparison.OrdinalIgnoreCase)
                                              .Trim().Equals("NOT CONNECTED", StringComparison.OrdinalIgnoreCase) ? "" : dr["VAR_AGENT_ID"].ToString()
                                                  .Replace("pjsip/", "", StringComparison.OrdinalIgnoreCase).Replace("pjsip", "", StringComparison.OrdinalIgnoreCase).Trim(),

                                    status = dr["VAR_STATUS"] == DBNull.Value ? null : dr["VAR_STATUS"]
                                             .ToString().Trim().ToLower() == "continue"
                                             ? "connected" : dr["VAR_STATUS"].ToString(),

                                    startdate = dr["VAR_WAIT_START_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_WAIT_START_TIME"]),
                                    enddate = dr["VAR_WAIT_END_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_WAIT_END_TIME"]),
                                    waitduration = dr["VAR_WAIT_DURATION"]?.ToString(),
                                    conversationhold = dr["VAR_CONVERSATION_HOLD"]?.ToString(),
                                });
                            }
                        }
                    }

                    if (login.Count == 0)
                    {
                        return Ok("Nodata");
                    }
                    return Ok(login);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromQuery] string StartDate, [FromQuery] string EndDate)
        {
            var result = new List<TBL_AGENT_DETAILS>();

            try
            {
                if (!string.IsNullOrWhiteSpace(StartDate) && !string.IsNullOrWhiteSpace(EndDate))
                {
                    string[] dateFormats = { "yyyy-MM-dd", "dd-MM-yyyy" };

                    DateTime start = DateTime.ParseExact(StartDate, dateFormats,
                                                        System.Globalization.CultureInfo.InvariantCulture,
                                                        System.Globalization.DateTimeStyles.None).Date;

                    DateTime end = DateTime.ParseExact(EndDate, dateFormats,
                                                      System.Globalization.CultureInfo.InvariantCulture,
                                                      System.Globalization.DateTimeStyles.None)
                                                      .Date.AddDays(1).AddTicks(-1);

                    using (SqlConnection con = new SqlConnection(_dbConnection))
                    {
                        con.Open();

                        string query = @"SELECT * FROM TBL_AGENT_DETAILS 
                        WHERE VAR_STATUS='AVAILABLE'
                        AND VAR_LOGIN_TIME >= @StartDate
                        AND (VAR_LOGOUT_TIME <= @EndDate OR VAR_LOGOUT_TIME IS NULL)
                        ORDER BY VAR_CALLED_DATE DESC";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = start;
                            cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = end;

                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                if (!dr.HasRows)
                                    return Ok("NODATA");

                                while (dr.Read())
                                {
                                    result.Add(new TBL_AGENT_DETAILS
                                    {
                                        calleddate = dr["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_CALLED_DATE"]),
                                        agentid = dr["VAR_AGENT_ID"] == DBNull.Value ? null :
                                                  dr["VAR_AGENT_ID"].ToString()
                                                  .Replace("pjsip/", "", StringComparison.OrdinalIgnoreCase)
                                                  .Replace("pjsip", "", StringComparison.OrdinalIgnoreCase)
                                                  .Trim(),
                                        queuename = dr["VAR_QUEUE_NAME"]?.ToString(),
                                        status = dr["VAR_STATUS"]?.ToString(),
                                        Logindate = dr["VAR_LOGIN_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_LOGIN_TIME"]),
                                        Logoutdate = dr["VAR_LOGOUT_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_LOGOUT_TIME"]),
                                        duration = dr["VAR_DURATION"]?.ToString()
                                    });
                                }
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        public IActionResult Logout([FromQuery] string StartDate, [FromQuery] string EndDate)
        {
            var result = new List<TBL_AGENT_DETAILS>();
            try
            {
                if (!string.IsNullOrWhiteSpace(StartDate) && !string.IsNullOrWhiteSpace(EndDate))
                {
                    string[] dateFormats = { "yyyy-MM-dd", "dd-MM-yyyy" };

                    DateTime start = DateTime.ParseExact(StartDate, dateFormats,
                                                        System.Globalization.CultureInfo.InvariantCulture,
                                                        System.Globalization.DateTimeStyles.None).Date;

                    DateTime end = DateTime.ParseExact(EndDate, dateFormats,
                                                      System.Globalization.CultureInfo.InvariantCulture,
                                                      System.Globalization.DateTimeStyles.None)
                                                      .Date.AddDays(1).AddTicks(-1);

                    using (SqlConnection con = new SqlConnection(_dbConnection))
                    {
                        con.Open();

                        string query = @"SELECT * FROM TBL_AGENT_DETAILS 
                        WHERE VAR_STATUS='UNAVAILABLE'
                        AND VAR_LOGIN_TIME >= @StartDate
                        AND (VAR_LOGOUT_TIME <= @EndDate OR VAR_LOGOUT_TIME IS NULL)
                        ORDER BY VAR_CALLED_DATE DESC";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = start;
                            cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = end;

                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                if (!dr.HasRows)
                                    return Ok("NODATA");

                                while (dr.Read())
                                {
                                    result.Add(new TBL_AGENT_DETAILS
                                    {
                                        calleddate = dr["VAR_CALLED_DATE"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_CALLED_DATE"]),
                                        agentid = dr["VAR_AGENT_ID"] == DBNull.Value ? null :
                                                  dr["VAR_AGENT_ID"].ToString()
                                                  .Replace("pjsip/", "", StringComparison.OrdinalIgnoreCase)
                                                  .Replace("pjsip", "", StringComparison.OrdinalIgnoreCase)
                                                  .Trim(),
                                        queuename = dr["VAR_QUEUE_NAME"]?.ToString(),
                                        status = dr["VAR_STATUS"]?.ToString(),
                                        Logindate = dr["VAR_LOGIN_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_LOGIN_TIME"]),
                                        Logoutdate = dr["VAR_LOGOUT_TIME"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["VAR_LOGOUT_TIME"]),
                                        duration = dr["VAR_DURATION"]?.ToString()
                                    });
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(result);

        }

        [HttpGet("PhoneNumber")]
        public IActionResult Phone_Number([FromQuery] string phoneNumber)
        {
            var result = new List<PAS_SYSTEM>();
            try
            {
                if (phoneNumber != "")
                {
                    string insertquery = "select* from TBL_PAS_SYSTEM where phoneNumber=@PhoneNumber";
                    using (SqlConnection con = new SqlConnection(_dbConnection))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(insertquery, con))
                        {
                            cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                if (dr.HasRows)
                                {
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        while (dr.Read())
                                        {
                                            PAS_SYSTEM pass = new PAS_SYSTEM
                                            {
                                                ssn = Convert.ToInt32(dr["ssn"]),
                                                memberNumber = dr["memberNumber"]?.ToString(),
                                                nin = dr["nin"]?.ToString(),
                                                tin = dr["tin"]?.ToString(),
                                                phoneNumber = dr["phoneNumber"]?.ToString(),
                                                email = dr["email"]?.ToString(),
                                                firstname = dr["firstname"]?.ToString(),
                                                surname = dr["surname"]?.ToString(),
                                                otherNames = dr["otherNames"]?.ToString(),
                                                gender = dr["gender"]?.ToString(),
                                                membershipStatus = dr["membershipStatus"]?.ToString(),
                                                dateOfBirth = dr["dateOfBirth"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["dateOfBirth"]),
                                                dateJoinedScheme = dr["dateJoinedScheme"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["dateJoinedScheme"]),
                                                servicePeriod = (float)Convert.ToDecimal(dr["servicePeriod"]),
                                                createdAt = dr["createdAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["createdAt"]),
                                                updateAt = dr["updateAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["updateAt"])
                                            };
                                            result.Add(pass);
                                        }
                                    }
                                }
                                else
                                {
                                    return Ok("Nodata");
                                }
                            }
                        }
                        con.Close();
                    }
                }
                else
                {
                    return Ok("PhoneNumber not match");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while processing the file.");
            }
            //return Ok("Member Found successfully","data:["+result+"]");
            return Ok(new
            {
                success = true,
                msg = "Member Found successfully",
                data = result
            });
        }
        [HttpPost("Insert_PasSystem")]
        public IActionResult Insert_Pas_System([FromBody] PAS_SYSTEM pAS_SYSTEM)
        {
            try
            {
                using (SqlConnection Insertcon = new SqlConnection(_dbConnection))
                {
                    Insertcon.Open();
                    string insertquery = "Insert into TBL_PAS_SYSTEM (ssn,memberNumber,nin,tin,phoneNumber,email," +
                   "firstname,surname,otherNames,gender,membershipStatus,dateOfBirth,dateJoinedScheme,servicePeriod,createdAt,updateAt)" +
                   "values(@ssn,@memberNumber,@nin,@tin,@phoneNumber,@email,@firstname,@surname,@otherNames,@gender," +
                   "@membershipStatus,@dateOfBirth,@dateJoinedScheme,@servicePeriod,@createdAt,@updateAt)";
                    using (SqlCommand cmd = new SqlCommand(insertquery, Insertcon))
                    {
                        //cmd.Parameters.AddWithValue("@id", pAS_SYSTEM.id);
                        cmd.Parameters.AddWithValue("@ssn", pAS_SYSTEM.ssn);
                        cmd.Parameters.AddWithValue("@memberNumber", pAS_SYSTEM.memberNumber);
                        cmd.Parameters.AddWithValue("@nin", pAS_SYSTEM.nin);
                        cmd.Parameters.AddWithValue("@tin", pAS_SYSTEM.tin);
                        cmd.Parameters.AddWithValue("@phoneNumber", pAS_SYSTEM.phoneNumber);
                        cmd.Parameters.AddWithValue("@email", pAS_SYSTEM.email);
                        cmd.Parameters.AddWithValue("@firstname", pAS_SYSTEM.firstname);
                        cmd.Parameters.AddWithValue("@surname", pAS_SYSTEM.surname);
                        cmd.Parameters.AddWithValue("@otherNames", pAS_SYSTEM.otherNames);
                        cmd.Parameters.AddWithValue("@gender", pAS_SYSTEM.gender);
                        cmd.Parameters.AddWithValue("@membershipStatus", pAS_SYSTEM.membershipStatus);
                        cmd.Parameters.AddWithValue("@dateOfBirth", pAS_SYSTEM.dateOfBirth);
                        cmd.Parameters.AddWithValue("@dateJoinedScheme", pAS_SYSTEM.dateJoinedScheme);
                        cmd.Parameters.AddWithValue("@servicePeriod", pAS_SYSTEM.servicePeriod);
                        cmd.Parameters.AddWithValue("@createdAt", pAS_SYSTEM.createdAt);
                        cmd.Parameters.AddWithValue("@updateAt", pAS_SYSTEM.updateAt);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Ok("Data insert Successfully");
                        }
                        else
                        {
                            return StatusCode(500, "Insert failed");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("Update_PasSystem")]
        public IActionResult Update_Pas_System([FromBody] PAS_SYSTEM pAS_SYSTEM)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_dbConnection))
                {
                    con.Open();

                    string updateQuery = @"UPDATE TBL_PAS_SYSTEM
                    SET memberNumber=@memberNumber,
                        nin=@nin,
                        tin=@tin,
                        phoneNumber=@phoneNumber,
                        email=@email,
                        firstname=@firstname,
                        surname=@surname,
                        otherNames=@otherNames,
                        gender=@gender,
                        membershipStatus=@membershipStatus,
                        dateOfBirth=@dateOfBirth,
                        dateJoinedScheme=@dateJoinedScheme,
                        servicePeriod=@servicePeriod,
                        updateAt=@updateAt
                    WHERE ssn=@ssn";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@ssn", pAS_SYSTEM.ssn);
                        cmd.Parameters.AddWithValue("@memberNumber", pAS_SYSTEM.memberNumber);
                        cmd.Parameters.AddWithValue("@nin", pAS_SYSTEM.nin);
                        cmd.Parameters.AddWithValue("@tin", pAS_SYSTEM.tin);
                        cmd.Parameters.AddWithValue("@phoneNumber", pAS_SYSTEM.phoneNumber);
                        cmd.Parameters.AddWithValue("@email", pAS_SYSTEM.email);
                        cmd.Parameters.AddWithValue("@firstname", pAS_SYSTEM.firstname);
                        cmd.Parameters.AddWithValue("@surname", pAS_SYSTEM.surname);
                        cmd.Parameters.AddWithValue("@otherNames", pAS_SYSTEM.otherNames);
                        cmd.Parameters.AddWithValue("@gender", pAS_SYSTEM.gender);
                        cmd.Parameters.AddWithValue("@membershipStatus", pAS_SYSTEM.membershipStatus);
                        cmd.Parameters.AddWithValue("@dateOfBirth", pAS_SYSTEM.dateOfBirth);
                        cmd.Parameters.AddWithValue("@dateJoinedScheme", pAS_SYSTEM.dateJoinedScheme);
                        cmd.Parameters.AddWithValue("@servicePeriod", pAS_SYSTEM.servicePeriod);
                        cmd.Parameters.AddWithValue("@updateAt", DateTime.Now);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                            return Ok("Data updated successfully");
                        else
                            return NotFound("Record not found");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpDelete("Delete_PasSystem")]
        public IActionResult Delete_Pas_System([FromQuery] int ssn)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_dbConnection))
                {
                    con.Open();

                    string deleteQuery = @"DELETE FROM TBL_PAS_SYSTEM WHERE ssn = @ssn";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                    {
                        cmd.Parameters.Add("@ssn", SqlDbType.Int).Value = ssn;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return NotFound(new
                            {
                                message = "No record found with the given ID"
                            });
                        }

                        return Ok(new
                        {
                            deletedId = ssn,
                            message = "Record deleted successfully"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error occurred",
                    error = ex.Message
                });
            }
        }

        [HttpPost("InsertBlackList")]
        public IActionResult InsertBlackList([FromBody] TBL_BLACKLIST blacklist)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_dbConnection))
                {
                    con.Open();
                    string insertqueryd = @"Insert into TBL_BLACKLIST (VAR_MOBILE_NUMBER)values" +
                        "(@VAR_MOBILE_NUMBER)";
                    using (SqlCommand CMD = new SqlCommand(insertqueryd, con))
                    {
                        CMD.Parameters.AddWithValue("@VAR_MOBILE_NUMBER", blacklist.mobile_number);

                        int rowsAffected = CMD.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Ok("Data insert Successfully");
                        }
                        else
                        {
                            return StatusCode(500, "Insert Failed");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("DeleteBlackList")]
        public IActionResult Delete_BlackList([FromQuery] string NUMBER)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_dbConnection))
                {
                    con.Open();

                    string deleteQuery = @"DELETE FROM TBL_BLACKLIST WHERE VAR_MOBILE_NUMBER= '" + NUMBER + "'";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, con))
                    {
                        //cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return NotFound(new
                            {
                                message = "No record found with the given ID"
                            });
                        }

                        return Ok(new
                        {
                            NUMBER = NUMBER,
                            message = "Record deleted successfully"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Internal server error occurred",
                    error = ex.Message
                });
            }
        }
    }
}
