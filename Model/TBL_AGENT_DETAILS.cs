namespace Call_Details_API.Model
{
    public class TBL_AGENT_DETAILS
    {
        public DateTime? calleddate { get; set; }
        public string? agentid { get; set; }
        public string? queuename { get; set; }
        public string? status { get; set; }
        public DateTime? Logindate { get; set; }
        public DateTime? Logoutdate { get; set; }
        public string? duration { get; set; }
    }
}
