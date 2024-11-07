using Microsoft.Ajax.Utilities;
using Microsoft.JScript;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WebGrease.Configuration;
using WebUtil;

namespace Interlock.Controllers
{
	/// <summary>
	/// 
	/// </summary>
	[PageFilter, CustomAuthorize(Roles = UserType.User)]
	public class Sap_PnAExpenseReportController : BaseController
	{
		// GET: Sap_PnAExpenseReport
		public ActionResult Index()
		{
			return View();
		}

		const string errorMsg1 = "조회 중 오류가 발생하였습니다.";
		const string errorMsg2 = "데이터 처리 중 오류가 발생하였습니다.";

		#region [Page] P&A 경비 예산 Upload

		const string SubFilePath = "PnAExpenseReport_Sample";
		string LastExpenseBudgetGUID
		{
			get
			{
				string EB_GUID = null;
				string EB_GUID_END_TIME = null;

				try
				{
					EB_GUID_END_TIME = Session["LEB_GUID_END"].ToString();
					DateTime dt = new DateTime();
					if (DateTime.TryParse(EB_GUID_END_TIME, out dt) && DateTime.Now < dt)
					{
						EB_GUID = Session["LEB_GUID"].ToString();
					}
				}
				catch { }

				return EB_GUID;
			}
			set
			{
				Session["LEB_GUID"] = value;
				Session["LEB_GUID_END"] = DateTime.Now.AddMinutes(10).ToString("yyyy-MM-dd HH:mm:ss");
			}
		}

