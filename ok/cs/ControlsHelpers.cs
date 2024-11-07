using Newtonsoft.Json;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.HtmlControls;
using Interlock.Controllers;
using WebUtil;

namespace Interlock.Helpers
{
    #region -----------------Site Helper----------------------
    ///////////////////////////////////////////////////////////////
    // WebUtil -> Helpers 와 Name이 중복되지 않도록 주의
    ///////////////////////////////////////////////////////////////

    /// <summary>
    /// Site Helper
    /// </summary>
    public static class SiteHelper
    {
        /// <summary>
        /// 회원 검색
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id">아이디</param>
        /// <param name="htmlAttributes">속성</param>
        /// <returns></returns>
        public static IHtmlString User(this HtmlHelper helper, string id, object htmlAttributes = null)
        {
            return User(helper, id, "", "", htmlAttributes);
        }

        /// <summary>
        /// 회원 검색
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id">아이디</param>
        /// <param name="value">Text</param>
        /// <param name="code">Code</param>
        /// <param name="htmlAttributes">속성</param>
        /// <returns></returns>
        public static IHtmlString User(this HtmlHelper helper, string id, string value, string code, object htmlAttributes = null)
        {
            HtmlGenericControl input = null;
            string RtnHtml = "";

            try
            {
                input = new HtmlGenericControl("input");
                input.Attributes.Add("type", "text");
                input.Attributes.Add("id", id);
                input.Attributes.Add("name", id);

                if (value != "")
                {
                    input.Attributes.Add("value", value);
                    input.Attributes.Add("OldValue", value);
                }

                if (code != "")
                {
                    input.Attributes.Add("Code", code);
                    input.Attributes.Add("OldCode", code);
                }

                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        input.Attributes.Add(attributes.Key, attributes.Value.ToString());
                    }
                }

                input.Attributes.Add("url", "/Custom/UserSearch");
                input.Attributes.Add("onblur", "Custom.GetCodeSeach($(this));");
                input.Attributes.Add("onkeydown", "Custom.GetCodeKeydown(event, $(this));");
                input.Attributes.Add("ondblclick", "Custom.GetCodeSeachShow($(this));");

                using (Util util = new Util())
                {
                    RtnHtml = util.GetHtml(input);
                }
            }
            catch
            {
            }
            finally
            {
                if (input != null)
                    input.Dispose(); input = null;
            }

