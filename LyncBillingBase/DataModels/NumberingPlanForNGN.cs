﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

<<<<<<< HEAD
using ORM;
using ORM.DataAccess;
using ORM.DataAttributes;
=======
using CCC.ORM;
using CCC.ORM.DataAccess;
using CCC.ORM.DataAttributes;
>>>>>>> 4d2825ed2d6c07fa47ef8a534e938e39e0b8f09c

namespace LyncBillingBase.DataModels
{
    [DataSource(Name = "NGN_NumberingPlan", Type = GLOBALS.DataSource.Type.DBTable, AccessMethod = GLOBALS.DataSource.AccessMethod.SingleSource)]
    public class NumberingPlanForNGN : DataModel
    {
        [IsIDField]
        [DbColumn("ID")]
        public long ID { get; set; }

        [DbColumn("DialingCode")]
        public string DialingCode { get; set; }

        [DbColumn("CountryCodeISO3")]
        public string ISO3CountryCode { get; set; }

        [AllowNull]
        [DbColumn("Provider")]
        public string Provider { get; set; }

        [AllowNull]
        [DbColumn("Description")]
        public string Description { get; set; }

        [AllowNull]
        [DbColumn("TypeOfServiceID")]
        public int TypeOfServiceID { get; set; }


        //
        // Relations
        [DataRelation(WithDataModel = typeof(CallType), OnDataModelKey = "TypeID", ThisKey = "TypeOfServiceID")]
        public CallType TypeOfService { get; set; }

        [DataRelation(WithDataModel = typeof(Country), OnDataModelKey = "ISO3Code", ThisKey = "ISO3CountryCode")]
        public Country Country { get; set; }

        //[DataRelation(Name="CountryID_Country.ID", WithDataModel = typeof(Country), OnDataModelKey = "ID", ThisKey = "CountryID")]
        //public Country Country { get; set; }
    }
}
