using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Collections;
using System.Data.SQLite;
using System.Configuration;//添加.net引用  
using System.Security.Cryptography;
namespace Weibo.Core
{
    /// <summary>  
    /// 对SQLite操作的类  
    /// 引用：System.Data.SQLite.dll【版本：3.6.23.1（原始文件名：SQlite3.DLL）】  
    /// </summary>  
    public class SQLiteHelper
    {
        /// <summary>  
        /// 所有成员函数都是静态的，构造函数定义为私有  
        /// </summary>  
        private SQLiteHelper()
        {
        }
        /// <summary>  
        /// 连接字符串  
        /// </summary>  
        public static string ConnectionString
        {//"Data Source=Test.db3;Pooling=true;FailIfMissing=false";  
            get
            {
                ////(AppSettings节点下的"SQLiteConnectionString")  
                string text = ConfigurationManager.AppSettings["SQLiteConnectionString"];
                
                return text;
            }
        }
        private static SQLiteConnection _Conn = null;
        /// <summary>  
        /// 连接对象  
        /// </summary>  
        public static SQLiteConnection Conn
        {
            get
            {
                if (_Conn == null) _Conn = new SQLiteConnection(ConnectionString);
                return SQLiteHelper._Conn;
            }
            set { SQLiteHelper._Conn = value; }
        }


        #region CreateCommand(commandText,SQLiteParameter[])  
        /// <summary>  
        /// 创建命令  
        /// </summary>  
        /// <param name="connection">连接</param>  
        /// <param name="commandText">语句</param>  
        /// <param name="commandParameters">语句参数.</param>  
        /// <returns>SQLite Command</returns>  
        public static SQLiteCommand CreateCommand(string commandText, params SQLiteParameter[] commandParameters)
        {
            SQLiteCommand cmd = new SQLiteCommand(commandText, Conn);
            if (commandParameters.Length > 0)
            {
                foreach (SQLiteParameter parm in commandParameters)
                    cmd.Parameters.Add(parm);
            }
            return cmd;
        }
        #endregion


        #region CreateParameter(parameterName,parameterType,parameterValue)  
        /// <summary>  
        /// 创建参数  
        /// </summary>  
        /// <param name="parameterName">参数名</param>  
        /// <param name="parameterType">参数类型</param>  
        /// <param name="parameterValue">参数值</param>  
        /// <returns>返回创建的参数</returns>  
        public static SQLiteParameter CreateParameter(string parameterName, System.Data.DbType parameterType, object parameterValue)
        {
            SQLiteParameter parameter = new SQLiteParameter();
            parameter.DbType = parameterType;
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            return parameter;
        }
        #endregion

        #region ExecuteDataSet(commandText,paramList[])  
        /// <summary>  
        /// 查询数据集  
        /// </summary>  
        /// <param name="cn">连接.</param>  
        /// <param name="commandText">查询语句.</param>  
        /// <param name="paramList">object参数列表.</param>  
        /// <returns></returns>  
        public static DataSet ExecuteDataSet(string commandText, params object[] paramList)
        {

            SQLiteCommand cmd = Conn.CreateCommand();
            cmd.CommandText = commandText;
            if (paramList != null)
            {
                AttachParameters(cmd, commandText, paramList);
            }
            DataSet ds = new DataSet();
            if (Conn.State == ConnectionState.Closed)
                Conn.Open();
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(ds);
            da.Dispose();
            cmd.Dispose();
            Conn.Close();
            return ds;
        }
        #endregion

        #region ExecuteDataSet(SQLiteCommand)  
        /// <summary>  
        /// 查询数据集  
        /// </summary>  
        /// <param name="cmd">SQLiteCommand对象</param>  
        /// <returns>返回数据集</returns>  
        public static DataSet ExecuteDataSet(SQLiteCommand cmd)
        {
            if (cmd.Connection.State == ConnectionState.Closed)
                cmd.Connection.Open();
            DataSet ds = new DataSet();
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            da.Fill(ds);
            da.Dispose();
            cmd.Connection.Close();
            cmd.Dispose();
            return ds;
        }
        #endregion

