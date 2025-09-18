using Microsoft.AspNetCore.Mvc.Controllers;

namespace PushAPI.Model
{
    public class TBL_CALL_TRANSFER
    {
        //public int VarSno { get; set; }
        public DateTime? called_date { get; set; }
        public string caller_id { get; set; }
        public string unique_id { get; set; }
        public DateTime? patch_starttime { get; set; }
        public DateTime? patch_endtime { get; set; } 
        public string  duration { get; set; }
        
        public string transfer_vdn { get; set; }
        public string transfer_status { get; set; }

    }



}
