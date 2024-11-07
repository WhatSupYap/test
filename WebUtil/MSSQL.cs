using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace WebUtil
{
    class MSSQL : InterfaceSQL, IDisposable
    {
        private bool disposed = false;
        private bool m_beginTran = false;
        private SqlTransaction tran = null;

        #region 인스턴스 변수
        /// <summary>
        /// 사용할 기본 SqlConnetion
        /// </summary>
        private SqlConnection connection;

        /// <summary>
        /// 조회된 DataSet내의 DataTable명을 지정하지 않으면 들어가는 이름
        /// </summary>
        private readonly string TABLE_NAME = "OUT_PUT";

        /// <summary>
        /// SqlCommand 실행지 TimeOut시간
        /// </summary>
        private readonly int SQL_COMMAND_TIMEOUT = 30;
        #endregion

        #region 생성자 부분
        // 생성자
        public MSSQL()
        {
            connection = new SqlConnection();
        }

        // 생성자
        public MSSQL(string connectionString)
            : this()
        {
            connection.ConnectionString = connectionString;
        }
        #endregion

        #region 속성 부분

        private SqlConnection Connection
        {
            get
            {
                return this.connection;
            }
        }
        #endregion

        #region 트랜젝션
        public void TranBegin()
        {
            try
            {
                if (m_beginTran == false)
                {
                    m_beginTran = true;
                    ConnectionOpen();
                    tran = this.connection.BeginTransaction();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString(), ex.InnerException);
            }
            finally
            {

            }
        }

        public void TranCommit()
        {
            try
            {
                if (m_beginTran == true)
                {
                    m_beginTran = false;
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString(), ex.InnerException);
            }
            finally
            {

            }
        }

        public void TranRollBack()
        {
            try
            {
                if (m_beginTran == true)
                {
                    m_beginTran = false;
                    tran.Rollback();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString(), ex.InnerException);
            }
            finally
            {

            }
        }

        public bool TranStatus()
        {
            return m_beginTran;
        }
        #endregion

        #region 연결 정보 및 연결 관리 부분
        private string ConnectionString
        {
            set
            {
                this.connection.ConnectionString = value;
            }
        }

        public bool ConnectionStatus()
        {
            if (Connection.State == ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ConnectionOpen()
        {
            try
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" Database Access Failed", ex);
            }
        }

        public void ConnectionClose()
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }
        #endregion

        #region SqlCommand 객체 생성하는 부분
        /// <summary>
        /// 실행할 스크립트과 파라미터를 이용하여 SQLCommand 객체를 만들어 반환 한다.
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="sqlCommandTimeout">쿼리 실행 타임아웃</param>
        /// <param name="commandType">SqlCommand Type</param>
        /// <returns>SqlCommand 객체</returns>
        private SqlCommand BuildCommand(string sqlScript, IDataParameter[] parameters, int sqlCommandTimeout, CommandType commandType)
        {
            using (SqlCommand cmd = new SqlCommand(sqlScript, Connection))
            {
                cmd.CommandType = commandType;
                cmd.CommandTimeout = sqlCommandTimeout;

                if (tran != null)
                {
                    cmd.Transaction = tran;
                }

                if (parameters.Length != 0)
                {
                    foreach (SqlParameter parameter in parameters)//paramert<"@parameter name",parameter value>
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }

                return cmd;
            }

        }
        #endregion

        //------------------------------------------------------------------------------------//
        //---------------------------- [SECTION : public  Method] --------------------------//
        //------------------------------------------------------------------------------------//

        #region SQL Script 실행 후 DataSet을 반환하는 Select용 메서드

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="commandtype">SqlCommand Type</param>
        public void RunScriptDataSet(string sqlScript, DataSet dataSet, CommandType commandtype)
        {
            RunScriptDataSet(sqlScript, new IDataParameter[0], dataSet, TABLE_NAME, SQL_COMMAND_TIMEOUT, commandtype);
        }

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="commandType">SqlCommand Type</param>
        public void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, CommandType commandtype)
        {
            RunScriptDataSet(sqlScript, parameters, dataSet, TABLE_NAME, SQL_COMMAND_TIMEOUT, commandtype);
        }

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="commandType">SqlCommand Type</param>
        /// <param name="output">output 값</param>
        public void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, CommandType commandtype, ref Hashtable output)
        {
            RunScriptDataSet(sqlScript, parameters, dataSet, TABLE_NAME, SQL_COMMAND_TIMEOUT, commandtype, ref output);
        }

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="commandType">SqlCommand Type</param>
        /// <param name="sqlCommandTimeout">Sql Query 타임아웃</param>
        public void RunScriptDataSet(string sqlScript, DataSet dataSet, int sqlCommandTimeout, CommandType commandtype)
        {
            RunScriptDataSet(sqlScript, new IDataParameter[0], dataSet, TABLE_NAME, sqlCommandTimeout, commandtype);
        }

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="commandType">SqlCommand Type</param>
        /// <param name="sqlCommandTimeout">Sql Query 타임아웃</param>
        public void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, int sqlCommandTimeout, CommandType commandtype)
        {
            RunScriptDataSet(sqlScript, parameters, dataSet, TABLE_NAME, sqlCommandTimeout, commandtype);
        }

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="commandType">SqlCommand Type</param>
        /// <param name="sqlCommandTimeout">Sql Query 타임아웃</param>
        /// <param name="output">output 값</param>
        public void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, int sqlCommandTimeout, CommandType commandtype, ref Hashtable output)
        {
            RunScriptDataSet(sqlScript, parameters, dataSet, TABLE_NAME, sqlCommandTimeout, commandtype, ref output);
        }

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="tablename">DataSet안의 DataTable</param>
        /// <param name="commandType">SqlCommand Type</param>
        public void RunScriptDataSet(string sqlScript, DataSet dataSet, string tablename, CommandType commandtype)
        {
            RunScriptDataSet(sqlScript, new IDataParameter[0], dataSet, tablename, SQL_COMMAND_TIMEOUT, commandtype);
        }

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="tablename">DataSet안의 DataTable</param>
        /// <param name="commandType">SqlCommand Type</param>
        public void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, string tablename, CommandType commandtype)
        {
            RunScriptDataSet(sqlScript, parameters, dataSet, tablename, SQL_COMMAND_TIMEOUT, commandtype);
        }

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="tablename">DataSet안의 DataTable</param>
        /// <param name="commandType">SqlCommand Type</param>
        /// <param name="output">output 값</param>
        public void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, string tablename, CommandType commandtype, ref Hashtable output)
        {
            RunScriptDataSet(sqlScript, parameters, dataSet, tablename, SQL_COMMAND_TIMEOUT, commandtype, ref output);
        }

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="tablename">DataSet안의 DataTable</param>
        /// <param name="sqlCommandTimeout">Sql Query 타임아웃</param>
        /// <param name="commandType">SqlCommand Type</param>
        public void RunScriptDataSet(string sqlScript, DataSet dataSet, string tablename, int sqlCommandTimeout, CommandType commandtype)
        {
            RunScriptDataSet(sqlScript, new IDataParameter[0], dataSet, tablename, sqlCommandTimeout, commandtype);
        }

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="tablename">DataSet안의 DataTable</param>
        /// <param name="commandType">SqlCommand Type</param>
        /// <param name="sqlCommandTimeout">Sql Query 타임아웃</param>
        public void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, string tablename, int sqlCommandTimeout, CommandType commandtype)
        {
            try
            {
                using (SqlDataAdapter SqlDA = new SqlDataAdapter())
                {
                    SqlDA.SelectCommand = BuildCommand(sqlScript, parameters, sqlCommandTimeout, commandtype);

                    SqlDA.Fill(dataSet, tablename);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString(), ex.InnerException);
            }
            finally
            {
            }
        }

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="tablename">DataSet안의 DataTable</param>
        /// <param name="commandType">SqlCommand Type</param>
        /// <param name="sqlCommandTimeout">Sql Query 타임아웃</param>
        /// <param name="output">output 값</param>
        public void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, string tablename, int sqlCommandTimeout, CommandType commandtype, ref Hashtable output)
        {
            try
            {
                using (SqlDataAdapter SqlDA = new SqlDataAdapter())
                {
                    SqlDA.SelectCommand = BuildCommand(sqlScript, parameters, sqlCommandTimeout, commandtype);

                    SqlDA.Fill(dataSet, tablename);

                    if (parameters.Length != 0)
                    {
                        int count = 0;
                        foreach (SqlParameter parameter in parameters)
                        {
                            if (parameter.Direction == ParameterDirection.Output && parameter.ParameterName == SqlDA.SelectCommand.Parameters[count].ParameterName)
                            {
                                output.Add(SqlDA.SelectCommand.Parameters[count].ParameterName, SqlDA.SelectCommand.Parameters[count].Value);
                            }
                            count++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString(), ex.InnerException);
            }
            finally
            {
            }
        }

        #endregion


        #region 쿼리 실행 후 Reader을 반환하는 Select용 메서드


        /// <summary>
        /// Select sqlScript를 싱행하는 메소드 반드시 reader.Close()를 호출하면 Connection도 닫힘
        /// </summary>
        /// <param name="sqlScript">쿼리</param>
        /// <param name="commandtype">CommandType</param>
        public IDataReader RunScriptDataReader(string sqlScript, CommandType commandtype)
        {
            return RunScriptDataReader(sqlScript, new IDataParameter[0], SQL_COMMAND_TIMEOUT, commandtype);
        }

        /// <summary>
        /// Select sqlScript를 싱행하는 메소드 반드시 reader.Close()를 호출하면 Connection도 닫힘
        /// </summary>
        /// <param name="sqlScript">쿼리</param>
        /// <param name="commandTimeout">쿼리 실행 타임아웃</param>
        /// <param name="commandtype">CommandType</param>
        /// <returns></returns>
        public IDataReader RunScriptDataReader(string sqlScript, int commandTimeout, CommandType commandtype)
        {
            return RunScriptDataReader(sqlScript, new IDataParameter[0], commandTimeout, commandtype);
        }



        /// <summary>
        /// Select sqlScript를 싱행하는 메소드 반드시 reader.Close()를 호출하면 Connection도 닫힘
        /// </summary>
        /// <param name="sqlScript">쿼리</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="commandtype">CommandType</param>
        public IDataReader RunScriptDataReader(string sqlScript, IDataParameter[] parameters, CommandType commandtype)
        {
            return RunScriptDataReader(sqlScript, parameters, SQL_COMMAND_TIMEOUT, commandtype);
        }

        /// <summary>
        /// Select sqlScript를 싱행하는 메소드 반드시 reader.Close()를 호출하면 Connection도 닫힘
        /// </summary>
        /// <param name="sqlScript">쿼리</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="commandtype">CommandType</param>
        public IDataReader RunScriptDataReader(string sqlScript, IDataParameter[] parameters, int sqlCommandTimeout, CommandType commandtype)
        {
            try
            {
                SqlDataReader reader;

                SqlCommand command = BuildCommand(sqlScript, parameters, sqlCommandTimeout, commandtype);

                if (tran != null)
                {
                    command.Transaction = tran;
                }

                reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                return reader;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString(), ex.InnerException);
            }
            finally
            {
            }
        }
        #endregion


        #region 쿼리 실행 후 적용받은 행의 갯수를 반환하는 메서드
        /// <summary>
        /// DELETE/UPDATE/INSERT 쿼리를 실행 후 적용 받은 행의 갯수를 반환하는 메서드
        /// </summary>
        /// <param name="sqlScript">실행할 SQL Script</param>
        /// <param name="commandtype">SQL Script의 CommandType</param>
        /// <param name="rowsAffected">적용받은 행의 수: -1: 오류</param>
        public void RunScriptNonQuery(string sqlScript, CommandType commandtype, out int rowsAffected)
        {
            RunScriptNonQuery(sqlScript, new IDataParameter[0], commandtype, SQL_COMMAND_TIMEOUT, out rowsAffected);
        }

        /// <summary>
        /// DELETE/UPDATE/INSERT 쿼리를 실행 후 적용 받은 행의 갯수를 반환하는 메서드
        /// </summary>
        /// <param name="sqlScript">실행할 SQL Script</param>
        /// <param name="parameters">SQL Script parameter</param>
        /// <param name="commandtype">SQL Script의 CommandType</param>
        /// <param name="rowsAffected">적용받은 행의 수: -1: 오류</param>
        public void RunScriptNonQuery(string sqlScript, IDataParameter[] parameters, CommandType commandtype, out int rowsAffected)
        {
            RunScriptNonQuery(sqlScript, parameters, commandtype, SQL_COMMAND_TIMEOUT, out rowsAffected);
        }

        /// <summary>
        /// DELETE/UPDATE/INSERT 쿼리를 실행 후 적용 받은 행의 갯수를 반환하는 메서드
        /// </summary>
        /// <param name="sqlScript">실행할 SQL Script</param>
        /// <param name="parameters">SQL Script parameter</param>
        /// <param name="commandtype">SQL Script의 CommandType</param>
        /// <param name="rowsAffected">적용받은 행의 수: -1: 오류</param>
        /// <param name="output">output 값</param>
        public void RunScriptNonQuery(string sqlScript, IDataParameter[] parameters, CommandType commandtype, out int rowsAffected, ref Hashtable output)
        {
            RunScriptNonQuery(sqlScript, parameters, commandtype, SQL_COMMAND_TIMEOUT, out rowsAffected, ref output);
        }

        /// <summary>
        /// DELETE/UPDATE/INSERT 쿼리를 실행 후 적용 받은 행의 갯수를 반환하는 메서드
        /// </summary>
        /// <param name="sqlScript">실행할 SQL Script</param>
        /// <param name="parameters">SQL Script parameter</param>
        /// <param name="commandtype">SQL Script의 CommandType</param>
        /// <param name="rowsAffected">적용받은 행의 수: -1: 오류</param>
        /// <param name="sqlCommandTimeout">Sql Query 타임아웃</param>
        public void RunScriptNonQuery(string sqlScript, int sqlCommandTimeout, CommandType commandtype, out int rowsAffected)
        {
            RunScriptNonQuery(sqlScript, new IDataParameter[0], commandtype, sqlCommandTimeout, out rowsAffected);
        }

        /// <summary>
        /// DELETE/UPDATE/INSERT 쿼리를 실행 후 적용 받은 행의 갯수를 반환하는 메서드
        /// </summary>
        /// <param name="sqlScript">실행할 SQL Script</param>
        /// <param name="parameters">SQL Script parameter</param>
        /// <param name="commandtype">SQL Script의 CommandType</param>
        /// <param name="sqlCommandTimeout">Sql Query 타임아웃</param>
        /// <param name="rowsAffected">적용받은 행의 수: -1: 오류</param>
        public void RunScriptNonQuery(string sqlScript, IDataParameter[] parameters, CommandType commandtype, int sqlCommandTimeout, out int rowsAffected)
        {
            try
            {
                SqlCommand command = BuildCommand(sqlScript, parameters, sqlCommandTimeout, commandtype);

                command.CommandType = commandtype;

                if (tran != null)
                {
                    command.Transaction = tran;
                }

                rowsAffected = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                rowsAffected = -1;
                throw new Exception(ex.Message.ToString(), ex.InnerException);
            }
            finally
            {
            }
        }

        /// <summary>
        /// DELETE/UPDATE/INSERT 쿼리를 실행 후 적용 받은 행의 갯수를 반환하는 메서드
        /// </summary>
        /// <param name="sqlScript">실행할 SQL Script</param>
        /// <param name="parameters">SQL Script parameter</param>
        /// <param name="commandtype">SQL Script의 CommandType</param>
        /// <param name="sqlCommandTimeout">Sql Query 타임아웃</param>
        /// <param name="rowsAffected">적용받은 행의 수: -1: 오류</param>
        /// <param name="output">output 값</param>
        public void RunScriptNonQuery(string sqlScript, IDataParameter[] parameters, CommandType commandtype, int sqlCommandTimeout, out int rowsAffected, ref Hashtable output)
        {
            try
            {
                SqlCommand command = BuildCommand(sqlScript, parameters, sqlCommandTimeout, commandtype);

                command.CommandType = commandtype;

                if (tran != null)
                {
                    command.Transaction = tran;
                }

                rowsAffected = command.ExecuteNonQuery();

                if (output.Count > 0)
                {
                    Hashtable tmpoutput = new Hashtable();
                    foreach (DictionaryEntry o in output)
                    {
                        if (command.Parameters.Contains("@" + o.Key.ToString()))
                        {
                            tmpoutput.Add(o.Key.ToString(), command.Parameters["@" + o.Key.ToString()].Value);
                        }
                    }
                    output = tmpoutput;
                }
                else if (parameters.Length != 0)
                {
                    int count = 0;
                    foreach (SqlParameter parameter in parameters)
                    {
                        if (parameter.Direction == ParameterDirection.Output && parameter.ParameterName == command.Parameters[count].ParameterName)
                        {
                            output.Add(command.Parameters[count].ParameterName, command.Parameters[count].Value);
                        }
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                rowsAffected = -1;
                throw new Exception(ex.Message.ToString(), ex.InnerException);
            }
            finally
            {
            }
        }
        #endregion


        #region 쿼리 실행 후 단일 Scalar 값을 반환 하는 메서드
        /// <summary>
        /// Select sqlScript를 실행하여 object에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="sqlCommandTimeout">Sql Query 타임아웃</param>
        /// <param name="commandType">SqlCommand Type</param>
        /// <param name="scalar">결과를 담을 object</param>
        public void RunScriptScalar(string sqlScript, int sqlCommandTimeout, CommandType commandtype, out object scalar)
        {
            RunScriptScalar(sqlScript, new IDataParameter[0], sqlCommandTimeout, commandtype, out scalar);
        }

        /// <summary>
        /// Select sqlScript를 실행하여 object에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="commandType">SqlCommand Type</param>
        /// <param name="scalar">결과를 담을 object</param>
        public void RunScriptScalar(string sqlScript, IDataParameter[] parameters, CommandType commandtype, out object scalar)
        {
            RunScriptScalar(sqlScript, parameters, SQL_COMMAND_TIMEOUT, commandtype, out scalar);

        }

        /// <summary>
        /// Select sqlScript를 실행하여 object에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="commandType">SqlCommand Type</param>
        /// <param name="scalar">결과를 담을 object</param>
        public void RunScriptScalar(string sqlScript, CommandType commandtype, out object scalar)
        {
            RunScriptScalar(sqlScript, new IDataParameter[0], SQL_COMMAND_TIMEOUT, commandtype, out scalar);
        }

        /// <summary>
        /// Select sqlScript를 실행하여 object에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="sqlCommandTimeout">Sql Query 타임아웃</param>
        /// <param name="commandType">SqlCommand Type</param>
        /// <param name="scalar">결과를 담을 object</param>
        public void RunScriptScalar(string sqlScript, IDataParameter[] parameters, int sqlCommandTimeout, CommandType commandtype, out object scalar)
        {
            try
            {
                SqlCommand command = BuildCommand(sqlScript, parameters, sqlCommandTimeout, commandtype);

                command.CommandType = commandtype;

                if (tran != null)
                {
                    command.Transaction = tran;
                }

                scalar = command.ExecuteScalar();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString(), ex.InnerException);
            }
            finally
            {
            }
        }
        #endregion

        #region Make Param

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);

        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    connection.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.

                // Note disposing has been done.
                disposed = true;

            }

        }

        #endregion
    }
}
