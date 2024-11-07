using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace WebUtil
{
    #region -----------------GetCheckIf---------------------------
    /// <summary>
    /// CheckLabel
    /// </summary>
    public static class GetCheckIfHelper
    {
        /// <summary>
        /// If Check Value
        /// </summary>
        /// <param name="helper">Helper</param>
        /// <param name="value">값</param>
        /// <param name="check">체크 값</param>
        /// <param name="trueValue">true 결과 값</param>
        /// <param name="falseValue">false 결과 값</param>
        /// <returns></returns>
        public static IHtmlString GetCheckIf(this HtmlHelper helper, object value, object check, string trueValue, string falseValue = "")
        {
            using (Util util = new Util())
            {
                return MvcHtmlString.Create(util.GetCheckIf(value, check, trueValue, falseValue));
            }
        }
    }
    #endregion

    #region -----------------ComboBox---------------------------
    /// <summary>
    /// ComboBox
    /// </summary>
    public static class ComboBoxHelper
    {
        /// <summary>
        /// 콤보박스
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlString ComboBox(this HtmlHelper helper, string id, object htmlAttributes = null)
        {
            object items = null;
            return ComboBox(helper, id, items, "", htmlAttributes);
        }

        /// <summary>
        /// 콤보박스
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="items"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlString ComboBox(this HtmlHelper helper, string id, object items, object htmlAttributes = null)
        {
            return ComboBox(helper, id, items, "", htmlAttributes);
        }

        /// <summary>
        /// 콤보박스
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlString ComboBox(this HtmlHelper helper, string id, string value, object htmlAttributes = null)
        {
            object items = null;
            return ComboBox(helper, id, items, value, htmlAttributes);
        }

        /// <summary>
        /// 콤보박스
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <param name="htmlAttributes">TextValueDisplay : Value + Text출력형태 (기본값 false), DisabledIgnore : Disabledt설정을 무시 (기본값 true)</param>
        /// <returns></returns>
        public static IHtmlString ComboBox(this HtmlHelper helper, string id, object items, string value, object htmlAttributes = null)
        {
            string RtnHtml = "";

            using (Util util = new Util())
            {
                bool TextValueDisplay = false;
                bool DisabledIgnore = true;
                bool Multiple = false;
                int YearTypeStartYear = 0;
                int YearTypeEndYear = 0;
                bool SetMonthType = false;
                string SetMonthTypeMask = "";
                string DataFiledName = "";
                string DataFiledCode = "";
                string NoneFieldText = "";
                HtmlGenericControl select;

                select = new HtmlGenericControl("select");
                select.Attributes.Add("id", id);
                select.Attributes.Add("name", id);
                select.Attributes.Add("default_value", value);

                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        if (attributes.Key == "TextValueDisplay")
                        {
                            TextValueDisplay = util.Convert.ObjToBool(attributes.Value.ToString());
                        }
                        else if (attributes.Key == "DisabledIgnore")
                        {
                            DisabledIgnore = util.Convert.ObjToBool(attributes.Value.ToString());
                        }
                        else if (attributes.Key == "YearTypeStartYear")
                        {
                            YearTypeStartYear = util.Convert.ObjToInt(attributes.Value.ToString());
                        }
                        else if (attributes.Key == "YearTypeEndYear")
                        {
                            YearTypeEndYear = util.Convert.ObjToInt(attributes.Value.ToString());
                        }
                        else if (attributes.Key == "SetMonthType")
                        {
                            SetMonthType = util.Convert.ObjToBool(attributes.Value.ToString());
                        }
                        else if (attributes.Key == "SetMonthTypeMask")
                        {
                            SetMonthTypeMask = attributes.Value.ToString();
                        }
                        else if (attributes.Key == "DataFiledName")
                        {
                            DataFiledName = attributes.Value.ToString();
                        }
                        else if (attributes.Key == "DataFiledCode")
                        {
                            DataFiledCode = attributes.Value.ToString();
                        }
                        else if (attributes.Key == "NoneFieldText")
                        {
                            NoneFieldText = attributes.Value.ToString();
                            select.Attributes.Add(attributes.Key, attributes.Value.ToString());
                        }
                        else if (attributes.Key.ToLower() == "multiple")
                        {
                            Multiple = true;
                            select.Attributes.Add(attributes.Key, attributes.Value.ToString());
                        }
                        else if (attributes.Key.ToLower() == "disabled")
                        {
                            if (attributes.Value.ToString() == "" || attributes.Value.ToString().ToLower() == "disabled")
                            {
                                select.Attributes.Add(attributes.Key, "disabled");
                            }
                            else
                            {
                                if (util.Convert.ObjToBool(attributes.Value.ToString()))
                                {
                                    select.Attributes.Add(attributes.Key, "disabled");
                                }
                            }
                        }
                        else
                        {
                            select.Attributes.Add(attributes.Key, attributes.Value.ToString());
                        }
                    }
                }

                if (YearTypeStartYear > 0)
                {
                    int endYear = util.Convert.ObjToInt(DateTime.Now.ToString("yyyy")) + 1;
                    if (YearTypeEndYear > 0)
                    {
                        endYear = YearTypeEndYear;
                    }

                    for (int i = YearTypeStartYear; i <= endYear; i++)
                    {
                        HtmlGenericControl option = new HtmlGenericControl("option");
                        option.InnerText = i.ToString() + "년";
                        option.Attributes.Add("value", i.ToString());

                        if (i.ToString() == value)
                            option.Attributes.Add("selected", "");

                        select.Controls.Add(option);
                    }
                }
                else if (SetMonthType == true)
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        HtmlGenericControl option = new HtmlGenericControl("option");
                        option.InnerText = i.ToString() + "월";
                        option.Attributes.Add("value", i.ToString());

                        if (SetMonthTypeMask == "MM")
                        {
                            option.Attributes.Add("value", util.GetZeroNumber(i, 2));
                        }
                        else
                        {
                            option.Attributes.Add("value", i.ToString());
                        }

                        if (i == util.Convert.ObjToInt(value))
                            option.Attributes.Add("selected", "");

                        select.Controls.Add(option);
                    }
                }
                else
                {
                    if ((items is List<SelectListItem>))
                    {
                        List<SelectListItem> listItems = (List<SelectListItem>)items;

                        if (NoneFieldText != "")
                        {
                            HtmlGenericControl option = new HtmlGenericControl("option");
                            option.Attributes.Add("value", "");
                            option.InnerHtml = NoneFieldText;
                            select.Controls.Add(option);
                        }

                        foreach (SelectListItem item in listItems)
                        {
                            if (DisabledIgnore == false && item.Disabled == false)
                                continue;

                            HtmlGenericControl option = new HtmlGenericControl("option");
                            option.Attributes.Add("value", item.Value);
                            if (TextValueDisplay)
                                option.InnerHtml = item.Value + " " + item.Text;
                            else
                                option.InnerHtml = item.Text;

                            if (Multiple)
                            {
                                string[] values = util.Split(value, ",");

                                for (int j = 0; j < values.Length; j++)
                                {
                                    if (values[j] == item.Value)
                                        option.Attributes.Add("selected", "");
                                }
                            }
                            else
                            {
                                if (item.Value == value)
                                    option.Attributes.Add("selected", "");
                            }

                            select.Controls.Add(option);
                        }
                    }
                    else if ((items is List<Dictionary<string, string>>))
                    {
                        List<Dictionary<string, string>> listItems = (List<Dictionary<string, string>>)items;

                        if (NoneFieldText != "")
                        {
                            HtmlGenericControl option = new HtmlGenericControl("option");
                            option.Attributes.Add("value", "");
                            option.InnerHtml = NoneFieldText;
                            select.Controls.Add(option);
                        }

                        if (listItems != null && listItems.Count > 0)
                        {
                            foreach (Dictionary<string, string> itm in listItems)
                            {
                                ListItem item = new ListItem(itm[DataFiledName], itm[DataFiledCode]);

                                HtmlGenericControl option = new HtmlGenericControl("option");
                                option.Attributes.Add("value", item.Value);
                                if (TextValueDisplay)
                                    option.InnerHtml = item.Value + " " + item.Text;
                                else
                                    option.InnerHtml = item.Text;

                                if (Multiple)
                                {
                                    string[] values = util.Split(value, ",");

                                    for (int j = 0; j < values.Length; j++)
                                    {
                                        if (values[j] == item.Value)
                                            option.Attributes.Add("selected", "");
                                    }
                                }
                                else
                                {
                                    if (item.Value == value)
                                        option.Attributes.Add("selected", "");
                                }

                                select.Controls.Add(option);
                            }
                        }
                    }
                    else if ((items is DataTable))
                    {
                        if (NoneFieldText != "")
                        {
                            HtmlGenericControl option = new HtmlGenericControl("option");
                            option.Attributes.Add("value", "");
                            option.InnerHtml = NoneFieldText;
                            select.Controls.Add(option);
                        }

                        DataTable dt = (DataTable)items;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string Value = "";
                            string Name = "";

                            if (DataFiledCode == "")
                            {
                                Value = dt.Rows[i][0].ToString();
                            }
                            else
                            {
                                Value = dt.Rows[i][DataFiledCode].ToString();
                            }

                            if (DataFiledName == "")
                            {
                                Name = dt.Rows[i][1].ToString();
                            }
                            else
                            {
                                Name = dt.Rows[i][DataFiledName].ToString();
                            }

                            HtmlGenericControl option = new HtmlGenericControl("option");
                            option.Attributes.Add("value", Value);
                            if (TextValueDisplay)
                                option.InnerHtml = Value + " " + Name;
                            else
                                option.InnerHtml = Name;

                            if (Multiple)
                            {
                                string[] values = util.Split(value, ",");

                                for (int j = 0; j < values.Length; j++)
                                {
                                    if (values[j] == Value)
                                        option.Attributes.Add("selected", "");
                                }
                            }
                            else
                            {
                                if (Value == value)
                                    option.Attributes.Add("selected", "");
                            }

                            select.Controls.Add(option);
                        }
                    }
                    else if ((items is DataRow[]))
                    {
                        if (NoneFieldText != "")
                        {
                            HtmlGenericControl option = new HtmlGenericControl("option");
                            option.Attributes.Add("value", "");
                            option.InnerHtml = NoneFieldText;
                            select.Controls.Add(option);
                        }

                        DataRow[] dr = (DataRow[])items;
                        for (int i = 0; i < dr.Length; i++)
                        {
                            string Value = "";
                            string Name = "";

                            if (DataFiledCode == "")
                            {
                                Value = dr[i][0].ToString();
                            }
                            else
                            {
                                Value = dr[i][DataFiledCode].ToString();
                            }

                            if (DataFiledName == "")
                            {
                                Name = dr[i][1].ToString();
                            }
                            else
                            {
                                Name = dr[i][DataFiledName].ToString();
                            }

                            HtmlGenericControl option = new HtmlGenericControl("option");
                            option.Attributes.Add("value", Value);
                            if (TextValueDisplay)
                                option.InnerHtml = Value + " " + Name;
                            else
                                option.InnerHtml = Name;

                            if (Multiple)
                            {
                                string[] values = util.Split(value, ",");

                                for (int j = 0; j < values.Length; j++)
                                {
                                    if (values[j] == Value)
                                        option.Attributes.Add("selected", "");
                                }
                            }
                            else
                            {
                                if (Value == value)
                                    option.Attributes.Add("selected", "");
                            }

                            select.Controls.Add(option);
                        }
                    }
                }

                RtnHtml = util.GetHtml(select);
            }
            return MvcHtmlString.Create(RtnHtml);
        }

        /// <summary>
        /// 콤보박스의 Text 값 리턴
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ComboBoxText(this HtmlHelper helper, object items, string value = "", object htmlAttributes = null)
        {
            string RtnValue = "";

            using (Util util = new Util())
            {
                string DataFiledName = "";
                string DataFiledCode = "";
                bool Multiple = false;

                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        if (attributes.Key == "DataFiledName")
                        {
                            DataFiledName = attributes.Value.ToString();
                        }
                        else if (attributes.Key == "DataFiledCode")
                        {
                            DataFiledCode = attributes.Value.ToString();
                        }
                        else if (attributes.Key.ToLower() == "multiple")
                        {
                            Multiple = true;
                        }
                    }
                }

                if ((items is List<SelectListItem>))
                {
                    List<SelectListItem> listItems = (List<SelectListItem>)items;

                    if (listItems.Any(l => l.Value == value))
                    {
                        var selected = listItems.Where(x => x.Value == value).First();
                        RtnValue = selected.Text;
                    }
                }
                else if ((items is DataTable))
                {
                    DataTable dt = (DataTable)items;

                    int DataFiledCodeIndex = 0;
                    int DataFiledNameIndex = 1;

                    if (DataFiledCode != "")
                    {
                        if (dt.Columns.Contains(DataFiledCode))
                        {
                            DataFiledCodeIndex = dt.Columns.IndexOf(DataFiledCode);
                        }
                    }

                    if (DataFiledName != "")
                    {
                        if (dt.Columns.Contains(DataFiledName))
                        {
                            DataFiledNameIndex = dt.Columns.IndexOf(DataFiledName);
                        }
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (Multiple)
                        {
                            string[] values = util.Split(value, ",");

                            for (int j = 0; j < values.Length; j++)
                            {
                                if (values[j] == dt.Rows[i][DataFiledCodeIndex].ToString())
                                {
                                    if (RtnValue != "")
                                    {
                                        RtnValue += ",";
                                    }
                                    RtnValue += dt.Rows[i][DataFiledNameIndex].ToString();
                                }
                            }
                        }
                        else
                        {
                            if (dt.Rows[i][DataFiledCodeIndex].ToString() == value)
                            {
                                RtnValue = dt.Rows[i][DataFiledNameIndex].ToString();
                                break;
                            }
                        }
                    }
                }
            }

            return RtnValue;
        }

        /// <summary>
        /// 콤보박스의 Value Text 값 리턴
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ComboBoxValueText(this HtmlHelper helper, object items, string value = "", object htmlAttributes = null)
        {
            string RtnValue = "";
            bool TextValueDisplay = false;

            using (Util util = new Util())
            {
                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        if (attributes.Key == "TextValueDisplay")
                        {
                            TextValueDisplay = util.Convert.ObjToBool(attributes.Value.ToString());
                        }
                    }
                }

                if ((items is List<SelectListItem>))
                {
                    List<SelectListItem> listItems = (List<SelectListItem>)items;

                    if (listItems.Any(l => l.Value == value))
                    {
                        var selected = listItems.Where(x => x.Value == value).First();

                        if (TextValueDisplay)
                        {
                            RtnValue = selected.Value + " " + selected.Text;
                        }
                        else
                        {
                            RtnValue = selected.Text;
                        }
                        
                    }
                }
            }

            return RtnValue;
        }
    }
    #endregion

    #region -----------------RadioBox---------------------------
    /// <summary>
    /// RadioBox
    /// </summary>
    public static class RadioBoxHelper
    {
        /// <summary>
        /// RadioBox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="items"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlString RadioBox(this HtmlHelper helper, string id, object items, object htmlAttributes = null)
        {
            return RadioBox(helper, id, items, "", htmlAttributes);
        }

        /// <summary>
        /// RadioBox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlString RadioBox(this HtmlHelper helper, string id, object items, string value, object htmlAttributes = null)
        {
            System.Text.StringBuilder RtnHtml = new System.Text.StringBuilder();
            bool DisabledIgnore = true;
            bool Disabled = false;
            string DisplayType = "";
            string NoneFieldText = "";
            string DataFiledName = "";
            string DataFiledCode = "";
            string IsDefaultName = "";
            string EmptyFrist = "";
            string TmpHtml = "";
            string ButtonSize = "btn-sm";
            string Class = "";
            string strChecked = "";
            string strCheckedClass = "";
            string onclick = "";
            string onchange = "";
            bool isSelect = false;
            int index = 0;

            using (Util util = new Util())
            {
                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        if (attributes.Key == "DisabledIgnore")
                        {
                            DisabledIgnore = util.Convert.ObjToBool(attributes.Value.ToString());
                        }
                        else if (attributes.Key == "DisplayType")
                        {
                            DisplayType = attributes.Value.ToString();
                        }
                        else if (attributes.Key == "NoneFieldText")
                        {
                            NoneFieldText = attributes.Value.ToString();
                        }
                        else if (attributes.Key == "DataFiledName")
                        {
                            DataFiledName = attributes.Value.ToString();
                        }
                        else if (attributes.Key == "DataFiledCode")
                        {
                            DataFiledCode = attributes.Value.ToString();
                        }
                        else if (attributes.Key == "IsDefaultName")
                        {
                            IsDefaultName = attributes.Value.ToString();
                        }
                        else if (attributes.Key == "EmptyFrist")
                        {
                            EmptyFrist = attributes.Value.ToString();
                        }
                        else if (attributes.Key.ToLower() == "buttonsize")
                        {
                            ButtonSize = attributes.Value.ToString();
                        }
                        else if (attributes.Key.ToLower() == "class")
                        {
                            Class = attributes.Value.ToString();
                        }
                        else if (attributes.Key.ToLower() == "onclick")
                        {
                            onclick = attributes.Value.ToString();
                        }
                        else if (attributes.Key.ToLower() == "onchange")
                        {
                            onchange = attributes.Value.ToString();
                        }
                        else if (attributes.Key.ToLower() == "disabled")
                        {
                            if (util.Convert.ObjToBool(attributes.Value.ToString()))
                            {
                                Disabled = true;
                            }
                        }
                    }
                }

                if (DisplayType == "label")
                {
                    TmpHtml = "<label class='checkbox-inline i-checks'> <input type='radio' name='{name}' value='{value}'{checked}{disabled}> {text} </label>";
                }
                else if (DisplayType == "div")
                {
                    TmpHtml = "<div class='i-checks'><label style='font-weight:normal;cursor:pointer;'> <input type='radio' name='{name}' value='{value}'{checked}{disabled}> <i></i> {text} </label></div>";
                }
                else if (DisplayType == "buttons")
                {
                    TmpHtml = "<label class='btn " + ButtonSize + " {checkedClass} margin-bottom-none' onclick=\"{onclick}\" onchange=\"{onchange}\"> <input type='radio' name='{name}' value='{value}'{checked}> {text} </label>";
                    RtnHtml.Append("<div data-toggle='buttons' class='btn-group " + Class + "'>");
                }

                if ((items is List<SelectListItem>))
                {
                    List<SelectListItem> listItems = (List<SelectListItem>)items;

                    if (DisplayType != "")
                    {
                        if (NoneFieldText != "")
                        {
                            strChecked = "";
                            strCheckedClass = "btn-white";
                            string html = TmpHtml;
                            html = html.Replace("{name}", id);
                            html = html.Replace("{value}", "");

                            if ("" == value || value == null)
                            {
                                strChecked = " checked";
                                strCheckedClass = "btn-gray active";
                                isSelect = true;
                            }

                            html = html.Replace("{checked}", strChecked);
                            html = html.Replace("{checkedClass}", strCheckedClass);
                            html = html.Replace("{text}", "<span>" + NoneFieldText + "</span>");
                            html = html.Replace("{onclick}", onclick);
                            html = html.Replace("{onchange}", onchange);
                            if (Disabled)
                            {
                                html = html.Replace("{disabled}", " disabled='disabled'");
                            }
                            else
                            {
                                html = html.Replace("{disabled}", "");
                            }
                            RtnHtml.Append(html);
                        }

                        foreach (SelectListItem item in listItems)
                        {
                            if (DisabledIgnore == false && item.Disabled == false)
                                continue;

                            strChecked = "";
                            strCheckedClass = "btn-white";
                            string html = TmpHtml;
                            html = html.Replace("{name}", id);
                            html = html.Replace("{value}", item.Value);

                            if (isSelect == false)
                            {
                                if (value == item.Value)
                                {
                                    strChecked = " checked";
                                    strCheckedClass = "btn-gray active";
                                    isSelect = true;
                                }
                            }

                            html = html.Replace("{checked}", strChecked);
                            html = html.Replace("{checkedClass}", strCheckedClass);
                            html = html.Replace("{text}", "<span>" + item.Text + "</span>");
                            html = html.Replace("{onclick}", onclick);
                            html = html.Replace("{onchange}", onchange);
                            if (Disabled)
                            {
                                html = html.Replace("{disabled}", " disabled='disabled'");
                            }
                            else
                            {
                                html = html.Replace("{disabled}", "");
                            }
                            RtnHtml.Append(html);
                        }
                    }
                    else
                    {
                        if (NoneFieldText != "")
                        {
                            HtmlGenericControl label = new HtmlGenericControl("label");
                            HtmlGenericControl input = new HtmlGenericControl("input");

                            input.Attributes.Add("type", "radio");
                            input.Attributes.Add("id", id + "_" + index.ToString());
                            input.Attributes.Add("name", id);
                            input.Attributes.Add("id", id + "_" + index.ToString());
                            input.Attributes.Add("value", "");
                            if ("" == value)
                            {
                                input.Attributes.Add("checked", "checked");
                                isSelect = true;
                            }

                            label.Attributes.Add("for", id + "_" + index.ToString());
                            label.InnerHtml = NoneFieldText;

                            // 속성 값 세팅
                            if (htmlAttributes != null)
                            {
                                var attributesData = new RouteValueDictionary(htmlAttributes);

                                foreach (var attributes in attributesData)
                                {
                                    input.Attributes.Add(attributes.Key, attributes.Value.ToString());
                                }
                            }

                            RtnHtml.Append(util.GetHtml(input));
                            RtnHtml.Append(util.GetHtml(label));
                            RtnHtml.Append("&nbsp;&nbsp;&nbsp;");
                        }

                        foreach (SelectListItem item in listItems)
                        {
                            if (DisabledIgnore == false && item.Disabled == false)
                                continue;

                            index++;

                            HtmlGenericControl input = new HtmlGenericControl("input");
                            HtmlGenericControl label = new HtmlGenericControl("label");

                            input.Attributes.Add("type", "radio");
                            input.Attributes.Add("id", id + "_" + index.ToString());
                            input.Attributes.Add("name", id);
                            input.Attributes.Add("id", id + "_" + index.ToString());
                            input.Attributes.Add("value", item.Value);
                            if (item.Value == value)
                                input.Attributes.Add("checked", "checked");


                            label.Attributes.Add("for", id + "_" + index.ToString());
                            label.InnerHtml = item.Text;

                            // 속성 값 세팅
                            if (htmlAttributes != null)
                            {
                                var attributesData = new RouteValueDictionary(htmlAttributes);

                                foreach (var attributes in attributesData)
                                {
                                    input.Attributes.Add(attributes.Key, attributes.Value.ToString());
                                }
                            }

                            RtnHtml.Append(util.GetHtml(input));
                            RtnHtml.Append(util.GetHtml(label));
                            RtnHtml.Append("&nbsp;&nbsp;&nbsp;");
                        }
                    }
                }
                else if ((items is List<Dictionary<string, string>>))
                {
                    List<Dictionary<string, string>> listItems = (List<Dictionary<string, string>>)items;

                    // 첫번째 값으로 세팅으로 되어 있을 경우
                    if (EmptyFrist == "Y")
                    {
                        bool empIsSelect = false;

                        if (listItems != null && listItems.Count > 0)
                        {
                            foreach (Dictionary<string, string> itm in listItems)
                            {
                                bool isDefault = false;
                                ListItem item = new ListItem(itm[DataFiledName], itm[DataFiledCode]);
                                if (IsDefaultName != "")
                                {
                                    isDefault = (itm[IsDefaultName] == "Y" ? true : false);
                                }

                                if (empIsSelect == false)
                                {
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        if (item.Value == value)
                                        {
                                            empIsSelect = true;
                                        }
                                    }
                                    else
                                    {
                                        if (isDefault == true)
                                        {
                                            empIsSelect = true;
                                        }
                                    }
                                }
                            }

                            if (empIsSelect != true)
                            {
                                value = listItems[0][DataFiledCode];
                            }
                        }
                    }

                    if (DisplayType != "")
                    {
                        if (NoneFieldText != "")
                        {
                            strChecked = "";
                            strCheckedClass = "btn-white";
                            string html = TmpHtml;
                            html = html.Replace("{name}", id);
                            html = html.Replace("{value}", "");

                            if ("" == value || value == null)
                            {
                                strChecked = " checked";
                                strCheckedClass = "btn-gray active";
                                isSelect = true;
                            }

                            html = html.Replace("{checked}", strChecked);
                            html = html.Replace("{checkedClass}", strCheckedClass);
                            html = html.Replace("{text}", "<span>" + NoneFieldText + "</span>");
                            html = html.Replace("{onclick}", onclick);
                            html = html.Replace("{onchange}", onchange);

                            RtnHtml.Append(html);
                        }

                        foreach (Dictionary<string, string> itm in listItems)
                        {
                            bool isDefault = false;
                            ListItem item = new ListItem(itm[DataFiledName], itm[DataFiledCode]);
                            if (IsDefaultName != "")
                            {
                                isDefault = (itm[IsDefaultName] == "Y" ? true : false);
                            }

                            index++;

                            strChecked = "";
                            strCheckedClass = "btn-white";
                            string html = TmpHtml;
                            html = html.Replace("{name}", id);
                            html = html.Replace("{value}", item.Value);

                            if (isSelect == false)
                            {
                                if (!string.IsNullOrEmpty(value))
                                {
                                    if (item.Value == value)
                                    {
                                        strChecked = " checked";
                                        strCheckedClass = "btn-gray active";
                                        isSelect = true;
                                    }
                                }
                                else
                                {
                                    if (isDefault == true)
                                    {
                                        strChecked = " checked";
                                        strCheckedClass = "btn-gray active";
                                        isSelect = true;
                                    }
                                }
                            }

                            html = html.Replace("{checked}", strChecked);
                            html = html.Replace("{checkedClass}", strCheckedClass);
                            html = html.Replace("{text}", "<span>" + item.Text + "</span>");
                            html = html.Replace("{onclick}", onclick);
                            html = html.Replace("{onchange}", onchange);

                            RtnHtml.Append(html);
                        }
                    }
                    else
                    {
                        if (NoneFieldText != "")
                        {
                            HtmlGenericControl label = new HtmlGenericControl("label");
                            HtmlGenericControl input = new HtmlGenericControl("input");

                            input.Attributes.Add("type", "radio");
                            input.Attributes.Add("id", id + "_" + index.ToString());
                            input.Attributes.Add("name", id);
                            input.Attributes.Add("id", id + "_" + index.ToString());
                            input.Attributes.Add("value", "");
                            if ("" == value)
                            {
                                input.Attributes.Add("checked", "checked");
                                isSelect = true;
                            }

                            label.Attributes.Add("for", id + "_" + index.ToString());
                            label.InnerHtml = NoneFieldText;

                            // 속성 값 세팅
                            if (htmlAttributes != null)
                            {
                                var attributesData = new RouteValueDictionary(htmlAttributes);

                                foreach (var attributes in attributesData)
                                {
                                    input.Attributes.Add(attributes.Key, attributes.Value.ToString());
                                }
                            }

                            RtnHtml.Append(util.GetHtml(input));
                            RtnHtml.Append(util.GetHtml(label));
                            RtnHtml.Append("&nbsp;&nbsp;&nbsp;");
                        }

                        if (listItems != null && listItems.Count > 0)
                        {
                            foreach (Dictionary<string, string> itm in listItems)
                            {
                                bool isDefault = false;
                                ListItem item = new ListItem(itm[DataFiledName], itm[DataFiledCode]);
                                if (IsDefaultName != "")
                                {
                                    isDefault = (itm[IsDefaultName] == "Y" ? true : false);
                                }

                                index++;

                                HtmlGenericControl input = new HtmlGenericControl("input");
                                HtmlGenericControl label = new HtmlGenericControl("label");

                                input.Attributes.Add("type", "radio");
                                input.Attributes.Add("id", id + "_" + index.ToString());
                                input.Attributes.Add("name", id);
                                input.Attributes.Add("id", id + "_" + index.ToString());
                                input.Attributes.Add("value", item.Value);

                                if (isSelect == false)
                                {
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        if (item.Value == value)
                                        {
                                            input.Attributes.Add("checked", "checked");
                                            isSelect = true;
                                        }
                                    }
                                    else
                                    {
                                        if (isDefault == true)
                                        {
                                            input.Attributes.Add("checked", "checked");
                                            isSelect = true;
                                        }
                                    }
                                }

                                label.Attributes.Add("for", id + "_" + index.ToString());
                                label.InnerHtml = item.Text;

                                // 속성 값 세팅
                                if (htmlAttributes != null)
                                {
                                    var attributesData = new RouteValueDictionary(htmlAttributes);

                                    foreach (var attributes in attributesData)
                                    {
                                        input.Attributes.Add(attributes.Key, attributes.Value.ToString());
                                    }
                                }

                                RtnHtml.Append(util.GetHtml(input));
                                RtnHtml.Append(util.GetHtml(label));
                                RtnHtml.Append("&nbsp;&nbsp;&nbsp;");
                            }
                        }
                    }
                }

                if (DisplayType == "buttons")
                {
                    RtnHtml.Append("</div>");
                }
            }

            return MvcHtmlString.Create(RtnHtml.ToString());
        }
    }
    #endregion

    #region -----------------CheckBox---------------------------
    public static class CheckBoxHelper
    {
        /// <summary>
        /// CheckBox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="items"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlString CheckBox(this HtmlHelper helper, string id, object items, object htmlAttributes = null)
        {
            return CheckBox(helper, id, items, "", htmlAttributes);
        }

        public static IHtmlString CheckBox(this HtmlHelper helper, string id, object items, string value, object htmlAttributes = null)
        {
            using (Util util = new Util())
            {
                if (value == null)
                {
                    value = "";
                }

                string[] values = util.Split(value, ",");
                return CheckBox(helper, id, items, values, htmlAttributes);
            }
        }

        /// <summary>
        /// CheckBox
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="items"></param>
        /// <param name="value"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static IHtmlString CheckBox(this HtmlHelper helper, string id, object items, string[] values, object htmlAttributes = null)
        {
            System.Text.StringBuilder RtnHtml = new System.Text.StringBuilder();

            using (Util util = new Util())
            {
                bool Disabled = false;
                string DisplayType = "label";
                string DataFiledName = "";
                string DataFiledCode = "";
                string strChecked = "";
                string strCheckedClass = "";
                string TmpHtml = "";
                string onclick = "";
                string onchange = "";
                string AllText = "";
                string ButtonSize = "btn-sm";
                string Class = "";

                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        if (attributes.Key == "DataFiledName")
                        {
                            DataFiledName = attributes.Value.ToString();
                        }
                        else if (attributes.Key == "DataFiledCode")
                        {
                            DataFiledCode = attributes.Value.ToString();
                        }
                        else if (attributes.Key == "DisplayType")
                        {
                            DisplayType = attributes.Value.ToString();
                        }
                        else if (attributes.Key.ToLower() == "onclick")
                        {
                            onclick = attributes.Value.ToString();
                        }
                        else if (attributes.Key.ToLower() == "onchange")
                        {
                            onchange = attributes.Value.ToString();
                        }
                        else if (attributes.Key.ToLower() == "alltext")
                        {
                            AllText = attributes.Value.ToString();
                        }
                        else if (attributes.Key.ToLower() == "buttonsize")
                        {
                            ButtonSize = attributes.Value.ToString();
                        }
                        else if (attributes.Key.ToLower() == "class")
                        {
                            Class = attributes.Value.ToString();
                        }
                        else if (attributes.Key.ToLower() == "disabled")
                        {
                            if (util.Convert.ObjToBool(attributes.Value.ToString()))
                            {
                                Disabled = true;
                            }
                        }
                    }
                }

                if (DisplayType == "label")
                {
                    TmpHtml = "<label class='checkbox-inline i-checks'> <input type='checkbox' name='{name}' value='{value}'{checked}{disabled}> {text} </label>";
                }
                else if (DisplayType == "div")
                {
                    TmpHtml = "<div class='i-checks'><label style='font-weight:normal;cursor:pointer;'> <input type='checkbox' name='{name}' value='{value}'{checked}{disabled}> <i></i> {text} </label></div>";
                }
                else if (DisplayType == "buttons")
                {
                    TmpHtml = "<label class='btn " + ButtonSize + " {checkedClass} margin-bottom-none' onclick=\"{onclick}\" onchange=\"{onchange}\"> <input type='checkbox' name='{name}' value='{value}'{checked}> {text} </label>";
                    RtnHtml.Append("<div data-toggle='buttons' class='btn-group " + Class + "'>");
                }

                if (AllText != "")
                {
                    string html = TmpHtml;
                    html = html.Replace("{name}", id);
                    html = html.Replace("{value}", "");
                    html = html.Replace("{checked}", "");
                    html = html.Replace("{checkedClass}", "btn-white");
                    html = html.Replace("{text}", AllText);
                    html = html.Replace("{onclick}", onclick);
                    html = html.Replace("{onchange}", "Common.Controls.Fn.CheckAll($(this));" + onchange);
                    if (Disabled)
                    {
                        html = html.Replace("{disabled}", " disabled='disabled'");
                    }
                    else
                    {
                        html = html.Replace("{disabled}", "");
                    }

                    RtnHtml.Append(html);
                }

                if ((items is List<SelectListItem>))
                {
                    List<SelectListItem> listItems = (List<SelectListItem>)items;

                    foreach (SelectListItem item in listItems)
                    {
                        strChecked = "";
                        strCheckedClass = "btn-white";
                        string html = TmpHtml;
                        html = html.Replace("{name}", id);
                        html = html.Replace("{value}", item.Value);

                        for (int j = 0; j < values.Length; j++)
                        {
                            if (values[j] == item.Value)
                            {
                                strChecked = " checked";
                                strCheckedClass = "btn-gray active";
                                break;
                            }
                        }

                        html = html.Replace("{checked}", strChecked);
                        html = html.Replace("{checkedClass}", strCheckedClass);
                        html = html.Replace("{text}", "<span>" + item.Text + "</span>");
                        html = html.Replace("{onclick}", onclick);
                        html = html.Replace("{onchange}", onchange);
                        if (Disabled)
                        {
                            html = html.Replace("{disabled}", " disabled='disabled'");
                        }
                        else
                        {
                            html = html.Replace("{disabled}", "");
                        }
                        RtnHtml.Append(html);
                    }
                }
                else if ((items is DataTable))
                {
                    DataTable dt = (DataTable)items;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string Value = "";
                        string Name = "";

                        if (DataFiledCode == "")
                        {
                            Value = dt.Rows[i][0].ToString();
                        }
                        else
                        {
                            Value = dt.Rows[i][DataFiledCode].ToString();
                        }

                        if (DataFiledName == "")
                        {
                            Name = dt.Rows[i][1].ToString();
                        }
                        else
                        {
                            Name = dt.Rows[i][DataFiledName].ToString();
                        }

                        strChecked = "";
                        strCheckedClass = "btn-white";
                        string html = TmpHtml;
                        html = html.Replace("{name}", id);
                        html = html.Replace("{value}", Value);

                        for (int j = 0; j < values.Length; j++)
                        {
                            if (values[j] == Value)
                            {
                                strChecked = " checked";
                                strCheckedClass = "btn-gray active";
                                break;
                            }
                        }

                        html = html.Replace("{checked}", strChecked);
                        html = html.Replace("{checkedClass}", strCheckedClass);
                        html = html.Replace("{text}", Name);
                        html = html.Replace("{onclick}", onclick);
                        html = html.Replace("{onchange}", onchange);
                        if (Disabled)
                        {
                            html = html.Replace("{disabled}", " disabled='disabled'");
                        }
                        else
                        {
                            html = html.Replace("{disabled}", "");
                        }
                        RtnHtml.Append(html);
                    }
                }
                else if ((items is DataRow[]))
                {
                    DataRow[] dr = (DataRow[])items;
                    for (int i = 0; i < dr.Length; i++)
                    {
                        string Value = "";
                        string Name = "";

                        if (DataFiledCode == "")
                        {
                            Value = dr[i][0].ToString();
                        }
                        else
                        {
                            Value = dr[i][DataFiledCode].ToString();
                        }

                        if (DataFiledName == "")
                        {
                            Name = dr[i][1].ToString();
                        }
                        else
                        {
                            Name = dr[i][DataFiledName].ToString();
                        }

                        strChecked = "";
                        strCheckedClass = "btn-white";
                        string html = TmpHtml;
                        html = html.Replace("{name}", id);
                        html = html.Replace("{value}", Value);

                        for (int j = 0; j < values.Length; j++)
                        {
                            if (values[j] == Value)
                            {
                                strChecked = " checked";
                                strCheckedClass = "btn-gray active";
                                break;
                            }
                        }

                        html = html.Replace("{checked}", strChecked);
                        html = html.Replace("{checkedClass}", strCheckedClass);
                        html = html.Replace("{text}", Name);
                        html = html.Replace("{onclick}", onclick);
                        html = html.Replace("{onchange}", onchange);
                        if (Disabled)
                        {
                            html = html.Replace("{disabled}", " disabled='disabled'");
                        }
                        else
                        {
                            html = html.Replace("{disabled}", "");
                        }
                        RtnHtml.Append(html);
                    }
                }
                else if ((items is List<string>))
                {
                    List<string> data = (List<string>)items;
                    if (data != null && data.Count > 0)
                    {
                        int selectValue = util.Convert.ObjToInt(values[0], -1);
                        int intValue = 1;
                        for (int i = 0; i < data.Count; i++)
                        {
                            string Name = data[i];
                            string Value = intValue.ToString();

                            strChecked = "";
                            strCheckedClass = "btn-white";
                            string html = TmpHtml;

                            if (selectValue != -1)
                            {
                                if ((intValue & selectValue) > 0)
                                {
                                    strChecked = " checked";
                                    strCheckedClass = "btn-gray active";
                                }
                            }

                            html = html.Replace("{name}", id);
                            html = html.Replace("{value}", Value);

                            html = html.Replace("{checked}", strChecked + " multiNumber='multiNumber'");
                            html = html.Replace("{checkedClass}", strCheckedClass);
                            html = html.Replace("{text}", Name);
                            html = html.Replace("{onclick}", onclick);
                            html = html.Replace("{onchange}", onchange);
                            if (Disabled)
                            {
                                html = html.Replace("{disabled}", " disabled='disabled'");
                            }
                            else
                            {
                                html = html.Replace("{disabled}", "");
                            }
                            RtnHtml.Append(html);
                            intValue = intValue * 2;
                        }
                    }
                }

                if (DisplayType == "buttons")
                {
                    RtnHtml.Append("</div>");
                }
            }

            return MvcHtmlString.Create(RtnHtml.ToString());
        }
    }
    #endregion

    #region -----------------MaskBox---------------------------
    /// <summary>
    /// Mask Control
    /// </summary>
    public static class MaskBoxHelper
    {
        /// <summary>
        /// Mask Control [js File(Common.Control Mask Box 참조)]
        /// </summary>
        /// <param name="helper">Helper</param>
        /// <param name="id">아이디</param>
        /// <param name="mask">Mask 값</param>
        /// <param name="maskMode">mask 모드</param>
        /// <param name="htmlAttributes">속성</param>
        /// <returns></returns>
        public static IHtmlString MaskBox(this HtmlHelper helper, string id, string mask, string maskMode, object htmlAttributes = null)
        {
            return MaskBox(helper, id, mask, maskMode, "", htmlAttributes);
        }

        /// <summary>
        /// Mask Control [js File(Common.Control Mask Box 참조)]
        /// </summary>
        /// <param name="helper">Helper</param>
        /// <param name="id">아이디</param>
        /// <param name="mask">Mask 값</param>
        /// <param name="maskMode">mask 모드</param>
        /// <param name="value">기본값</param>
        /// <param name="htmlAttributes">속성</param>
        /// <returns></returns>
        public static IHtmlString MaskBox(this HtmlHelper helper, string id, string mask, string maskMode, string value, object htmlAttributes = null)
        {
            HtmlGenericControl input = null;
            string RtnHtml = "";

            try
            {
                input = new HtmlGenericControl("input");
                input.Attributes.Add("type", "text");
                input.Attributes.Add("id", id);
                input.Attributes.Add("name", id);
                input.Attributes.Add("onkeydown", "Common.Controls.MaskBox.MaskKeyDown(event, $(this));");
                input.Attributes.Add("onkeyup", "Common.Controls.MaskBox.MaskKeyUp(event, $(this));");

                if (value != "")
                {
                    input.Attributes.Add("value", value);
                }

                input.Attributes.Add("Mask", mask);

                if (maskMode == "1")
                    input.Attributes.Add("MaskMode", "numericMandatory");
                else if (maskMode == "2")
                    input.Attributes.Add("MaskMode", "alphaNumeric");
                else if (maskMode == "3")
                    input.Attributes.Add("MaskMode", "numericSpace");

                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        input.Attributes.Add(attributes.Key, attributes.Value.ToString());
                    }
                }

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
    }
    #endregion

    #region -----------------DateBox---------------------------
    /// <summary>
    /// 날짜
    /// </summary>
    public static class DateBoxHelper
    {

        public static IHtmlString DateBox(this HtmlHelper helper, string id, object htmlAttributes = null)
        {
            return DateBox(helper, id, "", htmlAttributes);
        }

        public static IHtmlString DateBox(this HtmlHelper helper, string id, string value, object htmlAttributes = null)
        {
            HtmlGenericControl div = null;
            HtmlGenericControl input = null;
            HtmlGenericControl em = null;
            string RtnHtml = "";

            try
            {
                div = new HtmlGenericControl("div");
                div.Attributes.Add("class", "wrap_input");

                input = new HtmlGenericControl("input");
                input.Attributes.Add("type", "text");
                input.Attributes.Add("id", id);
                input.Attributes.Add("name", id);

                em = new HtmlGenericControl("em");
                em.Attributes.Add("class", "ico calendar");

                if (value != "")
                    input.Attributes.Add("value", value);

                // 속성 값 세팅
                if (htmlAttributes != null)
                {
                    var attributesData = new RouteValueDictionary(htmlAttributes);

                    foreach (var attributes in attributesData)
                    {
                        input.Attributes.Add(attributes.Key, attributes.Value.ToString());
                    }
                }

                using (Util util = new Util())
                {
                    div.InnerHtml = util.GetHtml(input) + " " + util.GetHtml(em);
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

                if (input != null)
                    input.Dispose(); input = null;

                if (em != null)
                    em.Dispose(); em = null;
            }

            return MvcHtmlString.Create(RtnHtml);
        }
    }
    #endregion
}
