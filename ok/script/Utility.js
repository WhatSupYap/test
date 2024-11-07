{//이준희(2010-10-05): Added a singleton handler to prevent duplicated loading of this file.
    try {
        CreateXmlDocument.toString();
    }
    catch (e) {
        var _Browser = "";
        var _BVersion = null;
        fnWorkplace2UtilCommon();
        var m_evalXML = CreateXmlDocument(); //alert(document.location.href + ': m_evalXML');
        var m_xmlHTTP = CreateXmlHttpRequest();
        var m_xmlHTTP2 = CreateXmlHttpRequest(); //shj 2015-01-30 : 한페이지에서 두개의 웹서비스 호출 시 사용
        var m_xmlDom = CreateXmlDocument();
        var m_xslProcessor = CreateXmlDocument();
        var m_oReceive = CreateXmlDocument();
        var m_oAdd = CreateXmlDocument();
    }
}

function isWindow() {
    return "ActiveXObject" in window || window.ActiveXObject;
}

function fnWorkplace2UtilCommon() {
    /**
    * @func CreateXmlDocument() { return object }
    * @brief DOM 객체를 호출함.
    */
    CreateXmlDocument = function() {
        /* AOP 그룹웨어 Covision */
        if (isWindow()) {
            return new ActiveXObject("Microsoft.XMLDOM");
        } else if (document.implementation.createDocument) {// code for Mozilla, Firefox, Opera, etc.
            return document.implementation.createDocument("", "", null);
        } else {
            alert('Your browser cannot handle this script');
            return null;
        }
    }

    /**
    * @func CreateXmlHttpRequest() { return object }
    * @brief XMLHTTP 객체를 호출함.
    */
    CreateXmlHttpRequest = function () {
        /* AOP 그룹웨어 Covision */
        if (isWindow()) {
            try {
                return new ActiveXObject("Microsoft.XMLHTTP");
            } catch (e) {
                return new ActiveXObject("Msxml.XMLHTTP");
            }
        }
        else if (window.XMLHttpRequest) { return new XMLHttpRequest(); }
        else { return null; }
    }

    SelectSingleNode = function(xmlDoc, elementPath) {
        /* AOP 그룹웨어 Covision */
        if (isWindow()) {
            if (elementPath == "response/error") {
                return null;
            } else {
                return xmlDoc.selectSingleNode(elementPath).text;
            }
        }
        else {
            var xpe = new XPathEvaluator();
            var nsResolver = xpe.createNSResolver(xmlDoc.ownerDocument == null ? xmlDoc.documentElement : xmlDoc.ownerDocument.documentElement);
            var results = xpe.evaluate(elementPath, xmlDoc, nsResolver, XPathResult.FIRST_ORDERED_NODE_TYPE, null);

            if (elementPath == "response/error") {
                return null;
            } else {
                return results.singleNodeValue.textContent;
            }
        }
    }
    /* AOP 그룹웨어 Covision */
    if (!isWindow()) {

        // JScript 파일
        // prototying the XMLDocument.selectNodes        
        XMLDocument.prototype.selectNodes = function(cXPathString, xNode) {
            if (!xNode) { xNode = this; }
            //        var resolver=null;
            //        if (!resolver){
            //            if (this.nodeType == Node.DOCUMENT_NODE){
            //                resolver = document.createNSResolver (this.documentElement);
            //            }else if(this.nodeType == Node.ELEMENT_NODE && this.ownerDocument && this.ownerDocument.documentElement ){
            //                resolver = document.createNSResolver (this.ownerDocument.documentElement);
            //            }else{
            //                resolver = document.createNSResolver (this);
            //            }
            //        }

            //        var oNSResolver = document.createNSResolver(this.ownerDocument == null ? this.documentElement : this.ownerDocument.documentElement);
            //       
            //        function resolver() {
            //            return 'http://schemas.saarchitect.net/ajax/2008/09/user';
            //        }
            var xpe = new XPathEvaluator();
            var nsResolver = xpe.createNSResolver(xNode.ownerDocument == null ?
        xNode.documentElement : xNode.ownerDocument.documentElement);

            var aItems = xpe.evaluate(cXPathString, xNode, nsResolver, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);
            var xns = new XMLNodes(aItems);
            //        var array = [];
            //        for (var i = 0; i < aItems.snapshotLength; i++) {
            //            array[i] = aItems.snapshotItem(i);
            //        }
            //        return array;
            return xns;
        }
        XMLNodes = function(result) {
            this.length = 0;
            this.pointer = 0;
            this.item = new Array();
            var i = 0;
            //        while((this.array[i]=result.iterateNext())!=null){
            //        i++;
            //        this.length = this.array.length;
            //        }
            var array1 = [];
            for (var i = 0; i < result.snapshotLength; i++) {
                try {
                    array1[array1.length] = result.snapshotItem(i);
                } catch (e) { alert(e.message) }
            }
            this.item = array1;
            this.length = this.item.length;

            XMLNodes.prototype.nextNode = function() {
                this.pointer++;
                return this.item[this.pointer - 1];
            }
            XMLNodes.prototype.reset = function() {
                this.pointer = 0;
            }
        }
        // prototying the Element
        Element.prototype.selectNodes = function(cXPathString) {
            if (this.ownerDocument.selectNodes) {
                return this.ownerDocument.selectNodes(cXPathString, this);
            }
            else { throw "For XML Elements Only"; }
        }
        // prototying the XMLDocument.selectSingleNode
        XMLDocument.prototype.selectSingleNode = function(cXPathString, xNode) {
            if (!xNode) { xNode = this; }
            var xItems = this.selectNodes(cXPathString, xNode);
            if (xItems.length > 0) {
                // xItems.item[0].text =xItems.item[0].textContent;
                //  xItems.item[0].xml =  (new XMLSerializer()).serializeToString(xItems.item[0]);
                return xItems.item[0];
            } else {
                return null;
            }
        }
        // prototying the Element
        Element.prototype.selectSingleNode = function(cXPathString) {
            if (this.ownerDocument.selectSingleNode) {
                return this.ownerDocument.selectSingleNode(cXPathString, this);
            }
            else { throw "For XML Elements Only"; }

        }

        Node.prototype.transformNode = function(oXslDom) {
            var oProcessor = new XSLTProcessor();
            oProcessor.importStylesheet(oXslDom);
            var oResultDom = oProcessor.transformToDocument(this);
            //                    var oResultDom = oProcessor.transformToFragment(this, document);
            //                    var sResult = oResultDom.xml;
            var oSerializer = new XMLSerializer();
            var sResult = oSerializer.serializeToString(oResultDom);
            if (sResult.indexOf("<transformiix:result") > -1) {
                sResult = sResult.substring(sResult.indexOf(">") + 1,
                                        sResult.lastIndexOf("<"));
            }
            return sResult;
        };

        XMLDocument.prototype.loadXML = function(xmlString) {
            var breturn = false;
            try {
                var childNodes = this.childNodes;
                for (var i = childNodes.length - 1; i >= 0; i--)
                    this.removeChild(childNodes[i]);
                var dp = new DOMParser();
                var newDOM = dp.parseFromString(xmlString, "text/xml");
                var newElt = this.importNode(newDOM.documentElement, true);
                this.appendChild(newElt);
                breturn = true;
            } catch (e) {
            }
            return breturn;
        };
        XMLDocument.prototype.load = function(filePath) {
            var xmlhttp = new XMLHttpRequest();
            xmlhttp.open("GET", filePath, false);
            xmlhttp.setRequestHeader("Content-Type", "text/xml");
            xmlhttp.send(null);
            var newDOM = xmlhttp.responseXML;
            if (newDOM) {
                var newElt = this.importNode(newDOM.documentElement, true);
                this.appendChild(newElt);
                return true;
            }
        }

        Node.prototype.__defineGetter__("xml", function() { return (new XMLSerializer).serializeToString(this); });

        Node.prototype.__defineGetter__
      (
      "text",
      function() {
          //          var   cs   =   this.childNodes;   
          //          var   l   =   cs.length;   
          //          var   sb   =   new   Array(l);   
          //            
          //          for   (var   i   =   0;   i   <   l;   i++) {   
          //            sb[i]   =   cs[i].text.replace(/^\n/,   "");   
          //          }   
          //            
          //          return   sb.join("");
          return this.textContent;
      }
   );

        Node.prototype.transformNodeToObject = function(oXsltNode, oOutputDocument) {
            var doc = this.nodeType == 9 ? this : this.ownerDocument;
            var outDoc = oOutputDocument.nodeType == 9 ? oOutputDocument : oOutputDocument.ownerDocument;
            var processor = new XSLTProcessor();
            processor.importStylesheet(oXsltNode);
            var df = processor.transformToFragment(this, doc);
            if (df == null) return;

            while (oOutputDocument.hasChildNodes()) {
                oOutputDocument.removeChild(oOutputDocument.lastChild);
            }

            var cs = df.childNodes;
            var l = cs.length;

            for (var i = 0; i < l; i++) {
                oOutputDocument.appendChild(outDoc.importNode(cs[i], true));
            }
        };
    }

    // JScript 파일
    fn_GetWebRoot = function() {
        var RtUrl = "";
        try {
            RtUrl = "/" + ApplicationName + "/";
        }
        catch (exception) {
            alert(exception);
        }
        return RtUrl;
    }
    fn_GetAdminWebRoot = function() {
        var RtUrl = "";
        try {
            RtUrl = SiteAdminURL + "/" + ApplicationAdminName + "/";
        }
        catch (exception) {
            alert(exception);
        }
        return RtUrl;
    }
    // 사용자 BK 주소
    fn_GetUserWebRoot = function() {
        var RtUrl = "";
        try {
            RtUrl = "/" + ApplicationName + "/";
        }
        catch (exception) {
            alert(exception);
        }
        return RtUrl;
    }
    // 웹파트 주소
    fn_GetWebPartWebRoot = function() {
        var RtUrl = "";
        try {
            RtUrl = SiteWebPartURL + "/" + ApplicationWebPartName + "/";
        }
        catch (exception) {
            alert(exception);
        }
        return RtUrl;
    }
    // 전자 결재 주소
    fn_SetAppLocation = function(strUrl) {
        try {
            window.frames["App_Main"].document.location.href = fn_GetWebRoot() + strUrl;
        }
        catch (exception) {
            alert(exception);
        }
    }

    /************************************************************************
    함수명		: fn_OpenModalDialog
    작성목적	: 팝업창을 띠운다.
    Parameter :	sUrl - 띠울 URL
    sFeature - 창 속성
    Return	  :

작 성 자	: 
    최초작성일	: 2007.06.13
    최종작성일	:
    수정내역	:
    *************************************************************************/
    fn_OpenModalDialog = function(sUrl, sParam, sFeature) {
        try {
            var strReturn = "";
            if (sFeature != null) {
                //			var strNewFearture = ModifyDialogFeature(sFeature);
                //			strNewFearture = IeVersionChk(strNewFearture);

                var strNewFearture = ModifyDialogFeature(sFeature);

                strReturn = window.showModalDialog(fn_GetWebRoot() + "SiteReference/ModalDialog.htm?" + sUrl, sParam, strNewFearture);
            }
            else {
                strReturn = window.showModalDialog(fn_GetWebRoot() + "SiteReference/ModalDialog.htm?" + sUrl, null, sParam);
            }

            return strReturn;
        }
        catch (exception) {
            alert(exception);
        }
    }
    /************************************************************************
    함수명		: fn_OpenDialog
    작성목적	: 팝업창을 띠운다.
    Parameter :	sUrl - 띠울 URL
    sFrame - 띠울 Frame
    sFeature - 창 속성
    Return	  :

작 성 자	: 
    최초작성일	: 2007.06.13
    최종작성일	:
    수정내역	:
    *************************************************************************/
    fn_OpenDialog = function(sUrl, sFrame, sFeature) {
        var strNewFearture = ModifyWindowFeature(sFeature);
        window.open(sUrl, sFrame, strNewFearture).focus();
    }
    /************************************************************************
    함수명		: fn_OpenDialog2
    작성목적	: 팝업창을 띠운다.
    Parameter :	sUrl - 띠울 URL
    sFrame - 띠울 Frame
    iWidth - 창넓이
    iHeight - 창높이
    sMode - 창 속성 모드
    Return	  :

작 성 자	: 
    최초작성일	: 2007.06.13
    최종작성일	:
    수정내역	:
    *************************************************************************/
    fn_OpenDialog2 = function(sUrl, sFrame, iWidth, iHeight, sMode) {
        var iSize = 0;
        // 프레임명
        if (sFrame == "newMessageWindow" || sFrame == "") {
            sFrame = new String(Math.round(Math.random() * 100000));
        }

        if (sFrame.split("_")[0].toUpperCase() == "POPUPNOTICE") {
            iSize = sFrame.split("_")[1];
        }

        // 창속성
        var x = iWidth;
        var y = iHeight;

        var sx = window.screen.width / 2 - x / 2 + eval(iSize);
        var sy = window.screen.height / 2 - y / 2 - 40 + eval(iSize);

        var sFeature = "toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=0,resizable=1";

        if (sMode == 'fix') {
            sFeature = "toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=0,resizable=0";
        }
        else if (sMode == 'resize') {
            sFeature = "toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=0,resizable=1";
        }
        else if (sMode == 'scroll') {
            sFeature = "toolbar=0,location=0,directories=0,status=1,menubar=0,scrollbars=1,resizable=1";
        }

        if (sy < 0) {
            sy = 0;
        }

        var sz = ",top=" + sy + ",left=" + sx;

        sFeature += ",width=" + x + ",height=" + y + sz;

        return fn_OpenDialog(sUrl, sFrame, sFeature);
    }


    /************************************************************************
    함수명		: ModifyDialogFeature(feature)  : 모달창
    ModifyWindowFeature(feature)  : 오픈창
    작성목적	: OS, IE 버전별 팝업창 사이지 조정

작 성 자	: 
    최초작성일	: 2007.06.13
    최종작성일	:
    수정내역	:
    *************************************************************************/

    //윈도우XP SP2 적용시 브라우저에 상태바가 출력되어 강제로 창 사이즈 늘린다.
    // xp에 ie7.0 으로 변경 XP 또는 비스타 일경우 창 늘림
    ModifyDialogFeature = function(feature) {
        try {

            var bIsXP = false;
            var bIsXPSP2 = false;

            if (navigator.userAgent.indexOf("Windows NT 5.1") > -1 || navigator.userAgent.indexOf("Windows NT 6.0") > -1) {
                bIsXP = true;
                bIsXPSP2 = true;
            }

            //		if(bIsXP==true)
            //		{
            //		  if(navigator.userAgent.indexOf("SV1")>-1)
            //		  {
            //		   bIsXPSP2 = true;
            //		  }
            //		}
            var n = 0;

            //OS, IE 버젼별 처리
            //OS : 2000
            //		var dd = navigator.userAgent;
            //		alert(dd);
            if (navigator.userAgent.indexOf("Windows NT 5.0") > -1) {
                if (navigator.userAgent.indexOf("MSIE 6.0") > -1) { //IE6.0
                    n = 0;
                } else if (navigator.userAgent.indexOf("MSIE 7.0") > -1) { //IE7.0
                    n = 0;
                } else {
                    n = 0;
                }
                //OS : XP			
            } else if (navigator.userAgent.indexOf("Windows NT 5.1") > -1) {
                if (navigator.userAgent.indexOf("MSIE 6.0") > -1) { //IE6.0
                    n = 7;
                } else if (navigator.userAgent.indexOf("MSIE 7.0") > -1) { //IE7.0
                    n = -3;
                } else {
                    n = 20;
                }
                //OS : 2003
            } else if (navigator.userAgent.indexOf("Windows NT 5.2") > -1) {
                if (navigator.userAgent.indexOf("MSIE 6.0") > -1) { //IE6.0
                    n = 0;
                } else if (navigator.userAgent.indexOf("MSIE 7.0") > -1) { //IE7.0
                    n = -30;
                } else {
                    n = 20;
                }
                //OS : Vista
            } else if (navigator.userAgent.indexOf("Windows NT 6.0") > -1) {
                if (navigator.userAgent.indexOf("MSIE 6.0") > -1) { //IE6.0
                    n = 20;
                } else if (navigator.userAgent.indexOf("MSIE 7.0") > -1) { //IE7.0
                    n = -25;
                } else {
                    n = 20;
                }
            } else {
            }

            if (n != 0) {
                var strStyle = "DialogHeight".toUpperCase() + ":";
                var strStyleLen = strStyle.length;
                var strFeature = feature.toUpperCase();
                var strFeartureLen = strFeature.length;
                var i = 0;
                while (i < strFeartureLen) {
                    var j = i + strStyleLen;
                    if (strFeature.substring(i, j) == strStyle) {
                        var endstr = strFeature.indexOf(";", j);
                        if (endstr == -1) endstr = strFeature.length;
                        var strValue = strFeature.substring(j, endstr);
                        strValue = strValue.toUpperCase();
                        if (strValue.indexOf("PX") > (-1)) strValue = strValue.replace("PX", "");
                        if (strValue.length <= 0) return strFeature;
                        var strNewValue = (parseInt(strValue) + n).toString();
                        strFeature = strFeature.replace((strStyle + strValue), (strStyle + strNewValue));
                        return strFeature.toLowerCase();
                    }
                    i = strFeature.indexOf(";", i) + 1;
                    if (i == 0) break;
                }

                return strFeature;
            }
            else {
                return feature;
            }
        }
        catch (exception) {
            alert(exception);
        }
    }

    //윈도우XP SP2 적용시 브라우저에 상태바가 출력되어 강제로 창 사이즈 늘린다.
    // xp에 ie7.0 으로 변경 XP 또는 비스타 일경우 창 늘림
    ModifyWindowFeature = function(feature) {
        try {

            var bIsXP = false;
            var bIsXPSP2 = false;
            if (navigator.userAgent.indexOf("Windows NT 5.1") > -1 || navigator.userAgent.indexOf("Windows NT 6.0") > -1) {
                bIsXP = true;
                bIsXPSP2 = true;
            }

            //		if(bIsXP==true)
            //		{
            //		  if(navigator.userAgent.indexOf("SV1")>-1)
            //		  {
            //		   bIsXPSP2 = true;
            //		  }
            //		}

            var n = 0;

            //OS, IE 버젼별 처리
            //OS : 2000
            if (navigator.userAgent.indexOf("Windows NT 5.0") > -1) {
                if (navigator.userAgent.indexOf("MSIE 6.0") > -1) { //IE6.0
                    n = 19;
                } else if (navigator.userAgent.indexOf("MSIE 7.0") > -1) { //IE7.0
                    n = 20;
                } else {
                    n = 19;
                }
                //OS : XP			
            } else if (navigator.userAgent.indexOf("Windows NT 5.1") > -1) {
                if (navigator.userAgent.indexOf("MSIE 6.0") > -1) { //IE6.0
                    n = 20;
                } else if (navigator.userAgent.indexOf("MSIE 7.0") > -1) { //IE7.0
                    n = 20;
                } else {
                    n = 20;
                }
                //OS : 2003
            } else if (navigator.userAgent.indexOf("Windows NT 5.2") > -1) {
                if (navigator.userAgent.indexOf("MSIE 6.0") > -1) { //IE6.0
                    n = 20;
                } else if (navigator.userAgent.indexOf("MSIE 7.0") > -1) { //IE7.0
                    n = 20;
                } else {
                    n = 20;
                }
                //OS : Vista
            } else if (navigator.userAgent.indexOf("Windows NT 6.0") > -1) {
                if (navigator.userAgent.indexOf("MSIE 6.0") > -1) { //IE6.0
                    n = 20;
                } else if (navigator.userAgent.indexOf("MSIE 7.0") > -1) { //IE7.0
                    n = 20;
                } else {
                    n = 20;
                }
            } else if (navigator.userAgent.indexOf("Windows NT 6.1") > -1) {
                if (navigator.userAgent.indexOf("MSIE 6.0") > -1) { //IE6.0
                    n = 20;
                } else if (navigator.userAgent.indexOf("MSIE 7.0") > -1) { //IE7.0
                    n = 20;
                } else {
                    n = 20;
                }
            } else {
            }

            if (n != 0) {
                var strStyle = "Height".toUpperCase() + "=";
                var strStyleLen = strStyle.length;
                var strFeature = feature.toUpperCase();
                var strFeartureLen = strFeature.length;
                var i = 0;
                while (i < strFeartureLen) {
                    var j = i + strStyleLen;
                    if (strFeature.substring(i, j) == strStyle) {
                        var endstr = strFeature.indexOf(",", j);
                        if (endstr == -1) endstr = strFeature.length;
                        var strValue = strFeature.substring(j, endstr);
                        strValue = strValue.toUpperCase();
                        if (strValue.indexOf("PX") > (-1)) strValue = strValue.replace("PX", "");
                        if (strValue.length <= 0) return strFeature;
                        var strNewValue = (parseInt(strValue) + n).toString();
                        strFeature = strFeature.replace((strStyle + strValue), (strStyle + strNewValue));
                        return strFeature.toLowerCase();
                    }
                    i = strFeature.indexOf(",", i) + 1;
                    if (i == 0) break;
                }

                return strFeature;
            }
            else {
                return feature;
            }
        }
        catch (exception) {
            alert(exception);
        }
    }




    /************************************************************************
    함수명		: fn_DocumentPath
    작성목적	: Document Path를 반환한다.
    작 성 자	: 
    최초작성일	: 2007.06.14
    최종작성일	:
    수정내역	:
    *************************************************************************/

    fn_DocumentPath = function() {
        try {
            var strHref = "";
            var strPath = "";
            var arrPath = null;
            strHref = document.location.href;
            arrPath = strHref.split("/");
            for (var i = 0; i < arrPath.length - 1; i++) {
                strPath += arrPath[i] + "/";
            }
            return strPath;
        }
        catch (exception) {
            alert(exception);
        }
    }
    /************************************************************************
    함수명		: fn_LeadingZero
    작성목적	: 숫자를 2자리 문자열로 변환한다.
    Parameter :	iNum - 2자리 이하 숫자
    Return	  :	2자리 숫자 문자열




작 성 자	: 
    최초작성일	: 2007.06.14
    최종작성일	:
    수정내역	:
    *************************************************************************/
    fn_LeadingZero = function(iNum) {
        var strReturn;
        try {
            if (iNum < 10)
                strReturn = "0" + iNum;
            else
                strReturn = "" + iNum;
        }
        catch (exception) {
        }
        return strReturn;
    }
    /************************************************************************
    함수명		: fn_CheckStringLength(str, limit)
    작성목적	: 문자열 길이제한을 체크한다.
    Parameter :	str - 입력문자
    limit - 한계개수
					
    Return	  :	

작 성 자	: 
    최초작성일	: 2007.06.14
    최종작성일	:
    수정내역	:
    *************************************************************************/
    fn_CheckStringLength = function(str, limit) {
        if (str.length >= limit)
            event.returnValue = false;
    }

    /************************************************************************
    함수명		: fn_GetXmlDomDocument()
    작성목적	: Xml받아오기
    Parameter   : sPage 요청URL
    Return	    :	
    작 성 자	: 
    최초작성일	: 2007.06.14
    최종작성일	:
    수정내역	:
    *************************************************************************/
    fn_GetXmlDomDocument = function(sPage) {
        var oXmlDoc = new ActiveXObject("Microsoft.XMLDOM");
        oXmlDoc.async = false;
        oXmlDoc.load(sPage);

        if (oXmlDoc.readyState != 4 && !oXmlDoc.parseError) {
            alert("Error Retry!!");
            return; // window.event.returnValue = false;
        }
        else {
            return oXmlDoc;
        }
    }

    /*=======================================================================
    Function명 : Trim
    내용 : String 의 공백을 모두 제거한다.
    작  성  자  : 
    최초작성일  : 2007.06.14

========================================================================*/
    fn_Trim = function(sourceString) {
        var strResult;

        strResult = sourceString.replace(/\s/g, "");

        return strResult;
    }

    /*=======================================================================
    Function명 : Trim
    내용 : String 의 양쪽공백을 모두 제거한다.
    작  성  자  :
    최초작성일  :  2007.06.14
    ========================================================================*/
    fn_RLTrim = function(strSource) {
        return strSource.replace(/(^\s*)|(\s*$)/g, "");
    }

    /*=======================================================================
    Function명  : 
    내용        : 임시 메뉴 스크립트 개발자분들은 링크 걸으세요.
    작  성  자  :
    최초작성일  : 2007.06.14
    ========================================================================*/
    // for flash
    // top ect&search
    TopMenu = function(type) {
        switch (type) {
            // 그룹홈 
            case '1': location.href = fn_GetWebRoot(); // + 'default.aspx'; // alert('그룹홈'); 
                break;
            // 메일 
            case '2': location.href = "http://email.test.com/index_sso.html"; break;
            // 통합검색 
            case '3': alert('통합검색  준비중입니다.'); break; //통합검색  준비중입니다.
            // 웹하드 
            case '4': alert('웹하드  준비중입니다.'); break; //웹하드  준비중입니다.
            // 경영정보 
            case '5': alert('경영정보  준비중입니다.'); break; //경영정보  준비중입니다.
            // 고객지원 
            case '6': alert('고객지원  준비중입니다.'); break; //고객지원  준비중입니다.
            // 관리자 센터 
            case '7': location.href = '/Admin/default.aspx';
                //alert('관리자 센터  준비중입니다.'); 
                break;
            // 통합검색 
            case '8': alert('통합검색  준비중입니다.'); break; //통합검색  준비중입니다.
        }
        //location.href = '/';
    }
    // 1 My company
    Menu1 = function(type) {
        //alert("Mycompany");
        location.href = fn_GetWebRoot() + 'PortalService/Affiliate.aspx';
    }
    // community
    Menu2 = function(type) {
        switch (type) {
            // 까페 
            case '1': location.href = fn_GetWebRoot() + "BaseService/CLUB/CB_ClubMain.aspx";
                break;
            // 전자설문 
            case '2': location.href = fn_GetWebRoot() + "BaseService/Poll/PO_Main.aspx"; //location.href = fn_GetWebRoot() + 'BaseServise/Poll/PO_Main.aspx'; //alert('전자설문'); 
                break;
        }
        //location.href = '/';
    }
    // know
    Menu3 = function(type) {
        switch (type) {
            // 지식마켙 
            case '1': alert('지식마켙 준비중입니다.'); //location.href = fn_GetWebRoot() + 'ExtensionService/Knowledge/KW_Main.aspx'; //'지식마켙 준비중입니다.'
                break;
            // 오픈지식 
            case '2': alert('오픈지식 준비중입니다.'); // location.href = fn_GetWebRoot() + 'ExtensionService/Knowledge/KW_OpenDic_Main.aspx'; //'오픈지식 준비중입니다.'
                break;
            // cop 팀블로그 
            case '3': alert('팀블로그  준비중입니다.'); //'팀블로그  준비중입니다.'
                break;
            // 제안지식 
            case '4': alert('제안지식  준비중입니다.'); //'제안지식  준비중입니다.'
                break;
        }
        //location.href = '/';
    }
    // blog
    Menu4 = function(type) {
        switch (type) {
            // 블로그 
            case '1': alert('블로그  준비중입니다.'); break; //'블로그  준비중입니다.'
        }
        //location.href = '/';
    }
    // bk viliage
    Menu5 = function(type) {
        switch (type) {
            // 임직원프로필 
            case '1': alert('임직원프로필  준비중입니다.'); break; //'임직원프로필  준비중입니다.'
            // 임직원동점 
            case '2': alert('임직원동점  준비중입니다.'); break; //'임직원동점  준비중입니다.')
            // 보광전망대 
            case '3': alert('보광전망대  준비중입니다.'); break; //'보광전망대  준비중입니다.'
        }
        //location.href = '/';
    }

    ///////////////////////  이곳을 편집해주세요.
    // 서브 페이지
    // for flash Sub

    // 전자결재
    Sub_Menu1 = function(type) {
        //alert("전자결재  준비중입니다.");
        location.href = fn_GetWebRoot() + 'approval/appdefault.aspx';
    }
    // 알리미
    Sub_Menu2 = function(type) {
        switch (type) {
            // 알리미 
            case '1': location.href = fn_GetWebRoot() + "BaseService/Board/BD_BoardMain.aspx"; //alert('알리미  준비중입니다.'); break;
                break;
        }
        //location.href = '/';
    }
    // 업무지원
    Sub_Menu3 = function(type) {
        switch (type) {
            // 업무지원 
            case '1': alert('업무지원  준비중입니다.'); break; //'업무지원  준비중입니다.'
            // 일정관리 
            case '2': alert('일정관리  준비중입니다.'); break; //'일정관리  준비중입니다.'
            // 인명관리 
            case '3': location.href = fn_GetWebRoot() + 'BaseService/Contacts/CT_Main.aspx'; //alert('인명관리'); 
                break;
            // 자원예약 
            case '4': location.href = fn_GetWebRoot() + 'BaseService/Booking/BK_TodayBooking.aspx';
                break;
            // 방문예약 
            case '5': location.href = fn_GetWebRoot() + 'BaseService/VReservation/VR_VisitManagementList.aspx';
                break;
        }
        //location.href = '/';
    }
    // 문서관리
    Sub_Menu4 = function(type) {
        switch (type) {
            // 문서관리 
            case '1': location.href = fn_GetWebRoot() + 'ExtensionService/Doc/DM_Main.aspx'; // alert('그룹홈'); 
        }
        //location.href = '/';
    }
    // 테스크 관리
    Sub_Menu5 = function(type) {
        switch (type) {
            // 테스크관리 
            case '1': alert('테스크관리  준비중입니다.'); break; //'테스크관리  준비중입니다.'
            // 업무의뢰 
            case '2': alert('업무의뢰  준비중입니다.'); break; //'업무의뢰  준비중입니다.')
            // 프로젝트관리 
            case '3': alert('프로젝트관리  준비중입니다.'); break; //'프로젝트관리  준비중입니다.'
        }
        //location.href = '/';
    }
    // 통합검색
    Sub_Menu6 = function(type) {
        switch (type) {
            // 통합검색 
            case '1': alert('통합검색  준비중입니다.'); break; //'통합검색  준비중입니다.'
        }
        //location.href = '/';
    }
    /*/////////////////////////
    //// 그룹포탈 메인 플래시, 왼쪽 타이틀 플래시//
    /////////////////////////*/
    ShowFlashObject = function(strFlashUrl, nWith, nHeight) {
        document.write("<object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,19,0\" width=\"" + nWith + "\" height=\"" + nHeight + "\">");
        document.write("<param name=\"movie\" value=\"" + strFlashUrl + "\">");
        document.write("<param name=\"quality\" value=\"high\">");
        document.write("<embed src=\"" + strFlashUrl + "\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" width=\"" + nWith + "\" height=\"" + nHeight + "\"></embed>");
        document.write("</object>");

    }

    fn_MainflashTop = function(strFlashUrl, nWith, nHeight) {
        //    document.write("<object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=7,0,19,0\" width=\"" + nWith + "\" height=\"" + nHeight + "\">");
        //	document.write("<param name=\"movie\" value=\"" + strFlashUrl + "\">");
        //	document.write("<param name=\"quality\" value=\"high\"><PARAM NAME=wmode VALUE=transparent>");
        //	document.write("<embed src=\"" + strFlashUrl + "\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" type=\"application/x-shockwave-flash\" width=\"" + nWith + "\" height=\"" + nHeight + "\"></embed>");
        //	document.write("</object>");
    }

    fn_SubflashTop = function() {
        //    document.write("<object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" codebase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=9,0,28,0\" width=\"968\" height=\"84\">");
        //    document.write("<param name=\"movie\" value=\"/CoviWeb/images/Master/sub_menu.swf\">");
        //    document.write("<param name=\"quality\" value=\"high\">");
        //    document.write("<embed src=\"/CoviWeb/images/Master/sub_menu.swf\" quality=\"high\" pluginspage=\"http://www.adobe.com/shockwave/download/download.cgi?P1_Prod_Version=ShockwaveFlash\" type=\"application/x-shockwave-flash\" width=\"968\" height=\"84\"></embed>");
        //    document.write("</object>");
    }

    //조직도 오픈
    GetOrgMap = function(strEntcode, strSelectType, strSelectMulit, strType, strSelectJob, strXml) {

        /* //strEntcode 조직도에 기본으로 보여질 회사코드,''이 올경우 기본 값이 입력되어진다.
        //strEntcode 'CannotSelectOtherCompany' 을 전달하면 본인 소속 회사만 선택가능하다
        //strSelectType 선택구성원 타입 A:부서,개인 P:개인 U:부서 E:A + 회사,그룹
        //strSelectMulit 여러 구성원 선택 가능여부 Y:가능,N:불가능
        //strType 한페이지에서 조직도 여러번 사용할때 사용
        //오픈페이지에 OrgMap(oDomOrgMap,strType) 함수 만들어 줘야한다
        //결과값은 오픈페이지의 OrgMap function을 호출하여 oDomOrgMap에 들어간다 물론 Dom 형태임.
    
    var URL="/CoviWeb/PortalService/OrgMap/OM_OrgMap.aspx?SelectType="+strSelectType+"&SelectMulit="+strSelectMulit;
        if(strEntcode !="")
        URL += "&Entcode="+strEntcode;
        var oDomOrgMap =  window.showModalDialog(URL, "", "dialogWidth:790px; dialogHeight:662px");
        //var oDomOrgMap = fn_OpenModalDialog(URL, "", "dialogWidth:800px; dialogHeight:670px");
        if(oDomOrgMap != null)
        OrgMap(oDomOrgMap,strType);
        //    }
        */
        try {

            action = "acl";
            var rgParams = null;
            rgParams = new Array();

            rgParams["bMail"] = false;
            rgParams["bUser"] = false;
            rgParams["bGroup"] = true;
            rgParams["bRef"] = false;
            //rgParams["objMessage"] = (bAdmin=="true")?myform.admin_id:myform.lstMember;
            rgParams["bMember"] = true;
            rgParams["bAdmin"] = false;
            rgParams["strXml"] = strXml;

            var nFontHeight = 9;
            var nWidth = 960;
            var nHeight = 680;

            if (_Browser == "IE") {
                var oStyle = window.document.body.currentStyle;
                var szFont = "FONT:" + oStyle.fontFamily + ";font-size:" + nFontHeight + "px;";
            } else {
                var szFont = "font-size:" + nFontHeight + "px;";
            }

            if (window.navigator.userAgent.indexOf("MSIE 6.0") == 25) // IE 6.0
            {
                nWidth = (parseInt(nWidth) + 20).toString();
                nHeight = (parseInt(nHeight) + 50).toString();
            }

            var pass;
            //pass = "/CoviWeb/PortalService/Address/address.aspx?control=MsgTo&requestType=Poll" + "&strEntcode=" + strEntcode + "&strSelectType=" + strSelectType + "&strSelectMulit=" + strSelectMulit + "&strType=" + strType + "&strSelectJob=" + strSelectJob;
            /* AOP 그룹웨어 Covision - ModalChange */
            pass ="/CoviWeb/PortalService/Address/address.aspx"
            rgParams["control"] = "MsgTo";
            rgParams["requestType"] = "Poll";
            rgParams["strEntcode"] = strEntcode;
            rgParams["strSelectType"] = strSelectMulit;
            rgParams["strSelectMulit"] = strSelectMulit;
            rgParams["strType"] = strType;
            rgParams["strSelectJob"] = strSelectJob;
            
            var vRetval = fn_OpenDialog2(pass + makeUrl(rgParams), "", nWidth+"px", nHeight+"px", "help:no;scroll:no;status:no;");
            //var vRetval = window.showModalDialog(pass, rgParams, szFont + "dialogHeight:" + nHeight + "px;dialogWidth:" + nWidth + "px;status:no;resizable:yes;help:no;scroll=yes");
            //window.open (pass, rgParams, szFont+"dialogHeight:"+nHeight+"px;dialogWidth:"+nWidth+"px;status:no;resizable:yes;help:no;scroll=no");
            
            //2008.4.28 김시은 - 스키마 변경 -
            //스키마 변경!!! 기존 로레알 버전을 WorkPlace2.0 환경에 맞추기 위해 데이터를 변경해서 리턴해줌
            //불필요한 소스 수정을 줄이기 위해 org_memberquery.xsl의 Element를 수정하지 않고 아래와 같이 변경함.
            //
            if (vRetval != null) {
                if (strType == "_TARGET_") {
                    document.getElementById(el_ResearchTargetDataHiddenOrg).value = vRetval.xml;
                }
                else if (strType == "_VIEW_TARGET_") {
                    document.getElementById(el_ViewTargetDataHiddenOrg).value = vRetval.xml;
                }
                var m_objXML = CreateXmlDocument();
                var m_objXML2 = CreateXmlDocument();
                var m_oMemberXSLProcessor = makeProcessor(SiteURL + "/CoviWeb/PortalService/Address/schema.xsl");
                var oXml = vRetval.xml;
                m_objXML2.loadXML(oXml.replace("\r\n", "").replace("\t", ""));
                //m_oMemberXSLProcessor.input = vRetval;
                m_oMemberXSLProcessor.input = m_objXML2;
                m_oMemberXSLProcessor.transform();
                m_objXML.loadXML(m_oMemberXSLProcessor.output);
                OrgMap(m_objXML, strType);
            }
        }
        catch (e) {
            alert("오류가 발생했습니다. 오류 내용은 다음과 같습니다. \r\nError File : addmember(bAdmin) in Survey/Survey_New.aspx\r\nError Description: " + e.description);
        }



    }

    GetOrgMapConfirm = function (type, vRetval, strType) {

        if (type == true) {
            if (strType == "_TARGET_") {
                document.getElementById(el_ResearchTargetDataHiddenOrg).value = vRetval.xml;
            }
            else if (strType == "_VIEW_TARGET_") {
                document.getElementById(el_ViewTargetDataHiddenOrg).value = vRetval.xml;
            }
            var m_objXML = CreateXmlDocument();
            var m_objXML2 = CreateXmlDocument();
            var m_oMemberXSLProcessor = makeProcessor(SiteURL + "/CoviWeb/PortalService/Address/schema.xsl");
            var oXml = vRetval.xml;
            m_objXML2.loadXML(oXml.replace("\r\n", "").replace("\t", ""));
            m_oMemberXSLProcessor.input = m_objXML2;
            m_oMemberXSLProcessor.transform();
            m_objXML.loadXML(m_oMemberXSLProcessor.output);
            OrgMap(m_objXML, strType);
        }
    }

    // 업무보고 - 업무관리 다중 정보이동
    GetOrgMap_BusinessReport = function(strEntcode, strSelectType, strSelectMulit, strType, strSelectJob) {
        try {

            action = "acl";
            var rgParams = null;
            rgParams = new Array();

            rgParams["bMail"] = false;
            rgParams["bUser"] = false;
            rgParams["bGroup"] = true;
            rgParams["bRef"] = false;
            //rgParams["objMessage"] = (bAdmin=="true")?myform.admin_id:myform.lstMember;
            rgParams["bMember"] = true;
            rgParams["bAdmin"] = false;

            var nFontHeight = 9;
            var nWidth = 960;
            var nHeight = 680;

            if (_Browser == "IE") {
                var oStyle = window.document.body.currentStyle;
                var szFont = "FONT:" + oStyle.fontFamily + ";font-size:" + nFontHeight + "px;";
            } else {
                var szFont = "font-size:" + nFontHeight + "px;";
            }

            if (window.navigator.userAgent.indexOf("MSIE 6.0") == 25) // IE 6.0
            {
                nWidth = (parseInt(nWidth) + 20).toString();
                nHeight = (parseInt(nHeight) + 50).toString();
            }

            var pass;
            pass = "/CoviWeb/PortalService/Address/address.aspx?control=MsgTo&requestType=Poll" + "&strEntcode=" + strEntcode + "&strSelectType=" + strSelectType + "&strSelectMulit=" + strSelectMulit + "&strType=" + strType + "&strSelectJob=" + strSelectJob;
            var vRetval = window.showModalDialog(pass, rgParams, szFont + "dialogHeight:" + nHeight + "px;dialogWidth:" + nWidth + "px;status:no;resizable:yes;help:no;scroll=yes");
            //window.open (pass, rgParams, szFont+"dialogHeight:"+nHeight+"px;dialogWidth:"+nWidth+"px;status:no;resizable:yes;help:no;scroll=no");

            //2008.4.28 김시은 - 스키마 변경 -
            //스키마 변경!!! 기존 로레알 버전을 WorkPlace2.0 환경에 맞추기 위해 데이터를 변경해서 리턴해줌
            //불필요한 소스 수정을 줄이기 위해 org_memberquery.xsl의 Element를 수정하지 않고 아래와 같이 변경함.
            //
            if (vRetval != null) {
                var m_objXML = CreateXmlDocument();
                var m_objXML2 = CreateXmlDocument();
                var m_oMemberXSLProcessor = makeProcessor(SiteURL + "/CoviWeb/PortalService/Address/schema_BusinessReport.xsl");
                var oXml = vRetval.xml;
                m_objXML2.loadXML(oXml.replace("\r\n", "").replace("\t", ""));
                //m_oMemberXSLProcessor.input = vRetval;
                m_oMemberXSLProcessor.input = m_objXML2;
                m_oMemberXSLProcessor.transform();
                m_objXML.loadXML(m_oMemberXSLProcessor.output);
                OrgMap(m_objXML, strType);
            }
        }
        catch (e) {
            alert("오류가 발생했습니다. 오류 내용은 다음과 같습니다. \r\nError File : addmember(bAdmin) in Survey/Survey_New.aspx\r\nError Description: " + e.description);
        }
    }

    //2008.4.28 김시은 - 조직도 오픈 
    GetAddressMap = function(strEntcode, strSelectType, strSelectMulit, strType) {
        try {
            action = "acl";
            var rgParams = null;
            rgParams = new Array();

            rgParams["bMail"] = false;
            rgParams["bUser"] = false;
            rgParams["bGroup"] = true;
            rgParams["bRef"] = false;
            //rgParams["objMessage"] = (bAdmin=="true")?myform.admin_id:myform.lstMember;
            rgParams["bMember"] = true;
            rgParams["bAdmin"] = false;

            var oStyle = window.document.body.currentStyle;
            var nFontHeight = 9;
            var szFont = "FONT:" + oStyle.fontFamily + ";font-size:" + nFontHeight + "px;";
            var nWidth = 860;
            var nHeight = 630;

            var pass;
            pass = "/CoviWeb/PortalService/Address/address.aspx?control=MsgTo&requestType=Poll" + "&strEntcode=" + strEntcode + "&strSelectType=" + strSelectType + "&strSelectMulit=" + strSelectMulit + "&strType=" + strType;
            var vRetval = window.showModalDialog(pass, rgParams, szFont + "dialogHeight:" + nHeight + "px;dialogWidth:" + nWidth + "px;status:no;resizable:yes;help:no;scroll=yes");
            //window.open (pass, rgParams, szFont+"dialogHeight:"+nHeight+"px;dialogWidth:"+nWidth+"px;status:no;resizable:yes;help:no;scroll=no");	
            //2008.4.28 김시은 - 스키마 변경 -
            //스키마 변경!!! 기존 로레알 버전을 WorkPlace2.0 환경에 맞추기 위해 데이터를 변경해서 리턴해줌
            //불필요한 소스 수정을 줄이기 위해 org_memberquery.xsl의 Element를 수정하지 않고 아래와 같이 변경함.
            //

            if (vRetval != null) {
                var m_objXML = CreateXmlDocument();
                var m_objXML2 = CreateXmlDocument();
                var m_oMemberXSLProcessor = makeProcessor(SiteURL + "/CoviWeb/PortalService/Address/schema.xsl");
                var oXml = vRetval.xml;
                m_objXML2.loadXML(oXml.replace("\r\n", "").replace("\t", ""));
                //m_oMemberXSLProcessor.input = vRetval;
                m_oMemberXSLProcessor.input = m_objXML2;
                m_oMemberXSLProcessor.transform();
                m_objXML.loadXML(m_oMemberXSLProcessor.output);
                OrgMap(m_objXML, strType);
            }
        }
        catch (e) {
            alert("오류가 발생했습니다. 오류 내용은 다음과 같습니다. \r\nError File : addmember(bAdmin) in Survey/Survey_New.aspx\r\nError Description: " + e.description);
        }
    }

    //2008.4.29 김시은
    //function makeProcessor(urlXsl){
    //	// XSL 문서를 DOM 객체로 로딩
    //	var oXslDom = new ActiveXObject("MSXML2.FreeThreadedDOMDocument");
    //	oXslDom.async = false;
    //	if(!oXslDom.load(urlXsl))
    //	{
    //	    
    //		alertParseError(oXslDom.parseError);
    //		throw new Error(-1,"couldn't make TemplateProcessor with ["+urlXsl+"].");
    //	}
    //	// XML 문서와 XSL 문서를 병합하여 결과를 저장할 XSLTemplate 객체 생성 
    //	var oXSLTemplate = new ActiveXObject("MSXML2.XSLTemplate");
    //	oXSLTemplate.stylesheet = oXslDom;
    //	// XSLTemplate 프로세서 생성
    //	return oXSLTemplate.createProcessor();
    //}

    makeProcessor = function (urlXsl) {
        /* AOP 그룹웨어 Covision */
        if (isWindow()) {
            var oXslDom = new ActiveXObject("MSXML2.FreeThreadedDOMDocument");
            oXslDom.async = false;
            if (!oXslDom.load(urlXsl)) {
                alertParseError(oXslDom.parseError);
                throw new Error(-1, "couldn't make TemplateProcessor with [" + urlXsl + "].");
            }
            var oXSLTemplate = new ActiveXObject("MSXML2.XSLTemplate");
            oXSLTemplate.stylesheet = oXslDom;
            return oXSLTemplate.createProcessor();
        } else {
            var oXSL = "";
            var oXslDom = CreateXmlDocument();
            //        if (urlXsl.indexOf(".xsl") > -1){
            //            oXslDom.async = false;
            //            oXslDom.load(urlXsl);
            //        }else{
            var oXMLHttp = CreateXmlHttpRequest();
            oXMLHttp.open("GET", urlXsl, false);
            oXMLHttp.send();
            //시간 늘리기
            delay(600);
            if (oXMLHttp.status == 200) {
                var parser = new DOMParser();
                oXslDom = parser.parseFromString(oXMLHttp.responseText, "text/xml");
                //oXSL = oXMLHttp.responseText.substring(38,oXMLHttp.responseText.length) ;
            }
            //        }
            var oProcessor = new XSLTProcessor();
            oProcessor.importStylesheet(oXslDom);
            return oProcessor;
            //return oXMLHttp.responseText.replace("<![CDATA[", "&lt;![CDATA[").replace("]]>", "]]&gt;").replace('(iVal<10?"0"+iVal:iVal)','(iVal&lt;!10?"0"+iVal:iVal)').replace('for(var i=0; i < aDotCount.length-1; i++){','for(var i=0; i &lt;! aDotCount.length-1; i++){').replace('"<br>"','"&lt;!br&gt;"').replace('"<font color=\'white\'>-</font>"','"&lt;!font color=\'white\'&gt;-&lt;!/font&gt;"');
            // return oXMLHttp.responseText.replace("<![CDATA[", "@CDATASTART").replace("]]>", "@CDATAEND");
        }
    }

    // 리소스 테이블의 리소스 내용을 가져온다.
    var comm_xmlHttp = CreateXmlHttpRequest();
    var sReturnValue = "";

    GetResourceManager = function(sResourceCD, sServiceName) {
        var js_msg = null;

        try {
            js_msg = eval(sResourceCD);
        } catch (e) {
            js_msg = null;
        }

        if (typeof (js_msg) == "string") {
            return js_msg;
        } else {
            var sURL = fn_GetWebRoot() + "SiteReference/Common/GetResource.aspx?ResourceCD=" + sResourceCD + "&ServiceName=" + sServiceName;
            sReturnValue = "";
            comm_xmlHttp.open("GET", sURL, false);



            //if (_Browser == "FIREFOX") { // Utility.js 참조변수
            //    comm_xmlHttp.onload = comm_xmlHttp.onerror = comm_xmlHttp.onabort = listenXMLHTTP_GetResource;
            //} else {
                comm_xmlHttp.onreadystatechange = listenXMLHTTP_GetResource;
            //}

            comm_xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
            comm_xmlHttp.send(null);

            return sReturnValue;
        }

    }

    listenXMLHTTP_GetResource = function() {
        if (comm_xmlHttp.readyState == 4) {
            comm_xmlHttp.onreadystatechange = event_noop; //re-entrant gate

            if (comm_xmlHttp.responseText.charAt(0) == '\r') {
                alert("error in listenXMLHTTP(): no responseText returned");
            } else {
                var xmlNode = comm_xmlHttp.responseXML.selectSingleNode("//value");
                sReturnValue = xmlNode.text;
            }
        }
    }

    // 리소스 테이블의 리소스 내용을 가져온다.
    GetResource = function(sResourceCD) {
        var js_msg = null;

        try {
            js_msg = eval(sResourceCD);
        }
        catch (e) {
            js_msg = null;
        };

        if (typeof (js_msg) == "string") {
            return js_msg;
        }
        else {
            var sURL = fn_GetWebRoot() + "SiteReference/Common/GetResource.aspx?ResourceCD=" + sResourceCD;
            var sReturnValue = "";

            var comm_xmlHttp = CreateXmlHttpRequest();
            comm_xmlHttp.open("GET", sURL, false);
            comm_xmlHttp.send();

            if (comm_xmlHttp.responseXML.selectSingleNode("//error")) {
                alert("Error : " + comm_xmlHttp.responseXML.selectSingleNode("//error").text);
                return false;
            }

            var xmlNode = comm_xmlHttp.responseXML.selectSingleNode("//value");
            sReturnValue = xmlNode.text;

            return sReturnValue;
        }

    }

    DataGrid_OnMouseOutEventHandler = function(gridName, id, objectType) {
        if (objectType == 0) {
            var row = igtbl_getRowById(id);
            //row.Element.style.backgroundColor = "#FFFFFF";
            row.Element.className = "DataGridMouseOut"
        }
    }

    //조직도 오픈
    GetOrgMap2 = function(strEntcode, strSelectType, strSelectMulit, strType) {
        //strEntcode 조직도에 기본으로 보여질 회사코드,''이 올경우 기본 값이 입력되어진다.
        //strSelectType 선택구성원 타입 A:부서,개인 P:개인 U:부서 E:A + 회사,그룹
        //strSelectMulit 여러 구성원 선택 가능여부 Y:가능,N:불가능
        //strType 한페이지에서 조직도 여러번 사용할때 사용
        //오픈페이지에 OrgMap(oDomOrgMap,strType) 함수 만들어 줘야한다
        //결과값은 오픈페이지의 OrgMap function을 호출하여 oDomOrgMap에 들어간다 물론 Dom 형태임.
        /*
        var URL=fn_GetUserWebRoot()+"PortalService/OrgMap/OM_OrgMap.aspx?SelectType="+strSelectType+"&SelectMulit="+strSelectMulit;
        if(strEntcode !="")
        URL += "&Entcode="+strEntcode;

	var oDomOrgMap =  fn_OpenModalDialog(URL, "", "dialogWidth:870px; dialogHeight:600px");
        if(oDomOrgMap != null)
        return oDomOrgMap;
        else
        return null;
        */

        // 새로 작업한 조직도로 변경    
        try {
            action = "acl";
            var rgParams = null;
            rgParams = new Array();

            rgParams["bMail"] = false;
            rgParams["bUser"] = false;
            rgParams["bGroup"] = true;
            rgParams["bRef"] = false;
            //rgParams["objMessage"] = (bAdmin=="true")?myform.admin_id:myform.lstMember;
            rgParams["bMember"] = true;
            rgParams["bAdmin"] = false;

            var oStyle = window.document.body.currentStyle;
            var nFontHeight = 9;
            var szFont = "FONT:" + oStyle.fontFamily + ";font-size:" + nFontHeight + "px;";
            var nWidth = 860;
            var nHeight = 630;

            var pass;
            pass = "/CoviWeb/PortalService/Address/address.aspx?control=MsgTo&requestType=Poll" + "&strEntcode=" + strEntcode + "&strSelectType=" + strSelectType + "&strSelectMulit=" + strSelectMulit + "&strType=" + strType;
            var vRetval = window.showModalDialog(pass, rgParams, szFont + "dialogHeight:" + nHeight + "px;dialogWidth:" + nWidth + "px;status:no;resizable:yes;help:no;scroll=yes");
            //window.open (pass, rgParams, szFont+"dialogHeight:"+nHeight+"px;dialogWidth:"+nWidth+"px;status:no;resizable:yes;help:no;scroll=no");	

            //2008.4.28 김시은 - 스키마 변경 -
            //스키마 변경!!! 기존 로레알 버전을 WorkPlace2.0 환경에 맞추기 위해 데이터를 변경해서 리턴해줌
            //불필요한 소스 수정을 줄이기 위해 org_memberquery.xsl의 Element를 수정하지 않고 아래와 같이 변경함.
            //
            if (vRetval != null) {
                var m_objXML = CreateXmlDocument();
                var m_objXML2 = CreateXmlDocument();
                var m_oMemberXSLProcessor = makeProcessor(SiteURL + "/CoviWeb/PortalService/Address/schema.xsl");
                var oXml = vRetval.xml;
                m_objXML2.loadXML(oXml.replace("\r\n", "").replace("\t", ""));
                //m_oMemberXSLProcessor.input = vRetval;
                m_oMemberXSLProcessor.input = m_objXML2;
                m_oMemberXSLProcessor.transform();
                m_objXML.loadXML(m_oMemberXSLProcessor.output);
                return m_objXML;
            }
        }
        catch (e) {
            alert("오류가 발생했습니다. 오류 내용은 다음과 같습니다. \r\nError File : addmember(bAdmin) in Survey/Survey_New.aspx\r\nError Description: " + e.description);
        }
    }


    //임직원현황 검색관련 조직도-----
    GetOrgMap3 = function(strEntcode, strSelectType, strSelectMulit, strType, sSearchType, sSearchWord, STYPE) {
        //strEntcode 조직도에 기본으로 보여질 회사코드,''이 올경우 기본 값이 입력되어진다.
        //strSelectType 선택구성원 타입 A:부서,개인 P:개인 U:부서 E:A + 회사,그룹
        //strSelectMulit 여러 구성원 선택 가능여부 Y:가능,N:불가능
        //strType 한페이지에서 조직도 여러번 사용할때 사용
        //오픈페이지에 OrgMap(oDomOrgMap,strType) 함수 만들어 줘야한다
        //결과값은 오픈페이지의 OrgMap function을 호출하여 oDomOrgMap에 들어간다 물론 Dom 형태임.
        /*
        var URL=fn_GetUserWebRoot()+"PortalService/OrgMap/OM_OrgMap.aspx?SelectType="+strSelectType+"&SelectMulit="+strSelectMulit;
        if(strEntcode !="")
        URL += "&Entcode="+strEntcode;

	var oDomOrgMap =  fn_OpenModalDialog(URL, "", "dialogWidth:870px; dialogHeight:600px");
        if(oDomOrgMap != null)
        return oDomOrgMap;
        else
        return null;
        */

        //try
        //{
        action = "acl";
        var rgParams = null;
        rgParams = new Array();

        rgParams["bMail"] = false;
        rgParams["bUser"] = false;
        rgParams["bGroup"] = true;
        rgParams["bRef"] = false;
        //rgParams["objMessage"] = (bAdmin=="true")?myform.admin_id:myform.lstMember;
        rgParams["bMember"] = true;
        rgParams["bAdmin"] = false;

        var oFontFamily = "";
        if (window.getComputedStyle) {
            oFontFamily = window.getComputedStyle(window.document.body, null).fontFamily;
        }
        else if (window.document.body.currentStyle) {
            oFontFamily = window.document.body.currentStyle.fontFamily;
        }


        var nFontHeight = 9;
        var szFont = "FONT:" + oFontFamily + ";font-size:" + nFontHeight + "px;";

        if (STYPE != "" && STYPE != undefined) // 설문조사 
        {
            var nWidth = 860;
        }
        else {
            var nWidth = 740;
        }
        var nHeight = 690;
        var options = 'width=' + nWidth;
        options += ' ,height=' + nHeight;
        options += ' ,left=' + (screen.availWidth - nWidth) / 2;
        options += ' ,top=' + (screen.availHeight - nHeight) / 2;
        options += ' ,scrollbars=yes';
        options += ' ,titlebar=no';
        options += ' ,resizable=no';
        options += ' ,Status=no';
        options += ' ,toolbar=no';
        
        var pass;

        if (STYPE != "" && STYPE != undefined) // 설문조사 
        {
            pass = "/CoviWeb/PortalService/Address/address.aspx?control=MsgTo&requestType=Poll" + "&strEntcode=" + strEntcode + "&strSelectType=" + strSelectType + "&strSelectMulit=" + strSelectMulit + "&strType=" + strType + "&strSearchType=" + sSearchType + "&strSearchWord=" + sSearchWord;
        }
        else {
            pass = "/CoviWeb/PortalService/Address/AddressSearch.aspx?control=MsgTo&requestType=Poll" + "&strEntcode=" + strEntcode + "&strSelectType=" + strSelectType + "&strSelectMulit=" + strSelectMulit + "&strType=" + strType + "&strSearchType=" + sSearchType + "&strSearchWord=" + sSearchWord;
        }

        var vRetval = null;
        if (window.showModelessDialog) {
            vRetval = window.showModalDialog(pass, rgParams, szFont + "dialogHeight:" + nHeight + "px;dialogWidth:" + nWidth + "px;status:no;resizable:yes;help:no;scroll=yes");
        }
        else {
            vRetval = window.open(pass, window, options);
        }

        //window.open(pass, "", options);	

        //2008.4.28 김시은 - 스키마 변경 -
        //스키마 변경!!! 기존 로레알 버전을 WorkPlace2.0 환경에 맞추기 위해 데이터를 변경해서 리턴해줌
        //불필요한 소스 수정을 줄이기 위해 org_memberquery.xsl의 Element를 수정하지 않고 아래와 같이 변경함.
        //
        if (vRetval != null) {
            var m_objXML = CreateXmlDocument();
            var m_objXML2 = CreateXmlDocument();
            var m_oMemberXSLProcessor = makeProcessor(SiteURL + "/CoviWeb/PortalService/Address/schema.xsl");
            var oXml = vRetval.xml;
            m_objXML2.loadXML(oXml.replace("\r\n", "").replace("\t", ""));
            //m_oMemberXSLProcessor.input = vRetval;
            m_oMemberXSLProcessor.input = m_objXML2;
            m_oMemberXSLProcessor.transform();
            m_objXML.loadXML(m_oMemberXSLProcessor.output);

            if (STYPE != "" && STYPE != undefined) // 설문조사 
                OrgMap(m_objXML, STYPE);
            else
                return m_objXML;
        }
        //window.open(pass, rgParams, options);	
        //}
        //catch(e)
        //{
        //	alert("오류가 발생했습니다. 오류 내용은 다음과 같습니다. \r\nError File : addmember(bAdmin) in Survey/Survey_New.aspx\r\nError Description: " + e.description);
        //}   
    }

    DataGrid_OnMouseOverEventHandler = function(gridName, id, objectType) {
        if (objectType == 0) {
            var row = igtbl_getRowById(id);
            //row.Element.style.backgroundColor = "#EFF2F7";
            row.Element.className = "DataGridMouseOver"
        }
    }

    // 색상선택 팝업 화면을 호출한다
    ColorPickerOpen = function() {
        var retColor = "";
        try {
            /* AOP 그룹웨어 Covision */
            retColor = fn_OpenDialog2(fn_GetWebRoot() + "SiteReference/Common/ColorPicker.htm", "", "370px", "300px", "scroll:no;status:no;resizable:yes;");
            //retColor = fn_OpenModalDialog(fn_GetWebRoot() + "SiteReference/Common/ColorPicker.htm", "", "dialogWidth:370px;dialogHeight:300px;toolbar=no;menubar=no;status=no;scrollbars=no;resizable=yes");

            if (retColor != null) {
                return retColor;
            }
        }
        catch (exception) {
            alert(exception);
            return false;
        }
    }

    /* HTML 에디터 ActiveX에서의 작성 여부 확인 */
    fn_HtmlTextCheck = function(htmlText) {
        htmlText = htmlText.replace(/&nbsp;/g, "");

        var s_idx = htmlText.indexOf("<BODY ");
        var e_idx = htmlText.lastIndexOf("</BODY>");

        s_idx = (htmlText.indexOf(">") + 1);

        htmlText = htmlText.substring(s_idx, e_idx);

        s_idx = (htmlText.indexOf("<P") + 2);

        htmlText = htmlText.substring(s_idx, e_idx);

        s_idx = (htmlText.indexOf(">") + 1);
        e_idx = htmlText.lastIndexOf("</P>");

        htmlText = htmlText.substring(s_idx, e_idx);

        if (htmlText.length < 1)
            return false;

        return true;

    };

    GetFolderSelect = function () {
        /* AOP 그룹웨어 Covision */
        return fn_OpenDialog2(fn_GetWebRoot() + "BaseService/Board/BD_FolderSelect.aspx", "", "325px", "245px", "scroll:no;status:no;resizable:yes;");

        //return fn_OpenModalDialog(fn_GetWebRoot() + "BaseService/Board/BD_FolderSelect.aspx", "BoardFolderSelect", "dialogWidth:325px;dialogHeight:245px;scroll=no;toolbar=no;menubar=no;status=no;resizable=no;");
    }

    /*=======================================================================
    Function명 : GetEDMS(strDocumentID,strVersionID,strFolderID,strAclID,strPAclID)
    내용 : 통합검색에서 문서관리결과를 클릭하면 권한 체크후 문서를 보여준다.
    작  성  자  :
    최초작성일  :  2007.06.14
    ========================================================================*/
    GetEDMS = function(strDocumentID, strVersionID, strFolderID, strAclID, strPAclID) {
        var strPath = fn_GetWebRoot() + "ExtensionService/Doc/DM_Document_ACLCheck.aspx?ACLID=" + strAclID + "&PACLID=" + strPAclID + "&DocumentID=" + strDocumentID;
        var oxml = new ActiveXObject("Microsoft.XMLHTTP");

        oxml.open("Get", strPath, false, "", "");
        oxml.send();



        var oData = oxml.ResponseText;

        if (oData.ResponseText == "OK") {
            //문서조회670, 490
            var DocPath = fn_GetWebRoot() + "ExtensionService/Doc/DM_Document_View.aspx?Privilege=DVI&FolderID=" + strFolderID + "&VersionID=" + versionID + "&DocumentID=" + DocumentID;
            fn_OpenDialog(DocPath, "DocView", "toolbar=0, location=0, directories=0, status=0, menubar=0, scrollbars=0, resizable=0, copyhistory=0, width=670,height=490");
        }
        else {
            //권한 없음
            alert('<%= Resources.CM_Msg.msg_2%>');
        }
    }


    /*
    롤 오버 처리 스크립트...
    */
    MM_preloadImages = function() { //v3.0
        var d = document; if (d.images) {
            if (!d.MM_p) d.MM_p = new Array();
            var i, j = d.MM_p.length, a = MM_preloadImages.arguments; for (i = 0; i < a.length; i++)
                if (a[i].indexOf("#") != 0) { d.MM_p[j] = new Image; d.MM_p[j++].src = a[i]; } 
        }
    }

    MM_swapImgRestore = function() { //v3.0
        var i, x, a = document.MM_sr; for (i = 0; a && i < a.length && (x = a[i]) && x.oSrc; i++) x.src = x.oSrc;
    }

    MM_findObj = function(n, d) { //v4.01
        var p, i, x; if (!d) d = document; if ((p = n.indexOf("?")) > 0 && parent.frames.length) {
            d = parent.frames[n.substring(p + 1)].document; n = n.substring(0, p);
        }
        if (!(x = d[n]) && d.all) x = d.all[n]; for (i = 0; !x && i < d.forms.length; i++) x = d.forms[i][n];
        for (i = 0; !x && d.layers && i < d.layers.length; i++) x = MM_findObj(n, d.layers[i].document);
        if (!x && d.getElementById) x = d.getElementById(n); return x;
    }

    MM_swapImage = function() { //v3.0
        var i, j = 0, x, a = MM_swapImage.arguments; document.MM_sr = new Array; for (i = 0; i < (a.length - 2); i += 3)
            if ((x = MM_findObj(a[i])) != null) { document.MM_sr[j++] = x; if (!x.oSrc) x.oSrc = x.src; x.src = a[i + 2]; }
    }

    fn_GetStrByteLength = function(strVal) {
        return fn_StrByteLengthEx(strVal);
    }

    fn_StrByteLength = function(strVal) {// 리턴 문자가 2Byte(13+10) 계산됨...
        var strLen = 0;

        for (var i = 0; i < strVal.length; i++) {
            var chrCode = strVal.charCodeAt(i);

            strLen++;

            if (chrCode > 255)
                strLen++;
        }

        return strLen;
    }

    fn_StrByteLengthEx = function(strVal) {// 2Byte(13+10)의 리턴 문자에서 1바이트(10)는 계산에서 제외...
        var strLen = 0;

        for (var i = 0; i < strVal.length; i++) {
            var chrCode = strVal.charCodeAt(i);

            if (chrCode == 10) // 무시하기 위한 바이트값
                continue;

            strLen++;

            if (chrCode > 255)
                strLen++;
        }

        return strLen;
    }

    /*******************************
    사용자 이름에 마우스 올렸을때 와 내렸을때 처리
    *********************************/
    fnCoviNameContextMenuMenuOver = function(obj) {
        obj.children[0].setAttribute("src", "/GWImages/Common/icon/icon_writer_on.gif");
        //obj.children[0].style.display = '';
    }
    fnCoviNameContextMenuMenuOut = function(obj) {
        obj.children[0].setAttribute("src", "/GWImages/Common/icon/icon_writer_off.gif");
        //obj.children[0].style.display = 'none';
    }


    /*********************************************************************************************
    개인정보 비밀번호 변경 관련 스크립트 시작
    ***********************************************************************************************/

    fn_ChkUser = function() {
        var oPwd = document.getElementById("TxtOldPwd").value;
    }

    fn_ChangePwd = function(Mode) {

        var sResult = fn_ChkNewPwd();

        if (sResult == "true") {
            void_GetUserInfo(Mode);
        }
        else {
            alert(sResult);
            document.getElementById("TxtChkPwd").select();
        }
    }

    var chkLength = 5; // 비밀번호 자릿수

    fn_ChkNewPwd = function() {
        var oldPwd = document.getElementById("TxtOldPwd").value;
        var newPwd = document.getElementById("TxtNewPwd").value;
        var chkPwd = document.getElementById("TxtChkPwd").value;
        var chkResult = "true";
        var chkSum = 0;

        if (oldPwd != "" || newPwd != "" || chkPwd != "") {
            if (newPwd != chkPwd) {
                chkResult = "변경할 암호가 동일하지 않습니다.\r\n\r\n다시 입력해주세요.";
            }

            if (chkPwd.replace(/ /g, "").length <= chkLength) {
                chkResult = "비밀번호는 " + (chkLength + 1) + " 자리 이상 입력해주세요.";
            }
            else {
                chkSum = fn_ChkRegPwd(chkPwd);

                if (chkSum >= 3) {
                    chkResult = "true";
                }
                else {
                    chkResult = "암호가 암호 정책 요구 사항에 맞지 않습니다. 암호 복잡도 및 암호 기록 요구 사항을 확인하십시오.";
                }
            }
        }

        else {
            chkResult = "모든 정보를 입력해주세요.";
        }

        return chkResult;
    }

    fn_ChkRegPwd = function(Pwd) {
        var chkArr = new Array();

        var chkNum1 = 0;
        var chkNum2 = 0;
        var chkNum3 = 0;
        var chkNum4 = 0;

        var chkSum = 0;

        // 정규식 패턴 //
        var regExp1 = /^[a-z]/;
        var regExp2 = /^[A-Z]/;
        var regExp3 = /^[0-9]/;
        var regExp4 = /[^0-9a-zA-Zㄱ-ㅎㅏ-ㅣ가-힣]/;
        // 정규식 패턴 //

        for (var i = 0; i < Pwd.replace(/ /g, "").length; i++) {
            chkArr[i] = Pwd.charAt(i);
            if (regExp1.test(chkArr[i])) {
                chkNum1 = 1;
                continue;
            }
            else if (regExp2.test(chkArr[i])) {
                chkNum2 = 1;
                continue;
            }
            else if (regExp3.test(chkArr[i])) {
                chkNum3 = 1;
                continue;
            }
            else if (regExp4.test(chkArr[i])) {
                chkNum4 = 1;
                continue;
            }
        }
        chkSum = chkNum1 + chkNum2 + chkNum3 + chkNum4;
        return chkSum;
    }

    //var m_evalXML = new ActiveXObject("MSXML2.DOMDocument");
    //var m_xmlHTTP = new ActiveXObject("MSXML2.XMLHTTP");	
    //var m_xmlDom  = new ActiveXObject("MSXML2.DOMDocument");
    //var m_xslProcessor = new ActiveXObject("MSXML2.FreeThreadedDOMDocument");
    //var m_oReceive = new ActiveXObject("MSXML2.DOMDocument");
    //var m_oAdd = new ActiveXObject("MSXML2.DOMDocument");

    /*이준희(2010-10-07): Moved the variables' location to the top of this file.
    var m_evalXML = CreateXmlDocument();//alert(document.location.href + ': m_evalXML');
    var m_xmlHTTP = CreateXmlHttpRequest();	
    var m_xmlDom  = CreateXmlDocument();
    var m_xslProcessor = CreateXmlDocument();
    var m_oReceive = CreateXmlDocument();
    var m_oAdd = CreateXmlDocument();
    */

    evalXML = function(sXML) {
        if (!m_evalXML.loadXML(sXML)) {
            var err = m_evalXML.parseError;
            throw new Error(err.errorCode, "desc:" + err.reason + "\nsrctxt:" + err.srcText + "\nline:" + err.line + "\tcolumn:" + err.linepos);
        }
    }

    requestHTTP = function(sMethod, sUrl, bAsync, sCType, pCallback, vFileName) {
        m_xmlHTTP.open(sMethod, sUrl, bAsync);
        m_xmlHTTP.setRequestHeader("Accept-Language", "ko");
        m_xmlHTTP.setRequestHeader("Content-type", sCType);
        if (pCallback != null) m_xmlHTTP.onreadystatechange = pCallback;
        (vFileName != null) ? m_xmlHTTP.send(vFileName) : m_xmlHTTP.send();
    }

    //shj 2015-01-30 : 한페이지에서 두개의 웹서비스 호출 시 사용
    requestHTTP2 = function (sMethod, sUrl, bAsync, sCType, pCallback, vFileName) {
        m_xmlHTTP2.open(sMethod, sUrl, bAsync);
        m_xmlHTTP2.setRequestHeader("Accept-Language", "ko");
        m_xmlHTTP2.setRequestHeader("Content-type", sCType);
        if (pCallback != null) m_xmlHTTP2.onreadystatechange = pCallback;
        (vFileName != null) ? m_xmlHTTP2.send(vFileName) : m_xmlHTTP2.send();
    }

    makeXMLDomDocument = function(strText) {
        m_xmlDom.async = false;
        if (!m_xmlDom.loadXML(strText)) {
            alertParseError(m_xmlDom.parseError);
            throw new Error(-1, "couldn't make XMLDomDocument.");
        }
    }

    alertParseError = function(err) {
        alert("Error. desc:" + err.reason + "\nsrcurl:" + err.url + "\nsrctxt:" + err.srcText + "\nline:" + err.line + "\tcolumn:" + err.linepos);
    }

    event_noop = function() { return (false); }


    SetParams = function(params, values) {
        var sText = "";

        try {
            sText = "<Parameters>";
            for (var i = 0; i < params.length; i++) {
                sText += "<" + params[i] + ">" + values[i] + "</" + params[i] + ">";
            }
            sText += "</Parameters>";

            evalXML(sText);
            //alert(sText);

            var szurl = "../OrgList/Request.aspx?Mode=" + document.all.HdnMODE.value;

            if (document.all.HdnMODE.value == "R")
                requestHTTP("POST", szurl, false, "text/xml", receiveTree, sText);
            else if (document.all.HdnMODE.value == "ddr")
                requestHTTP("POST", szurl, false, "text/xml", receiveDdrInfo, sText);
            else if (document.all.HdnMODE.value == "IH" || document.all.HdnMODE.value == "IP")
                requestHTTP("POST", szurl, false, "text/xml", receiveResult, sText);
            else if (document.all.HdnMODE.value == "WP")
                requestHTTP("POST", szurl, false, "text/xml", receiveGroupTree, sText);
            else
                requestHTTP("POST", szurl, false, "text/xml", receivePersonInfo, sText);
        }
        catch (e) {
            alert(e.description)
        }
    }

    receiveDdrInfo = function() {
        if (m_xmlHTTP.readyState == 4) {
            m_xmlHTTP.onreadystatechange = event_noop;
            if (m_xmlHTTP.responseText.charAt(0) == '\r') {
                alert(m_xmlHTTP.responseText);
            } else {
                makeXMLDomDocument(m_xmlHTTP.responseText);
                var errorNode = m_xmlDom.selectSingleNode("Response/Root/Error");
                if (errorNode != null) {
                    alert("Desc: " + errorNode.text);
                } else {
                    try {
                        //alert(m_xmlHTTP.responseXML.xml);
                        var HdnTeamData = "";
                        listCount = m_xmlDom.selectNodes("//ROW").length;

                        for (var i = 0; i < listCount; i++) {
                            HdnTeamData += m_xmlDom.selectNodes("//ROW/TEAM_ID")[i].text + "|";
                            HdnTeamData += m_xmlDom.selectNodes("//ROW/NAME")[i].text + "^";
                        }

                        fn_DDRTeamBind(HdnTeamData);
                    }
                    catch (e) {
                        alert(e.description);
                    }
                }
            }
        }
    }

    receiveResult = function() {
        if (m_xmlHTTP.readyState == 4) {
            m_xmlHTTP.onreadystatechange = event_noop;
            if (m_xmlHTTP.responseText.charAt(0) == '\r') {
                alert(m_xmlHTTP.responseText);
            } else {
                makeXMLDomDocument(m_xmlHTTP.responseText);
                var errorNode = m_xmlDom.selectSingleNode("Response/Root/Error");
                if (errorNode != null) {
                    alert("Desc: " + errorNode.text);
                } else {
                    try {
                        //alert(m_xmlHTTP.responseXML.xml);
                        //alert("등록하였습니다.");
                    }
                    catch (e) {
                        alert(e.description);
                    }
                }
            }
        }
    }


    receiveGroupTree = function() {
        if (m_xmlHTTP.readyState == 4) {
            m_xmlHTTP.onreadystatechange = event_noop;
            if (m_xmlHTTP.responseText.charAt(0) == '\r') {
                alert(m_xmlHTTP.responseText);
            } else {
                makeXMLDomDocument(m_xmlHTTP.responseText);
                var errorNode = m_xmlDom.selectSingleNode("Response/Root/Error");
                if (errorNode != null) {
                    alert("Desc: " + errorNode.text);
                } else {
                    try {
                        //alert(m_xmlHTTP.responseXML.xml);
                        var HdnGrpData = "";
                        listCount = m_xmlDom.selectNodes("//ROW").length;

                        for (var i = 0; i < listCount; i++) {
                            HdnGrpData += m_xmlDom.selectNodes("//ROW/UNIT_CODE")[i].text + "|";
                            HdnGrpData += m_xmlDom.selectNodes("//ROW/NAME")[i].text + "|";
                            HdnGrpData += m_xmlDom.selectNodes("//ROW/SUBCOUNT")[i].text + "^";
                        }

                        fn_MakeGrpTree(HdnGrpData);
                    }
                    catch (e) {
                        alert(e.description);
                    }
                }
            }
        }
    }

    //================================================================================================================
    //  메소드
    //================================================================================================================

    /// <summary>
    /// Behind/AliasChange_Behind.aspx 페이지를 호출하여
    /// 검색된 XML 결과를 void_GetUserInfo_After 함수를 호출하여 현재 접속한 사용자의 이름 및 대화명을 조회합니다.
    /// </summary>
    void_GetUserInfo = function(Mode) {
        try {
            var sEmpNo = document.getElementById("hTxtUserId").value;
            var sOldPwd = document.getElementById("TxtOldPwd").value;
            var sNewPwd = document.getElementById("TxtNewPwd").value;
            sMode = Mode;

            if (sMode == "C")
                sMode = "CHANGEPWD";
            else
                sMode = "GETUSERINFO";

            var sXML = "<Parameters>";
            sXML += "<Mode><![CDATA[" + sMode + "]]></Mode>";
            sXML += "<EmpNo><![CDATA[" + sEmpNo + "]]></EmpNo>";
            sXML += "<OldPwd><![CDATA[" + sOldPwd + "]]></OldPwd>";
            sXML += "<NewPwd><![CDATA[" + sNewPwd + "]]></NewPwd>";
            sXML += "</Parameters>";

            evalXML(sXML);

            var szurl = "Behind/Ad_PwdChange_Behind.aspx";

            requestHTTP("POST", szurl, false, "text/xml", void_GetUserInfo_After, sXML);
        }
        catch (ex) {
            this.wObjCommon_AlertMsg.AlertMessage("AliasChange.js", "void_GetUserInfo", ex);
        }
    }

    /// <summary>
    /// void_GetUserInfo 함수에 의해 호출되며, 현재 접속한 사용자의 이름 및 대화명을 화면에 표시합니다.
    /// </summary>
    void_GetUserInfo_After = function() {
        try {
            if (m_xmlHTTP.readyState == 4) {
                m_xmlHTTP.onreadystatechange = event_noop;
                if (m_xmlHTTP.responseText.charAt(0) == '\r') {
                    alert(m_xmlHTTP.responseText);
                }
                else {
                    makeXMLDomDocument(m_xmlHTTP.responseText);
                    var sResult = "";
                    //var errorNode = m_xmlDom.selectSingleNode("Response/Root/Error");

                    if (sMode == "GETUSERINFO") {
                        //alert(m_xmlHTTP.responseText);
                        sResult = m_xmlDom.selectSingleNode("Response/Name").text;
                        document.getElementById("LbResult").innerText = "Name : " + sResult;
                    }
                    else {
                        //alert(m_xmlHTTP.responseText);
                        sResult = m_xmlDom.selectSingleNode("Response/Name").text;

                        if (sResult == "S") {
                            alert("암호가 변경되었습니다.");
                            // 암호 변경 시 변경된 암호로 메일 인증을 받는다.
                            LoginExchanged_Basic("http://mail.covision.co.kr/mail/auth.aspx", document.getElementById("hTxtUserId").value, document.getElementById("TxtNewPwd").value);
                            location.reload();
                        }
                        else {
                            alert(sResult);
                            //alert("기존 비밀번호가 일치하지 않습니다.\r\n\r\n다시 입력해주세요.");
                            document.getElementById("TxtOldPwd").select();
                            return;
                        }
                    }
                }
            }
        }
        catch (ex) {
            this.wObjCommon_AlertMsg.AlertMessage("AliasChange.js", "void_GetUserInfo_After", ex);
        }
    }

    /*********************************************************************************************
    개인정보 비밀번호 변경 관련 스크립트 끝
    ***********************************************************************************************/

    /*********************************************************************************************
    메일 인증 - 기본인증
    ***********************************************************************************************/
    LoginExchanged_Basic = function(g_sExchURI, userid, password) {
        //////////////////////// 메일 로그인 처리///////////////////////
        //var g_sExchURI = '<%= System.Configuration.ConfigurationManager.AppSettings["ExchangeServerUrl"].ToString() %>' + '/mail/auth.aspx';
        var strUserName = userid;
        var strUserPWD = password;
        var g_szAcceptLang = "ko";

        var boolRet = true;

        try {
            var oHTTP = CreateXmlHttpRequest();

            oHTTP.open("GET", g_sExchURI, false, strUserName, strUserPWD);
            //oHTTP.setRequestHeader("Accept-Language:", "ko");
            oHTTP.setRequestHeader("Content-type:", "text/xml");
            oHTTP.send();

            if (oHTTP.status == 200) {
                boolRet = true;
            }
            else {
                alert("메일인증 실패");
                boolRet = false;
            }
        }
        catch (e) {
            //로긴임시수정
            //alert("OWA시스템 인증 오류가 발생하였습니다. 관리자에게 문의하십시오" + "\n" + e.description);
            boolRet = true;
        }

        return boolRet;

    }

    fn_CheckDate = function(form, Mode) {
        var inputDate = form.value;
        var todayDate = new Date();
        var sYear = todayDate.getYear(); // 년도 가져오기 
        var sMonth = todayDate.getMonth(); // 월 가져오기 (+1) 
        var sDate = todayDate.getDate(); // 날짜 가져오기 

        if (sMonth + 1 >= 10)
            sMonth = sMonth + 1;
        else
            sMonth = "0" + (sMonth + 1);

        if (sDate < 10)
            sDate = "0" + sDate;


        var toDay = sYear + "-" + sMonth + "-" + sDate;

        var sAlert = "";
        var flag = true;
        if (inputDate < toDay && inputDate != "") {
            switch (Mode) {
                case "S":
                    sAlert = "시작일이 잘못되었습니다.";
                    flag = false;
                    break;
                case "E":
                    sAlert = "종료일이 잘못되었습니다.";
                    toDay = addDay(toDay, 'd', 7);
                    flag = false;
                    break;
                case "EE":
                    sAlert = "만료일이 잘못되었습니다.";
                    toDay = addDay(toDay, 'm', 3);
                    flag = false;
                    break;
            }
        }

        if (!flag) {
            alert(sAlert + "\r\n" + "날짜를 다시 입력해주세요.");
            form.value = toDay;
            return;
        }

        addDay = function(ymd, mode, v_day) {
            var yyyy = ymd.substr(0, 4);
            var mm = eval(ymd.substr(5, 2) + "- 1");
            var dd = ymd.substr(8, 2);

            if (mode == "d")
                var dt3 = new Date(yyyy, mm, eval(dd + '+' + v_day));
            else if (mode == "m")
                var dt3 = new Date(yyyy, eval(mm + '+' + v_day), dd);
            else if (mode == "y")
                var dt3 = new Date(eval(yyyy + '+' + v_day), mm, dd);

            yyyy = dt3.getYear();
            mm = (dt3.getMonth() + 1) < 10 ? "0" + (dt3.getMonth() + 1) : (dt3.getMonth() + 1);
            dd = dt3.getDate() < 10 ? "0" + dt3.getDate() : dt3.getDate();

            var addDate = yyyy + "-" + mm + "-" + dd;
            return addDate;
        }
    }
    delay = function(gap) {/*gap is in milisecs*/
        var then, now;
        then = new Date().getTime();
        now = then;
        while ((now - then) < gap) {
            now = new Date().getTime();
        }
    }

    //2010.05.18 browser 환경변수 설정
    /*이준희(2010-10-07): Moved the variables' location to the top of this file.
    var _Browser = "";
    var _BVersion = null;
    */

    GetBrowserInfo = function() {
        if (navigator.appName.indexOf("Microsoft") != -1) {
            _Browser = "IE";
        } else if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
            _Browser = "CHROME";
        } else if (navigator.userAgent.indexOf('Opera') != -1) {
            _Browser = "OPERA";
            _BVersion = parseFloat(navigator.userAgent.substr(navigator.userAgent.indexOf('Opera') + 6, 3));
        } else if (navigator.userAgent.indexOf('Macintosh') != -1) {
            _Browser = "MAC";
        } else if ((navigator.userAgent.indexOf('Safari') != -1) || (navigator.userAgent.indexOf('Konqueror') != -1)) {
            var _KHTMLrv = parseFloat(navigator.userAgent.substr(navigator.userAgent.indexOf('Safari') + 7, 5));

            if (_KHTMLrv > 525) {
                _Browser = "FIREFOX";
                _BVersion = 1.9;
            } else {
                _Browser = "KHTML";
            }
        } else {
            _Browser = "FIREFOX";
            _BVersion = parseFloat(navigator.userAgent.split("rv:")[1]);
        }
    }

    GetBrowserInfo();

    Utility_addEvent = function(pStrEvent, pObjObject) {
        if (typeof window.addEventListener != 'undefined') {
            pStrEvent = ChangeEventMethod(pStrEvent);
            window.addEventListener(pStrEvent, pObjObject, false);
        } else if (typeof document.addEventListener != 'undefined') {
            pStrEvent = ChangeEventMethod(pStrEvent);
            document.addEventListener(pStrEvent, pObjObject, false);
        } else if (typeof window.attachEvent != 'undefined') {
            window.attachEvent(pStrEvent, pObjObject);
        } else {
            window.alert('Failed to attach an Event Listener\n\n Please Report');
        }
    }

    ChangeEventMethod = function(pStrEvent) {
        var strChange;

        switch (pStrEvent) {
            case "onmousemove":
                strChange = "mousemove";
                break;
            case "onmousedown":
                strChange = "mousedown";
                break;
            case "ondblclick":
                strChange = "dblclick";
                break;
            case "onkeydown":
                strChange = "keydown";
                break;
            default:
                strChange = pStrEvent;
                break;
        }

        return strChange;
    }

    Utility_AddNewStyleRule = function(pObjStyleSheets, pObject, pStrRule) {
        if (pObjStyleSheets.addRule) {        //Internet Explorer
            pObjStyleSheets.addRule(pObject, pStrRule, 0);
        }
        else {
            if (pObjStyleSheets.insertRule) {        // Firefox, Safari, Opera
                pObjStyleSheets.insertRule(pObject + " {" + pStrRule + "}", 0);
            }
        }
    }

    //다국어처리
    try {//이준희(2010-10-05): Added a singleton handler to prevent duplicated definition.
        getLngLabel.toString();
    }
    catch (e) {
        getLngLabel = function(szLngLabel, szType, szSplit) {
            var rtnValue = "";
            var idxlng = gLngIdx;
            if (szType) { idxlng++; }
            var ary = szLngLabel.split(";");
            if (szSplit) {
                ary = szLngLabel.split(szSplit);
            }
            if (ary.length > idxlng) {
                rtnValue = ary[idxlng];
            }
            else {
                if (szType) {
                    rtnValue = ary[1];
                }
                else {
                    rtnValue = ary[0];
                }
            }
            return rtnValue;
        }
    }
    /*********************************************************************************************
    Modal Change 공통함수 추가 시작 - 2019.10.10
    ***********************************************************************************************/

    urlParse = function (param) {

        var nUrl = decodeURI(window.location.search.substring(1));

        var pArray = nUrl.split("&");

        if (nUrl != "") {
            for (var i = 0; i < pArray.length; i++) {
                var value = pArray[i].split("=");
                if (value[0] == param) {
                    return (value[1])?value[1]:"";
                }
            }
        } else {
            return;
        }

    }

    makeUrl = function (nArray) {
        var pUrl;
        var keys = new Array();

        for (var key in nArray) {
            keys.push(key);
        }

        for (var i = 0; i < keys.length; i++) {
            if (i == 0) {
                pUrl = "?" + keys[i] + "=" + nArray[keys[i]];
            } else {
                pUrl += "&" + keys[i] + "=" + nArray[keys[i]];
            }
        }

        return pUrl;

    }

    urlParse_parent = function (param, pnt) {
      
        var nUrl = decodeURI(parent.location.search.substring(1));
        if (nUrl == "") {
            nUrl = decodeURI(parent.parent.location.search.substring(1));
        }
        var pArray = nUrl.split("&");

        if (nUrl != "") {
            for (var i = 0; i < pArray.length; i++) {
                var value = pArray[i].split("=");
                if (value[0] == param) {
                    return value[1];
                }
            }
        } else {
            return null;
        }

    }


    /*********************************************************************************************
    Modal Change 공통함수 추가 끝
    ***********************************************************************************************/


}