		/// <summary>
		/// P&A 경비 예산 Upload
		/// </summary>
		/// <returns></returns>
		public ActionResult ExpenseBudgetUpload()
		{
			try
			{
                util.DB.WriteDBLog("SEL", "0ms");

                /*
					@MODE
						YEAR: 연도별 조회
						ALL: 전체 조회

					@YEAR: 4자리 연도

					@OPT1
						1: (FY25 이상인 경우) IB 추가
						2: (FY25 이상인 경우) IB 추가, ADV 추가
				*/
                using (DataSet ds = util.GetList(@"SP_COSTCENTER_FUNCTION_SEL3", new { @MODE = "ALL", @OPT1 = "1" }, 600, false))
				{
					ViewBag.buList = util.Convert.DataTableToString(ds.Tables[0]);
				}
			}
			catch (Exception ex)
			{
				ErrorMsg(errorMsg1);
			}

			return View();
		}
		/// <summary>
		/// Expense Budget 엑셀 업로드
		/// </summary>
		/// <param name="inputFile"></param>
		/// <returns></returns>
		[HttpPost, AjaxFilter, CSRFFilter]
		public void ExpenseBudgetExcelUpload(HttpPostedFileBase inputFile)
		{
			string Rtn = "OK";

			try
			{
				string FiscalYear = GR("FiscalYear");
				string BusinessUnit = GR("selBusinessUnit");


				string filePath = FileRoot;
				string saveFileName = string.Empty;
				string fullPath = string.Empty;


				if (inputFile != null)
				{
					OleDbConnection oleDBcon = null;
					OleDbCommand oleDBCom = null;
					OleDbDataReader oleDBReader = null;

					try
					{
						Hashtable output2 = new Hashtable();
						output2.Add("Msg", "");

						Hashtable ht = util.File.FileUPload(filePath, inputFile, "");
						saveFileName = ht["FileName"].ToString();

						if (chkFileExtension(ht["FilePath"].ToString()))
						{
							fullPath = ht["FilePath"].ToString();

							string strProvider = string.Empty;

							strProvider = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + fullPath + "; Extended Properties=Excel 12.0";

							oleDBcon = new OleDbConnection(strProvider);
							oleDBcon.Open();
							//var dtTemp = oleDBcon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
							//string SheetName = dtTemp.Rows[0]["TABLE_NAME"].ToString();//"DATA";//Sheet1
							//string strQuery = "SELECT * FROM [" + SheetName + "]";
							//string strQuery = "SELECT * FROM [Sheet1$]";

							string strQuery = "SELECT * FROM [DATA$]";

							oleDBCom = new OleDbCommand(strQuery, oleDBcon);

							oleDBReader = oleDBCom.ExecuteReader();
							DataTable dt = new DataTable();
							dt.Load(oleDBReader);

							//Regex regex = new Regex(@"[0-9]");


							if (dt != null)
							{
								/*
                                    (임시테이블)삭제 > 등록 > 조회 순을 거치는데

                                    사용자(A,B) 2명이 동시에 시작하여

                                    A 삭제 > A 등록 > B 삭제 > A 조회시 A가 값을 조회 못하는 상황이 벌어질 수 있음

                                    관리자가 한명이여서 문제가 없다고 함 2024.05.08
                                */

								Guid guid = Guid.NewGuid();

								//TEMP 삭제
								util.Excute(@"SP_PE_BUDGET_TEMP_DEL", new { @TRANS_ID = guid });

								string sp_pp = "";
								string sp_real = "";

								if (dt.Columns.Count != 3) throw new Exception("잘못된 템플릿 입니다.");

								string filterExpression = "[F1] IS NOT NULL";
								string sp = "SP_PE_BUDGET_TEMP_INS";

								int seq = 1;
								//BU, 예실리포트 항목
								foreach (DataRow dr in dt.Select(filterExpression))
								{
									if (dr[0].ToString().ToUpper() == "BU" ||
										dr[1].ToString().ToUpper() == "예산항목 구분" ||
										dr[2].ToString().ToUpper() == "금액"
									) continue;


									DateTime dtNow = DateTime.Now;

									util.Excute(sp, new
									{
										@TRANS_ID = guid,
										@SEQ = seq,
										@FY = FiscalYear,
										@BU_CD = dr[0].ToString(),
										@ZSM_SEQ = dr[1].ToString(),
										@WKG = dr[2].ToString(),
										@CRNAM = UserData.DISPLAY_NAME,
										@CRDAT = dtNow.ToString("yyyyMMdd"),
										@CRTIM = dtNow.ToString("HHmmss")
									});

									seq++;
								}

								sp_pp = "SP_PE_BUDGET_TEMP_INS_PP";
								sp_real = "SP_PE_BUDGET_INS";


								// INSERT 후처리
								DataSet ds = util.GetList(sp_pp, new { @TRANS_ID = guid });

								bool isValid = false;
								string returnGuid = "";
								int cntTotal = 0;
								int cntSucced = 0;
								if (ds != null && 0 < ds.Tables.Count && 0 < ds.Tables[0].Rows.Count)
								{
									returnGuid = ds.Tables[0].Rows[0]["TRANS_ID"].ToString();

									isValid = returnGuid == guid.ToString()
									&& int.TryParse(ds.Tables[0].Rows[0]["CNT_TOTAL"].ToString(), out cntTotal)
									&& int.TryParse(ds.Tables[0].Rows[0]["CNT_SUCCED"].ToString(), out cntSucced)
									&& 0 < cntTotal && 0 < cntSucced && cntTotal == cntSucced;
								}

								LastExpenseBudgetGUID = guid.ToString();

								if (!isValid)
								{
									Rtn = "MSG";
								}
								else
								{
									string moveTempToReal(string sp1, Guid transID)
									{
										string value = "OK";

										DateTime dtNow = DateTime.Now;
										// SP_PE_BUDGET_INS

										util.Excute(sp1, new
										{
											@TRANS_ID = transID,
											@CRNAM = UserData.DISPLAY_NAME,
											@CRDAT = dtNow.ToString("yyyyMMdd"),
											@CRTIM = dtNow.ToString("HHmmss")
										});//, ref output);

										return value;
									}

									Rtn = moveTempToReal(sp_real, guid);
									// ViewBag.guid = guid;
								}

							}
							else
							{
								throw new Exception("Excel 파일의 데이터가 잘못 되었습니다.");
							}

						}
						else
						{
							throw new Exception("Excel 파일만 업로드 가능합니다.");
						}

					}
					catch (Exception ex)
					{

						//throw new Exception("Excel 파일이 잘못 되었습니다.");
						throw ex;
					}
					finally
					{
						if (oleDBReader != null)
							oleDBReader.Close();
						if (oleDBCom != null)
							oleDBCom.Dispose();
						if (oleDBcon != null)
							oleDBcon.Close();

						util.File.FileDelete(filePath, saveFileName);
					}
				}
				else if (inputFile == null)
				{

					//ErrorMsg("Excel 파일이 없습니다.");
					//Rtn = "ERR";
					throw new Exception("Excel 파일이 없습니다.");
				}
			}
			catch (Exception ex)
			{
				ErrorMsg("올바른 Excel 파일이 아닙니다.");
				Rtn = "ERR";
			}

			Print(Rtn);
		}
		/// <summary>
		/// Expense Budget 업로드 실패 내역 조회
		/// </summary>
		[HttpPost, AjaxFilter, CSRFFilter]
		public void ExpBudgetSelectFailed()
		{
			try
			{
				string FiscalYear = GR("FiscalYear");
				string BusinessUnit = GR("selBusinessUnit");
				string guid = LastExpenseBudgetGUID;

				object param = new { };

				if (!string.IsNullOrEmpty(guid))
				{
					param = new
					{
						@TRANS_ID = Guid.Parse(guid)
					};
				}

				string sp = "SP_PE_BUDGET_TEMP_SEL";

				using (DataSet ds = util.GetList(sp, param))
				{
					if (Ajax == true)
					{
						Print(util.Convert.DataSetToString(ds));
					}

					//ViewBag.ds = ds;
				}
			}
			catch (Exception ex)
			{
				ErrorMsg(errorMsg1);
			}
		}
		/// <summary>
		/// Expense Budget 업로드 성공 내역 조회
		/// </summary>
		[HttpPost, AjaxFilter, CSRFFilter]
		public void ExpBudgetSelect()
		{
			try
			{
				string FiscalYear = GR("FiscalYear");
				string BusinessUnit = GR("selBusinessUnit");
				string guid = LastExpenseBudgetGUID;

				object param = new { };

				if (!string.IsNullOrEmpty(guid))
				{
					param = new
					{
						@S_TYPE = "T",
						@TRANS_ID = Guid.Parse(guid)
					};
				}
				else
				{
					param = new
					{
						@S_TYPE = "",
						@FY = FiscalYear,
						@BU_CD = BusinessUnit,
					};
				}

				string sp = "SP_PE_BUDGET_SEL";

				// partnerNo

				using (DataSet ds = util.GetList(sp, param))
				{
					if (Ajax == true)
					{
						Print(util.Convert.DataSetToString(ds));
					}

					//ViewBag.ds = ds;
				}
			}
			catch (Exception ex)
			{
				ErrorMsg(errorMsg1);
			}
			finally
			{
				LastExpenseBudgetGUID = "";
			}
		}
		/// <summary>
		/// Expense Budget 삭제
		/// </summary>
		[HttpPost, AjaxFilter, CSRFFilter]
		public void ExpBudgetDelete()
		{
			Hashtable output = new Hashtable();
			output.Add("ERROR_MSG", "");

			try
			{
				string datas = GlobalObject.decodeURIComponent(GR("reqBody"));
				dynamic reqBody = JsonConvert.DeserializeObject<dynamic>(datas);

				string sp = "SP_PE_BUDGET_DEL";
				string[] namesForJson = null;

				namesForJson = new string[] { "FY", "BU_CD", "ZSM_SEQ" };

				IDictionary<string, object> param = new Dictionary<string, object>();
				bool isError = false;
				foreach (dynamic dy in reqBody.rows)
				{
					param = new Dictionary<string, object>();

					foreach (string name in namesForJson)
					{
						param.Add(name, dy[name]);
					}
					DateTime dtNow = DateTime.Now;
					param.Add("CRNAM", UserData.DISPLAY_NAME);
					param.Add("CRDAT", dtNow.ToString("yyyyMMdd"));
					param.Add("CRTIM", dtNow.ToString("HHmmss"));

					util.Excute(sp, param, ref output);
					if (output["ERROR_MSG"].ToString() != "")
					{
						isError = true;
						throw new Exception(output["ERROR_MSG"].ToString());
					}
				}

				Print($"OK||삭제되었습니다.");
			}
			catch (Exception ex)
			{
				Print(ex.Message);
			}
		}
		/// <summary>
		/// Expense Budget 수정
		/// </summary>
		[HttpPost, AjaxFilter, CSRFFilter]
		public void ExpBudgetUpdate()
		{
			Hashtable output = new Hashtable();
			output.Add("ERROR_MSG", "");

			try
			{
				string datas = GlobalObject.decodeURIComponent(GR("reqBody"));
				dynamic reqBody = JsonConvert.DeserializeObject<dynamic>(datas);

				string sp = "SP_PE_BUDGET_UPD";
				//string[] namesForJson = null;
				List<string> namesForJson = new List<string>()
				{
					{"FY"},
					{"BU_CD"},
					{"ZSM_SEQ"}
				};

				IDictionary<string, object> param = new Dictionary<string, object>();
				bool isError = false;
				foreach (dynamic dy in reqBody.rows)
				{
					param = new Dictionary<string, object>();

					foreach (string name in namesForJson)
					{
						param.Add(name, dy[name]);
					}
					DateTime dtNow = DateTime.Now;
					param.Add("CRNAM", UserData.DISPLAY_NAME);
					param.Add("CRDAT", dtNow.ToString("yyyyMMdd"));
					param.Add("CRTIM", dtNow.ToString("HHmmss"));

					util.Excute(sp, param, ref output);
					if (output["ERROR_MSG"].ToString() != "")
					{
						isError = true;
						throw new Exception(output["ERROR_MSG"].ToString());
					}
				}

				Print($"OK||저장되었습니다.");
			}
			catch (Exception ex)
			{
				Print(ex.Message);
			}
		}
		/// <summary>
		/// TemplatesDown [E]xpense[B]udget[U]pload
		/// </summary>
		[HttpPost, AjaxFilter, CSRFFilter]
		public ActionResult GetTemplatesDownEBU()
		{
			string fullPath = "";
			try
			{
				string fileName = "P&A_경비_Plan_Upload_template.xlsx?v="+DateTime.Now.ToString("yyyyMMddhhmmss");

				fullPath = "/" + FileRootDirName + "/" + SubFilePath + "/" + fileName;
			}
			catch (Exception ex)
			{
				ErrorMsg(errorMsg1);
			}
			return Content(fullPath);
		}
		#endregion

