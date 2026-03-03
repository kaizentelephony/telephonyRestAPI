namespace Call_Details_API.Model
{
    public class TBL_CAMPAIGN_DETAILS
    {
        #region
        //public int? VAR_SNO { get; set; }
        //public string? VAR_CALLER_ID { get; set; }

        //public string? VAR_CHANNEL_ID { get; set; }

        //public string? VAR_WAIT_TIME { get; set; }
        //public string? VAR_MAXRETRIES { get; set; }
        //public string? VAR_RETRYTIME { get; set; }
        //public string? VAR_EXTENSION { get; set; }
        //public string? VAR_STATUS { get; set; }
        #endregion
        public int? VAR_SNO { get; set; }
        public string? VAR_CALLER_ID { get; set; }

        public string? VAR_CHANNEL_ID { get; set; }

        public int? VAR_WAIT_TIME { get; set; }
        public int? VAR_MAXRETRIES { get; set; }
        public int? VAR_RETRYTIME { get; set; }
        public string? VAR_EXTENSION { get; set; }
        public string? VAR_STATUS { get; set; }
        public TimeSpan? VAR_CAMPAIGN_START_TIME { get; set; }
        public string? VAR_CAMPAIGN_ID { get; set; }
    }
}
