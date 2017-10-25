using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Text.RegularExpressions;

namespace wpfdemo
{
    /// <summary>
    /// MySqlHelper操作类
    /// </summary>
    public sealed partial class SQLHelper
    {
        //数据库连接字符串
        public static string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.ToString();
        #region ExecuteNonQuery
        //执行SQL语句，返回影响的记录数
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteNonQuery(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteNonQuery(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (MySqlException e)
                    {
                        throw e;
                    }
                }
            }
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object ExecuteScalar(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object ExecuteScalar(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (MySqlException e)
                    {
                        throw e;
                    }
                }
            }
        }
        #endregion

        #region ExecuteReader
        /// <summary>
        /// 执行查询语句，返回MySqlDataReader (注意：调用该方法后，一定要对MySqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(string strSQL)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(strSQL, connection);
            MySqlDataReader myReader = null;
            try
            {
                connection.Open();
                myReader = cmd.ExecuteReader();

                return myReader;
            }
            catch (MySqlException e)
            {
                throw e;
            }
            finally
            {
                myReader.Close();
            }
        }
        /// <summary>
        /// 执行查询语句，返回MySqlDataReader ( 注意：调用该方法后，一定要对MySqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(string SQLString, params MySqlParameter[] cmdParms)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();
            MySqlDataReader myReader = null;
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                myReader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (MySqlException e)
            {
                throw e;
            }
            finally
            {
                myReader.Close();
                cmd.Dispose();
                connection.Close();

            }
        }
        #endregion

        #region ExecuteDataTable
        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteDataTable(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds.Tables[0];
            }
        }
        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteDataTable(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (MySqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds.Tables[0];
                }
            }
        }
        //获取起始页码和结束页码
        public static DataTable ExecuteDataTable(string cmdText, int startResord, int maxRecord)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(cmdText, connection);
                    command.Fill(ds, startResord, maxRecord, "ds");
                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds.Tables[0];
            }
        }
        #endregion

        #region PageList Without Proc
        /// <summary>
        /// 获取分页数据 在不用存储过程情况下
        /// </summary>
        /// <param name="recordCount">总记录条数</param>
        /// <param name="selectList">选择的列逗号隔开,支持top num</param>
        /// <param name="tableName">表名字</param>
        /// <param name="whereStr">条件字符 必须前加 and</param>
        /// <param name="orderExpression">排序 例如 ID</param>
        /// <param name="pageIdex">当前索引页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        public static DataTable getPager(out int recordCount, string selectList, string tableName, string whereStr, string orderExpression, int pageIdex, int pageSize)
        {
            int rows = 0;
            DataTable dt = new DataTable();
            MatchCollection matchs = Regex.Matches(selectList, @"top\s+\d{1,}", RegexOptions.IgnoreCase);//含有top
            string sqlStr = sqlStr = string.Format("select {0} from {1} where 1=1 {2}", selectList, tableName, whereStr);
            if (!string.IsNullOrEmpty(orderExpression)) { sqlStr += string.Format(" Order by {0}", orderExpression); }
            if (matchs.Count > 0) //含有top的时候
            {
                DataTable dtTemp = ExecuteDataTable(sqlStr);
                rows = dtTemp.Rows.Count;
            }
            else //不含有top的时候
            {
                string sqlCount = string.Format("select count(*) from {0} where 1=1 {1} ", tableName, whereStr);
                //获取行数
                object obj = ExecuteScalar(sqlCount);
                if (obj != null)
                {
                    rows = Convert.ToInt32(obj);
                }
            }
            dt = ExecuteDataTable(sqlStr, (pageIdex - 1) * pageSize, pageSize);
            recordCount = rows;
            return dt;
        }
        #endregion

        #region 创建command
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (MySqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                    (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        ///大批量数据插入,返回成功插入行数  
        /// </summary>  
        /// <param name="connectionString">数据库连接字符串</param>  
        /// <param name="table">数据表</param>  
        /// <returns>返回成功插入行数</returns>  
        public static int BulkInsert(DataTable table)
        {
            if (string.IsNullOrEmpty(table.TableName)) throw new Exception("请给DataTable的TableName属性附上表名称");
            if (table.Rows.Count == 0) return 0;
            int insertCount = 0;
            string tmpPath = Path.GetTempFileName();
            string csv = DataTableToCsv(table);
            File.WriteAllText(tmpPath, csv);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {

                try
                {

                    conn.Open();
                    //tran = conn.BeginTransaction();  
                    MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
                    {
                        FieldTerminator = ",",
                        FieldQuotationCharacter = '"',
                        EscapeCharacter = '"',
                        LineTerminator = "\r\n",
                        FileName = tmpPath,
                        NumberOfLinesToSkip = 0,
                        TableName = table.TableName,
                    };
                    //bulk.Columns.AddRange(table.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToArray());  
                    insertCount = bulk.Load();
                    // tran.Commit();  
                }
                catch (MySqlException ex)
                {
                    // if (tran != null) tran.Rollback();  
                    throw ex;
                }
            }
            File.Delete(tmpPath);
            return insertCount;
        }

        ///将DataTable转换为标准的CSV  
        /// </summary>  
        /// <param name="table">数据表</param>  
        /// <returns>返回标准的CSV</returns>  
        private static string DataTableToCsv(DataTable table)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。  
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。  
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。  
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }


            return sb.ToString();
        }
        #endregion
    }
}
