﻿using LyncBillingBase.Libs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LyncBillingBase.DataAccess;
using LyncBillingBase.DataAttributes;

namespace LyncBillingBase.DataModels
{
    [DataSource(Name = "Sites", SourceType = GLOBALS.DataSourceType.DBTable, AccessType = GLOBALS.DataSourceAccessType.SingleSource)]
    public class Site : DataModel
    {
        [IsIDField]
        [DbColumn("SiteID")]
        public int ID { get; set; }

        [DbColumn("SiteName")]
        public string Name { get; set; }

        [DbColumn("CountryCode")]
        public string CountryCode { get; set; }

        [DbColumn("Description")]
        public string Description { get; set; }

        //[DbColumn("CountryId")]
        //public string CountryId { get; set; }

        //[DataRelation(Name = "SiteID_CountryID", WithDataModel = typeof(Country), OnDataModelKey = "ID", ThisKey = "CountryId")]
        //public Country SiteCountry { get; set; }
    }
}