function GetBaseCode(codeGroup) {
    var sUrl;
    sUrl = "/CoviWeb/WinC/Common/service/WincService.asmx/GetBaseCode";

    $.ajax({
        type: "POST",
        url: sUrl,
        data: "{CodeGroup:\"" + codeGroup + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            return data.d;
        },
        error: function (request, status, error) {
            alert("code = " + request.status + " message = " + request.responseText + " error = " + error); // 실패 시 처리
        }
    });
}

function GetBaseConfig(configName) {
    var retVal;
    var sUrl;
    sUrl = "/CoviWeb/WinC/Common/service/WincService.asmx/Get_BaseConfig";
    $.ajax({
        type: "POST",
        url: sUrl,
        data: "{ConfigName:\"" + configName + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (data) {
            retVal = data.d;
        },
        error: function (request, status, error) {
            alert("code = " + request.status + " message = " + request.responseText + " error = " + error); // 실패 시 처리
        }
    });
    return retVal;
}

/* AOP 그룹웨어 Covision - 전자결재 이벤트 로그 */
function SetEventLog(menu, logType, url, folderId, keyword) {
    var sItems = "<request>";
    var sUrl;
    sUrl = "/CoviWeb/SiteReference/Common/SetEventLog.aspx";

    sItems += MakeNode_EventLog("menu", menu)
        + MakeNode_EventLog("logType", logType)
        + MakeNode_EventLog("url", url)
        + MakeNode_EventLog("folderId", folderId)
        + MakeNode_EventLog("keyword", keyword);

    sItems += "</request>";
    //alert(sUrl);
    requestHTTP("POST", sUrl, true, "text/xml; charset=utf-8", null, sItems);
}
function SetEventLog2(menu, logType, url, folderId, keyword, authInfo, approvedUserID, uniqueInfo, downInfo) {
    var sItems = "<request>";
    var sUrl;
    sUrl = "/CoviWeb/SiteReference/Common/SetEventLog.aspx";

    sItems += MakeNode_EventLog("menu", menu)
        + MakeNode_EventLog("logType", logType)
        + MakeNode_EventLog("url", url)
        + MakeNode_EventLog("folderId", folderId)
        + MakeNode_EventLog("keyword", keyword)
        + MakeNode_EventLog("authInfo", authInfo)
        + MakeNode_EventLog("approvedUserID", approvedUserID)
        + MakeNode_EventLog("uniqueInfo", uniqueInfo)
        + MakeNode_EventLog("downInfo", downInfo);
        // + MakeNode_EventLog("downFiles", downFiles);


    sItems += "</request>";
    //alert(sUrl);
    requestHTTP("POST", sUrl, true, "text/xml; charset=utf-8", null, sItems);
}

