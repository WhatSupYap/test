using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;

namespace WebUtil
{
    /// <summary>
    /// 
    /// </summary>
    public class Converts : IDisposable
    {
        private bool disposed = false;
        private Files File = new Files();
        private Util Utils = null;

        /// <summary>
        /// 생성자
        /// </summary>
        public Converts(Util util)
        {
            Utils = util;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public string ObjToString(object strText)
        {
            try
            {
                if (strText == null)
                    return string.Empty;
                else
                    return strText.ToString();
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public int ObjToInt(object strValue)
        {
            try
            {
                if (strValue == null) return 0;
                if (strValue.ToString().Trim() == "") return 0;
                if (strValue.ToString().IndexOf(".") == -1)
                {
                    return Convert.ToInt32(strValue.ToString().Replace(",", ""));
                }
                else
                {
                    return Convert.ToInt32(strValue.ToString().Replace(",", "").Substring(0, strValue.ToString().IndexOf(".")));
                }

            }
            catch
            {
                return 0;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public int ObjToInt(object strValue, int defValue)
        {
            try
            {
                return ObjToInt(strValue);
            }
            catch
            {
                return defValue;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public bool StringToBool(string strText)
        {
            try
            {
                if (strText == "1")
                    return true;
                else if (strText == "0" || strText.Trim() == "")
                    return false;
                else
                    return bool.Parse(strText.ToString());
            }
            catch
            {
                return false;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public bool ObjToBool(object strText)
        {
            try
            {
                if (strText == null || strText.ToString() == "") return false;

                if (strText.ToString().Trim() == "1")
                    return true;
                else if (strText.ToString().Trim() == "0" || strText.ToString().Trim() == "")
                    return false;
                else if (strText.ToString() == "True")
                    return true;
                else if (strText.ToString() == "False")
                    return false;
                else if (strText.ToString() == "true")
                    return true;
                else if (strText.ToString() == "false")
                    return false;
                else if (strText.ToString() == "Y")
                    return true;
                else if (strText.ToString() == "N")
                    return false;
                else if (strText.ToString() == "y")
                    return true;
                else if (strText.ToString() == "n")
                    return false;
                else if (strText.ToString() == "yes")
                    return true;
                else if (strText.ToString() == "no")
                    return false;
                else
                {
                    int value = int.Parse(strText.ToString());
                    if (value == 1) return true;
                    else return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public byte[] Base64ToByte(string base64String)
        {
            byte[] imageBytes = null;
            try
            {
                imageBytes = System.Convert.FromBase64String(base64String);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Base64ToByte = " + e.Message);
            }
            return imageBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string FileToBase64(string path)
        {
            if (File.IsFile(path))
            {
                byte[] buffer = System.IO.File.ReadAllBytes(path);
                return Convert.ToBase64String(buffer);
            }
            else
            {
                return "";
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string ImageToBase64(string path)
        {
            string imageString = null;
            System.Drawing.Image image = System.Drawing.Image.FromFile(path);
            if (image != null)
            {
                MemoryStream ms = new MemoryStream();
                if (ms != null)
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] bytes = ms.ToArray();
                    imageString = Convert.ToBase64String(bytes, 0, bytes.Length, Base64FormattingOptions.None);
                }
            }

            if (image != null)
                image.Dispose();
            return imageString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public System.Drawing.Image Base64ToImage(string base64String)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new MemoryStream(bytes);
                return System.Drawing.Image.FromStream(ms);
            }
            catch
            {
                return null;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public double ObjToDouble(object strValue)
        {
            try
            {
                return double.Parse(strValue.ToString());
            }
            catch
            {
                return 0.0;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public double ObjToDouble(object strValue, int number)
        {
            try
            {
                return Math.Round(double.Parse(strValue.ToString()), number);
            }
            catch
            {
                return 0.0;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public long ObjToLong(object strText)
        {
            try
            {
                if (strText.ToString().Trim() == "")
                    return 0;

                return long.Parse(strText.ToString());
            }
            catch
            {
                if (strText == null) return 0;

                int po = strText.ToString().IndexOf(".");

                if (po > -1)
                {
                    return long.Parse(strText.ToString().Substring(0, po));
                }
                return 0;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public decimal ObjToDecimal(object strValue)
        {
            try
            {
                return Decimal.Parse(strValue.ToString());
            }
            catch
            {
                return 0M;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string DataSetToString(DataSet ds)
        {
            if (ds == null) return "";

            StringBuilder strReturn = new StringBuilder();

            for (int i = 0; i < ds.Tables.Count; i++)
            {
                strReturn.Append(DataTableToString(ds.Tables[i]));
                strReturn.Append(Utils.SP_TABLE);
            }

            return strReturn.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string DataTableToString(DataTable dt)
        {
            if (dt == null)
            {
                return "";
            }

            StringBuilder strReturn = new StringBuilder();

            strReturn.Append(dt.TableName + Utils.CNST_SP_NT);

            // 컬럼명
            for (int col = 0; col < dt.Columns.Count; col++)
            {
                var ColumnsName = dt.Columns[col].ColumnName;
                var ColumnsType = "string";

                if (dt.Columns[col].DataType.Name == "String")
                    ColumnsType = "string";
                else if (dt.Columns[col].DataType.Name == "Int32")
                    ColumnsType = "int";
                else if (dt.Columns[col].DataType.Name == "double")
                    ColumnsType = "double";
                else if (dt.Columns[col].DataType.Name == "Decimal")
                    ColumnsType = "Decimal";
                else if (dt.Columns[col].DataType.Name == "Boolean")
                    ColumnsType = "Boolean";

                strReturn.Append(ColumnsName + "||" + ColumnsType + Utils.CNST_SP);
            }
            strReturn.Append(Utils.CNST_SP_LF);

            // 데이타
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    var ColumnsName = dt.Columns[col].ColumnName;
                    strReturn.Append(dt.Rows[i][ColumnsName] + Utils.CNST_SP);
                }
                strReturn.Append(Utils.CNST_SP_LF);
            }

            return strReturn.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strDs"></param>
        /// <returns></returns>
        public DataSet StringToDataSet(string strDs)
        {
            DataSet ds = new DataSet("ROOT");

            ArrayList dsTmp = Utils.SplitToArrayList(strDs, Utils.SP_TABLE);
            for (int i = 0; i < dsTmp.Count; i++)
            {
                string dtStr = dsTmp[i].ToString();
                if (dtStr == "")
                    continue;

                DataTable dsTable = StringToDataTable(dtStr);
                if (dsTable != null)
                    ds.Tables.Add(dsTable);
            }

            return ds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strDt"></param>
        /// <returns></returns>
        public DataTable StringToDataTable(string strDt)
        {
            if (strDt == "")
                return null;

            try
            {
                string[] dtArray = Utils.Split(strDt, Utils.CNST_SP_NT);
                string dtName = dtArray[0];
                string[] dtData = Utils.Split(dtArray[1], Utils.CNST_SP_LF);

                DataTable dt = new DataTable(dtName);

                int[] ColArr = null;
                for (int i = 0; i < dtData.Length - 1; i++)
                {
                    string strRow = dtData[i];

                    if (strRow != "")
                    {
                        string[] strRowData = Utils.Split(strRow, Utils.CNST_SP);
                        if (i == 0)
                        {
                            ColArr = new int[strRowData.Length];

                            for (int col = 0; col < strRowData.Length - 1; col++)
                            {
                                string[] ColumnsInfo = Utils.Split(strRowData[col], "||");

                                // 컬럼 생성
                                if (dt.Columns.Contains(ColumnsInfo[0]) == false)
                                {
                                    if (ColumnsInfo[1] == "string")
                                        dt.Columns.Add(ColumnsInfo[0], typeof(String));
                                    else if (ColumnsInfo[1] == "int")
                                        dt.Columns.Add(ColumnsInfo[0], typeof(int));
                                    else if (ColumnsInfo[1] == "Int64")
                                        dt.Columns.Add(ColumnsInfo[0], typeof(Int64));
                                    else if (ColumnsInfo[1] == "double")
                                        dt.Columns.Add(ColumnsInfo[0], typeof(double));
                                    else if (ColumnsInfo[1] == "Decimal")
                                        dt.Columns.Add(ColumnsInfo[0], typeof(Decimal));
                                    else if (ColumnsInfo[1] == "Boolean")
                                        dt.Columns.Add(ColumnsInfo[0], typeof(Boolean));
                                    else
                                        dt.Columns.Add(ColumnsInfo[0], typeof(String));

                                    ColArr[col] = col;
                                }
                                else
                                {
                                    ColArr[col] = -1;
                                }
                            }
                        }
                        else
                        {
                            // 데이타 넣기
                            DataRow Rows = dt.NewRow();
                            int ColNumber = 0;
                            for (int col = 0; col < ColArr.Length - 1; col++)
                            {
                                if (ColArr[col] > -1)
                                {
                                    try
                                    {
                                        if (strRowData[col].ToString().Trim() != "undefined")
                                            Rows[ColNumber] = strRowData[col].ToString().Trim();
                                    }
                                    catch
                                    {

                                    }
                                    ColNumber++;
                                }
                            }
                            dt.Rows.Add(Rows);
                        }
                    }
                }

                return dt;
            }
            catch
            {
                return null;
            }
        }

        #region --------------------------IDisposable Members-----------------------------
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
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (File != null)
                        File = null;
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
