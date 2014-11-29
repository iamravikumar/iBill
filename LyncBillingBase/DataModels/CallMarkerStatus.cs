﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LyncBillingBase.DataAccess;
using LyncBillingBase.DataAttributes;

namespace LyncBillingBase.DataModels
{
    [DataSource(Name = "CallMarkerStatus", SourceType = Enums.DataSourceType.DBTable, AccessType = Enums.DataSourceAccessType.SingleSource)]
    public class CallMarkerStatus : DataModel
    {
        [IsIDField]
        [DbColumn("markerId")]
        public int ID { get; set; }

        [DbColumn("phoneCallsTable")]
        public string PhoneCallsTable { get; set; }

        [DbColumn("type")]
        public string Type { get; set; }

        [DbColumn("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