function MakeNode_EventLog(tag, value) {
    return "<" + tag + "><![CDATA[" + value + "]]></" + tag + ">";
}


/* WinC 필수 입력 필드에 대한 벨리데이션 체크 
 * 필수값 체크 태그 (textbox, textarea, checkbox, radio)
 * 필수값 기본 체크시 태그안에 꼭 삽입되어야 하는 속성 (Required='Required' title="오류제목" chkType="text")
 * Required='Required' : 태그안에 선언시 필수값 체크
 * title="오류제목" : alert창 띄울 시 오류제목 [오류제목] 필수항목 입니다.
 * chkType : 체크구분 (text, check, radio)
 * 태그사용예제)
 *  chkType : text
 *  <input name="new_owner_eng_name1" type="text" value="" id="mField" style="font-family:맑은 고딕;width:95%;" Required='Required' title="텍스트 박스" chkType="text" />
 *  <textarea name="new_shareholder_name" rows="4" cols="20" id="mField" style="font-family:맑은 고딕;width:96%;background-color:#E7F9FF;" Required='Required' title="텍스트 textarea" chkType="text"></textarea>
 *  chkType : check
 *  <div Required='Required' title="체크 박스" chkType="check" chkName="chk_custom">
 *      <input id="new_listed_1" type="checkbox" name="new_listed_1" />거래소 상장
 *      <input id="new_listed_2" type="checkbox" name="new_listed_2" />KosDaQ 상장
 *      <input id="new_listed_3" type="checkbox" name="new_listed_3" />국외 상장
 *  </div>
 *  chkType : radio
 *  <div Required='Required' title="라디오 버튼" chkType="radio" chkName="rdo_custom">
 *      <input id="new_is_fi_yn" type="radio" name="new_is_fi_yn" value="1" style="border:0;" />Yes
 *      <input id="new_is_fi_yn" type="radio" name="new_is_fi_yn" value="2" style="border:0;" />No
 *  </div>
 */
