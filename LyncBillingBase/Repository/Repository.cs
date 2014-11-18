﻿using LyncBillingBase.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyncBillingBase.Helpers;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Common;
using System.Data;
using System.Data.Linq;


namespace LyncBillingBase.Repository
{
    public class Repository<T> : IRepository<T>
    {
        /**
         * Helper classes
         */
        public class DbTableProperty
        {
            public string ColumnName { get; set; }
            public bool IsIDField { get; set; }
            public bool AllowNull { get; set; }
            public bool AllowIDInsert { get; set; }
            public Type FieldType { get; set; }
        }

        /**
         * Private instance variables
         */
        private static DBLib DBRoutines = new DBLib();

        /**
         * Public instance variables
         */
        public string TableName { private set; get; }
        public string IDFieldName { private set; get; }
        public List<DbTableProperty> Properties { private set; get; }

        /***
         * Private functions.
         */
        /// <summary>
        /// Tries to read the TableName attribute value if it exists; if it doesn't it throws and exception
        /// </summary>
        /// <returns>TableName attribute value (string), if exists.</returns>
        private string tryReadTableNameAttributeValue()
        {
            var tableName = typeof(T).GetCustomAttributes(typeof(TableNameAttribute));

            if (tableName != null || tableName.Count() > 0)
            {
                return ((TableNameAttribute)tableName.First()).Name; 
            }

            throw new Exception("Table Name was not provided for " + typeof(T).GetType().Name + ". Kindly add [TableName(...)] attribute to the class.");
        }

        /// <summary>
        /// Tries to read the IDField property attribute value if it exists; if it doesn't it throws and exception
        /// </summary>
        /// <returns>IDField attribute value (string), if exists.</returns>
        private string tryReadIDFieldAttributeValue()
        {
            var IDField = Properties.Find(item => item.IsIDField == true);

            if (IDField != null)
            {
                this.IDFieldName = IDField.ColumnName;
            }

            throw new Exception("No ID field is defined. Kindly annotate " + typeof(T).GetType().Name + " ID property with [IsIdField] attribute.");
        }

        /// <summary>
        /// Tries to read the Class Db Properties, which are the properties marked with DbColumn Attribute. It tries to resolve the other attribute values, if they exist, 
        /// otherwise, it assigns the default values.
        /// </summary>
        /// <returns>List of DbTableProperty objects, if the class has DbColumn Properties.</returns>
        private List<DbTableProperty> tryReadClassDbProperties()
        {
            var objProperties = typeof(T).GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (objProperties != null || objProperties.Count() > 0)
            {
                return (
                    objProperties.Where(property => property.GetCustomAttribute<DbColumnAttribute>() != null)
                        .Select(item => new DbTableProperty
                        {
                            ColumnName = item.GetCustomAttribute<DbColumnAttribute>().Name,
                            IsIDField = item.GetCustomAttribute<IsIDFieldAttribute>() != null ? item.GetCustomAttribute<IsIDFieldAttribute>().Status : false,
                            AllowNull = item.GetCustomAttribute<AllowNullAttribute>() != null ? item.GetCustomAttribute<AllowNullAttribute>().Status : false,
                            AllowIDInsert = item.GetCustomAttribute<AllowIDInsertAttribute>() != null ? item.GetCustomAttribute<AllowIDInsertAttribute>().Status : false,
                            FieldType = item.PropertyType
                        })
                        .ToList<DbTableProperty>()
                );
            }

            throw new Exception("Cant find valid properties in " + typeof(T).GetType().Name);
        }


        /**
         * Repository Constructor
         */
        public Repository() 
        {
            //Get the Table Name and List of Class Attributes
            try
            {
                this.TableName = tryReadTableNameAttributeValue();
                this.IDFieldName = tryReadIDFieldAttributeValue();
                this.Properties = tryReadClassDbProperties();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 

        public int Insert(T dataObject)
        {
            int rowID = 0;
            Dictionary<string, object> columnsValues = new Dictionary<string, object>();
          
            if (dataObject != null)
            {
                foreach (var property in Properties)
                {
                    var dataObjectAttr = dataObject.GetType().GetProperty(property.ColumnName);

                    //Don't insert ID Fields into the Database
                    if(property.IsIDField == true)
                    {
                        continue;
                    }

                    //Continue handling the properties
                    if (property.AllowNull == false && dataObjectAttr != null)
                    {
                        var dataObjectAttrValue = dataObjectAttr.GetValue(dataObject, null);

                        if (dataObjectAttrValue != null)
                        {
                            columnsValues.Add(property.ColumnName, Convert.ChangeType(dataObjectAttrValue, property.FieldType));
                        }
                        else
                        {
                            throw new Exception("The Property " + property.ColumnName + " in the " + dataObject.GetType().Name + " Table is not allowed to be null kindly annotate the property with [IsAllowNull]");
                        }
                    }
                    else
                    {
                        var dataObjectAttrValue = dataObjectAttr.GetValue(dataObject, null);

                        if (dataObjectAttrValue != null)
                        {
                            columnsValues.Add(property.ColumnName, Convert.ChangeType(dataObjectAttrValue, property.FieldType));
                        }
                    }
                    //end-inner-if

                }//end-foreach

                try
                {
                  
                   
                    rowID = DBRoutines.INSERT(tableName: TableName, columnsValues: columnsValues, idFieldName: IDFieldName);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }//end-outer-if

            return rowID;  
        }

        public bool Delete(T dataObject)
        {
            long ID= 0;

            var dataObjectAttr = dataObject.GetType().GetProperty(IDFieldName);

            if (dataObjectAttr == null)
            {
                throw new Exception("There is no available ID field. kindly annotate " + dataObject.GetType().Name);
            }
            else 
            {
                var dataObjectAttrValue = dataObjectAttr.GetValue(dataObject, null);

                if(dataObjectAttrValue == null)
                {
                    throw new Exception("There is no available ID field is presented but not set kindly set the value of the ID field Object for the following class: " + dataObject.GetType().Name);
                }
                else
                {
                    
                    long.TryParse(dataObjectAttrValue.ToString(),out ID);

                    return DBRoutines.DELETE(tableName: TableName, idFieldName: IDFieldName, ID: ID);
                }
                
            }

        }
        
        public bool Update(T dataObject)
        {
            throw new NotImplementedException();
        }

        public T GetById(long id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Get(Dictionary<string, object> where, int limit = 25)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}