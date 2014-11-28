﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LyncBillingBase.DataAttributes;

namespace LyncBillingBase.DataModels
{
    [DataSource(Name = "DIDs", SourceType = Enums.DataSourceType.DBTable, AccessType = Enums.DataSourceAccessType.SingleSource)]
    public class DID
    {
        [IsIDField]
        [DbColumn("ID")]
        public int ID { get; set; }

        [DbColumn("Regex")]
        public string Regex { get; set; }

        [AllowNull]
        [DbColumn("Description")]
        public string Description { get; set; }

        [AllowNull]
        [DbColumn("SiteID")]
        public int SiteID { get; set; }

        public string SiteName { get; set; }
    }
}