function CFN_ValidationRequiredCheck() {
    var l_ObjFirstTarget = null;
    var l_ObjTarget = null;
    var l_LableAlt = "Validation Check";
    var l_ObjID = "";
    var l_Message = "";
    var returnResult = true;
    $("[Required='Required']").each(function () {
        if ($(this).attr("chkType") == "check") { // 체크 박스
            if ($(this).children().length > 1) {
                var chkcnt = 0;
                if ($('input:checkbox[name="' + $(this).attr("chkName") + '"]').is(":checked")) {
                    chkcnt = 1;
                }

                if (chkcnt == 0) {
                    alert("[" + $(this).attr("title") + "] 필수항목 입니다.");
                    $(this).focus();
                    returnResult = false;
                    return false;
                }
            }
        } else if ($(this).attr("chkType") == "radio") { // 라디오 박스
            if ($(this).children().length > 1) {
                var rdocnt = 0;
                if ($('input:radio[name="' + $(this).attr("chkName") + '"]').is(":checked")) {
                    rdocnt = 1;
                }

                if (rdocnt == 0) {
                    alert("[" + $(this).attr("title") + "] 필수항목 입니다.");
                    $(this).focus();
                    returnResult = false;
                    return false;
                }
            }
        }  else { // 텍스트 박스
            if ($.trim($(this).val()) == "") {
                l_ObjTarget = $(this);
                if (l_ObjFirstTarget == null) {
                    l_ObjFirstTarget = $(this);
                }
                l_ObjID = $(this).attr("name");
                if ($(this).attr("title") != "") {
                    l_LableAlt = $(this).attr("title");
                } else if ($(this).prop("alt") != "") {
                    l_LableAlt = $(this).attr("alt");
                }

                alert("[" + l_LableAlt + "] 필수항목 입니다.");
            }

            if (l_ObjFirstTarget != null) {
                $(l_ObjFirstTarget).focus();
                returnResult = false;
                return false;
            } else {
                return true;
            }
        }
    });

    //220-03-02 정지현 추가
    //select box는 required소문자 사용
    $("[required='Required']").each(function () {
        if ($(this).attr("chkType") == "select") { // 셀렉트 박스
            if ($("select[name='" + $(this).attr("chkName") + "'] option:selected").val() == "") {
                alert("[" + $(this).attr("title") + "] 필수항목 입니다.");
                $(this).focus();
                returnResult = false;
                return false;
            }
        }
    });

    return returnResult;
}




