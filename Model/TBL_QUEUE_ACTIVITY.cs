using System.Text.Json.Serialization;

namespace Call_Details_API.Model
{
    public class TBL_QUEUE_ACTIVITY
    {

        public DateTime? calleddate { get; set; }
        public string? callerid { get; set; }
        public string? uniqueid { get; set; }
        public string? queuename { get; set; }
        public string? agentid { get; set; }
        public string? status { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }

        public string? waitduration { get; set; }

        //public DateTime? talkstarttime { get; set; }

        //public DateTime?  talkendtime { get; set; }

        //public string? talkduration {  get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? conversationhold { get; set; }

        //public DateTime?  abandontime { get; set; }
    }
}
