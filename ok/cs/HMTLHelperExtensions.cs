using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using WebUtil;

namespace Interlock
{
    /// <summary>
    /// 
    /// </summary>
    public static class HMTLHelperExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="cssClass"></param>
        /// <returns></returns>
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {

            if (String.IsNullOrEmpty(cssClass)) 
                cssClass = "active";

            string currentAction = (string)html.ViewBag.ActionName;
            string currentController = (string)html.ViewBag.ControllerName;

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(action))
                action = currentAction;

            return controller == currentController && action == currentAction ?
                cssClass : String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string PageClass(this HtmlHelper html)
        {
            string currentAction = (string)html.ViewBag.ActionName;
            return currentAction;
        }
    }
}