/**
 * 
 * EM -> 매니저 이상 (Manager, Director, Partner)
 * PD Lead Partner -> 파트너 이상 (Partner)
 * EP -> 파트너 이상 (Partner)
 * LCSP -> 매니저 이상 (Manager, Director, Partner)
 * 공헌도평가 // SB(Section B : Partner만), SC(Section C : Director, Senior Manager, Manager)
 * @param {any} levelCode = M, P....(M=Manager, P=Partner),A(ALL)
 * @param {any} returnTarget = LCSP, EM, EP, ALL .....
 */
function userClassList_Pop(levelCode, returnTarget) {
    var nWidth = 750;
    var nHeight = 750;
    var pass = "/CoviWeb/WinC/Common/UserList.aspx?LEVEL_CODE=" + levelCode + "&RETURN_TARGET=" + returnTarget;
    var options = 'width=' + nWidth;
    options += ' ,height=' + nHeight;
    options += ' ,left=' + (screen.availWidth - nWidth) / 2;
    options += ' ,top=' + (screen.availHeight - nHeight) / 2;
    options += ' ,scrollbars=yes';
    options += ' ,titlebar=no';
    options += ' ,resizable=no';
    options += ' ,Status=no';
    options += ' ,toolbar=no';

    window.open(pass, "userClassListPop", options);

}