        #region ExecuteDataSet(SQLiteTransaction,commandText,params SQLiteParameter[])  
        /// <summary>  
        /// 查询数据集  
        /// </summary>  
        /// <param name="transaction">SQLiteTransaction对象. </param>  
        /// <param name="commandText">查询语句.</param>  
        /// <param name="commandParameters">命令的参数列表.</param>  
        /// <returns>DataSet</returns>  
        /// <remarks>必须手动执行关闭连接transaction.connection.Close</remarks>  
        public static DataSet ExecuteDataSet(SQLiteTransaction transaction, string commandText, params SQLiteParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rolled back or committed, please provide an open transaction.", "transaction");
            IDbCommand cmd = transaction.Connection.CreateCommand();
            cmd.CommandText = commandText;
            foreach (SQLiteParameter parm in commandParameters)
            {
                cmd.Parameters.Add(parm);
            }
            if (transaction.Connection.State == ConnectionState.Closed)
                transaction.Connection.Open();
            DataSet ds = ExecuteDataSet((SQLiteCommand)cmd);
            return ds;
        }
        #endregion

        #region ExecuteDataSet(SQLiteTransaction,commandText,object[] commandParameters)  
        /// <summary>  
        /// 查询数据集  
        /// </summary>  
        /// <param name="transaction">SQLiteTransaction对象 </param>  
        /// <param name="commandText">查询语句.</param>  
        /// <param name="commandParameters">命令参数列表</param>  
        /// <returns>返回数据集</returns>  
        /// <remarks>必须手动执行关闭连接transaction.connection.Close</remarks>  
        public static DataSet ExecuteDataSet(SQLiteTransaction transaction, string commandText, object[] commandParameters)
        {

            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rolled back or committed,                                                          please provide an open transaction.", "transaction");
            IDbCommand cmd = transaction.Connection.CreateCommand();
            cmd.CommandText = commandText;
            AttachParameters((SQLiteCommand)cmd, cmd.CommandText, commandParameters);
            if (transaction.Connection.State == ConnectionState.Closed)
                transaction.Connection.Open();

            DataSet ds = ExecuteDataSet((SQLiteCommand)cmd);
            return ds;
        }
        #endregion

        #region UpdateDataset(insertCommand,deleteCommand,updateCommand,dataSet,tableName)  
        /// <summary>  
        /// 更新数据集中数据到数据库  
        /// </summary>  
        /// <param name="insertCommand">insert语句</param>  
        /// <param name="deleteCommand">delete语句</param>  
        /// <param name="updateCommand">update语句</param>  
        /// <param name="dataSet">要更新的DataSet</param>  
        /// <param name="tableName">数据集中要更新的table名</param>  
        public static void UpdateDataset(SQLiteCommand insertCommand, SQLiteCommand deleteCommand, SQLiteCommand updateCommand, DataSet dataSet, string tableName)
        {
            if (insertCommand == null) throw new ArgumentNullException("insertCommand");
            if (deleteCommand == null) throw new ArgumentNullException("deleteCommand");
            if (updateCommand == null) throw new ArgumentNullException("updateCommand");
            if (tableName == null || tableName.Length == 0) throw new ArgumentNullException("tableName");

            // Create a SQLiteDataAdapter, and dispose of it after we are done  
            using (SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter())
            {
                // Set the data adapter commands  

                dataAdapter.UpdateCommand = updateCommand;
                dataAdapter.InsertCommand = insertCommand;
                dataAdapter.DeleteCommand = deleteCommand;
                // Update the dataset changes in the data source  
                dataAdapter.Update(dataSet, tableName);

                // Commit all the changes made to the DataSet  
                dataSet.AcceptChanges();
            }
        }
        #endregion

        #region ExecuteReader(SQLiteCommand,commandText, object[] paramList)  
        /// <summary>  
        /// ExecuteReader方法  
        /// </summary>  
        /// <param name="cmd">查询命令</param>  
        /// <param name="commandText">含有类似@colume参数的sql语句</param>  
        /// <param name="paramList">语句参数列表</param>  
        /// <returns>IDataReader</returns>  
        public static IDataReader ExecuteReader(SQLiteCommand cmd, string commandText, object[] paramList)
        {
            if (cmd.Connection == null)
                throw new ArgumentException("没有为命令指定活动的连接.", "cmd");
            cmd.CommandText = commandText;
            AttachParameters(cmd, commandText, paramList);
            if (cmd.Connection.State == ConnectionState.Closed)
                cmd.Connection.Open();
            IDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return rdr;
        }
        #endregion

