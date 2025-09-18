using System.Globalization;

namespace PushAPI.Model
{
    public class TBL_CALLDETAILS
    {
        //public int SNO { get; set; }
        public DateTime? called_date { get; set; }
        public string? caller_id { get; set; }
        public string? unique_id { get; set; }
        public string? ivr_starttime { get; set; }
        public string? ivr_endtime { get; set; }
        public string? duration { get; set; }
        public string? dnis { get; set; }

        
        // public string? Status { get; set; }
        public string? Call_Type { get; set; }
        public string? Black_list { get; set; }
        public string? rmnin_status { get; set; }

        public class calldetails
        {
            public List<string> UniqueIds { get; set; }
        }
    }

}