		#region [Page] P&A 경비 예실관리 리포트(PTR)
		/// <summary>
		/// P&A 경비 예실관리 리포트(PTR)
		/// </summary>
		/// <returns>ExpenseBudgetReport를 그대로 리턴함</returns>
		public ActionResult PtrExpenseBudgetReport()
		{
            util.DB.WriteDBLog("SEL", "0ms");

            // 기능 적으로 같아서 추가 개발을 하지 않음 나중에 달라지면 추가 개발
            return ExpenseBudgetReport();
		}
		#endregion

		#region [Page] P&A 경비 예실관리 리포트
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ActionResult ExpenseBudgetReport()
		{
			try
			{
				ViewBag.SelectCostCenterUrl = "/Custom/SelectCostCenter";

				util.DB.WriteDBLog("SEL", "0ms");

                //UserData.UNIT_CODE
                string UUC = UserData.UNIT_CODE;
				string GROWTH_UC = "32001627";//Growth 32001627 
				bool IsGrowth = false;

				using (DataSet ds = util.GetList(@"SP_DEPT_DEPTH_YN", new { @UNIT_CODE = UUC, @TAR_UNIT_CODE = GROWTH_UC }, 600, false))
				{
					//ViewBag.DD = util.Convert.DataTableToString(ds.Tables[0]);
					//IsGrowth = System.Convert.ToBoolean(ds.Tables[0].Rows[0][0].ToString());
					IsGrowth = util.Convert.StringToBool(ds.Tables[0].Rows[0][0].ToString());
				}

				#region 권한 설정
				string IT_CHECK_YN = PageCheckAuthFn("IT_CHECK").Equals(true) ? "Y" : "N";//IT
				string BU_CHECK_YN = PageCheckAuthFn("BU_CHECK").Equals(true) ? "Y" : "N";//Finance
				string PT_CHECK_YN = PageCheckAuthFn("PT_CHECK").Equals(true) ? "Y" : "N";//Pliot Tester
				string SMD_VIEW_YN = PageCheckAuthFn("SMD_VIEW").Equals(true) ? "Y" : "N";//SMD Above
				string BU_ADMIN_YN = PageCheckAuthFn("BU_ADMIN").Equals(true) ? "Y" : "N";//BU Admin
				string BL_ADMIN_YN = PageCheckAuthFn("BL_ADMIN").Equals(true) ? "Y" : "N";//BL Admin
				string BU_HEAD_YN = PageCheckAuthFn("BU_HEAD").Equals(true) ? "Y" : "N";//BU HEAD
				string AuthLevel = "";
				string BU_CD = "";

				/*
					우선순위 1
					
					BU 부문장, BL_ADMIN 이 있는 사람은 BU_CHECK 가 있어도 BL_ADMIN 이 우선

					1. 구분 FIRM 기본값
					2. BU값에 본인 소속 BU값으로 기본값이 표시되고 비활성
					3. Firm에 본인 BU값만 표시 됨
				*/
				if (BL_ADMIN_YN.Equals("Y"))
				{
					AuthLevel = "BL_ADMIN";
				}
				/*
					우선순위 1

					BU Admin, 조건은 BU 부문장과 동일
				*/
				else if (BU_ADMIN_YN.Equals("Y"))
				{
					AuthLevel = "BU_ADMIN";
				}
				/*
					우선순위 2

					관리자(재경부, OCFO):  BU, Parnter, 구분 제한 없음, 모두 선택 가능
					+ IT 권한
				*/
				else if (IT_CHECK_YN.Equals("Y") || BU_CHECK_YN.Equals("Y"))
				{
					AuthLevel = "ADMIN";
				}
				/*
					우선순위 3

					BU 부서장

					- 구분 BU 기본값, Frim 선택 못함 
					- BU값에 본인 소속 BU값으로 기본값이 표시되고 비활성
					- CostCenter에 본인이 조회할 수 있는 값(권한이 있는 CC와 본인CC) List만 표시됨
					- BU와 Detail Tab만 조회 
				 */
				else if (BU_HEAD_YN.Equals("Y"))
				{
					AuthLevel = "BU_HEAD";
				}
				#endregion

				// AuthLevel = "BL_ADMIN"; // 테스트용 삭제 필요
				//AuthLevel = "BU_HEAD";

				if (string.IsNullOrEmpty(AuthLevel)) ErrorMove("권한이 없습니다.", "/");

				BU_CD = UserData.BU_CD;
				//기존 BU 값 REPLACE 그리고 FY25 기준으로 Replace
				BU_CD = BU_CD.Replace("AD", "AA").Replace("ER", "FA").Replace("RA", "FA");

                // ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
                // ▼ 테스트 데이터
                // ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼
                //AuthLevel = "BU_HEAD";
                //BU_CD = "FA";
                // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲
                // ▲ 테스트 데이터
                // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

                string bu_cd_fy25 = "";
				string PCS = UserData.PERSON_CODE_SWT;
				//
				// Growth	손재호	13008587	변경원	13000820 : CS -> IB
				if (true)
				{
					if (new string[] { "13008587", "13000820" }.Contains(PCS)) bu_cd_fy25 = "IB";
					else if (IsGrowth) bu_cd_fy25 = "IB";//Growth 소속이면 BU 를 Growth 설정
					else if (bu_cd_fy25.ToUpper() == "RA") bu_cd_fy25 = "FA";
				}

				ViewBag.PCS = UserData.PERSON_CODE_SWT;
				ViewBag.PartnerInfo = UserData.UNIT_NAME + " " + UserData.DISPLAY_NAME + " " + UserData.LEVEL_NAME;
				
				
				ViewBag.BU_CD = BU_CD;
				ViewBag.BU_CD_FY25 = bu_cd_fy25;
				ViewBag.BU_NM = UserData.BU_NM;
				ViewBag.AuthLevel = AuthLevel;
				ViewBag.dtGubun = GetGubunData(AuthLevel);

				// UserData

				//Company Code
				using (DataSet ds = util.GetList(@"SP_COMPANY_SEL", new { @Page = 1, @ListLimit = 10 }, 600, false))
				{
					ViewBag.dt_CC = ds.Tables[0];
				}

				if (AuthLevel == "BU_HEAD")
				{
					string PC = UserData.PERSON_CODE;
					PC = "10115"; // 테스트용 삭제 필요

					using (DataSet ds = util.GetList(@"SP_EMPNO_AUTH_SEL", new { @PERSON_CODE = PC, @GROUP_CODE = "MORE_AUTH_CC" }, 600, false))
					{
						// ViewBag.dt_CostCenterAll
						string CostCenterAll = "";
						foreach (DataRow dr in ds.Tables[0].Rows)
						{
							if (!string.IsNullOrEmpty(CostCenterAll)) CostCenterAll += ",";
							CostCenterAll += dr["KOSTL"].ToString();
						}
						ViewBag.CostCenterAll = CostCenterAll;
						ViewBag.dt_CostCenter = ds.Tables[0];
					}
				}

				/*
					@MODE
						YEAR: 연도별 조회
						ALL: 전체 조회

					@YEAR: 4자리 연도

					@OPT1
						1: (FY25 이상인 경우) IB 추가
						2: (FY25 이상인 경우) IB 추가, ADV 추가
				*/
				using (DataSet ds = util.GetList(@"SP_COSTCENTER_FUNCTION_SEL3", new { @MODE = "ALL", @OPT1 = "1" }, 600, false))
				{

					if (new string[] { "BL_ADMIN", "BU_ADMIN" }.Contains(AuthLevel))
					{
						string where = "CostCenterFunction = '" + BU_CD + "'";
						if (!string.IsNullOrEmpty(bu_cd_fy25)) where += " OR CostCenterFunction = '" + bu_cd_fy25 + "'";

						DataRow[] selectedRows = ds.Tables[0].Select(where); // 조회된 DataRow[]
						// 새로운 DataTable 생성 (구조 복사)
						DataTable newDataTable = ds.Tables[0].Clone();
						// 조회된 행들을 새로운 DataTable에 추가
						foreach (DataRow row in selectedRows) newDataTable.Rows.Add(row.ItemArray);

						ViewBag.buList = util.Convert.DataTableToString(newDataTable);
					}
					else
					{
						ViewBag.buList = util.Convert.DataTableToString(ds.Tables[0]);
					}
				}


			}
			catch (Exception ex)
			{
				ErrorMsg(errorMsg1);
			}

			return View();
		}

