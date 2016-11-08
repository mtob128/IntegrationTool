﻿using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess
{
    public class MsSqlMetadataProvider: IDatabaseMetadataProvider
    {
        private IConnection connection;


        public MsSqlMetadataProvider(IConnection connection)
        {
            this.connection = connection;
        }

        public List<DbMetadataTable> DatabaseTables { get; set; }

        public void Initialize()
        {
            this.DatabaseTables = GetDatabaseTables();
        }

        private List<DbMetadataTable> GetDatabaseTables()
        {
            DataTable dt = null;
            List<DbMetadataTable> tables = new List<DbMetadataTable>();
            using (MssqlWrapper sqlWrapper = new MssqlWrapper(connection.GetConnection() as SqlConnection))
            {
                var dataReader = sqlWrapper.ExecuteQuery("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES");

                dt = DatabaseHelper.ConvertDataReaderToDataTable(dataReader);
            }

            foreach (DataRow dr in dt.Rows)
            {
                string tableName = dr["TABLE_NAME"].ToString();
                tables.Add(
                    new DbMetadataTable()
                    {
                        TableName = tableName,
                        Columns = GetTableColumns(tableName)
                    });
            }

            return tables;
        }

        public List<DbMetadataColumn> GetTableColumns(string tableName)
        {
            List<DbMetadataColumn> columns = new List<DbMetadataColumn>();
            using (MssqlWrapper sqlWrapper = new MssqlWrapper(connection.GetConnection() as SqlConnection))
            {
                var dataReader = sqlWrapper.ExecuteQuery("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = @dbname AND TABLE_NAME = @tablename",
                    new [] 
                    { 
                        new SqlParameter("@dbname", sqlWrapper.DatabaseName),
                        new SqlParameter("@tablename", tableName)
                    });

                DataTable dt = DatabaseHelper.ConvertDataReaderToDataTable(dataReader);
                foreach (DataRow dr in dt.Rows)
                {
                    columns.Add(new DbMetadataColumn() { ColumnName = dr["COLUMN_NAME"].ToString() });
                }
            }

            return columns;
        }
    }
}
