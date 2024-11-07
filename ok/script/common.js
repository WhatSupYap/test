//===========================================================================================
// 제목 : 공통
//===========================================================================================
if (!Common) {
    var Common = {};
}
if (!Common.Lang) {
    Common.Lang = {};
}

Common.CNST_SP = "~,";
Common.CNST_SP_LF = "~I~";
Common.CNST_SP_NT = "|~|";
Common.SP_TABLE = "~~||TBL||~~";

//===========================================================================================
//제목 : 공통함수
//===========================================================================================
// Url 유효성 검사
Common.MoveUrlCheck = function (Url) {
    // 등록된 페이지 유효성 검사
    var existsPage = false;
    var ReqInfo = new Common.RequestInfo();
    ReqInfo.AddParameter("MOVE_URL", Url.split("?")[0]);

    Common.Ajax("/PageUrlCheck", ReqInfo, function (rtn) {
        if (Common.Convert.Bool(rtn)) {
            existsPage = true;
        }
    });

    return existsPage;
}

//페이지 이동
Common.MoveUrl = function (Url, Msg, Check) {
    if (Url == "")
        return;

    if (Common.MoveUrlCheck(Url) == false) {
        return;
    }

    if (Msg == null || Msg == undefined) {
        location.href = Url;
    } else if (Check == null || Check == undefined || Check == false) {
        swal({
            title: Msg,
            showCancelButton: false,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "확인",
            closeOnConfirm: true,
            animation: false
        }).then(function () {
            location.href = Url;
        });
    } else if (Check == true) {
        swal({
            title: Msg,
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "확인",
            cancelButtonText: "취소",
            closeOnConfirm: false,
            animation: false
        }).then(function () {
            location.href = Url;
        });
    }
}

//페이지 새창
Common.OpenUrl = function (url, wname, width, height, scrl, resi, stat, addOption) {
    if (Common.MoveUrlCheck(url) == false) {
        alert("정상적인 URL이 아닙니다.");
        return;
    }

    if (typeof wname == "undefined") {
        return window.open(url);
    }

    // 듀얼모니터 일경우 가운데 정렬에 문제가 좀 있어서 우선 선택한 창에서 나오게는 처리함
    //var winl = (screen.width - width) / 2;
    var winl = window.screenX + (screen.width / 2) - (width / 2);
    var wint = (screen.height - height) / 2;

    if (typeof scrl == "undefined") {
        var scroll = "no";
    } else {
        var scroll = scrl;
    }

    if (typeof resi == "undefined") {
        var resizable = "no";
    } else {
        var resizable = resi;
    }

    if (typeof stat == "undefined") {
        var status = "yes";
    } else {
        var status = stat;
    }

    if (typeof addOption == "undefined") {
        addOption = "";
    } else {
        addOption = "," + addOption;
    }

    return window.open(url, wname, "left=" + winl + ", top=" + wint + ", scrollbars=" + scroll + ", status=" + status + ", resizable=" + resizable + ", width=" + width + ", height=" + height + addOption);
}

Common.Msg = function (Msg, info) {
    info = (info || {});
    var mode = (info.mode || "msg");
    var okback = (info.okback || undefined);
    var cancelback = (info.cancelback || undefined);
    var text = (info.text || "");
    var html = (info.html || "");
    var allowOutsideClick = true;

    if (info.allowOutsideClick != undefined) {
        allowOutsideClick = info.allowOutsideClick;
    }

    var option = {};
    option.title = Msg;
    if (text != "") {
        option.text = text;
    }
    if (html != "") {
        option.html = html;
    }
    option.showCancelButton = false;
    option.confirmButtonColor = "#DD6B55";
    option.confirmButtonText = "확인";
    option.closeOnConfirm = true;
    option.allowOutsideClick = allowOutsideClick;
    option.animation = false;

    if (mode == "msg") {
        swal(option).then(function () {
            if (typeof okback == "function") {
                okback();
            }
        }
        );
    } else if (mode == "confrim") {
        option.type = "warning";
        option.cancelButtonText = "취소";
        option.showCancelButton = true;

        swal(option).then(function () {
            if (typeof okback == "function") {
                okback();
            }
        }, function (dismiss) {
            if (dismiss === 'cancel') {
                if (typeof cancelback == "function") {
                    cancelback();
                }
            }
        });
    }
}

//상위 이벤트 멈추기
Common.EventCancelBubble = function (event) {
    try {
        event = event || window.event;

        if (event.preventDefault) {
            event.stopPropagation();
        } else {
            event.cancelBubble = false;
        }
    } catch (e) {
    }
}

//하위 이벤트 멈추기
Common.EventReturnValue = function (event) {
    try {
        event = event || window.event;

        if (event.preventDefault) {
            event.preventDefault();
        } else {
            event.returnValue = false;
        }
    } catch (e) {
    }
}

Common.isDateValid = function (dateStr) {
    return !isNaN(new Date(dateStr));
}


Common.GetFiscalYear = function (d) {
    const RtnValue = {};

    if (Common.isDateValid(d)) {
        const myDate = new Date(d);
        // (CASE WHEN MONTH(@STD_DT) < 6 THEN YEAR(@STD_DT) - 1 ELSE YEAR(@STD_DT) END)
        let fy = myDate.getFullYear();
        const m = myDate.getMonth() + 1;
        if (m < 6) fy = fy - 1;
        RtnValue["fiscalYear"] = fy + 1;
        RtnValue["startDate"] = new Date(`${fy}-06-01`);
        RtnValue["endDate"] = (new Date(myDate.getFullYear(), m, 1)).addSeconds(-0.001);
        RtnValue["txtPre"] = `FY${(fy).toString().slice(-2)}`;
        RtnValue["txt"] = `FY${(fy + 1).toString().slice(-2)}`;
        RtnValue["txtNext"] = `FY${(fy + 2).toString().slice(-2)}`;

    }
    return RtnValue;
}

//현재 날짜시간 포함 string 리턴
Common.GetTodayTimeString = function () {
    var RtnValue = "";

    var date = new Date(); // 날짜
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hour = date.getHours();
    var min = date.getMinutes();
    var sec = date.getSeconds();
    if (String(month).length == 1) { month = "0" + month; };
    if (String(day).length == 1) { day = "0" + day; };
    if (String(hour).length == 1) { hour = "0" + hour; };
    if (String(min).length == 1) { min = "0" + min; };
    if (String(sec).length == 1) { sec = "0" + sec; };

    RtnValue = String(year) + String(month) + String(day) + String(hour) + String(min) + String(sec);

    return RtnValue;
}

Common.GetDateString = (date, format) => {

    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hour = date.getHours();
    var min = date.getMinutes();
    var sec = date.getSeconds();

    if (String(month).length == 1) { month = "0" + month; };
    if (String(day).length == 1) { day = "0" + day; };
    if (String(hour).length == 1) { hour = "0" + hour; };
    if (String(min).length == 1) { min = "0" + min; };
    if (String(sec).length == 1) { sec = "0" + sec; };

    if (format === undefined || format === null || format == "yyyy-MM-dd") {
        RtnValue = `${String(year)}-${String(month)}-${String(day)}`;
    }
    else if (format == "yyyy-MM") {
        RtnValue = `${String(year)}-${String(month)}`;
    }
    else {
        RtnValue = String(year) + String(month) + String(day) + String(hour) + String(min) + String(sec);
    }

    return RtnValue;
}

// 타이머 세팅
Common.SetTimer = function (obj) {
    if (typeof obj == "undefined" || obj.length == 0) {
        return;
    }

    var starttime = obj.attr("starttime");      // yyyyMMddhhmmss
    if (typeof starttime == "undefined") {
        starttime = Common.GetTodayTimeString();
    }
    var endtime = obj.attr("endtime");          // yyyyMMddhhmmss
    var ingtime = obj.attr("ingtime");          // yyyyMMddhhmmss
    var time_sec = Common.Convert.Int(obj.attr("time_sec"));
    if (time_sec > 0) {
        if (typeof ingtime == "undefined") {
            ingtime = starttime;
        }

        var s_year, s_month, s_day, s_hour, s_min, s_sec;
        var e_year, e_month, e_day, e_hour, e_min, e_sec;

        s_year = ingtime.substring(0, 4);
        s_month = ingtime.substring(4, 6);
        s_day = ingtime.substring(6, 8);
        s_hour = ingtime.substring(8, 10);
        s_min = ingtime.substring(10, 12);
        s_sec = ingtime.substring(12, 14);
        var ingdate = new Date(s_month + "/" + s_day + "/" + s_year + " " + s_hour + ":" + s_min + ":" + s_sec);

        e_year = endtime.substring(0, 4);
        e_month = endtime.substring(4, 6);
        e_day = endtime.substring(6, 8);
        e_hour = endtime.substring(8, 10);
        e_min = endtime.substring(10, 12);
        e_sec = endtime.substring(12, 14);
        var enddate = new Date(e_month + "/" + e_day + "/" + e_year + " " + e_hour + ":" + e_min + ":" + e_sec);

        if (s_year + s_month + s_day != e_year + e_month + e_day) {
            var days = (enddate.getTime() - ingdate.getTime()) / 1000 / 60 / 60 / 24;
            if (days > 0) {
                obj.html(Math.ceil(days) + "일");
            } else {
                obj.html("");
            }
        } else {
            setInterval(function () {
                ingdate = ingdate.addSeconds(time_sec);

                if (ingdate >= enddate) {
                    obj.html("00:00");
                } else {
                    var year = ingdate.getFullYear();
                    var month = ingdate.getMonth() + 1;
                    var day = ingdate.getDate();
                    var hour = ingdate.getHours();
                    var min = ingdate.getMinutes();
                    var sec = ingdate.getSeconds();
                    if (String(month).length == 1) { month = "0" + month; };
                    if (String(day).length == 1) { day = "0" + day; };
                    if (String(hour).length == 1) { hour = "0" + hour; };
                    if (String(min).length == 1) { min = "0" + min; };
                    if (String(sec).length == 1) { sec = "0" + sec; };

                    obj.attr("ingtime", String(year) + String(month) + String(day) + String(hour) + String(min) + String(sec))

                    var dateGap = enddate.getTime() - ingdate.getTime();
                    var timeGap = new Date(0, 0, 0, 0, 0, 0, enddate - ingdate);

                    year = timeGap.getFullYear();
                    month = timeGap.getMonth() + 1;
                    day = timeGap.getDate();
                    hour = timeGap.getHours();
                    min = timeGap.getMinutes();
                    sec = timeGap.getSeconds();
                    if (String(month).length == 1) { month = "0" + month; };
                    if (String(day).length == 1) { day = "0" + day; };
                    if (String(hour).length == 1) { hour = "0" + hour; };
                    if (String(min).length == 1) { min = "0" + min; };
                    if (String(sec).length == 1) { sec = "0" + sec; };

                    if (hour == "00") {
                        obj.html(min + ":" + sec);
                    } else {
                        obj.html(hour + ":" + min + ":" + sec);
                    }
                }
            }, 1000);
        }
    }
}

