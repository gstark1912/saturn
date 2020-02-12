using System;

namespace Model.Model.Customer
{
    public class MailRecord
    {
        public MailRecord()
        {
            this.TimeStamp = DateTime.Now;
        }
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}