		/// <summary>
		/// 
		/// </summary>
		[HttpPost, CSRFFilter]
		public void EBReportSelectDetail()
		{
			//dynamic params1 = new ExpandoObject();
			try
			{
				string AuthLevel = GR("AuthLevel");
				string keyMonth = GR("txtKeyMonth").Replace("-", "");
				string gubun = GR("selGubun");
				string companyCode = GR("selCompanyCode");
				string costCenter = GR("ctrlCostCenter");
				string businessUnit = GR("selBusinessUnit");
				string selType = GR("selType").ToUpper();
				int page_idx = 1;
				int page_size = 50;

				int.TryParse(GR("pq_curpage"), out page_idx);
				int.TryParse(GR("pq_rpp"), out page_size);

				// 김범섭 작업중
				dynamic param = new ExpandoObject();
				string sp = "";

				param.@KEY_MONTH = keyMonth;
				param.@COMPANY_CD = companyCode;
				// 이론상 @COST_CENTER는 무한으로 늘어날 수 있음 현재는 VARCHAR(1000)으로
				param.@COST_CENTER = costCenter;
				if (AuthLevel != "BU_HEAD")
				{
					param.@BUSINESS_UNIT = businessUnit;
				}
				param.@IDX = page_idx;
				param.@SIZE = page_size;

				sp = $"SP_PE_BUDGET_REPORT_DETAIL_SEL";

				using (DataSet ds = util.GetList(sp, param))
				{

					DataTable dt1 = ds.Tables[1];
					StringBuilder sb = new StringBuilder(@"{""totalRecords"":" + dt1.Rows[0]["CNT"] + @",""curPage"":" + page_idx.ToString() + @",""data"":");
					DataTable dt2 = ds.Tables[0];
					sb.Append(JsonConvert.SerializeObject(dt2));
					sb.Append("}");
					Print(sb.ToString());
				}
			}
			catch (Exception ex)
			{
				StringBuilder sb = new StringBuilder(@"{""totalRecords"":" + 0.ToString() + @",""curPage"":" + 1.ToString() + @",""data"": [] }");
				Print(sb.ToString());
			}
		}


