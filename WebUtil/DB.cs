using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace WebUtil
{
    /// <summary>
    /// 
    /// </summary>
    public class DB : IDisposable
    {
        private ParamsClass m_Params = new ParamsClass();
        private bool disposed = false;
        private InterfaceSQL sqlHandler = null;
        private bool m_error = false;
        private bool m_auto_transaction = true;
        private Stopwatch sw;

        /// <summary>
        /// 생성자
        /// </summary>
        public DB()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public DB(string connectionString)
        {
            // 다른 DB일 경우 생성후 여기서 처리한다.
            sqlHandler = new MSSQL(connectionString);
        }

        private void Connect()
        {
            if (sqlHandler.ConnectionStatus() == false)
            {
                sqlHandler.ConnectionOpen();
            }
        }

        private void Close()
        {
            if (sqlHandler != null && sqlHandler.ConnectionStatus() == true)
            {
                sqlHandler.ConnectionClose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Tran()
        {
            if (sqlHandler.TranStatus() == false)
            {
                sqlHandler.TranBegin();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Commit()
        {
            if (sqlHandler.TranStatus() == true)
            {
                sqlHandler.TranCommit();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RollBack()
        {
            if (sqlHandler.TranStatus() == true)
            {
                sqlHandler.TranRollBack();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void TranStart()
        {
            this.m_auto_transaction = false;
            Connect();
            Tran();
        }

        public void TranEnd()
        {
            if (this.m_auto_transaction == false)
            {
                try
                {
                    if (m_error)
                    {
                        RollBack();
                    }
                    else
                    {
                        Commit();
                    }
                }
                catch
                {
                }

                this.m_auto_transaction = true;
            }
            Close();
        }

        #region --------------------------Property-----------------------------
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString
        {
            set
            {
                if (sqlHandler != null)
                    sqlHandler.Dispose();

                // 다른 DB일 경우 생성후 여기서 처리한다.
                sqlHandler = new MSSQL(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDataParameter[] Params
        {
            get
            {
                return m_Params.param;
            }
        }
        #endregion

        #region --------------------------Parameters--------------------------
        /// <summary>
        /// 
        /// </summary>
        private class ParamsClass
        {
            private List<IDataParameter> rtnParameters = new List<IDataParameter>();
            public List<IDataParameter> items
            {
                get
                {
                    return rtnParameters;
                }
            }

            public IDataParameter[] param
            {
                get
                {
                    return rtnParameters.ToArray<IDataParameter>();
                }
            }
        }

        /// <summary>
        /// paramInfo : new {Name="", Value="", Out=false, SqlDbType=SqlDbType, Size=0}
        /// <para>Name : Param 명(string)</para>
        /// <para>Value : 입력 값(object)</para>
        /// <para>Out : 입출력 구분(bool)</para>
        /// <para>SqlDbType : 형식(SqlDbType)</para>
        /// <para>Size : 크기(int)</para>
        /// </summary>
        public void AddParam(object paramInfo)
        {
            SqlParameter sqlParam = null;

            var infosArr = new RouteValueDictionary(paramInfo);
            string Name = "";

            if (infosArr.ContainsKey("Name") == false)
                return;
            else
                Name = infosArr["Name"].ToString();

            // 객체생성
            sqlParam = new SqlParameter();
            sqlParam.ParameterName = Name;

            if (infosArr.ContainsKey("Value") == true)
                sqlParam.Value = infosArr["Value"];

            if (infosArr.ContainsKey("Out") == true && ((bool)infosArr["Out"]) == true)
                sqlParam.Direction = ParameterDirection.Output;

            if (infosArr.ContainsKey("SqlDbType") == true)
                sqlParam.SqlDbType = (SqlDbType)infosArr["SqlDbType"];

            if (infosArr.ContainsKey("Size") == true)
                sqlParam.Size = (int)infosArr["Size"];

            m_Params.items.Add(sqlParam);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearParam()
        {
            m_Params.items.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public string ParamsInto(IDataParameter[] parameters)
        {
            StringBuilder sParam = new StringBuilder();

            if (parameters.Length != 0)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    sParam.Append(parameter.ParameterName.ToString() + " (" + parameter.SqlDbType.ToString() + ")" + " = " + parameter.Value.ToString() + "\n");
                }
            }

            return sParam.ToString();
        }
        #endregion

        #region StoredProcedure / Text 값 리턴
        public CommandType GetSQLCmdType(string strQuery)
        {
            try
            {
                if (strQuery.Contains(" ") == true || strQuery.Contains("\n") == true)
                {
                    return CommandType.Text;
                }

                return CommandType.StoredProcedure;
            }
            catch
            {
                return CommandType.Text;
            }
        }
        #endregion

        #region SQL Script 실행 후 DataSet을 반환하는 Select용 메서드
        /// <summary>
        /// Select sqlScript를 실행하여 Dataset 리턴
        /// </summary>
        /// <param name="쿼리문"></param>
        /// <param name="실행시간"></param>
        /// <returns>조회된 DataSet</returns>
        public DataSet Execute(string strQuery, int commandTimeout = 600, bool logging = true)
        {
            try
            {
                Connect();

                using (DataSet ds = new DataSet())
                {
                    sw = new Stopwatch();
                    sw.Start();

                    CommandType cmdType = GetSQLCmdType(strQuery);
                    sqlHandler.RunScriptDataSet(strQuery, ds, commandTimeout, cmdType);

                    if (logging)
                    {
                        WriteDBLog(strQuery, sw.ElapsedMilliseconds.ToString() + "ms");
                    }

                    Close();
                    sw.Stop();

                    return ds;
                }
            }
            catch (Exception ex)
            {
                if (sw != null && sw.IsRunning == true)
                {
                    sw.Stop();
                }

                m_error = true;
                Close();
                WriteDBErrorLog(ex.Message.ToString());
                throw new Exception(ex.Message, ex);
            }
        }

        public DataSet Execute(string strQuery, object parameter, int commandTimeout = 600, bool logging = true)
        {
            try
            {
                Connect();

                CommandType cmdType = GetSQLCmdType(strQuery);

                DataSet sp_param_ds = new DataSet();
                if (cmdType == CommandType.StoredProcedure)
                {
                    sqlHandler.RunScriptDataSet("sp_help " + strQuery, sp_param_ds, CommandType.Text);
                }

                if (parameter.GetType().ToString() == "System.Data.IDataParameter[]")
                {
                    using (DataSet ds = new DataSet())
                    {
                        sw = new Stopwatch();
                        sw.Start();

                        sqlHandler.RunScriptDataSet(strQuery, (IDataParameter[])parameter, ds, commandTimeout, cmdType);

                        if (logging)
                        {
                            WriteDBLog(strQuery, sw.ElapsedMilliseconds.ToString() + "ms", parameter);
                        }

                        Close();
                        sw.Stop();

                        return ds;
                    }
                }
                else
                {
                    string param = Newtonsoft.Json.JsonConvert.SerializeObject(
                        new
                        {
                            param = parameter
                        });

                    m_Params.items.Clear();

                    dynamic dParam = JsonConvert.DeserializeObject<dynamic>(param);

                    if (dParam != null)
                    {
                        foreach (dynamic p in dParam["param"])
                        {
                            var jObj = new RouteValueDictionary(p);

                            if (cmdType == CommandType.StoredProcedure)
                            {
                                if (sp_param_ds.Tables.Count >= 2)
                                {
                                    if (sp_param_ds.Tables[1].Select("Parameter_name = '@" + jObj["Name"].ToString() + "'").Length == 0)
                                    {
                                        continue;
                                    }
                                }
                            }

                            if ((jObj["Name"].ToString() == "util" && jObj["Value"].GetType().Name == "JObject") || jObj["Value"].GetType().Name == "JArray")
                            {
                                continue;
                            }
                            if (jObj["Value"].ToString() != "" && (((Newtonsoft.Json.Linq.JValue)jObj["Value"]).Value.GetType().Name == "Int32" || ((Newtonsoft.Json.Linq.JValue)jObj["Value"]).Value.GetType().Name == "Int64"))
                            {
                                AddParam(new { @Name = "@" + jObj["Name"].ToString(), @Value = jObj["Value"].ToString(), @SqlDbType = SqlDbType.Int });
                            }
                            else
                            {
                                while (strQuery.Contains("{" + jObj["Name"].ToString() + "}"))
                                {
                                    strQuery = strQuery.Replace("{" + jObj["Name"].ToString() + "}", jObj["Value"].ToString());
                                }

                                AddParam(new { @Name = "@" + jObj["Name"].ToString(), @Value = jObj["Value"].ToString() });
                            }
                        }
                    }

                    using (DataSet ds = new DataSet())
                    {
                        sw = new Stopwatch();
                        sw.Start();

                        sqlHandler.RunScriptDataSet(strQuery, m_Params.param, ds, commandTimeout, cmdType);

                        if (logging)
                        {
                            WriteDBLog(strQuery, sw.ElapsedMilliseconds.ToString() + "ms", parameter);
                        }

                        Close();
                        sw.Stop();

                        return ds;
                    }
                }
            }
            catch (Exception ex)
            {
                if (sw != null && sw.IsRunning == true)
                {
                    sw.Stop();
                }

                m_error = true;
                Close();
                WriteDBErrorLog(ex.Message.ToString());
                throw new Exception(ex.Message, ex);
            }
        }

        public DataSet Execute(string strQuery, object parameter, ref Hashtable output, int commandTimeout = 600, bool logging = true)
        {
            try
            {
                Connect();

                CommandType cmdType = GetSQLCmdType(strQuery);

                DataSet sp_param_ds = new DataSet();
                if (cmdType == CommandType.StoredProcedure)
                {
                    sqlHandler.RunScriptDataSet("sp_help " + strQuery, sp_param_ds, CommandType.Text);
                }

                if (parameter.GetType().ToString() == "System.Data.IDataParameter[]")
                {
                    using (DataSet ds = new DataSet())
                    {
                        sw = new Stopwatch();
                        sw.Start();

                        sqlHandler.RunScriptDataSet(strQuery, (IDataParameter[])parameter, ds, commandTimeout, cmdType, ref output);

                        if (logging)
                        {
							WriteDBLog(strQuery, sw.ElapsedMilliseconds.ToString() + "ms", parameter);
						}

                        Close();
                        sw.Stop();

                        return ds;
                    }
                }
                else
                {
                    string param = Newtonsoft.Json.JsonConvert.SerializeObject(
                        new
                        {
                            param = parameter
                        });

                    m_Params.items.Clear();

                    dynamic dParam = JsonConvert.DeserializeObject<dynamic>(param);

                    if (dParam != null)
                    {
                        foreach (dynamic p in dParam["param"])
                        {
                            var jObj = new RouteValueDictionary(p);

                            if (cmdType == CommandType.StoredProcedure)
                            {
                                if (sp_param_ds.Tables.Count >= 2)
                                {
                                    if (sp_param_ds.Tables[1].Select("Parameter_name = '@" + jObj["Name"].ToString() + "'").Length == 0)
                                    {
                                        continue;
                                    }
                                }
                            }

                            if ((jObj["Name"].ToString() == "util" && jObj["Value"].GetType().Name == "JObject") || jObj["Value"].GetType().Name == "JArray")
                            {
                                continue;
                            }
                            if (jObj["Value"].ToString() != "" && (((Newtonsoft.Json.Linq.JValue)jObj["Value"]).Value.GetType().Name == "Int32" || ((Newtonsoft.Json.Linq.JValue)jObj["Value"]).Value.GetType().Name == "Int64"))
                            {
                                AddParam(new { @Name = "@" + jObj["Name"].ToString(), @Value = jObj["Value"].ToString(), @SqlDbType = SqlDbType.Int });
                            }
                            else
                            {
                                while (strQuery.Contains("{" + jObj["Name"].ToString() + "}"))
                                {
                                    strQuery = strQuery.Replace("{" + jObj["Name"].ToString() + "}", jObj["Value"].ToString());
                                }

                                AddParam(new { @Name = "@" + jObj["Name"].ToString(), @Value = jObj["Value"].ToString() });
                            }
                        }
                    }

                    using (DataSet ds = new DataSet())
                    {
                        sw = new Stopwatch();
                        sw.Start();

                        sqlHandler.RunScriptDataSet(strQuery, m_Params.param, ds, commandTimeout, cmdType, ref output);

                        if (logging)
                        {
							WriteDBLog(strQuery, sw.ElapsedMilliseconds.ToString() + "ms", parameter);
						}

                        Close();
                        sw.Stop();

                        return ds;
                    }
                }
            }
            catch (Exception ex)
            {
                if (sw != null && sw.IsRunning == true)
                {
                    sw.Stop();
                }

                m_error = true;
                Close();
                WriteDBErrorLog(ex.Message.ToString());
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region 반환하는 데이타가 없는 Execute용 메서드
        /// <summary>
        /// 쿼리 실행
        /// </summary>
        /// <param name="쿼리문"></param>
        /// <returns>적용 수</returns>
        public int ExecuteNonQuery(string strQuery, int commandTimeout = 600, bool logging = true)
        {
            try
            {
                if (m_auto_transaction)
                {
                    Tran();
                }
                else
                {
                    Connect();
                }

                CommandType cmdType = GetSQLCmdType(strQuery);
                int affecterow = 0;

                sw = new Stopwatch();
                sw.Start();

                sqlHandler.RunScriptNonQuery(strQuery, commandTimeout, cmdType, out affecterow);

                if (logging)
                {
					WriteDBLog(strQuery, sw.ElapsedMilliseconds.ToString() + "ms");
				}

                if (m_auto_transaction)
                {
                    Commit();
                }
                else
                {
                    if (sqlHandler.TranStatus() == false)
                    {
                        Close();
                    }
                }

                sw.Stop();

                return affecterow;
            }
            catch (Exception ex)
            {
                if (sw != null && sw.IsRunning == true)
                {
                    sw.Stop();
                }

                m_error = true;

                if (m_auto_transaction)
                {
                    RollBack();
                }
                else
                {
                    TranEnd();
                }

                WriteDBErrorLog(ex.Message.ToString());

                throw new Exception(ex.Message, ex);
            }
        }


        /// <summary>
        /// 다건 처리용 트랜잭션 X, 로그 X
        /// </summary>
        /// <param name="strQuery"></param>
        /// <param name="parameter"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="logging"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int ExecuteNonQueryForMulti(string strQuery, SqlParameter[] spa, int commandTimeout = 600, bool logging = true)
        {
            try
            {
                Connect();

                CommandType cmdType = GetSQLCmdType(strQuery);

                DataSet sp_param_ds = new DataSet();
                if (cmdType == CommandType.StoredProcedure)
                {
                    sqlHandler.RunScriptDataSet("sp_help " + strQuery, sp_param_ds, CommandType.Text);
                }

                int affecterow = 0;

                //sw = new Stopwatch();
                //sw.Start();


                m_Params.items.Clear();

                foreach (SqlParameter sp in spa)
                {
                    m_Params.items.Add(sp);
                }

                sqlHandler.RunScriptNonQuery(strQuery, m_Params.param, cmdType, commandTimeout, out affecterow);

                Close();

                //sw.Stop();

                return affecterow;
            }
            catch (Exception ex)
            {
                if (sw != null && sw.IsRunning == true)
                {
                    sw.Stop();
                }

                m_error = true;

                //if (m_auto_transaction)
                //{
                //    RollBack();
                //}
                //else
                //{
                //    TranEnd();
                //}

                Close();

                WriteDBErrorLog(ex.Message.ToString());

                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 쿼리 실행
        /// </summary>
        /// <param name="쿼리문"></param>
        /// <param name="파라메타"></param>
        /// <returns>적용 수</returns>
        public int ExecuteNonQuery(string strQuery, object parameter, int commandTimeout = 600, bool logging = true)
        {
            try
            {
                if (m_auto_transaction)
                {
                    Tran();
                }
                else
                {
                    Connect();
                }

                CommandType cmdType = GetSQLCmdType(strQuery);

                DataSet sp_param_ds = new DataSet();
                if (cmdType == CommandType.StoredProcedure)
                {
                    sqlHandler.RunScriptDataSet("sp_help " + strQuery, sp_param_ds, CommandType.Text);
                }

                int affecterow = 0;

                sw = new Stopwatch();
                sw.Start();

                if (parameter.GetType().ToString().Equals("System.Data.IDataParameter[]") ||
                    parameter.GetType().ToString().Equals("System.Data.SqlClient.SqlParameter[]")
                    )
                {
                    sqlHandler.RunScriptNonQuery(strQuery, (IDataParameter[])parameter, cmdType, commandTimeout, out affecterow);

                    if (logging)
                    {
                        WriteDBLog(strQuery, sw.ElapsedMilliseconds.ToString() + "ms", parameter);
                    }
                }
                else
                {
                    string param = Newtonsoft.Json.JsonConvert.SerializeObject(
                        new
                        {
                            param = parameter
                        });

                    m_Params.items.Clear();

                    dynamic dParam = JsonConvert.DeserializeObject<dynamic>(param);

                    if (dParam != null)
                    {
                        foreach (dynamic p in dParam["param"])
                        {
                            var jObj = new RouteValueDictionary(p);

                            if (cmdType == CommandType.StoredProcedure)
                            {
                                if (sp_param_ds.Tables.Count >= 2)
                                {
                                    if (sp_param_ds.Tables[1].Select("Parameter_name = '@" + jObj["Name"].ToString() + "'").Length == 0)
                                    {
                                        continue;
                                    }
                                }
                            }

                            if ((jObj["Name"].ToString() == "util" && jObj["Value"].GetType().Name == "JObject") || jObj["Value"].GetType().Name == "JArray")
                            {
                                continue;
                            }
                            if (jObj["Value"].ToString() != "" && (((Newtonsoft.Json.Linq.JValue)jObj["Value"]).Value.GetType().Name == "Int32" || ((Newtonsoft.Json.Linq.JValue)jObj["Value"]).Value.GetType().Name == "Int64"))
                            {
                                AddParam(new { @Name = "@" + jObj["Name"].ToString(), @Value = jObj["Value"].ToString(), @SqlDbType = SqlDbType.Int });
                            }
                            else
                            {
                                while (strQuery.Contains("{" + jObj["Name"].ToString() + "}"))
                                {
                                    strQuery = strQuery.Replace("{" + jObj["Name"].ToString() + "}", jObj["Value"].ToString());
                                }

                                AddParam(new { @Name = "@" + jObj["Name"].ToString(), @Value = jObj["Value"].ToString() });
                            }
                        }
                    }

                    sqlHandler.RunScriptNonQuery(strQuery, m_Params.param, cmdType, commandTimeout, out affecterow);

                    if (logging)
                    {
                        WriteDBLog(strQuery, sw.ElapsedMilliseconds.ToString() + "ms", parameter);
                    }
                }

                if (m_auto_transaction)
                {
                    Commit();
                }
                else
                {
                    if (sqlHandler.TranStatus() == false)
                    {
                        Close();
                    }
                }

                sw.Stop();

                return affecterow;
            }
            catch (Exception ex)
            {
                if (sw != null && sw.IsRunning == true)
                {
                    sw.Stop();
                }

                m_error = true;

                if (m_auto_transaction)
                {
                    RollBack();
                }
                else
                {
                    TranEnd();
                }

                WriteDBErrorLog(ex.Message.ToString());

                throw new Exception(ex.Message, ex);
            }
        }

        public int ExecuteNonQuery(string strQuery, object parameter, ref Hashtable output, int commandTimeout = 600, bool logging = true)
        {
            try
            {
                if (m_auto_transaction)
                {
                    Tran();
                }
                else
                {
                    Connect();
                }

                CommandType cmdType = GetSQLCmdType(strQuery);

                DataSet sp_param_ds = new DataSet();
                if (cmdType == CommandType.StoredProcedure)
                {
                    sqlHandler.RunScriptDataSet("sp_help " + strQuery, sp_param_ds, CommandType.Text);
                }

                int affecterow = 0;

                sw = new Stopwatch();
                sw.Start();

                if (parameter.GetType().ToString() == "System.Data.IDataParameter[]")
                {
                    sqlHandler.RunScriptNonQuery(strQuery, (IDataParameter[])parameter, cmdType, commandTimeout, out affecterow, ref output);

                    if (logging)
                    {
                        WriteDBLog(strQuery, sw.ElapsedMilliseconds.ToString() + "ms", parameter);
                    }
                }
                else
                {
                    string param = Newtonsoft.Json.JsonConvert.SerializeObject(
                        new
                        {
                            param = parameter
                        });

                    m_Params.items.Clear();

                    dynamic dParam = JsonConvert.DeserializeObject<dynamic>(param);

                    if (dParam != null)
                    {
                        foreach (dynamic p in dParam["param"])
                        {
                            var jObj = new RouteValueDictionary(p);

                            if (cmdType == CommandType.StoredProcedure)
                            {
                                if (sp_param_ds.Tables.Count >= 2)
                                {
                                    if (sp_param_ds.Tables[1].Select("Parameter_name = '@" + jObj["Name"].ToString() + "'").Length == 0)
                                    {
                                        continue;
                                    }
                                }
                            }

                            if ((jObj["Name"].ToString() == "util" && jObj["Value"].GetType().Name == "JObject") || jObj["Value"].GetType().Name == "JArray")
                            {
                                continue;
                            }
                            if (jObj["Value"].ToString() != "" && (((Newtonsoft.Json.Linq.JValue)jObj["Value"]).Value.GetType().Name == "Int32" || ((Newtonsoft.Json.Linq.JValue)jObj["Value"]).Value.GetType().Name == "Int64"))
                            {
                                AddParam(new { @Name = "@" + jObj["Name"].ToString(), @Value = jObj["Value"].ToString(), @SqlDbType = SqlDbType.Int });
                            }
                            else
                            {
                                while (strQuery.Contains("{" + jObj["Name"].ToString() + "}"))
                                {
                                    strQuery = strQuery.Replace("{" + jObj["Name"].ToString() + "}", jObj["Value"].ToString());
                                }

                                AddParam(new { @Name = "@" + jObj["Name"].ToString(), @Value = jObj["Value"].ToString() });
                            }
                        }

                        foreach (DictionaryEntry o in output)
                        {
                            DataRow[] spParams = sp_param_ds.Tables[1].Select("Parameter_name = '@" + o.Key.ToString() + "'");

                            if (spParams.Length > 0)
                            {
                                if (spParams[0]["Type"].ToString() == "int")
                                {
                                    AddParam(new { @Name = "@" + o.Key.ToString(), @Out = true, @SqlDbType = SqlDbType.Int });
                                }
                                else
                                {
                                    AddParam(new { @Name = "@" + o.Key.ToString(), @Out = true, @Size = Convert.ToInt32(spParams[0]["Length"].ToString()) });
                                }
                            }
                        }
                    }

                    sqlHandler.RunScriptNonQuery(strQuery, m_Params.param, cmdType, commandTimeout, out affecterow, ref output);

                    if (logging)
                    {
                        WriteDBLog(strQuery, sw.ElapsedMilliseconds.ToString() + "ms", parameter);
                    }
                }

                if (m_auto_transaction)
                {
                    Commit();
                }
                else
                {
                    if (sqlHandler.TranStatus() == false)
                    {
                        Close();
                    }
                }

                sw.Stop();

                return affecterow;
            }
            catch (Exception ex)
            {
                if (sw != null && sw.IsRunning == true)
                {
                    sw.Stop();
                }

                m_error = true;

                if (m_auto_transaction)
                {
                    RollBack();
                }
                else
                {
                    TranEnd();
                }

                WriteDBErrorLog(ex.Message.ToString());

                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 쿼리 실행
        /// </summary>
        /// <param name="strQuery">쿼리문</param>
        /// <param name="parameters">파라메타</param>
        /// <param name="commandTimeout">실행시간</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string strQuery, IDataParameter[] parameters, int commandTimeout = 600, bool logging = true)
        {
            try
            {
                if (m_auto_transaction)
                {
                    Tran();
                }
                else
                {
                    Connect();
                }

                CommandType cmdType = GetSQLCmdType(strQuery);
                int affecterow = 0;

                sw = new Stopwatch();
                sw.Start();

                sqlHandler.RunScriptNonQuery(strQuery, parameters, cmdType, commandTimeout, out affecterow);

                if (logging)
                {
                    WriteDBLog(strQuery, sw.ElapsedMilliseconds.ToString() + "ms", parameters);
                }

                if (m_auto_transaction)
                {
                    Commit();
                }
                else
                {
                    if (sqlHandler.TranStatus() == false)
                    {
                        Close();
                    }
                }

                sw.Stop();

                return affecterow;
            }
            catch (Exception ex)
            {
                if (sw != null && sw.IsRunning == true)
                {
                    sw.Stop();
                }

                m_error = true;

                if (m_auto_transaction)
                {
                    RollBack();
                }
                else
                {
                    TranEnd();
                }

                WriteDBErrorLog(ex.Message.ToString());
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region Log
        /// <summary>
        /// 현재 사용자의 아이디
        /// </summary>
        private string GetUserId
        {
            get
            {
                try
                {
                    if (HttpContext.Current.Session["Email"] != null)
                    {
                        return HttpContext.Current.Session["Email"].ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                catch { return ""; }
            }
        }

        /// <summary>
        /// get ip address 
        /// </summary>
        /// <returns></returns>
        private string GetIPAddress()
        {
            try
            {
                string ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    string[] addresses = ipAddress.Split(',');
                    if (addresses.Length != 0)
                    {
                        return addresses[0];
                    }
                }

                return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch
            {
                return "";
            }
        }

        private string GetActionType(string commandText)
        {
            int imax = commandText.Length;
            string type = string.Empty;

            if (string.IsNullOrEmpty(commandText))
            {
                type = "NON";
            }
            else
            {
                //공통코드 : C00033
                switch (commandText.Substring(imax - 3, 3).ToUpper())
                {
                    case "SEL": type = "SEL"; break;
                    case "INS": type = "INS"; break;
                    case "UPD": type = "UPD"; break;
                    case "DEL": type = "DEL"; break;
                }
            }

            return type;
        }

        /// <summary>
        /// write db log 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="elapsedTime"></param>
        public void WriteDBLog(String commandText, String elapsedTime, object parameter_ = null, string fileInfo = "", string fileName = "")
        {
            if (HttpContext.Current == null) return;// 웹어플리케이션이 아닌 배치인 경우 로그를 기록 하지 않는다. 

            Dictionary<string, string> parameter = new Dictionary<string, string>();

            String ip = "";
            try
            {
                String pathUrl = string.Empty;
                string userAgent = string.Empty;
                string urlReferrer = string.Empty;
                string UserApprove = string.Empty;

                try
                {
                    pathUrl = HttpContext.Current.Request.Url.PathAndQuery;
                    userAgent = HttpContext.Current.Request.UserAgent;
                    if (HttpContext.Current.Request.UrlReferrer != null)
                    {
                        urlReferrer = HttpContext.Current.Request.UrlReferrer.ToString();
                    }

                    ip = GetIPAddress();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (HttpContext.Current.Session["Levels"] != null)
                {
                    if (Convert.ToInt32(HttpContext.Current.Session["Levels"].ToString()) == 100)
                    {
                        UserApprove = "Admin";
                    }
                    else
                    {
                        UserApprove = "User";
                    }
                }
                else
                {
                    UserApprove = "";
                }


				string param = Newtonsoft.Json.JsonConvert.SerializeObject(
						new
						{
                            sql = commandText,
							param = parameter_
						});


				parameter.Add("USER_EMAIL", GetUserId);
                parameter.Add("LOG_INFO", "IP : [" + GetIPAddress() + "] Host Name : [" + HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "]");
                parameter.Add("APPROVE_TYPE", UserApprove);
                parameter.Add("APPLICATION_URL", pathUrl);
                parameter.Add("ACTION_TYPE", GetActionType(commandText));
                parameter.Add("UNIQUE_INFO", param);
                parameter.Add("DOWNLOAD_INFO", fileInfo);
                parameter.Add("DOWNLOAD_FILES", fileName);
                parameter.Add("HEADER_USER_AGENT", userAgent);
                parameter.Add("REFERRER_PAGE", urlReferrer);

                ExecuteNonQuery("SP_LOG_INS", parameter, 30, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorMessage"></param>
        public void WriteDBErrorLog(string errorMessage)
        {
            try
            {

                //Dictionary<string, string> parameter = new Dictionary<string, string>();
                //parameter.Add("USER_EMAIL", GetUserId);
                //parameter.Add("LOG_INFO", "IP : [" + GetIPAddress() + "] Host Name : [" + HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "]");
                //parameter.Add("ERROR_URL", HttpContext.Current.Request.Url.PathAndQuery);
                //parameter.Add("ERROR_MESSAGE", errorMessage);

                SqlParameter[] parameter = new SqlParameter[]
                {
                    new SqlParameter("USER_EMAIL", GetUserId),
                    new SqlParameter("LOG_INFO", "IP : [" + GetIPAddress() + "] Host Name : [" + HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "]" ),
                    new SqlParameter("ERROR_URL", HttpContext.Current.Request.Url.PathAndQuery ),
                    new SqlParameter("ERROR_MESSAGE", errorMessage )
                };

                ExecuteNonQuery("SP_ERROR_LOG_INS", parameter, 30, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// 
        /// </summary>
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
            Close();

            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (sqlHandler != null)
                        sqlHandler.Dispose();
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
