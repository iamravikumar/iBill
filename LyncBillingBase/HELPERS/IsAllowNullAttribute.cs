﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyncBillingBase.Helpers
{

    [System.AttributeUsage(System.AttributeTargets.Property)]
    class IsAllowNullAttribute : Attribute
    {

        public IsAllowNullAttribute(){}
            
    }
}