		/// <summary>
		/// 
		/// </summary>
		[HttpPost, AjaxFilter, CSRFFilter]
		public ActionResult EBReportSelectDetail_Excel()
		{
			string fullPath = "";
			try
			{
				string AuthLevel = GR("AuthLevel");
				string keyMonth = GR("txtKeyMonth").Replace("-", "");
				string gubun = GR("selGubun");
				string companyCode = GR("selCompanyCode");
				string costCenter = GR("ctrlCostCenter");
				string businessUnit = GR("selBusinessUnit");
				string selType = GR("selType").ToUpper();
				int page_idx = 1;
				int page_size = 50;

				dynamic param = new ExpandoObject();
				string sp = "";

				param.@KEY_MONTH = keyMonth;
				param.@COMPANY_CD = companyCode;
				param.@COST_CENTER = costCenter;

				if (AuthLevel != "BU_HEAD")
				{
					param.@BUSINESS_UNIT = businessUnit;
				}
				param.@IDX = page_idx;
				param.@SIZE = page_size;
				param.@MODE = "EXCEL";

				sp = $"SP_PE_BUDGET_REPORT_DETAIL_SEL";

				// 명확한 이유는 모르겠으나 CNT를 받는 DataTable이 있는 경우 조회가 잘 안되는 현상을 발견
				using (DataSet ds = util.GetList(sp, param))
				{

					string fileName = "PnA_Expense_Budget_Report_Detail_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv";

					if (IsDataEmpty(ds) || IsDataEmpty(ds.Tables[0])) throw new Exception("데이터가 없습니다.");

					string pathSaved = CsvHelper.SaveCSV(ds.Tables[0], FileRoot, fileName);
					fullPath = "/" + FileRootDirName + "/" + pathSaved;
				}

			}
			catch (Exception ex)
			{
				ErrorMsg(ex);
			}
			return Content(fullPath);
		}