function userClassList_Select(personCode, engName, korName, unit, unitCode, func_code, returnTarget) {

    if ($(opener.document).find("#" + returnTarget + "_UserNameCode").length > 0) {
        $(opener.document).find("#" + returnTarget + "_UserNameCode").val(korName + "(" + personCode+")");
    }

    if (returnTarget == "ALL" || returnTarget == "ALL2") {
        opener.$("#" + returnTarget + "_UserDeptName").val(unit); // 부서, 소속
        opener.$("#" + returnTarget + "_UserCode").val(personCode); // 사번
        opener.$("#" + returnTarget + "_UserName").val(engName); // 영문이름
        opener.$("#" + returnTarget + "_UserNameKor").val(korName); // 한글이름
        opener.$("#" + returnTarget + "_UserJobTitle").val(unitCode); //직급
        opener.$("#" + returnTarget + "_UserTel").val(func_code); // 내선번호


        if ($(opener.document).find("#hidDraftUserCode").length > 0) {
            $(opener.document).find("#hidDraftUserCode").val(personCode);
        }

        self.close();
    }
    

    if (returnTarget.toLowerCase().indexOf('_orgwin') > -1) { //ex) pdList 검색
        var TargetID = returnTarget.split("_")[0];
        $("input[id*=" + TargetID + "_UserName]", opener.document).val(korName);
        if ($(opener.document).find("input[id *=" + TargetID + "_UserCode]").length > 0) {
            $("input[id*=" + TargetID + "_UserCode]", opener.document).val(personCode);
        }
    } else {
        if ($(opener.document).find("#" + returnTarget + "_UserName").length > 0) {
            $(opener.document).find("#" + returnTarget + "_UserName").val(engName);
        }
        if ($(opener.document).find("#" + returnTarget + "_UserCode").length > 0) {
            $(opener.document).find("#" + returnTarget + "_UserCode").val(personCode);
        }
        if ($(opener.document).find("#" + returnTarget + "_UserNameKor").length > 0) {
            $(opener.document).find("#" + returnTarget + "_UserNameKor").val(korName);
        }
        if ($(opener.document).find("#" + returnTarget + "_UserUnitName").length > 0) {
            $(opener.document).find("#" + returnTarget + "_UserUnitName").val(unit);
        }
        if ($(opener.document).find("#" + returnTarget + "_UserUnitCode").length > 0) {
            $(opener.document).find("#" + returnTarget + "_UserUnitCode").val(unitCode);
        }
        //
        if ($(opener.document).find("#" + returnTarget + "_UserFUNC_CODE").length > 0) {
            $(opener.document).find("#" + returnTarget + "_UserFUNC_CODE").val(func_code);
        }

        // 결재양식 C&I Check
        if (returnTarget == "txt_new_lcsp1") {
            if ($(opener.document).find("[name=hid_new_lcsp]").length > 0) {
                $(opener.document).find("[name=hid_new_lcsp]").val(korName + "(" + personCode + ")");
            }
            if ($(opener.document).find("[name=hid_new_lcspCode]").length > 0) {
                $(opener.document).find("[name=hid_new_lcspCode]").val(personCode);
            }
            if ($(opener.document).find("[name=wincApvLineCode]").length > 0) {
                $(opener.document).find("[name=wincApvLineCode]").val(""); // 코드가 변경될 때 결재선 재확인을 위해
            }
        }
        if (returnTarget == "txt_new_auditep1") {
            if ($(opener.document).find("[name=hid_new_auditep]").length > 0) {
                $(opener.document).find("[name=hid_new_auditep]").val(korName + "(" + personCode + ")");
            }
            if ($(opener.document).find("[name=hid_new_auditepCode]").length > 0) {
                $(opener.document).find("[name=hid_new_auditepCode]").val(personCode);
            }
            if ($(opener.document).find("[name=wincApvLineCode]").length > 0) {
                $(opener.document).find("[name=wincApvLineCode]").val(""); // 코드가 변경될 때 결재선 재확인을 위해
            }
            try {
                $(opener.document).find('[name=hid_new_auditeplog]').val('USER_SEL'); //hyjseo audit ep log 추가
            } catch (ex) {}
        }
       
        //결재양식 공헌도평가
        // 필수 Section
        if (returnTarget == "txt_E1_1") {
            opener.CalculatorTableE(1);
        }
        else if (returnTarget == "txt_E1_2") {
            opener.CalculatorTableE(2);
        }
        else if (returnTarget == "txt_E1_3") {
            opener.CalculatorTableE(3);
        }
        else if (returnTarget == "txt_E1_4") {
            opener.CalculatorTableE(4);
        }
        else if (returnTarget == "txt_E1_5") {
            opener.CalculatorTableE(5);
        }
        else if (returnTarget == "txt_E1_6") {
            opener.CalculatorTableE(6);
        }
        else if (returnTarget == "txt_E1_7") {
            opener.CalculatorTableE(7);
        }
        else if (returnTarget == "txt_E1_8") {
            opener.CalculatorTableE(8);
        }
        // 선택 Section
        else if (returnTarget == "txt_F1_1") {
            opener.CalculatorTableF(1);
        }
        else if (returnTarget == "txt_F1_2") {
            opener.CalculatorTableF(2);
        }
        else if (returnTarget == "txt_F1_3") {
            opener.CalculatorTableF(3);
        }
        else if (returnTarget == "txt_F1_4") {
            opener.CalculatorTableF(4);
        }
        else if (returnTarget == "txt_F1_5") {
            opener.CalculatorTableF(5);
        }
        else if (returnTarget == "txt_F1_6") {
            opener.CalculatorTableF(6);
        }
        else if (returnTarget == "txt_F1_7") {
            opener.CalculatorTableF(7);
        }
        else if (returnTarget == "txt_F1_8") {
            opener.CalculatorTableF(8);
        }
        // 삭제됨
        else if (returnTarget == "txt_G1_1") {
            opener.SetWeightTableG(1);
        }
        else if (returnTarget == "txt_G1_2") {
            opener.SetWeightTableG(2);
        }
        else if (returnTarget == "txt_G1_3") {
            opener.SetWeightTableG(3);
        }
        else if (returnTarget == "txt_G1_4") {
            opener.SetWeightTableG(4);
        }

        // My Network Bank Owner 정보
        if (returnTarget == "NBA_Owner") {
            opener.$("#ownerDept").html(unit); // 부서, 소속
            opener.$("#ownerID").val(personCode); // 사번
            opener.$("#ownerName").val(korName); // 한글이름
            opener.$("#ownerPosition").html(unitCode); //직급
        }

        // My Network Bank Supporter 정보
        if (returnTarget == "NBA_Supporter") {
            opener.$("#supporter").val(korName); // 한글이름
            opener.$("#hidSupporter_id").val(personCode); // 사번
        }

        // UserInfo - 한글명 과 사번만 리턴
        if (returnTarget.toLowerCase().indexOf('_orgnameid') > -1) { 
            var TargetID = returnTarget.split("_")[0];
            $("input[id*=" + TargetID + "_UserName]", opener.document).val(korName);
            $("input[id*=" + TargetID + "_UserID]", opener.document).val(personCode);
        }
    }
    
    self.close();
}