//yyyymmdd -> yyyy-mm-dd, yyyy-mm-dd -> yyyymmdd
//val(string) : 날짜, kind(string) : 날짜형태
Common.GetDate = function (val, Kind) {
    var RtnValue = "";

    if (val == "" || val.trim() == "")
        return "";

    if (Kind == undefined) {
        if (Common.Contains(val, "-")) {
            RtnValue = val.replaceAll("-", "");
        } else {
            RtnValue = val.substring(0, 4).toString() + "-" + val.substring(4, 6).toString() + "-" + val.substring(6, 8).toString();
        }
    } else {
        if (Kind.toLowerCase() == "yyyymmdd") {
            if (Common.Contains(val, "-")) {
                RtnValue = val.replaceAll("-", "");
            } else {
                RtnValue = val;
            }
        } else if (Kind.toLowerCase() == "yyyy-mm-dd" || Kind.toLowerCase() == "yyyy-MM-dd") {
            if (Common.Contains(val, "-")) {
            } else {
                RtnValue = val.substring(0, 4).toString() + "-" + val.substring(4, 6).toString() + "-" + val.substring(6, 8).toString();
            }
        } else if (Kind.toLowerCase() == "yyyymm") {
            RtnValue = val.substring(0, 6).toString();
        } else if (Kind.toLowerCase() == "yyyy-mm") {
            RtnValue = val.substring(0, 4).toString() + "-" + val.substring(4, 6).toString();
        } else if (Kind.toLowerCase() == "yyyy") {
            RtnValue = val.substring(0, 4).toString();
        }
    }

    return RtnValue;
}

//yyyymm -> yyyy-mm, yyyy-mm -> yyyymm
//val(string) : 날짜, kind(string) : 날짜형태
Common.GetDateYearMonth = function (val, Kind) {
    var RtnValue = "";

    if (val.trim() == "")
        return "";

    if (Kind == undefined) {
        if (Common.Contains(val, "-")) {
            RtnValue = val.substring(0, 6).replaceAll("-", "");
        } else {
            RtnValue = val.substring(0, 4).toString() + "-" + val.substring(4, 6).toString();
        }
    } else {
        if (Kind.toLowerCase() == "yyyymm" || Kind.toLowerCase() == "yyyymmdd") {
            if (Common.Contains(val, "-")) {
                RtnValue = val.replaceAll("-", "");
            } else {
                RtnValue = val;
            }
        } else if (Kind.toLowerCase() == "yyyy-mm") {
            if (Common.Contains(val, "-")) {
                RtnValue = val;
            } else {
                RtnValue = val.substring(0, 4).toString() + "-" + val.substring(4, 6).toString();
            }
        } else if (Kind.toLowerCase() == "yyyy-mm-dd") {
            if (Common.Contains(val, "-")) {
                RtnValue = val;
            } else {
                RtnValue = val.substring(0, 4).toString() + "-" + val.substring(4, 6).toString() + "-" + val.substring(6, 8).toString();
            }
        }
    }

    return RtnValue;
}

//DateAdd
//dateType (day, month, year), number, defaultDate (없을시 오늘날짜)
Common.DateAdd = function (dateType, number, defaultDate) {
    var setDate;
    var Month, Day, Year;

    if (defaultDate && defaultDate != "") {
        defaultDate = Common.GetDate(defaultDate, "yyyymmdd");
        var year, month, day;
        year = defaultDate.substring(0, 4);
        month = defaultDate.substring(4, 6);
        day = defaultDate.substring(6, 8);

        setDate = new Date(month + "/" + day + "/" + year);
    } else {
        setDate = new Date();
    }

    if (dateType.toLowerCase() == "day") {
        setDate = setDate.addDays(number);
    } else if (dateType.toLowerCase() == "month") {
        setDate = setDate.addMonth(number);
    } else if (dateType.toLowerCase() == "year") {
        setDate = setDate.addYear(number);
    }

    var Day = setDate.getDate();
    var Month = setDate.getMonth() + 1;
    var Year = setDate.getFullYear();
    Month = "0" + Month.toString();
    if (Month.length == 3)
        Month = Month.substring(1, 3);
    Day = "0" + Day.toString();
    if (Day.length == 3)
        Day = Day.substring(1, 3);

    if (defaultDate == "yyyyMM" || defaultDate == "yyyymm")
        return Year + Month;
    else
        return Year + Month + Day;
}

// 두 날짜 사이 이수 계산
Common.DateCompare = function (startDate, endDate) {
    var startDateArr = startDate.split('.');
    var endDateArr = endDate.split('.');

    var stDate = new Date(startDateArr[0], startDateArr[1], startDateArr[2]);
    var endDate = new Date(endDateArr[0], endDateArr[1], endDateArr[2]);

    var btMs = endDate.getTime() - stDate.getTime();
    var btDay = btMs / (1000 * 60 * 60 * 24);

    return btDay;
}

//마우스 오른쪽 버튼 이벤트
Common.MouseDownRight = function (e) {
    var isRightMB;
    e = e || window.event;

    if ("which" in e)  // Gecko (Firefox), WebKit (Safari/Chrome) & Opera
        isRightMB = e.which == 3;
    else if ("button" in e)  // IE, Opera 
        isRightMB = e.button == 2;

    if (isRightMB == true) {
        return true;
    } else {
        return false;
    }
}

//값체크
//strValue(string) : 원본문자열, strCheck(string) : 체크문자열
Common.Contains = function (strValue, strCheck) {
    if (strValue == undefined || strValue == null)
        return false;

    if (strValue.indexOf(strCheck) == -1)
        return false;
    else
        return true;
}

//형식변환
Common.Convert = {
    Bool: function (value) {
        if (!value) return false;

        var RtnValue = false;

        if (value == 1 || value == "1") {
            RtnValue = true;
        } else if (value != "" && value.toLowerCase() == "true") {
            RtnValue = true;
        } else {
            RtnValue = false;
        }

        return RtnValue;
    },
    Int: function (value, DefValue) {
        if (isNaN(value) == true) value = 0;
        if (DefValue == undefined) DefValue = 0;
        if (value == "")
            value = DefValue;

        try {
            if (value == null) return DefValue;
            value = value.toString();

            if (value.indexOf(",") > -1) {
                value = value.replaceAll(",", "");
            }

            if (value.indexOf(".") == -1) {
                return parseInt(value, 10);
            } else {
                var Tmp = value.split(".");
                return parseInt(Tmp[0], 10);
            }
        } catch (e) {
            return DefValue;
        }
    },
    FileSize: function (bytes) {
        bytes = Common.Convert.Int(bytes);

        var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];

        if (bytes == 0) return '0MB';

        var round = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));

        if (round == 0) return bytes + ' ' + sizes[round];
        return (bytes / Math.pow(1024, round)).toFixed(1) + ' ' + sizes[round];
    },
    IntToRGB: function (value) {
        return "#" + ('000000' + (Common.Convert.Int(value) & 0xFFFFFF).toString(16)).slice(-6);
    },
    DecimalToHex: function (number) {
        if (number < 0) {
            number = 0xFFFFFFFF + number + 1;
        }
        return number.toString(16).toUpperCase();
    }
};

//EnterKey 체크
Common.Enter = function (e, textareaCheck) {
    textareaCheck = (textareaCheck || false);

    var EventObj;
    // Event Setting
    if (window.event) {
        EventObj = window.event;
    } else {
        EventObj = e;
    }

    if (EventObj.keyCode == 13 && (textareaCheck == false || (textareaCheck == true && EventObj.srcElement.tagName != "TEXTAREA"))) {
        return true;
    } else {
        return false;
    }
}

// 키code 리턴
Common.KeyCode = function (e) {
    var keyCode;

    if (window.event) {
        keyCode = window.event.keyCode;
    } else {
        keyCode = e.which || e.keyCode;
    }

    return keyCode;
}

// check box checked
Common.Checked = function (Obj, AreaObj, Name) {
    var CheckObj = null;
    if (AreaObj == undefined) {
        AreaObj = $("body");
    }

    if (Name == undefined) {
        CheckObj = AreaObj.find("input:checkbox");
    } else {
        CheckObj = AreaObj.find("input:checkbox[name=" + Name + "]");
    }

    if (Obj.is(":checked") == true) {
        CheckObj.prop("checked", true);
    } else {
        CheckObj.prop("checked", false);
    }
}

// Property 추가
Common.AddProperty = function (obj, name, onGet, onSet) {
    // wrapper functions
    var oldValue = obj[name],
        getFn = function () {
            return onGet.apply(obj, [oldValue]);
        },
        setFn = function (newValue) {
            return oldValue = onSet.apply(obj, [newValue]);
        };

    // Modern browsers, IE9+, and IE8 (must be a DOM object),
    if (Object.defineProperty) {

        Object.defineProperty(obj, name, {
            get: getFn,
            set: setFn
        });

        // Older Mozilla
    } else if (obj.__defineGetter__) {

        obj.__defineGetter__(name, getFn);
        obj.__defineSetter__(name, setFn);

        // IE6-7
        // must be a real DOM object (to have attachEvent) and must be attached to document (for onpropertychange to fire)
    } else {
        var onPropertyChange = function (e) {
            if (event.propertyName == name) {
                // temporarily remove the event so it doesn't fire again and create a loop
                obj.detachEvent("onpropertychange", onPropertyChange);

                // get the changed value, run it through the set function
                var newValue = setFn(obj[name]);

                // restore the get function
                //obj[name] = getFn;
                //obj[name].toString = getFn;

                // restore the event
                obj.attachEvent("onpropertychange", onPropertyChange);
            }
        };

        obj[name] = onGet;
        obj[name].toString = onGet;

        obj.attachEvent("onpropertychange", onPropertyChange);
    }
}

Common.ExcelGetTableHtml = function (tableHtml) {
    var tab_text = "<table border='1px'><tr bgcolor='#87AFC6'>";
    var textRange; var j = 0;

    var tableObj = $(tableHtml);
    tableObj.find("tr").each(function () {
        if ($(this).css("display") == "none") {
            $(this).remove();
        }
    });

    tableObj.find("th[excel=false]").remove();
    tableObj.find("td[excel=false]").remove();

    tab = tableObj.get(0)

    for (j = 0; j < tab.rows.length; j++) {
        tab_text = tab_text + tab.rows[j].innerHTML + "</tr>";
    }

    tab_text = tab_text + "</table>";
    tab_text = tab_text.replace(/<A[^>]*>|<\/A>/g, "");//remove if u want links in your table
    tab_text = tab_text.replace(/<img[^>]*>/gi, ""); // remove if u want images in your table
    tab_text = tab_text.replace(/<input[^>]*>|<\/input>/gi, ""); // reomves input params

    return tab_text;
}

