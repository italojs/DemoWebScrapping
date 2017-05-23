using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DemoWebScrapping2.DAL
{

    public abstract class Database
    {
        protected DbConnection connection = null;
        protected DbCommand command = null;

        public abstract void SetDbConnection();
        public abstract void SetBasicDbCommand();

        private void SetParameters(DbCommand commands)
        {
            
            command.CommandText = commands.CommandText;

            foreach (DbParameter command in commands.Parameters)
            {
                DbParameter param = this.command.CreateParameter();

                param.DbType = command.DbType;
                param.Size = command.Value != null ? command.Value.ToString().Length : 0;
                param.Precision = command.Precision;
                param.Scale = command.Scale;

                if (command.Value == null)
                    param.Direction = ParameterDirection.Output;
                else
                {
                    param.Value = command.Value;  
                }
                param.ParameterName = command.ParameterName;

                this.command.Parameters.Add(param);
            }
        }

        public List<object> OExecProc(DbCommand commands)
        {

            List<object> objs = new List<object>();
            try
            {
                if (commands != null)
                {
                    SetParameters(commands);

                    OpenConnection();
                    command.ExecuteNonQuery();

                    foreach (DbParameter parameter in command.Parameters)
                    {
                        if (parameter.Direction == ParameterDirection.Output)
                        {
                            objs.Add(parameter.Value);
                        }
                    }

                    CloseConnection();
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                //Tratar melhor essa mensagem
                throw new Exception(ex.Message);

            }

            return objs;
        }

        public void IExecNonQueryProc(DbCommand commands)
        {
            List<object> objs = new List<object>();
            try
            {
                if (commands != null)
                {
                    SetParameters(commands);

                    OpenConnection();
                    command.ExecuteNonQuery();
                    CloseConnection();
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
        }

        public List<Dictionary<string, object>> IOExecProc(DbCommand commands)
        {
            List<Dictionary<string, object>> lDict = new List<Dictionary<string, object>>();

            try
            {
                using (connection)
                {
                    OpenConnection();
                    using (commands)
                    {
                        SetParameters(commands);
                        DbDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Dictionary<string, object> line = new Dictionary<string, object>();

                            for (int x = 0; x < reader.FieldCount; x++)
                            {
                                if (!reader.IsDBNull(x))
                                    line.Add(reader.GetName(x), reader.GetValue(x).ToString());
                                else
                                    line.Add(reader.GetName(x), 0);
                            }
                            lDict.Add(line);
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

            return lDict;
        }

        private void OpenConnection()
        {
            // connection.ConnectionString = connectionString;
            connection.Open();
        }

        private void CloseConnection()
        {
            connection.Close();
            connection.Dispose();
            command.Dispose();
        }
    }


}