function baseCategoryPop(type) {
    var nWidth = 400;
    var nHeight = 550;

    //var pass = "/CoviWeb/WinC/Common/CategorySelect.aspx?CATEGORY_TYPE=" + type;
    var pass = "";

    if (type == "IP") {
        pass = "/CoviWeb/Approval/CreateSubCode/Popup/Create_Tree.aspx?type=CLNT"; 
    } else if (type == "SL") {
        pass = "/CoviWeb/Approval/CreateSubCode/Popup/Create_Tree.aspx?type=ENGM"; 
    } else if (type == "SF") {
        //pass = "/CoviWeb/WinC/Common/CategorySelect.aspx?CATEGORY_TYPE=" + type;
        pass = "/CoviWeb/WinC/Common/Storefront.aspx";
    }

    var options = 'width=' + nWidth;
    options += ' ,height=' + nHeight;
    options += ' ,left=' + (screen.availWidth - nWidth) / 2;
    options += ' ,top=' + (screen.availHeight - nHeight) / 2;
    options += ' ,scrollbars=yes';
    options += ' ,titlebar=no';
    options += ' ,resizable=no';
    options += ' ,Status=no';
    options += ' ,toolbar=no';

    window.open(pass, "categorySelectPop", options);
}

function categoryList_Select(cateNameID, returnTarget) {

    var namepath = $("#cate" + cateNameID).attr("namepath");
    var codepath = $("#cate" + cateNameID).attr("codepath");
    var userDisplay = namepath;

    var l = namepath.split('\\').length;

    for (var i = 0; i < l; i++) {
        userDisplay = userDisplay.replace("\\", " > ");
    }

    userDisplay = userDisplay.substring(0, userDisplay.length - 2);
    $(opener.document).find("#" + returnTarget + "_CateName").val(namepath);
    $(opener.document).find("#" + returnTarget + "_CateCode").val(codepath);
    $(opener.document).find("#" + returnTarget + "_CateUserDisplay").text(userDisplay);

    if (typeof (window.opener.CateCodeSet) != 'undefined') {
        window.opener.CateCodeSet(returnTarget);
    }

    self.close();

}


//Client 선택
function winOpen(nwidth, nheight, popdiv) {
    var nWidth = nwidth;
    var nHeight = nheight;
    var nLeft = (window.screen.width - nWidth) / 2;
    var nTop = (window.screen.height - nHeight) / 2;
    var sFeatures = "top=" + nTop + "px,left=" + nLeft + "px,width=" + nWidth + "px,height=" + nHeight + "px,scrollbars=yes";
    switch (popdiv) {
        case "Client":  //클라이언트
            var sURL = "/CoviWeb/WinC/PD/ClientSelectPop.aspx?pid=client";
            break;
        case "PD":  //클라이언트
            var sURL = "/CoviWeb/WinC/Common/PDSelectPop.aspx?pid=PD";
            break;
        case "AccountPgm": // 
            sURL = "/CoviWeb/WinC/PD/AccountPgmPop.aspx";
            break;
        case "CostCenter": // 
            sURL = "/CoviWeb/WinC/PD/CostCenterPop.aspx";
            break;
        case "WonDropLostReasonHelp": // WonLostDrop 도움말 팝업
            sURL = "/CoviWeb/WinC/PD/WonDropLostReasonHelp.aspx";
            break;
        case "spContractGuide": // Engagement Code SetUp 계약서 작성지침 바로가기
            sURL = "https://asone.deloitte.co.kr/km/CompanyBylaws/Companyraws/Attachments/24/10.%20%EC%95%88%EC%A7%84%ED%9A%8C%EA%B3%84%EB%B2%95%EC%9D%B8%20%EA%B3%84%EC%95%BD%EC%84%9C%20%EC%9E%91%EC%84%B1%20%EC%A7%80%EC%B9%A8_20220118.pdf";  // 임시
            break;
        case "spAuditFATAXForm": // Engagement Code Audit, FA, TAX 
            sURL = "https://gw.deloitte.co.kr/CoviWeb/Approval/Forms/Form.aspx?fmid=e159590d-277f-43d5-a987-62a7c9fe50d2&fmnm=%ec%82%ac%ec%a0%84%ec%9a%a9%ec%97%ad%ed%88%ac%ec%9e%85%20%ec%82%ac%ec%9c%a0%ec%84%9c&fmpf=WF_SUNTOIP_PUMEI_ALL&scid=b99bbbb2-d29a-49de-870f-b51b0efce167&mode=DRAFT&fmrv=2&fmfn=WF_SUNTOIP_PUMEI_ALL_V2&TaskID=&TaskName=";
           
            break;     
        case "spConsultingForm": // Consulting : 선투입 품의서
            sURL = "https://gw.deloitte.co.kr/CoviWeb/Approval/Forms/Form.aspx?fmid=2a22fe3d-10de-4e39-8944-9c2f4b413361&fmnm=%ec%84%a0%ed%88%ac%ec%9e%85%20%ed%92%88%ec%9d%98&fmpf=WF_SUNTOIP_PUMEI&scid=390b4048-1957-4193-8065-22f82bfe446e&mode=DRAFT&fmrv=2&fmfn=WF_SUNTOIP_PUMEI_V2&TaskID=&TaskName=";
            break;
        case "spERSForm": // ERS 선투입 품의서
            sURL = "https://gw.deloitte.co.kr/CoviWeb/Approval/Forms/Form.aspx?fmid=f2bd37a0-cbe4-4ffa-b98c-0d0cea57e911&fmnm=%45%52%53%20%ec%84%a0%ed%88%ac%ec%9e%85%20%ed%92%88%ec%9d%98&fmpf=WF_ERS_SUNTOIP_PUMEI&scid=b99bbbb2-d29a-49de-870f-b51b0efce167&mode=DRAFT&fmrv=0&fmfn=WF_ERS_SUNTOIP_PUMEI_REQ_V0&TaskID=&TaskName=";
            break;
        case "ChargePartner":  //Partner in Charge
            var sURL = "/CoviWeb/WinC/PD/ClientSelectPop.aspx?pid=chargepartner";
            break;
        case "BillingPartner":  //Billing Partner
            var sURL = "/CoviWeb/WinC/PD/ClientSelectPop.aspx?pid=billingpartner";
            break;
        case "AbacTaperList":  //청탁금지법 대상자 조회
            var sURL = "/CoviWeb/approval/abac/selectuserlist.aspx?seltype=multi&returntype=json&callbackfunction=setValue";
            break;
        case "WinLost":  //경쟁력분석 선택
            var sURL = "/CoviWeb/WinC/PD/WinLostPop.aspx";
            break;
        case "Competitor":  //경쟁업체
            var sURL = "/CoviWeb/WinC/PD/CompetitorPop.aspx";
            break;
        case "PDListClient":  //PDList상세검색 - 클라이언트
            var sURL = "/CoviWeb/WinC/PD/ClientSelectPop.aspx?pid=pdlistsearch";
            break;
        case "PDListCostCenter":  //PDList상세검색 - Costcenter
            var sURL = "/CoviWeb/WinC/PD/CostCenterPop.aspx?pid=pdlistsearch";
            break;
        case "PDListAccountPgm":  //PDList상세검색 - AccountPgm
            var sURL = "/CoviWeb/WinC/PD/AccountPgmPop.aspx?pid=pdlistsearch";
            break;
        case "ContractClient":  //PD등록  - 실제계약 클라이언트
            var sURL = "/CoviWeb/WinC/PD/ClientSelectPop.aspx?pid=cntclient";
            break;
        case "AccountList": // Account(Client) 검색
            var sURL = "/CoviWeb/WinC/PD/ClientSelectPop.aspx?pid=accountsearch";
            break;
        case "CostCenterList": // Costcenter 검색
            var sURL = "/CoviWeb/WinC/PD/CostCenterPop.aspx?pid=costcenterlist";
            break;
        default:
            

        // code block      
    }

    window.open(sURL, "", sFeatures);
}


function winClientOpen(nwidth, nheight, popdiv, textId) {
    var nWidth = nwidth;
    var nHeight = nheight;
    var nLeft = (window.screen.width - nWidth) / 2;
    var nTop = (window.screen.height - nHeight) / 2;
    var sFeatures = "top=" + nTop + "px,left=" + nLeft + "px,width=" + nWidth + "px,height=" + nHeight + "px,scrollbars=yes";
    switch (popdiv) {
        case "ClientMulti":  //클라이언트
            var sURL = "/CoviWeb/WinC/PD/ClientSelectPop.aspx?pid=clientMulti&textId=" + textId;
            break;
        
        default:
        // code block      
    }

    window.open(sURL, "", sFeatures);
}


//////////////////////////////////////////////////////////////
//SAP 연동
/////////////////////////////////////////////////////////////

//function SAP_Interface(type, returnFunction) {
//    var data = { type: type };
//    call_winCws("/CoviWeb/WinC/Common/service/Get_Functions.ashx", data, Set_Restricted_Table);
//}

function call_winCws(_url, _data, _callback) {
    //showLoading();

    $.ajax({
        url: _url,
        type: "POST",
        dataType: "json",
        data: _data,
        async: false, //동기 방식으로 진행함
        success: _callback,
        error: function (error) {
            alert('데이터 조회에 실패하였습니다. 다시 시도해 주세요.\r\n- ' + error.responseText);
        }
    });
}


/////////////////////////////////////////////////////////////



//파일 다운로드
function PopListSingleWinC(strServiceAlias, strMessageID, strFileID) {

    /* AOP 그룹웨어 Covision */
    SetEventLog(strServiceAlias, "filedown", window.location.href, "", strFileID);

    var strHost = GetBaseConfig("AOP_URL");
    var strDownURL = "";
    var strParam = "?pServiceAlias=" + strServiceAlias + "&pMessageID=" + strMessageID + "&pFileID=" + strFileID; //"&pMenuAlias=" + strMenuAlias +


    //CoviFileTrans download
    /* AOP 그룹웨어 Covision */
    if (("ActiveXObject" in window)) {
        strDownURL = "WinC/Common/FileDownload_IE.aspx";
    }
    else {
        strDownURL = "WinC/Common/FileDownload_NonIE.aspx";

    }

    if (strDownURL != "") {
        document.getElementById("download").src = strHost + strDownURL + strParam;
    }
}


