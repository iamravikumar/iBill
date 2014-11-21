﻿using LyncBillingBase.Helpers;
using LyncBillingBase.Libs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LyncBillingBase.DA
{
    public class DataAccess<T> : IDataAccess<T> where T : class, new()
    {
        /**
         * Private instance variables
         */
        private static DBLib DBRoutines = new DBLib();

        /**
         * Public instance variables
         */
        public string dsName { private set; get; }
        public string IDFieldName { private set; get; }
        public Enums.DataSources dsType { private set; get; }
        public List<DbTableField> Properties { private set; get; }

        private string errorMessage = string.Empty;

        /***
         * Private functions.
         */
        /// <summary>
        /// Tries to read the TableName attribute value if it exists; if it doesn't it throws and exception
        /// </summary>
        /// <returns>TableName attribute value (string), if exists.</returns>
        private void tryReadDataSourceAttributeValue()
        {
            //Get the table name attribute
            IEnumerable<Attribute> dataSourceAtt = typeof(T).GetCustomAttributes(typeof(DataSourceAttribute));

            // This mean that the Class is unstructured Class and it could be related to table/function or procedure or not.
            if (dataSourceAtt.Count() == 0) 
            {
                dsType = Enums.DataSources.Default;
                dsName = "";
            }
            else if(dataSourceAtt.Count() == 1) 
            {
                dsName = ((DataSourceAttribute)dataSourceAtt.First()).Name;

                if (string.IsNullOrEmpty(dsName))
                {
                    throw new Exception(String.Format("DataSource Name was not provided for class \"{0}\". Kindly add the [TableName(...)] Attribute to the class.", typeof(T).Name));
                }

                dsType = ((DataSourceAttribute)dataSourceAtt.First()).SourceType;

                if (dsType == Enums.DataSources.Default)
                {
                    throw new Exception(String.Format("DataSource Name was not provided for class \"{0}\". Kindly add the [TableName(...)] Attribute to the class.", typeof(T).Name));
                }
            }
        }

        /// <summary>
        /// Tries to read the IDField property attribute value if it exists; if it doesn't it throws and exception
        /// </summary>
        /// <returns>IDField attribute value (string), if exists.</returns>
        private string tryReadIDFieldAttributeValue()
        {
            DbTableField IDField;
          
            //Get the IDField DbTableProperty attribute
            IDField = Properties.Find(item => item.IsIDField == true );

            if (IDField != null)
            {
                return IDField.ColumnName;
            }
            else if (string.IsNullOrEmpty(dsName) && dsType == Enums.DataSources.Default)
            {
                return string.Empty;
            }
            else
            {
                throw new Exception(String.Format("No ID field is defined. Kindly annotate the ID property in class \"{0}\" with the [IsIDField] Attribute.", typeof(T).Name));
            }
        }

        /// <summary>
        /// Tries to read the Class Db Properties, which are the properties marked with DbColumn Attribute. It tries to resolve the other attribute values, if they exist, 
        /// otherwise, it assigns the default values.
        /// </summary>
        /// <returns>List of DbTableProperty objects, if the class has DbColumn Properties.</returns>
        private List<DbTableField> tryReadClassDbProperties()
        {
            
            var objProperties = typeof(T).GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).
                                                        Where(property => property.GetCustomAttribute<DbColumnAttribute>() != null).
                                                        ToList();

            if (objProperties != null && objProperties.Count() > 0)
            {
                return (
                    objProperties.Select(item => new DbTableField
                        {
                            ColumnName = item.GetCustomAttribute<DbColumnAttribute>().Name,
                            IsIDField = item.GetCustomAttribute<IsIDFieldAttribute>() != null ? item.GetCustomAttribute<IsIDFieldAttribute>().Status : false,
                            AllowNull = item.GetCustomAttribute<AllowNullAttribute>() != null ? item.GetCustomAttribute<AllowNullAttribute>().Status : false,
                            AllowIDInsert = item.GetCustomAttribute<AllowIDInsertAttribute>() != null ? item.GetCustomAttribute<AllowIDInsertAttribute>().Status : false,
                            FieldType = item.PropertyType
                        })
                        .ToList<DbTableField>()
                );
            }

            throw new Exception(String.Format("Couldn't find any class property marked with the [DbColumn] Attribute in the class \"{0}\". Kindly revise the class definition.", typeof(T).Name));
        }

        private Dictionary<int,string> tryReadClassDBFunctionParameters(T dataObject) 
        {
            Dictionary<int, string> tmpParam = new Dictionary<int,string> ();
            var objProperties = typeof(T).GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).
                                                        Where(property => property.GetCustomAttribute<DbColumnAttribute>(true) != null).
                                                        ToList();
            
            if (objProperties != null && objProperties.Count() > 0) 
            {
               foreach(PropertyInfo property in objProperties)
                {
                   int postion = property.GetCustomAttribute<FunctionsParametersAttribute>().Position;
                   string value =  property.GetCustomAttribute<FunctionsParametersAttribute>().ParamerterName;

                   tmpParam.Add(postion,value);
                } 
            }
            return tmpParam;
        }       


        /**
         * Repository Constructor
         */
        public DataAccess() 
        {
            //Get the Table Name and List of Class Attributes
            try
            {
                this.Properties = tryReadClassDbProperties();
                tryReadDataSourceAttributeValue();
                this.IDFieldName = tryReadIDFieldAttributeValue();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int Insert(T dataObject, string dataSourceName = null, Enums.DataSources dataSource = Enums.DataSources.Default)
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
                    rowID = DBRoutines.INSERT(tableName: dsName, columnsValues: columnsValues, idFieldName: IDFieldName);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }//end-outer-if

            return rowID;  
        }


        public bool Delete(T dataObject, string dataSourceName = null, Enums.DataSources dataSource = Enums.DataSources.Default)
        {
            long ID= 0;

            var dataObjectAttr = dataObject.GetType().GetProperty(IDFieldName);

            if (dataObjectAttr == null)
            {
                throw new Exception("There is no available ID field. kindly annotate " + typeof(T).Name);
            }
            else 
            {
                var dataObjectAttrValue = dataObjectAttr.GetValue(dataObject, null);

                if(dataObjectAttrValue == null)
                {
                    throw new Exception("There is no available ID field is presented but not set kindly set the value of the ID field Object for the following class: " + typeof(T).Name);
                }
                else
                {
                    
                    long.TryParse(dataObjectAttrValue.ToString(),out ID);

                    return DBRoutines.DELETE(tableName: dsName, idFieldName: IDFieldName, ID: ID);
                }//end-inner-if-else
            }//end-outer-if-else
        }


        public bool Update(T dataObject, string dataSourceName = null, Enums.DataSources dataSource = Enums.DataSources.Default)
        {
            Dictionary<string, object> columnsValues = new Dictionary<string, object>();
            bool status = false;

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
                    status = DBRoutines.UPDATE(tableName:dsName,columnsValues:columnsValues,wherePart:null);

                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }

            }//end-outer-if

            return status;  
        }


        public T GetById(long id, string dataSourceName = null, Enums.DataSources dataSource = Enums.DataSources.Default)
        {
            string errorMessage = string.Empty;

            if (id != null && id > 0)
            {
                DataTable dt =  DBRoutines.SELECT(dsName,IDFieldName,id);

                if (dt.Rows.Count == 0)
                {
                    return (T)Activator.CreateInstance(typeof(T));
                }
                else 
                {
                    return dt.ConvertToList<T>().FirstOrDefault<T>()??null; 
                }
            }

            errorMessage = String.Format("The ID Field is either null or zero. Kindly pass a valid ID. Class name: \"{0}\".", typeof(T).Name);

            throw new Exception(errorMessage);
        }


        public IQueryable<T> Get(Expression<Func<T, bool>> predicate, string dataSourceName = null, Enums.DataSources dataSource = Enums.DataSources.Default)
        {
            DataTable dt = new DataTable();

            if (predicate == null) 
            {
                errorMessage =string.Format("There is no defined Predicate. {0} ",typeof(T).Name);
                
                throw new Exception(errorMessage);
            }
            else 
            {
                CustomExpressionVisitor ev = new CustomExpressionVisitor();
                
                string whereClause = ev.Translate(predicate);

                if (string.IsNullOrEmpty(whereClause))
                {
                    dt = DBRoutines.SELECT(dsName);
                }
                else 
                {
                    dt = DBRoutines.SELECT(dsName, whereClause);
                }
            }

            return dt.ConvertToList<T>().AsQueryable<T>();
            
        }


        public IQueryable<T> Get(Dictionary<string, object> whereCondition, int limit = 25, string dataSourceName = null, Enums.DataSources dataSource = Enums.DataSources.Default)
        {
            string errorMessage = string.Empty;
            List<string> allColumns = null;

            if (whereCondition != null && whereCondition.Count > 0)
            {
                DataTable dt = DBRoutines.SELECT(dsName, allColumns, whereCondition, limit);

                return dt.ConvertToList<T>() as IQueryable<T>;
            }

            errorMessage = String.Format("The \"whereConditions\" parameter is either null or empty. Kindly pass a valid \"whereConditions\" parameter. Class name: \"{0}\".", typeof(T).Name);

            throw new Exception(errorMessage);
        }


        public IQueryable<T> GetAll(string dataSourceName = null, Enums.DataSources dataSource = Enums.DataSources.Default)
        {
            int maximumLimit = 0;
            List<string> allColumns = null;
            Dictionary<string, object> whereConditions = null;
            
            DataTable dt  = new DataTable();

            if (dsType == Enums.DataSources.DBTable) 
            {
                dt = DBRoutines.SELECT(dsName, allColumns, whereConditions, maximumLimit);
            }
            
            return (new DataTable().ConvertToList<T>()).AsQueryable<T>();
            
        }



        public bool update(string sqlStatement)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string sqlStatemnet)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Get(string sqlStatemnet)
        {
            throw new NotImplementedException();
        }
    }
}