Common.ExcelDownload = (function () {
    var uri = 'data:application/vnd.ms-excel;base64,'
        , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--><meta http-equiv="content-type" content="text/plain; charset=UTF-8"/></head><body>{table}</body></html>'
        , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
        , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) }

    return function (tableHtml, name, htmlMode) {
        var tab_text;
        if (htmlMode == true) {
            tab_text = tableHtml;
        } else {
            tab_text = Common.ExcelGetTableHtml(tableHtml);
        }

        if (tab_text.length > 1400000 || htmlMode == true) {
            // 용량이 클경우는 서버에 넘겨서 다운로드 하도록 처리
            var formObj = $("<form id='scriptexceldownformtmp' method='post' action='/ExcelDownload' style='display:none'><input type='hidden' name='tableHtml'><input type='hidden' name='filename'></form>");
            if ($("form[id=scriptexceldownformtmp]").length > 0) {
                $("form[id=scriptexceldownformtmp]").remove();
            }

            var htmlObj = $(tab_text);
            htmlObj.find("th").each(function () {
                $(this).css("border", "1px solid #2f4050");
                $(this).css("background-color", "#F5F5F6");
            });
            htmlObj.find("td").each(function () {
                $(this).css("border", "1px solid #2f4050");
            });

            formObj.find("input[name=tableHtml]").val(htmlObj.outerHTML());
            formObj.find("input[name=filename]").val(name);
            $('body').append(formObj);
            $("#scriptexceldownformtmp").submit();
            $("#scriptexceldownformtmp").remove();
        } else {
            var ua = window.navigator.userAgent;
            var msie = ua.indexOf("MSIE ");

            if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
                if (typeof Blob !== "undefined") {
                    tab_text = [tab_text];
                    var blob1 = new Blob(tab_text, { type: "text/html" });
                    window.navigator.msSaveBlob(blob1, name);
                } else {
                    if ($("iframe[id=scriptexceldowniframetmp]").length == 0) {
                        $('body').append("<iframe id='scriptexceldowniframetmp' style='display:none'></iframe>");
                    }
                    // If Internet Explorer
                    scriptexceldowniframetmp.document.open("txt/html", "replace");
                    scriptexceldowniframetmp.document.write(tab_text);
                    scriptexceldowniframetmp.document.close();
                    scriptexceldowniframetmp.focus();
                    sa = scriptexceldowniframetmp.document.execCommand("SaveAs", true, name);
                }
            } else {
                var ctx = { worksheet: name || 'Worksheet', table: tab_text }
                var link = document.createElement("a");
                link.download = name;
                link.href = uri + base64(format(template, ctx));
                link.click();
            }
        }
    }
})();

// 빈값여부
Common.IsEmpty = function (data) {
    if (typeof (data) === 'object') {
        if (JSON.stringify(data) === '{}' || JSON.stringify(data) === '[]') {
            return true;
        } else if (!data) {
            return true;
        }
        return false;
    } else if (typeof (data) === 'string') {
        if (!data.trim()) {
            return true;
        }
        return false;
    } else if (typeof (data) === 'undefined') {
        return true;
    } else if (isNaN(data) == true) {
        return true;
    } else if (data === 0) {
        return true;
    } else {
        return false;
    }
}

//===========================================================================================
//제목 : Ajax
//===========================================================================================
//Param : [{ Key: key, Value: value }, { Key: key, Value: value }] 형태의 Default로 무조건 들어가는 값
Common.RequestInfo = function (Param) {
    var thisObj = this;
    var defaultParam;
    this.Parameters = [];
    this.formData = null;

    this.Default = function () {
        if (this.Parameters.length == 0 && defaultParam) {
            for (var i = 0; i < defaultParam.length; i++) {
                var dataInfo = defaultParam[i];
                this.Parameters[i] = { Key: dataInfo.Key, Value: dataInfo.Value };
            }
        }
    };

    this.AddParameter = function (key, value, addCheck) {
        if (typeof value == "undefined" && typeof key == "object" && key != null && key.find) {
            var NoSearchObj = null;
            if (key.attr("NoSearchParam") != undefined && $("#" + key.attr("NoSearchParam")).length > 0) {
                NoSearchObj = $("#" + key.attr("NoSearchParam")).find("input,textarea,select");
            }
            key.find("input,textarea,select").not(NoSearchObj).each(function (n) {
                if ($(this).attr("name") && $(this).attr("name") != "") {
                    if ($(this)[0].tagName == "INPUT" && $(this).attr("type") && $(this).attr("type").toLowerCase() == "radio") {
                        if ($(this).prop("checked") == true) {
                            thisObj.AddParameter($(this).attr("name"), $(this).val());
                        }
                    } else if ($(this)[0].tagName == "INPUT" && $(this).attr("type") && $(this).attr("type").toLowerCase() == "checkbox") {
                        if ($(this).prop("checked") == true) {
                            var tmpValue = thisObj.GetParameter($(this).attr("name"));
                            if (tmpValue == "") {
                                thisObj.AddParameter($(this).attr("name"), $(this).val());
                            } else {
                                if ($(this).attr("multiNumber") != undefined) {
                                    thisObj.AddParameter($(this).attr("name"), Common.Convert.Int(tmpValue) + Common.Convert.Int($(this).val()));
                                } else {
                                    thisObj.AddParameter($(this).attr("name"), tmpValue + "," + $(this).val());
                                }
                            }
                        }
                    } else if ($(this)[0].tagName == "INPUT" && $(this).attr("Code") && $(this).attr("OldCode")) {
                        thisObj.AddParameter($(this).attr("name"), $(this).attr("Code"), true);
                    } else if ($(this)[0].tagName == "INPUT" && $(this).attr("type") && $(this).attr("type").toLowerCase() == "file" && thisObj.formData != null) {
                        if ($(this).val() == "" && $(this).attr("dragFile") != undefined) {
                            thisObj.formData.append($(this).attr("name"), Common.RequestInfo.Files[$(this).attr("dragFile")]);
                        } else if ($(this).val() == "" && $(this).attr("dragFiles") != undefined) {
                            var file_arr = Common.RequestInfo.Files[$(this).attr("dragFiles")];
                            for (var Idx in file_arr) {
                                if (file_arr[Idx]) {
                                    thisObj.formData.append($(this).attr("uploadFileName"), file_arr[Idx].file);
                                }
                            }
                        } else {
                            thisObj.formData.append($(this).attr("name"), $(this)[0].files[0]);
                        }
                    } else if ($(this)[0].tagName == "SELECT" && $(this).attr("multiple") && $(this).attr("multiple") == "multiple") {
                        thisObj.AddParameter($(this).attr("name"), $(this).val().toString(), true);
                    } else {
                        thisObj.AddParameter($(this).attr("name"), $(this).val(), true);
                    }
                }
            });
        } else {
            value = (value || "");
            var dataParametersLength = this.Parameters.length;
            var exists = false;
            for (var i = 0; i < dataParametersLength; i++) {
                var dataInfo = this.Parameters[i];
                if (dataInfo.Key == key) {
                    if (typeof addCheck == "undefined") {
                        this.Parameters[i] = { Key: key, Value: value };
                    } else if (addCheck == true) {
                        var tmpValue = dataInfo.Value;
                        this.Parameters[i] = { Key: key, Value: tmpValue + "," + value };
                    }
                    exists = true;
                }
            }

            if (exists == false)
                this.Parameters.push({ Key: key, Value: value });
        }
    };

    this.GetParameter = function (key) {
        var rtnParam = "1=1";
        var dataParametersLength = this.Parameters.length;
        if (typeof key == "undefined") {
            for (var i = 0; i < dataParametersLength; i++) {
                rtnParam = rtnParam + "&" + this.Parameters[i].Key + "=" + this.Parameters[i].Value;
            }
            return rtnParam;
        } else {
            rtnParam = "";
            for (var i = 0; i < dataParametersLength; i++) {
                if (this.Parameters[i].Key == key) {
                    rtnParam = this.Parameters[i].Value;
                    break;
                }
            }
            return rtnParam;
        }
    };

    this.SetParameter = function (param) {
        var params = param.split("&");
        for (var i = 0; i < params.length; i++) {
            if (params[i] != "") {
                var p = params[i].split("=");
                if (p.length == 2)
                    AddParameter(p[0], p[1]);
            }
        }
    };

    this.GetJson = function () {
        var rtnParam = {};
        var dataParametersLength = this.Parameters.length;

        for (var i = 0; i < dataParametersLength; i++) {
            if (thisObj.formData == null) {
                rtnParam[this.Parameters[i].Key] = this.Parameters[i].Value;
            } else {
                thisObj.formData.append(this.Parameters[i].Key, this.Parameters[i].Value);
            }
        }

        if (thisObj.formData == null) {
            return rtnParam;
        } else {
            return thisObj.formData;
        }
    };

    return (function () {
        defaultParam = Param;
        if (Param)
            thisObj.Default();
    })();
}
Common.RequestInfo.Files = {};

//url(string) : 실행경로, data(Common.RequestInfo) : 파라메타 데이타, callback(function) : 처리, responseType(string) : 처리내역 형태
//formMethod(string) : 전송방식, async(bool) : 시크타입, errback(function) : 에러 처리
Common.Ajax = function (url, data, callback, info) {
    if (url == undefined || url == "") {
        Common.Msg("No Url Info.");
        return;
    }

    var responseType, formMethod, async, errback, okErr;

    if (info == undefined) {
        formMethod = "POST";
    } else {
        if (info.async == undefined) async = false
        else async = info.async

        formMethod = (info.formMethod || "POST");
        responseType = (info.responseType || undefined);
        errback = (info.errback || undefined);
        okErr = (info.okErr || undefined);
    }

    async = (async || false);
    data = (data || new Common.RequestInfo());
    var Parameters;

    if (formMethod && formMethod == "GET") {
        data.formData = null;
        AType = { type: "GET", contentType: "charset=utf-8", processData: true };
        Parameters = data.GetParameter();
    } else {
        if (data.formData == null) {
            AType = {
                type: "POST", contentType: "application/x-www-form-urlencoded;charset=utf-8", processData: true
            };
        } else {
            AType = {
                type: "POST", contentType: false, processData: false
            };
        }
        Parameters = data.GetJson();
    }

    $.ajax({
        type: AType.type,
        contentType: AType.contentType,
        processData: AType.processData,
        headers: {
            "cache-control": "no-cache", "pragma": "no-cache", "requestType": "ajax"
        },
        async: async,
        cache: false,
        url: url,
        data: Parameters,
        dataType: (responseType || "text"),
        timeout: 1000 * 60 * 60 * 24, //24시간
        success: function (result, textStatus) {
            if (result == "|||[NOLOGIN]|||") {
                Common.Msg("로그인 세션이 종료되었습니다.");
                location.href = "/?move_url=" + encodeURIComponent(location.href);
                return;
            }

            var xmlData = false;
            if (responseType && responseType.toLowerCase() == "xml" && typeof result == "object" && !result.substring) xmlData = true;
            if (xmlData == false && Common.CheckErrorMsg(result, false) == false) {
                if (typeof okErr == "function") {
                    okErr(result);
                } else {
                    Common.Loading.Hide();
                    Common.CheckErrorMsg(result);
                }

                return;
            };
            if (typeof callback == "function")
                callback(result);
        },
        error: ((errback == null || errback == undefined) ? Common.AjaxError : function (XMLHttpRequest, textStatus, errorThrown) {
            Common.Loading.Hide();
            if (typeof errback == "function")
                errback(errorThrown);
        })
    });
}
Common.AjaxError = function (XMLHttpRequest, textStatus, errorThrown) {
    Common.Loading.Hide();
    if (errorThrown == "")
        return;

    try {
        if (errorThrown.indexOf("|||[ERROR]|||") != -1) {
            var mString = errorThrown.split("|||[ERROR]|||")[1];
            Common.Msg(mString);
        } else {
            Common.Msg(errorThrown);
        }
    } catch (e) {
    };
}
Common.CheckErrorMsg = function (MsgStr, ShowMsg) {
    ShowMsg = (ShowMsg == undefined ? true : ShowMsg);
    if (MsgStr != null && !MsgStr.ResultCode) {
        if (MsgStr.substring(0, 13) == "|||[ERROR]|||") {
            var mString = MsgStr.replace("|||[ERROR]|||", "");
            if (ShowMsg == true) {
                Common.Msg(mString);
            }
            return false;
        } else if (MsgStr.substring(0, 17) == "|||[CSRFERROR]|||") {
            var mString = MsgStr.replace("|||[CSRFERROR]|||", "");
            if (ShowMsg == true) {
                Common.Msg(mString, {
                    mode: "msg"
                    , html: "확인을 클릭하시면 페이지 새로고침이 실행됩니다. <br/>다시 작업을 시도해 주세요."
                    , okback: function () {
                        location.reload();
                    }
                });
            }
            return false;
        }
    } else if (!MsgStr) {
        Common.Msg("Message Is Null!");
        return false;
    }
    return true;
}