            return MvcHtmlString.Create(RtnHtml);
        }

        /// <summary>
        /// 사원 검색 (커스텀)  
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id">아이디</param>
        /// <param name="value">Text</param>
        /// <param name="code">Code</param>
        /// <param name="htmlAttributes">속성</param>
        /// <returns></returns>
        public static IHtmlString UserCode(this HtmlHelper helper, string id, string value, string code, object htmlAttributes = null)
        {
            HtmlGenericControl input = null;
            string RtnHtml = "";

            try
            {
                input = new HtmlGenericControl("input");
                input.Attributes.Add("type", "text");
                input.Attributes.Add("id", id);
                input.Attributes.Add("name", id);

                if (value != "")
                {
                    input.Attributes.Add("value", value);
                    input.Attributes.Add("OldValue", value);
                }

                if (code != "")
                {
                    input.Attributes.Add("Code", code);
                    input.Attributes.Add("OldCode", code);
                }

                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        input.Attributes.Add(attributes.Key, attributes.Value.ToString());
                    }
                }

                input.Attributes.Add("url", "/Custom/UserCodeSearch");
                //input.Attributes.Add("onblur", "Custom.GetCodeSeach($(this));");
                //input.Attributes.Add("onkeydown", "Custom.GetCodeKeydown(event, $(this));");
                input.Attributes.Add("ondblclick", "Custom.GetCodeSeachShow($(this));");

                using (Util util = new Util())
                {
                    RtnHtml = util.GetHtml(input);
                }
            }
            catch
            {
            }
            finally
            {
                if (input != null)
                    input.Dispose(); input = null;
            }

            return MvcHtmlString.Create(RtnHtml);
        }

        /// <summary>
        /// 사원 검색 (SWIFT)  
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id">아이디</param>
        /// <param name="value">Text</param>
        /// <param name="code">Code</param>
        /// <param name="htmlAttributes">속성</param>
        /// <returns></returns>
        public static IHtmlString UserCodeSWIFT(this HtmlHelper helper, string id, string value, string code, object htmlAttributes = null)
        {
            HtmlGenericControl input = null;
            string RtnHtml = "";

            try
            {
                input = new HtmlGenericControl("input");
                input.Attributes.Add("type", "text");
                input.Attributes.Add("id", id);
                input.Attributes.Add("name", id);

                if (value != "")
                {
                    input.Attributes.Add("value", value);
                    input.Attributes.Add("OldValue", value);
                }

                if (code != "")
                {
                    input.Attributes.Add("Code", code);
                    input.Attributes.Add("OldCode", code);
                }

                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        input.Attributes.Add(attributes.Key, attributes.Value.ToString());
                    }
                }

                input.Attributes.Add("url", "/Custom/UserCodeSWIFTSearch");
                input.Attributes.Add("onblur", "Custom.GetCodeSeach2($(this));");
                input.Attributes.Add("onkeydown", "Custom.GetCodeKeydown2(event, $(this));");
                input.Attributes.Add("ondblclick", "Custom.GetCodeSeachShow2($(this));");

                using (Util util = new Util())
                {
                    RtnHtml = util.GetHtml(input);
                }
            }
            catch
            {
            }
            finally
            {
                if (input != null)
                    input.Dispose(); input = null;
            }

            return MvcHtmlString.Create(RtnHtml);
        }

        /// <summary>
        /// 나라 검색(INTERFIRM)  
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id">아이디</param>
        /// <param name="value">Text</param>
        /// <param name="code">Code</param>
        /// <param name="htmlAttributes">속성</param>
        /// <returns></returns>
        public static IHtmlString NationCode(this HtmlHelper helper, string id, string value, string code, object htmlAttributes = null)
        {
            HtmlGenericControl input = null;
            string RtnHtml = "";

            try
            {
                input = new HtmlGenericControl("input");
                input.Attributes.Add("type", "text");
                input.Attributes.Add("id", id);
                input.Attributes.Add("name", id);

                if (value != "")
                {
                    input.Attributes.Add("value", value);
                    input.Attributes.Add("OldValue", value);
                }

                if (code != "")
                {
                    input.Attributes.Add("Code", code);
                    input.Attributes.Add("OldCode", code);
                }

                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        input.Attributes.Add(attributes.Key, attributes.Value.ToString());
                    }
                }

                input.Attributes.Add("url", "/Sap_Interfirm/NationSearch");
                input.Attributes.Add("onblur", "Custom.GetCodeSeach2($(this));");
                input.Attributes.Add("onkeydown", "Custom.GetCodeKeydown2(event, $(this));");
                input.Attributes.Add("ondblclick", "Custom.GetCodeSeachShow2($(this));");

                using (Util util = new Util())
                {
                    RtnHtml = util.GetHtml(input);
                }
            }
            catch
            {
            }
            finally
            {
                if (input != null)
                    input.Dispose(); input = null;
            }

            return MvcHtmlString.Create(RtnHtml);
        }

        /// <summary>
        /// 부서 검색
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id">아이디</param>
        /// <param name="htmlAttributes">속성</param>
        /// <returns></returns>
        public static IHtmlString Dept(this HtmlHelper helper, string id, object htmlAttributes = null)
        {
            return Dept(helper, id, "", "", htmlAttributes);
        }

        /// <summary>
        /// 회원 검색
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id">아이디</param>
        /// <param name="value">Text</param>
        /// <param name="code">Code</param>
        /// <param name="htmlAttributes">속성</param>
        /// <returns></returns>
        public static IHtmlString Dept(this HtmlHelper helper, string id, string value, string code, object htmlAttributes = null)
        {
            HtmlGenericControl input = null;
            string RtnHtml = "";

            try
            {
                input = new HtmlGenericControl("input");
                input.Attributes.Add("type", "text");
                input.Attributes.Add("id", id);
                input.Attributes.Add("name", id);

                if (value != "")
                {
                    input.Attributes.Add("value", value);
                    input.Attributes.Add("OldValue", value);
                }

                if (code != "")
                {
                    input.Attributes.Add("Code", code);
                    input.Attributes.Add("OldCode", code);
                }

                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        input.Attributes.Add(attributes.Key, attributes.Value.ToString());
                    }
                }

                input.Attributes.Add("url", "/Custom/DeptSearch");
                input.Attributes.Add("onblur", "Custom.GetCodeSeach($(this));");
                input.Attributes.Add("onkeydown", "Custom.GetCodeKeydown(event, $(this));");
                input.Attributes.Add("ondblclick", "Custom.GetCodeSeachShow($(this));");

                using (Util util = new Util())
                {
                    RtnHtml = util.GetHtml(input);
                }
            }
            catch
            {
            }
            finally
            {
                if (input != null)
                    input.Dispose(); input = null;
            }

            return MvcHtmlString.Create(RtnHtml);
        }

        /// <summary>
        /// 부서 검색
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id">아이디</param>
        /// <param name="htmlAttributes">속성</param>
        /// <returns></returns>
        public static IHtmlString Group(this HtmlHelper helper, string id, object htmlAttributes = null)
        {
            return Group(helper, id, "", "", htmlAttributes);
        }

        /// <summary>
        /// 회원 검색
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id">아이디</param>
        /// <param name="value">Text</param>
        /// <param name="code">Code</param>
        /// <param name="htmlAttributes">속성</param>
        /// <returns></returns>
        public static IHtmlString Group(this HtmlHelper helper, string id, string value, string code, object htmlAttributes = null)
        {
            HtmlGenericControl input = null;
            string RtnHtml = "";

            try
            {
                input = new HtmlGenericControl("input");
                input.Attributes.Add("type", "text");
                input.Attributes.Add("id", id);
                input.Attributes.Add("name", id);

                if (value != "")
                {
                    input.Attributes.Add("value", value);
                    input.Attributes.Add("OldValue", value);
                }

                if (code != "")
                {
                    input.Attributes.Add("Code", code);
                    input.Attributes.Add("OldCode", code);
                }

                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        input.Attributes.Add(attributes.Key, attributes.Value.ToString());
                    }
                }

                input.Attributes.Add("url", "/Custom/GroupSearch");
                input.Attributes.Add("onblur", "Custom.GetCodeSeach($(this));");
                input.Attributes.Add("onkeydown", "Custom.GetCodeKeydown(event, $(this));");
                input.Attributes.Add("ondblclick", "Custom.GetCodeSeachShow($(this));");

                using (Util util = new Util())
                {
                    RtnHtml = util.GetHtml(input);
                }
            }
            catch
            {
            }
            finally
            {
                if (input != null)
                    input.Dispose(); input = null;
            }

            return MvcHtmlString.Create(RtnHtml);
        }

        /// <summary>
        /// 파일 업로드
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlString FileUpload(this HtmlHelper helper, string id, object htmlAttributes = null)
        {
            HtmlGenericControl div = new HtmlGenericControl("div");

            // 파일 input
            HtmlGenericControl ar_fl_div = null;
            HtmlGenericControl file_btn = null;
            HtmlGenericControl file_hidden = null;
            HtmlGenericControl del_btn = null;

            // 파일 설명
            HtmlGenericControl ar_fr_div = null;
            HtmlGenericControl form_txt_srmy = null;
            HtmlGenericControl form_txt_srmy2 = null;
            HtmlGenericControl show_hide_btn = null;

            // 파일 상세
            HtmlGenericControl file_list_wrap_div = null;
            HtmlGenericControl file_no_list = null;
            HtmlGenericControl file_list = null;

            string UploadType = "";
            string RtnHtml = "";

            try
            {
                using (Util util = new Util())
                {
                    // 속성 값 세팅
                    if (htmlAttributes != null)
                    {
                        var attributesData = new RouteValueDictionary(htmlAttributes);

                        foreach (var attributes in attributesData)
                        {
                            if (attributes.Key == "UploadType")
                            {
                                UploadType = attributes.Value.ToString();
                            }
                        }
                    }

                    if (UploadType == "")
                    {
                        div.Attributes.Add("class", "fileinput fileinput-new input-group");
                        div.Attributes.Add("data-provides", "fileinput");

                        div.InnerHtml = @"
                                            <div class='form-control' data-trigger='fileinput'>
                                                <i class='glyphicon glyphicon-file fileinput-exists'></i>
                                                <span class='fileinput-filename'></span>
                                            </div>
                                            <span class='input-group-addon btn btn-default btn-file'>
                                                <span class='fileinput-new'>파일선택</span>
                                                <span class='fileinput-exists'>변경</span>
                                                <input type='file' name='" + id + @"' />
                                            </span>
                                            <a href='#' class='input-group-addon btn btn-default fileinput-exists' data-dismiss='fileinput'>삭제</a>
                        ";
                    }
                    else if (UploadType == "multi")
                    {
                        div.Attributes.Add("class", "form_file_wrap");
                        div.Attributes.Add("oncontextmenu", "return false");
                        div.Attributes.Add("fileName", id);

                        // 속성 값 세팅
                        if (htmlAttributes != null)
                        {
                            var attributesData = new RouteValueDictionary(htmlAttributes);

                            foreach (var attributes in attributesData)
                            {
                                div.Attributes.Add(attributes.Key, attributes.Value.ToString());
                            }
                        }

                        //
                        // 파일 input
                        //
                        ar_fl_div = new HtmlGenericControl("div");
                        ar_fl_div.Attributes.Add("class", "ar_fl");

                        // 파일 첨부 버튼
                        file_btn = new HtmlGenericControl("button");
                        file_btn.Attributes.Add("type", "button");
                        file_btn.InnerHtml = "파일 첨부하기";
                        file_btn.Attributes.Add("class", "btn btn-xs btn-default");
                        file_btn.Attributes.Add("onclick", "$('#" + id + "_file_input').trigger('click');");

                        // 모두삭제
                        del_btn = new HtmlGenericControl("button");
                        del_btn.Attributes.Add("type", "button");
                        del_btn.InnerHtml = "모두 삭제";
                        del_btn.Attributes.Add("class", "btn btn-xs btn-danger");
                        del_btn.Attributes.Add("fileDelMode", id);
                        del_btn.Style.Add("margin-left", "4px;");
                        del_btn.Style.Add("display", "none");

                        ar_fl_div.Controls.Add(file_btn);
                        ar_fl_div.Controls.Add(del_btn);

                        //
                        // 파일 설명
                        //
                        ar_fr_div = new HtmlGenericControl("div");
                        ar_fr_div.Attributes.Add("class", "ar_fr");

                        form_txt_srmy = new HtmlGenericControl("span");
                        form_txt_srmy.Attributes.Add("class", "form_txt_srmy");
                        form_txt_srmy.InnerHtml = "파일 크기 : ";
                        form_txt_srmy2 = new HtmlGenericControl("span");
                        form_txt_srmy2.Attributes.Add("id", id + "_size_srmy");
                        form_txt_srmy2.Attributes.Add("class", "form_txt_srmy st3");
                        form_txt_srmy2.Attributes.Add("filesize", "0");
                        form_txt_srmy2.InnerHtml = "0KB";

                        show_hide_btn = new HtmlGenericControl("button");
                        show_hide_btn.Attributes.Add("class", "btn btn-xs btn-default m-l-sm");
                        show_hide_btn.Attributes.Add("type", "button");
                        show_hide_btn.Attributes.Add("iType", "showhide");
                        show_hide_btn.InnerHtml = "<i class=\"fa fa-chevron-down\"></i> 펼치기";

                        ar_fr_div.Controls.Add(form_txt_srmy);
                        ar_fr_div.Controls.Add(form_txt_srmy2);
                        ar_fr_div.Controls.Add(show_hide_btn);

                        //
                        // 파일 상세
                        //
                        file_list_wrap_div = new HtmlGenericControl("div");
                        file_list_wrap_div.Attributes.Add("class", "file_list_wrap");
                        file_list_wrap_div.Style.Add("display", "none");

                        // 파일 input
                        file_hidden = new HtmlGenericControl("input");
                        file_hidden.Attributes.Add("type", "file");
                        file_hidden.Attributes.Add("class", "file_hidden");
                        file_hidden.Attributes.Add("multiple", "true");
                        file_hidden.Attributes.Add("id", id + "_file_input");
                        file_hidden.Attributes.Add("name", id + "_file_input");
                        file_hidden.Attributes.Add("dragFiles", "");
                        file_hidden.Attributes.Add("uploadFileName", id);

                        // 드래그 하세요 msg
                        file_no_list = new HtmlGenericControl("p");
                        file_no_list.Attributes.Add("class", "file_cmt");
                        file_no_list.Attributes.Add("style", "line-height:58px;margin:0px;");
                        file_no_list.InnerHtml = "여러개의 파일을 마우스로 끌어놓으세요.";

                        file_list = new HtmlGenericControl("ul");
                        file_list.Attributes.Add("class", "file_list");
                        file_list.Attributes.Add("id", id + "_file_list_area");
                        file_list.Style.Add("display", "none");

                        file_list_wrap_div.Controls.Add(file_hidden);
                        file_list_wrap_div.Controls.Add(file_no_list);
                        file_list_wrap_div.Controls.Add(file_list);

                        //
                        // 합치기
                        //
                        div.Controls.Add(ar_fl_div);
                        div.Controls.Add(ar_fr_div);
                        div.Controls.Add(file_list_wrap_div);
                    }

                    RtnHtml = util.GetHtml(div);
                }
            }
            catch
            {
            }
            finally
            {
                if (div != null)
                    div.Dispose(); div = null;
                if (ar_fl_div != null)
                    ar_fl_div.Dispose(); ar_fl_div = null;
                if (file_btn != null)
                    file_btn.Dispose(); file_btn = null;
                if (file_hidden != null)
                    file_hidden.Dispose(); file_hidden = null;
                if (del_btn != null)
                    del_btn.Dispose(); del_btn = null;
                if (ar_fr_div != null)
                    ar_fr_div.Dispose(); ar_fr_div = null;
                if (form_txt_srmy != null)
                    form_txt_srmy.Dispose(); form_txt_srmy = null;
                if (form_txt_srmy2 != null)
                    form_txt_srmy2.Dispose(); form_txt_srmy2 = null;
                if (file_list_wrap_div != null)
                    file_list_wrap_div.Dispose(); file_list_wrap_div = null;
                if (file_no_list != null)
                    file_no_list.Dispose(); file_no_list = null;
                if (file_list != null)
                    file_list.Dispose(); file_list = null;
            }

            return MvcHtmlString.Create(RtnHtml);
        }

        /// <summary>
        /// 파일 업로드
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="boardType"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlString FileUpload(this HtmlHelper helper, string id, string boardType, object htmlAttributes = null)
        {
            // 전체 div
            HtmlGenericControl form_div = null;
            HtmlGenericControl file_value = null;

            // 파일 input
            HtmlGenericControl ar_fl_div = null;
            HtmlGenericControl file_btn = null;
            HtmlGenericControl file_hidden = null;
            HtmlGenericControl del_btn = null;

            // 파일 설명
            HtmlGenericControl ar_fr_div = null;
            HtmlGenericControl form_txt_srmy = null;
            HtmlGenericControl form_txt_srmy2 = null;
            HtmlGenericControl show_hide_btn = null;

            // 파일 상세
            HtmlGenericControl file_list_wrap_div = null;
            HtmlGenericControl file_no_list = null;
            HtmlGenericControl file_list = null;

            string RtnHtml = "";

            try
            {
                using (Util util = new Util())
                {

                    form_div = new HtmlGenericControl("div");
                    form_div.Attributes.Add("class", "form_file_wrap col-lg-12");
                    form_div.Attributes.Add("oncontextmenu", "return false");
                    form_div.Attributes.Add("fileName", id);
                    form_div.Attributes.Add("boardType", boardType);

                    // 속성 값 세팅
                    if (htmlAttributes != null)
                    {
                        var attributesData = new RouteValueDictionary(htmlAttributes);

                        foreach (var attributes in attributesData)
                        {
                            form_div.Attributes.Add(attributes.Key, attributes.Value.ToString());
                        }
                    }

                    // 값 세팅
                    file_value = new HtmlGenericControl("input");
                    file_value.Attributes.Add("type", "hidden");
                    file_value.Attributes.Add("id", id);
                    file_value.Attributes.Add("name", id);
                    file_value.Attributes.Add("value", "");

                    //
                    // 파일 input
                    //
                    ar_fl_div = new HtmlGenericControl("div");
                    ar_fl_div.Attributes.Add("class", "ar_fl");

                    // 파일 첨부 버튼
                    file_btn = new HtmlGenericControl("button");
                    file_btn.Attributes.Add("type", "button");
                    file_btn.InnerHtml = "파일 첨부하기";
                    file_btn.Attributes.Add("class", "btn btn-xs btn-default");
                    file_btn.Attributes.Add("onclick", "$('#" + id + "_file_input').trigger('click');");

                    // 모두삭제
                    del_btn = new HtmlGenericControl("button");
                    del_btn.Attributes.Add("type", "button");
                    del_btn.InnerHtml = "모두 삭제";
                    del_btn.Attributes.Add("class", "btn btn-xs btn-danger");
                    del_btn.Attributes.Add("fileDelMode", id);
                    del_btn.Style.Add("margin-left", "4px;");
                    del_btn.Style.Add("display", "none");

                    ar_fl_div.Controls.Add(file_btn);
                    ar_fl_div.Controls.Add(del_btn);

                    //
                    // 파일 설명
                    //
                    ar_fr_div = new HtmlGenericControl("div");
                    ar_fr_div.Attributes.Add("class", "ar_fr");

                    form_txt_srmy = new HtmlGenericControl("span");
                    form_txt_srmy.Attributes.Add("class", "form_txt_srmy");
                    form_txt_srmy.InnerHtml = "파일 크기 : ";
                    form_txt_srmy2 = new HtmlGenericControl("span");
                    form_txt_srmy2.Attributes.Add("id", id + "_size_srmy");
                    form_txt_srmy2.Attributes.Add("class", "form_txt_srmy st3");
                    form_txt_srmy2.Attributes.Add("filesize", "0");
                    form_txt_srmy2.InnerHtml = "0KB";

                    show_hide_btn = new HtmlGenericControl("button");
                    show_hide_btn.Attributes.Add("class", "btn btn-xs btn-default m-l-sm");
                    show_hide_btn.Attributes.Add("type", "button");
                    show_hide_btn.Attributes.Add("iType", "showhide");
                    show_hide_btn.InnerHtml = "<i class=\"fa fa-chevron-down\"></i> 펼치기";

                    ar_fr_div.Controls.Add(form_txt_srmy);
                    ar_fr_div.Controls.Add(form_txt_srmy2);
                    ar_fr_div.Controls.Add(show_hide_btn);

                    //
                    // 파일 상세
                    //
                    file_list_wrap_div = new HtmlGenericControl("div");
                    file_list_wrap_div.Attributes.Add("class", "file_list_wrap");
                    file_list_wrap_div.Style.Add("display", "none");

                    // 파일 input
                    file_hidden = new HtmlGenericControl("input");
                    file_hidden.Attributes.Add("type", "file");
                    file_hidden.Attributes.Add("class", "file_hidden");
                    file_hidden.Attributes.Add("multiple", "true");
                    file_hidden.Attributes.Add("id", id + "_file_input");
                    file_hidden.Attributes.Add("name", id + "_file_input");

                    // 드래그 하세요 msg
                    file_no_list = new HtmlGenericControl("p");
                    file_no_list.Attributes.Add("class", "file_cmt");
                    file_no_list.Attributes.Add("style", "line-height:58px;margin:0px;");
                    file_no_list.InnerHtml = "여러개의 파일을 마우스로 끌어놓으세요.";

                    file_list = new HtmlGenericControl("ul");
                    file_list.Attributes.Add("class", "file_list");
                    file_list.Attributes.Add("id", id + "_file_list_area");
                    file_list.Style.Add("display", "none");

                    file_list_wrap_div.Controls.Add(file_hidden);
                    file_list_wrap_div.Controls.Add(file_no_list);
                    file_list_wrap_div.Controls.Add(file_list);

                    //
                    // 합치기
                    //
                    form_div.Controls.Add(file_value);
                    form_div.Controls.Add(ar_fl_div);
                    form_div.Controls.Add(ar_fr_div);
                    form_div.Controls.Add(file_list_wrap_div);
                    RtnHtml = util.GetHtml(form_div);
                }
            }
            catch
            {
            }
            finally
            {
                if (form_div != null)
                    form_div.Dispose(); form_div = null;
                if (file_value != null)
                    file_value.Dispose(); file_value = null;
                if (ar_fl_div != null)
                    ar_fl_div.Dispose(); ar_fl_div = null;
                if (file_btn != null)
                    file_btn.Dispose(); file_btn = null;
                if (file_hidden != null)
                    file_hidden.Dispose(); file_hidden = null;
                if (del_btn != null)
                    del_btn.Dispose(); del_btn = null;
                if (ar_fr_div != null)
                    ar_fr_div.Dispose(); ar_fr_div = null;
                if (form_txt_srmy != null)
                    form_txt_srmy.Dispose(); form_txt_srmy = null;
                if (form_txt_srmy2 != null)
                    form_txt_srmy2.Dispose(); form_txt_srmy2 = null;
                if (file_list_wrap_div != null)
                    file_list_wrap_div.Dispose(); file_list_wrap_div = null;
                if (file_no_list != null)
                    file_no_list.Dispose(); file_no_list = null;
                if (file_list != null)
                    file_list.Dispose(); file_list = null;
            }

            return MvcHtmlString.Create(RtnHtml);
        }

        /// <summary>
        /// 사원 검색 (SWIFT)  
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id">아이디</param>
        /// <param name="value">Text</param>
        /// <param name="code">Code</param>
        /// <param name="htmlAttributes">속성</param>
        /// <returns></returns>
        public static IHtmlString MngBuMailingBtn(this HtmlHelper helper, string id, string value, object htmlAttributes = null)
        {
            HtmlGenericControl button = null;
            string RtnHtml = "";

            try
            {
                button = new HtmlGenericControl("button");
                button.Attributes.Add("type", "button");
                button.Attributes.Add("id", id);
                button.Attributes.Add("name", id);

                button.InnerHtml = value;

                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        button.Attributes.Add(attributes.Key, attributes.Value.ToString());
                    }
                }

                button.Attributes.Add("url", "/Custom/MngBuRecipsByRpt");
                button.Attributes.Add("onclick", "Custom.MngBuMailing($(this))");

                using (Util util = new Util())
                {
                    RtnHtml = util.GetHtml(button);
                }
            }
            catch
            {
            }
            finally
            {
                if (button != null)
                    button.Dispose(); button = null;
            }

            return MvcHtmlString.Create(RtnHtml);
        }
    }
    #endregion
}