using Microsoft.JScript;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebUtil;
using Interlock.Models;
using System.Collections.Generic;
using System.Collections;

namespace Interlock.Controllers
{
    /// <summary>
    /// 모든 페이지에서 참조하는 기본 Controller
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// 공통 Util
        /// </summary>
        public Util util = null;
        public Mail mail = null;

        /// <summary>
        /// 로그인 사용자 정보
        /// </summary>
        public UserInfo UserData = new UserInfo();

        private string m_Action = "";
        private bool m_PageEnd = false;
        private string m_ErrorMessage = "";
        private CultureInfo m_Language = CultureInfo.CurrentUICulture;
        private bool m_modelstate_clear = true;
        private bool m_isSessionExpired = false;

        /// <summary>
        /// 생성자
        /// </summary>
        public BaseController()
        {
            util = new Util(Request);
            util.DbConnection = ConfigurationManager.AppSettings["DBConnection"].ToString();

            mail = new Mail(util);
            UserData.EMAIL = "";
            UserData.NAME = "";
            UserData.LEVLES = 0;

            // 언어지정하기
            Resources.resLanguage.Culture = m_Language;
        }

        /// <summary>
        /// 세션 세팅
        /// </summary>
        public void SessiotnSetting()
        {
            if (Session != null && Session["Email"] != null)
                UserData.EMAIL = Session["Email"].ToString();
            if (Session != null && Session["Name"] != null)
                UserData.NAME = Session["Name"].ToString();
            if (Session != null && Session["levels"] != null)
                UserData.LEVLES = util.Convert.ObjToInt(Session["Levels"].ToString());
            if (Session != null && Session["PERSON_CODE"] != null)
                UserData.PERSON_CODE = Session["PERSON_CODE"].ToString();
            if (Session != null && Session["PERSON_CODE_SWT"] != null)
                UserData.PERSON_CODE_SWT = Session["PERSON_CODE_SWT"].ToString();
            if (Session != null && Session["DISPLAY_NAME"] != null)
                UserData.DISPLAY_NAME = Session["DISPLAY_NAME"].ToString();
            if (Session != null && Session["DISPLAY_ENG_NAME"] != null)
                UserData.DISPLAY_ENG_NAME = Session["DISPLAY_ENG_NAME"].ToString();
            if (Session != null && Session["UNIT_CODE"] != null)
                UserData.UNIT_CODE = Session["UNIT_CODE"].ToString();
            if (Session != null && Session["LEVEL_CODE"] != null)
                UserData.LEVEL_CODE = Session["LEVEL_CODE"].ToString();
            if (Session != null && Session["SABUN"] != null)
                UserData.SABUN = Session["SABUN"].ToString();
            if (Session != null && Session["USAGE_STATE"] != null)
                UserData.USAGE_STATE = Session["USAGE_STATE"].ToString();
            if (Session != null && Session["UNIT_NAME"] != null)
                UserData.UNIT_NAME = Session["UNIT_NAME"].ToString();
            if (Session != null && Session["LEVEL_NAME"] != null)
                UserData.LEVEL_NAME = Session["LEVEL_NAME"].ToString();
            if (Session != null && Session["BU_NM"] != null)
                UserData.BU_NM = Session["BU_NM"].ToString();
            if (Session != null && Session["BU_CD"] != null)
                UserData.BU_CD = Session["BU_CD"].ToString();
        }

        #region -----------------Property------------------------------
        #endregion