		/// <summary>
		/// 
		/// </summary>
		[HttpPost, AjaxFilter, CSRFFilter]
		public void EBReportSelect()
		{
			//dynamic params1 = new ExpandoObject();
			try
			{
				string AuthLevel = GR("AuthLevel");
				string keyMonth = GR("txtKeyMonth").Replace("-", "");
				string gubun = GR("selGubun");
				string companyCode = GR("selCompanyCode");
				string costCenter = GR("ctrlCostCenter");
				string businessUnit = GR("selBusinessUnit");
				string selType = GR("selType").ToUpper();
				int page_idx = 1; ;
				int page_size = 50; 

				int.TryParse(GR("page_idx"), out page_idx);
				int.TryParse(GR("page_size"), out page_size);

				dynamic param = new ExpandoObject();
				string sp = "";

				param.@KEY_MONTH = keyMonth;
				if (AuthLevel != "BU_HEAD")
				{
					param.@BUSINESS_UNIT = businessUnit;
				}

				if (new string[] { "BU","DETAIL" }.Contains(selType))
				{
					param.@COMPANY_CD = companyCode;
					// 이론상 @COST_CENTER는 무한으로 늘어날 수 있음 현재는 VARCHAR(1000)으로
					param.@COST_CENTER = costCenter;
				}

				sp = $"SP_PE_BUDGET_REPORT_{selType.ToUpper()}_SEL";

				using (DataSet ds = util.GetList(sp, param))
				{
					if (Ajax == true)
					{
						Print(util.Convert.DataSetToString(ds));
					}
				}
			}
			catch (Exception ex)
			{

				ErrorMsg(errorMsg1);
			}
		}


