﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LyncBillingBase.DataAccess;
using LyncBillingBase.DataAttributes;

namespace LyncBillingBase.DataModels
{
    [DataSource(Name = "NEW_Currencies", SourceType = GLOBALS.DataSourceType.DBTable, AccessType = GLOBALS.DataSourceAccessType.SingleSource)]
    public class Currency : DataModel
    {
        [IsIDField]
        [DbColumn("ID")]
        public int ID { get; set; }

        [DbColumn("Name")]
        public string Name { get; set; }

        [DbColumn("ISO3Code")]
        public string ISO3Code { get; set; }
    }
}