        #region -----------------Page Value----------------------------
        /// <summary>
        /// Ajax 여부
        /// </summary>
        public bool Ajax
        {
            get
            {
                if (Request.Headers["requestType"] != null && Request.Headers["requestType"].ToString() == "ajax")
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// IP
        /// </summary>
        public string IP
        {
            get
            {
                try
                {
                    string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        string[] addresses = ipAddress.Split(',');
                        if (addresses.Length != 0)
                        {
                            return addresses[0];
                        }
                    }

                    return Request.ServerVariables["REMOTE_ADDR"];
                }
                catch
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// DomainUrl (도메인), ex) tripod.com:1234
        /// </summary>
        public string DomainUrl
        {
            get
            {
                return ((System.Web.HttpRequestWrapper)Request).Url.Authority;
            }
        }

        /// <summary>
        /// 현재 URL 가져오기
        /// </summary>
        public string NowUrl
        {
            get
            {
                return Request.Url.ToString();
            }
        }

        /// <summary>
        /// 물리적인 저장파일 경로
        /// </summary>
        public string FileRootDirName
        {
			get
			{
				return "_Data";
			}
		}

        /// <summary>
        /// 물리적인 저장파일 경로
        /// </summary>
        public string FileRoot
        {
            get
            {
                return Server.MapPath("~/" + FileRootDirName);
            }
        }

        /// <summary>
        /// Http 포함 도메인 정보
        /// </summary>
        public string RootUrl
        {
            get
            {
                return "https://" + Request.ServerVariables["HTTP_HOST"].ToString();
            }
        }

        /// <summary>
        /// 첨부파일 Url 정보
        /// </summary>
        public string AttachFileUrl
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// 에러 유무
        /// </summary>
        public bool Error
        {
            get
            {
                if (m_Action == "ERROR")
                    return false;
                else
                    return false;
            }
        }

        /// <summary>
        /// View에 Model 정보 Clear
        /// </summary>
        public bool ModelStateClear
        {
            get
            {
                return m_modelstate_clear;
            }
            set
            {
                m_modelstate_clear = value;
            }
        }
        #endregion

        #region -----------------Page Function-------------------------
        /// <summary>
        /// 값 존재여부 체크
        /// </summary>
        /// <param name="CheckValue"></param>
        /// <returns></returns>
        public bool IsValue(string CheckValue)
        {
            return util.IsValue(CheckValue);
        }

        /// <summary>
        /// 값 존재여부 체크 멀티
        /// </summary>
        /// <param name="CheckValues"></param>
        /// <returns></returns>
        public bool IsValue(string[] CheckValues)
        {
            bool rtnValue = true;

            for (int i = 0; i < CheckValues.Length; i++)
            {
                if (IsValue(CheckValues[i]) == false)
                {
                    rtnValue = false;
                    break;
                }
            }

            return rtnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="CheckValues"></param>
        /// <returns></returns>
        public bool IsCheck(string value, string[] CheckValues)
        {
            return util.IsCheck(value, CheckValues);
        }


        /// <summary>
        /// 데이터 없는 경우 True
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public bool IsDataEmpty(DataTable dt)
        {
            bool isDataEmpty = false;
            try
            {
                if (dt == null) return true;
                if (dt.Rows.Count == 0) return true;
            }
            catch
            {
				isDataEmpty = true;
            }
            return isDataEmpty;
        }

		/// <summary>
		/// 데이터 없는 경우 True
		/// </summary>
		/// <param name="ds"></param>
		/// <returns></returns>
		public bool IsDataEmpty(DataSet ds)
        {
			bool isDataEmpty = false;
			try
			{
				if (ds == null) return true;
                if (ds.Tables.Count == 0) return true;
                foreach (DataTable dt in ds.Tables) {
                    if (!IsDataEmpty(dt)) return false;
				}
			}
			catch
			{
				isDataEmpty = true;
			}
			return isDataEmpty;
		}

        /// <summary>
        /// 사용자 변수 값
        /// </summary>
        /// <param name="name">param명</param>
        /// <returns></returns>
        [CustomAuthorize(Roles = UserType.User)]
        public string GetRequest(string name)
        {
            return GetRequest(name, "");
        }

        /// <summary>
        /// 사용자 변수 값
        /// </summary>
        /// <param name="name"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        [CustomAuthorize(Roles = UserType.User)]
        public string GetRequest(string name, string DefaultValue)
        {
            if (util.Request == null) util.Request = this.Request;
            string RtnValue = util.GetRequest(name, DefaultValue);

            return RtnValue;
        }

        /// <summary>
        /// 쿠키 값
        /// </summary>
        /// <param name="name">쿠키명</param>
        /// <returns></returns>
        public string GetCookie(string name)
        {
            return GetCookie(name, "");
        }

        /// <summary>
        /// 쿠키 값
        /// </summary>
        /// <param name="name">쿠키명</param>
        /// <param name="DefulatValue">기본값</param>
        /// <returns></returns>
        public string GetCookie(string name, string DefulatValue)
        {
            if (util.Request == null) util.Request = this.Request;
            return util.GetCookie(name, DefulatValue);
        }

        /// <summary>
        /// 쿠키 저장
        /// </summary>
        /// <param name="name">쿠키명</param>
        /// <param name="Value">값</param>
        /// <param name="Expires">저장기간 (day)</param>
        public void SetCookie(string name, string Value, int Expires = 365)
        {
            if (util.Response == null) util.Response = this.Response;
            util.SetCookie(name, Value, Expires);
        }

        /// <summary>
        /// 내용 가지고 오기 (HTML)
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="datas"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        [CustomAuthorize(Roles = UserType.User)]
        public string GetHtml(string FileName, object datas = null, DataSet ds = null)
        {
            string bodyHtml = util.File.Read(Server.MapPath(FileName));

            if (ds != null)
                bodyHtml = util.GetHtml(ds, bodyHtml);

            if (datas != null)
            {
                bodyHtml = util.GetHtml(datas, bodyHtml);
            }

            return bodyHtml;
        }

        /// <summary>
        /// 로그 저장
        /// </summary>
        /// <param name="Msg">로그메시지</param>
        public void ErrorLog(string Msg)
        {
            util.Excute(@"
                INSERT INTO TB_ERROR (BODY)
                VALUES (@BODY)
            ", new
            {
                @BODY = Msg
            });
        }

        /// <summary>
        /// 디버그 로그 저장
        /// </summary>
        /// <param name="Msg">로그 메시지</param>
        public void Debug(string Msg)
        {
        }

        /// <summary>
        /// 에러 로그 저장
        /// </summary>
        /// <param name="ex">Exception</param>
        public void ErrorMsg(Exception ex)
        {
            if (m_Action != "ERROR")
            {
                m_Action = "ERROR";
                m_ErrorMessage = ex.Message.ToString();

                try
                {
                    ErrorLog("ErrorMsg 오류 (" + m_ErrorMessage + ")");
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 에러 처리
        /// </summary>
        /// <param name="Msg">메시지</param>
        public void ErrorMsg(string Msg)
        {
            if (m_Action != "ERROR")
            {
                m_Action = "ERROR";
                m_ErrorMessage = Msg;
                throw new Exception();
            }
        }

        /// <summary>
        /// 세션 세팅
        /// </summary>
        public void PageStart(ActionExecutingContext filterContext)
        {
            string action = this.ControllerContext.RouteData.Values["action"].ToString();
            string controller = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewBag.ActionName = action;
            ViewBag.ControllerName = controller;
            m_isSessionExpired = false;

            //if ((controller.ToLower() == "invoices")
            //{
            //    if (ConfigurationManager.AppSettings["DBConnectionQA"] != null)
            //    {
            //        util.DbConnection = ConfigurationManager.AppSettings["DBConnectionQA"].ToString();
            //    }
            //}

            if (controller.ToUpper() == "MNG")
            {
                ViewBag.MenuDs = util.GetList("SP_MNG_MENU_SEL", 30, false);
            }
            else if (controller.ToUpper() != "LOGIN")
            {
                if (UserData.PERSON_CODE != null)
                {
                    ViewBag.MenuDs = util.GetList("SP_USER_MENU_PAGE_SEL", new { @PERSON_CODE = UserData.PERSON_CODE, @UNIT_CODE = UserData.UNIT_CODE, @CONTROLS = controller, @ACTION = action }, 30, false);

                    if (!controller.ToUpper().Equals("BASE"))//controller: Base, action: Main - 오류나서 제외
                    {
                        //권한없는 페이지 직접 접근 제어
                        DataRow[] page_idx_dr = ViewBag.MenuDs.Tables[3].Select("CONTROLS = '" + controller + "' AND ACTION = '" + action + "'");
                        if (page_idx_dr.Length == 0 && !Ajax)
                        {
                            filterContext.Result = new ViewResult { ViewName = "~/Views/LogOut.cshtml" };
                            var v = filterContext.Result as ViewResult;
                            v.ViewBag.ErrorMsg = m_ErrorMessage;
                        }
                    }
                }
                // Session expired
                else
                {
                    m_isSessionExpired = true;
                }
            }
        }

        /// <summary>
        /// 화면별 권한기능 체크 
        /// </summary>
        /// <param name="CheckFn"></param>
        /// <returns></returns>
        public bool PageCheckAuthFn(string CheckFn)
        {
            DataTable dt = ((DataSet)(ViewBag.MenuDs)).Tables[4];

            if (dt.Select("IS_ALL_AUTH = 'Y'").Length > 0 || dt.Select("ADD_AUTH_FN LIKE '%" + CheckFn + ",%'").Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 페이지 종료 처리, View 실행전
        /// </summary>
        /// <param name="filterContext"></param>
        public void PageEnd(ActionExecutedContext filterContext)
        {
            if (ConfigurationManager.AppSettings["DBConnectionQA"] != null)
            {
                if (util.DBConnectStrCheck(ConfigurationManager.AppSettings["DBConnectionQA"].ToString()))
                {
                    util.DbConnection = ConfigurationManager.AppSettings["DBConnection"].ToString();
                }
            }

            if (ViewBag.ActionName == null || ViewBag.ActionName == "")
                ViewBag.ActionName = this.ControllerContext.RouteData.Values["action"].ToString();
            if (ViewBag.ControllerName == null || ViewBag.ControllerName == "")
                ViewBag.ControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            if (m_Action == "ERROR")
            {
                if (Ajax)
                {
                    string prefixMsg = m_isSessionExpired ? "|||[CSRFERROR]|||" : "|||[ERROR]|||";

                    Response.Write(prefixMsg + m_ErrorMessage);
                    m_PageEnd = true;
                }
                else
                {
                    filterContext.Result = new ViewResult { ViewName = "~/Views/Error.cshtml" };
                    var v = filterContext.Result as ViewResult;
                    v.ViewBag.ErrorMsg = m_ErrorMessage;
                }
            }

            if (m_PageEnd == false)
            {
                ViewBag.Ajax = Ajax;
                ViewBag.util = util;
                ViewBag.User = UserData;
                ViewBag.FileRoot = FileRoot;
                ViewBag.csrf_token = CSRF_SetToken();

                if (ViewBag.AttachFileUrl == null || ViewBag.AttachFileUrl == "")
                    ViewBag.AttachFileUrl = AttachFileUrl;
            }
            else
            {
                filterContext.Result = new EmptyResult();
            }

            ViewBag.SiteName = "Interlock";

            // View에서 컨트롤의 value를 Model로 자동 매칭하는 부분을 작동안되게 하기 위해서 ModelState.Clear(); 처리
            if (m_modelstate_clear == true)
                ModelState.Clear();
        }

        /// <summary>
        /// 페이지 종료, View 실행 후
        /// </summary>
        public void Close()
        {
            if (util != null)
                util.Dispose();
        }

        /// <summary>
        /// 화면출력
        /// </summary>
        /// <param name="Msg">메시지</param>
        /// <param name="PageEnd">페이지 종료 처리</param>
        public void Print(string Msg, bool PageEnd = true)
        {
            m_PageEnd = PageEnd;
            Response.Write(Msg);
        }

        /// <summary>
        /// 화면출력
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="url"></param>
        public void ErrorMove(string Msg, string url = "")
        {
            m_PageEnd = true;
            Response.Write("<script>alert(\"" + Msg + "\");");

            if (url == "")
            {
                Response.Write("history.go(-1);</script>");
            }
            else
            {
                Response.Write("location.href=\"" + url + "\";</script>");
            }

        }

        /// <summary>
        /// 기초코드 가져오기
        /// </summary>
        /// <param name="groupCode"></param>
        /// <returns></returns>
        public DataTable GetBaseCode(string groupCode)
        {
            try
            {
                using (DataSet ds = util.GetList(@"SP_GET_BASE_CODE", new
                {
                    @GROUP_CODE = groupCode
                }))
                {
                    return ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }



        }
        #endregion

        #region -----------------Authorize-----------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string CSRF_SetToken(string key = "user")
        {
            if (Session["csrf_token_" + key] != null && Session["csrf_token_" + key].ToString() != "")
            {
                return Session["csrf_token_" + key].ToString();
            }
            else
            {
                if (key == "Login")
                {
                    string csrf_token = util.UniqStringRNG("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", 30);

                    // 세션이 로그인에서는 사라지기 때문에 세션에 담을수 없음
                    util.Excute("SP_CHECK_LOGIN_INS", new { @CSRF_TOKEN = csrf_token }, 30, false);

                    return csrf_token;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool CSRF_LoginCheckToken(string value)
        {
            try
            {
                Hashtable csrf_token_Login = util.GetData("SP_CHECK_LOGIN_CSRF_SEL", new { @CSRF_TOKEN = value }, true, 30, false);

                if (csrf_token_Login == null || IsValue(value) == false)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string CSRF_SetToken(string key)
        {
            string[] value_arr = util.Split(key, "");
            string[] date_arr = util.Split(DateTime.Now.ToString("ddHHmmddHHmmddHHmmddHHmmddHHmmddHHmmddHHmmddHHmmddHHmmddHHmmddHHmm"), "");
            StringBuilder csrf_token = new StringBuilder();

            for (int i = 0; i < value_arr.Length; i++)
            {
                string val = value_arr[i];
                string date_val = date_arr[i];
                if (val != "")
                {
                    csrf_token.Append(date_val);
                    csrf_token.Append(val);
                }
            }
            csrf_token.Append("_" + DateTime.Now.ToString("yyyyMMddHHmm"));

            Session["csrf_token_" + key] = GlobalObject.encodeURIComponent(util.AesEnc(csrf_token.ToString()));
            return Session["csrf_token_" + key].ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        protected bool CSRF_CheckToken(string key, string value, int time = 60 * 12)
        {
            try
            {
                if (Session["csrf_token_" + key] == null || IsValue(value) == false)
                {
                    return false;
                }

                #region 사용자에서 넘어온 키정보
                string csrf_org_token = util.AesDec(GlobalObject.decodeURIComponent(value));
                string[] csrf_token_arr = util.Split(csrf_org_token, "_");

                // 키정보 유효 시간
                long keyDate = util.Convert.ObjToLong(csrf_token_arr[1]);
                long checkDate = util.Convert.ObjToLong(DateTime.Now.AddMinutes(time * -1).ToString("yyyyMMddHHmm"));

                if (keyDate < checkDate)
                {
                    return false;
                }

                string[] csrf_arr = util.Split(csrf_token_arr[0], "");
                StringBuilder csrf_token = new StringBuilder();

                for (int i = 1; i < csrf_arr.Length; i++)
                {
                    if (csrf_arr[i] != "")
                    {
                        if (i % 2 == 0)
                        {
                            csrf_token.Append(csrf_arr[i]);
                        }
                    }
                }
                #endregion

                #region 세션에 있는 키정보
                string csrf_org_token_session = util.AesDec(GlobalObject.decodeURIComponent(Session["csrf_token_" + key].ToString()));
                string[] csrf_org_token_session_arr = util.Split(csrf_org_token_session, "_");
                string[] csrf_session_arr = util.Split(csrf_org_token_session_arr[0], "");
                StringBuilder csrf_token_session = new StringBuilder();

                for (int i = 1; i < csrf_session_arr.Length; i++)
                {
                    if (csrf_session_arr[i] != "")
                    {
                        if (i % 2 == 0)
                        {
                            csrf_token_session.Append(csrf_session_arr[i]);
                        }
                    }
                }
                #endregion

                if (csrf_token.ToString() == csrf_token_session.ToString())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        */

        /// <summary>
        /// 사용자 정보 Class
        /// </summary>
        public class UserInfo
        {
            /// <summary>
            /// 이메일
            /// </summary>
            public string EMAIL { get; set; }

            /// <summary>
            /// 이름
            /// </summary>
            public string NAME { get; set; }

            /// <summary>
            /// 레벨
            /// </summary>
            public int LEVLES { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string PERSON_CODE { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string PERSON_CODE_SWT { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string DISPLAY_NAME { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string DISPLAY_ENG_NAME { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UNIT_CODE { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string LEVEL_CODE { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string SABUN { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string USAGE_STATE { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UNIT_NAME { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string LEVEL_NAME { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string BU_CD { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string BU_NM { get; set; }

        }

        /// <summary>
        /// User 구분
        /// </summary>
        public enum UserType
        {
            /// <summary>
            /// 게스트
            /// </summary>
            Guest = 0,

            /// <summary>
            /// 사용자
            /// </summary>
            User = 1,

            /// <summary>
            /// 관리자
            /// </summary>
            Admin = 100,
        }

        /// <summary>
        /// 권한 체크
        /// </summary>
        public class CustomAuthorize : AuthorizeAttribute
        {
            /// <summary>
            /// 권한 체크안의 사용자 구분
            /// </summary>
            public new UserType Roles;

            /// <summary>
            /// 권한 체크 Core
            /// </summary>
            /// <param name="httpContext"></param>
            /// <returns></returns>
            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                using (Util util = new Util())
                {
                    if (httpContext == null)
                    {
                        throw new ArgumentNullException("httpContext");
                    }

                    if (null == httpContext.Session["Levels"] || "" == httpContext.Session["Levels"].ToString())
                    {
                        return false;
                    }

                    int Levels = util.Convert.ObjToInt(httpContext.Session["Levels"]);
                    int roles = (int)Roles;

                    if (Roles != 0 && Levels < roles)
                    {
                        return false;
                    }

                    //httpContext.Session.Timeout = 60;
                }

                return true;
            }

            /// <summary>
            /// 권한이 없을 경우
            /// </summary>
            /// <param name="filterContext"></param>
            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                string move_url = ((System.Web.HttpRequestWrapper)((System.Web.HttpContextWrapper)filterContext.RequestContext.HttpContext).Request).Url.ToString();
                move_url = GlobalObject.encodeURIComponent(move_url);

                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    if (((Interlock.Controllers.BaseController)filterContext.Controller).Ajax == false)
                    {
                        //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index", loginCheck = "check", move_url = move_url }));
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index", loginCheck = "", move_url = move_url }));

                    }
                    else
                    {
                        filterContext.HttpContext.Response.Write("|||[NOLOGIN]|||");
                        filterContext.Result = new EmptyResult();
                    }
                }
                else
                {
                    if (((Interlock.Controllers.BaseController)filterContext.Controller).Ajax == false)
                    {
                        filterContext.RequestContext.HttpContext.Session["Levels"] = 1;
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Base", action = "Main", move_url = move_url }));
                    }
                    else
                    {
                        filterContext.RequestContext.HttpContext.Session["Levels"] = 1;
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Base", action = "Main" }));
                    }
                }
            }
        }
        #endregion

        #region -----------------Base Page-----------------------------
        /// <summary>
        /// Main 페이지
        /// </summary>
        /// <returns></returns>
        [Route("PageUrlCheck")]
        [PageFilter, CustomAuthorize(Roles = UserType.User)]
        public void PageUrlCheck()
        {
            try
            {
                string MOVE_URL = GetRequest("MOVE_URL");
                if (MOVE_URL == "/Main")
                {
                    Print("true");
                }
                else
                {
                    DataSet MenuDs = ViewBag.MenuDs;
                    DataTable page_dt = MenuDs.Tables[3];
                    DataRow[] page_idx_dr = page_dt.Select("MOVE_URL = '" + MOVE_URL + "'");

                    if (page_idx_dr.Length > 0)
                    {
                        Print("true");
                    }
                    else
                    {
                        Print("false");
                    }
                }
            }
            catch
            {
                Print("false");
            }
        }

        /// <summary>
        /// Main 페이지
        /// </summary>
        /// <returns></returns>
        [Route("Main")]
        [PageFilter, CustomAuthorize(Roles = UserType.User)]
        public ActionResult Main()
        {
            try
            {
                string userEMail = "";
                string userName = "";

                if (Session["Test"] != null)
                {
                    userEMail = Session["Email"].ToString();
                    userName = Session["Name"].ToString();
                }
                else
                {

                    if (Request.IsLocal)
                    {
                        //userEMail = "hchoi@deloitte.com";
                        //userName = "hchoi";

						//userEMail = "jaehoson@deloitte.com";//BU 안나옴
						//userName = "jaehoson";
						//userEMail = "kkil@deloitte.com";
						//userName = "kkil";

					}
					else
                    {

                        if (!Request.IsAuthenticated)
                        {
                            return Redirect("/Login");
                        }

                        var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;
                        userEMail = userClaims?.FindFirst("preferred_username")?.Value;
                        userName = userClaims?.FindFirst("name")?.Value;
                    }
                }

                Session["Email"] = userEMail;
                Session["Name"] = userName;

                if (IsAdmin(userEMail))
                {
                    Session["levels"] = 100;
                }
                else
                {
                    Session["levels"] = 1;
                }

                // 사용자 정보 세팅
                Hashtable PersonInfo = GetPersonInfo(userEMail);

                SessiotnSetting();

                if (PersonInfo == null)
                {
                    return Redirect("/Login?loginCheck=check");
                }
                else
                {
                    if (Session["Test"] != null)
                    {
                        Session["Name"] = (PersonInfo["DISPLAY_ENG_NAME"].ToString() == PersonInfo["DISPLAY_NAME"].ToString() ? PersonInfo["DISPLAY_NAME"].ToString() : PersonInfo["DISPLAY_ENG_NAME"].ToString());
                        UserData.NAME = PersonInfo["DISPLAY_NAME"].ToString();
                    }

                    Session["PERSON_CODE"] = PersonInfo["PERSON_CODE"].ToString();
                    Session["PERSON_CODE_SWT"] = PersonInfo["PERSON_CODE_SWT"].ToString();                    
                    Session["DISPLAY_NAME"] = PersonInfo["DISPLAY_NAME"].ToString();
                    Session["DISPLAY_ENG_NAME"] = PersonInfo["DISPLAY_ENG_NAME"].ToString();
                    Session["UNIT_CODE"] = PersonInfo["UNIT_CODE"].ToString();
                    Session["LEVEL_CODE"] = PersonInfo["LEVEL_CODE"].ToString();
                    Session["SABUN"] = PersonInfo["SABUN"].ToString();
                    Session["USAGE_STATE"] = PersonInfo["USAGE_STATE"].ToString();
                    Session["UNIT_NAME"] = PersonInfo["UNIT_NAME"].ToString();
                    Session["LEVEL_NAME"] = PersonInfo["LEVEL_NAME"].ToString();
                    Session["BU_NM"] = PersonInfo["BU_NM"].ToString();
                    Session["BU_CD"] = PersonInfo["BU_CD"].ToString();

                    string csrf_token = util.UniqStringRNG("ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", 30);
                    Session["csrf_token_user"] = csrf_token;
                }

                string move_url = GlobalObject.decodeURIComponent(GetRequest("move_url"));

                if (move_url != "")
                {
                    return Redirect(move_url);
                }
                //사용자 화면 첫번째 등록된 페이지로 redirect
                //else
                //{
                //    DataSet MenuDs = ViewBag.MenuDs;
                //    DataTable page_dt = MenuDs.Tables[3];

                //    return Redirect(page_dt.Rows[0]["MOVE_URL"].ToString());
                //}

                using (DataSet ds = util.GetList("SP_MAIN_PAGE_SEL", new {
                    @PERSON_CODE = UserData.PERSON_CODE,
                    @UNIT_CODE = UserData.UNIT_CODE
                }, 30, false)) {
                    ViewBag.dsTop5 = ds;
                }

                if (Ajax && m_isSessionExpired == true)
                {
                    ErrorMsg("인증이 만료되었습니다.");
                }

            }
            catch (Exception ex)
            {
                ErrorMsg(ex);
            }

            return View("~/Views/Main.cshtml");
        }

        /// <summary>
        /// 사용자 정보 세팅
        /// </summary>
        /// <returns></returns>
        protected Hashtable GetPersonInfo(string userEMail = null)
        {
            System.Security.Claims.ClaimsIdentity userClaims = null;

            if (string.IsNullOrEmpty(userEMail))
            {
                userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;
                userEMail = userClaims?.FindFirst("preferred_username")?.Value;
            }

           
            Hashtable PersonInfo = util.GetData("SP_PERSON_INFO_SEL", new
            {
                @EMAIL = userEMail
            }, true, 30, false);

            return PersonInfo;
        }

        /// <summary>
        /// Main 페이지
        /// </summary>
        /// <returns></returns>
        [Route("OrgShow")]
        [PageFilter, CustomAuthorize(Roles = UserType.User)]
        public ActionResult OrgShow()
        {
            try
            {
                string kind = GetRequest("kind");
                string multi = GetRequest("multi");
                ViewBag.kind = kind;
                ViewBag.multi = multi;

                using (DataSet ds = util.GetList("SP_EMPNO_USER_SEL", new { @Search = "" }, 30, false))
                {
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        ViewBag.person_dt = ds.Tables[0];
                }

                using (DataSet ds = util.GetList("SP_EMPNO_DEPT_SEL", new { @Search = "" }, 30, false))
                {
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        ViewBag.dept_dt = ds.Tables[0];
                }

                using (DataSet ds = util.GetList("SP_EMPNO_ADDJOB_SEL", 30, false))
                {
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        ViewBag.addJob_dt = ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                ErrorMsg(ex);
            }

            return View("~/Views/OrgShow.cshtml");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("PersonShow")]
        [PageFilter, CustomAuthorize(Roles = UserType.User)]
        public ActionResult PersonShow()
        {
            try
            {
                using (DataSet ds = util.GetList("SP_EMPNO_USER_SEL", new { @Search = "" }, 30, false))
                {
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        ViewBag.person_dt = ds.Tables[0];
                }
                using (DataSet ds = util.GetList("SP_EMPNO_ADDJOB_SEL", 30, false))
                {
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        ViewBag.addJob_dt = ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                ErrorMsg(ex);
            }

            return View("~/Views/PersonShow.cshtml");
        }

        private bool IsAdmin(string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
                return false;

            try
            {
                string Sql = "SELECT * FROM TB_ADMIN_USER WHERE EMAIL = @EMAIL";

                AdminModel adminModel = (AdminModel)util.GetData(Sql, new
                {
                    @EMAIL = userEmail
                }, true, 30, false).ToModel<AdminModel>();

                if (userEmail.Equals(adminModel?.EMAIL, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            catch (Exception ex)
            {
                ErrorMsg(ex);
            }

            return false;
        }

        /// <summary>
        /// 다국어 언어 리소스 내용 js 파일로 데이타 내리기
        /// </summary>
        /// <returns></returns>
        public JavaScriptResult LanguageJs()
        {
            StringBuilder sb = new StringBuilder();

            /*
            ResourceSet resourceSet = Resources.resLanguage.ResourceManager.GetResourceSet(m_Language, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                string resourceKey = entry.Key.ToString();
                string resource = entry.Value.ToString();

                sb.Append("Common.Lang." + resourceKey + " = \"" + resource.Replace("\"", "") + "\";");
            }
            */

            return new JavaScriptResult()
            {
                Script = sb.ToString()
            };
        }
        #endregion

        /// <summary>
        /// DropDownList 사용여부 YN 
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetYNList()
        {
            List<SelectListItem> UseYN_List = new List<SelectListItem>();

            UseYN_List.Add(new SelectListItem() { Text = "Y", Value = "Y", Selected = true });
            UseYN_List.Add(new SelectListItem() { Text = "N", Value = "N" });

            return UseYN_List;
        }
    }
}

#region -----------------PageFilter---------------------------
/// <summary>
/// 페이지 공통
/// </summary>
public class PageFilterAttribute : ActionFilterAttribute
{
    /// <summary>
    /// ActionResult (Controller) 종류 후
    /// </summary>
    /// <param name="filterContext"></param>
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        ((Interlock.Controllers.BaseController)filterContext.Controller).PageEnd(filterContext);
    }

    /// <summary>
    /// View 종류 후
    /// </summary>
    /// <param name="filterContext"></param>
    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
        ((Interlock.Controllers.BaseController)filterContext.Controller).Close();
    }

    /// <summary>
    /// ActionResult (Controller) 시작전
    /// </summary>
    /// <param name="filterContext"></param>
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        ((Interlock.Controllers.BaseController)filterContext.Controller).SessiotnSetting();
        ((Interlock.Controllers.BaseController)filterContext.Controller).PageStart(filterContext);

    }
}

/// <summary>
/// Ajax 체크
/// </summary>
public class AjaxFilterAttribute : ActionFilterAttribute
{
    /// <summary>
    /// ActionResult (Controller) 시작전
    /// </summary>
    /// <param name="filterContext"></param>
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (((Interlock.Controllers.BaseController)filterContext.Controller).Ajax == false)
        {
            filterContext.HttpContext.Response.Write("");
            filterContext.Result = new EmptyResult();
            base.OnActionExecuting(filterContext);
            //filterContext.HttpContext.Response.End();
        }
    }
}

/// <summary>
/// CSRF 체크
/// </summary>
public class CSRFFilterAttribute : ActionFilterAttribute
{
    /// <summary>
    /// ActionResult (Controller) 시작전
    /// </summary>
    /// <param name="filterContext"></param>
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.HttpContext.Request["csrf_token"] != null
            && filterContext.HttpContext.Request["csrf_token"].ToString() != ""
            && filterContext.HttpContext.Session["csrf_token_user"] != null
            && filterContext.HttpContext.Session["csrf_token_user"].ToString() != ""
            && filterContext.HttpContext.Request["csrf_token"].ToString() == filterContext.HttpContext.Session["csrf_token_user"].ToString()
        )
        {
        }
        else
        {
            filterContext.HttpContext.Response.Write("|||[CSRFERROR]|||인증이 만료되었습니다.");
            filterContext.Result = new EmptyResult();
            base.OnActionExecuting(filterContext);
        }
    }
}
#endregion