		private DataTable GetGubunData(string AuthLevel)
		{
			DataTable dtGubun = new DataTable();
			dtGubun.Columns.Add("value");
			dtGubun.Columns.Add("name");

			DataRow dr = null;

			if (new string[] { "BL_ADMIN", "BU_ADMIN", "ADMIN" }.Contains(AuthLevel))
			{
				dr = dtGubun.NewRow();
				dr["name"] = "FIRM"; dr["value"] = "F"; dtGubun.Rows.Add(dr);
			}

			dr = dtGubun.NewRow();
			dr["name"] = "BU"; dr["value"] = "B";
			dtGubun.Rows.Add(dr);

			return dtGubun;
		}
		#endregion

		#region [popup] CC권한관리
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ActionResult CostCenterAuth()
		{
			ViewBag.GroupCode = "MORE_AUTH_CC";
			return View();
		}



		#endregion

		#region 공통
		/// <summary>
		/// 파일 유효성 검사
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		//[HttpPost, AjaxFilter, CSRFFilter]
		protected bool chkFileExtension(string filePath)
		{
			bool returnResult = false;
			string fileExtension = string.Empty;
			string[] xls = { "D0", "CF", "11", "E0", "A1", "B1", "1A", "E1" };  //Microsoft Office 97-2003 응용 프로그램(Word, Powerpoint, Excel, Wizard) 으로 doc, ppt, xls 모두 같음
			string[] xlsx = { "50", "4B", "03", "04", "14", "00", "06", "00" }; //Microsoft Office OOXML(Open XML 형식) 문서로 DOCX, PPTX, XLSX 모두 같음

			string[] file = BitConverter.ToString(util.File.ReadByte(filePath)).Split('-');

			if (file.Take(8).SequenceEqual(xls))
			{
				returnResult = true;
			}
			else if (file.Take(8).SequenceEqual(xlsx))
			{
				returnResult = true;
			}

			return returnResult;
		}

		string GR(string name) { return GetRequest(name).Trim(); }
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected string getFiscalYear()
		{
			DateTime dt = DateTime.Now;

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
		#endregion

		/// <summary>
		/// Information - Help
		/// </summary>
		/// <returns></returns>
		public ActionResult Help()
		{
			string sp = "SP_RPT_FlashReport_CODE_ITEM_SEL";


			using (DataSet ds = util.GetList(sp, new { @GROUP_CODE = "PAE" }))
			{
				ViewBag.CodeList = ds.Tables[0];
			}


			return View();
		}
	}
}