//===========================================================================================
//쿠키
//===========================================================================================
var Cookies = function (name, value, expire) {
    if (typeof value != "undefined" && typeof expire != "undefined") {
        var day = new Date();
        day.setDate(day.getDate() + expire);
        document.cookie = name + "=" + escape(value) + "; path=/; expires=" + day.toGMTString() + ";";
    } else {
        var org = document.cookie;
        var dlm = name + "=";
        var x = 0;
        var y = 0;
        var z = 0;

        while (x <= org.length) {
            y = x + dlm.length;

            if (org.substring(x, y) == dlm) {
                if ((z = org.indexOf(";", y)) == -1) {
                    z = org.length;
                }

                return org.substring(y, z);
            }

            x = org.indexOf(" ", x) + 1;

            if (x == 0) {
                break;
            }
        }

        return "";
    }
}


//===========================================================================================
//제목 : jquery GroupWare Add Function
//===========================================================================================
$.fn.outerHTML = function (s) {
    return (s)
        ? this.before(s).remove()
        : $('<p>').append(this.eq(0).clone()).html();
},
    $.fn.reverse = function () {
        return this.pushStack(this.get().reverse(), arguments);
    }
$.fn.getCursorPosition = function () {
    var el = $(this).get(0);
    var pos = 0;
    if ('selectionStart' in el) {
        pos = el.selectionStart;
    } else if ('selection' in document) {
        el.focus();
        var Sel = document.selection.createRange();
        var SelLength = document.selection.createRange().text.length;
        Sel.moveStart('character', -el.value.length);
        pos = Sel.text.length - SelLength;
    }
    return pos;
};
$.fn.setCursorPosition = function (pos) {
    this.each(function (index, elem) {

        if (elem.setSelectionRange) {
            // elem.focus();
            elem.setSelectionRange(pos, pos);
        } else if (elem.createTextRange) {
            var range = elem.createTextRange();
            range.collapse(true);
            range.moveEnd('character', pos);
            range.moveStart('character', pos);
            range.select();
        }
    });
    return this;
};


//===========================================================================================
//제목 : Common Prototype
//===========================================================================================
//Trim
String.prototype.trim = function () {
    return this.replace(/(^\s*)|(\s*$)/gi, "");
}
Number.prototype.trim = function () {
    return this.replace(/(^\s*)|(\s*$)/gi, "");
}
//Contains (값체크)
String.prototype.Contains = function (strCheck) {
    return Common.Contains(this, strCheck);
}
Number.prototype.Contains = function (strCheck) {
    return Common.Contains(this, strCheck);
}
//bool형변환
String.prototype.toBool = function () {
    return Common.Convert.Bool(this);
}
Number.prototype.toBool = function () {
    return Common.Convert.Bool(this);
}
//Int형변환
String.prototype.toInt = function (DefValue) {
    return Common.Convert.Int(this, DefValue);
}
Number.prototype.toInt = function (DefValue) {
    return Common.Convert.Int(this, DefValue);
}
//Replace All
String.prototype.replaceAll = function (org, rep) {
    var temp_str = this.trim();
    try {
        temp_str = temp_str.split(org).join(rep);
    } catch (e) { }
    return temp_str;
}
String.prototype.insert = function (index, strValue) {
    if (index > 0)
        return this.substring(0, index) + strValue + this.substring(index, this.length);
    else
        return strValue + this;
}
//Date AddMonth
Date.prototype.addYear = function (intNum) {
    var sdate = new Date(this.getTime());
    sdate.setFullYear(sdate.getFullYear() + intNum);
    return sdate;
}
//Date AddMonth
Date.prototype.addMonth = function (intNum) {
    var sdate = new Date();

    if (sdate && intNum) {
        var m, d = (sdate = new Date(+sdate)).getUTCDate()

        sdate.setUTCMonth(sdate.getUTCMonth() + intNum, 1)
        m = sdate.getUTCMonth()
        sdate.setUTCDate(d)
        if (sdate.getUTCMonth() !== m) sdate.setUTCDate(0)
    }

    return sdate;
}

//문자열 바이트 체크(utf-8도 가능)
String.prototype.byteLength = function (mode) {
    mode = (!mode) ? 'euc-kr' : mode;
    text = this;
    byte = 0;
    switch (mode) {
        case 'utf-8':
            for (byte = i = 0; char = text.charCodeAt(i++); byte += char >> 11 ? 3 : char >> 7 ? 2 : 1);
            break;

        default:
            for (byte = i = 0; char = text.charCodeAt(i++); byte += char >> 7 ? 2 : 1);

    }
    return byte
};

function addMonthsUTC(date, count) {
    if (date && count) {
        var m, d = (date = new Date(+date)).getUTCDate()

        date.setUTCMonth(date.getUTCMonth() + count, 1)
        m = date.getUTCMonth()
        date.setUTCDate(d)
        if (date.getUTCMonth() !== m) date.setUTCDate(0)
    }
    return date
}

//Date AddDays
Date.prototype.addDays = function (days) {
    return new Date(this.getTime() + days * 24 * 60 * 60 * 1000);
}

//Date Add minutes
Date.prototype.addMins = function (mins) {
    return new Date(this.getTime() + mins * 60000);
}

//Date AddDays
Date.prototype.addSeconds = function (seconds) {
    return new Date(this.getTime() + seconds * 1000);
}

String.prototype.string = function (len) { var s = '', i = 0; while (i++ < len) { s += this; } return s; };
String.prototype.zf = function (len) { return "0".string(len - this.length) + this; };
Number.prototype.zf = function (len) { return this.toString().zf(len); };

//Date Format
Date.prototype.format = function (f) {
    if (!this.valueOf()) return " ";

    var weekName = ["일요일", "월요일", "화요일", "수요일", "목요일", "금요일", "토요일"];
    var d = this;

    return f.replace(/(yyyy|yy|MM|dd|E|hh|mm|ss|a\/p)/gi, function ($1) {
        switch ($1) {
            case "yyyy": return d.getFullYear();
            case "yy": return (d.getFullYear() % 1000).zf(2);
            case "MM": return (d.getMonth() + 1).zf(2);
            case "dd": return d.getDate().zf(2);
            case "E": return weekName[d.getDay()];
            case "HH": return d.getHours().zf(2);
            case "hh": return ((h = d.getHours() % 12) ? h : 12).zf(2);
            case "mm": return d.getMinutes().zf(2);
            case "ss": return d.getSeconds().zf(2);
            case "a/p": return d.getHours() < 12 ? "오전" : "오후";
            default: return $1;
        }
    });
};

//Date Add
Date.prototype.add = function (sInterval, iNum) {
    var dTemp = this;
    if (!sInterval || iNum == 0) return dTemp;
    switch (sInterval.toLowerCase()) {
        case "ms":
            dTemp.setMilliseconds(dTemp.getMilliseconds() + iNum);
            break;
        case "s":
            dTemp.setSeconds(dTemp.getSeconds() + iNum);
            break;
        case "mi":
            dTemp.setMinutes(dTemp.getMinutes() + iNum);
            break;
        case "h":
            dTemp.setHours(dTemp.getHours() + iNum);
            break;
        case "d":
            dTemp.setDate(dTemp.getDate() + iNum);
            break;
        case "mo":
            dTemp.setMonth(dTemp.getMonth() + iNum);
            break;
        case "y":
            dTemp.setFullYear(dTemp.getFullYear() + iNum);
            break;
    }
    return dTemp;
}

//===========================================================================================
//제목 : Common Control
//===========================================================================================
Common.Controls = {
};