        #region ExecuteNonQuery(commandText,paramList)  
        /// <summary>  
        /// 执行ExecuteNonQuery方法  
        /// </summary>  
        /// <param name="cn">连接</param>  
        /// <param name="commandText">语句</param>  
        /// <param name="paramList">参数</param>  
        /// <returns></returns>  
        public static int ExecuteNonQuery(string commandText, params object[] paramList)
        {

            SQLiteCommand cmd = Conn.CreateCommand();
            cmd.CommandText = commandText;
            AttachParameters(cmd, commandText, paramList);
            if (Conn.State == ConnectionState.Closed)
                Conn.Open();
            int result = cmd.ExecuteNonQuery();
            cmd.Dispose();
            Conn.Close();

            return result;
        }
        #endregion

        #region ExecuteNonQuery(SQLiteTransaction,commandText,paramList)  
        /// <summary>  
        /// 执行ExecuteNonQuery方法,带事务  
        /// </summary>  
        /// <param name="transaction">之前创建好的SQLiteTransaction对象</param>  
        /// <param name="commandText">语句.</param>  
        /// <param name="paramList">参数.</param>  
        /// <returns>返回影响的行数</returns>  
        /// <remarks>  
        /// 定义事务  DbTransaction trans = conn.BeginTransaction();  
        ///     或者：SQLiteTransaction trans = Conn.BeginTransaction();  
        /// 操作代码示例：  
        /// try  
        ///{  
        ///    // 连续操作记录   
        ///    for (int i = 0; i < 1000; i++)  
        ///    {  
        ///        ExecuteNonQuery(trans,commandText,[] paramList);  
        ///    }  
        ///    trans.Commit();  
        ///}  
        ///catch  
        ///{  
        ///    trans.Rollback();  
        ///    throw;  
        ///}  
        ///trans.Connection.Close();//关闭事务连接  
        ///transaction.Dispose();//释放事务对象  
        /// </remarks>  
        public static int ExecuteNonQuery(SQLiteTransaction transaction, string commandText, params object[] paramList)
        {
            if (transaction == null) throw new ArgumentNullException("transaction is null");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rolled back or committed,please provide an open transaction.", "transaction");
            using (IDbCommand cmd = transaction.Connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                AttachParameters((SQLiteCommand)cmd, cmd.CommandText, paramList);
                if (transaction.Connection.State == ConnectionState.Closed)
                    transaction.Connection.Open();
                int result = cmd.ExecuteNonQuery();
                return result;
            }

        }
        #endregion

        #region ExecuteNonQuery(IDbCommand)  
        /// <summary>  
        /// 执行ExecuteNonQuery方法  
        /// </summary>  
        /// <param name="cmd">创建好的命令.</param>  
        /// <returns></returns>  
        public static int ExecuteNonQuery(IDbCommand cmd)
        {
            if (cmd.Connection.State == ConnectionState.Closed)
                cmd.Connection.Open();
            int result = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
            cmd.Dispose();
            return result;
        }
        #endregion

        #region ExecuteScalar(commandText,paramList)  
        /// <summary>  
        /// 执行ExecuteScalar  
        /// </summary>  
        /// <param name="commandText">语句s</param>  
        /// <param name="paramList">参数</param>  
        /// <returns></returns>  
        public static object ExecuteScalar(string commandText, params object[] paramList)
        {
            SQLiteConnection cn = new SQLiteConnection(ConnectionString);
            SQLiteCommand cmd = cn.CreateCommand();
            cmd.CommandText = commandText;
            AttachParameters(cmd, commandText, paramList);
            if (cn.State == ConnectionState.Closed)
                cn.Open();
            object result = cmd.ExecuteScalar();
            cmd.Dispose();
            cn.Close();
            return result;
        }
        #endregion

        #region ExecuteXmlReader(IDbCommand)  
        /// <summary>  
        /// ExecuteXmlReader返回xml格式  
        /// </summary>  
        /// <param name="command">语句</param>  
        /// <returns>返回XmlTextReader对象</returns>  
        public static XmlReader ExecuteXmlReader(IDbCommand command)
        { // open the connection if necessary, but make sure we   
            // know to close it when we�re done.  
            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
            }

            // get a data adapter    
            SQLiteDataAdapter da = new SQLiteDataAdapter((SQLiteCommand)command);
            DataSet ds = new DataSet();
            // fill the data set, and return the schema information  
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            da.Fill(ds);
            // convert our dataset to XML  
            StringReader stream = new StringReader(ds.GetXml());
            command.Connection.Close();
            // convert our stream of text to an XmlReader  
            return new XmlTextReader(stream);
        }
        #endregion

