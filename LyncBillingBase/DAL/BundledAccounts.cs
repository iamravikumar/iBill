﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyncBillingBase.Helpers;

namespace LyncBillingBase.DAL
{
    [TableName("BundledAccounts")]
    public class BundledAccounts
    {
        [IsIDField]
        [DbColumn("ID")]
        public int ID { get; }

        [DbColumn("PrimarySipAccount")]
        public string PrimarySipAccount { get; set; }

        [DbColumn("AssociatedSipAccount")]
        public List<string> AssociatedSipAccounts { get; set; }

        public Users PrimaryUserAccount { get; set; }
    }
}
