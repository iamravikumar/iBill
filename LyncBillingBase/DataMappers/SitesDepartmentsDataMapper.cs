﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

using LyncBillingBase.DataAccess;
using LyncBillingBase.DataModels;

namespace LyncBillingBase.DataMappers
{
    public class SitesDepartmentsDataMapper : DataAccess<SiteDepartment>
    {
        /// <summary>
        /// Given a Site's ID, return the list of it's Site-Departments.
        /// </summary>
        /// <param name="SiteID">Site.ID (int)</param>
        /// <returns>List of SiteDepartment objects</returns>
        public List<SiteDepartment> GetBySiteID(long SiteID)
        {
            Dictionary<string, object> condition = new Dictionary<string,object>();
            condition.Add("SiteID", SiteID);

            try
            {
                return Get(whereConditions: condition, limit: 0).ToList<SiteDepartment>();
            }
            catch(Exception ex)
            {
                throw ex.InnerException;
            }
        }


        /// <summary>
        /// Given a Site's ID, return the list of it's Departments.
        /// </summary>
        /// <param name="SiteID">Site.ID (int)</param>
        /// <returns>List of Department objects</returns>
        public List<Department> GetDepartmentsBySiteID(long SiteID)
        {
            List<Department> departments = null;
            List<SiteDepartment> siteDepartments = null;

            Dictionary<string, object> condition = new Dictionary<string, object>();
            condition.Add("SiteID", SiteID);

            try
            {
                siteDepartments = Get(whereConditions: condition, limit: 0).ToList<SiteDepartment>();

                if(siteDepartments != null && siteDepartments.Count > 0)
                {
                    departments = siteDepartments.Select<SiteDepartment, Department>(siteDep => siteDep.Department).ToList<Department>();
                }

                return departments;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

    }

}