        #region AttachParameters(SQLiteCommand,commandText,object[] paramList)  
        /// <summary>  
        /// 增加参数到命令（自动判断类型）  
        /// </summary>  
        /// <param name="commandText">命令语句</param>  
        /// <param name="paramList">object参数列表</param>  
        /// <returns>返回SQLiteParameterCollection参数列表</returns>  
        /// <remarks>Status experimental. Regex appears to be handling most issues. Note that parameter object array must be in same ///order as parameter names appear in SQL statement.</remarks>  
        private static SQLiteParameterCollection AttachParameters(SQLiteCommand cmd, string commandText, params object[] paramList)
        {
            if (paramList == null || paramList.Length == 0) return null;

            SQLiteParameterCollection coll = cmd.Parameters;
            string parmString = commandText.Substring(commandText.IndexOf("@"));
            // pre-process the string so always at least 1 space after a comma.  
            parmString = parmString.Replace(",", " ,");
            // get the named parameters into a match collection  
            string pattern = @"(@)\S*(.*?)\b";
            Regex ex = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection mc = ex.Matches(parmString);
            string[] paramNames = new string[mc.Count];
            int i = 0;
            foreach (Match m in mc)
            {
                paramNames[i] = m.Value;
                i++;
            }

            // now let's type the parameters  
            int j = 0;
            Type t = null;
            foreach (object o in paramList)
            {
                t = o.GetType();

                SQLiteParameter parm = new SQLiteParameter();
                switch (t.ToString())
                {

                    case ("DBNull"):
                    case ("Char"):
                    case ("SByte"):
                    case ("UInt16"):
                    case ("UInt32"):
                    case ("UInt64"):
                        throw new SystemException("Invalid data type");


                    case ("System.String"):
                        parm.DbType = DbType.String;
                        parm.ParameterName = paramNames[j];
                        parm.Value = (string)paramList[j];
                        coll.Add(parm);
                        break;

                    case ("System.Byte[]"):
                        parm.DbType = DbType.Binary;
                        parm.ParameterName = paramNames[j];
                        parm.Value = (byte[])paramList[j];
                        coll.Add(parm);
                        break;

                    case ("System.Int32"):
                        parm.DbType = DbType.Int32;
                        parm.ParameterName = paramNames[j];
                        parm.Value = (int)paramList[j];
                        coll.Add(parm);
                        break;
                    case ("System.Int64"):
                        parm.DbType = DbType.Int64;
                        parm.ParameterName = paramNames[j];
                        parm.Value = Convert.ToInt64(paramList[j]);
                        coll.Add(parm);
                        break;
                    case ("System.Boolean"):
                        parm.DbType = DbType.Boolean;
                        parm.ParameterName = paramNames[j];
                        parm.Value = (bool)paramList[j];
                        coll.Add(parm);
                        break;

                    case ("System.DateTime"):
                        parm.DbType = DbType.DateTime;
                        parm.ParameterName = paramNames[j];
                        parm.Value = Convert.ToDateTime(paramList[j]);
                        coll.Add(parm);
                        break;

                    case ("System.Double"):
                        parm.DbType = DbType.Double;
                        parm.ParameterName = paramNames[j];
                        parm.Value = Convert.ToDouble(paramList[j]);
                        coll.Add(parm);
                        break;

                    case ("System.Decimal"):
                        parm.DbType = DbType.Decimal;
                        parm.ParameterName = paramNames[j];
                        parm.Value = Convert.ToDecimal(paramList[j]);
                        break;

                    case ("System.Guid"):
                        parm.DbType = DbType.Guid;
                        parm.ParameterName = paramNames[j];
                        parm.Value = (System.Guid)(paramList[j]);
                        break;

                    case ("System.Object"):

                        parm.DbType = DbType.Object;
                        parm.ParameterName = paramNames[j];
                        parm.Value = paramList[j];
                        coll.Add(parm);
                        break;

                    default:
                        throw new SystemException("Value is of unknown data type");

                } // end switch  

                j++;
            }
            return coll;
        }
        #endregion

        #region ExecuteNonQueryTypedParams(IDbCommand, DataRow)  
        /// <summary>  
        /// Executes non query typed params from a DataRow  
        /// </summary>  
        /// <param name="command">Command.</param>  
        /// <param name="dataRow">Data row.</param>  
        /// <returns>Integer result code</returns>  
        public static int ExecuteNonQueryTypedParams(IDbCommand command, DataRow dataRow)
        {
            int retVal = 0;

            // If the row has values, the store procedure parameters must be initialized  
            if (dataRow != null && dataRow.ItemArray.Length > 0)
            {
                // Set the parameters values  
                AssignParameterValues(command.Parameters, dataRow);

                retVal = ExecuteNonQuery(command);
            }
            else
            {
                retVal = ExecuteNonQuery(command);
            }

            return retVal;
        }
        #endregion

