using System;

namespace MystropolisExclusive.DataAccess
{
    public class MysticlusiveCode
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public bool OneTimeUse { get; set; }

        public string Video { get; set; }

        public string Remarks { get; set; }

        public int? MinimumDuration { get; set; }

        public bool Used { get; set; }

        public DateTime UsedDateTime { get; set; }
    }
}
