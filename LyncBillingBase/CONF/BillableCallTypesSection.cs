﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyncBillingBase.Conf
{

    public class BillableTypeElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return this["name"].ToString(); }
        }

        [ConfigurationProperty("value")]
        public int Value
        {
            get { return Convert.ToInt32(this["value"]); }
        }
    }

    public class BillableTypeCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new BillableTypeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((BillableTypeElement)element).Name;
        }
    }

    public class BillableCallTypesSection : ConfigurationSection
    {
        public static string ConfigurationSectionName
        {
            get { return "BillableCallTypesSection"; }
        }

        [ConfigurationProperty("BillableTypes")]
        public BillableTypeCollection BillableTypes
        {
            get { return (BillableTypeCollection)this["BillableTypes"]; }
        }

        public List<int> BillableTypesList
        {
            get
            {
                List<int> billableTypesList = new List<int>();

                foreach (BillableTypeElement el in BillableTypes)
                {
                    billableTypesList.Add(el.Value);
                }

                return billableTypesList;
            }
        }

        public List<int> FixedlinesIdsList
        {
            get
            {
                List<int> fixedlinesIdsList = new List<int>();

                foreach (BillableTypeElement el in BillableTypes)
                {
                    if (el.Name.Contains("FIXEDLINE"))
                        fixedlinesIdsList.Add(el.Value);
                    else
                        continue;
                }

                return fixedlinesIdsList;
            }
        }

        public List<int> NGNLinesIdsList
        {
            get
            {
                List<int> ngnlinesIdsList = new List<int>();

                foreach (BillableTypeElement el in BillableTypes)
                {
                    if (el.Name.Contains("NGN") || el.Name.Contains("TOLL-FREE"))
                        ngnlinesIdsList.Add(el.Value);
                    else
                        continue;
                }

                return ngnlinesIdsList;
            }
        }

        public List<int> MobileLinesIdsList
        {
            get
            {
                List<int> mobilelinesIdsList = new List<int>();

                foreach (BillableTypeElement el in BillableTypes)
                {
                    if (el.Name.Contains("MOBILE"))
                        mobilelinesIdsList.Add(el.Value);
                    else
                        continue;
                }

                return mobilelinesIdsList;
            }
        }

        //Easier to compare against agnostic keyValuePair with typeof functions
        public Array BillableTypesArrayList
        {
            get
            {
                List<int> billableTypesList = new List<int>();

                foreach (BillableTypeElement el in BillableTypes)
                {
                    billableTypesList.Add(el.Value);
                }

                return billableTypesList.ToArray();
            }
        }

    }
}