        #region AssignParameterValues  
        /// <summary>  
        /// This method assigns dataRow column values to an IDataParameterCollection  
        /// </summary>  
        /// <param name="commandParameters">The IDataParameterCollection to be assigned values</param>  
        /// <param name="dataRow">The dataRow used to hold the command's parameter values</param>  
        /// <exception cref="System.InvalidOperationException">Thrown if any of the parameter names are invalid.</exception>  
        protected internal static void AssignParameterValues(IDataParameterCollection commandParameters, DataRow dataRow)
        {
            if (commandParameters == null || dataRow == null)
            {
                // Do nothing if we get no data  
                return;
            }

            DataColumnCollection columns = dataRow.Table.Columns;

            int i = 0;
            // Set the parameters values  
            foreach (IDataParameter commandParameter in commandParameters)
            {
                // Check the parameter name  
                if (commandParameter.ParameterName == null ||
                 commandParameter.ParameterName.Length <= 1)
                    throw new InvalidOperationException(string.Format(
                           "Please provide a valid parameter name on the parameter #{0},                            the ParameterName property has the following value: '{1}'.",
                     i, commandParameter.ParameterName));

                if (columns.Contains(commandParameter.ParameterName))
                    commandParameter.Value = dataRow[commandParameter.ParameterName];
                else if (columns.Contains(commandParameter.ParameterName.Substring(1)))
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];

                i++;
            }
        }
        #endregion

        #region AssignParameterValues  
        /// <summary>  
        /// This method assigns dataRow column values to an array of IDataParameters  
        /// </summary>  
        /// <param name="commandParameters">Array of IDataParameters to be assigned values</param>  
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values</param>  
        /// <exception cref="System.InvalidOperationException">Thrown if any of the parameter names are invalid.</exception>  
        protected void AssignParameterValues(IDataParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters == null) || (dataRow == null))
            {
                // Do nothing if we get no data  
                return;
            }

            DataColumnCollection columns = dataRow.Table.Columns;

            int i = 0;
            // Set the parameters values  
            foreach (IDataParameter commandParameter in commandParameters)
            {
                // Check the parameter name  
                if (commandParameter.ParameterName == null ||
                 commandParameter.ParameterName.Length <= 1)
                    throw new InvalidOperationException(string.Format(
                     "Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.",
                     i, commandParameter.ParameterName));

                if (columns.Contains(commandParameter.ParameterName))
                    commandParameter.Value = dataRow[commandParameter.ParameterName];
                else if (columns.Contains(commandParameter.ParameterName.Substring(1)))
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];

                i++;
            }
        }
        #endregion

        #region AssignParameterValues  
        /// <summary>  
        /// This method assigns an array of values to an array of IDataParameters  
        /// </summary>  
        /// <param name="commandParameters">Array of IDataParameters to be assigned values</param>  
        /// <param name="parameterValues">Array of objects holding the values to be assigned</param>  
        /// <exception cref="System.ArgumentException">Thrown if an incorrect number of parameters are passed.</exception>  
        protected void AssignParameterValues(IDataParameter[] commandParameters, params object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                // Do nothing if we get no data  
                return;
            }

            // We must have the same number of values as we pave parameters to put them in  
            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            // Iterate through the IDataParameters, assigning the values from the corresponding position in the   
            // value array  
            for (int i = 0, j = commandParameters.Length, k = 0; i < j; i++)
            {
                if (commandParameters[i].Direction != ParameterDirection.ReturnValue)
                {
                    // If the current array value derives from IDataParameter, then assign its Value property  
                    if (parameterValues[k] is IDataParameter)
                    {
                        IDataParameter paramInstance;
                        paramInstance = (IDataParameter)parameterValues[k];
                        if (paramInstance.Direction == ParameterDirection.ReturnValue)
                        {
                            paramInstance = (IDataParameter)parameterValues[++k];
                        }
                        if (paramInstance.Value == null)
                        {
                            commandParameters[i].Value = DBNull.Value;
                        }
                        else
                        {
                            commandParameters[i].Value = paramInstance.Value;
                        }
                    }
                    else if (parameterValues[k] == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = parameterValues[k];
                    }
                    k++;
                }
            }
        }
        #endregion


    }
}