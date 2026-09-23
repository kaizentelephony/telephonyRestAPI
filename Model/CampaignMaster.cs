namespace Call_Details_API.Model
{
    public class CampaignMaster
{
    [Browsable(false)]
    public string? Campaign_id { get; set; }
    public string? Campaign_Name { get; set; }
    public string? Status { get; set; }
    public string? Campaign_Description { get; set; }
    public string? Campaign_Type { get; set; }
    public string? Time_Zone { get; set; }
    public DateTime? Start_Date { get; set; }
    public DateTime? End_Date { get; set; }
    public string? Dialing_Mode { get; set; }
    public string? Max_Concurrent_Calls { get; set; }
    public string? Call_duration_Limit { get; set; }
    public string? Teams { get; set; }
    public string? Max_Leads { get; set; }
    public string? Skill_Tags { get; set; }
    public string? Is_Recording { get; set; }
    public int? SuccessCount { get; set; }
    public int? FailedCount { get; set; }
    public string? Extension_Number { get; set; }
}
public class DNIS
{
    public string? DNIS_EXTENSION { get; set; }
    public string? CAMPAIGN_ID { get; set; }

}
}