///////////////////////////////////////////////////////////////////////////////////////
// CheckBox
Common.Controls.Fn = new function () {
    this.CheckAll = function (Obj) {
        Obj = Obj.find("input");
        var pObj = Obj.parent().parent();

        if (Obj.prop("checked") == true) {
            pObj.find("input").each(function () {
                $(this).prop("checked", true);
                if ($(this).parent().parent().attr("class").Contains("btn-group")) {
                    $(this).parent().removeClass("btn-white");
                    $(this).parent().addClass("btn-gray");
                    $(this).parent().addClass("active");
                }
            });
        } else {
            pObj.find("input").each(function () {
                $(this).prop("checked", false);
                if ($(this).parent().parent().attr("class").Contains("btn-group")) {
                    $(this).parent().removeClass("btn-gray");
                    $(this).parent().addClass("btn-white");
                    $(this).parent().removeClass("active");
                }
            });
        }
    }
}
///////////////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////////////
// Mask Box
Common.Controls.MaskBox = new function () {
    this.MaskKeyDown = function (event, Obj) {
        if (Obj.attr("Mask") != "") {
            if (event) {
                var keyCode = Common.KeyCode(event);

                /**
                    KeyCodes:
                    backspace=8
                    tab=9
                    enter=13
                    shift=16
                    ctr=17
                    alt=18
                    space=32
                    space=32
                    left-Up-Right-Down:37-38-39-40
                    delete=46
                    number=48-57
                    a-z=65-90
                    A-Z=97-122
                    
                    **/
                if (keyCode == 8 || keyCode == 46 || keyCode == 37 || keyCode == 39) return; // 백스페이스는 예외처리       
            }

            var value = Obj.val();
            if (keyCode == 13 || keyCode == 9) {
                if (Obj.attr('MaskMode') == 'numericMandatory') {
                    var minLength = Obj.attr('minlength');
                    if (Obj.val().length < minLength) {
                        //Common.Msg("minimum length not fulfilled");
                        Obj.val('');
                        return;
                    }
                }
                var lastChar = value.slice(-1);
                if (lastChar == ':' || lastChar == '-') {
                    Obj.val(value.substring(0, value.length - 1));
                    return;
                }
            } else {
                if (Obj.attr('MaskMode') == "numericMandatory") {
                    if (!(keyCode >= 48 && keyCode <= 57) && !(keyCode == 17 || keyCode == 16) && !(keyCode >= 96 && keyCode <= 122)) {
                        event.preventDefault();
                        return;
                    }
                } else if (Obj.attr('MaskMode') == "alphaNumeric") {
                    if (Obj.attr('AsciiOnly') == 'true') {
                        if (!(keyCode >= 65 && keyCode <= 90) && !(keyCode >= 96 && keyCode <= 122)) {
                            event.preventDefault();
                            return;
                        }
                    } else {
                        if (!(keyCode >= 65 && keyCode <= 90) && !(keyCode >= 96 && keyCode <= 122) && !(keyCode >= 48 && keyCode <= 57) && !(keyCode == 32)) {
                            event.preventDefault();
                            return;
                        }
                    }
                } else if (Obj.attr('MaskMode') == "numericSpace") {
                    if (!(keyCode >= 48 && keyCode <= 57) && !(keyCode >= 96 && keyCode <= 122) && !(keyCode == 32) && !(keyCode == 17 || keyCode == 16)) {
                        event.preventDefault();
                        return;
                    }
                }
            }
            return;
        }
    }

    this.MaskKeyUp = function (event, Obj) {
        if (Obj.attr("Mask") != "") {
            if (event) {
                var keyCode = Common.KeyCode(event);

                /**
                    KeyCodes:
                    backspace=8
                    tab=9
                    enter=13
                    shift=16
                    ctr=17
                    alt=18
                    space=32
                    space=32
                    left-Up-Right-Down:37-38-39-40
                    delete=46
                    number=48-57
                    a-z=65-90
                    A-Z=97-122
                 **/
                if (keyCode == 8 || keyCode == 37 || keyCode == 39 || keyCode == 46) return; // 백스페이스는 예외처리       
            }

            var RtnValue = "";
            var value = Obj.val();
            var maskAndCaption = Obj.attr("Mask");

            if (Common.Contains(value, ":")) {
                //var ValArr = value.split(":");
                //RtnValue = ValArr[0] + ":" + ValArr[1];
                RtnValue = value;
            }

            var pos = Obj.getCursorPosition();
            RtnValue = value;

            switch (maskAndCaption) {
                case 'AA:AA':
                    Obj.attr('minlength', 5);
                    if (value.length >= 2) {
                        if (pos == 3) pos = pos + 1;
                        var count = value.split(':').length;
                        value = value.replace(/[:]|\./, '');
                        var l = value.length;
                        if (value.length > 4) {
                            l = 4;
                        }
                        RtnValue = (value.substring(0, 2) + ':' + value.substring(2, l));
                        count = RtnValue.split(':').length - count;
                        if (count > 0) {
                            pos = pos + 1;
                        }
                    }
                    break;

                case '##:##':
                    Obj.attr('minlength', 5);
                    if (value.length >= 2) {
                        if (pos == 3) pos = pos + 1;
                        var count = value.split(':').length;
                        value = value.replace(/[:]|\./, '');
                        var l = value.length;
                        if (value.length > 4) {
                            l = 4;
                        }
                        RtnValue = (value.substring(0, 2) + ':' + value.substring(2, l));
                        count = RtnValue.split(':').length - count;
                        if (count > 0) {
                            pos = pos + 1;
                        }
                    }
                    break;

                case '00:00':
                    Obj.attr('minlength', 5);
                    if (value.length >= 2) {
                        if (pos == 3) pos = pos + 1;
                        var count = value.split(':').length;
                        value = value.replace(/[:]|\./, '');
                        var l = value.length;
                        if (value.length > 4) {
                            l = 4;
                        }
                        RtnValue = (value.substring(0, 2) + ':' + value.substring(2, l));
                        count = RtnValue.split(':').length - count;
                        if (count > 0) {
                            pos = pos + 1;
                        }
                    }
                    break;

                case '00-00':
                    Obj.attr('minlength', 5);
                    if (value.length >= 2) {
                        if (pos == 3) pos = pos + 1;
                        var count = value.split('-').length;
                        value = value.replace(/[\"-]|\./, '');
                        var l = value.length;
                        if (value.length > 4) {
                            l = 4;
                        }
                        RtnValue = (value.substring(0, 2) + '-' + value.substring(2, l));
                        count = RtnValue.split('-').length - count;
                        if (count > 0) {
                            pos = pos + 1;
                        }
                    }
                    break;

                case '000-000-000':
                    Obj.attr('minlength', 11);
                    if (value.length >= 3) {
                        if (pos == 3) {
                            pos = pos + 1;
                        }
                        var count = value.split('-').length;

                        value = value.replace(/[\"-]|\./, '');
                        if (value.length >= 7) {
                            if (pos == 8) {
                                pos = pos + 1;
                            }
                            value = value.replace(/[\"-]|\./, '');
                            var l = value.length;
                            if (value.length > 9) {
                                l = 9;
                            }
                            RtnValue = (value.substring(0, 3) + '-' + value.substring(3, 6) + '-' + value.substring(6, l));
                            // to count if - was already in value and increase the position accordingly
                            count = RtnValue.split('-').length - count;
                            if (count > 0) {
                                pos = pos + count;
                            }
                        }
                        else {
                            RtnValue = (value.substring(0, 3) + '-' + value.substring(3, value.length));
                        }

                    }
                    break;

                case 'AAA-AA-AAAAA':
                    if (value.length >= 3) {
                        if (pos == 3) {
                            pos = pos + 1;
                        }
                        var count = value.split('-').length;
                        value = value.replace(/[\"-]|\./, '');
                        if (value.length >= 6) {
                            if (pos == 7) {
                                pos = pos + 1;
                            }
                            value = value.replace(/[\"-]|\./, '');
                            var l = value.length;
                            if (value.length > 10) {
                                l = 10;
                            }
                            RtnValue = (value.substring(0, 3) + '-' + value.substring(3, 5) + '-' + value.substring(5, l));
                            count = RtnValue.split('-').length - count;
                            if (count > 0) {
                                pos = pos + count;
                            }
                        }
                        else {
                            RtnValue = (value.substring(0, 3) + '-' + value.substring(3, value.length));
                            pos = pos + 1;
                        }
                    }
                    break;

                case '999-99-99999':
                    if (value.length >= 3) {
                        if (pos == 3) {
                            pos = pos + 1;
                        }
                        var count = value.split('-').length;
                        value = value.replace(/[\"-]|\./, '');
                        if (value.length >= 6) {
                            if (pos == 7) {
                                pos = pos + 1;
                            }
                            value = value.replace(/[\"-]|\./, '');
                            var l = value.length;
                            if (value.length > 10) {
                                l = 10;
                            }
                            RtnValue = (value.substring(0, 3) + '-' + value.substring(3, 5) + '-' + value.substring(5, l));
                            var spaceString = RtnValue.split(" ").length + 1;
                            // to restrict input with spaces only
                            if (spaceString >= RtnValue.length) {
                                RtnValue = '';
                            }
                            count = RtnValue.split('-').length - count;
                            if (count > 0) {
                                pos = pos + count;
                            }
                        }
                        else {
                            RtnValue = (value.substring(0, 3) + '-' + value.substring(3, value.length));
                            pos = pos + 1;
                        }
                    }

                    break;

                case '######-#######':
                    if (value.length >= 6) {
                        value = value.replace(/[\"-]|\./, '');
                        value = value.replace(/[\"-]|\./, '');
                        var l = value.length;
                        if (value.length > 13) {
                            l = 13;
                        }
                        RtnValue = (value.substring(0, 6) + '-' + value.substring(6, l));
                        var spaceString = RtnValue.split(" ").length;
                        if (spaceString >= RtnValue.length) {
                            RtnValue = '';
                        }
                        if (pos == 6) {
                            pos = pos + 1;
                        }
                    }
                    break;

                case 'AAAAAA-AAAAAAA':
                    if (value.length >= 6) {
                        value = value.replace(/[\"-]|\./, '');
                        value = value.replace(/[\"-]|\./, '');
                        var l = value.length;
                        if (value.length > 13) {
                            l = 13;
                        }
                        RtnValue = (value.substring(0, 6) + '-' + value.substring(6, l));
                        var spaceString = RtnValue.split(" ").length;
                        if (spaceString >= RtnValue.length) {
                            RtnValue = '';
                        }
                        if (pos == 6) {
                            pos = pos + 1;
                        }
                    }
                    break;

                case '999999-9999999':
                    if (value.length >= 6) {
                        value = value.replace(/[\"-]|\./, '');
                        value = value.replace(/[\"-]|\./, '');
                        var l = value.length;
                        if (value.length > 13) {
                            l = 13;
                        }
                        RtnValue = (value.substring(0, 6) + '-' + value.substring(6, l));
                        var spaceString = RtnValue.split(" ").length;
                        if (spaceString >= RtnValue.length) {
                            RtnValue = '';
                        }
                        if (pos == 6) {
                            pos = pos + 1;
                        }
                    }
                    break;

                case 'A':
                    if (value.length >= 1) {
                        RtnValue = (value.charAt(0));
                    }
                    break;

                default:
                    break;
            }

            Obj.val(RtnValue);
            Obj.setCursorPosition(pos);
        }
    }
};


//===========================================================================================
//제목 : Form
//===========================================================================================
Common.Validate = function (FormObj) {
    var RtnValue = true;

    if (FormObj == null) {
        return RtnValue;
    }

    FormObj.find("input,select,textarea").each(function (n) {
        if (typeof $(this).attr("type") != "undefined" && $(this).attr("type").toLowerCase() == "checkbox" && typeof $(this).attr("reqcheck") != "undefined") {
            if ((typeof $(this).attr("Multi") == "undefined" && $("input:checkbox[name='" + $(this).attr("name") + "']").is(":checked") == false) || (typeof $(this).attr("Multi") != "undefined" && $("input:checkbox[Multi='" + $(this).attr("Multi") + "']").is(":checked") == false)) {
                var thisObj = $(this);
                if (thisObj.attr("messages")) {
                    Common.Msg(thisObj.attr("messages"));
                } else {
                    Common.Msg("Enter the required input value.");
                }
                RtnValue = false;
                return false;
            }
        } else if (typeof $(this).attr("type") != "undefined" && $(this).attr("type").toLowerCase() == "radio" && typeof $(this).attr("reqcheck") != "undefined") {
            if ($("input[name=" + $(this).attr("name") + "]").is(":checked") == false) {
                var thisObj = $(this);
                if (thisObj.attr("messages")) {
                    Common.Msg(thisObj.attr("messages"));
                } else {
                    Common.Msg("Enter the required input value.");
                }
                RtnValue = false;
                return false;
            }
        } else if (typeof $(this).attr("reqcheck") != "undefined" && ($(this).val() == "" || $(this).val() == null)) {
            var thisObj = $(this);
            var alertMsg = "";
            if (thisObj.attr("messages")) {
                alertMsg = thisObj.attr("messages");
            } else {
                alertMsg = "Enter the required input value.";
            }

            Common.Msg(alertMsg, {
                okback: function () {
                    if (thisObj.attr("type") && thisObj.attr("type").toLowerCase() != "hidden")
                        thisObj.focus();
                }
            });
            RtnValue = false;
            return false;
        } else if (typeof $(this).attr("type") != "undefined" && $(this).attr("type") == "email" && $(this).val() != "") {
            var pattern = /^[_a-zA-Z0-9-\.\_]+@[\.a-zA-Z0-9-]+\.[a-zA-Z]+$/;
            if (pattern.exec($(this).val())) {
            } else {
                Common.Msg("잘못입력된 이메일주소 입니다.");
                RtnValue = false;
                return false;
            }
        } else if ($(this).attr("passCheck") && $(this).attr("passCheck") != "") {
            if ($(this).val() != $("#" + $(this).attr("passCheck")).val()) {
                Common.Msg("비밀번호와 비밀번호 확인정보가 다릅니다.");
                RtnValue = false;
                return false;
            }

            if (!Common.ChkPwd($.trim($(this).val()))) {
                Common.Msg("비밀번호를 확인하세요.<br>(영문,숫자를 혼합하여 10~20자 이내)");
                RtnValue = false;
                return false;
            }
        }
    });
    return RtnValue;
}

Common.ChkPwd = function (str) {
    var reg_pwd = /^.*(?=.{10,20})(?=.*[0-9])(?=.*[a-zA-Z]).*$/;
    if (!reg_pwd.test(str)) {
        return false;
    }
    return true;
}

Common.NumberKeyPress = function (Obj) {
    if (/[^0-9\-]/g.test(Obj.val())) {
        Obj.val("");
    }
}
Common.NumberCommaKeyPress = function (Obj) {
    if (/[^0-9,.\-]/g.test(Obj.val())) {
        Obj.val("");
    }
}
Common.NumCheck = function (num, Separator, DecLength) {
    if (Separator == null || Separator == undefined) {
        Separator = "";
    }
    if (DecLength == null || DecLength == undefined) {
        DecLength = 0;
    }

    var sign = "";
    num = num.toString().replaceAll(",", "");

    if (isNaN(num)) {
        //Common.Msg("숫자만 입력할 수 있습니다.");
        return 0;
    }

    if (num == 0) {
        return 0;
    }

    if (Separator == "")
        return num;

    if (num < 0) {
        num = num * (-1);
        sign = "-";
    } else {
        num = num * 1;
    }

    num = num.toString();

    var temp = "";
    var pos = Common.Convert.Int(DecLength);
    var num_len = num.length;

    if (pos == 0)
        return num;

    while (num_len > 0) {
        num_len = num_len - pos;
        if (num_len < 0) {
            pos = num_len + pos;
            num_len = 0;
        }
        temp = Separator + num.substr(num_len, pos) + temp;
    }
    return sign + temp.substr(1);
}

Common.ChkEmail = function (str) {
    var pattern = /^[_a-zA-Z0-9-\.\_]+@[\.a-zA-Z0-9-]+\.[a-zA-Z]+$/;
    if (pattern.exec(str)) {
        return true;
    } else {
        return false;
    }
}
//숫자만 입력
Common.InputNumber = function (evt) {
    var theEvent = evt || window.event;

    // Handle paste
    if (theEvent.type === 'paste') {
        key = event.clipboardData.getData('text/plain');
    } else {
        // Handle key press
        var key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
    }
    var regex = /[0-9]|\./;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}
//Input 태그 세자리 콤마
Common.InputNumberComma = function (obj) {
    obj.value = comma(uncomma(obj.value));
}
function comma(str) {
    str = String(str);
    return str.replace(/(\d)(?=(?:\d{3})+(?!\d))/g, '$1,');
}
function uncomma(str) {
    str = String(str);
    return str.replace(/[^\d]+/g, '');
}

//===========================================================================================
//제목 : DataSet
//===========================================================================================
// DataSet
Common.DataSet = {
    DataSetCreate: function (ds, TableName, ColumnSchema, Datas) {
        var tmpDs;
        if (ds) {
            tmpDs = ds;
        } else {
            tmpDs = { Tables: {} };
        }

        if (TableName && TableName != "") {
            tmpDs = Common.DataSet.DataTableCreate(tmpDs, TableName, ColumnSchema, Datas);
        } else {
            if (!tmpDs.Tables.Count) {
                Common.AddProperty(tmpDs.Tables, "Count", function () {
                    var count = 0;
                    for (var Idx in tmpDs.Tables) {
                        if (Idx == "Count" || isNaN(Idx) == false) {
                            continue
                        }
                        count++;
                    }

                    return count;
                }, function (strValue) {
                });
            }
        }

        return tmpDs;
    },

    DataTableCreate: function (ds, TableName, ColumnSchema, Datas) {
        if (!ds) {
            ds = Common.DataSet.DataSetCreate(ds);
        }

        ds.Tables[TableName] = { Rows: [] };
        ds.Tables[TableName].TableName = TableName;
        ds.Tables[TableName].Columns = [];

        if (!ds.Tables.Count) {
            Common.AddProperty(ds.Tables, "Count", function () {
                var count = 0;
                for (var Idx in ds.Tables) {
                    if (Idx == "Count" || isNaN(Idx) == false) {
                        continue
                    }
                    count++;
                }

                return count;
            }, function (strValue) {
            });
        }

        if (!ds.Tables[TableName].Rows.Count) {
            Common.AddProperty(ds.Tables[TableName].Rows, "Count", function () {
                var count = 0;
                for (var Idx in ds.Tables[TableName].Rows) {
                    count++;
                }

                return count;
            }, function (strValue) {
            });
        }

        if (!ds.Tables[TableName].Columns.Count) {
            Common.AddProperty(ds.Tables[TableName].Columns, "Count", function () {
                var count = 0;
                for (var Idx in ds.Tables[TableName].Columns)
                    count++;

                return count;
            }, function (strValue) {
            });
        }

        if (ColumnSchema && ColumnSchema != "") {
            var strColumns = ColumnSchema.split(Common.CNST_SP);

            for (var col = 0; col < strColumns.length - 1; col++) {
                var ColumnName = strColumns[col].split("||")[0];
                var ColumnType = strColumns[col].split("||")[1];
                var Exists = false;

                for (var Idx in ds.Tables[TableName].Columns) {
                    var RowColumns = ds.Tables[TableName].Columns[Idx];
                    if (RowColumns != undefined && RowColumns.ColumnName == ColumnName) {
                        Exists = true;
                        ds.Tables[TableName].Columns[Idx].ColumnType = ColumnType;
                        break;
                    }
                }

                if (Exists == false) {
                    ds.Tables[TableName].Columns.push({ ColumnName: ColumnName, ColumnType: ColumnType });
                }
            }
        }

        if (Datas && Datas != "") {
            var dtData = Datas.split(Common.CNST_SP_LF);
            var dtDataLength = dtData.length;
            var RowIndex = ds.Tables[TableName].Rows.Count;
            for (var i = 0; i < dtDataLength; i++) {
                if (!dtData[i] || dtData[i] == "")
                    continue;

                var strRowData = dtData[i].split(Common.CNST_SP);

                if (!strRowData && strRowData == "") {
                    continue;
                }


                ds.Tables[TableName].Rows.push({});

                try {
                    for (var col = 0; col < strColumns.length - 1; col++) {
                        var ColumnsObj = ds.Tables[TableName].Columns[col];

                        var dataVal = strRowData[col];
                        if (ColumnsObj.ColumnType == "int")
                            dataVal = parseInt(dataVal);
                        else if (ColumnsObj.ColumnType == "double" || ColumnsObj.ColumnType == "float" || ColumnsObj.ColumnType == "Decimal")
                            dataVal = parseFloat(dataVal);

                        ds.Tables[TableName].Rows[RowIndex][ColumnsObj.ColumnName] = dataVal;

                        if (Common.Device.IOS || Common.Device.SAFARI)
                            ds.Tables[TableName].Rows[RowIndex][col] = ds.Tables[TableName].Rows[RowIndex][ColumnsObj.ColumnName];
                        else
                            Common.DataSet.DataSetTableRowsIndexCreateExec(ds.Tables[TableName].Rows[RowIndex], col);
                    }
                } catch (e) { };

                RowIndex++;
            }
        }

        return Common.DataSet.DataSetTableIndexCreate(ds);
    },

    AddColumn: function (dt, ColumnName, ColumnType) {
        var Exists = false;
        for (var Idx in dt.Columns) {
            var RowColumns = dt.Columns[Idx];
            if (RowColumns != undefined && RowColumns.ColumnName == ColumnName) {
                Exists = true;
                dt.Columns[Idx].ColumnType = ColumnType;
                break;
            }
        }

        if (Exists == false) {
            dt.Columns.push({ ColumnName: ColumnName, ColumnType: ColumnType });
        }
    },

    AddRows: function (dt) {
        dt.Rows.push({});
    },

    AddData: function (dt, RowIndex, ColumnName, Data) {
        dt.Rows[RowIndex][ColumnName] = Data;
    },

    AddDataTable: function (ds, dt) {
        // 스키마
        var ColumnSchema = "";
        for (var col = 0; col < dt.Columns.Count; col++) {
            var ColumnName = dt.Columns[col].ColumnName;
            var ColumnType = dt.Columns[col].ColumnType;
            ColumnSchema += ColumnName + "||" + ColumnType + Common.CNST_SP;
        }

        // 데이타
        var Datas = "";
        var dtRowsCount = dt.Rows.Count;
        var dtColumnsCount = dt.Columns.Count;
        for (var i = 0; i < dtRowsCount; i++) {
            for (var col = 0; col < dtColumnsCount; col++) {
                var ColumnName = dt.Columns[col].ColumnName;
                Datas += dt.Rows[i][ColumnName] + Common.CNST_SP;
            }
            Datas += Common.CNST_SP_LF;
        }

        ds = Common.DataSet.DataTableCreate(ds, dt.TableName, ColumnSchema, Datas);
    },

    DataSetTableRowsIndexCreateExec: function (Rows, Index) {
        if (!Rows[Index]) {
            Common.AddProperty(Rows, Index, function () {
                var Count = 0;
                for (var keyName in Rows) {
                    if (keyName == "Count") continue;
                    if (Count == Index) {
                        return Rows[keyName];
                        break;
                    }
                    Count++;
                }

                return null;
            }, function (strValue) {
                var Count = 0;
                for (var keyName in Rows) {
                    if (keyName == "Count") continue;
                    if (Count == Index) {
                        Rows[keyName] = strValue;
                        break;
                    }
                    Count++;
                }
            });
        }
    },

    DataSetTableIndexCreate: function (ds) {
        if (!ds || !ds.Tables)
            return null;

        var Count = 0;
        for (var keyName in ds.Tables) {
            if (keyName == "Count") continue;
            Common.DataSet.DataSetTableIndexCreateExec(ds.Tables, Count);
            Count++;
        }
        return ds;
    },

    DataSetTableIndexCreateExec: function (dts, Index) {
        if (!dts[Index]) {
            Common.AddProperty(dts, Index, function () {
                var Count = 0;
                for (var keyName in dts) {
                    if (keyName == "Count") continue;
                    if (Count == Index) {
                        return dts[keyName];
                        break;
                    }
                    Count++;
                }

                return null;
            }, function (strValue) {
                var Count = 0;
                for (var keyName in dts) {
                    if (keyName == "Count") continue;
                    if (Count == Index) {
                        dts[keyName] = strValue;
                        break;
                    }
                    Count++;
                }
            });
        }
    },

    DataSetToString: function (ds) {
        var strReturn = "";

        try {
            for (var i = 0; i < ds.Tables.Count; i++) {
                strReturn += Common.DataSet.DataTableToString(ds.Tables[i]);
                strReturn += Common.SP_TABLE;
            }
        } catch (e) {
            strReturn = "";
        }

        return strReturn;
    },
    DataTableToString: function (dt) {
        if (!dt || !dt.TableName)
            return "";

        var strReturn = dt.TableName + Common.CNST_SP_NT;
        try {
            // 컬럼명
            for (var col = 0; col < dt.Columns.Count; col++) {
                var ColumnName = dt.Columns[col].ColumnName;
                var ColumnType = dt.Columns[col].ColumnType;
                strReturn += ColumnName + "||" + ColumnType + Common.CNST_SP;
            }
            strReturn += Common.CNST_SP_LF;

            // 데이타
            for (var i = 0; i < dt.Rows.Count; i++) {
                for (var col = 0; col < dt.Columns.Count; col++) {
                    var ColumnName = dt.Columns[col].ColumnName;
                    strReturn += dt.Rows[i][ColumnName] + Common.CNST_SP;
                }
                strReturn += Common.CNST_SP_LF;
            }
        } catch (e) {
            strReturn = "";
        }
        return strReturn;
    },
    StringToDataSet: function (strDs) {
        var ds = Common.DataSet.DataSetCreate();

        try {
            var dsTmp = strDs.split(Common.SP_TABLE);
            for (var dsi = 0; dsi < dsTmp.length; dsi++) {
                var dtStr = dsTmp[dsi];
                if (dtStr == "")
                    continue;

                var dtArray = dtStr.split(Common.CNST_SP_NT);
                var TableName = dtArray[0];

                var dtData = dtArray[1].split(Common.CNST_SP_LF);
                var ColumnSchema = dtData[0];
                var Datas = "";
                for (var dti = 1; dti < dtData.length; dti++) {
                    if (dtData[dti] && dtData[dti] != "")
                        Datas += dtData[dti] + Common.CNST_SP_LF;
                }

                ds = Common.DataSet.DataTableCreate(ds, TableName, ColumnSchema, Datas);
            }
        } catch (e) {
            ds = Common.DataSet.DataSetCreate();
        }

        return ds;
    },
    StringToDataTable: function (strDt, strTableName) {
        if (strDt == "")
            return null;

        try {
            var dtArray = strDt.split(Common.CNST_SP_NT);
            var TableName = dtArray[0];
            if (strTableName && strTableName != "")
                TableName = strTableName;

            var dtData = dtArray[1].split(Common.CNST_SP_LF);
            var ColumnSchema = dtData[0];
            var Datas = "";
            for (var i = 1; i < dtData.length; i++) {
                if (dtData[i] && dtData[i] != "")
                    Datas += dtData[i] + Common.CNST_SP_LF;
            }

            var ds = Common.DataSet.DataSetCreate(null, TableName, ColumnSchema, Datas);

            if (ds.Tables.Count > 0) {
                return ds.Tables[0];
            } else {
                return null;
            }
        } catch (e) {
            return null;
        }
    },
    // DataTable ReName
    DataTableReName: function (ds, fromTableName, toTableName) {
        if (ds.Tables[fromTableName]) {
            var strDt = Common.DataSet.DataTableToString(ds.Tables[fromTableName]);
            var dt = Common.DataSet.StringToDataTable(strDt, toTableName);

            // formTable을 지운다
            delete ds.Tables[fromTableName];

            // toTable을 지운다
            if (ds.Tables[toTableName])
                delete ds.Tables[toTableName];

            // toTable에 formTable 데이타로 새로 생성된다
            Common.DataSet.AddDataTable(ds, dt);
        }
    },
    // DataTable Copy
    DataTableCopy: function (ds, fromTableName, toTableName) {
        if (ds.Tables[fromTableName]) {
            var strDt = Common.DataSet.DataTableToString(ds.Tables[fromTableName]);
            var dt = Common.DataSet.StringToDataTable(strDt, toTableName);
            Common.DataSet.AddDataTable(ds, dt);
        }
    },
    // 테이블의 필드명 변경
    DataTableChangeDataFieldName: function (ds, tableName, columnName, toColumnName) {
        var dt = ds.Tables[tableName];
        var strDt = Common.DataSet.DataTableToString(dt);

        var dtArray = strDt.split(Common.CNST_SP_NT);
        var TableName = dtArray[0];
        var dtData = dtArray[1].split(Common.CNST_SP_LF);
        var ColumnSchema = "";

        // 컬럼명
        for (var col = 0; col < dt.Columns.Count; col++) {
            var ColumnName = dt.Columns[col].ColumnName;
            var ColumnType = dt.Columns[col].ColumnType;
            if (ColumnName == columnName)
                ColumnName = toColumnName;

            ColumnSchema += ColumnName + "||" + ColumnType + Common.CNST_SP;
        }

        var Datas = "";
        for (var i = 1; i < dtData.length; i++) {
            if (dtData[i] && dtData[i] != "")
                Datas += dtData[i] + Common.CNST_SP_LF;
        }

        var tmpDs = Common.DataSet.DataSetCreate(null, TableName, ColumnSchema, Datas);

        delete ds.Tables[tableName];
        Common.DataSet.AddDataTable(ds, tmpDs.Tables[0]);
    },
    // DataSet Delete DataTable
    DataSetDeleteDataTable: function (ds, dtName) {
        var strReturn = "";

        try {
            for (var i = 0; i < ds.Tables.Count; i++) {
                if (dtName != ds.Tables[i].TableName) {
                    strReturn += Common.DataSet.DataTableToString(ds.Tables[i]);
                    strReturn += Common.SP_TABLE;
                }
            }
        } catch (e) {
            strReturn = "";
        }

        return Common.DataSet.StringToDataSet(strReturn);
    },
    // DataTable Merge
    DataTableMerge: function (dt1, dt2, dtName) {
        var newDt = { Rows: [] };
        newDt.Rows.Count = 0;
        newDt.TableName = dtName;
        newDt.Columns = { Count: 0 };

        var MaxDtRowCount = ((dt1.Rows.Count > dt2.Rows.Count) ? dt1.Rows.Count : dt2.Rows.Count);

        // dt1 컬럼 만들어주기
        for (var i = 0; i < dt1.Columns.Count; i++) {
            var ColumnIdx = newDt.Columns.Count;
            var TmpColumn = dt1.Columns[i];
            newDt.Columns[ColumnIdx] = { ColumnName: TmpColumn.ColumnName, ColumnIdx: TmpColumn.ColumnIdx, ColumnType: TmpColumn.ColumnType };
            newDt.Columns.Count = ColumnIdx + 1;
        }

        // dt2 컬럼 만들어주기
        for (var i = 0; i < dt2.Columns.Count; i++) {
            var ColumnIdx = newDt.Columns.Count;
            var TmpColumn = dt2.Columns[i];

            // 기존 컬럼이 생성이 되었는지 체크 후 생성되었다면 업데이트한다
            var AddCheck = true;
            for (var dt1i = 0; dt1i < dt1.Columns.Count; dt1i++) {
                if (dt1.Columns[dt1i].ColumnName == TmpColumn.ColumnName) {
                    AddCheck = false;
                    ColumnIdx = dt1i;
                    break;
                }
            }

            if (AddCheck == true) {
                newDt.Columns[ColumnIdx] = { ColumnName: TmpColumn.ColumnName, ColumnIdx: TmpColumn.ColumnIdx, ColumnType: TmpColumn.ColumnType };
                newDt.Columns.Count = ColumnIdx + 1;
            } else {
                newDt.Columns[ColumnIdx] = { ColumnName: TmpColumn.ColumnName, ColumnIdx: TmpColumn.ColumnIdx, ColumnType: TmpColumn.ColumnType };
            }
        }

        for (var i = 0; i < MaxDtRowCount; i++) {
            if (!newDt.Rows[i]) {
                newDt.Rows[i] = {};
            }

            // dt1 데이타 넣어주기
            for (var dti = 0; dti < dt1.Columns.Count; dti++) {
                var TmpColumn = dt1.Columns[dti];

                if (TmpColumn && TmpColumn.ColumnName) {
                    if (dt1.Rows[i]) {
                        newDt.Rows[i][TmpColumn.ColumnName] = dt1.Rows[i][TmpColumn.ColumnName];
                    } else {
                        newDt.Rows[i][TmpColumn.ColumnName] = undefined;
                    }
                }
            }

            // dt2 데이타 넣어주기
            for (var dti = 0; dti < dt2.Columns.Count; dti++) {
                var TmpColumn = dt2.Columns[dti];

                if (dt2.Rows[i]) {
                    newDt.Rows[i][TmpColumn.ColumnName] = dt2.Rows[i][TmpColumn.ColumnName];
                } else {
                    newDt.Rows[i][TmpColumn.ColumnName] = "";
                }
            }

            newDt.Rows.Count = newDt.Rows.Count + 1;
        }

        return newDt;
    },
    // DataSet Value Check
    ContainsDataSet: function (ds, value) {
        for (var i = 0; i < ds.Tables.Count; i++) {
            if (ds.Tables[i].TableName == value)
                return true;
        }
        return false;
    },
    // DataTable Value Check
    ContainsDataTable: function (dt, index, value) {
        for (var i = 0; i < dt.Rows.Count; i++) {
            if (dt.Rows[i][index] == value)
                return true;
        }
        return false;
    },
    // Column Value Check
    ContainsColumn: function (dt, value) {
        for (var i = 0; i < dt.Columns.Count; i++) {
            if (dt.Columns[i].ColumnName == value) {
                return true;
            }
        }
        return false;
    }
}


//===========================================================================================
// 제목 : 디바이스 정보
//===========================================================================================
Common.Device = new function () {
    var ua, iphone, ipad, ios, fullscreen, lastWidth, android, safari, firefox, ieCheck, firefoxCheck;
    var agent = navigator.userAgent.toLowerCase();

    // 기기의기본 정보를 세팅관
    ua = navigator.userAgent,
        iphone = ~ua.indexOf("iPhone") || ~ua.indexOf("iPod"),
        ipad = ~ua.indexOf('iPad'),
        ios = iphone || ipad,
        android = ~ua.indexOf('Android');
    safari = ~ua.indexOf('Safari');
    firefox = ~ua.indexOf('Firefox');
    chrome = ~ua.indexOf('Chrome');

    if ((navigator.appName == 'Netscape' && navigator.userAgent.search('Trident') != -1) || (agent.indexOf("msie") != -1)) {
        ieCheck = true;
    } else {
        ieCheck = false;
    }

    if (firefox == 0) {
        firefoxCheck = false;
    } else {
        firefoxCheck = true;
    }

    this.IEVer = function () {
        var rv = -1;
        if (navigator.appName == 'Microsoft Internet Explorer') {
            var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
            if (re.exec(ua) != null)
                rv = parseFloat(RegExp.$1);
        }

        return rv;
    }

    this.IPHONE = iphone;
    this.IPAD = ipad;
    this.IOS = ios;
    this.ANDROID = android;
    this.SAFARI = safari;
    this.FIREFOX = firefoxCheck;
    this.CHROME = chrome;
    this.IE = ieCheck;
};



//===========================================================================================
// 제목 : iframe print
//===========================================================================================
Common.printIframe = function (iFrameId) {
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    var iframe = document.getElementById(iFrameId);

    if (msie !== 0) {
        iframe.contentWindow.document.execCommand('print', false, null);
    } else {
        iframe.contentWindow.print();
    }
}



//===========================================================================================
// 제목 : Pageing
//===========================================================================================
Common.Pageing = function (info) {
    var obj = null;

    var first = $("<li class='footable-page-arrow'><a href='#first' onclick='return false;'>«</a></li>");
    var prev = $("<li class='footable-page-arrow'><a href='#prev' onclick='return false;'>‹</a></li>");
    var next = $("<li class='footable-page-arrow'><a href='#next' onclick='return false;'>›</a></li>");
    var last = $("<li class='footable-page-arrow'><a href='#last' onclick='return false;'>»</a></li>");
    var total = 0;
    var now = 0;
    var pageLimit = 10;
    var listLimit = 20;
    var url = "";
    var paramName = "p";

    if (info.obj == undefined) obj = null
    else obj = info.obj

    var total = (info.total || 0);
    var now = (info.now || 0);
    var pageLimit = (info.pageLimit || 10);
    var listLimit = (info.listLimit || 20);
    var url = (info.url || "");
    var paramName = (info.paramName || "p");
    var callback = (info.callback || null);
    var params = "";

    if (url == "") {
        url = location.pathname.split("?")[0];
    }

    var reqInfo = new Common.RequestInfo();
    reqInfo.AddParameter(obj);
    params = reqInfo.GetParameter().replace("1=1", "");

    if (obj && obj.length > 0) {
        var pageObj = $("<ul class='pagination'></div>");

        var StartPage = Common.Convert.Int((now - 1) / pageLimit) * pageLimit;
        var List = 1;
        var MovePage = 0;
        var TotalPage = Common.GetSize(total, listLimit);

        if (now > 1) {
            first.bind("click", function () {
                if (callback == null) {
                    Common.MoveUrl(url + "?" + paramName + "=1" + params);
                } else {
                    callback("1");
                }
            });
        } else {
            first.addClass("disabled");
        }
        pageObj.append(first);

        if (now > pageLimit) {
            prev.bind("click", function () {
                if (callback == null) {
                    Common.MoveUrl(url + "?" + paramName + "=" + StartPage + params);
                } else {
                    callback(StartPage);
                }
            });
        } else {
            prev.addClass("disabled");
        }
        pageObj.append(prev);

        do {
            MovePage = List + StartPage;
            var numObj = $("<li class='footable-page' page='" + MovePage + "'><a href='#' onclick='return false;'>" + MovePage + "</a></li>");

            if (now == MovePage) {
                numObj.addClass("active");
            } else {
                numObj.bind("click", function () {
                    if (callback == null) {
                        Common.MoveUrl(url + "?" + paramName + "=" + $(this).attr("page") + params);
                    } else {
                        callback($(this).attr("page"));
                    }
                });
            }
            pageObj.append(numObj);

            List++;
        } while (List + StartPage <= TotalPage && List <= pageLimit);

        if (TotalPage > MovePage) {
            next.bind("click", function () {
                if (callback == null) {
                    Common.MoveUrl(url + "?" + paramName + "=" + (MovePage + 1) + params);
                } else {
                    callback((MovePage + 1));
                }
            });
        } else {
            next.addClass("disabled");
        }
        pageObj.append(next);

        if (TotalPage > 1 && now < TotalPage) {
            last.bind("click", function () {
                if (callback == null) {
                    Common.MoveUrl(url + "?" + paramName + "=" + TotalPage + params);
                } else {
                    callback(TotalPage);
                }
            });
        } else {
            last.addClass("disabled");
        }
        pageObj.append(last);

        if (total != 0) {
            obj.append(pageObj);
        }
    }
}

Common.GetSize = function (Total, Limit) {
    if (Limit == 0)
        return 0;

    return (Total % Limit == 0) ? Common.Convert.Int(Total / Limit) : Common.Convert.Int(Total / Limit + 1);
}



//===========================================================================================
//제목 : Loading
//===========================================================================================
Common.Loading = new function (op) {
    var overlay = null;
    var Option = {
        msg: "Loading", duration: 0
    };

    // 옵션 설정
    this.Options = function (OrgOption, NewOption) {
        if (NewOption) {
            for (var keyName in NewOption) {
                Option[keyName] = NewOption[keyName];
            }
        }
        return Option;
    };
    Option = this.Options(Option, op);

    this.Show = function (op) {
        Option = this.Options(Option, op);

        if (overlay != null) {
            overlay.hide();
        }

        var opts = {
            lines: 13, // The number of lines to draw
            length: 11, // The length of each line
            width: 5, // The line thickness
            radius: 17, // The radius of the inner circle
            corners: 1, // Corner roundness (0..1)
            rotate: 0, // The rotation offset
            color: '#FFF', // #rgb or #rrggbb
            speed: 1, // Rounds per second
            trail: 60, // Afterglow percentage
            shadow: false, // Whether to render a shadow
            hwaccel: false, // Whether to use hardware acceleration
            className: 'spinner', // The CSS class to assign to the spinner
            zIndex: 2e9, // The z-index (defaults to 2000000000)
            top: 'auto', // Top position relative to parent in px
            left: 'auto' // Left position relative to parent in px
        };
        var target = document.createElement("div");
        document.body.appendChild(target);
        var spinner = new Spinner(opts).spin(target);
        overlay = iosOverlay({
            text: Option.msg,
            duration: Option.duration,
            spinner: spinner
        });
    };

    this.Hide = function (Mode, Msg) {
        if (Mode == "Success") {
            Msg = (Msg || "성공");

            overlay.update({ icon: "/Images/check.png", text: Msg });

            window.setTimeout(function () {
                overlay.hide();
            }, 500);
        } else {
            if (overlay != null) {
                overlay.hide();
            }
        }
    };

    this.Message = function (Msg) {
        overlay.update({
            text: Msg
        });
    };
};


//===========================================================================================
// 제목 : Clipboard
//===========================================================================================
Common.CopyToClipboard = function (data) {
    data += '';
    var IE = window.external && (navigator.platform == "Win32" || (window.ScriptEngine && ScriptEngine().indexOf("InScript") + 1));
    var FF = navigator.userAgent.toLowerCase();
    var GC = (FF.indexOf("chrome") + 1) ? true : false;
    var FF = (FF.indexOf("firefox") + 1) ? true : false;
    var OP = window.opera && window.print;
    var NS = window.netscape && !OP;

    if (window.clipboardData || NS) {
        if (IE && !FF) {
            if (!window.clipboardData.setData("Text", data.replace("&amp;", "&"))) {
                return false
            }
        } else {
            try {
                netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect")
            } catch (e) {
                return false
            }

            try {
                e = Components.classes["@mozilla.org/widget/clipboard;1"].createInstance(Components.interfaces.nsIClipboard)
            } catch (e) {
                return false
            }
            try {
                b = Components.classes["@mozilla.org/widget/transferable;1"].createInstance(Components.interfaces.nsITransferable)
            } catch (e) {
                return false
            }
            b.addDataFlavor("text/unicode");
            o = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);
            o.data = data;
            b.setTransferData("text/unicode", o, data.length * 2);
            try {
                t = Components.interfaces.nsIClipboard
            } catch (e) {
                return false
            }
            e.setData(b, null, t.kGlobalClipboard)
        }
    } else {
        return false
    }
    return false;
}


///////////////////////////////////////////////////////////////////////////////////////
// Calendar
Common.Controls.pqGrid = new function () {
    this.DateInit = function (ui, grid) {
        $(ui.$editor).datepicker({
            format: "yyyy-mm-dd",
            language: "kr",
            todayBtn: "linked",
            keyboardNavigation: false,
            forceParse: false,
            calendarWeeks: true,
            autoclose: true,
            todayHighlight: true,
            toggleActive: true
        }).on("show", function (e) {
            $(".datepicker").css('z-index', 99999999999999);
            $(".datepicker").bind("click", function () {
                if ($(this).find("td.active").length > 0) {
                    var InDate = "";
                    var day = $(this).find("td.active").html();

                    $(this).find("th.datepicker-switch").each(function () {
                        var YearMonth = $(this).html();
                        if (YearMonth.Contains(" ")) {
                            var Arr = YearMonth.replace("월", "").replace("년", "").split(" ");
                            var month = Arr[1];

                            if (String(month).length == 1) { month = "0" + month; };
                            if (String(day).length == 1) { day = "0" + day; };

                            InDate = Arr[0] + "-" + month + "-" + day;
                        }
                    });

                    $(ui.$cell).find("input").val(InDate);

                    var insertData = {};
                    insertData[ui.dataIndx] = $(ui.$cell).find("input").val();

                    $("#" + grid[0].bindings[0].id).pqGrid("updateRow", {
                        rowList: [
                            { rowIndx: ui.rowIndx, newRow: insertData }
                        ]
                    });
                }
            });
        });

        $(ui.$editor).prop("readonly", true);
    }

    this.MaskTimeInit = function (ui, grid) {
        var Obj = $(ui.$editor);
        Obj.attr("Mask", "00:00");
        Obj.attr("MaskMode", "numericMandatory");

        Obj.bind("keydown", function (event) {
            Common.Controls.MaskBox.MaskKeyDown(event, $(this));
        });

        Obj.bind("keyup", function (event) {
            Common.Controls.MaskBox.MaskKeyUp(event, $(this));
        });
    }
}


/*------------------------------------------------------------

    비공개 Post 호출용

    Parameter 설명
    doc = 현재 doc
    url = 호출 url
    data = data 객체 형식 {}
    del =  기존 iframe 삭제 여부 true / false, 기본값 true

------------------------------------------------------------*/
Common.createAndPostIframe = function (doc, url, data, del = true) {

    // IFrame 요소 생성
    var iframe = document.createElement('iframe');
    iframe.name = 'hiddenFrame';
    iframe.style.display = 'none';

    // IFrame에 POST 전송 폼 추가
    var form = document.createElement('form');
    form.method = 'post';
    form.action = url;
    form.target = iframe.name;

    // 전송할 데이터 추가
    for (var key in data) {
        if (data.hasOwnProperty(key)) {
            var input = document.createElement('input');
            input.type = 'hidden';
            input.name = key;
            input.value = data[key];
            form.append(input);
        }
    }

    if (typeof del !== 'undefined' && typeof del === 'boolean' && del) {
        const iframeBeforeList = doc.body.querySelectorAll(`iframe[name="${iframe.name}"]`);
        for (const ifb of iframeBeforeList) {
            doc.body.removeChild(ifb);
        }
    }
    // IFrame 문서에 추가
    doc.body.appendChild(iframe);
    doc.body.appendChild(form);

    // 폼 전송
    form.submit();

    // IFrame 제거 (선택 사항)
    //setTimeout(function () {
        //doc.body.removeChild(iframe);
        //doc.body.removeChild(form);
    //}, 1000);
}

//------------------------------------------------------------
// 파일 다운로드
//------------------------------------------------------------
Common.FileDown = function(doc, data) {
    Common.createAndPostIframe(doc, '/Custom/DownloadFile', data);
}