namespace Call_Details_API.Model
{
    public class TBL_CALL_TRANSFER_2
    {
        public DateTime? calleddate { get; set; }
        public string? callerid { get; set; }
        public string? uniqueid { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public string? duration { get; set; }
        public string? status { get; set; }
    }
}
