using System.ComponentModel.DataAnnotations;

namespace Campaign_Master.Model
{
    public class CampaignMaster
    {
        
        public string? Campaign_id { get; set; }
        public string? Campaign_Name { get; set; }
        public string? Status { get; set; }
        public string? Campaign_Description { get; set; }
        public string? Campaign_Type { get; set; }
        public string? Time_Zone { get; set; }
        public string? Start_Date { get; set; }
        public string? End_Date { get; set; }
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
}




