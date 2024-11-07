using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Configuration;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.parser;
using iTextSharp.text;

namespace WebUtil
{
	/// <summary>
	/// 
	/// </summary>
	public class Util : IDisposable
	{
		#region -----------------변수 및 Property---------------------------
		private bool disposed = false;

		private Converts m_Convert = null;
		private Files m_File = null;
		private DB m_Db = null;

		private string cNST_SP = "~,";
		private string cNST_SP_LF = "~I~";
		private string cNST_SP_NT = "|~|";
		private string fILE_SP = "~~<SFP>~~";
		private string fILE_SP_LF = "~~<FSPLF>~~";
		private string sP_TABLE = "~~||TBL||~~";
		private HttpRequestBase utilRequest = null;
		private HttpResponseBase utilResponse = null;
		private string m_AuthToken = "AUTHTOKEN";
		private string m_SendSiteName = "SENDSITENAME";
		private string m_DBConnectionFilePath = "";
		private string m_DBXmlFilePath = "";
		private string m_DBConnectString = "";

		public string SP_TABLE { get { return sP_TABLE; } }
		public string CNST_SP_NT { get { return cNST_SP_NT; } }
		public string FILE_SP { get { return fILE_SP; } }
		public string FILE_SP_LF { get { return fILE_SP_LF; } }
		public string CNST_SP_LF { get { return cNST_SP_LF; } }
		public string CNST_SP { get { return cNST_SP; } }
		public HttpRequestBase Request
		{
			get
			{
				return this.utilRequest;
			}
			set
			{
				this.utilRequest = value;
			}
		}
		public HttpResponseBase Response
		{
			get
			{
				return this.utilResponse;
			}
			set
			{
				this.utilResponse = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Converts Convert
		{
			get
			{
				return this.m_Convert;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Files File
		{
			get
			{
				return this.m_File;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public DB DB
		{
			get
			{
				return this.m_Db;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string DBConnectionFilePath
		{
			set
			{
				m_DBConnectionFilePath = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string DBXmlFilePath
		{
			set
			{
				m_DBXmlFilePath = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string DbConnection
		{
			set
			{
				string StrCon = "";

				if (m_DBConnectionFilePath != "")
				{
					try
					{
						XmlDocument xmlDoc = new XmlDocument();
						xmlDoc.Load(m_DBConnectionFilePath);

						XmlNode connStrNode = xmlDoc.DocumentElement.SelectSingleNode("ConnectionString/Item[@name='" + value + "']");

						StrCon = connStrNode.Attributes["value"].Value;
					}
					catch
					{
						StrCon = value;
					}
				}
				else
				{
					StrCon = value;
				}

				m_DBConnectString = value;

				if (this.m_Db != null)
					this.m_Db.Dispose(); this.m_Db = null;
				this.m_Db = new DB(StrCon);
			}
		}

		public string DBConnCfg
		{
			set
			{
				string ConnectionFilePath = "D:\\DBConnCfg\\ConnStr.xml";
				string StrCon = "";

				try
				{
					XmlDocument xmlDoc = new XmlDocument();
					xmlDoc.Load(ConnectionFilePath);

					XmlNode connStrNode = xmlDoc.DocumentElement.SelectSingleNode("ConnectionString/Item[@name='" + value + "']");

					StrCon = connStrNode.Attributes["value"].Value;
				}
				catch
				{
					StrCon = value;
				}

				if (this.m_Db != null)
					this.m_Db.Dispose(); this.m_Db = null;
				this.m_Db = new DB(StrCon);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string AuthToken
		{
			get
			{
				return m_AuthToken;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string SendSiteName
		{
			get
			{
				return m_SendSiteName;
			}
		}
		#endregion

		/// <summary>
		/// 생성자
		/// </summary>
		public Util(string Connection = "")
		{
			this.m_Convert = new Converts(this);
			this.m_File = new Files();
			this.m_Db = new DB();

			if (Connection != "")
			{
				DbConnection = Connection;
			}
		}

		/// <summary>
		/// 생성자2
		/// </summary>
		/// <param name="request"></param>
		public Util(HttpRequestBase request, string Connection = "")
		{
			this.m_Convert = new Converts(this);
			this.m_File = new Files();
			this.utilRequest = request;
			this.m_Db = new DB();

			if (Connection != "")
			{
				DbConnection = Connection;
			}
		}

		#region -----------------Function---------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="CheckValue"></param>
		/// <returns></returns>
		public bool DBConnectStrCheck(string val)
		{
			if (val == m_DBConnectString)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="CheckValue"></param>
		/// <returns></returns>
		public bool IsValue(object CheckValue)
		{
			try
			{
				if (CheckValue == null || CheckValue.ToString().Trim() == "")
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="CheckValues"></param>
		/// <returns></returns>
		public bool IsCheck(string value, string[] CheckValues)
		{
			bool rtnValue = false;
			if (value == null) value = "";

			for (int i = 0; i < CheckValues.Length; i++)
			{
				if (CheckValues[i].Trim() == value.Trim())
				{
					rtnValue = true;
					break;
				}
			}

			return rtnValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public bool IsValidEmail(string email)
		{
			bool valid = Regex.IsMatch(email, @"^[0-9a-zA-Z]([-.]?[\w])*@[0-9a-zA-Z]([-.]?[\w])*\.[a-zA-Z]{2,10}$");
			return valid;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strValue"></param>
		/// <param name="startIndex"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public string Mid(string strValue, int startIndex, int length)
		{
			try
			{
				if (strValue.Length >= startIndex + length)
				{
					return strValue.Substring(startIndex, length);
				}
				else
				{
					return strValue.Substring(startIndex, strValue.Length);
				}
			}
			catch
			{
				return "";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strValue"></param>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		public string Mid(string strValue, int startIndex)
		{
			try
			{
				return strValue.Substring(startIndex);
			}
			catch
			{
				return "";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strValue"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public string Left(string strValue, int length)
		{
			try
			{
				if (strValue.Length < length)
					return strValue.Substring(0, strValue.Length);
				else
					return strValue.Substring(0, length);
			}
			catch
			{
				return strValue.Substring(0, strValue.Length);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strValue"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public string Right(string strValue, int length)
		{
			try
			{
				if (length > strValue.Length)
					length = strValue.Length;

				return strValue.Substring(strValue.Length - length, length);
			}
			catch
			{
				return "";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="src"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public string FindGetStr(string src, string start, string end)
		{
			int startPos = src.IndexOf(start);

			if (startPos < 0)
			{
				return "";
			}

			startPos += start.Length;

			int endPos = src.IndexOf(end, startPos);

			if (endPos < 0)
			{
				return "";
			}

			string content = src.Substring(startPos, endPos - startPos);

			return content;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public string[] Split(string input, string pattern)
		{
			if (IsValue(input) == false) input = "";
			pattern = pattern.Replace("|", "\\|");
			return Regex.Split(input, pattern);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public ArrayList SplitToArrayList(string input, string pattern)
		{
			ArrayList ar = new ArrayList();

			pattern = pattern.Replace("|", "\\|");
			string[] stringArray = Regex.Split(input, pattern);
			for (int ii = 0; ii < stringArray.Length; ii++)
			{
				ar.Add(stringArray[ii]);
			}
			return ar;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <param name="sp"></param>
		/// <param name="sp_lf"></param>
		/// <returns></returns>
		public string[][] SplitToArray(string input, string sp, string sp_lf)
		{
			int intTagIdx = input.IndexOf(sp_lf);
			sp_lf = sp_lf.Replace("|", "\\|");
			sp = sp.Replace("|", "\\|");

			if (intTagIdx > 0)
			{
				string[] stringRow = System.Text.RegularExpressions.Regex.Split(input, sp_lf);

				string[][] strData = new string[stringRow.Length][];

				for (int ii = 0; ii < stringRow.Length - 1; ii++)
				{
					strData[ii] = System.Text.RegularExpressions.Regex.Split(stringRow[ii], sp);
				}
				return strData;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 숫자 0 붙이기
		/// </summary>
		/// <param name="number"></param>
		/// <param name="intMaxNumber"></param>
		/// <returns></returns>
		public string GetZeroNumber(int number, int intMaxNumber)
		{
			return string.Format("{0:D" + intMaxNumber.ToString() + "}", number);
		}

		/// <summary>
		/// 날짜가지고 오기
		/// </summary>
		/// <param name="Date"></param>
		/// <returns></returns>
		public string GetDate(string Date, string Gubun = ".")
		{
			if (Date == "")
				return "";
			else
			{
				Date = Date.Replace("-", "").Replace(".", "");
				try
				{
					return Date.Substring(0, 4) + Gubun + Date.Substring(4, 2) + Gubun + Date.Substring(6, 2);
				}
				catch
				{
					return "";
				}
			}
		}

		public string GetEngMonth(string Month)
		{
			string EngMonth = string.Empty;
			switch (Month)
			{
				case "01":
					EngMonth = "January";
					break;
				case "02":
					EngMonth = "February";
					break;
				case "03":
					EngMonth = "March";
					break;
				case "04":
					EngMonth = "April";
					break;
				case "05":
					EngMonth = "May";
					break;
				case "06":
					EngMonth = "June";
					break;
				case "07":
					EngMonth = "July";
					break;
				case "08":
					EngMonth = "August";
					break;
				case "09":
					EngMonth = "September";
					break;
				case "10":
					EngMonth = "October";
					break;
				case "11":
					EngMonth = "November";
					break;
				case "12":
					EngMonth = "December";
					break;
				default:
					throw new NotImplementedException($"{Month} is not Month");
			}

			return EngMonth;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetTimes(bool secondAdd = false)
		{
			System.DateTime Date = System.DateTime.Now;

			if (secondAdd)
			{
				return Date.ToString("yyyyMMddHHmmssfffff");
			}
			else
			{
				return Date.ToString("yyyyMMddHHmmss");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Date"></param>
		/// <returns></returns>
		public string GetDateTime(string Date)
		{
			if (Date == "")
				return "";
			else
			{
				Date = Date.Replace("-", "").Replace(".", "");
				try
				{
					return Date.Substring(0, 4) + "-" + Date.Substring(4, 2) + "-" + Date.Substring(6, 2) + " " + Date.Substring(8, 2) + "시";
				}
				catch
				{
					return "";
				}
			}
		}

		/// <summary>
		/// 두 날짜 사이 날짜 비교
		/// </summary>
		/// <param name="Date1"></param>
		/// <param name="Date2"></param>
		/// <returns></returns>
		public int GetDateDiffDay(DateTime Date1, DateTime Date2)
		{
			try
			{
				string date1 = Date1.ToShortDateString();
				string date2 = Date2.ToShortDateString();
				DateTime T1 = DateTime.Parse(date1);
				DateTime T2 = DateTime.Parse(date2);

				TimeSpan ts = T1 - T2;

				// Difference in days.
				int differenceInDays = ts.Days;

				return differenceInDays;
			}
			catch
			{
				return 0;
			}
		}

		/// <summary>
		/// 두 날짜 사이 시간 비교
		/// </summary>
		/// <param name="Date1"></param>
		/// <param name="Date2"></param>
		/// <returns></returns>
		public double GetDateDiffHours(DateTime T1, DateTime T2)
		{
			try
			{
				TimeSpan ts = T1 - T2;

				// Difference in days.
				double differenceInHours = ts.TotalHours;

				return differenceInHours;
			}
			catch
			{
				return 0;
			}
		}

		/// <summary>
		/// 두 날짜 사이 시간:분
		/// </summary>
		/// <param name="Date1"></param>
		/// <param name="Date2"></param>
		/// <returns></returns>
		public string GetDateDiffHoursMinutes(DateTime T1, DateTime T2, string format = "00:00")
		{
			try
			{
				TimeSpan ts = T1 - T2;

				// Difference in Minutes.
				double differenceInMinutes = ts.TotalMinutes;

				if (format == "00:00")
				{
					TimeSpan ts2 = TimeSpan.FromMinutes(differenceInMinutes);
					return string.Format("{0}:{1}", ts2.Hours, ts2.Minutes);
				}
				else
				{
					return differenceInMinutes.ToString();
				}
			}
			catch
			{
				return "";
			}
		}

		/// <summary>
		///  Fiscal Year 구하기 (현재일자)
		/// </summary>
		/// <returns></returns>
		public string GetFiscalYear()
		{
			return GetFiscalYear(DateTime.Now);
		}

		/// <summary>
		/// Fiscal Year 구하기
		/// </summary>
		/// <param name="dt">DateTime</param>
		/// <returns>"YYYY" ex: "2024"</returns>
		public string GetFiscalYear(DateTime dt)
		{
			string returnResult = string.Empty;

			if (dt.Month > 5)
			{
				returnResult = dt.AddYears(1).Year.ToString();
			}
			else if (dt.Month < 6)
			{
				returnResult = dt.Year.ToString();
			}

			return returnResult;
		}

		/// <summary>
		/// 날짜가지고 오기
		/// </summary>
		/// <param name="Date"></param>
		/// <returns></returns>
		public string GetDateAMPM(string Date)
		{
			if (Date == "")
				return "";
			else
			{
				Date = Date.Replace("AM", "오전").Replace("PM", "오후");
				return Date;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="check"></param>
		/// <param name="trueValue"></param>
		/// <param name="falseValue"></param>
		/// <returns></returns>
		public string GetCheckIf(object value, object check, string trueValue, string falseValue = "")
		{
			if (check is string[])
			{
				string[] tmp_check = (string[])check;
				for (int i = 0; i < tmp_check.Length; i++)
				{
					if (tmp_check[i] == Convert.ObjToString(value))
					{
						return trueValue;
					}
				}

				return falseValue;
			}
			else
			{
				if (Convert.ObjToString(value) == Convert.ObjToString(check))
				{
					return trueValue;
				}
				else
				{
					return falseValue;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Str"></param>
		/// <param name="Rev"></param>
		/// <returns></returns>
		public string DBStr(string Str, bool Rev = false)
		{
			string RtnValue = "";
			if (Str == null || Str == "")
			{
				RtnValue = "";
			}
			else
			{
				if (Rev == true)
				{
					RtnValue = Str.Replace("&#39;", "'").Replace("&#34;", "\"");
				}
				else
				{
					RtnValue = Str.Replace("'", "&#39;").Replace("\"", "&#34;");
				}
			}

			return RtnValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public string GetNumComma(object value)
		{
			string RtnValue = "";
			string val = Convert.ObjToString(value);

			if (val != "" && val != "0")
			{
				string[] unpadded_arr = val.Split('.');
				string unpadded = "";
				if (unpadded_arr.Length == 2)
				{
					val = unpadded_arr[0];
					unpadded = unpadded_arr[1];
				}

				val = val.Replace(",", "");
				bool minus = val.Contains("-");
				val = val.Replace("-", "");

				string tmpValue = m_Convert.ObjToLong(val).ToString();

				int count = 0;
				for (int i = tmpValue.Length; i > 0; i--)
				{
					if (count == 3 || count == 6 || count == 9 || count == 12 || count == 15 || count == 18)
						RtnValue = "," + RtnValue;
					RtnValue = tmpValue.Substring(i - 1, 1) + RtnValue;
					count++;
				}

				if (minus) RtnValue = "-" + RtnValue;
				if (unpadded != "") RtnValue = RtnValue + "." + unpadded;
			}

			if (RtnValue == "")
				RtnValue = "0";

			return RtnValue;
		}

		/// <summary>
		/// 월화수목금토일  DPCRM 코드 체크 (문자로 리턴함)
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public string GetDayOfWeekCheck(object value)
		{
			string RtnValue = "";
			string val = Convert.ObjToString(value);

			string[] week = { "월", "화", "수", "목", "금", "토", "일" };
			int[] week_int = { 1, 2, 4, 8, 16, 32, 64 };

			if (val != "" && val != "0")
			{
				for (int i = 0; i < week.Length; i++)
				{

					if ((week_int[i] & int.Parse(val)) > 0)
					{
						RtnValue += week[i].ToString() + ",";
					}

				}
			}

			if (RtnValue == "")
			{
				RtnValue = "없음";
			}
			else
			{
				RtnValue = RtnValue.Remove(RtnValue.Length - 1, 1);
			}


			return RtnValue;
		}

		/// <summary>
		/// 숫자 형식의 데이타 멀티
		/// </summary>
		/// <param name="value"></param>
		/// <returns>datas/ints/rtnInts/rtnDatas/rtnData</returns>
		public Hashtable GetNumberMultiObj(List<string> data, int value = -1, string splitString = ",")
		{
			Hashtable rtnData = new Hashtable();
			List<int> rtnInts = new List<int>();
			List<string> rtnDatas = new List<string>();
			StringBuilder rtnDataString = new StringBuilder();
			rtnData.Add("datas", data);

			if (data != null && data.Count > 0)
			{
				List<int> ints = new List<int>();

				int intValue = 1;
				for (int i = 0; i < data.Count; i++)
				{
					ints.Add(intValue);

					if (value != -1)
					{
						if ((intValue & value) > 0)
						{
							rtnInts.Add(intValue);
							rtnDatas.Add(data[i]);

							if (rtnDataString.ToString() != "")
								rtnDataString.Append(splitString);

							rtnDataString.Append(data[i]);
						}
					}

					intValue = intValue * 2;
				}

				rtnData.Add("ints", ints);
			}
			else
			{
				rtnData.Add("ints", new List<int>());
			}

			rtnData.Add("rtnInts", rtnInts);
			rtnData.Add("rtnDatas", rtnDatas);
			rtnData.Add("rtnData", rtnDataString.ToString());
			return rtnData;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public string GetPhone(string val)
		{
			string RtnValue = "";

			if (val != "")
			{
				val = val.Replace("-", "");
				string tmpValue = val;

				if (tmpValue.Length == 11)
					RtnValue = tmpValue.Substring(0, 3) + "-" + tmpValue.Substring(3, 4) + "-" + tmpValue.Substring(7, 4);
				else
					RtnValue = tmpValue;
			}

			return RtnValue;
		}

		/// <summary>
		/// 3자리로 처리된 전화번호를 하나의 문자열로 합치기.
		/// </summary>
		/// <param name="no1"></param>
		/// <param name="no2"></param>
		/// <param name="no3"></param>
		/// <returns></returns>
		public string GetPhone(string no1, string no2, string no3)
		{
			string phoneNum = "";

			if (!string.IsNullOrWhiteSpace(no1) && !string.IsNullOrWhiteSpace(no2) && !string.IsNullOrWhiteSpace(no3))
			{
				phoneNum = no1 + "-" + no2 + "-" + no3;
			}

			return phoneNum;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public string GetSSN(string val)
		{
			string RtnValue = "";

			if (val != "")
			{
				val = val.Replace("-", "");
				string tmpValue = val;

				if (tmpValue.Length == 13)
					RtnValue = tmpValue.Substring(0, 6) + "-" + tmpValue.Substring(6, 7);
				else
					RtnValue = tmpValue;
			}

			return RtnValue;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Total"></param>
		/// <param name="Limit"></param>
		/// <returns></returns>
		public int GetSize(int Total, int Limit = 10)
		{
			if (Limit == 0)
				return 0;

			return (Total % Limit == 0) ? this.m_Convert.ObjToInt(Total / Limit) : this.m_Convert.ObjToInt(Total / Limit + 1);
		}

		/// <summary>
		/// 줄바꿈
		/// </summary>
		/// <param name="body"></param>
		/// <returns></returns>
		public string NL2Br(string body)
		{
			if (body == null || body == "")
			{
				return "";
			}
			else
			{
				return body.Replace("\n", "<br />");
			}
		}

		/// <summary>
		/// 빈칸적용
		/// </summary>
		/// <param name="body"></param>
		/// <returns></returns>
		public string NBSP(string body)
		{
			if (body == null || body == "")
			{
				return "";
			}
			else
			{
				return body.Replace(" ", "&nbsp;");
			}
		}

		/// <summary>
		/// 빈칸적용
		/// </summary>
		/// <param name="body"></param>
		/// <returns></returns>
		public string ReplaceNBSP(string body)
		{
			if (body == null || body == "")
			{
				return "";
			}
			else
			{
				return body.Replace("&nbsp;", " ");
			}
		}

		public string GetFileSize(int FileSize)
		{
			string strFileSize = "";

			if (FileSize >= 1024000)
				strFileSize = GetNumComma(FileSize / 1024000).ToString() + " MB";
			else if (FileSize >= 1024)
				strFileSize = GetNumComma(FileSize / 1024).ToString() + " KB";
			else if (FileSize == 0)
				strFileSize = "0 KB";
			else
				strFileSize = "1 KB";

			return strFileSize;
		}

		public string GetFileSize(long FileSize, string viewMode = "", string dec = "")
		{
			string strFileSize = "";

			if (viewMode == "GB")
			{
				strFileSize = ((decimal)FileSize / (decimal)1073741824).ToString("#,##0" + dec) + " GB";
			}
			else if (viewMode == "MB")
			{
				strFileSize = ((decimal)FileSize / (decimal)1048576).ToString("#,##0" + dec) + " MB";
			}
			else if (viewMode == "KB")
			{
				strFileSize = ((decimal)FileSize / (decimal)1024).ToString("#,##0" + dec) + " KB";
			}
			else
			{
				if (FileSize >= 1073741824)
					strFileSize = GetNumComma(FileSize / 1073741824).ToString() + " GB";
				else if (FileSize >= 1048576)
					strFileSize = GetNumComma(FileSize / 1048576).ToString() + " MB";
				else if (FileSize >= 1024)
					strFileSize = GetNumComma(FileSize / 1024).ToString() + " KB";
				else if (FileSize == 0)
					strFileSize = "0 KB";
				else
					strFileSize = "1 KB";
			}

			return strFileSize;
		}


		#region 용량 단위 변환
		public long Mb2Kb(long nMb)
		{
			long nKb = nMb * 1024;
			return nKb;
		}

		public long ByteToKb(long nByte)
		{
			long nKb = nByte / 1024;
			return nKb;
		}

		public long Kb2Mb(long nKb)
		{
			long nMb = nKb / 1024;
			return nMb;
		}
		#endregion

		#region 퍼센트 계산(소숫점 1자리)
		public decimal ProcPercent(long n1, long n2)
		{
			decimal nPercent = (decimal)n2 / (decimal)((decimal)n1 / 100);
			nPercent = Math.Round(nPercent, 1);
			return nPercent;
		}
		#endregion

		/// <summary>
		/// 줄바꿈 + 빈칸적용
		/// </summary>
		/// <param name="body"></param>
		/// <returns></returns>
		public string BrNBSP(string body)
		{
			return NL2Br(NBSP(body));
		}

		/// <summary>
		/// HTML 태그제거
		/// </summary>
		/// <param name="orgText"></param>
		/// <param name="replaceStr"></param>
		/// <returns></returns>
		public string ReplaceHtmlTag(string orgText, string replaceStr = "")
		{
			if (orgText == null)
				return "";

			// Type1 
			string pattern = @"<[^>]+>";

			// Type? 
			// string pattern = @"<\s*(\S+)(\s[^>]*)?>[\s\S]*<\s*\/\1\s*>"; 

			Regex rg = new Regex(pattern);
			return rg.Replace(orgText, replaceStr);
		}

		/// <summary>
		/// Script 및 iframe 등 제거
		/// </summary>
		/// <param name="orgText"></param>
		/// <returns></returns>
		public string ReplaceScriptTag(string orgText)
		{
			if (orgText == null)
				return "";

			string result = Regex.Replace(
				orgText,
				@"</?(?i:script|embed|object|frameset|frame|iframe)(.|\n|\s)*?>",
				string.Empty,
				RegexOptions.Singleline | RegexOptions.IgnoreCase
			);

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="htmlObj"></param>
		/// <returns></returns>
		public string GetHtml(HtmlGenericControl htmlObj)
		{
			string RtnHtml = "";

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.IO.StringWriter stWriter = new System.IO.StringWriter(sb);
			System.Web.UI.HtmlTextWriter htmlWriter = new System.Web.UI.HtmlTextWriter(stWriter);
			htmlObj.RenderControl(htmlWriter);
			RtnHtml = sb.ToString();

			return RtnHtml;
		}

		public string GetHtml(string FilePath)
		{
			string BodyHtml = m_File.Read(FilePath);
			return BodyHtml;
		}

		public string GetHtml(Object Data, string BodyHtml, bool FilePath = false)
		{
			if (FilePath == true)
				BodyHtml = m_File.Read(BodyHtml);

			string type = Data.GetType().Name;

			switch (type)
			{
				case "Hashtable":
					Hashtable hdata = (Hashtable)Data;
					BodyHtml = GetHtml(hdata, BodyHtml);
					break;
				case "Dictionary`2":
					Dictionary<string, object> data = (Dictionary<string, object>)Data;
					BodyHtml = GetHtml(data, BodyHtml);
					break;
				case "DataTable":
					DataTable dt = (DataTable)Data;
					BodyHtml = GetHtml(dt, BodyHtml);
					break;
				case "DataSet":
					DataSet ds = (DataSet)Data;
					BodyHtml = GetHtml(ds, BodyHtml);
					break;
			}

			return BodyHtml;
		}

		public string GetHtml(Hashtable datas, string BodyHtml)
		{

			if (BodyHtml.Length > 0)
			{
				Match match = Regex.Match(BodyHtml, @"{(\w+)}");

				while (match.Success)
				{
					if (datas.ContainsKey(match.Groups[1].Value) == true)
						BodyHtml = BodyHtml.Replace("{" + match.Groups[1].Value + "}", HttpUtility.HtmlEncode((string)datas[match.Groups[1].Value]));
					match = match.NextMatch();
				}
				BodyHtml = FunctionHtml(BodyHtml, datas);
			}

			return BodyHtml;
		}

		public string GetHtml(Dictionary<string, object> datas, string BodyHtml)
		{
			if (BodyHtml.Length > 0)
			{
				Match match = Regex.Match(BodyHtml, @"{(\w+)}");

				while (match.Success)
				{
					if (datas.ContainsKey(match.Groups[1].Value) == true)
					{
						object value = "";
						datas.TryGetValue(match.Groups[1].Value, out value);
						BodyHtml = BodyHtml.Replace("{" + match.Groups[1].Value + "}", HttpUtility.HtmlEncode((string)value));
					}
					match = match.NextMatch();
				}
			}

			return BodyHtml;
		}

		public string GetHtml(DataTable dt, string BodyHtml)
		{
			if (BodyHtml.Length > 0)
			{
				if (dt != null && dt.Rows.Count > 0)
				{
					Match match = Regex.Match(BodyHtml, @"{(\w+)}");

					while (match.Success)
					{
						if (dt.Columns.Contains(match.Groups[1].Value))
							BodyHtml = BodyHtml.Replace("{" + match.Groups[1].Value + "}", HttpUtility.HtmlEncode(dt.Rows[0][match.Groups[1].Value].ToString()));

						match = match.NextMatch();
					}
					BodyHtml = FunctionHtml(BodyHtml, dt.Rows[0]);
				}
			}

			return BodyHtml;
		}

		public string GetHtml(DataSet ds, string BodyHtml)
		{
			if (BodyHtml.Length > 0)
			{
				if (ds != null && ds.Tables.Count > 0)
				{
					for (int i = 0; i < ds.Tables.Count; i++)
					{
						DataTable dt = ds.Tables[i];
						BodyHtml = GetLoopHtml(dt, BodyHtml);
					}
				}
			}

			return BodyHtml;
		}

		public string GetLoopHtml(DataTable dt, string BodyHtml)
		{
			string loopString = GetSkin(ref BodyHtml, "<!--[" + dt.TableName + "]>", "<[" + dt.TableName + "]-->", "[[" + dt.TableName + ".Replace]]");
			string noDataLoopString = GetSkin(ref BodyHtml, "<!--[" + dt.TableName + ".NoData]>", "<[" + dt.TableName + ".NoData]-->", "");

			if (loopString != "")
			{
				if (dt.Rows.Count == 0)
				{
					BodyHtml = BodyHtml.Replace("[[" + dt.TableName + ".Replace]]", noDataLoopString);
				}
				else
				{
					string strLoop = "";
					for (int dti = 0; dti < dt.Rows.Count; dti++)
					{
						Match loopMatch = Regex.Match(loopString, @"{(\w+)}");

						string tmpLoopString = loopString;
						tmpLoopString = FunctionHtml(tmpLoopString, dt.Rows[dti]);

						while (loopMatch.Success)
						{
							if (dt.Columns.Contains(loopMatch.Groups[1].Value))
								tmpLoopString = tmpLoopString.Replace("{" + loopMatch.Groups[1].Value + "}", HttpUtility.HtmlEncode(dt.Rows[dti][loopMatch.Groups[1].Value].ToString()));

							loopMatch = loopMatch.NextMatch();
						}
						tmpLoopString = tmpLoopString.Replace("{DataBindLoopIndex0}", dti.ToString());
						tmpLoopString = tmpLoopString.Replace("{DataBindLoopIndex1}", (dti + 1).ToString());
						strLoop += tmpLoopString;
					}
					BodyHtml = BodyHtml.Replace("[[" + dt.TableName + ".Replace]]", strLoop);
				}
			}

			return BodyHtml;
		}

		public string GetLoopHtml(DataRow[] rows, string BodyHtml)
		{
			StringBuilder Html = new StringBuilder();

			foreach (DataRow row in rows)
			{
				Html.Append(GetLoopHtml(row, BodyHtml));
			}

			string ReturnHtml = Html.ToString();

			// Index 치환하기
			int Count = 0;
			Match match = Regex.Match(ReturnHtml, @"{(\w+)}");

			while (match.Success)
			{
				ReturnHtml = ReturnHtml.Replace("{DataBindLoopIndex0}", Count.ToString());
				ReturnHtml = ReturnHtml.Replace("{DataBindLoopIndex1}", (Count + 1).ToString());
				Count++;
				match = match.NextMatch();
			}

			return ReturnHtml;
		}

		public string GetLoopHtml(DataRow row, string BodyHtml)
		{
			Match match = Regex.Match(BodyHtml, @"{(\w+)}");

			while (match.Success)
			{
				if (row.Table.Columns.Contains(match.Groups[1].Value))
					BodyHtml = BodyHtml.Replace("{" + match.Groups[1].Value + "}", HttpUtility.HtmlEncode(row[match.Groups[1].Value].ToString()));
				match = match.NextMatch();
			}

			return BodyHtml;
		}

		public string GetSkin(ref string Bodies, string Dlm1, string Dlm2, string Rep)
		{
			string RtnVal = "";
			if (Bodies.IndexOf(Dlm1) >= 0 && Bodies.IndexOf(Dlm2) >= 0)
			{
				// 루프부분 찾기
				RtnVal = Bodies.Substring(Bodies.IndexOf(Dlm1), Bodies.IndexOf(Dlm2) - Bodies.IndexOf(Dlm1));

				// 본문에서 루프부분 제거
				Bodies = Bodies.Replace(RtnVal + Dlm2, Rep);

				// 루프부분에서 구분자 제거
				RtnVal = RtnVal.Replace(Dlm1, "");
				RtnVal = RtnVal.Replace(Dlm2, "");
			}

			return RtnVal;
		}

		public string FunctionHtml(string strBind, Object DataBind)
		{
			if (strBind.IndexOf("{? ") >= 0 && strBind.IndexOf("{/}") >= 0)
			{
				string IfHtml = GetSkin(ref strBind, "{? ", "{/}", "[[IfBind.Replace]]");

				while (IfHtml != "")
				{
					int SplitIndex = IfHtml.IndexOf("}");
					string strIf = IfHtml.Substring(0, SplitIndex);
					string strHtmlTotal = IfHtml.Substring(SplitIndex + 1, IfHtml.Length - (SplitIndex + 1));
					string strIfHtml = "";
					string strElseHtml = "";
					string printHtml = "";

					if (strHtmlTotal.IndexOf("{:}") > 0)
					{
						strIfHtml = IfHtml.Substring(SplitIndex + 1, IfHtml.IndexOf("{:}") - (strIf.Length + 1));
						strElseHtml = IfHtml.Substring(IfHtml.IndexOf("{:}") + 3, IfHtml.Length - (IfHtml.IndexOf("{:}") + 3));
					}
					else
					{
						strIfHtml = strHtmlTotal;
						strElseHtml = "";
					}

					if (CheckIfHtml(DataBind, strIf))
						printHtml = strIfHtml;
					else
						printHtml = strElseHtml;

					// 컨트롤 적용
					strBind = strBind.Replace("[[IfBind.Replace]]", printHtml);

					// 다음 If
					IfHtml = GetSkin(ref strBind, "{? ", "{/}", "[[IfBind.Replace]]");
				}
			}

			return strBind;
		}

		// If 구문에 따른 조건 체크
		public bool CheckIfHtml(Object Data, string strIf)
		{
			bool RtnValue = true;

			string type = Data.GetType().Name;

			switch (type)
			{
				case "DataTable":
					DataTable dt = (DataTable)Data;

					break;
				case "DataSet":
					DataSet ds = (DataSet)Data;
					break;

				case "DataRow":
					DataRow dr = (DataRow)Data;

					if (strIf.IndexOf("==") > 0)
					{
						string[] ifArr = Split(strIf, "==");
						string if1 = ifArr[0].Trim();
						string if2 = ifArr[1].Trim();

						if (dr[if1] == null)
							return false;

						Match match = Regex.Match(if2, @""".*""");

						if (match.Success)
						{
							if ("\"" + dr[if1].ToString() + "\"" == if2)
								RtnValue = true;
							else
								RtnValue = false;
						}
					}
					if (strIf.IndexOf("!=") > 0)
					{
						string[] ifArr = Split(strIf, "!=");
						string if1 = ifArr[0].Trim();
						string if2 = ifArr[1].Trim();

						if (dr[if1] == null)
							return false;

						Match match = Regex.Match(if2, @""".*""");

						if (match.Success)
						{
							if ("\"" + dr[if1].ToString() + "\"" != if2)
								RtnValue = true;
							else
								RtnValue = false;
						}
					}

					break;

				case "Hashtable":
					Hashtable hdata = (Hashtable)Data;

					if (strIf.IndexOf("==") > 0)
					{
						string[] ifArr = Split(strIf, "==");
						string if1 = ifArr[0].Trim();
						string if2 = ifArr[1].Trim();

						if (hdata[if1] == null)
							return false;

						Match match = Regex.Match(if2, @""".*""");

						if (match.Success)
						{
							if ("\"" + hdata[if1].ToString() + "\"" == if2)
								RtnValue = true;
							else
								RtnValue = false;
						}
					}
					if (strIf.IndexOf("!=") > 0)
					{
						string[] ifArr = Split(strIf, "!=");
						string if1 = ifArr[0].Trim();
						string if2 = ifArr[1].Trim();

						if (hdata[if1] == null)
							return false;

						Match match = Regex.Match(if2, @""".*""");

						if (match.Success)
						{
							if ("\"" + hdata[if1].ToString() + "\"" != if2)
								RtnValue = true;
							else
								RtnValue = false;
						}
					}

					break;
			}

			return RtnValue;
		}

		/// <summary>
		/// AddInfo : new {Param="", Method="", ContentType="", Accept=""}
		/// <para>Param : 키=값&amp;키=값</para>
		/// <para>Method : GET[기본]</para>
		/// <para>ContentType : application/x-www-form-urlencoded[기본]</para>
		/// <para>Accept : 결과형식 [application/json(기본) , application/xml]</para>
		/// <para>결과 : json (dynamic)[기본] / xml (XmlDocument)</para>
		/// </summary>
		/// <param name="Url">Http포함 URL</param>
		/// <param name="Token">GetToken 값</param>
		/// <param name="AddInfo">new {Param, Method, Accept, ContentType}</param>
		/// <returns>기본 json</returns>
		public object RequstExcute(string Url, string Token, object AddInfo = null)
		{
			return RequstExcute(Url, Token, "", AddInfo);
		}

		/// <summary>
		/// AddInfo : new {Param="", Method="", ContentType="", Accept=""}
		/// <para>Param : 키=값&amp;키=값</para>
		/// <para>Method : GET[기본]</para>
		/// <para>ContentType : application/x-www-form-urlencoded[기본]</para>
		/// <para>Accept : 결과형식 [application/json(기본) , application/xml]</para>
		/// <para>결과 : json (dynamic)[기본] / xml (XmlDocument)</para>
		/// </summary>
		/// <param name="Url">Http포함 URL</param>
		/// <param name="Token">GetToken 값</param>
		/// <param name="SendSiteName">요청하는 Site 이름</param>
		/// <param name="AddInfo">new {Param, Method, Accept, ContentType}</param>
		/// <returns>기본 json</returns>
		public object RequstExcute(string Url, string Token, string SendSiteName, object AddInfo = null)
		{
			string ContentType = "application/x-www-form-urlencoded";
			string Method = "GET";
			string Accept = "application/json";
			byte[] SendData = UTF8Encoding.UTF8.GetBytes("");

			if (AddInfo != null)
			{
				var InfoData = new RouteValueDictionary(AddInfo);

				if (InfoData.ContainsKey("Method"))
					Method = InfoData["Method"].ToString();

				if (InfoData.ContainsKey("ContentType"))
					ContentType = InfoData["ContentType"].ToString();

				if (InfoData.ContainsKey("Accept"))
					Accept = InfoData["Accept"].ToString();

				if (InfoData.ContainsKey("Param"))
					SendData = UTF8Encoding.UTF8.GetBytes(InfoData["Param"].ToString());
			}

			HttpWebRequest request;
			request = (HttpWebRequest)WebRequest.Create(Url);
			request.ContentType = ContentType;
			request.Method = Method;
			request.ContentLength = SendData.Length;
			request.Headers.Add(m_AuthToken, Token);
			request.Timeout = 1 * 60 * 60 * 1000; //1시간, 30초(30*1000) 2023.11.10
			if (SendSiteName != "")
				request.Headers.Add(m_SendSiteName, SendSiteName);

			if (Accept != "")
				request.Accept = Accept;

			if (Method.ToUpper() != "GET")
			{
				using (Stream requestStream = request.GetRequestStream())
				{
					requestStream.Write(SendData, 0, SendData.Length);
				}
			}

			using (HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse())
			{
				using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
				{
					if (Accept.ToLower() == "application/xml" || Accept.ToLower() == "text/xml")
					{
						XmlDocument xd = new XmlDocument();
						xd.LoadXml(streamReader.ReadToEnd());
						return xd;
					}
					else if (Accept.ToLower() == "text/plain" || Accept.ToLower() == "text/html")
					{
						return streamReader.ReadToEnd();
					}
					else
					{
						return JsonConvert.DeserializeObject<dynamic>(streamReader.ReadToEnd());
					}
				}
			}
		}

		/// <summary>
		/// 요일을 숫자로 변환
		/// DayOfWeek 함수 사용시 0 : 일요일, 1 : 월요일
		/// 이를 편하게 사용하기 위해서 사용 (인덱스 비교 목적)
		/// 0 : 월요일, 1 : 화요일, 2 : 수요일 ~
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public int WeekDayToInt(DateTime dt, bool mondayStart = true)
		{
			if (mondayStart)
			{
				int weekDay = (int)(dt.DayOfWeek + 6) % 7;

				return weekDay;
			}
			else
			{
				return (int)dt.DayOfWeek;
			}
		}

		/// <summary>
		/// Reverse String
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public string ReverseString(string text)
		{
			char[] charArray = text.ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);
		}

		/// <summary>
		/// string 에서 원하는 text 찾는 함수, 2021-08-12 이규하
		/// </summary>
		/// <param name="strSource">원문</param>
		/// <param name="strStart">시작점</param>
		/// <param name="strEnd">끝 지점</param>
		/// <returns></returns>
		public string GetBetween(string strSource, string strStart, string strEnd)
		{
			if (strSource.Contains(strStart) && strSource.Contains(strEnd))
			{
				int Start, End;
				Start = strSource.IndexOf(strStart, 0) + strStart.Length;
				End = strSource.IndexOf(strEnd, Start);
				return strSource.Substring(Start, End - Start);
			}

			return "";
		}

		/// <summary>
		/// 날짜가 두 날짜 사이에 있는지 확인
		/// </summary>
		/// <param name="input">날짜</param>
		/// <param name="start">시작 날짜</param>
		/// <param name="end">끝 날짜</param>
		/// <returns></returns>
		public bool InBetweenDates(DateTime input, DateTime dtStart, DateTime dtEnd)
		{
			if (input.CompareTo(dtStart) >= 0 && input.CompareTo(dtEnd) <= 0)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// AddInfo : new {Param="", Method="", ContentType="", Accept=""}
		/// <para>Param : 키=값&amp;키=값</para>
		/// <para>Method : GET[기본]</para>
		/// <para>ContentType : application/x-www-form-urlencoded[기본]</para>
		/// <para>Accept : 결과형식 [application/json(기본) , application/xml]</para>
		/// <para>결과 : json (dynamic)[기본] / xml (XmlDocument)</para>
		/// </summary>
		/// <param name="Url">Http포함 URL</param>
		/// <param name="AddInfo">new {Param, Method, Accept, ContentType}</param>
		/// <param name="HeaderInfo">헤더에 추가 할 정보</param>
		/// <returns>기본 json</returns>
		public object RequstExcute(string Url, object AddInfo = null, object HeaderInfo = null)
		{
			string ContentType = "application/x-www-form-urlencoded";
			string Method = "GET";
			string Accept = "application/json";
			int Timeout = 0;
			bool SecurityProtocol = true;
			byte[] SendData = UTF8Encoding.UTF8.GetBytes("");

			if (AddInfo != null)
			{
				var InfoData = new RouteValueDictionary(AddInfo);

				if (InfoData.ContainsKey("Method"))
					Method = InfoData["Method"].ToString();

				if (InfoData.ContainsKey("ContentType"))
					ContentType = InfoData["ContentType"].ToString();

				if (InfoData.ContainsKey("Accept"))
					Accept = InfoData["Accept"].ToString();

				if (InfoData.ContainsKey("Param"))
					SendData = UTF8Encoding.UTF8.GetBytes(InfoData["Param"].ToString());

				if (InfoData.ContainsKey("SecurityProtocol"))
					SecurityProtocol = Convert.ObjToBool(InfoData["SecurityProtocol"].ToString());

				if (InfoData.ContainsKey("Timeout"))
					Timeout = Convert.ObjToInt(InfoData["Timeout"].ToString());
			}

			if (SecurityProtocol == true)
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			}

			HttpWebRequest request;
			request = (HttpWebRequest)WebRequest.Create(Url);
			request.ContentType = ContentType;
			request.Method = Method;
			request.ContentLength = SendData.Length;
			if (Timeout > 0)
			{
				request.Timeout = Timeout;
			}

			if (HeaderInfo != null)
			{
				var HeaderData = new RouteValueDictionary(HeaderInfo);

				foreach (var Header in HeaderData)
				{
					if (Header.Value != null && Header.Value.ToString() != "")
						request.Headers.Add(Header.Key, Header.Value.ToString());
				}
			}

			if (Accept != "")
				request.Accept = Accept;

			if (Method.ToUpper() != "GET")
			{
				using (Stream requestStream = request.GetRequestStream())
				{
					requestStream.Write(SendData, 0, SendData.Length);
				}
			}

			using (HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse())
			{
				using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8))
				{
					if (Accept.ToLower() == "application/xml" || Accept.ToLower() == "text/xml")
					{
						try
						{
							XmlDocument xd = new XmlDocument();
							xd.LoadXml(streamReader.ReadToEnd());
							return xd;
						}
						catch
						{
							return streamReader.ReadToEnd();
						}
					}
					else if (Accept.ToLower() == "text/plain" || Accept.ToLower() == "text/html")
					{
						return streamReader.ReadToEnd();
					}
					else
					{
						try
						{
							return JsonConvert.DeserializeObject<dynamic>(streamReader.ReadToEnd());
						}
						catch
						{
							return streamReader.ReadToEnd();
						}
					}
				}
			}
		}

		/// <summary>
		/// 요일정보 리턴
		/// </summary>
		/// <param name="dateTime"></param>
		/// <param name="RtnType"></param>
		/// <returns></returns>
		public string GetDayOfWeekStr(DateTime dateTime, string RtnType = "")
		{
			var day = dateTime.DayOfWeek;
			string week = string.Empty;
			switch (day)
			{
				case DayOfWeek.Monday:
					week = "월";
					break;
				case DayOfWeek.Tuesday:
					week = "화";
					break;
				case DayOfWeek.Wednesday:
					week = "수";
					break;
				case DayOfWeek.Thursday:
					week = "목";
					break;
				case DayOfWeek.Friday:
					week = "금";
					break;
				case DayOfWeek.Saturday:
					week = "토";
					break;
				case DayOfWeek.Sunday:
					week = "일";
					break;
				default:
					break;
			}

			return week;

		}

		/// <summary>
		/// DateTime 형변환 시 마지막 날짜 체크
		/// </summary>
		/// <param name="sDate">yyyy.MM.dd</param>
		/// <returns>true/false</returns>
		public bool LastDayCheck(string sDate)
		{
			// 해당 날자의 말일
			DateTime dBaseDay = DateTime.Parse(sDate.Substring(0, 7) + ".01");
			dBaseDay = dBaseDay.AddMonths(1).AddDays(-1);
			int nBaseLastDay = dBaseDay.Day;

			// 해당 날자의 일 부분
			string[] aChkDate = sDate.Split('.');
			int nChkDay = int.Parse(aChkDate[2]);

			// 해당 날자의 일이 말일보다 크면 false
			if (nChkDay > nBaseLastDay)
				return false;
			else
				return true;
		}

		/// <summary>
		/// 말일 날짜 구하기
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public DateTime GetMonthLastDay(DateTime dt)
		{
			DateTime dtFirstDay = DateTime.Parse(dt.ToString("yyyy.MM.01"));

			return dtFirstDay.AddMonths(1).AddDays(-1);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public DateTime GetMonthLastDay(string dt)
		{
			if (string.IsNullOrEmpty(dt))
			{
				throw new Exception("");
			}

			DateTime dtParse = DateTime.Parse(dt);

			return GetMonthLastDay(dtParse);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sBaseMonth">기준월 [2014.01]</param>
		/// <param name="nMonthDiff">이번달말=1, 다음달=2, 이전달 = 0</param>
		/// <returns></returns>
		public string GetLastMonthDate(string sBaseMonth, int nMonthDiff = 1)
		{
			DateTime dtDate = new DateTime();

			string DateStr = sBaseMonth + ".01";

			if (!DateTime.TryParse(DateStr, out dtDate))
			{
				throw new FormatException("날짜 형식이 올바르지 않습니다.");
			}

			DateTime LastDate = dtDate.AddMonths(nMonthDiff).AddDays(-1);

			return LastDate.ToString("yyyy.MM.dd");
		}

		/// <summary>
		///	시작 일과 종료일 사이의 날짜 계산
		/// </summary>
		/// <param name="startDate">시작 일자</param>
		/// <param name="endDate">종료 일자</param>
		/// <param name="isInclude">시작일 카운트에 포함여부(생략 시 기본 포함)</param>
		/// <returns>[0]:월, [1]:일</returns>
		public int[] GetDateDiff(DateTime startDate, DateTime endDate, bool isInclude = true)
		{
			int[] nReturn = new int[2];
			nReturn[0] = 0;
			nReturn[1] = 0;

			DateTime fromDate = startDate;
			DateTime toDate = endDate;

			if (fromDate.CompareTo(toDate) > 0)
			{
				throw new Exception("시작일이 종료일 보다 큽니다.");
			}

			// 시작일 월의 말일
			DateTime calcMonthLastDate = GetMonthLastDay(startDate);

			if (startDate.ToString("yyyy.MM") == endDate.ToString("yyyy.MM") &&
				startDate.Day == 1 && endDate.Day == calcMonthLastDate.Day)
			{
				// 시작일과 종료일이 같은 월이고 시작일과 종료일이 1일과 말일이면 한 달 요금으로 계산한다.
				nReturn[0] = 1;
				nReturn[1] = 0;
			}
			else
			{
				if (isInclude)
				{
					// 시작일도 카운트에 포함 되도록 설정
					toDate = toDate.AddDays(1);
				}

				int diffMonth = 0;
				int diffDay = 0;

				// 달 수를 구한다.
				diffMonth = ((toDate.Year - fromDate.Year) * 12) + (toDate.Month - fromDate.Month);

				if (diffMonth > 0)
				{
					// 한달이 넘는 다면 
					if (fromDate.AddMonths(diffMonth) > toDate)
					{
						diffMonth -= 1;
					}

					diffDay = (int)((toDate - fromDate.AddMonths(diffMonth)).TotalDays);
				}
				else
				{
					// 한달 이내의 기간이면 일 수만 반환한다.
					diffDay = (int)((toDate - fromDate).TotalDays);
				}

				nReturn[0] = diffMonth;
				nReturn[1] = diffDay;
			}

			return nReturn;
		}

		/// <summary>
		/// From ~ To 기간 계산.
		/// </summary>
		/// <returns>일수</returns>
		public int GetPeriodDayCnt(DateTime dTmpFromDate, DateTime dTmpToDate)
		{
			int nReturn = 0;

			try
			{
				DateTime dFromDate = dTmpFromDate.Date;
				DateTime dToDate = dTmpToDate.Date;

				int nDateDiff_Day = (int)DateAndTime.DateDiff(DateInterval.Day, dFromDate, dToDate);

				nReturn = nDateDiff_Day;
			}
			catch
			{
			}

			return nReturn;
		}

		/// <summary>
		/// 양력데이터 음력데이터로 변환
		/// </summary>
		/// <returns>yyyyMMdd</returns>
		public string SolaToLuna(string solaDate)
		{
			// 음력 데이터 (평달 - 작은달 :1,  큰달:2 )
			// (윤달이 있는 달 - 평달이 작고 윤달도 작으면 :3 , 평달이 작고 윤달이 크면 : 4)
			// (윤달이 있는 달 - 평달이 크고 윤달이 작으면 :5,  평달과 윤달이 모두 크면 : 6)
			var kk = new int[,]
			{{1, 2, 4, 1, 1, 2, 1, 2, 1, 2, 2, 1 },   /* 1841 */
            {2, 2, 1, 2, 1, 1, 2, 1, 2, 1, 2, 1},
			{2, 2, 2, 1, 2, 1, 4, 1, 2, 1, 2, 1},
			{2, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2},
			{1, 2, 1, 2, 2, 1, 2, 1, 2, 1, 2, 1},
			{2, 1, 2, 1, 5, 2, 1, 2, 2, 1, 2, 1},
			{2, 1, 1, 2, 1, 2, 1, 2, 2, 2, 1, 2},
			{1, 2, 1, 1, 2, 1, 2, 1, 2, 2, 2, 1},
			{2, 1, 2, 3, 2, 1, 2, 1, 2, 1, 2, 2},
			{2, 1, 2, 1, 1, 2, 1, 1, 2, 2, 1, 2},
			{2, 2, 1, 2, 1, 1, 2, 1, 2, 1, 5, 2},   /* 1851 */
            {2, 1, 2, 2, 1, 1, 2, 1, 2, 1, 1, 2},
			{2, 1, 2, 2, 1, 2, 1, 2, 1, 2, 1, 2},
			{1, 2, 1, 2, 1, 2, 5, 2, 1, 2, 1, 2},
			{1, 1, 2, 1, 2, 2, 1, 2, 2, 1, 2, 1},
			{2, 1, 1, 2, 1, 2, 1, 2, 2, 2, 1, 2},
			{1, 2, 1, 1, 5, 2, 1, 2, 1, 2, 2, 2},
			{1, 2, 1, 1, 2, 1, 1, 2, 2, 1, 2, 2},
			{2, 1, 2, 1, 1, 2, 1, 1, 2, 1, 2, 2},
			{2, 1, 6, 1, 1, 2, 1, 1, 2, 1, 2, 2},
			{1, 2, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2},   /* 1861 */
            {2, 1, 2, 1, 2, 2, 1, 2, 2, 3, 1, 2},
			{1, 2, 2, 1, 2, 1, 2, 2, 1, 2, 1, 2},
			{1, 1, 2, 1, 2, 1, 2, 2, 1, 2, 2, 1},
			{2, 1, 1, 2, 4, 1, 2, 2, 1, 2, 2, 1},
			{2, 1, 1, 2, 1, 1, 2, 2, 1, 2, 2, 2},
			{1, 2, 1, 1, 2, 1, 1, 2, 1, 2, 2, 2},
			{1, 2, 2, 3, 2, 1, 1, 2, 1, 2, 2, 1},
			{2, 2, 2, 1, 1, 2, 1, 1, 2, 1, 2, 1},
			{2, 2, 2, 1, 2, 1, 2, 1, 1, 5, 2, 1},
			{2, 2, 1, 2, 2, 1, 2, 1, 2, 1, 1, 2},   /* 1871 */
            {1, 2, 1, 2, 2, 1, 2, 1, 2, 2, 1, 2},
			{1, 1, 2, 1, 2, 4, 2, 1, 2, 2, 1, 2},
			{1, 1, 2, 1, 2, 1, 2, 1, 2, 2, 2, 1},
			{2, 1, 1, 2, 1, 1, 2, 1, 2, 2, 2, 1},
			{2, 2, 1, 1, 5, 1, 2, 1, 2, 2, 1, 2},
			{2, 2, 1, 1, 2, 1, 1, 2, 1, 2, 1, 2},
			{2, 2, 1, 2, 1, 2, 1, 1, 2, 1, 2, 1},
			{2, 2, 4, 2, 1, 2, 1, 1, 2, 1, 2, 1},
			{2, 1, 2, 2, 1, 2, 2, 1, 2, 1, 1, 2},
			{1, 2, 1, 2, 1, 2, 5, 2, 2, 1, 2, 1},   /* 1881 */
            {1, 2, 1, 2, 1, 2, 1, 2, 2, 1, 2, 2},
			{1, 1, 2, 1, 1, 2, 1, 2, 2, 2, 1, 2},
			{2, 1, 1, 2, 3, 2, 1, 2, 2, 1, 2, 2},
			{2, 1, 1, 2, 1, 1, 2, 1, 2, 1, 2, 2},
			{2, 1, 2, 1, 2, 1, 1, 2, 1, 2, 1, 2},
			{2, 2, 1, 5, 2, 1, 1, 2, 1, 2, 1, 2},
			{2, 1, 2, 2, 1, 2, 1, 1, 2, 1, 2, 1},
			{2, 1, 2, 2, 1, 2, 1, 2, 1, 2, 1, 2},
			{1, 5, 2, 1, 2, 2, 1, 2, 1, 2, 1, 2},
			{1, 2, 1, 2, 1, 2, 1, 2, 2, 1, 2, 2},   /* 1891 */
            {1, 1, 2, 1, 1, 5, 2, 2, 1, 2, 2, 2},
			{1, 1, 2, 1, 1, 2, 1, 2, 1, 2, 2, 2},
			{1, 2, 1, 2, 1, 1, 2, 1, 2, 1, 2, 2},
			{2, 1, 2, 1, 5, 1, 2, 1, 2, 1, 2, 1},
			{2, 2, 2, 1, 2, 1, 1, 2, 1, 2, 1, 2},
			{1, 2, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1},
			{2, 1, 5, 2, 2, 1, 2, 1, 2, 1, 2, 1},
			{2, 1, 2, 1, 2, 1, 2, 2, 1, 2, 1, 2},
			{1, 2, 1, 1, 2, 1, 2, 5, 2, 2, 1, 2},
			{1, 2, 1, 1, 2, 1, 2, 1, 2, 2, 2, 1},   /* 1901 */
            {2, 1, 2, 1, 1, 2, 1, 2, 1, 2, 2, 2},
			{1, 2, 1, 2, 3, 2, 1, 1, 2, 2, 1, 2},
			{2, 2, 1, 2, 1, 1, 2, 1, 1, 2, 2, 1},
			{2, 2, 1, 2, 2, 1, 1, 2, 1, 2, 1, 2},
			{1, 2, 2, 4, 1, 2, 1, 2, 1, 2, 1, 2},
			{1, 2, 1, 2, 1, 2, 2, 1, 2, 1, 2, 1},
			{2, 1, 1, 2, 2, 1, 2, 1, 2, 2, 1, 2},
			{1, 5, 1, 2, 1, 2, 1, 2, 2, 2, 1, 2},
			{1, 2, 1, 1, 2, 1, 2, 1, 2, 2, 2, 1},
			{2, 1, 2, 1, 1, 5, 1, 2, 2, 1, 2, 2},   /* 1911 */
            {2, 1, 2, 1, 1, 2, 1, 1, 2, 2, 1, 2},
			{2, 2, 1, 2, 1, 1, 2, 1, 1, 2, 1, 2},
			{2, 2, 1, 2, 5, 1, 2, 1, 2, 1, 1, 2},
			{2, 1, 2, 2, 1, 2, 1, 2, 1, 2, 1, 2},
			{1, 2, 1, 2, 1, 2, 2, 1, 2, 1, 2, 1},
			{2, 3, 2, 1, 2, 2, 1, 2, 2, 1, 2, 1},
			{2, 1, 1, 2, 1, 2, 1, 2, 2, 2, 1, 2},
			{1, 2, 1, 1, 2, 1, 5, 2, 2, 1, 2, 2},
			{1, 2, 1, 1, 2, 1, 1, 2, 2, 1, 2, 2},
			{2, 1, 2, 1, 1, 2, 1, 1, 2, 1, 2, 2},   /* 1921 */
            {2, 1, 2, 2, 3, 2, 1, 1, 2, 1, 2, 2},
			{1, 2, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2},
			{2, 1, 2, 1, 2, 2, 1, 2, 1, 2, 1, 1},
			{2, 1, 2, 5, 2, 1, 2, 2, 1, 2, 1, 2},
			{1, 1, 2, 1, 2, 1, 2, 2, 1, 2, 2, 1},
			{2, 1, 1, 2, 1, 2, 1, 2, 2, 1, 2, 2},
			{1, 5, 1, 2, 1, 1, 2, 2, 1, 2, 2, 2},
			{1, 2, 1, 1, 2, 1, 1, 2, 1, 2, 2, 2},
			{1, 2, 2, 1, 1, 5, 1, 2, 1, 2, 2, 1},
			{2, 2, 2, 1, 1, 2, 1, 1, 2, 1, 2, 1},   /* 1931 */
            {2, 2, 2, 1, 2, 1, 2, 1, 1, 2, 1, 2},
			{1, 2, 2, 1, 6, 1, 2, 1, 2, 1, 1, 2},
			{1, 2, 1, 2, 2, 1, 2, 2, 1, 2, 1, 2},
			{1, 1, 2, 1, 2, 1, 2, 2, 1, 2, 2, 1},
			{2, 1, 4, 1, 2, 1, 2, 1, 2, 2, 2, 1},
			{2, 1, 1, 2, 1, 1, 2, 1, 2, 2, 2, 1},
			{2, 2, 1, 1, 2, 1, 4, 1, 2, 2, 1, 2},
			{2, 2, 1, 1, 2, 1, 1, 2, 1, 2, 1, 2},
			{2, 2, 1, 2, 1, 2, 1, 1, 2, 1, 2, 1},
			{2, 2, 1, 2, 2, 4, 1, 1, 2, 1, 2, 1},   /* 1941 */
            {2, 1, 2, 2, 1, 2, 2, 1, 2, 1, 1, 2},
			{1, 2, 1, 2, 1, 2, 2, 1, 2, 2, 1, 2},
			{1, 1, 2, 4, 1, 2, 1, 2, 2, 1, 2, 2},
			{1, 1, 2, 1, 1, 2, 1, 2, 2, 2, 1, 2},
			{2, 1, 1, 2, 1, 1, 2, 1, 2, 2, 1, 2},
			{2, 5, 1, 2, 1, 1, 2, 1, 2, 1, 2, 2},
			{2, 1, 2, 1, 2, 1, 1, 2, 1, 2, 1, 2},
			{2, 2, 1, 2, 1, 2, 3, 2, 1, 2, 1, 2},
			{2, 1, 2, 2, 1, 2, 1, 1, 2, 1, 2, 1},
			{2, 1, 2, 2, 1, 2, 1, 2, 1, 2, 1, 2},   /* 1951 */
            {1, 2, 1, 2, 4, 2, 1, 2, 1, 2, 1, 2},
			{1, 2, 1, 1, 2, 2, 1, 2, 2, 1, 2, 2},
			{1, 1, 2, 1, 1, 2, 1, 2, 2, 1, 2, 2},
			{2, 1, 4, 1, 1, 2, 1, 2, 1, 2, 2, 2},
			{1, 2, 1, 2, 1, 1, 2, 1, 2, 1, 2, 2},
			{2, 1, 2, 1, 2, 1, 1, 5, 2, 1, 2, 2},
			{1, 2, 2, 1, 2, 1, 1, 2, 1, 2, 1, 2},
			{1, 2, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1},
			{2, 1, 2, 1, 2, 5, 2, 1, 2, 1, 2, 1},
			{2, 1, 2, 1, 2, 1, 2, 2, 1, 2, 1, 2},   /* 1961 */
            {1, 2, 1, 1, 2, 1, 2, 2, 1, 2, 2, 1},
			{2, 1, 2, 3, 2, 1, 2, 1, 2, 2, 2, 1},
			{2, 1, 2, 1, 1, 2, 1, 2, 1, 2, 2, 2},
			{1, 2, 1, 2, 1, 1, 2, 1, 1, 2, 2, 1},
			{2, 2, 5, 2, 1, 1, 2, 1, 1, 2, 2, 1},
			{2, 2, 1, 2, 2, 1, 1, 2, 1, 2, 1, 2},
			{1, 2, 2, 1, 2, 1, 5, 2, 1, 2, 1, 2},
			{1, 2, 1, 2, 1, 2, 2, 1, 2, 1, 2, 1},
			{2, 1, 1, 2, 2, 1, 2, 1, 2, 2, 1, 2},
			{1, 2, 1, 1, 5, 2, 1, 2, 2, 2, 1, 2},   /* 1971 */
            {1, 2, 1, 1, 2, 1, 2, 1, 2, 2, 2, 1},
			{2, 1, 2, 1, 1, 2, 1, 1, 2, 2, 2, 1},
			{2, 2, 1, 5, 1, 2, 1, 1, 2, 2, 1, 2},
			{2, 2, 1, 2, 1, 1, 2, 1, 1, 2, 1, 2},
			{2, 2, 1, 2, 1, 2, 1, 5, 2, 1, 1, 2},
			{2, 1, 2, 2, 1, 2, 1, 2, 1, 2, 1, 1},
			{2, 2, 1, 2, 1, 2, 2, 1, 2, 1, 2, 1},
			{2, 1, 1, 2, 1, 6, 1, 2, 2, 1, 2, 1},
			{2, 1, 1, 2, 1, 2, 1, 2, 2, 1, 2, 2},
			{1, 2, 1, 1, 2, 1, 1, 2, 2, 1, 2, 2},   /* 1981 */
            {2, 1, 2, 3, 2, 1, 1, 2, 2, 1, 2, 2},
			{2, 1, 2, 1, 1, 2, 1, 1, 2, 1, 2, 2},
			{2, 1, 2, 2, 1, 1, 2, 1, 1, 5, 2, 2},
			{1, 2, 2, 1, 2, 1, 2, 1, 1, 2, 1, 2},
			{1, 2, 2, 1, 2, 2, 1, 2, 1, 2, 1, 1},
			{2, 1, 2, 2, 1, 5, 2, 2, 1, 2, 1, 2},
			{1, 1, 2, 1, 2, 1, 2, 2, 1, 2, 2, 1},
			{2, 1, 1, 2, 1, 2, 1, 2, 2, 1, 2, 2},
			{1, 2, 1, 1, 5, 1, 2, 1, 2, 2, 2, 2},
			{1, 2, 1, 1, 2, 1, 1, 2, 1, 2, 2, 2},   /* 1991 */
            {1, 2, 2, 1, 1, 2, 1, 1, 2, 1, 2, 2},
			{1, 2, 5, 2, 1, 2, 1, 1, 2, 1, 2, 1},
			{2, 2, 2, 1, 2, 1, 2, 1, 1, 2, 1, 2},
			{1, 2, 2, 1, 2, 2, 1, 5, 2, 1, 1, 2},
			{1, 2, 1, 2, 2, 1, 2, 1, 2, 2, 1, 2},
			{1, 1, 2, 1, 2, 1, 2, 2, 1, 2, 2, 1},
			{2, 1, 1, 2, 3, 2, 2, 1, 2, 2, 2, 1},
			{2, 1, 1, 2, 1, 1, 2, 1, 2, 2, 2, 1},
			{2, 2, 1, 1, 2, 1, 1, 2, 1, 2, 2, 1},
			{2, 2, 2, 3, 2, 1, 1, 2, 1, 2, 1, 2},   /* 2001 */
            {2, 2, 1, 2, 1, 2, 1, 1, 2, 1, 2, 1},
			{2, 2, 1, 2, 2, 1, 2, 1, 1, 2, 1, 2},
			{1, 5, 2, 2, 1, 2, 1, 2, 2, 1, 1, 2},
			{1, 2, 1, 2, 1, 2, 2, 1, 2, 2, 1, 2},
			{1, 1, 2, 1, 2, 1, 5, 2, 2, 1, 2, 2},
			{1, 1, 2, 1, 1, 2, 1, 2, 2, 2, 1, 2},
			{2, 1, 1, 2, 1, 1, 2, 1, 2, 2, 1, 2},
			{2, 2, 1, 1, 5, 1, 2, 1, 2, 1, 2, 2},
			{2, 1, 2, 1, 2, 1, 1, 2, 1, 2, 1, 2},
			{2, 1, 2, 2, 1, 2, 1, 1, 2, 1, 2, 1},   /* 2011 */
            {2, 1, 6, 2, 1, 2, 1, 1, 2, 1, 2, 1},
			{2, 1, 2, 2, 1, 2, 1, 2, 1, 2, 1, 2},
			{1, 2, 1, 2, 1, 2, 1, 2, 5, 2, 1, 2},
			{1, 2, 1, 1, 2, 1, 2, 2, 2, 1, 2, 2},
			{1, 1, 2, 1, 1, 2, 1, 2, 2, 1, 2, 2},
			{2, 1, 1, 2, 3, 2, 1, 2, 1, 2, 2, 2},
			{1, 2, 1, 2, 1, 1, 2, 1, 2, 1, 2, 2},
			{2, 1, 2, 1, 2, 1, 1, 2, 1, 2, 1, 2},
			{2, 1, 2, 5, 2, 1, 1, 2, 1, 2, 1, 2},
			{1, 2, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1},   /* 2021 */
            {2, 1, 2, 1, 2, 2, 1, 2, 1, 2, 1, 2},
			{1, 5, 2, 1, 2, 1, 2, 2, 1, 2, 1, 2},
			{1, 2, 1, 1, 2, 1, 2, 2, 1, 2, 2, 1},
			{2, 1, 2, 1, 1, 5, 2, 1, 2, 2, 2, 1},
			{2, 1, 2, 1, 1, 2, 1, 2, 1, 2, 2, 2},
			{1, 2, 1, 2, 1, 1, 2, 1, 1, 2, 2, 2},
			{1, 2, 2, 1, 5, 1, 2, 1, 1, 2, 2, 1},
			{2, 2, 1, 2, 2, 1, 1, 2, 1, 1, 2, 2},
			{1, 2, 1, 2, 2, 1, 2, 1, 2, 1, 2, 1},
			{2, 1, 5, 2, 1, 2, 2, 1, 2, 1, 2, 1},   /* 2031 */
            {2, 1, 1, 2, 1, 2, 2, 1, 2, 2, 1, 2},
			{1, 2, 1, 1, 2, 1, 5, 2, 2, 2, 1, 2},
			{1, 2, 1, 1, 2, 1, 2, 1, 2, 2, 2, 1},
			{2, 1, 2, 1, 1, 2, 1, 1, 2, 2, 1, 2},
			{2, 2, 1, 2, 1, 4, 1, 1, 2, 1, 2, 2},
			{2, 2, 1, 2, 1, 1, 2, 1, 1, 2, 1, 2},
			{2, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 1},
			{2, 2, 1, 2, 5, 2, 1, 2, 1, 2, 1, 1},
			{2, 1, 2, 2, 1, 2, 2, 1, 2, 1, 2, 1},
			{2, 1, 1, 2, 1, 2, 2, 1, 2, 2, 1, 2},   /* 2041 */
            {1, 5, 1, 2, 1, 2, 1, 2, 2, 2, 1, 2},
			{1, 2, 1, 1, 2, 1, 1, 2, 2, 1, 2, 2}};

			if (solaDate.IndexOf(".") != -1 || solaDate.IndexOf("-") != -1 || solaDate.IndexOf(":") != -1)
			{
				solaDate = solaDate.Replace(".", "").Replace("-", "").Replace(":", "");
			}
			var input_day = solaDate;

			if (new Regex("[0-9]{8}").Match(input_day).Success == false)
			{
				return null;
			}

			var md = new int[] { 31, 0, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

			int year = int.Parse(Left(input_day, 4));
			int month = int.Parse(Mid(input_day, 4, 2));
			int day = int.Parse(Right(input_day, 2));
			// 음력에서 양력으로 변환
			string syear, smonth, sday;
			int mm = 29, y1, y2, m1, leapyes;
			int i, j;
			float td, y;

			if (year < 1841 || year > 2043 || month < 1 || month > 12)
			{
				return null;
			}

			y1 = year - 1841;
			m1 = month - 1;
			leapyes = 0;
			if (kk[y1, m1] > 2)
			{
				switch (kk[y1, m1])
				{
					case 2: mm = 30; break;
					case 5: mm = 30; break;
					case 6: mm = 30; break;
				}
			}

			if (day < 1 || day > mm)
			{
				return null;
			}

			td = 0;
			for (i = 0; i < y1; i++)
			{
				for (j = 0; j < 12; j++)
				{
					switch (kk[i, j])
					{
						case 1:
							td = td + 29;
							break;
						case 2:
							td = td + 30;
							break;
						case 3:
							td = td + 58;    // 29+29
							break;
						case 4:
							td = td + 59;    // 29+30
							break;
						case 5:
							td = td + 59;    // 30+29
							break;
						case 6:
							td = td + 60;    // 30+30
							break;
					}
				}
			}
			for (j = 0; j < m1; j++)
			{
				switch (kk[y1, j])
				{
					case 1:
						td = td + 29;
						break;
					case 2:
						td = td + 30;
						break;
					case 3:
						td = td + 58;    // 29+29
						break;
					case 4:
						td = td + 59;    // 29+30
						break;
					case 5:
						td = td + 59;    // 30+29
						break;
					case 6:
						td = td + 60;    // 30+30
						break;
				}
			}
			if (leapyes == 1)
			{
				switch (kk[y1, m1])
				{
					case 3:
					case 4:
						td = td + 29;
						break;
					case 5:
					case 6:
						td = td + 30;
						break;
				}
			}
			td = td + (float)day + 22;
			// td : 1841 년 1 월 1 일 부터 원하는 날짜까지의 전체 날수의 합
			y1 = 1840;
			do
			{
				y1 = y1 + 1;
				if ((y1 % 400 == 0) || ((y1 % 100 != 0) && (y1 % 4 == 0)))
				{
					y2 = 366;
				}
				else
				{
					y2 = 365;
				}
				if (td <= y2)
				{
					break;
				}
				else
				{
					td = td - y2;
				}
			} while (true);
			syear = "" + y1;
			md[1] = y2 - 337;
			m1 = 0;
			do
			{
				m1 = m1 + 1;
				if (td <= md[m1 - 1])
				{
					break;
				}
				else
				{
					td = td - md[m1 - 1];
				}
			} while (true);
			smonth = "" + m1;
			sday = "" + td;
			y = int.Parse(syear) - 1;
			td = y * 365 + y / 4 - y / 100 + y / 400;
			for (i = 0; i < int.Parse(smonth) - 1; i++)
			{
				td = td + md[i];
			}

			if (int.Parse(smonth) < 10)
			{
				smonth = "0" + smonth;
			}
			if (int.Parse(sday) < 10)
			{
				sday = "0" + sday;
			}
			return year + "." + smonth + "." + sday;
		}

		/// <summary>
		/// 소수점이 있는 Double 형식의 문자열을 long 형으로 변경한다.
		/// Convert 하면서 소수점 1자리에서 반올림 된다.
		/// </summary>
		/// <param name="doubleString"></param>
		/// <returns></returns>
		public long DoubleStrToLong(string doubleString)
		{
			double orgDouble = 0.0;
			if (!double.TryParse(doubleString, out orgDouble))
			{
				throw new InvalidCastException(doubleString + " 은 올라 숫자 형식이 아닙니다.");
			}

			return System.Convert.ToInt64(orgDouble);
		}

		public string EncodeURIComponent(string val)
		{
			return Microsoft.JScript.GlobalObject.encodeURIComponent(val);
		}

		public string DecodeURIComponent(string val)
		{
			return Microsoft.JScript.GlobalObject.decodeURIComponent(val);
		}
		#endregion

		#region -----------------Request---------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Names"></param>
		/// <returns></returns>
		public string GetRequest(string[] Names)
		{
			if (Names == null || Names.Length == 0)
				return "";

			string Rtn = "";
			for (int i = 0; i < Names.Length; i++)
			{
				string RequestValue = GetRequest(Names[i]);
				if (RequestValue != "")
					Rtn += "&" + Names[i] + "=" + RequestValue;
			}
			if (Rtn.Length > 0)
				return Rtn.Substring(1, Rtn.Length - 1);
			else
				return "";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Name"></param>
		/// <returns></returns>
		public string GetRequest(string Name)
		{
			return GetRequest(Name, "");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="DefaultValue"></param>
		/// <returns></returns>
		public string GetRequest(string Name, string DefaultValue)
		{
			string RtnValue = "";
			try
			{
				if (utilRequest.RequestType == "GET")
				{
					if (utilRequest[Name] != null)
						RtnValue = AntiXSS(utilRequest.QueryString[Name]);
				}
				else
				{
					if (utilRequest[Name] != null)
						RtnValue = AntiXSS(utilRequest.Form[Name]);
				}

				if (string.IsNullOrEmpty(RtnValue)) RtnValue = DefaultValue;
				return ReplaceScriptTag(RtnValue);
			}
			catch
			{
				return DefaultValue;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Names"></param>
		/// <returns></returns>
		public object ObjectGetRequest(object obj)
		{
			IList<PropertyInfo> properties = obj.GetType().GetProperties().ToList();

			foreach (var property in properties)
			{
				try
				{
					property.SetValue(obj, GetRequest(property.Name), null);
				}
				catch
				{
				}
			}

			return obj;
		}

		public string AntiXSS(string str)
		{
			string result = str;

			try
			{
				Dictionary<string, string> riskytag = new Dictionary<string, string>();
				riskytag.Add(@"%3C", @"&lt;");
				riskytag.Add(@"<", @"&lt;");
				riskytag.Add(@"%3E", @"&gt;");
				riskytag.Add(@">", @"&gt;");
				riskytag.Add(@"(", @"&#40;");
				riskytag.Add(@"%28", @"&#40;");
				riskytag.Add(@")", @"&#41;");
				riskytag.Add(@"%29", @"&#41;");
				riskytag.Add("%22", @"&quot;");
				riskytag.Add(@"'", @"&#x27;");

				if (!string.IsNullOrEmpty(result))
				{
					foreach (string key in riskytag.Keys)
					{
						result = result.Replace(key, riskytag[key]);
					}
				}

			}
			catch (Exception ex)
			{
				result = str;
			}

			return result;
		}
		#endregion

		#region -----------------Session/Cookie---------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string SessVal(string key)
		{
			string sVal = "";

			if (HttpContext.Current != null)
			{
				object sessionValue = HttpContext.Current.Session[key];

				if (sessionValue != null)
				{
					sVal = sessionValue.ToString();
				}
			}

			return sVal;
		}

		/// <summary>
		/// 쿠키 값
		/// </summary>
		/// <param name="Name">쿠키명</param>
		/// <param name="DefulatValue">기본값</param>
		/// <returns></returns>
		public string GetCookie(string Name, string DefulatValue = "")
		{
			string RtnValue = DefulatValue;
			if (utilRequest.Cookies[Name] != null)
			{
				RtnValue = utilRequest.Cookies[Name].Value;
			}

			return RtnValue;
		}

		/// <summary>
		/// 쿠키 저장
		/// </summary>
		/// <param name="Name">쿠키명</param>
		/// <param name="Value">값</param>
		/// <param name="Expires">저장기간(day)</param>
		public void SetCookie(string Name, string Value, int Expires)
		{
			utilResponse.Cookies[Name].Value = Value;
			utilResponse.Cookies[Name].Expires = DateTime.Now.AddDays(Expires);
		}
		#endregion

		#region AES 암호화/복호화 함수 (리눅스와 연동 가능)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sText"></param>
		/// <param name="sKey"></param>
		/// <returns></returns>
		public string AesEnc(string sText, string sKey = "dsxnDolsAyO1kCfauyrnqg==")
		{
			RijndaelManaged rm = new RijndaelManaged();
			rm.Padding = PaddingMode.Zeros;
			rm.Mode = CipherMode.ECB;
			rm.BlockSize = 128;
			rm.KeySize = 256;
			rm.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
			rm.GenerateIV();

			using (ICryptoTransform enc = rm.CreateEncryptor(rm.Key, rm.IV))
			{
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
					{
						using (StreamWriter sw = new StreamWriter(cs))
						{
							sw.Write(sText);
							sw.Close();
						}
					}

					return System.Convert.ToBase64String(ms.ToArray());
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sText"></param>
		/// <param name="sKey"></param>
		/// <returns></returns>
		public string AesDec(string sText, string sKey = "dsxnDolsAyO1kCfauyrnqg==")
		{
			byte[] bTxt = System.Convert.FromBase64String(sText);

			RijndaelManaged rm = new RijndaelManaged();
			rm.Padding = PaddingMode.Zeros;
			rm.Mode = CipherMode.ECB;
			rm.BlockSize = 128;
			rm.KeySize = 256;
			rm.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
			rm.GenerateIV();

			using (ICryptoTransform dec = rm.CreateDecryptor(rm.Key, rm.IV))
			{
				using (MemoryStream ms = new MemoryStream(bTxt))
				{
					using (CryptoStream cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
					{
						byte[] decByte = new byte[bTxt.Length];

						cs.Read(decByte, 0, decByte.Length);

						string sDecString = Encoding.UTF8.GetString(decByte);

						return sDecString.TrimEnd('\0');
					}
				}
			}
		}
		#endregion

		#region SHA
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sTxt"></param>
		/// <returns></returns>
		public string SHAEnc(string sTxt)
		{
			SHA512 sha = SHA512.Create();
			byte[] result = sha.ComputeHash(Encoding.UTF8.GetBytes(sTxt));

			string hash = "";
			hash = BitConverter.ToString(result).Replace("-", String.Empty);
			sha.Clear();

			return hash;
		}
		#endregion

		#region 유일키 만들기
		/// <summary>
		/// 유일키 만드는 함수
		/// </summary>
		/// <param name="a">키 생성에 사용할 문자열 (기본값은 영어 대문자와 숫자)</param>
		/// <param name="size">키의 길이</param>
		/// <returns></returns>
		public string UniqStringRNG(string a, int size)
		{
			if (size <= 0)
			{
				return string.Empty;
			}

			if (string.IsNullOrWhiteSpace(a))
			{
				a = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			}

			char[] chars = new char[a.Length];
			chars = a.ToCharArray();

			byte[] data = new byte[size];
			RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
			crypto.GetNonZeroBytes(data);

			StringBuilder result = new StringBuilder(size);
			foreach (byte b in data)
			{
				result.Append(chars[b % (chars.Length - 1)]);
			}
			return result.ToString();
		}
		#endregion

		#region 쿼리 가지고 오기 XML
		/// <summary>
		/// xml 쿼리 string get
		/// </summary>
		/// <param name="xmlFile"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public string GetQuery(string xmlFile, string id)
		{
			StringBuilder sQuery = new StringBuilder();
			string msXmlFilePath = m_DBXmlFilePath + "models.querys." + xmlFile + ".xml";

			try
			{
				//where 조건문을 사용해 봄, book 의 id 의 특정 id 접근하여 객체할당함
				XDocument document = XDocument.Load(msXmlFilePath);
				var select = from r in document.Descendants("select").Where(r => (string)r.Attribute("id") == id)
							 select new
							 {
								 query = r.Element("query").Value,
							 };

				foreach (var r in select)
				{
					sQuery.Append(r.query);
				}
			}
			catch
			{
				sQuery.Clear();
			}

			return sQuery.ToString();
		}
		#endregion

		#region 쿼리 실행 GetList
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="commandTimeout"></param>
		/// <returns></returns>
		public DataSet GetList(string sqlQuery, int commandTimeout = 600, bool logging = true)
		{
			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");

				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				Hashtable paramters = new Hashtable();
				using (DataSet ds = GetList(sqlQuery, paramters, commandTimeout, logging))
				{
					return ds;
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="param"></param>
		/// <param name="commandTimeout"></param>
		/// <returns></returns>
		public DataSet GetList(string sqlQuery, object param, int commandTimeout = 0, bool logging = true)
		{
			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");


				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				using (DataSet ds = DB.Execute(sqlQuery, param, commandTimeout, logging))
				{
					return ds;
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="param"></param>
		/// <param name="output"></param>
		/// <param name="commandTimeout"></param>
		/// <returns></returns>
		public DataSet GetList(string sqlQuery, object param, ref Hashtable output, int commandTimeout = 0, bool logging = true)
		{
			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");

				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				using (DataSet ds = DB.Execute(sqlQuery, param, ref output, commandTimeout, logging))
				{
					return ds;
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}
		#endregion

		#region 쿼리 실행 GetData
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="EmptNull"></param>
		/// <param name="commandTimeout"></param>
		/// <returns></returns>
		public Hashtable GetData(string sqlQuery, bool EmptNull = false, int commandTimeout = 600, bool logging = true)
		{
			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");

				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				Hashtable paramters = new Hashtable();
				return GetData(sqlQuery, paramters, EmptNull, commandTimeout, logging);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="param"></param>
		/// <param name="EmptNull"></param>
		/// <param name="commandTimeout"></param>
		/// <returns></returns>
		public Hashtable GetData(string sqlQuery, object param, bool EmptNull = false, int commandTimeout = 600, bool logging = true)
		{
			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");

				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				using (DataSet ds = DB.Execute(sqlQuery, param, commandTimeout, logging))
				{
					if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
					{
						if (EmptNull)
						{
							return null;
						}
						else
						{
							throw new Exception("결과 데이타가 없습니다.");
						}
					}

					Hashtable row = new Hashtable();

					for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
					{
						string colName = ds.Tables[0].Columns[i].ColumnName;
						row.Add(colName, ds.Tables[0].Rows[0][colName].ToString());
					}

					return row;
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="param"></param>
		/// <param name="output"></param>
		/// <param name="EmptNull"></param>
		/// <param name="commandTimeout"></param>
		/// <returns></returns>
		public Hashtable GetData(string sqlQuery, object param, ref Hashtable output, bool EmptNull = false, int commandTimeout = 600, bool logging = true)
		{
			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");

				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				using (DataSet ds = DB.Execute(sqlQuery, param, ref output, commandTimeout, logging))
				{
					if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
					{
						if (EmptNull)
						{
							return null;
						}
						else
						{
							throw new Exception("결과 데이타가 없습니다.");
						}
					}

					Hashtable row = new Hashtable();

					for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
					{
						string colName = ds.Tables[0].Columns[i].ColumnName;
						row.Add(colName, ds.Tables[0].Rows[0][colName].ToString());
					}

					return row;
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}
		#endregion

		#region 쿼리 실행 GetText
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="commandTimeout"></param>
		/// <returns></returns>
		public object GetText(string sqlQuery, int commandTimeout = 600, bool logging = true)
		{
			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");

				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				Hashtable paramters = new Hashtable();
				return GetText(sqlQuery, paramters, commandTimeout, logging);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="param"></param>
		/// <param name="commandTimeout"></param>
		/// <returns></returns>
		public object GetText(string sqlQuery, object param, int commandTimeout = 600, bool logging = true)
		{
			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");

				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				using (DataSet ds = DB.Execute(sqlQuery, param, commandTimeout, logging))
				{
					if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
						return "";

					if (ds.Tables[0].Columns[0].DataType.Name.ToLower() == "string")
					{
						return ds.Tables[0].Rows[0][0].ToString();
					}
					else if (ds.Tables[0].Columns[0].DataType.Name.ToLower() == "int32")
					{
						return Convert.ObjToInt(ds.Tables[0].Rows[0][0].ToString());
					}
					else if (ds.Tables[0].Columns[0].DataType.Name.ToLower() == "double")
					{
						return Convert.ObjToDouble(ds.Tables[0].Rows[0][0].ToString());
					}
					else if (ds.Tables[0].Columns[0].DataType.Name.ToLower() == "decimal")
					{
						return Convert.ObjToDecimal(ds.Tables[0].Rows[0][0].ToString());
					}
					else if (ds.Tables[0].Columns[0].DataType.Name.ToLower() == "dateTime")
					{
						if (ds.Tables[0].Rows[0][0].ToString() == "")
						{
							return (new DateTime());
						}
						else
						{
							return DateTime.Parse(ds.Tables[0].Rows[0][0].ToString());
						}
					}

					return ds.Tables[0].Rows[0][0].ToString();
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="param"></param>
		/// <param name="output"></param>
		/// <param name="commandTimeout"></param>
		/// <returns></returns>
		public object GetText(string sqlQuery, object param, ref Hashtable output, int commandTimeout = 600, bool logging = true)
		{
			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");

				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				using (DataSet ds = DB.Execute(sqlQuery, param, ref output, commandTimeout, logging))
				{
					if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
						return "";

					if (ds.Tables[0].Columns[0].DataType.Name.ToLower() == "string")
					{
						return ds.Tables[0].Rows[0][0].ToString();
					}
					else if (ds.Tables[0].Columns[0].DataType.Name.ToLower() == "int32")
					{
						return Convert.ObjToInt(ds.Tables[0].Rows[0][0].ToString());
					}
					else if (ds.Tables[0].Columns[0].DataType.Name.ToLower() == "double")
					{
						return Convert.ObjToDouble(ds.Tables[0].Rows[0][0].ToString());
					}
					else if (ds.Tables[0].Columns[0].DataType.Name.ToLower() == "decimal")
					{
						return Convert.ObjToDecimal(ds.Tables[0].Rows[0][0].ToString());
					}
					else if (ds.Tables[0].Columns[0].DataType.Name.ToLower() == "dateTime")
					{
						if (ds.Tables[0].Rows[0][0].ToString() == "")
						{
							return (new DateTime());
						}
						else
						{
							return DateTime.Parse(ds.Tables[0].Rows[0][0].ToString());
						}
					}

					return ds.Tables[0].Rows[0][0].ToString();
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}
		#endregion

		#region 쿼리 실행 Excute
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="commandTimeout"></param>
		public void Excute(string sqlQuery, int commandTimeout = 600, bool logging = true)
		{
			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");

				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				Hashtable paramters = new Hashtable();
				//Excute(sqlQuery, paramters, commandTimeout);
				DB.ExecuteNonQuery(sqlQuery, paramters, commandTimeout, logging);

			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="param"></param>
		/// <param name="commandTimeout"></param>
		public void Excute(string sqlQuery, object param, int commandTimeout = 600, bool logging = true)
		{

			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");

				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				DB.ExecuteNonQuery(sqlQuery, param, commandTimeout, logging);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="param"></param>
		/// <param name="output"></param>
		/// <param name="commandTimeout"></param>
		public void Excute(string sqlQuery, object param, ref Hashtable output, int commandTimeout = 600, bool logging = true)
		{
			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");

				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				DB.ExecuteNonQuery(sqlQuery, param, ref output, commandTimeout, logging);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="commandTimeout"></param>
		public int ExcuteReturnRows(string sqlQuery, int commandTimeout = 600, bool logging = true)
		{
			int affectedRows = 0;

			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");

				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				Hashtable paramters = new Hashtable();
				//Excute(sqlQuery, paramters, commandTimeout);
				affectedRows = DB.ExecuteNonQuery(sqlQuery, paramters, commandTimeout, logging);

				return affectedRows;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlQuery"></param>
		/// <param name="param"></param>
		/// <param name="commandTimeout"></param>
		public int ExcuteReturnRows(string sqlQuery, object param, int commandTimeout = 600, bool logging = true)
		{
			int affectedRows = 0;

			try
			{
				if (IsValue(sqlQuery) == false) throw new Exception("쿼리를 확인해주세요.");

				// DB에 Xml파일 경로 값이 있는 경우 file.query 패턴인지 체크
				if (m_DBXmlFilePath != "")
				{
					int splitNumber = sqlQuery.IndexOf('.');
					string fileName = Left(sqlQuery, splitNumber);
					string queryName = Mid(sqlQuery, splitNumber + 1);

					string tmpSqlQuery = GetQuery(fileName, queryName);
					if (IsValue(tmpSqlQuery))
					{
						sqlQuery = tmpSqlQuery;
					}
				}

				affectedRows = DB.ExecuteNonQuery(sqlQuery, param, commandTimeout, logging);

				return affectedRows;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message.ToString());
			}
		}
		#endregion

		#region --------------------------Site-----------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="splitLen"></param>
		/// <param name="sep"></param>
		/// <returns></returns>
		public string GetIndexToID(string s, int splitLen, string sep)
		{
			List<string> lst = new List<string>();

			string indexID = "";

			int len = s.Length;

			if (len > 0 && splitLen > 0)
			{
				for (int i = 0; i < len; i += splitLen)
				{
					int cutLen = 0;

					if ((i + splitLen) < len)
					{
						cutLen = splitLen;
					}
					else
					{
						cutLen = len - i;
					}

					lst.Add(s.Substring(i, cutLen));
				}

				indexID = string.Join(sep, lst.ToArray());
			}

			return indexID;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dic"></param>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public string DicVal(Dictionary<string, string> dic, string key, string defaultValue = "")
		{
			string val = "";

			if (dic != null)
			{
				val = dic.ContainsKey(key) ? dic[key] : "";
			}

			// 파라미터에 값이 없는데 기본값이 있는 경우 기본값으로 반환 한다.
			if (string.IsNullOrWhiteSpace(val) && !string.IsNullOrWhiteSpace(defaultValue))
			{
				val = defaultValue;
			}

			return val;
		}

		/// <summary>
		/// 구분자(,;)로 구분된 문자열의 첫번째 값을 가지고 온다.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public string GetOneMultiString(string str)
		{
			string one = "";

			str = str.Replace(";", ",");

			if (str.IndexOf(',') > -1)
			{
				string[] arrMngdUrl = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				one = arrMngdUrl[0].Trim();
			}
			else
			{
				one = str;
			}

			return one;
		}

		/// <summary>
		/// 전화번호 배열을 문자열로 반환한다.
		/// </summary>
		/// <param name="telNos"></param>
		/// <returns>xxx-xxxx-xxxx</returns>
		public string TelNoToString(List<string> telNos)
		{
			var tel = new List<string>();

			foreach (string item in telNos)
			{
				if (!string.IsNullOrWhiteSpace(item))
				{
					tel.Add(item.Trim());
				}
			}

			return string.Join("-", tel.ToArray());
		}

		/// <summary>
		/// 회원 암호 체크룰
		/// </summary>
		/// <param name="sStr"></param>
		/// <returns></returns>
		public bool RegExPassword(string pwd)
		{
			string exp = @"^.*(?=.{8,50})(((?=.*[a-zA-Z])(?=.*\d))|((?=.*\d)(?=.*\W))|((?=.*[a-zA-Z])(?=.*\W))|((?=.*[a-zA-Z])(?=.*\d)(?=.*\W)))(?!.*[\s]).*$";

			Regex re = new Regex(exp, RegexOptions.IgnorePatternWhitespace);

			Match mc = re.Match(pwd);

			if (mc.Success)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 사업자번호 변경
		/// </summary>
		/// <param name="telNos"></param>
		/// <returns>xxx-xxxx-xxxx</returns>
		public string RegNoToString(string val, bool Rev = false)
		{
			string RtnValue = "";

			if (val != "")
			{
				val = val.Replace("-", "");
				string tmpValue = val;

				if (tmpValue.Length == 10)
					RtnValue = tmpValue.Substring(0, 3) + "-" + tmpValue.Substring(3, 2) + "-" + tmpValue.Substring(5, 5);
				else
					RtnValue = tmpValue;
			}

			return RtnValue;
		}
		/// <summary>
		/// 사업자번호 유효성 체크
		/// </summary>
		/// <param name="RegNo"></param>
		/// <returns></returns>
		public bool ValidateRegNo(string regNo)
		{
			regNo = regNo.Replace("_", "");
			if (regNo.Length != 10)
			{
				return false;
			}

			int sum = 0;
			string keyString = "137137135";

			for (int i = 0; i < keyString.Length; i++)
			{
				sum += (int)Char.GetNumericValue(regNo[i]) * (int)Char.GetNumericValue(keyString[i]);
			}

			sum += (int)Char.GetNumericValue(regNo[8]) * 5 / 10;

			sum %= 10;
			if (sum != 0)
				sum = 10 - sum;

			if (sum != (int)Char.GetNumericValue(regNo[9]))
				return false;
			else
				return true;
		}
		#endregion

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
					if (m_Convert != null)
						m_Convert.Dispose(); m_Convert = null;

					if (m_File != null)
						m_File = null;

					if (m_Db != null)
						m_Db.Dispose(); m_Db = null;
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


		#region --------------------------PDF 파일 Text파일 추출-----------------------------
		public string ExtractTextFromPdf(string path, string pass = "")
		{
			ITextExtractionStrategy its = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();

			if (pass != "")
			{
				ASCIIEncoding encodedData = new ASCIIEncoding();
				byte[] postParam = encodedData.GetBytes(pass);

				using (PdfReader reader = new PdfReader(path, postParam))
				{
					StringBuilder text = new StringBuilder();

					for (int i = 1; i <= reader.NumberOfPages; i++)
					{
						string thePage = PdfTextExtractor.GetTextFromPage(reader, i, its);
						string[] theLines = thePage.Split('\n');
						foreach (var theLine in theLines)
						{
							text.AppendLine(theLine);
						}
					}
					return text.ToString();
				}
			}
			else
			{
				using (PdfReader reader = new PdfReader(path))
				{
					StringBuilder text = new StringBuilder();

					for (int i = 1; i <= reader.NumberOfPages; i++)
					{
						string thePage = PdfTextExtractor.GetTextFromPage(reader, i, its);
						string[] theLines = thePage.Split('\n');
						foreach (var theLine in theLines)
						{
							text.AppendLine(theLine);
						}
					}
					return text.ToString();
				}
			}
		}

		public void PdfMerge(string mFILE, string sFILE = "", string sFILE2 = "", string sFILE3 = "", string sFILE4 = "", string sFILE5 = "", string sFILE6 = "", string sFILE7 = "", string sFILE8 = "", string sFILE9 = "", string sFILE10 = "")
		{
			//두 PDF 합치기
			Document document = new Document();
			PdfCopy copy = new PdfCopy(document, new FileStream(mFILE, FileMode.Create));
			document.Open();
			PdfReader reader1;

			try
			{
				if (sFILE != "")
				{
					//파일1의 모든 페이지를 새 파일에 추가한다.
					reader1 = new PdfReader(sFILE);
					for (int i = 1; i <= reader1.NumberOfPages; i++)
					{
						copy.AddPage(copy.GetImportedPage(reader1, i));
					}
				}

				if (sFILE2 != "")
				{
					//파일2의 모든 페이지를 새 파일에 추가한다.
					reader1 = new PdfReader(sFILE2);
					for (int i = 1; i <= reader1.NumberOfPages; i++)
					{
						copy.AddPage(copy.GetImportedPage(reader1, i));
					}
				}

				if (sFILE3 != "")
				{
					//파일2의 모든 페이지를 새 파일에 추가한다.
					reader1 = new PdfReader(sFILE3);
					for (int i = 1; i <= reader1.NumberOfPages; i++)
					{
						copy.AddPage(copy.GetImportedPage(reader1, i));
					}
				}

				if (sFILE4 != "")
				{
					//파일2의 모든 페이지를 새 파일에 추가한다.
					reader1 = new PdfReader(sFILE4);
					for (int i = 1; i <= reader1.NumberOfPages; i++)
					{
						copy.AddPage(copy.GetImportedPage(reader1, i));
					}
				}

				if (sFILE5 != "")
				{
					//파일2의 모든 페이지를 새 파일에 추가한다.
					reader1 = new PdfReader(sFILE5);
					for (int i = 1; i <= reader1.NumberOfPages; i++)
					{
						copy.AddPage(copy.GetImportedPage(reader1, i));
					}
				}

				if (sFILE6 != "")
				{
					//파일2의 모든 페이지를 새 파일에 추가한다.
					reader1 = new PdfReader(sFILE6);
					for (int i = 1; i <= reader1.NumberOfPages; i++)
					{
						copy.AddPage(copy.GetImportedPage(reader1, i));
					}
				}

				if (sFILE7 != "")
				{
					//파일2의 모든 페이지를 새 파일에 추가한다.
					reader1 = new PdfReader(sFILE7);
					for (int i = 1; i <= reader1.NumberOfPages; i++)
					{
						copy.AddPage(copy.GetImportedPage(reader1, i));
					}
				}

				if (sFILE8 != "")
				{
					//파일2의 모든 페이지를 새 파일에 추가한다.
					reader1 = new PdfReader(sFILE8);
					for (int i = 1; i <= reader1.NumberOfPages; i++)
					{
						copy.AddPage(copy.GetImportedPage(reader1, i));
					}
				}

				if (sFILE9 != "")
				{
					//파일2의 모든 페이지를 새 파일에 추가한다.
					reader1 = new PdfReader(sFILE9);
					for (int i = 1; i <= reader1.NumberOfPages; i++)
					{
						copy.AddPage(copy.GetImportedPage(reader1, i));
					}
				}

				if (sFILE10 != "")
				{
					//파일2의 모든 페이지를 새 파일에 추가한다.
					reader1 = new PdfReader(sFILE10);
					for (int i = 1; i <= reader1.NumberOfPages; i++)
					{
						copy.AddPage(copy.GetImportedPage(reader1, i));
					}
				}
			}
			catch
			{
			}
			finally
			{
				document.Close();
				document.Dispose();
			}
		}

		public string PdfMerge(string pdfFiles, string outputFileName)
		{
			Document document = new Document();

			try
			{
				string[] pdfArr = Split(pdfFiles, CNST_SP_NT);

				using (PdfCopy copy = new PdfCopy(document, new FileStream(outputFileName, FileMode.Create)))
				{
					document.Open();

					foreach (string pdfFile in pdfArr)
					{
						if (!String.IsNullOrEmpty(pdfFile))
						{
							PdfReader reader = new PdfReader(pdfFile);
							reader.ConsolidateNamedDestinations();

							for (int i = 1; i <= reader.NumberOfPages; i++)
							{
								copy.AddPage(copy.GetImportedPage(reader, i));
							}

							reader.Close();
						}
					}

					copy.Close();
				}
			}
			catch (Exception ex)
			{

			}
			finally
			{
				document.Close();
				document.Dispose();
			}

			return outputFileName;
		}
		#endregion

		#region HTML To PDF 생성
		/// <summary>
		/// HTML TO PDF String 
		/// </summary>
		/// <param name="html"></param>
		public string HtmlToPDFString(string html)
		{
			string result = string.Empty;
			// 파일 IO 스트림을 취득한다.
			using (var stream = new MemoryStream())
			{
				// Pdf형식의 document를 생성한다.
				Document document = new Document(PageSize.A4, 10, 10, 10, 10);
				// PdfWriter를 취득한다.
				PdfWriter writer = PdfWriter.GetInstance(document, stream);
				// document Open
				document.Open();
				try
				{
					var helper = XMLWorkerHelper.GetInstance();
					var cssResolver = new StyleAttrCSSResolver();
					// 폰트 설정
					var font = new XMLWorkerFontProvider(XMLWorkerFontProvider.DONTLOOKFORFONTS);
					font.Register(@"c:/windows/fonts/malgun.ttf", "MalgunGothic");

					var cssAppliers = new CssAppliersImpl(font);
					//htmlContext 생성
					var htmlContext = new HtmlPipelineContext(cssAppliers);
					htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
					//pipeline 생성.
					var pdfPipeline = new PdfWriterPipeline(document, writer);
					var htmlPipeline = new HtmlPipeline(htmlContext, pdfPipeline);
					var cssResolverPipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
					//Work 생성 pipeline 연결
					var worker = new XMLWorker(cssResolverPipeline, true);
					//Xml파서 생성(Html -> 변환)
					var xmlParser = new XMLParser(true, worker, Encoding.GetEncoding("UTF-8"));
					using (StringReader strReader = new StringReader(html))
					{
						xmlParser.Parse(strReader);
					}
				}
				finally
				{
					//document Close
					document.Close();
				}
				byte[] bytes = stream.ToArray();
				result = BitConverter.ToString(bytes).Replace("-", string.Empty);
				return result;

			}

		}
		public class PageEventHelper : PdfPageEventHelper
		{
			PdfContentByte cb;
			PdfTemplate template;
			BaseFont RunDateFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
			float fontSzie = 10.0f;

			public override void OnOpenDocument(PdfWriter writer, Document document)
			{

				cb = writer.DirectContent;
				template = cb.CreateTemplate(50, 50);
			}

			public override void OnEndPage(PdfWriter writer, Document document)
			{
				base.OnEndPage(writer, document);

				int pageN = writer.PageNumber;
				String text = "Page " + pageN.ToString() + " of ";
				float len = fontSzie;

				iTextSharp.text.Rectangle pageSize = document.PageSize;

				cb.SetRGBColorFill(100, 100, 100);

				cb.BeginText();
				cb.SetFontAndSize(RunDateFont, fontSzie);
				//cb.SetTextMatrix(document.LeftMargin, pageSize.GetBottom(document.BottomMargin));
				cb.SetTextMatrix((document.Right / 2) - 25, pageSize.GetBottom(document.BottomMargin) - 10);
				cb.ShowText(text);

				cb.EndText();

				//cb.AddTemplate(template, document.LeftMargin + len, pageSize.GetBottom(document.BottomMargin));
				cb.AddTemplate(template, (document.Right / 2) + 25, pageSize.GetBottom(document.BottomMargin) - 10);
			}

			public override void OnCloseDocument(PdfWriter writer, Document document)
			{
				base.OnCloseDocument(writer, document);

				template.BeginText();
				template.SetFontAndSize(RunDateFont, fontSzie);
				template.SetTextMatrix(0, 0);
				//template.ShowText("" + (writer.PageNumber - 1));
				template.ShowText("" + (writer.PageNumber));
				template.EndText();
			}
		}
		/// <summary>
		/// HTML TO PDF String 
		/// </summary>
		/// <param name="html"></param>
		public byte[] EngagementReportPDF(string html, string css)
		{
			string result = string.Empty;
			// 파일 IO 스트림을 취득한다.
			using (var stream = new MemoryStream())
			{
				html = html.Replace("<br>", "<br/>");

				// Pdf형식의 document를 생성한다.
				Document document = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
				//document.SetPageSize(PageSize.A4.Rotate());

				// PdfWriter를 취득한다.
				PdfWriter writer = PdfWriter.GetInstance(document, stream);
				PageEventHelper pageEventHelper = new PageEventHelper();
				writer.PageEvent = pageEventHelper;

				// document Open
				document.Open();

				try
				{
					//var logo = iTextSharp.text.Image.GetInstance(LogoPath);
					////logo.SetAbsolutePosition(10f, 10f);
					//document.Add(logo);

					var helper = XMLWorkerHelper.GetInstance();
					var cssResolver = new StyleAttrCSSResolver();
					// 폰트 설정
					var font = new XMLWorkerFontProvider(XMLWorkerFontProvider.DONTLOOKFORFONTS);
					font.Register(@"c:/windows/fonts/malgun.ttf", "MalgunGothic");

					// CSS
					cssResolver.AddCss(css, true);

					var cssAppliers = new CssAppliersImpl(font);
					//htmlContext 생성
					var htmlContext = new HtmlPipelineContext(cssAppliers);
					htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
					//pipeline 생성.
					var pdfPipeline = new PdfWriterPipeline(document, writer);
					var htmlPipeline = new HtmlPipeline(htmlContext, pdfPipeline);
					var cssResolverPipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
					//Work 생성 pipeline 연결
					var worker = new XMLWorker(cssResolverPipeline, true);
					//Xml파서 생성(Html -> 변환)
					var xmlParser = new XMLParser(true, worker, Encoding.GetEncoding("UTF-8"));
					using (StringReader strReader = new StringReader(html))
					{
						xmlParser.Parse(strReader);
					}
				}
				finally
				{
					//document Close
					document.Close();
				}

				return stream.ToArray(); ;

			}

		}

		public byte[] HtmlWorkerToPDFBinary(string GridHtml)
		{
			StringReader sr = new StringReader(GridHtml);
			Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
			iTextSharp.text.html.simpleparser.HTMLWorker htmlparser = new iTextSharp.text.html.simpleparser.HTMLWorker(pdfDoc);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
				pdfDoc.Open();

				htmlparser.Parse(sr);
				pdfDoc.Close();

				byte[] bytes = memoryStream.ToArray();
				memoryStream.Close();

				return bytes;
			}
		}

		/// <summary>
		/// HTML TO PDF Binary
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public byte[] HtmlToPDFBinary(string html)
		{
			string result = string.Empty;
			// 파일 IO 스트림을 취득한다.
			using (var stream = new MemoryStream())
			{
				// Pdf형식의 document를 생성한다.
				Document document = new Document(PageSize.A4, 10, 10, 10, 10);
				// PdfWriter를 취득한다.
				PdfWriter writer = PdfWriter.GetInstance(document, stream);
				// document Open
				document.Open();
				try
				{
					var helper = XMLWorkerHelper.GetInstance();
					var cssResolver = new StyleAttrCSSResolver();
					// 폰트 설정
					var font = new XMLWorkerFontProvider(XMLWorkerFontProvider.DONTLOOKFORFONTS);
					font.Register(@"c:/windows/fonts/malgun.ttf", "MalgunGothic");
					font.Register(@"c:/windows/fonts/times.ttf", "Serif");

					var cssAppliers = new CssAppliersImpl(font);
					//htmlContext 생성
					var htmlContext = new HtmlPipelineContext(cssAppliers);
					htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
					//pipeline 생성.
					var pdfPipeline = new PdfWriterPipeline(document, writer);
					var htmlPipeline = new HtmlPipeline(htmlContext, pdfPipeline);
					var cssResolverPipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
					//Work 생성 pipeline 연결
					var worker = new XMLWorker(cssResolverPipeline, true);
					//Xml파서 생성(Html -> 변환)
					var xmlParser = new XMLParser(true, worker, Encoding.GetEncoding("UTF-8"));
					using (StringReader strReader = new StringReader(html))
					{
						xmlParser.Parse(strReader);
					}
				}
				finally
				{
					//document Close
					document.Close();
				}
				return stream.ToArray();

			}

		}
		#endregion

		public string GetRandomString(int length, bool number = false)
		{
			string charPool = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
			if (number == true)
			{
				charPool = "1234567890";
			}
			Random rnd = new Random();
			return GetRandomString(rnd, length, charPool);
		}

		public string GetRandomString(Random rnd, int length, string charPool)
		{
			StringBuilder rs = new StringBuilder();

			while (length > 0)
			{
				rs.Append(charPool[(int)(rnd.NextDouble() * charPool.Length)]);
				length--;
			}

			return rs.ToString();
		}

		public bool CheckPWContainID(string pwd, string id)
		{
			return pwd.IndexOf(id) == -1;
		}

		public bool CheckSameFiveChar(string input)
		{
			for (int i = 0; i < input.Length - 4; i++)
			{
				for (int j = i + 1; j < input.Length; j++)
				{
					if (input[i] != input[j]) break;
					if (i + 4 == j) return false;
				}
			}
			return true;
		}

		public bool CheckPassword(string pwd)
		{
			Regex regex = new Regex(@"^(?=.*[a-zA-Z])(?=.*[0-9])(?=.*[~!@#$ %^*()\-_=+\\\|\[\]{};:\'\,.<>\/?]).{10,50}$");
			return regex.IsMatch(pwd);
		}

		public bool CheckID(string id)
		{
			Regex regex = new Regex(@"^[a-z]{1}[0-9a-z\-_]{3,49}$");
			return regex.IsMatch(id);
		}


	}

	#region --------------------------ExcelHelper-----------------------------
	/// <summary>
	/// 엑셀 헬퍼
	/// </summary>
	public static class ExcelHelper
	{
		private static int HEADER_CELLSTYLE = 0;
		private static int DATA_CELLSTYLE_TEXT = 0;
		private static int DATA_CELLSTYLE_CODE = 0;
		private static int DATA_CELLSTYLE_CURRENCY = 0;
		//////////////////////////////////////////////////////////////////////////////////////////////////// Method
		////////////////////////////////////////////////////////////////////////////////////////// Static
		//////////////////////////////////////////////////////////////////////////////// Public
		#region 엑셀 문서 생성하기 - CreateExcelDocument(sourceSet, xlsxFilePath, startRowIndex, startColumnIndex)
		/// <summary>
		/// 엑셀 문서 생성하기
		/// </summary>
		/// <param name="sourceSet">소스 셋</param>
		/// <param name="xlsxFilePath">XLSX 파일 경로</param>
		/// <param name="startRowIndex">시작 행 인덱스</param>
		/// <param name="startColumnIndex">시작 컬럼 인덱스</param>
		/// <returns>처리 결과</returns>
		public static bool CreateExcelDocument(DataSet sourceSet, string xlsxFilePath, uint startRowIndex, int startColumnIndex)
		{
			try
			{
				string titleText = (xlsxFilePath.IndexOf("Outbound") > 0) ? "매출 세금계산서 목록조회" : "매입 전자(수정) 세금계산서 목록조회";
				using (SpreadsheetDocument document = SpreadsheetDocument.Create(xlsxFilePath, SpreadsheetDocumentType.Workbook))
				{
					WriteExcelFile(sourceSet, document, startRowIndex, startColumnIndex, titleText);
				}

				return true;
			}
			catch
			{
				return false;
			}
		}
		public static bool CreateExcelDocument(DataSet sourceSet, uint startRowIndex, int startColumnIndex)
		{
			try
			{
				using (MemoryStream ms = new MemoryStream())
				{
					using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook, false))
					{
						WriteExcelFile(sourceSet, document, startRowIndex, startColumnIndex);
					}

				}
				return true;
			}
			catch
			{
				return false;
			}
		}
		/// <summary>
		/// 엑셀 문서 생성하기
		/// </summary>
		/// <param name="sourceSet">소스 셋</param>
		/// <param name="xlsxFilePath">XLSX 파일 경로</param>
		/// <param name="startRowIndex">시작 행 인덱스</param>
		/// <param name="startColumnIndex">시작 컬럼 인덱스</param>
		/// <param name="titleText"></param>
		/// <returns>처리 결과</returns>
		public static bool CreateExcelDocument(DataSet sourceSet, string xlsxFilePath, uint startRowIndex, int startColumnIndex, string titleText)
		{
			try
			{
				using (SpreadsheetDocument document = SpreadsheetDocument.Create(xlsxFilePath, SpreadsheetDocumentType.Workbook))
				{
					WriteExcelFile(sourceSet, document, startRowIndex, startColumnIndex, titleText);
				}

				return true;
			}
			catch
			{
				return false;
			}
		}
		#endregion

		#region 엑셀 문서 생성하기 - CreateExcelDocument(sourceTable, xlsxFilePath, startRowIndex, startColumnIndex)
		/// <summary>
		/// 엑셀 문서 생성하기
		/// </summary>
		/// <param name="sourceTable">소스 테이블</param>
		/// <param name="xlsxFilePath">XLSX 파일 경로</param>
		/// <param name="startRowIndex">시작 행 인덱스</param>
		/// <param name="startColumnIndex">시작 컬럼 인덱스</param>
		/// <returns>처리 결과</returns>
		public static bool CreateExcelDocument(DataTable sourceTable, string xlsxFilePath, uint startRowIndex, int startColumnIndex)
		{
			DataSet set = new DataSet();

			set.Tables.Add(sourceTable);

			bool result = CreateExcelDocument(set, xlsxFilePath, startRowIndex, startColumnIndex);

			set.Tables.Remove(sourceTable);

			return result;
		}
		#endregion

		#region 엑셀 문서 생성하기 - CreateExcelDocument<T>(sourceList, xlsxFilePath, startRowIndex, startColumnIndex)
		/// <summary>
		/// 엑셀 문서 생성하기
		/// </summary>
		/// <typeparam name="T">소스 리스트 타입</typeparam>
		/// <param name="sourceList">소스 리스트</param>
		/// <param name="xlsxFilePath">XLSX 파일 경로</param>
		/// <param name="startRowIndex">시작 행 인덱스</param>
		/// <param name="startColumnIndex">시작 컬럼 인덱스</param>
		/// <returns>처리 결과</returns>
		public static bool CreateExcelDocument<T>(List<T> sourceList, string xlsxFilePath, uint startRowIndex, int startColumnIndex)
		{
			DataSet sourceSet = new DataSet();

			sourceSet.Tables.Add(GetDataTable(sourceList));

			return CreateExcelDocument(sourceSet, xlsxFilePath, startRowIndex, startColumnIndex);
		}
		#endregion

		//////////////////////////////////////////////////////////////////////////////// Private
		#region 엑셀 컬럼명 구하기 - GetExcelColumnName(columnIndex)
		/// <summary>
		/// 엑셀 컬럼명 구하기
		/// </summary>
		/// <param name="columnIndex">컬럼 인덱스</param>
		/// <returns>엑셀 컬럼명</returns>
		private static string GetExcelColumnName(int columnIndex)
		{
			// columnIndex Excel Column Name
			// ----------- -----------------
			// 0           A
			// 1           B
			// 25          Z
			// 26          AA
			// 27          AB
			if (columnIndex < 26)
			{
				return ((char)('A' + columnIndex)).ToString();
			}

			char firstCharacter = (char)('A' + (columnIndex / 26) - 1);
			char secondCharacter = (char)('A' + (columnIndex % 26));

			return string.Format("{0}{1}", firstCharacter, secondCharacter);
		}
		#endregion


		#region 헤더 셀 추가하기 - AppendHeaderCell(excelRow, cellReference, cellStringValue)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="excelRow">엑셀 행</param>
		/// <param name="cellReference">셀 참조</param>
		/// <param name="cellStringValue">셀 문자열 값</param>
		private static void AppendHeaderCell(Row excelRow, string cellReference, string cellStringValue)
		{
			Cell cell = new Cell() { CellReference = cellReference, DataType = CellValues.String };
			CellValue cellValue = new CellValue();

			cellValue.Text = cellStringValue;
			cell.StyleIndex = UInt32Value.FromUInt32((UInt32)HEADER_CELLSTYLE);

			cell.Append(cellValue);

			excelRow.Append(cell);
		}
		#endregion


		#region 텍스트 셀 추가하기 - AppendTextCell(excelRow, cellReference, cellStringValue)
		/// <summary>
		/// 텍스트 셀 추가하기
		/// </summary>
		/// <param name="excelRow">엑셀 행</param>
		/// <param name="cellReference">셀 참조</param>
		/// <param name="cellStringValue">셀 문자열 값</param>
		private static void AppendTextCell(Row excelRow, string cellReference, string cellStringValue)
		{
			Cell cell = new Cell() { CellReference = cellReference, DataType = CellValues.String };
			CellValue cellValue = new CellValue();

			cellValue.Text = cellStringValue;
			cell.StyleIndex = UInt32Value.FromUInt32((UInt32)DATA_CELLSTYLE_TEXT);

			cell.Append(cellValue);

			excelRow.Append(cell);
		}
		#endregion

		#region 숫자 셀 추가하기 - AppendNumericCell(excelRow, cellReference, cellStringValue)
		/// <summary>
		/// 숫자 셀 추가하기
		/// </summary>
		/// <param name="excelRow">엑셀 행</param>
		/// <param name="cellReference">셀 참조</param>
		/// <param name="cellStringValue">셀 문자열 값</param>
		private static void AppendNumericCell(Row excelRow, string cellReference, string cellStringValue)
		{
			Cell cell = new Cell() { CellReference = cellReference };
			CellValue cellValue = new CellValue();

			cellValue.Text = cellStringValue;
			cell.StyleIndex = UInt32Value.FromUInt32((UInt32)DATA_CELLSTYLE_CURRENCY);

			cell.Append(cellValue);

			excelRow.Append(cell);
		}
		#endregion

		#region 코드 셀 추가하기 - AppendCodeCell(excelRow, cellReference, cellStringValue)
		/// <summary>
		/// 숫자 셀 추가하기
		/// </summary>
		/// <param name="excelRow">엑셀 행</param>
		/// <param name="cellReference">셀 참조</param>
		/// <param name="cellStringValue">셀 문자열 값</param>
		private static void AppendCodeCell(Row excelRow, string cellReference, string cellStringValue)
		{
			Cell cell = new Cell() { CellReference = cellReference, DataType = CellValues.String };
			CellValue cellValue = new CellValue();

			cellValue.Text = cellStringValue;
			cell.StyleIndex = UInt32Value.FromUInt32((UInt32)DATA_CELLSTYLE_CODE);

			cell.Append(cellValue);

			excelRow.Append(cell);
		}
		#endregion


		#region 엑셀 워크시트에 데이터 테이블 쓰기 - WriteDataTableToExcelWorksheet(sourceTable, worksheetPart, startRowIndex, startColumnIndex)
		/// <summary>
		/// 엑셀 워크시트에 데이터 테이블 쓰기
		/// </summary>
		/// <param name="sourceTable">소스 테이블</param>
		/// <param name="worksheetPart">워크시트 파트</param>
		/// <param name="startRowIndex">시작 행 인덱스</param>
		/// <param name="startColumnIndex">시작 컬럼 인덱스</param>
		/// <remarks>시작 행 인덱스 및 시작 컬럼 인덱스는 1부터 시작한다.</remarks>
		private static void WriteDataTableToExcelWorksheet(DataTable sourceTable, WorksheetPart worksheetPart, uint startRowIndex, int startColumnIndex, string titleText = "제목없음")
		{
			Worksheet worksheet = worksheetPart.Worksheet;
			SheetData sheetData = worksheet.GetFirstChild<SheetData>();

			// 데이터 테이블에서 각 컬럼의 데이터를 위한 헤더를 포함하는, 헤더 행을 엑셀 파일에 생성한다.
			// 각 컬럼의 데이터가 무슨 타입인지를 나타내는 (텍스트 또는 숫자), 배열을 또한 생성한다.
			// 그래서 실제 셀의 데이터를 쓰게 될 때, 텍스트 값들 또는 숫자 셀 값들을 쓰는지 알게 된다.
			int columnCount = sourceTable.Columns.Count;
			bool[] isNumericColumnArray = new bool[columnCount];
			string[] excelColumnNameArray = new string[columnCount];

			bool[] codeColumnArray = GetCodeColumnArray(sourceTable);


			for (int i = 0; i < columnCount; i++)
			{
				excelColumnNameArray[i] = GetExcelColumnName(i + startColumnIndex - 1);
			}

			//타이틀
			uint rowIndex = startRowIndex;
			Row titleRow = new Row { RowIndex = rowIndex };

			Cell titleCell = new Cell() { CellReference = "A1", DataType = CellValues.String };
			CellValue titleCellValue = new CellValue();
			titleCellValue.Text = titleText;
			titleCell.Append(titleCellValue);
			titleCell.StyleIndex = UInt32Value.FromUInt32((UInt32)HEADER_CELLSTYLE);
			titleRow.Append(titleCell);
			sheetData.Append(titleRow);

			string mergeCell = $"{excelColumnNameArray[0]}1:{excelColumnNameArray[excelColumnNameArray.Length - 1]}1";
			MergeCells mergeCells = new MergeCells();
			mergeCells.Append(new MergeCell() { Reference = new StringValue(mergeCell) });
			worksheet.InsertAfter(mergeCells, worksheet.Elements<SheetData>().First());
			worksheet.Save();

			// 엑셀 워크시트에 헤더 행을 생성한다.
			++rowIndex;
			Row headerRow = new Row { RowIndex = rowIndex };
			sheetData.Append(headerRow);
			for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
			{
				DataColumn dataColumn = sourceTable.Columns[columnIndex];
				AppendHeaderCell(headerRow, excelColumnNameArray[columnIndex] + rowIndex.ToString(), dataColumn.ColumnName);
				isNumericColumnArray[columnIndex] = (dataColumn.DataType.FullName == "System.Decimal") || (dataColumn.DataType.FullName == "System.Int32");
			}



			// 데이터 테이블에서 각 행의 데이터를 단계적으로 작성한다.
			string cellValue = string.Empty;
			double numericCellValue = 0;

			foreach (DataRow dataRow in sourceTable.Rows)
			{
				// 신규 행을 생성하고, 이 행의 데이터 집합을 추가한다.
				++rowIndex;
				Row newExcelRow = new Row { RowIndex = rowIndex };
				sheetData.Append(newExcelRow);

				for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
				{
					cellValue = dataRow.ItemArray[columnIndex].ToString();

					// 데이터를 갖고 셀을 생성한다.
					if (isNumericColumnArray[columnIndex])
					{
						// 숫자 셀들을 위해, 입력 데이터가 숫자인지 확인한 다음, 엑셀 파일에 작성한다.
						// 숫자 값이 NULL인 경우, 엑셀 파일에 아무 것도 작성하지 않는다.
						numericCellValue = 0;

						if (double.TryParse(cellValue, out numericCellValue))
						{
							cellValue = numericCellValue.ToString();

							AppendNumericCell(newExcelRow, excelColumnNameArray[columnIndex] + rowIndex.ToString(), cellValue);
						}
					}
					else
					{
						if (codeColumnArray[columnIndex])
						{
							// 코드 셀들을 위해, 입력 데이터를 엑셀 파일에 바로 작성한다.
							AppendCodeCell(newExcelRow, excelColumnNameArray[columnIndex] + rowIndex.ToString(), cellValue);
						}
						else
						{
							// 텍스트 셀들을 위해, 입력 데이터를 엑셀 파일에 바로 작성한다.
							AppendTextCell(newExcelRow, excelColumnNameArray[columnIndex] + rowIndex.ToString(), cellValue);
						}

					}
				}
			}
		}
		#endregion

		#region 엑셀 파일 쓰기 - WriteExcelFile(sourceSet, spreadsheetDocument, startRowIndex, startColumnIndex)
		/// <summary>
		/// 엑셀 파일 쓰기
		/// </summary>
		/// <param name="sourceSet">소스 셋</param>
		/// <param name="spreadsheetDocument">스프레드 시트 문서</param>
		/// <param name="startRowIndex">시작 행 인덱스</param>
		/// <param name="startColumnIndex">시작 컬럼 인덱스</param>
		private static void WriteExcelFile(DataSet sourceSet, SpreadsheetDocument spreadsheetDocument, uint startRowIndex, int startColumnIndex, string titleText = "제목없음")
		{
			// 엑셀 파일 내용들을 생성한다.
			// 이 함수는 엑셀 파일을 생성할 뿐만 아니라 파일에 쓰거나, 또는 MemoryStream에 작성할 때 사용된다.
			spreadsheetDocument.AddWorkbookPart();
			spreadsheetDocument.WorkbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook();

			// 엑셀 2010에서 충돌을 방지해준다.
			spreadsheetDocument.WorkbookPart.Workbook.Append(new BookViews(new WorkbookView()));

			// "WorkbookStylesPart"를 추가하지 않는 경우, OLEDB가 이 .xlsx 파일에 접속하는 것을 거부할 것이다.
			WorkbookStylesPart workbookStylesPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");

			Stylesheet stylesheet = CreateStyleSheet();
			workbookStylesPart.Stylesheet = stylesheet;
			workbookStylesPart.Stylesheet.Save();

			// 데이터 셋에 있는 데이터 테이블들 각각을 통해 루프를 돌면서, 각 데이터 테이블을 위한 신규 엑셀 워크시트를 생성한다.
			uint worksheetNumber = 1;

			foreach (DataTable sourceTable in sourceSet.Tables)
			{
				string workSheetID = "rId" + worksheetNumber.ToString();
				string worksheetName = sourceTable.TableName;

				WorksheetPart newWorksheetPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();

				newWorksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet();

				// create the columns
				Columns worksheetColumns = CreateWorksheetColumns(sourceTable);
				// append the columns.  NOTE!!! Only works if you provide nothing to the
				// new Worksheet declaration
				newWorksheetPart.Worksheet.AppendChild(worksheetColumns);
				// save the workbook part
				spreadsheetDocument.WorkbookPart.Workbook.Save();

				// 시트 데이터를 생성한다.
				newWorksheetPart.Worksheet.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.SheetData());

				// 워크시트를 저장한다.
				WriteDataTableToExcelWorksheet(sourceTable, newWorksheetPart, startRowIndex, startColumnIndex, titleText);

				newWorksheetPart.Worksheet.Save();

				// 워크북 관계에 대한 워크시트를 생성한다.
				if (worksheetNumber == 1)
				{
					spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheets());
				}

				spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>().AppendChild
				(
					new DocumentFormat.OpenXml.Spreadsheet.Sheet()
					{
						Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(newWorksheetPart),
						SheetId = (uint)worksheetNumber,
						Name = sourceTable.TableName
					}
				);

				worksheetNumber++;
			}

			spreadsheetDocument.WorkbookPart.Workbook.Save();
		}

		// ------------------------------------------------------------------------------
		// CreateWorksheetStylesheet creates the stylesheet for the worksheet
		//
		// When creating a stylesheet, the idea is to:
		// First, Define all the number formats, fonts, fills, and borders as separate entities.
		// Then, pick a number format, a font, a fill, and a border and create a cell format.
		// Repeat this for as many cell formats as required in the worksheet. It can be
		// any amount.
		// ---------------------------------------------------------------------------------
		private static Stylesheet CreateStyleSheet()
		{

			// values to easily keep track of entities so when be
			// finally build the cell formats, we do it with something friendly
			int NUMBERING_FORMAT_CURRENCY = 203;


			// we are going to set these when we build the fills, that way,
			// we don't have to come back up here everytime and modify the values;
			int FONTID_DEFAULT = 0;
			int FONTID_DEFAULT_BOLD = 0;

			int FILLID_DEFAULT = 0;
			int FILLID_PATTERN_VALUE_GRAY_125 = 0;
			int FILLID_PATTERN_HEADER = 0;


			int BORDERID_DEFAULT = 0;
			int BORDERID_BLACK = 1;

			int CELLSTYLE_DEFAULT = 0;
			Stylesheet styleSheet = new Stylesheet();

			// define a new numbering format collection.  this collection will hold all the
			// numbering formats used throughout the worksheet(s)

			styleSheet.NumberingFormats = new NumberingFormats();

			// THis is for the date, we give the number format an ID of 200.  Note
			// ID is a UInt32Value
			NumberingFormat numberingFormat = new NumberingFormat();
			numberingFormat.FormatCode = StringValue.FromString("#,###");
			numberingFormat.NumberFormatId = UInt32Value.FromUInt32((UInt32)NUMBERING_FORMAT_CURRENCY);
			styleSheet.NumberingFormats.Append(numberingFormat);

			// update the collection count.  Don't know why object library can't do this but it doesn't
			styleSheet.NumberingFormats.Count = UInt32Value.FromUInt32((UInt32)styleSheet.NumberingFormats.ChildElements.Count);

			// Define a new FOnts collection.  This collection will contain all the fonts
			// used in the worksheet(s).  REMEMBER THE INDEXES on these.  O Based
			styleSheet.Fonts = new Fonts();

			// index 0
			// Default font
			DocumentFormat.OpenXml.Spreadsheet.Font font = new DocumentFormat.OpenXml.Spreadsheet.Font(new FontSize() { Val = 10 }, new FontName() { Val = "Arial" });
			styleSheet.Fonts.Append(font);
			FONTID_DEFAULT = styleSheet.Fonts.ChildElements.Count - 1;

			// default font bold
			font = new DocumentFormat.OpenXml.Spreadsheet.Font(new FontSize() { Val = 10 }, new FontName() { Val = "Arial" });
			font.Bold = new Bold();
			font.Bold.Val = BooleanValue.FromBoolean(true);
			styleSheet.Fonts.Append(font);
			FONTID_DEFAULT_BOLD = styleSheet.Fonts.ChildElements.Count - 1;

			// update the font collection count
			styleSheet.Fonts.Count = UInt32Value.FromUInt32((UInt32)styleSheet.Fonts.ChildElements.Count);

			// define a fills collection.  Fills are used to create the background and foreground colors.
			// they use another object called the pattern fill.  NOTE!!!! you have to always define the
			// two preset fills.
			styleSheet.Fills = new Fills();

			// Fill Index 0
			Fill fill = new Fill();
			PatternFill PatternFillPreset = new PatternFill();
			PatternFillPreset.PatternType = PatternValues.None;
			fill.PatternFill = PatternFillPreset;
			styleSheet.Fills.Append(fill);
			FILLID_DEFAULT = styleSheet.Fills.ChildElements.Count - 1;

			// Fill Index 1.  Defaults By Micorosoft
			fill = new Fill();
			PatternFillPreset = new PatternFill();
			PatternFillPreset.PatternType = PatternValues.Gray125;
			fill.PatternFill = PatternFillPreset;
			styleSheet.Fills.Append(fill);
			FILLID_PATTERN_VALUE_GRAY_125 = styleSheet.Fills.ChildElements.Count - 1;

			// Fill Index 2 (Custom - Gold)
			fill = new Fill();
			PatternFill patternFill = new PatternFill();
			patternFill.PatternType = PatternValues.Solid;
			patternFill.ForegroundColor = new ForegroundColor();
			patternFill.ForegroundColor.Rgb = HexBinaryValue.FromString("c0c0c0");
			fill.PatternFill = patternFill;
			styleSheet.Fills.Append(fill);
			FILLID_PATTERN_HEADER = styleSheet.Fills.ChildElements.Count - 1;

			// update the fills collection count
			styleSheet.Fills.Count = UInt32Value.FromUInt32((UInt32)styleSheet.Fills.ChildElements.Count);




			// Define the borders used in the worksheets.
			styleSheet.Borders = new Borders();

			// default border
			Border border = new Border();
			styleSheet.Borders.Append(border);
			BORDERID_DEFAULT = styleSheet.Borders.ChildElements.Count - 1;

			string BorderColorString = "000000";
			border = new Border();
			border.LeftBorder = new LeftBorder();
			border.LeftBorder.Style = BorderStyleValues.Thin;
			border.LeftBorder.Color = new Color();
			border.LeftBorder.Color.Rgb = HexBinaryValue.FromString(BorderColorString);
			border.RightBorder = new RightBorder();
			border.RightBorder.Style = BorderStyleValues.Thin;
			border.RightBorder.Color = new Color();
			border.RightBorder.Color.Rgb = HexBinaryValue.FromString(BorderColorString);
			border.BottomBorder = new BottomBorder();
			border.BottomBorder.Style = BorderStyleValues.Thin;
			border.BottomBorder.Color = new Color();
			border.BottomBorder.Color.Rgb = HexBinaryValue.FromString(BorderColorString);
			border.TopBorder = new TopBorder();
			border.TopBorder.Style = BorderStyleValues.Thin;
			border.TopBorder.Color = new Color();
			border.TopBorder.Color.Rgb = HexBinaryValue.FromString(BorderColorString);
			styleSheet.Borders.Append(border);
			BORDERID_BLACK = styleSheet.Borders.ChildElements.Count - 1;

			// update the borders collection count
			styleSheet.Borders.Count = UInt32Value.FromUInt32((UInt32)styleSheet.Borders.ChildElements.Count);

			// create a new cell formats collection for the stylesheet
			styleSheet.CellFormats = new CellFormats();


			// index 0 - DEfault Cell Format
			CellFormat cellFormat = new CellFormat();
			styleSheet.CellFormats.Append(cellFormat);
			CELLSTYLE_DEFAULT = styleSheet.CellFormats.ChildElements.Count - 1;

			// index 1 (Header)
			cellFormat = new CellFormat();
			cellFormat.FontId = UInt32Value.FromUInt32((UInt32)FONTID_DEFAULT);
			cellFormat.ApplyFont = BooleanValue.FromBoolean(true);
			cellFormat.FillId = UInt32Value.FromUInt32((UInt32)FILLID_PATTERN_HEADER);
			cellFormat.ApplyFill = BooleanValue.FromBoolean(true);
			cellFormat.BorderId = UInt32Value.FromUInt32((UInt32)BORDERID_BLACK);
			cellFormat.ApplyBorder = BooleanValue.FromBoolean(true);
			cellFormat.Alignment = new Alignment();
			cellFormat.Alignment.Horizontal = HorizontalAlignmentValues.Center;
			cellFormat.Alignment.Vertical = VerticalAlignmentValues.Top;
			cellFormat.ApplyAlignment = BooleanValue.FromBoolean(true);
			styleSheet.CellFormats.Append(cellFormat);
			HEADER_CELLSTYLE = styleSheet.CellFormats.ChildElements.Count - 1;

			// index 2 TEXT CELLS
			cellFormat = new CellFormat();
			cellFormat.FontId = UInt32Value.FromUInt32((UInt32)FONTID_DEFAULT);
			cellFormat.ApplyFont = BooleanValue.FromBoolean(true);
			cellFormat.BorderId = UInt32Value.FromUInt32((UInt32)BORDERID_BLACK);
			cellFormat.ApplyBorder = BooleanValue.FromBoolean(true);
			cellFormat.Alignment = new Alignment();
			cellFormat.Alignment.Horizontal = HorizontalAlignmentValues.Left;
			cellFormat.Alignment.Vertical = VerticalAlignmentValues.Top;
			cellFormat.ApplyAlignment = BooleanValue.FromBoolean(true);
			styleSheet.CellFormats.Append(cellFormat);
			DATA_CELLSTYLE_TEXT = styleSheet.CellFormats.ChildElements.Count - 1;

			//index 3 Unit, Total - Currency
			cellFormat = new CellFormat();
			cellFormat.FontId = UInt32Value.FromUInt32((UInt32)FONTID_DEFAULT);
			cellFormat.ApplyFont = BooleanValue.FromBoolean(true);
			cellFormat.NumberFormatId = UInt32Value.FromUInt32((UInt32)NUMBERING_FORMAT_CURRENCY);
			cellFormat.ApplyNumberFormat = BooleanValue.FromBoolean(true);
			cellFormat.BorderId = UInt32Value.FromUInt32((UInt32)BORDERID_BLACK);
			cellFormat.ApplyBorder = BooleanValue.FromBoolean(true);
			cellFormat.Alignment = new Alignment();
			cellFormat.Alignment.Horizontal = HorizontalAlignmentValues.Right;
			cellFormat.Alignment.Vertical = VerticalAlignmentValues.Top;
			cellFormat.ApplyAlignment = BooleanValue.FromBoolean(true);
			styleSheet.CellFormats.Append(cellFormat);
			DATA_CELLSTYLE_CURRENCY = styleSheet.CellFormats.ChildElements.Count - 1;

			// index 4 CODE CELLS
			cellFormat = new CellFormat();
			cellFormat.FontId = UInt32Value.FromUInt32((UInt32)FONTID_DEFAULT);
			cellFormat.ApplyFont = BooleanValue.FromBoolean(true);
			cellFormat.BorderId = UInt32Value.FromUInt32((UInt32)BORDERID_BLACK);
			cellFormat.ApplyBorder = BooleanValue.FromBoolean(true);
			cellFormat.Alignment = new Alignment();
			cellFormat.Alignment.Horizontal = HorizontalAlignmentValues.Center;
			cellFormat.Alignment.Vertical = VerticalAlignmentValues.Top;
			cellFormat.ApplyAlignment = BooleanValue.FromBoolean(true);
			styleSheet.CellFormats.Append(cellFormat);
			DATA_CELLSTYLE_CODE = styleSheet.CellFormats.ChildElements.Count - 1;

			// now update th cell formats count
			styleSheet.CellFormats.Count = UInt32Value.FromUInt32((UInt32)styleSheet.CellFormats.ChildElements.Count);

			return styleSheet;

		}

		// ---------------------------------------------------------------
		// CreateWorksheetColumns configures the sizing of columns that
		// comprise the worksheet
		// ---------------------------------------------------------------
		private static Columns CreateWorksheetColumns(DataTable sourceTable)
		{
			int columnCount = sourceTable.Columns.Count;
			int[] columnLengthArray = GetColumnLengthArray(sourceTable);
			bool[] codeColumnArray = GetCodeColumnArray(sourceTable);

			// define a new columns object
			Columns workSheetColumns = new Columns();


			for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
			{
				Column col = new Column();
				col.Min = UInt32Value.FromUInt32((UInt32)(columnIndex + 1));
				col.Max = col.Min;
				DataColumn dataColumn = sourceTable.Columns[columnIndex];

				double width = 0;
				int dataLength = (dataColumn.ColumnName.Length > columnLengthArray[columnIndex]) ? dataColumn.ColumnName.Length : columnLengthArray[columnIndex];

				if ((dataColumn.DataType.FullName == "System.Decimal") || (dataColumn.DataType.FullName == "System.Int32"))
				{
					width = 12;
					col.Width = DoubleValue.FromDouble(12);
				}
				else
				{

					//컬럼 길이는 코드성 데이터 여부 - 요소1 : 컬럼 별 데이터 길이 & 숫자여부
					if (codeColumnArray[columnIndex])
					{
						width = (dataLength * 1.5 > 30.0) ? 30.0 : dataLength * 1.5;
					}
					else
					{
						width = (dataLength * 2 > 30.0) ? 30.0 : dataLength * 2;
					}

				}
				//하드코딩(코드성 데이터로 구분되지만 실제 데이터 길이는 가변)
				if (dataColumn.ColumnName == "문서형태" || dataColumn.ColumnName == "문서상태" || dataColumn.ColumnName == "BU")
				{
					width = (dataLength * 2 > 30.0) ? 30.0 : dataLength * 2;
				}

				col.Width = DoubleValue.FromDouble(width);
				col.CustomWidth = BooleanValue.FromBoolean(true);
				workSheetColumns.Append(col);
			}
			return workSheetColumns;
		}

		/// <summary>
		/// row데이터 코드성컬럼여부 배열
		/// </summary>
		/// <param name="sourceTable"></param>
		/// <returns></returns>
		private static bool[] GetCodeColumnArray(DataTable sourceTable)
		{
			int columnCount = sourceTable.Columns.Count;
			bool[] codeColumnArray = new bool[columnCount];
			Regex regex = new Regex("^[a-zA-Z0-9\\-]*$");

			//조회 데이터 필드값의 Length 가 변하지 않으면 Code 데이터로 인식
			int[] columnLengthArray = GetColumnLengthArray(sourceTable);

			for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
			{
				foreach (DataRow dataRow in sourceTable.Rows)
				{
					codeColumnArray[columnIndex] = true;
					// row 데이터의 길이가 없는 건은 비교에서 제외하여 코드성 데이터인지 확인
					int comparedColumnLength = (dataRow.ItemArray[columnIndex].ToString().Length == 0) ? columnLengthArray[columnIndex] : dataRow.ItemArray[columnIndex].ToString().Length;
					//코드성 데이터 판단 - 같은 데이터 길이 여부 || 영숫자 여부
					if (columnLengthArray[columnIndex] == 0 || columnLengthArray[columnIndex] != comparedColumnLength || !regex.IsMatch(dataRow.ItemArray[columnIndex].ToString()))
					{
						codeColumnArray[columnIndex] = false;
						continue;
					}
				}
			}

			//foreach (DataRow dataRow in sourceTable.Rows)
			//{

			//    for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
			//    {
			//        codeColumnArray[columnIndex, 0] = true;
			//        codeColumnArray[columnIndex, 1] = true;

			//        // row 데이터의 길이가 없는 건은 비교에서 제외하여 코드성 데이터인지 확인
			//        int comparedColumnLength = (dataRow.ItemArray[columnIndex].ToString().Length == 0) ? ColumnLengthArray[columnIndex] : dataRow.ItemArray[columnIndex].ToString().Length;
			//        if (ColumnLengthArray[columnIndex] == 0 || ColumnLengthArray[columnIndex] != comparedColumnLength)
			//        {
			//            codeColumnArray[columnIndex, 0] = false;
			//        }
			//        if (!regex.IsMatch(dataRow.ItemArray[columnIndex].ToString()))
			//        {
			//            codeColumnArray[columnIndex, 1] = false;
			//        }
			//    }
			//}

			//예외 - 코드성 데이터 하드코딩
			for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
			{
				if (sourceTable.Columns[columnIndex].ColumnName == "문서형태" || sourceTable.Columns[columnIndex].ColumnName == "문서상태" || sourceTable.Columns[columnIndex].ColumnName == "BU")
				{
					//codeColumnArray[columnIndex, 0] = true;
					//codeColumnArray[columnIndex, 1] = true;
					codeColumnArray[columnIndex] = true;
				}

			}
			return codeColumnArray;
		}

		/// <summary>
		/// row데이터 컬럼별 최대길이
		/// </summary>
		/// <param name="sourceTable"></param>
		/// <returns></returns>
		private static int[] GetColumnLengthArray(DataTable sourceTable)
		{
			int columnCount = sourceTable.Columns.Count;
			int rowCount = sourceTable.Rows.Count;
			int[] ColumnLengthArray = new int[columnCount];

			for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
			{
				//각 column 데이터 최대길이
				int columnMaxLength = sourceTable.Rows[0].ItemArray[columnIndex].ToString().Length;
				for (int rowIndex = 1; rowIndex < rowCount; rowIndex++)
				{
					if (columnMaxLength < sourceTable.Rows[rowIndex].ItemArray[columnIndex].ToString().Length)
					{
						columnMaxLength = sourceTable.Rows[rowIndex].ItemArray[columnIndex].ToString().Length;
					}
				}
				ColumnLengthArray[columnIndex] = columnMaxLength;
			}

			return ColumnLengthArray;

		}




		#endregion


		#region 널 가능 타입 구하기 - GetNullableType(sourceType)
		/// <summary>
		/// 널 가능 타입 구하기
		/// </summary>
		/// <param name="sourceType">소스 타입</param>
		/// <returns>널 가능 타입</returns>
		private static Type GetNullableType(Type sourceType)
		{
			Type targetType = sourceType;

			if (sourceType.IsGenericType && sourceType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
			{
				targetType = Nullable.GetUnderlyingType(sourceType);
			}

			return targetType;
		}
		#endregion

		#region 널 가능 타입 여부 구하기 - IsNullableType(sourceType)
		/// <summary>
		/// 널 가능 타입 여부 구하기
		/// </summary>
		/// <param name="sourceType">소스 타입</param>
		/// <returns>널 가능 타입 여부</returns>
		private static bool IsNullableType(Type sourceType)
		{
			return (sourceType == typeof(string) || sourceType.IsArray || (sourceType.IsGenericType && sourceType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))));
		}
		#endregion

		#region 데이터 테이블 구하기 - GetDataTable<T>(list)
		/// <summary>
		/// 데이터 테이블 구하기
		/// </summary>
		/// <typeparam name="T">리스트 타입</typeparam>
		/// <param name="list">리스트</param>
		/// <returns>데이터 테이블</returns>

		private static DataTable GetDataTable<T>(List<T> list)
		{
			DataTable targetTable = new DataTable();

			foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
			{
				targetTable.Columns.Add(new DataColumn(propertyInfo.Name, GetNullableType(propertyInfo.PropertyType)));
			}

			foreach (T item in list)
			{
				DataRow dataRow = targetTable.NewRow();

				foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
				{
					if (!IsNullableType(propertyInfo.PropertyType))
					{
						dataRow[propertyInfo.Name] = propertyInfo.GetValue(item, null);
					}
					else
					{
						dataRow[propertyInfo.Name] = (propertyInfo.GetValue(item, null) ?? DBNull.Value);
					}
				}

				targetTable.Rows.Add(dataRow);
			}

			return targetTable;
		}
		#endregion

	}
	#endregion

	#region --------------------------CsvHelper-----------------------------
	/// <summary>
	/// xlsx 파일로 감당 안되는 많은 데이터 저장용
	/// </summary>
	public static class CsvHelper
	{
		private static string SubFilePath = "TempDownload";
		/// <summary>
		/// CSV 파일 저장
		/// </summary>
		/// <param name="dt">DataTable 형식</param>
		/// <param name="FileRoot">경로</param>
		/// <param name="fileName">파일명</param>
		/// <param name="isOldTempFileDeleted">올드 다운용 파일 삭제여부</param>
		public static string SaveCSV(DataTable dt, string FileRoot, string fileName, bool isOldTempFileDeleted = true)
		{
			string path = "";
			const string Comma = ",";
			const string Ext = ".csv";
			Files f = new Files();
			StringBuilder sbSheet = new StringBuilder();
			string fileFullPath = "";

			try
			{
				//------------------------------------------------------------
				// 유효성 체크
				//------------------------------------------------------------
				if (dt == null || dt.Rows.Count == 0) throw new Exception("데이터가 없습니다.");
				else if (string.IsNullOrEmpty(FileRoot)) throw new Exception("경로가 없습니다.");
				else if (string.IsNullOrEmpty(fileName)) throw new Exception("파일명이 없습니다.");
				

				//------------------------------------------------------------
				// 선작업
				//------------------------------------------------------------
				
				Util u = new Util();
				if (fileName.IndexOf(Ext) == -1) fileName += Ext;
				else if (u.Right(fileName, 4).ToLower() != Ext) fileName += Ext;

				// 경로 구성
				path = FileRoot + "\\" + SubFilePath;
				fileFullPath = System.IO.Path.Combine(path, fileName);

				if (f.IsDir(path))
				{
					// 오래된 템프파일 삭제
					if (isOldTempFileDeleted) f.DeleteOldFile(path);
				}
				else
				{
					// 폴더 없으면 생성
					f.MkDirs(path);
				}

				//------------------------------------------------------------
				//  본문 구성 및 파일 저장
				//------------------------------------------------------------
				#region 본문 구성

				StringBuilder sbRow = new StringBuilder();

				for (int i = 0; i < dt.Columns.Count; i++)
				{
					if (0 < i) sbRow.Append(Comma);
					sbRow.Append(dt.Columns[i].ColumnName);
				}

				sbSheet.AppendLine(sbRow.ToString());

				for (int i = 0; i < dt.Rows.Count; i++)
				{
					sbRow = new StringBuilder();

					for (int j = 0; j < dt.Columns.Count; j++)
					{
						if (0 < j) sbRow.Append(Comma);

						string value = dt.Rows[i][dt.Columns[j].ColumnName].ToString();
						//value.IndexOf(",", StringComparison.OrdinalIgnoreCase)
						if (0 <= value.IndexOf(",")) value = "\"" + value + "\"";
						sbRow.Append(value);
					}

					sbSheet.AppendLine(sbRow.ToString());
				}

				sbSheet.ToString();
				#endregion

				// 파일 저장
				f.Write_BOM(fileFullPath, sbSheet.ToString());

				return SubFilePath + "/" + fileName;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
	#endregion



	#region --------------------------확장-----------------------------
	/// <summary>
	/// 확장하기
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="v"></param>
		public static object ToModel<T>(this Hashtable v) where T : new()
		{
			IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
			T item = new T();

			if (v != null)
			{
				string propertyName = "";
				foreach (var property in properties)
				{
					if (v.ContainsKey(property.Name))
					{
						propertyName = property.Name;
						try
						{
							if (property.PropertyType.Name == "DateTime")
							{
								if (v[property.Name].ToString() != "")
								{
									DateTime date = DateTime.Parse(v[property.Name].ToString());
									property.SetValue(item, date, null);
								}
							}
							else if (property.PropertyType.Name == "Int32" || property.PropertyType.Name == "Int64")
							{
								if (v[property.Name].ToString() == "")
								{
									property.SetValue(item, 0, null);
								}
								else
								{
									property.SetValue(item, int.Parse(v[property.Name].ToString()), null);
								}
							}
							else if (property.PropertyType.Name == "Decimal")
							{
								if (v[property.Name].ToString() == "")
								{
									property.SetValue(item, 0M, null);
								}
								else
								{
									property.SetValue(item, decimal.Parse(v[property.Name].ToString()), null);
								}
							}
							else
							{
								property.SetValue(item, v[property.Name], null);
							}
						}
						catch (Exception ex)
						{
							throw new Exception(ex.Message + "(" + propertyName + ")");
						}
					}
				}
			}

			return item;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="v"></param>
		/// <returns></returns>
		public static object ToModel<T>(this DataTable v) where T : new()
		{
			IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
			T item = new T();

			if (v != null)
			{
				string propertyName = "";
				foreach (var property in properties)
				{
					if (v.Columns.Contains(property.Name))
					{
						propertyName = property.Name;

						try
						{
							if (property.PropertyType.Name == "DateTime")
							{
								if (v.Rows.Count == 0 || v.Rows[0][property.Name].ToString() == "")
								{
									DateTime date = new DateTime();
									property.SetValue(item, date, null);
								}
								else
								{
									DateTime date = DateTime.Parse(v.Rows[0][property.Name].ToString());
									property.SetValue(item, date, null);
								}
							}
							else if (property.PropertyType.Name == "Int32" || property.PropertyType.Name == "Int64")
							{
								if (v.Rows.Count == 0 || v.Rows[0][property.Name].ToString() == "")
								{
									property.SetValue(item, 0, null);
								}
								else
								{
									property.SetValue(item, int.Parse(v.Rows[0][property.Name].ToString()), null);
								}
							}
							else if (property.PropertyType.Name == "Decimal")
							{
								if (v.Rows.Count == 0 || v.Rows[0][property.Name].ToString() == "")
								{
									property.SetValue(item, 0M, null);
								}
								else
								{
									property.SetValue(item, decimal.Parse(v.Rows[0][property.Name].ToString()), null);
								}
							}
							else
							{
								if (v.Rows.Count == 0)
								{
									property.SetValue(item, "", null);
								}
								else
								{
									property.SetValue(item, v.Rows[0][property.Name].ToString(), null);
								}
							}
						}
						catch (Exception ex)
						{
							throw new Exception(ex.Message + "(" + propertyName + ")");
						}
					}
				}
			}

			return item;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="v"></param>
		/// <returns></returns>
		public static object ToModel<T>(this DataRow v) where T : new()
		{
			IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
			T item = new T();

			if (v != null)
			{
				string propertyName = "";
				foreach (var property in properties)
				{
					if (v.Table.Columns.Contains(property.Name))
					{
						propertyName = property.Name;
						try
						{
							if (property.PropertyType.Name == "DateTime")
							{
								if (v[property.Name].ToString() != "")
								{
									DateTime date = DateTime.Parse(v[property.Name].ToString());
									property.SetValue(item, date, null);
								}
							}
							else if (property.PropertyType.Name == "Int32" || property.PropertyType.Name == "Int64")
							{
								if (v[property.Name].ToString() == "")
								{
									property.SetValue(item, 0, null);
								}
								else
								{
									property.SetValue(item, int.Parse(v[property.Name].ToString()), null);
								}
							}
							else if (property.PropertyType.Name == "Decimal")
							{
								if (v[property.Name].ToString() == "")
								{
									property.SetValue(item, 0M, null);
								}
								else
								{
									property.SetValue(item, decimal.Parse(v[property.Name].ToString()), null);
								}
							}
							else
							{
								property.SetValue(item, v[property.Name], null);
							}
						}
						catch (Exception ex)
						{
							throw new Exception(ex.Message + "(" + propertyName + ")");
						}
					}
				}
			}

			return item;
		}

		/// <summary>
		/// 데이타를 list 객체로 리턴하기
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="datas"></param>
		/// <returns></returns>
		public static List<T> ToList<T>(this List<Dictionary<string, string>> datas) where T : new()
		{
			IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
			List<T> result = new List<T>();

			foreach (Dictionary<string, string> d in datas)
			{
				/*
                foreach (KeyValuePair<string, string> p in d)
                {
                    p.Key.ToString();
                    p.Value.ToString();
                }
                */
				var item = CreateItemFromRow<T>(d, properties);
				result.Add(item);
			}

			return result;
		}

		/// <summary>
		/// DataTable 데이타를 list 객체로 리턴하기
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="table"></param>
		/// <returns></returns>
		public static List<T> ToList<T>(this DataTable table) where T : new()
		{
			IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
			List<T> result = new List<T>();

			foreach (var row in table.Rows)
			{
				var item = CreateItemFromRow<T>((DataRow)row, properties);
				result.Add(item);
			}

			return result;
		}

		/// <summary>
		/// 데이타를 list 객체로 리턴하기
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="datas"></param>
		/// <returns></returns>
		public static List<T> ToList<T>(this ArrayList datas) where T : new()
		{
			IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
			List<T> result = new List<T>();

			foreach (object dataItem in datas)
			{
				if (dataItem is Hashtable)
				{
					var item = CreateItemFromRow<T>((Hashtable)dataItem, properties);
					result.Add(item);
				}
			}

			return result;
		}

		private static T CreateItemFromRow<T>(object v, IList<PropertyInfo> properties) where T : new()
		{
			T item = new T();

			string propertyName = "";
			foreach (var property in properties)
			{
				if (v is Dictionary<string, string>)
				{
					#region  Dictionary<string, string>
					Dictionary<string, string> vItem = (Dictionary<string, string>)v;

					if (property.PropertyType == typeof(System.DayOfWeek))
					{
						DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), vItem[property.Name].ToString());
						property.SetValue(item, day, null);
					}
					else
					{
						if (vItem.ContainsKey(property.Name))
						{
							propertyName = property.Name;
							try
							{
								if (property.PropertyType.Name == "DateTime")
								{
									if (vItem[property.Name].ToString() != "")
									{
										DateTime date = DateTime.Parse(vItem[property.Name].ToString());
										property.SetValue(item, date, null);
									}
								}
								else if (property.PropertyType.Name == "Int32" || property.PropertyType.Name == "Int64")
								{
									if (vItem[property.Name].ToString() == "")
									{
										property.SetValue(item, 0, null);
									}
									else
									{
										property.SetValue(item, int.Parse(vItem[property.Name].ToString()), null);
									}
								}
								else if (property.PropertyType.Name == "Decimal")
								{
									if (vItem[property.Name].ToString() == "")
									{
										property.SetValue(item, 0M, null);
									}
									else
									{
										property.SetValue(item, decimal.Parse(vItem[property.Name].ToString()), null);
									}
								}
								else
								{
									property.SetValue(item, vItem[property.Name], null);
								}
							}
							catch (Exception ex)
							{
								throw new Exception(ex.Message + "(" + propertyName + ")");
							}
						}
					}
					#endregion
				}
				else if (v is DataRow)
				{
					#region  DataRow
					DataRow vItem = (DataRow)v;

					if (property.PropertyType == typeof(System.DayOfWeek))
					{
						DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), vItem[property.Name].ToString());
						property.SetValue(item, day, null);
					}
					else
					{
						if (vItem.Table.Columns.Contains(property.Name))
						{
							propertyName = property.Name;

							if (property.PropertyType.Name == "DateTime")
							{
								if (vItem[property.Name].ToString() != "")
								{
									DateTime date = DateTime.Parse(vItem[property.Name].ToString());
									property.SetValue(item, date, null);
								}
							}
							else if (property.PropertyType.Name == "Int32" || property.PropertyType.Name == "Int64")
							{
								if (vItem[property.Name].ToString() == "")
								{
									property.SetValue(item, 0, null);
								}
								else
								{
									property.SetValue(item, int.Parse(vItem[property.Name].ToString()), null);
								}
							}
							else if (property.PropertyType.Name == "Decimal")
							{
								if (vItem[property.Name].ToString() == "")
								{
									property.SetValue(item, 0M, null);
								}
								else
								{
									property.SetValue(item, decimal.Parse(vItem[property.Name].ToString()), null);
								}
							}
							else
							{
								if (vItem[property.Name] == DBNull.Value)
									property.SetValue(item, null, null);
								else
									property.SetValue(item, vItem[property.Name].ToString(), null);
							}
						}
					}
					#endregion
				}
				else if (v is Hashtable)
				{
					#region  Hashtable
					Hashtable vItem = (Hashtable)v;

					if (property.PropertyType == typeof(System.DayOfWeek))
					{
						DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), vItem[property.Name].ToString());
						property.SetValue(item, day, null);
					}
					else
					{
						if (vItem.Contains(property.Name))
						{
							propertyName = property.Name;

							if (property.PropertyType.Name == "DateTime")
							{
								if (vItem[property.Name].ToString() != "")
								{
									DateTime date = DateTime.Parse(vItem[property.Name].ToString());
									property.SetValue(item, date, null);
								}
							}
							else if (property.PropertyType.Name == "Int32" || property.PropertyType.Name == "Int64")
							{
								if (vItem[property.Name].ToString() == "")
								{
									property.SetValue(item, 0, null);
								}
								else
								{
									property.SetValue(item, int.Parse(vItem[property.Name].ToString()), null);
								}
							}
							else if (property.PropertyType.Name == "Decimal")
							{
								if (vItem[property.Name].ToString() == "")
								{
									property.SetValue(item, 0M, null);
								}
								else
								{
									property.SetValue(item, decimal.Parse(vItem[property.Name].ToString()), null);
								}
							}
							else
							{
								if (vItem[property.Name] == DBNull.Value)
									property.SetValue(item, null, null);
								else
									property.SetValue(item, vItem[property.Name].ToString(), null);
							}
						}
					}
					#endregion
				}
			}
			return item;
		}

		/// <summary>
		/// 데이타 테이블을 데이타를 Dictionary 객체로 리턴하기
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public static Dictionary<string, string> ToDictionary(this DataTable table)
		{
			if (table.Rows.Count == 0) return null;

			Dictionary<string, string> result = new Dictionary<string, string>();

			foreach (DataRow row in table.Rows)
			{
				for (int i = 0; i < table.Columns.Count; i++)
				{
					string colName = table.Columns[i].ColumnName;
					string colValue = "";

					if (row[colName].ToString() != "")
					{
						colValue = row[colName].ToString();
					}

					result[colName] = colValue;
				}

				break;
			}

			return result;
		}

		/// <summary>
		/// 데이타 테이블을 데이타를 List Dictionary 객체로 리턴하기
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public static List<Dictionary<string, string>> ToDictionarys(this DataTable table)
		{
			List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

			foreach (DataRow row in table.Rows)
			{
				Dictionary<string, string> item = new Dictionary<string, string>();

				for (int i = 0; i < table.Columns.Count; i++)
				{
					string colName = table.Columns[i].ColumnName;
					string colValue = "";

					if (row[colName].ToString() != "")
					{
						colValue = row[colName].ToString();
					}

					item[colName] = colValue;
				}

				result.Add(item);
			}

			return result;
		}

		public static string AsString(this XmlDocument xmlDoc)
		{
			using (StringWriter sw = new StringWriter())
			{
				using (XmlTextWriter tx = new XmlTextWriter(sw))
				{
					xmlDoc.WriteTo(tx);
					string strXmlText = sw.ToString();
					return strXmlText;
				}
			}
		}

	}
	#endregion
}
