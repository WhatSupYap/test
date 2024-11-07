using System;
using System.Collections;
using System.Data;

namespace WebUtil
{
    public interface InterfaceSQL : IDisposable
    {
        bool ConnectionStatus();
        void ConnectionOpen();
        void ConnectionClose();

        void TranBegin();
        void TranCommit();
        void TranRollBack();
        bool TranStatus();

        #region SQL Script 실행 후 DataSet을 반환하는 Select용 메서드

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="commandType">OleDbCommand Type</param>
        void RunScriptDataSet(string sqlScript, DataSet dataSet, CommandType commandtype);

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="commandType">OleDbCommand Type</param>
        void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, CommandType commandtype);

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="commandType">OleDbCommand Type</param>
        /// <param name="output">output 값</param>
        void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, CommandType commandtype, ref Hashtable output);

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="commandType">OleDbCommand Type</param>
        /// <param name="commandTimeout">Sql Query 타임아웃</param>
        void RunScriptDataSet(string sqlScript, DataSet dataSet, int commandTimeout, CommandType commandtype);

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="commandType">OleDbCommand Type</param>
        /// <param name="commandTimeout">Sql Query 타임아웃</param>
        void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, int commandTimeout, CommandType commandtype);

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="commandType">OleDbCommand Type</param>
        /// <param name="commandTimeout">Sql Query 타임아웃</param>
        /// <param name="output">output 값</param>
        void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, int commandTimeout, CommandType commandtype, ref Hashtable output);

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="tablename">DataSet안의 DataTable</param>
        /// <param name="commandType">OleDbCommand Type</param>
        void RunScriptDataSet(string sqlScript, DataSet dataSet, string tablename, CommandType commandtype);

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="tablename">DataSet안의 DataTable</param>
        /// <param name="commandType">OleDbCommand Type</param>
        void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, string tablename, CommandType commandtype);

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="tablename">DataSet안의 DataTable</param>
        /// <param name="commandType">OleDbCommand Type</param>
        /// <param name="output">output 값</param>
        void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, string tablename, CommandType commandtype, ref Hashtable output);

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="tablename">DataSet안의 DataTable</param>
        /// <param name="commandTimeout">Sql Query 타임아웃</param>
        /// <param name="commandType">OleDbCommand Type</param>
        void RunScriptDataSet(string sqlScript, DataSet dataSet, string tablename, int commandTimeout, CommandType commandtype);


        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="tablename">DataSet안의 DataTable</param>
        /// <param name="commandType">OleDbCommand Type</param>
        /// <param name="commandTimeout">Sql Query 타임아웃</param>
        void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, string tablename, int commandTimeout, CommandType commandtype);

        /// <summary>
        /// Select sqlScript를 실행하여 Dataset에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="dataSet">데이터를 담을 DataSet</param>
        /// <param name="tablename">DataSet안의 DataTable</param>
        /// <param name="commandType">OleDbCommand Type</param>
        /// <param name="commandTimeout">Sql Query 타임아웃</param>
        void RunScriptDataSet(string sqlScript, IDataParameter[] parameters, DataSet dataSet, string tablename, int commandTimeout, CommandType commandtype, ref Hashtable output);

        #endregion

        #region 쿼리 실행 후 Reader을 반환하는 Select용 메서드


        /// <summary>
        /// Select sqlScript를 싱행하는 메소드 반드시 reader.Close()를 호출하면 Connection도 닫힘
        /// </summary>
        /// <param name="sqlScript">쿼리</param>
        IDataReader RunScriptDataReader(string sqlScript, CommandType commandtype);


        IDataReader RunScriptDataReader(string sqlScript, int commandTimeout, CommandType commandtype);


        /// <summary>
        /// Select sqlScript를 싱행하는 메소드 반드시 reader.Close()를 호출하면 Connection도 닫힘
        /// </summary>
        /// <param name="sqlScript">쿼리</param>
        /// <param name="parameters">쿼리 파라미터</param>
        IDataReader RunScriptDataReader(string sqlScript, IDataParameter[] parameters, CommandType commandtype);

        /// <summary>
        /// Select sqlScript를 싱행하는 메소드 반드시 reader.Close()를 호출하면 Connection도 닫힘
        /// </summary>
        /// <param name="sqlScript">쿼리</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="commandTimeout">Sql Query 타임아웃</param>
        IDataReader RunScriptDataReader(string sqlScript, IDataParameter[] parameters, int commandTimeout, CommandType commandtype);

        #endregion

        #region 쿼리 실행 후 적용받은 행의 갯수를 반환하는 메서드

        /// <summary>
        /// DELETE/UPDATE/INSERT 쿼리를 실행 후 적용 받은 행의 갯수를 반환하는 메서드
        /// </summary>
        /// <param name="sqlScript">실행할 SQL Script</param>
        /// <param name="commandtype">SQL Script의 CommandType</param>
        /// <param name="rowsAffected">적용받은 행의 수: -1: 오류</param>
        void RunScriptNonQuery(string sqlScript, CommandType commandtype, out int rowsAffected);


        /// <summary>
        /// DELETE/UPDATE/INSERT 쿼리를 실행 후 적용 받은 행의 갯수를 반환하는 메서드
        /// </summary>
        /// <param name="sqlScript">실행할 SQL Script</param>
        /// <param name="parameters">SQL Script parameter</param>
        /// <param name="commandtype">SQL Script의 CommandType</param>
        /// <param name="rowsAffected">적용받은 행의 수: -1: 오류</param>
        void RunScriptNonQuery(string sqlScript, IDataParameter[] parameters, CommandType commandtype, out int rowsAffected);


        /// <summary>
        /// DELETE/UPDATE/INSERT 쿼리를 실행 후 적용 받은 행의 갯수를 반환하는 메서드
        /// </summary>
        /// <param name="sqlScript">실행할 SQL Script</param>
        /// <param name="parameters">SQL Script parameter</param>
        /// <param name="commandtype">SQL Script의 CommandType</param>
        /// <param name="rowsAffected">적용받은 행의 수: -1: 오류</param>
        /// <param name="output">output 값</param>
        void RunScriptNonQuery(string sqlScript, IDataParameter[] parameters, CommandType commandtype, out int rowsAffected, ref Hashtable output);


        /// <summary>
        /// DELETE/UPDATE/INSERT 쿼리를 실행 후 적용 받은 행의 갯수를 반환하는 메서드
        /// </summary>
        /// <param name="sqlScript">실행할 SQL Script</param>
        /// <param name="parameters">SQL Script parameter</param>
        /// <param name="commandtype">SQL Script의 CommandType</param>
        /// <param name="rowsAffected">적용받은 행의 수: -1: 오류</param>
        /// <param name="commandTimeout">Sql Query 타임아웃</param>
        void RunScriptNonQuery(string sqlScript, int commandTimeout, CommandType commandtype, out int rowsAffected);



        /// <summary>
        /// DELETE/UPDATE/INSERT 쿼리를 실행 후 적용 받은 행의 갯수를 반환하는 메서드
        /// </summary>
        /// <param name="sqlScript">실행할 SQL Script</param>
        /// <param name="parameters">SQL Script parameter</param>
        /// <param name="commandtype">SQL Script의 CommandType</param>
        /// <param name="commandTimeout">Sql Query 타임아웃</param>
        /// <param name="rowsAffected">적용받은 행의 수: -1: 오류</param>
        void RunScriptNonQuery(string sqlScript, IDataParameter[] parameters, CommandType commandtype, int commandTimeout, out int rowsAffected);


        /// <summary>
        /// DELETE/UPDATE/INSERT 쿼리를 실행 후 적용 받은 행의 갯수를 반환하는 메서드
        /// </summary>
        /// <param name="sqlScript">실행할 SQL Script</param>
        /// <param name="parameters">SQL Script parameter</param>
        /// <param name="commandtype">SQL Script의 CommandType</param>
        /// <param name="commandTimeout">Sql Query 타임아웃</param>
        /// <param name="rowsAffected">적용받은 행의 수: -1: 오류</param>
        /// <param name="output">output 값</param>
        void RunScriptNonQuery(string sqlScript, IDataParameter[] parameters, CommandType commandtype, int commandTimeout, out int rowsAffected, ref Hashtable output);

        #endregion

        #region 쿼리 실행 후 단일 Scalar 값을 반환 하는 메서드


        /// <summary>
        /// Select sqlScript를 실행하여 object에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="commandTimeout">Sql Query 타임아웃</param>
        /// <param name="commandType">OleDbCommand Type</param>
        /// <param name="scalar">결과를 담을 object</param>
        void RunScriptScalar(string sqlScript, int commandTimeout, CommandType commandtype, out object scalar);


        /// <summary>
        /// Select sqlScript를 실행하여 object에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="commandType">OleDbCommand Type</param>
        /// <param name="scalar">결과를 담을 object</param>
        void RunScriptScalar(string sqlScript, IDataParameter[] parameters, CommandType commandtype, out object scalar);


        /// <summary>
        /// Select sqlScript를 실행하여 object에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="commandType">OleDbCommand Type</param>
        /// <param name="scalar">결과를 담을 object</param>
        void RunScriptScalar(string sqlScript, CommandType commandtype, out object scalar);


        /// <summary>
        /// Select sqlScript를 실행하여 object에 넣어 주는 메소드
        /// </summary>
        /// <param name="sqlScript">실행할 스크립트</param>
        /// <param name="parameters">쿼리 파라미터</param>
        /// <param name="commandTimeout">Sql Query 타임아웃</param>
        /// <param name="commandType">OleDbCommand Type</param>
        /// <param name="scalar">결과를 담을 object</param>
        void RunScriptScalar(string sqlScript, IDataParameter[] parameters, int commandTimeout, CommandType commandtype, out object scalar);

        #endregion
    }
}