//페이징 문자열 반환
function setPaging(curPage, pageSize, totalCount) {
    var curPageNo = curPage * 1;
    var pageSizeNo = pageSize * 1;
    var totalCountNo = totalCount * 1;
    var TotalPageCount = totalCountNo / pageSizeNo;
    var Blocksize = 10;

    var prevStr = "";
    var nextStr = "";
    var i = 1;

    var temp = ((curPageNo - 1) / Blocksize) * Blocksize + 1;

    if (curPageNo > 1) {
        prevStr = " <a href=\"javascript:pagingGo(1);\" title=\"첫페이지로이동\" class=\"btn_prev_start\"></a>";
        prevStr = prevStr + " <a href=\"javascript:pagingGo(" + (curPageNo - 1) + ");\" title=\"이전페이지로이동\" class=\"btn_prev\"></a>";
    } else {
        prevStr = " <a title=\"첫페이지로이동\" class=\"btn_prev_start\"></a>";
        prevStr = prevStr + " <a title=\"이전페이지로이동\" class=\"btn_prev\"></a>";
    }

    var sbPaging = "";

    sbPaging = sbPaging + "<div class='Paging' id='divPaging' name='divPaging'>";
    sbPaging = sbPaging + prevStr;

    while (temp <= TotalPageCount && i <= Blocksize) {
        if (temp == curPageNo) {
            sbPaging = sbPaging + " <a class=\"active\"> " + temp + " </a> ";
        } else {
            sbPaging = sbPaging + " <a href=\"javascript:pagingGo('" + temp + "')\"> " + temp + " </a> ";
        }
        temp++;
        i++;
    }

    if (curPageNo < TotalPageCount) {
        nextStr = "<a href=\"javascript:pagingGo(" + (curPageNo + 1) + ");\" title=\"다음페이지로이동\" class=\"btn_next\"></a>";
        nextStr = nextStr + " <a href=\"javascript:pagingGo(" + TotalPageCount + ");\" title=\"마지막페이지로이동\" class=\"btn_next_end\"></a>";
    } else {
        nextStr = "<a title=\"다음페이지로이동\" class=\"btn_next\"></a>";
        nextStr = nextStr + " <a title=\"마지막페이지로이동\" class=\"btn_next_end\"></a>";
    }

    sbPaging = sbPaging + nextStr + "</div>";

    return sbPaging;

}

function clickHidMenu(hidMenuID) {
    if ($("#hidMenu_" + hidMenuID).css("display") == "none") {
        $("#hidMenu_" + hidMenuID).css("display", "block");
    } else if ($("#hidMenu_" + hidMenuID).css("display") == "block") {
        $("#hidMenu_" + hidMenuID).css("display", "none");
    }
}



function ApprovalClassList_Pop(ruleType, ruleId, returnTarget) {
    var nWidth = 750;
    var nHeight = 510;
    var pass = "/Coviweb/Approval/Forms/winc_popup/Popup/ApprovalReviewer.aspx?RULE_TYPE=" + ruleType + "&RULE_ID=" + ruleId + "&RETURN_TARGET=" + returnTarget;
    var options = 'width=' + nWidth;
    options += ' ,height=' + nHeight;
    options += ' ,left=' + (screen.availWidth - nWidth) / 2;
    options += ' ,top=' + (screen.availHeight - nHeight) / 2;
    options += ' ,scrollbars=yes';
    options += ' ,titlebar=no';
    options += ' ,resizable=no';
    options += ' ,Status=no';
    options += ' ,toolbar=no';

    window.open(pass, "ApprovalClassListPop", options);

}
/*

个*/

function BIF_FormView(fmid, fmpf, scid, mode, fmrv, fiid, piid, wiid, pdcode, userID, formtype, order, isMig) {
    var nWidth = 900; //1530;  px *  px
    var nHeight = 765;//765;
    var nLeft = (window.screen.width - nWidth) / 2;
    var nTop = (window.screen.height - nHeight) / 2;
    var sFeatures = "top=" + nTop + "px,left=" + nLeft + "px,width=" + nWidth + "px,height=" + nHeight + "px,scrollbars=yes";

    if (isMig != "") {
        window.open(isMig, "", sFeatures);
    } else {
        if (fmrv == "") {
            fmrv = "0";
        }

        if (fmpf.substring(0, 8) == "WF_WINC_") { // 2020.02.20 WinC 양식은 Width : 1024로 오픈
            sFeatures = "top=" + nTop + "px,left=" + nLeft + "px,width=1024px,height=650px,scrollbars=yes";
            if (mode == "TEMPSAVE") {
                BIF_GetFormTempInfo(fmid, fmpf, scid, mode, fmrv, fiid, piid, wiid, pdcode, formtype, sFeatures, order);
            }
            else {
                BIF_GetFormWiidInfo(fmid, fmpf, scid, mode, fmrv, fiid, piid, wiid, pdcode, formtype, sFeatures, order, userID);
            }
        }
    }

    
    //if (fmpf == "WF_WINC_CNI_CHECK") { // 2020.03.17 WinC C&I check 양식 크기 변경 
    //    sFeatures = "top=" + nTop + "px,left=" + nLeft + "px,width=1200px,height=650px,scrollbars=yes";
    //}

}

function BIF_GetFormWiidInfo(fmid, fmpf, scid, mode, fmrv, fiid, piid, wiid, pdcode, formtype, sFeatures, order, userID) {
    var sUrl;
    var ret;
    var usid = userID;
    sUrl = "/CoviWeb/WinC/Common/service/WincService.asmx/SelectFormInfoWiid";

    $.ajax({
        type: "POST",
        url: sUrl,
        data: "{piid:\"" + piid + "\", usid:\"" + usid + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var WORKITEM_ID = data.d.split('/')[0] != undefined ? data.d.split('/')[0].split(':')[1] : "";
            var PROCESS_ID = data.d.split('/')[1] != undefined ? data.d.split('/')[1].split(':')[1] : "";
            var MODE = data.d.split('/')[2] != undefined ? data.d.split('/')[2].split(':')[1] : "";
            var PF_ID = data.d.split('/')[3] != undefined ? data.d.split('/')[3].split(':')[1] : "";
            //var PT_ID = data.d.split('/')[4] != undefined ? data.d.split('/')[4].split(':')[1] : "";

             //var ret = WORKITEM_ID + "/" + PROCESS_ID + "/" + MODE + "/" + PF_ID ;
            //var pfid = "";
            //var wiid;
            //var piid;
            //var mode;
            //var pfid;
            //var ptid;

            var ret = WORKITEM_ID + "/" + PROCESS_ID + "/" + MODE + "/" + PF_ID;
            var pfid = "";

            if (WORKITEM_ID != "") {
                wiid = ret.split('/')[0];
                piid = ret.split('/')[1];
                mode = ret.split('/')[2];
                pfid = ret.split('/')[3];

                sURL = "/CoviWeb/Approval/Forms/Form.aspx?fmid=" + fmid + "&fmpf=" + fmpf + "&scid=" + scid + "&mode=" + mode + "&fmrv=" + fmrv + "&fiid=" + fiid + "&piid=" + piid + "&wiid=" + wiid + "&pfid=" + pfid + "&formcode=" + pdcode + "&formtype=" + formtype + "&order=" + order;

            }
            //if (WORKITEM_ID != "") {


            //    sURL = "/CoviWeb/Approval/Forms/Form.aspx?fmid=" + fmid + "&fmpf=" + fmpf + "&scid=" + scid + "&mode=" + mode + "&fmrv=" + fmrv + "&fiid=" + fiid + "&piid=" + piid + "&wiid=" + wiid + "&pfid=" + pfid  + "&formcode=" + pdcode + "&formtype=" + formtype + "&order=" + order;

            //}
            //부서문서함
            //else if (WORKITEM_ID != "" && ptid != "") {
            //    sURL = "/CoviWeb/Approval/Forms/Form.aspx?fmid=" + fmid + "&fmpf=" + fmpf + "&scid=" + scid + "&mode=" + mode + "&fmrv=" + fmrv + "&fiid=" + fiid + "&piid=" + piid + "&wiid=" + wiid + "&formcode=" + pdcode + "&formtype=" + formtype + "&order=" + order
            //        + "&ptid=" + toUTF8(ptid) + "&gloct=DEPART";
            //}
            else {
                sURL = "/CoviWeb/Approval/Forms/Form.aspx?fmid=" + fmid + "&fmpf=" + fmpf + "&scid=" + scid + "&mode=" + mode + "&fmrv=" + fmrv + "&fiid=" + fiid + "&piid=" + piid + "&wiid=" + wiid + "&formcode=" + pdcode + "&formtype=" + formtype + "&order=" + order;
            }

            window.open(sURL, "", sFeatures);
        },
        error: function (request, status, error) {
            ret = "";
            alert("code = " + request.status + " message = " + request.responseText + " error = " + error); // 실패 시 처리
        }
    });
    return ret;
}
function BIF_GetFormTempInfo(fmid, fmpf, scid, mode, fmrv, fiid, piid, wiid, pdcode, formtype, sFeatures, order) {
    var sUrl;
    var ret;
    sUrl = "/CoviWeb/WinC/Common/service/WincService.asmx/SelectFormInfoTempSave";
    $.ajax({
        type: "POST",
        url: sUrl,
        data: "{fiid:\"" + fiid + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            var FORM_TEMP_INST_ID = data.d.split('/')[0] != "" ? data.d.split('/')[0].split(':')[1] : "";
            var fitn = "WF_FORM_INSTANCE_" + fmpf + "__V" + fmrv;

            sURL = "/CoviWeb/Approval/Forms/Form.aspx?fmid=" + fmid + "&fmpf=" + fmpf + "&scid=" + scid + "&mode=" + mode + "&fmrv=" + fmrv + "&fiid=" + fiid + "&piid=" + piid + "&wiid=" + wiid + "&fitn=" + fitn + "&ftid=" + FORM_TEMP_INST_ID + "&formcode=" + pdcode + "&formtype=" + formtype + "&order=" + order;

            window.open(sURL, "", sFeatures);

        },
        error: function (request, status, error) {
            alert("code = " + request.status + " message = " + request.responseText + " error = " + error); // 실패 시 처리
        }
    });
}
function toUTF8(szInput) {
    var wch, x, uch = "", szRet = "";
    for (x = 0; x < szInput.length; x++) {
        wch = szInput.charCodeAt(x);
        if (!(wch & 0xFF80)) {
            szRet += "%" + wch.toString(16);
        }
        else if (!(wch & 0xF000)) {
            uch = "%" + (wch >> 6 | 0xC0).toString(16) +
                "%" + (wch & 0x3F | 0x80).toString(16);
            szRet += uch;
        }
        else {
            uch = "%" + (wch >> 12 | 0xE0).toString(16) +
                "%" + (((wch >> 6) & 0x3F) | 0x80).toString(16) +
                "%" + (wch & 0x3F | 0x80).toString(16);
            szRet += uch;
        }
    }
    return (szRet);
}
function openPayerClientPop(bspRowID) {
    var nWidth = 900;
    var nHeight = 500;
    var pass = "/Coviweb/WinC/Common/PayerClientPop.aspx?bspRowID=" + bspRowID;
    var options = 'width=' + nWidth;
    options += ' ,height=' + nHeight;
    options += ' ,left=' + (screen.availWidth - nWidth) / 2;
    options += ' ,top=' + (screen.availHeight - nHeight) / 2;
    options += ' ,scrollbars=yes';
    options += ' ,titlebar=no';
    options += ' ,resizable=no';
    options += ' ,Status=no';
    options += ' ,toolbar=no';

    window.open(pass, "ApprovalClassListPop", options);

}

function checkAdminPage() {
    if (opener != null) { //부모창 있으면 팝업 닫기
        self.close();
    } else { //없으면 메인으로
        document.location.href = "/Coviweb/WinC/PD/PDList.aspx";
    }

}
function initReadonly() {

    //편집모드 구분
    if (m_oFormMenu.document.getElementById("btUserchangeSave").style.display == "") {

        $("[readonly2='y']").each(function () {
            $(this).attr("disabled", true);
        });
        return;
    }
  
}
function PopupPage(gopageURL, gopageName, gopageWidth, gopageHeight, scroll) {
    var pop = window.open(gopageURL, '',
        'width=' + gopageWidth + ', height=' + gopageHeight + ', menubar=no, scrollbars=' + scroll + ', resizable=yes');
    pop.focus();
}

//PD Audit Time Budget 작성 여부 체크
var PDTBCode = function (pdcode) {
    var sUrl;
    var TB_Code = "";
    var TB_Msg = "";
    var TB_STATUS_CD;
    var TB_Status_msg;

    sUrl = "/CoviWeb/WinC/Common/service/WincService.asmx/TimeBudgetSelect";
    $.ajax({
        type: "POST",
        url: sUrl,
        data: "{PdCode:\"" + pdcode + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (data) {
            if (data.d != "F") {
                TB_STATUS_CD = data.d.split('/')[0] != undefined ? data.d.split('/')[0].split(':')[1] : "";
                TB_Status_msg = "";
                if (TB_STATUS_CD == "10") {
                    TB_Status_msg = "작성 중";
                }
                else if (TB_STATUS_CD == "20") {
                    TB_Status_msg = "검토요청";
                }
                else if (TB_STATUS_CD == "30") {
                    TB_Status_msg = "보완요청";
                }
                else if (TB_STATUS_CD == "40") {
                    TB_Status_msg = "검토완료";
                }
                TB_Code = TB_STATUS_CD;
                TB_Msg = TB_Status_msg;
            } else {
                TB_Code = "0";
                TB_Msg = "";
            }
        },
        error: function (e) {
            TB_Code = "F";
            TB_Msg = "IT 부서에 문의하시기 바랍니다(2)";
        }
    });

    return { x: TB_Code, y: TB_Msg };
}
//마이그레이션 폼 View
function FormViewMig(sURL) {
    var nWidth = 1000;
    var nHeight = 765;
    var nLeft = (window.screen.width - nWidth) / 2;
    var nTop = (window.screen.height - nHeight) / 2;
    var sFeatures = "top=" + nTop + "px,left=" + nLeft + "px,width=" + nWidth + "px,height=" + nHeight + "px,scrollbars=yes,resizable=yes";
    window.open(sURL, "", sFeatures);
}

function getUrlParams(url, param) {
    var returnValue = "";
    url.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (str, key, value) {
        if (key.toUpperCase() === param.toUpperCase()) {
            returnValue = value;
        }
        
    });
    return returnValue;
}

function fn_clipboardcopy(obj) {
    var textToCopy = $(obj).attr("value");
    if (textToCopy === undefined || textToCopy === "") textToCopy = $(obj).val();
    if (textToCopy === undefined || textToCopy === "") textToCopy = $(obj).text();

    var msgDiv = '<div id="msgClipboard" style="width:100px; height:30px; line-height:30px; background:#000; text-align:center; position:fixed; left:45%; bottom:50px; display:none;">' +
        '  <span id="msgClipboardText" style = "color:#fff"> 복사완료</span>' +
        '</div> ';

    if ($("#msgClipboard").length < 1) {
        $("body").append(msgDiv);
    }

    copyToClipboard(textToCopy);
}

function copyToClipboard(textToCopy) {
    // navigator clipboard api needs a secure context (https)
    try {
        if (isWindow()) {
            // navigator clipboard api method'
            window.clipboardData.setData('Text', textToCopy);

            $("#msgClipboardText").text("복사완료");
            $("#msgClipboard").show();
            $("#msgClipboard").fadeOut(2000);
        } else {
            // text area method
            var textArea = document.createElement("textarea");
            textArea.value = textToCopy;
            // make the textarea out of viewport
            textArea.style.position = "fixed";
            textArea.style.left = "-999999px";
            textArea.style.top = "-999999px";
            document.body.appendChild(textArea);
            textArea.focus();
            textArea.select();
            return new Promise(function(res, rej) {
                // here the magic happens
                document.execCommand('copy') ? res() : rej();
                textArea.remove();

                $("#msgClipboardText").text("복사완료");
                $("#msgClipboard").show();
                $("#msgClipboard").fadeOut(2000);
            });
        }
    } catch (ex) {
        $("#msgClipboardText").text("복사실패");
        $("#msgClipboard").show();
        $("#msgClipboard").fadeOut(2000);
    }
}
