namespace Call_Details_API.Model
{

    #region
    public class DNIS_TABLE
    {
        public string? campaign_id { get; set; }
        public string? campaign_name { get; set; }
        public string? Status { get; set; }
        public string? Campaign_Description { get; set; }
        public string? Campaign_Type { get; set; }
        public string? Time_Zone { get; set; }
        public string? Start_Date { get; set; }
        public string? End_Date { get; set; }
        public string? Start_Time { get; set; }
        public string? End_Time { get; set; }       
        public string? Dialing_Mode { get; set; }
        public string? Max_Concurrent_Calls { get; set; }
        public string? Call_duration_Limit { get; set; }
        public string? Retry_attempts { get; set; }
        public string? Retry_intervals { get; set; }
        public string? Teams { get; set; }
        public string? Max_Leads { get; set; }
        public string? Skill_Tags { get; set; }
        public string? Is_Recording { get; set; }
    }
    #endregion


}
