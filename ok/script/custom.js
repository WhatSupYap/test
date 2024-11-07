/* 좌측 메뉴 활성화 및 페이지 세팅 */
$(document).ready(function () {
    // 네비바 이벤트
    $(".navbar-minimalize").bind("click", function () {
        if (!(Common.Device.IPHONE || Common.Device.ANDROID)) {
            if (Common.Contains($("body").attr("class"), "mini-navbar")) {
                Cookies("miniNaviBar", "Y", 365);
            } else {
                Cookies("miniNaviBar", "", 365);
            }
        }
    });

    if (location.href.toLowerCase().indexOf("/main") == -1) {
        // 페이지 이벤트 로딩
        Custom.Load();
    }
});



//===========================================================================================
// 제목 : Custom
//===========================================================================================
if (!Custom) {
    var Custom = { ActionName: "", ControllerName: "", AttachFileUrl: "" };
}

/////////////////////////////////////////////
// Page Event Setting
Custom.DatepickerSet = {
    format: "yyyy-mm-dd",
    language: "kr",
    todayBtn: "linked",
    keyboardNavigation: false,
    forceParse: false,
    calendarWeeks: true,
    autoclose: true,
    todayHighlight: true,
    toggleActive: true
}

var calDate = new Date();
calDate.setDate(calDate.getDate() - 1);

Custom.DatepickerEndDateSet = {
    format: "yyyy-mm-dd",
    language: "kr",
    todayBtn: false,
    keyboardNavigation: false,
    forceParse: false,
    calendarWeeks: true,
    autoclose: true,
    todayHighlight: false,
    toggleActive: true,
    endDate: calDate
}
Custom.DatepickerMonthSet = {
    format: "yyyy-mm",
    dateFormat: "yyyy-mm",
    startView: "year",
    minView: "year",
    minViewMode: 1,
    language: "kr",
    todayBtn: "linked",
    keyboardNavigation: false,
    forceParse: false,
    calendarWeeks: true,
    autoclose: true,
    todayHighlight: true,
    toggleActive: true
}
Custom.DatepickerYearSet = {
    format: "yyyy",
    dateFormat: "yyyy",
    startView: 2,
    minView: 2,
    minViewMode: 2,
    language: "kr",
    todayBtn: "linked",
    keyboardNavigation: false,
    forceParse: false,
    autoclose: true,
    todayHighlight: true,
    toggleActive: true
}
Custom.DatepickerShow = function (e) {
    var thisObj = $(this);
    var inputObj = null;
    if (thisObj[0].tagName.toLowerCase() == "input") {
        inputObj = thisObj;
    } else {
        inputObj = thisObj.find("input");
    }

    if (inputObj.attr("hide") != undefined) {
        thisObj.datepicker("hide");
    }

    if (inputObj.val().trim() != "") {
        thisObj.attr("openDate", inputObj.val());
        inputObj.one("keyup", function () {
            thisObj.removeAttr("openDate");
        });
    }
    $(".datepicker-days").find("tr").unbind("mouseover");
    $(".datepicker-days").find("tr").unbind("mouseout");

    if ($(".datepicker table tr td span.old").hasClass("disabled") == false) {
        $(".datepicker table tr td span.old").css("color", "#000000");
    }
    if ($(".datepicker table tr td span.new").hasClass("disabled") == false) {
        $(".datepicker table tr td span.new").css("color", "#000000");
    }

    thisObj.datepicker("place");
}
Custom.DatepickerWeekShow = function (e) {
    var thisObj = $(this);
    var inputObj = null;
    if (thisObj[0].tagName.toLowerCase() == "input") {
        inputObj = thisObj;
    } else {
        inputObj = thisObj.find("input");
    }

    if (inputObj.attr("hide") != undefined) {
        thisObj.datepicker("hide");
    }

    if (inputObj.val().trim() != "") {
        thisObj.attr("openDate", inputObj.val());
        inputObj.one("keyup", function () {
            thisObj.removeAttr("openDate");
        });
    }
    $(".datepicker-days").find(".day").parent().bind("mouseover", function () {
        $(this).css("background-color", "#eee");
    });
    $(".datepicker-days").find(".day").parent().bind("mouseout", function () {
        $(this).css("background-color", "transparent");
    });
}
Custom.DatepickerHide = function (e) {
    var changeEventExcute = false;
    var thisObj = $(this);
    var inputObj = null;
    if (thisObj[0].tagName.toLowerCase() == "input") {
        inputObj = thisObj;
    } else {
        inputObj = thisObj.find("input");
    }

    if (inputObj.val().trim() == "" && thisObj.attr("openDate") != undefined) {
        inputObj.val(thisObj.attr("openDate"));
    }

    if (thisObj.attr("openDate") == undefined || (thisObj.attr("openDate") != undefined && inputObj.val() != thisObj.attr("openDate"))) {
        changeEventExcute = true;
    }

    if (!(inputObj.attr("dateFormat") == undefined && inputObj.attr("dateFormat") == null || inputObj.attr("dateFormat") == "")) {
        var value = inputObj.val();
        if (inputObj.attr("dateFormat") == "yyyy-mm") {
            if (value.length > 7) {
                inputObj.val(e.date.format("yyyy") + "." + e.date.format("MM"));
            }
        } else if (inputObj.attr("dateFormat") == "yyyy") {
            if (value.length > 4) {
                inputObj.val(e.date.format("yyyy"));
            }
        }
    }
    if (changeEventExcute && inputObj.data("changeEvent") != undefined) {
        inputObj.data("changeEvent")();
    }
}
Custom.Load = function (Obj) {
    Obj = (Obj || $("body"));

    try {
        Obj.find('input[NumberKey]').each(function () {
            var thisObj = $(this);

            $(this).bind("keyup", function (e) {
                Common.NumberKeyPress(thisObj);
            });
        });

        Obj.find('.table-fixed').each(function () {
            var thisObj = $(this);
            var theadObj = $(this).clone();
            theadObj.attr("id", Common.GetTodayTimeString() + "_title");
            theadObj.find("tbody").remove();
            theadObj.css("position", "absolute");
            theadObj.css("top", thisObj.offset().top + "px");
            thisObj.before(theadObj)
            theadObj.css("width", thisObj.css("width"));
            thisObj.find("tbody").find("tr").first().children().each(function (n) {
                theadObj.find("th:nth-child(" + (n + 1) + ")").width($(this).width());
            });

            $(window).resize(function () {
                theadObj.css("width", thisObj.css("width"));
                thisObj.find("tbody").find("tr").first().children().each(function (n) {
                    theadObj.find("th:nth-child(" + (n + 1) + ")").width($(this).width());
                });
            });
        });

        Obj.find('textarea[editerMode=true]').each(function () {
            var thisObj = $(this);
            $(this).summernote({
                lang: 'ko-KR',
                toolbar: [
                    // [groupName, [list of button]]
                    ['style', ['bold', 'italic', 'underline']],
                    ['fontname', ['fontname']],
                    ['fontsize', ['fontsize']],
                    ['color', ['color']],
                    ['para', ['ul', 'ol', 'paragraph']],
                    ['height', ['height']],
                    ['codeview', ['codeview']],
                    ['misc', ['undo', 'redo']]
                ],
                height: $(this).css("height"),
                dialogsInBody: true,
                disableDragAndDrop: false,
                fontNames: ['돋움', '돋움체', '굴림', '굴림체', '바탕', '바탕체', '궁서', 'Arial', 'Tahoma', 'Times New Roman', 'Verdana'],
                fontNamesIgnoreCheck: ['돋움'],
                fontName: '돋움',
                callbacks: {
                    onEnter: function (e) {
                        // You may replace `c` with whatever key you want
                        if ((e.metaKey || e.ctrlKey) && e.which == 13) {
                            Common.EventCancelBubble(e);
                            Common.EventReturnValue(e);
                        }
                    }
                }
            });

            thisObj.data("Text", function (text) {
                setTimeout(function () {
                    thisObj.next().find(".panel-body").html(text);
                }, 100);
                thisObj.val(text);
            });
        });

        // 숫자 입력 폼
        Obj.find('.touchspin1').each(function () {
            var thisObj = $(this);
            var step = 1;
            var min = 0;
            var max = 0;
            var initval = $(this).val();
            var options = {};

            options.buttondown_class = "btn btn-white";
            options.buttonup_class = "btn btn-white";
            options.step = step;
            options.initval = initval;

            if ($(this).attr("btnClass") != undefined) {
                options.buttondown_class = "btn" + $(this).attr("btnClass");
                options.buttonup_class = "btn" + $(this).attr("btnClass");
            }
            if ($(this).attr("step") != undefined) {
                options.step = Common.Convert.Int($(this).attr("step"), 1);
            }
            if ($(this).attr("min") != undefined) {
                options.min = Common.Convert.Int($(this).attr("min"));
            }
            if ($(this).attr("max") != undefined) {
                options.max = Common.Convert.Int($(this).attr("max"), 1000);
            }

            if ($(this).attr("default") != undefined) {
                if (initval == "") {
                    options.initval = Common.Convert.Int($(this).attr("default"));
                }
            }

            $(this).TouchSpin(options);

            $(this).on("touchspin.on.stopspin", function () {
                if (thisObj.attr("eventTarget") != undefined) {
                    $("#" + thisObj.attr("eventTarget")).click();
                }
                if (thisObj.attr("eventFunction") != undefined) {
                    window[thisObj.attr("eventFunction")](thisObj);
                }
            });
        });

        Obj.find('.touchspin2').each(function () {
            var thisObj = $(this);
            var step = 1;
            var min = 0;
            var max = 0;
            var initval = $(this).val();
            var options = {};

            options.buttondown_class = "btn btn-white";
            options.buttonup_class = "btn btn-white";
            options.step = step;
            options.initval = initval;

            if ($(this).attr("btnClass") != undefined) {
                options.buttondown_class = "btn" + $(this).attr("btnClass");
                options.buttonup_class = "btn" + $(this).attr("btnClass");
            }
            if ($(this).attr("step") != undefined) {
                options.step = Common.Convert.Int($(this).attr("step"), 1);
            }
            if ($(this).attr("min") != undefined) {
                options.min = Common.Convert.Int($(this).attr("min"));
            }
            if ($(this).attr("max") != undefined) {
                options.max = Common.Convert.Int($(this).attr("max"), 1000);
            }
            if ($(this).attr("postfix") != undefined) {
                options.postfix = $(this).attr("postfix");
            }

            if ($(this).attr("default") != undefined) {
                if (initval == "") {
                    options.initval = Common.Convert.Int($(this).attr("default"));
                }
            }

            $(this).TouchSpin(options);

            $(this).on("touchspin.on.stopspin", function () {
                if (thisObj.attr("eventTarget") != undefined) {
                    $("#" + thisObj.attr("eventTarget")).click();
                }
                if (thisObj.attr("eventFunction") != undefined) {
                    window[thisObj.attr("eventFunction")](thisObj);
                }
            });
        });

        // 별점 기능 추가
        Obj.find('.starrr').each(function () {
            var __slice = [].slice;
            (function (e, t) {
                var n;
                n = function () {
                    function t(t, n) {
                        var r, i, s, o = this;
                        this.options = e.extend({}, this.defaults, n);
                        this.$el = t;
                        s = this.defaults;
                        for (r in s) {
                            i = s[r];
                            if (this.$el.data(r) != null) {
                                this.options[r] = this.$el.data(r)
                            }
                        }
                        this.createStars();
                        this.syncRating();
                        this.$el.on("mouseover.starrr", "span", function (e) {
                            return o.syncRating(o.$el.find("span").index(e.currentTarget) + 1)
                        });
                        this.$el.on("mouseout.starrr", function () {
                            return o.syncRating()
                        });
                        this.$el.on("click.starrr", "span", function (e) {
                            return o.setRating(o.$el.find("span").index(e.currentTarget) + 1)
                        });
                        this.$el.on("starrr:change", this.options.change)
                    }
                    t.prototype.defaults = {
                        rating: void 0, numStars: 5, change: function (e, t) { }
                    };
                    t.prototype.createStars = function () {
                        var e, t, n; n = [];
                        for (e = 1, t = this.options.numStars; 1 <= t ? e <= t : e >= t; 1 <= t ? e++ : e--) {
                            n.push(this.$el.append("<span class='glyphicon .glyphicon-star-empty' style='cursor:pointer;'></span>"))
                        }
                        return n
                    };
                    t.prototype.setRating = function (e) {
                        if (this.options.rating === e) { e = void 0 }
                        this.options.rating = e;
                        this.syncRating();
                        return this.$el.trigger("starrr:change", e)
                    };
                    t.prototype.syncRating = function (e) {
                        var t, n, r, i; e || (e = this.options.rating);
                        if (e) {
                            for (t = n = 0, i = e - 1; 0 <= i ? n <= i : n >= i; t = 0 <= i ? ++n : --n) {
                                this.$el.find("span").eq(t).removeClass("glyphicon-star-empty").addClass("glyphicon-star")
                            }
                        }
                        if (e && e < 5) {
                            for (t = r = e; e <= 4 ? r <= 4 : r >= 4; t = e <= 4 ? ++r : --r) {
                                this.$el.find("span").eq(t).removeClass("glyphicon-star").addClass("glyphicon-star-empty")
                            }
                        }
                        if (!e) {
                            return this.$el.find("span").removeClass("glyphicon-star").addClass("glyphicon-star-empty")
                        }
                    };
                    return t
                }();
                return e.fn.extend({
                    starrr: function () {
                        var t, r;
                        r = arguments[0], t = 2 <= arguments.length ? __slice.call(arguments, 1) : [];
                        return this.each(function () {
                            var i;
                            i = e(this).data("star-rating");
                            if (!i) {
                                e(this).data("star-rating", i = new n(e(this), r))
                            }
                            if (typeof r === "string") {
                                return i[r].apply(i, t)
                            }
                        })
                    }
                })
            })(window.jQuery, window); $(function () { return $(".starrr").starrr() })

            $(this).on('starrr:change', function (e, value) {
                if ($(this).attr("data_target") != undefined) {
                    $("#" + $(this).attr("data_target")).val(value);
                }
            });
        });

        // Buttons 기능 변경 추가
        if (Obj.find('div[data-toggle=buttons]').length > 0) {
            Obj.find('div[data-toggle=buttons]').find("label").bind("change", function () {
                if ($(this).find("input").attr("type").toLowerCase() == "radio") {
                    $(this).parent().find("label").removeClass("btn-gray");
                    $(this).parent().find("label").addClass("btn-white");
                }

                $(this).removeClass("btn-white");
                $(this).removeClass("btn-gray");

                if (Common.Contains($(this).attr("class"), "active")) {
                    $(this).addClass("btn-gray");
                } else {
                    $(this).addClass("btn-white");
                }
            });
        }

        Obj.find('select[comboMode=select2]').each(function () {
            if (Common.Device.IOS || Common.Device.ANDROID) {
                return;
            }

            var default_value = $(this).attr("default_value");

            if ($(this).attr("multiple") != undefined) {
                if (default_value != "") {
                    try {
                        var default_value_arr = default_value.split(",");
                        $(this).val(default_value_arr);
                    } catch (e) {
                    }
                }
            } else {
                if (default_value != "") {
                    $(this).val(default_value);
                }
            }

            var tObj = $(this);
            var select2Obj = null;

            if ($(".modal").length == 0) {
                select2Obj = tObj.select2();
            } else {
                if (Common.Device.IE || Common.Device.FIREFOX) {
                    select2Obj = tObj.select2({
                        dropdownParent: Obj
                        , width: null //select2 to take on width of parent
                    });
                } else {
                    select2Obj = tObj.select2();
                }
            }

            $(".select2-selection--multiple").css("border-radius", "0px");
            $(this).css("width", "10px");

            tObj.data("SetValue", function (value) {
                select2Obj.val(value).trigger("change");
            });

            tObj.data("Reset", function () {
                if ($(".modal").length == 0) {
                    select2Obj = tObj.select2();
                } else {
                    if (Common.Device.IE || Common.Device.FIREFOX) {
                        select2Obj = tObj.select2({
                            dropdownParent: Obj
                            , width: null //select2 to take on width of parent
                        });
                    } else {
                        select2Obj = tObj.select2();
                    }
                }
            });
        });

        // DatePicker (기본)
        var GetDateKeyOnlyNum = function (val) {
            var num = val;
            var tmp = "";

            for (var i = 0; i < num.length; i++) {
                if (num.charAt(i) >= 0 && num.charAt(i) <= 9)
                    tmp = tmp + num.charAt(i);
                else
                    continue;
            }
            return tmp;
        }
        var GetDateKeyUp = function (e, Obj) {
            var EventObj;
            if (window.event) {
                EventObj = window.event;
            } else {
                EventObj = e;
            }

            if (EventObj.keyCode == 46) {
                Obj.val("");
                return;
            }

            var str = GetDateKeyOnlyNum(Obj.val());
            var leng = str.length;

            switch (leng) {
                case 1:
                case 2:
                case 3:
                case 4: Obj.val(str); break;
                case 5:
                case 6: Obj.val(str.substring(0, 4) + "-" + str.substring(4)); break;
                case 7:
                case 8: Obj.val(str.substring(0, 4) + "-" + str.substring(4, 6) + "-" + str.substring(6)); break;
            }
        }
        var GetDateKeyRetrun = function (Obj) {
            var vDate = new Date();
            var dYear = vDate.getFullYear().toString();
            var dMonth = "";
            var dDate = "";

            dMonth = "0" + (vDate.getMonth() + 1).toString();
            if (dMonth.length == 3)
                dMonth = dMonth.substring(1, 3);

            dDate = "0" + vDate.getDate().toString();
            if (dDate.length == 3)
                dDate = dDate.substring(1, 3);

            var inputText = Obj.val();
            if (inputText.length == 1) {
                Obj.val(dYear + "-" + dMonth + "-0" + inputText);
            } else if (inputText.length == 2) {
                Obj.val(dYear + "-" + dMonth + "-" + inputText);
            } else if (inputText.length == 3) {
                var firstText = inputText.substring(0, 1);
                var lastText = inputText.substring(1, 3);
                Obj.val(dYear + "-0" + firstText + "-" + lastText);
            } else if (inputText.length == 4) {
                var firstText = inputText.substring(0, 2);
                if (firstText == "20" || firstText == "19") {
                    Obj.val(inputText);
                } else {
                    var lastText = inputText.substring(2, 4);
                    Obj.val(dYear + "-" + firstText + "-" + lastText);
                }
            } else if (inputText.length == 7) {
                var arr = inputText.split("-");
                if (arr.length == 2) {
                    if (arr[0].length == 4 && arr[1].length == 2) {
                        Obj.val(inputText);
                    } else {
                        Obj.val(dYear + "-" + dMonth + "-" + dDate);
                    }
                }
            }
        }

        var GetDateCheckDate = function (Obj) {
            var str = Obj.val().replace(".", "").replace(".", "").replace("-", "").replace("-", "").trim();

            if (str.length == 8) {
                var vDate = new Date();
                var nYear = parseInt(str.substring(0, 4), 10);
                var nMonth = parseInt(str.substring(4, 6), 10) - 1;
                var nDate = parseInt(str.substring(6), 10);

                vDate.setFullYear(nYear);
                vDate.setMonth(nMonth);
                vDate.setDate(nDate);

                if (vDate.getFullYear() != nYear ||
                    vDate.getMonth() != nMonth ||
                    vDate.getDate() != nDate) {
                    return false;
                }
                return true;
            } else if (str.length == 4) {
                var inputText = Obj.val();
                var firstText = inputText.substring(0, 2);
                if (firstText == "20" || firstText == "19") {
                    return true;
                } else {
                    return false;
                }
            } else if (str.length == 6) {
                var inputText = Obj.val();
                var arr = inputText.split(".");
                if (arr.length == 2) {
                    if (arr[0].length == 4 && arr[1].length == 2) {
                        return true;
                    } else {
                        return false;
                    }
                }
            } else {
                return false;
            }
        }
        Obj.find('.input-group.date').not("[dateMode]").datepicker(Custom.DatepickerSet).on("show", Custom.DatepickerShow).on("hide", Custom.DatepickerHide);
        Obj.find('.input-group.date').not("[dateMode]").find("input").each(function () {
            var thisObj = $(this);
            $(this).data("SetValue", function (value) {
                thisObj.val(value);
                thisObj.parent().datepicker(Custom.DatepickerSet).datepicker("update").on("show", Custom.DatepickerShow).on("hide", Custom.DatepickerHide);
            });

            $(this).bind("keypress", function (e) {
                $('.datepicker').hide();
                if (Common.Enter(e) == true) {
                    thisObj.blur();
                }
            });

            $(this).bind("keyup", function (e) {
                GetDateKeyUp(e, thisObj);
            });

            $(this).bind("blur", function (e) {
                GetDateKeyRetrun(thisObj);
                if (thisObj.val() != "" && GetDateCheckDate(thisObj) == false) {
                    thisObj.val("");
                }
            });
        });

        Obj.find('.input-group.date[dateMode=endDateLimit]').datepicker(Custom.DatepickerEndDateSet).on("show", Custom.DatepickerShow).on("hide", Custom.DatepickerHide);
        Obj.find('.input-group.date[dateMode=endDateLimit]').find("input").each(function () {
            var thisObj = $(this);
            $(this).data("SetValue", function (value) {
                thisObj.val(value);
                thisObj.parent().datepicker(Custom.DatepickerEndDateSet).datepicker("update").on("show", Custom.DatepickerShow).on("hide", Custom.DatepickerHide);
            });

            $(this).bind("keypress", function (e) {
                $('.datepicker').hide();
                if (Common.Enter(e) == true) {
                    thisObj.blur();
                }
            });

            $(this).bind("keyup", function (e) {
                GetDateKeyUp(e, thisObj);
            });

            $(this).bind("blur", function (e) {
                GetDateKeyRetrun(thisObj);
                if (thisObj.val() != "" && GetDateCheckDate(thisObj) == false) {
                    thisObj.val("");
                }
            });
        });

        Obj.find('.input-daterange').not("[dateMode]").datepicker(Custom.DatepickerSet).on("show", Custom.DatepickerShow).on("hide", Custom.DatepickerHide);
        Obj.find('.input-daterange').not("[dateMode]").find("input").each(function () {
            var thisObj = $(this);
            $(this).data("SetValue", function (value) {
                thisObj.val(value);
                thisObj.parent().datepicker(Custom.DatepickerSet).datepicker("update").on("show", Custom.DatepickerShow).on("hide", Custom.DatepickerHide);
            });

            $(this).bind("keypress", function (e) {
                $('.datepicker').hide();
                if (Common.Enter(e) == true) {
                    thisObj.blur();
                }
            });

            $(this).bind("keyup", function (e) {
                GetDateKeyUp(e, thisObj);
            });

            $(this).bind("blur", function (e) {
                GetDateKeyRetrun(thisObj);
                if (thisObj.val() != "" && GetDateCheckDate(thisObj) == false) {
                    thisObj.val("");
                }
            });
        });
        // DatePicker (일자 + 월)
        Obj.find('.input-group.date[dateMode=DateAndMonth]').find("input").each(function () {
            $(this).datepicker(Custom.DatepickerSet).on("show", Custom.DatepickerShow).on("hide", Custom.DatepickerHide);

            var thisObj = $(this);
            $(this).data("SetValue", function (value) {
                thisObj.val(value);
                thisObj.datepicker(Custom.DatepickerSet).datepicker("update").on("show", Custom.DatepickerShow).on("hide", Custom.DatepickerHide);
            });

            $(this).bind("keypress", function (e) {
                $('.datepicker').hide();
                if (Common.Enter(e) == true) {
                    thisObj.blur();
                }
            });

            $(this).bind("keyup", function (e) {
                GetDateKeyUp(e, thisObj);
            });

            $(this).bind("blur", function (e) {
                GetDateKeyRetrun(thisObj);
                if (thisObj.val() != "" && GetDateCheckDate(thisObj) == false) {
                    thisObj.val("");
                }
            });
        });
        Obj.find('.input-group.date[dateMode=DateAndMonth]').find("i").each(function () {
            var thisObj = $(this);
            var inputObj = thisObj.parent().parent().find("input");
            $(this).datepicker(Custom.DatepickerMonthSet).on("show", function () {
                if (inputObj.attr("hide") != undefined) {
                    thisObj.datepicker("hide");
                }
            }).on("hide", function (ev) {
                try {
                    var yyyyMM = ev.date.format("yyyy") + "." + ev.date.format("MM");
                    inputObj.data("SetValue")(yyyyMM);
                } catch (e) {
                }
            });
        });

        // DatePicker (월)
        Obj.find('.input-group.date[dateMode=Month]').datepicker(Custom.DatepickerMonthSet).on("show", Custom.DatepickerShow).on("hide", Custom.DatepickerHide);
        Obj.find('.input-group.date[dateMode=Month]').find("input").each(function () {
            var thisObj = $(this);
            $(this).data("SetValue", function (value) {
                thisObj.val(value);
                thisObj.parent().datepicker(Custom.DatepickerMonthSet).datepicker("update").on("show", Custom.DatepickerShow).on("hide", Custom.DatepickerHide);
            });

            $(this).bind("keypress", function (e) {
                $('.datepicker').hide();
                if (Common.Enter(e) == true) {
                    thisObj.blur();
                }
            });

            $(this).bind("keyup", function (e) {
                GetDateKeyUp(e, thisObj);
            });

            $(this).bind("blur", function (e) {
                GetDateKeyRetrun(thisObj);
                if (thisObj.val() != "" && GetDateCheckDate(thisObj) == false) {
                    thisObj.val("");
                }
            });

        });

        // DatePicker (년)
        Obj.find('.input-group.date[dateMode=Year]').datepicker(Custom.DatepickerYearSet).on("show", Custom.DatepickerShow).on("hide", Custom.DatepickerHide);
        Obj.find('.input-group.date[dateMode=Year]').find("input").each(function () {
            var thisObj = $(this);
            $(this).data("SetValue", function (value) {
                thisObj.val(value);
                thisObj.parent().datepicker(Custom.DatepickerYearSet).datepicker("update").on("show", Custom.DatepickerShow).on("hide", Custom.DatepickerHide);
            });
        });

        // DatePicker (기간)
        Obj.find('.input-daterange').datepicker({
            format: "yyyy-mm-dd",
            language: "kr",
            todayBtn: "linked",
            keyboardNavigation: false,
            forceParse: false,
            calendarWeeks: true,
            autoclose: true,
            todayHighlight: true
        });

        // clockpicker
        if (Obj.find('.clockpicker').clockpicker != undefined) {
            Obj.find('.clockpicker').clockpicker();
        }

        // Input Enter Event Set
        Obj.find("input[search]").each(function () {
            var BtnId = $(this).attr("search");

            if ($("#" + BtnId).length > 0) {
                $(this).bind("keypress", function (e) {
                    if (Common.Enter(e) == true) {
                        $("#" + BtnId).click();
                    }
                });
            }
        });

        // searchAll
        Obj.find("button[searchAll]").each(function () {
            $(this).bind("click", function () {
                var SearchQuery = $(this).attr("searchAll");
                var BtnId = $(this).attr("search");

                var arr = SearchQuery.split("|")
                for (var i = 0; i < arr.length; i++) {
                    $("#" + arr[i]).val("");
                }
                $("#" + BtnId).click();
            })
        });

        // File Event Set
        Obj.find(".form_file_wrap").each(function () {
            var thisObj = $(this);
            var fileName = thisObj.attr("fileName");
            var fileInputObj = thisObj.find("#" + fileName);
            var boardType = thisObj.attr("boardType");
            var attachFileUrl = Custom.AttachFileUrl;
            var show_hide_btn = thisObj.find("button[iType=showhide]");
            var fileDragShowHide = Cookies("comfdsh");

            if (fileDragShowHide == "hide") {
                Cookies("comfdsh", "hide", 365);
                show_hide_btn.html("<i class=\"fa fa-chevron-down\"></i> 펼치기");
                thisObj.find(".file_list_wrap").css("display", "none");
            } else {
                show_hide_btn.html("<i class=\"fa fa-chevron-up\"></i> 숨기기");
                thisObj.find(".file_list_wrap").css("display", "");
            }
            show_hide_btn.bind("click", function () {
                if (Common.Contains(show_hide_btn.find("i").attr("class"), "fa-chevron-up")) {
                    show_hide_btn.html("<i class=\"fa fa-chevron-down\"></i> 펼치기");
                    thisObj.find(".file_list_wrap").css("display", "none");
                    Cookies("comfdsh", "hide", 365);
                } else {
                    show_hide_btn.html("<i class=\"fa fa-chevron-up\"></i> 숨기기");
                    thisObj.find(".file_list_wrap").css("display", "");
                    Cookies("comfdsh", "", 365);
                }
            });

            if (thisObj.attr("AttachFileUrl") != undefined) {
                attachFileUrl = thisObj.attr("AttachFileUrl");
            }

            thisObj.find(".file_hidden").bind("change", function () {
                var fileObj = $(this)[0];
                for (var i = 0; i < fileObj.files.length; i++) {
                    var file = fileObj.files[i];
                    Custom.FileUpload(file, fileInputObj, thisObj.find("span.st3"), thisObj.find(".file_cmt"), thisObj.find(".file_list"), boardType, attachFileUrl, thisObj);
                    fileObj.value = "";
                }
                show_hide_btn.html("<i class=\"fa fa-chevron-up\"></i> 숨기기");
                thisObj.find(".file_list_wrap").css("display", "");
            });

            thisObj.find("button[filedelmode]").bind("click", function () {
                if (confirm("파일을 모두 삭제 하시겠습니까?") == true) {
                    if (typeof boardType == "undefined") {
                        thisObj.find(".file_cmt").css("display", "");
                        thisObj.find(".file_list").css("display", "none");
                        thisObj.removeAttr("dragFile");
                        thisObj.find("span.st3").attr("filesize", 0);
                        thisObj.find("span.st3").html(Common.Convert.FileSize(0));

                        delete Common.RequestInfo.Files[thisObj.attr("dragFile")]
                    } else {
                        var fileInputObjValue = fileInputObj.val();
                        var fileInputObjValueArr = fileInputObjValue.split("|~|")
                        var tmpValue = "";

                        for (var i = 0; i < fileInputObjValueArr.length; i++) {
                            if (fileInputObjValueArr[i] != "") {
                                var valueArr = fileInputObjValueArr[i].split("||");
                                Custom.FileDelete(attachFileUrl, fileInputObj, valueArr[2], thisObj.find("span.st3"), thisObj.find(".file_cmt"), thisObj.find(".file_list"), thisObj);
                            }
                        }
                    }

                    thisObj.find(".file_cmt").css("display", "");
                    thisObj.find(".file_list").css("display", "none");
                    thisObj.find(".file_list").html("");
                }
            });

            thisObj.bind("drop", function (e) {
                if (typeof FileList == 'undefined') {
                    alert("File Drag & Drop 미지원 브라우저 혹은 버전입니다. 다른 브라우저를 이용하거나 브라우저를 업데이트 해주세요.");
                    return;
                }

                var fileObj = "";

                if (e.originalEvent == undefined) {
                    fileObj = e.dataTransfer;
                } else {
                    fileObj = e.originalEvent.dataTransfer;
                }

                if (!fileObj.files) {
                    alert("File Drag & Drop 미지원 브라우저 혹은 버전입니다. 다른 브라우저를 이용하거나 브라우저를 업데이트 해주세요.");
                } else {
                    for (var i = 0; i < fileObj.files.length; i++) {
                        var file = fileObj.files[i];
                        Custom.FileUpload(file, fileInputObj, thisObj.find("span.st3"), thisObj.find(".file_cmt"), thisObj.find(".file_list"), boardType, attachFileUrl, thisObj);
                    }
                }

                // 에디터기 변경으로 인해 이벤트 멈춤 빼기
                //Common.EventReturnValue(e);
                //Common.EventCancelBubble(e);
                return;
            });

            // 파일 초기화 기능 추가
            fileInputObj.data("Init", function () {
                thisObj.find(".file_cmt").css("display", "");
                thisObj.find(".file_list").css("display", "none");
                thisObj.find(".file_list").html("");
            });
        });

        Obj.find(".fileinput").each(function () {
            var thisObj = $(this);

            thisObj.bind("drop", function (e) {
                if (typeof FileList == 'undefined') {
                    alert("File Drag & Drop 미지원 브라우저 혹은 버전입니다. 다른 브라우저를 이용하거나 브라우저를 업데이트 해주세요.");
                    return;
                }

                var fileObj = "";

                if (e.originalEvent == undefined) {
                    fileObj = e.dataTransfer;
                } else {
                    fileObj = e.originalEvent.dataTransfer;
                }

                if (!fileObj.files) {
                    alert("File Drag & Drop 미지원 브라우저 혹은 버전입니다. 다른 브라우저를 이용하거나 브라우저를 업데이트 해주세요.");
                } else {
                    if (fileObj.files.length > 1) {
                        alert("파일은 하나만 업로드 가능합니다.");
                    } else if (fileObj.files.length == 0) {
                        alert("파일이 없습니다.");
                    } else {
                        var fileName = Common.GetTodayTimeString();
                        thisObj.find("input[type=file]").get(0).value = "";
                        thisObj.find("input[type=file]").attr("dragFile", fileName);
                        thisObj.find(".fileinput-filename").html(fileObj.files[0].name);
                        thisObj.removeClass("fileinput-new");
                        thisObj.addClass("fileinput-exists");
                        Common.RequestInfo.Files[fileName] = fileObj.files[0];
                    }
                }

                // 에디터기 변경으로 인해 이벤트 멈춤 빼기
                //Common.EventReturnValue(e);
                //Common.EventCancelBubble(e);
                return;
            });
        });
        // table sort
        Obj.find("table th[sortable]").each(function () {
            $(this).css("cursor", "pointer");
        });

        // checkbox
        Obj.find(".i-checks").each(function () {
            $(this).find("label").css("cursor", "pointer");
            $(this).iCheck({
                checkboxClass: 'icheckbox_square-green',
                radioClass: 'iradio_square-green',
            });
        });

        Obj.find('a.nav-link').each(function () {
            var thisObj = $(this);
            $(this).bind("click", function () {
                thisObj.parent().parent().find("li").removeClass("active");
                thisObj.parent().parent().parent().find("div.tab-pane").removeClass("active");
                thisObj.parent().addClass("active");
                var tabContentId = thisObj.attr("href");
                $(tabContentId).addClass("active");
            });
        });

        Obj.find('.dropdown-toggle').each(function () {
            var thisObj = $(this);
            $(this).bind("click", function () {
                if (thisObj.parent().hasClass("open")) {
                    thisObj.parent().removeClass("open");
                } else {
                    thisObj.parent().addClass("open");

                    setTimeout(function () {
                        $("body").one("click", function () {
                            thisObj.parent().removeClass("open");
                        });
                    }, 100);
                }
            });
        });
    } catch (e) {
    }
}

/////////////////////////////////////////////
// 조회
Custom.List = function (search, list, info) {
    var RtnValue = false;

    info = (info || {});

    var url = (info.url || location.pathname);
    var callback = (info.callback || undefined);
    var customback = (info.customback || undefined);
    var message = (info.message || "");
    var async = false;
    var loading = true;

    if (info.async == undefined) {
        async = true;
    } else {
        async = info.async;
    }

    if (info.loading == undefined) loading = true
    else loading = info.loading

    if (Common.Validate(search) == true) {
        var ReqInfo = new Common.RequestInfo();
        ReqInfo.AddParameter(search)

        if (loading)
            Common.Loading.Show();

        Common.Ajax(url, ReqInfo, function (rtn) {
            if (message != null && message != "")
                alert(message);

            if (typeof callback == "function") {
                callback(rtn);
            } else {
                try {
                    var bodyObj = $(rtn);
                    if (bodyObj.find("#" + list.attr("id")).length == 0) {
                        list.html(bodyObj.html());
                        Custom.Load(list);
                    } else {
                        var listTmp = bodyObj.find("#" + list.attr("id"));
                        list.html(listTmp.html());
                        Custom.Load(list);
                    }
                } catch (e) {
                }

                if (typeof customback == "function") {
                    customback(rtn);
                }
            }

            RtnValue = true;

            if (loading) {
                Common.Loading.Hide();
            }
        }, { async: async, formMethod: info.formMethod });
    }

    return RtnValue;
}
/////////////////////////////////////////////


/////////////////////////////////////////////
// 다이얼로그
Custom.Dialog = function (info) {
    var diaObj = null;
    var url = (info.url || "");
    var name = (info.name || Common.GetTodayTimeString());
    var formMethod = (info.formMethod || "POST");
    var async, loading, blockClose, drag;
    var openFn = (info.openFn || undefined);
    var closeFn = (info.closeFn || undefined);
    var dialogEvent = (info.dialogEvent || "");
    var opener = (info.opener || null);
    var scrollTopPosition = 0;
    var scrollTopMode = false;

    if (url == "")
        return;

    if (info.async == undefined) async = false
    else async = info.async

    if (info.loading == undefined) loading = false
    else loading = info.loading

    if (info.blockClose == undefined) blockClose = true
    else blockClose = info.blockClose

    if (info.scrollTop == undefined) scrollTopMode = false
    else scrollTopMode = info.scrollTop

    if (info.drag == undefined) drag = true
    else drag = info.drag

    var reqInfo = new Common.RequestInfo();

    for (var keyName in info.param) {
        reqInfo.AddParameter(keyName, info.param[keyName]);
    }

    try {
        if ($("div[tmp_dialogOpenUrl=" + url.replaceAll(".", "").replaceAll("/", "") + "]").length != 0) {
            return;
        }
    } catch (e) { }

    if (loading)
        Common.Loading.Show();

    if (scrollTopMode == true && (Common.Device.IE == true || Common.Device.FIREFOX == true) && $(".modal").length == 0) {
        scrollTopPosition = $("html").scrollTop();
        $("html").scrollTop(0);
    }

    setTimeout(function () {
        Common.Ajax(url, reqInfo, function (html) {
            if ($("#diaCustom" + name).length > 0) {
                name += name + window.crypto.getRandomValues(new Uint32Array(1));
            }

            diaObj = $('<div class="modal inmodal ' + dialogEvent + '"></div>');
            diaObj.html(html);
            diaObj.attr("id", "diaCustom" + name);
            diaObj.attr("tmp_dialogOpenUrl", url);
            diaObj.attr("parent_title", document.title);

            if (opener != null)
                diaObj.attr("openerId", opener.attr("id"));

            diaObj.on('show.bs.modal', function () {
                //if (typeof openFn == "function") {
                //    openFn(diaObj);
                //}
            });

            diaObj.on('shown.bs.modal', function () {
                diaObj.html(html);
                Custom.Load(diaObj);

                if (typeof openFn == "function") {
                    openFn(diaObj);
                }

                if (drag == true) {
                    diaObj.find(".modal-dialog").draggable({ 'cancel': '.modal-body,.modal-footer' });
                }
            });

            diaObj.on('hide.bs.modal', function () {
                if (typeof closeFn == "function") {
                    closeFn(diaObj);
                }

                if ($('.modal:visible').length > 1) {
                }
            });

            diaObj.on('hidden.bs.modal', function () {
                // 객체 삭제
                diaObj.remove();

                // 스크롤 버그 수정
                if ($('.modal:visible').length > 0) {
                    $('body').addClass('modal-open');
                } else {
                    if (scrollTopMode == true && (Common.Device.IE == true || Common.Device.FIREFOX == true)) {
                        $("html").scrollTop(scrollTopPosition);
                    }
                }

                document.title = diaObj.attr("parent_title");
            });

            Common.Loading.Hide();

            var modalOp = {};
            if (blockClose == false) {
                modalOp.backdrop = "static";
            }

            diaObj.modal(modalOp);
            diaObj.modal('show');

            if (diaObj.find("button[btn=Save]").length > 0) {
                diaObj.bind('keydown', function (e) {
                    // You may replace `c` with whatever key you want
                    if ((e.metaKey || e.ctrlKey) && (String.fromCharCode(e.which).toLowerCase() === 's')) {
                        diaObj.find("button[btn=Save]").click();
                        Common.EventCancelBubble(e);
                        Common.EventReturnValue(e);
                    }
                });
            }

            if (diaObj && diaObj != null) {
                diaObj.data("ReLoad", function (SetReqInfo) {
                    if (!(SetReqInfo == null || SetReqInfo == undefined)) {
                        reqInfo = SetReqInfo;
                    }

                    Common.Ajax(url, reqInfo, function (html) {
                        diaObj.html(html);
                        Custom.Load(diaObj);

                        if (diaObj.find("button[btn=Save]").length > 0) {
                            diaObj.bind('keydown', function (e) {
                                // You may replace `c` with whatever key you want
                                if ((e.metaKey || e.ctrlKey) && (String.fromCharCode(e.which).toLowerCase() === 's')) {
                                    diaObj.find("button[btn=Save]").click();
                                    Common.EventCancelBubble(e);
                                    Common.EventReturnValue(e);
                                }
                            });
                        }
                    });
                });

                diaObj.data("Hide", function () {
                    diaObj.modal('hide');
                });
            }
        }, { formMethod: formMethod, async: async });
    }, 10);
}
/////////////////////////////////////////////


/////////////////////////////////////////////
// 실행
Custom.Exec = function (info) {
    info = (info || {});

    var obj = (info.obj || $("#wrapper"));
    var url = (info.url || location.pathname + "Save");
    var callback = (info.callback || undefined);
    var errback = (info.errback || undefined);
    var async = (info.async || false);
    var formMethod = (info.formMethod || "POST");
    var message = (info.message || "");
    var resultMessage = (info.resultMessage || false);
    var fileUpload = (info.fileUpload || false);
    var loading;

    if (info.loading == undefined) loading = true
    else loading = info.loading

    if (Common.Validate(obj) == true) {
        var ReqInfo = new Common.RequestInfo();
        if (fileUpload == true) {
            ReqInfo.formData = new FormData();
        }
        ReqInfo.AddParameter(obj)

        if (loading) {
            Common.Loading.Show();

            setTimeout(function () {
                Common.Ajax(url, ReqInfo, function (rtn) {
                    var msgCheck = false;
                    if (resultMessage == false && message != null && message != "") {
                        Common.Loading.Hide("Success", message);
                        msgCheck = true;
                    }

                    if (resultMessage == true) {
                        Common.Loading.Hide("Success", rtn);
                        msgCheck = true;
                    }

                    if (msgCheck == false) {
                        Common.Loading.Hide();
                    }

                    if (typeof callback == "function")
                        callback(rtn);
                }, { formMethod: formMethod, async: async, errback: errback });
            }, 10);
        } else {
            Common.Ajax(url, ReqInfo, function (rtn) {
                Common.Loading.Hide();

                if (resultMessage == false && message != null && message != "") {
                    Common.Msg(message);
                }

                if (resultMessage == true) {
                    alert(rtn);
                    Common.Msg(rtn);
                }

                if (typeof callback == "function")
                    callback(rtn);

            }, { formMethod: formMethod, async: async, errback: errback });
        }
    }
}
/////////////////////////////////////////////


/////////////////////////////////////////////
// 미리보기
Custom.PreView = function (body, width, height) {
    var mm = window.open("", "PREVIEW", "width=" + width + ", height=" + height + ", scrollbars=yes, top=0, left=0");

    var doc = "<html>\n"
        + "	<head>\n"
        + "		<title>미리보기</title>\n"
        + "		<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\n"
        + "	</head>\n"
        + "	<body style=\"margin:0;\">\n"
        + body
        + "	</body>\n"
        + "</html>";

    mm.document.write(doc);
}
/////////////////////////////////////////////


/////////////////////////////////////////////
// iframe 사이즈
Custom.resizeIframe = function (obj) {
    obj.style.height = (obj.contentWindow.document.body.scrollHeight + 30) + 'px';
}
/////////////////////////////////////////////

/////////////////////////////////////////////
// 키 이벤트 넣기 편의성 기능을 위해서...
$(document).bind('keydown', function (e) {
    // You may replace `c` with whatever key you want
    if ((e.metaKey || e.ctrlKey) && ((String.fromCharCode(e.which).toLowerCase() === 's') || e.which == "112")) {
        Common.EventCancelBubble(e);
        Common.EventReturnValue(e);
        if (e.which == "112") {

        }
    }
});

// Drag Text
Custom.DragTextTmp = "";
$(document).bind('mouseup', function (e) {
    var selectionText = "";
    if (document.getSelection) {
        selectionText = document.getSelection();
    } else if (document.selection) {
        selectionText = document.selection.createRange().text;
    }

    Custom.DragTextTmp = selectionText;
});
/////////////////////////////////////////////


/////////////////////////////////////////////
// 파일업로드
Custom.FileExe = function (fileName) {
    var rtnValue = "zip";

    try {
        var fileNameArr = fileName.split(".");
        var ext = fileNameArr[fileNameArr.length - 1];

        if (ext.toLowerCase() == "jpg" || ext.toLowerCase() == "gif" || ext.toLowerCase() == "png" || ext.toLowerCase() == "jpeg" || ext.toLowerCase() == "bmp") {
            rtnValue = "img";
        } else if (ext.toLowerCase() == "pdf") {
            rtnValue = "pdf";
        } else if (ext.toLowerCase() == "doc" || ext.toLowerCase() == "ppt" || ext.toLowerCase() == "pptx" || ext.toLowerCase() == "xlsx" || ext.toLowerCase() == "xls" || ext.toLowerCase() == "hwp") {
            rtnValue = "doc";
        } else if (ext.toLowerCase() == "mp3" || ext.toLowerCase() == "mp4" || ext.toLowerCase() == "avi" || ext.toLowerCase() == "wmv" || ext.toLowerCase() == "flv" || ext.toLowerCase() == "mov") {
            rtnValue = "mov";
        }

    } catch (e) {
        rtnValue = "zip";
    }

    return rtnValue;
}

Custom.FileUpload = function (file, fileInputObj, size_srmyObj, noListObj, listObj, boardType, attachFileUrl, controllObj) {
    var AddFile = "";
    AddFile += "<li>";
    AddFile += '<a href="#" style="display:none" class="file_del" onclick="return false;"><img src="/images/ico_file_del.gif" width="16" height="" alt="" /></a> ';
    AddFile += '<em class="ico_file ' + Custom.FileExe(file.name) + '"></em> ';
    AddFile += '<label class="file_name" style="cursor:pointer;">' + file.name + '</label>';
    AddFile += '<span class="file_size">' + Common.Convert.FileSize(file.size) + ' <em class="file_trans_gauge"><em class="gauge_in" style="width:0%">&nbsp;</em></em></span>';
    AddFile += "</li>";
    var AddFileObj = $(AddFile);

    noListObj.css("display", "none");
    listObj.css("display", "");
    listObj.append(AddFileObj);

    if (typeof boardType == "undefined") {
        var fileName = Common.GetTodayTimeString();

        var dragFileCount = 1;
        if (controllObj.attr("dragFileCount") == undefined) {
            controllObj.attr("dragFileCount", "1");
        } else {
            dragFileCount = Common.Convert.Int(controllObj.attr("dragFileCount")) + 1;
        }
        AddFileObj.attr("dragFileCount", dragFileCount);

        if (controllObj.attr("dragFile") != undefined && controllObj.attr("dragFile") != "") {
            fileName = controllObj.attr("dragFile");
        } else {
            controllObj.attr("dragFile", fileName);
        }
        controllObj.find("input[type=file]").attr("dragFiles", fileName);

        var sendFileObj = Common.RequestInfo.Files[fileName];

        if (sendFileObj == undefined || sendFileObj == null) {
            var file_arr = [];
            file_arr.push({ dragFileCount: dragFileCount, file: file });
            Common.RequestInfo.Files[fileName] = file_arr;
        } else {
            sendFileObj.push({ dragFileCount: dragFileCount, file: file });
            Common.RequestInfo.Files[fileName] = sendFileObj;
        }

        AddFileObj.find(".gauge_in").parent().remove();
        AddFileObj.find("a.file_del").css("display", "");

        AddFileObj.find("a.file_del").bind("click", function () {
            if (confirm("파일을 삭제하시겠습니까?") == true) {
                var dragFileCount = AddFileObj.attr("dragFileCount");
                AddFileObj.remove();

                var sendFileObj = Common.RequestInfo.Files[fileName];

                for (var Idx in sendFileObj) {
                    if (sendFileObj[Idx] && sendFileObj[Idx].dragFileCount == dragFileCount) {
                        delete sendFileObj[Idx];
                        Common.RequestInfo.Files[fileName] = sendFileObj;
                        break;
                    }
                }

                Custom.FileDelete(attachFileUrl, fileInputObj, "", size_srmyObj, noListObj, listObj, controllObj);
            }
        });

        AddFileObj.attr("filesize", file.size);

        var filesize = Common.Convert.Int(file.size);
        var total_filesize = Common.Convert.Int(size_srmyObj.attr("filesize"));
        size_srmyObj.attr("filesize", total_filesize + filesize);
        size_srmyObj.html(Common.Convert.FileSize(total_filesize + filesize))
    } else {
        var formData = new FormData();
        formData.append("DextUploadX", file);
        formData.append("BoardType", boardType);

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            async: true,
            cache: false,
            url: attachFileUrl + "/UploadActiveX",
            xhr: function () {
                myXhr = $.ajaxSettings.xhr();
                myXhr.addEventListener('progress', function (e) { }, false);
                if (myXhr.upload) {
                    myXhr.upload.onprogress = function (e) {
                        var completed = 0;
                        if (e.lengthComputable) {
                            var done = e.position || e.loaded,
                                total = e.totalSize || e.total;
                            completed = Math.round((done / total * 1000) / 10);
                        }

                        // % 진행율
                        AddFileObj.find(".gauge_in").css("width", completed + "%");
                    }
                }
                return myXhr;
            },
            data: formData,
            dataType: "text",
            timeout: 1000 * 60 * 60 * 24,
            success: function (result, textStatus) {
                var fileInputObjValue = fileInputObj.val();
                fileInputObj.val(fileInputObjValue + result);

                var resultArr = result.replace("|~|", "").split("||");
                AddFileObj.attr("FileName", resultArr[0]);
                AddFileObj.attr("FileLength", resultArr[1]);
                AddFileObj.attr("permanent_code", resultArr[2]);
                AddFileObj.attr("limit_code", resultArr[3]);

                if (resultArr.length != 4) {
                    AddFileObj.remove();
                    return;
                }

                AddFileObj.find(".gauge_in").parent().remove();
                AddFileObj.find("a.file_del").css("display", "");

                var filesize = Common.Convert.Int(resultArr[1]);
                var total_filesize = Common.Convert.Int(size_srmyObj.attr("filesize"));
                size_srmyObj.attr("filesize", total_filesize + filesize);
                size_srmyObj.html(Common.Convert.FileSize(total_filesize + filesize))

                AddFileObj.find("label.file_name").bind("click", function () {
                    Common.MoveUrl(attachFileUrl + "/FileDown?pCode=" + resultArr[2]);
                });

                AddFileObj.find("a.file_del").bind("click", function () {
                    if (confirm("파일을 삭제하시겠습니까?") == true) {
                        AddFileObj.remove();
                        Custom.FileDelete(attachFileUrl, fileInputObj, resultArr[2], size_srmyObj, noListObj, listObj, controllObj);
                    }
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
                AddFileObj.remove();
            }
        });
    }
}

Custom.FileDelete = function (attachFileUrl, fileInputObj, pCode, size_srmyObj, noListObj, listObj, controllObj) {
    if (pCode != "") {
        var formData = new FormData();
        formData.append("pCode", pCode);

        $.ajax({
            type: "POST",
            contentType: false,
            processData: false,
            async: true,
            cache: false,
            url: attachFileUrl + "/FileDelete",
            data: formData,
            dataType: "text",
            timeout: 1000 * 60 * 60 * 24,
            success: function (result, textStatus) {
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            }
        });
    }

    if (listObj.children().length == 0) {
        noListObj.css("display", "");
        listObj.css("display", "none");
        controllObj.removeAttr("dragFile");
    } else {
        noListObj.css("display", "none");
        listObj.css("display", "");
    }

    var tmpValue = "";
    var total_filesize = 0;

    if (pCode != "") {
        var fileInputObjValue = fileInputObj.val();
        var fileInputObjValueArr = fileInputObjValue.split("|~|")

        for (var i = 0; i < fileInputObjValueArr.length; i++) {
            if (fileInputObjValueArr[i] != "") {
                var valueArr = fileInputObjValueArr[i].split("||");

                if (pCode != valueArr[2]) {
                    tmpValue += fileInputObjValueArr[i];
                    total_filesize += Common.Convert.Int(valueArr[1]);
                }
            }
        }

        fileInputObj.val(tmpValue);
    } else {
        listObj.children().each(function () {
            total_filesize += Common.Convert.Int($(this).attr("filesize"));
        });
    }

    size_srmyObj.attr("filesize", total_filesize);
    size_srmyObj.html(Common.Convert.FileSize(total_filesize));
}
/////////////////////////////////////////////

/////////////////////////////////////////////
// Custom Code Search Function
Custom.GetCodeKeydown = function (e, Obj) {
    if (Common.Enter(e) == true) {
        Custom.GetCodeSeach(Obj);
    }
}
Custom.GetCodeSeach = function (Obj) {
    if (Obj.val() == "") {
        Obj.val("");
        Obj.attr("OldValue", "");
        Obj.attr("Code", "");
        Obj.attr("OldCode", "");
       // Obj.attr("value2", "");
        return;
    }

    if (Obj.attr("OldValue") && Obj.attr("OldValue") == Obj.val()) {
        return;
    }

    Custom.GetCodeSeachShow(Obj);
}
Custom.GetCodeKeydown2 = function (e, Obj) {
    if (Common.Enter(e) == true) {
        Custom.GetCodeSeach2(Obj);
    }
}
Custom.GetCodeSeach2 = function (Obj) {
    
    if (Obj.val() == "") {
    
        Obj.val("");
        Obj.attr("OldValue", "");
        Obj.attr("Code", "");
        Obj.attr("OldCode", "");
       
        Obj.parent().find('#' + Obj.attr("id") + 'Info').text('');
        return;
    }

    if (Obj.attr("OldValue") && Obj.attr("OldValue") == Obj.val()) {
        
        return;
    }

    Custom.GetCodeSeachShow2(Obj);
}
Custom.GetCodeSeachShow = function (Obj) {
    Custom.GetCodeSeachOpen(Obj, Obj.val());
}
Custom.GetCodeSeachShow2 = function (Obj) {
    
    Custom.GetCodeSeachOpen2(Obj, Obj.val());
}

Custom.GetCodeSeachOpen = function (Obj, search, showDia) {
    showDia = (showDia || false);

    var layerId = "layerPopup_" + Obj.attr("id");
    var reqInfo = new Common.RequestInfo();
    reqInfo.AddParameter("id", Obj.attr("id"));
    reqInfo.AddParameter("search", search);
    reqInfo.AddParameter("showDia", showDia);
    Obj.parent().find('#' + Obj.attr("id") + 'Info').text('');

    Common.Ajax(Obj.attr("url"), reqInfo, function (dataInfo) {
        if (dataInfo.Contains("layerPopup_")) {
            if ($("#" + layerId).length > 0) {
                var bodyObj = $(dataInfo);
                $("#" + layerId).html(bodyObj.html());

                $("#" + layerId + " .clickElment").bind("click", function () {
                    Obj.val($(this).attr("value"));
              
                    Obj.attr("OldValue", $(this).attr("value"));
                    //Obj.attr("value2", $(this).attr("value2"));
                    Obj.attr("Code", $(this).attr("code"));
                    Obj.attr("OldCode", $(this).attr("code"));

                    //Obj.parent().find('#' + Obj.attr("id") + 'Info').text('');
                    Obj.parent().find('#' + Obj.attr("id") + 'Info').text($(this).attr("value2"));

                    $("#" + layerId).parent().modal("hide");
                    
                    if (Obj.attr("autoEvent") && Obj.attr("autoEvent") != "")
                        window[Obj.attr("autoEvent")](Obj);
                });
                
                $("#" + layerId + " button[btn=search]").bind("click", function () {
                    var search = $("#" + layerId + " input[id=search]").val();
                    Custom.GetCodeSeachOpen(Obj, search, true);
                });

                $("#" + layerId + " input[id=search]").bind("keypress", function (e) {
                    if (Common.Enter(e) == true) {
                        var search = $(this).val();
                        Custom.GetCodeSeachOpen(Obj, search, true);
                    }
                });
            } else {
                Custom.Dialog({
                    url: Obj.attr("url"), param: {
                        "id": Obj.attr("id"), "search": search, "showDia": showDia
                    }
                });

                Obj.val("");
                Obj.attr("Code", "");

                var diaObj = setInterval(function () {
                    if ($("#" + layerId).length > 0) {
                        clearInterval(diaObj);
                        $("#" + layerId + " .clickElment").bind("click", function () {
                            Obj.val($(this).attr("value"));
                        
                            Obj.attr("OldValue", $(this).attr("value"));
                          //  Obj.attr("value2", $(this).attr("value2"));
                            Obj.attr("Code", $(this).attr("code"));
                            Obj.attr("OldCode", $(this).attr("code"));

                            //Obj.parent().find('#' + Obj.attr("id") + 'Info').text('');
                            Obj.parent().find('#' + Obj.attr("id") + 'Info').text($(this).attr("value2"));

                            $("#" + layerId).parent().modal("hide");                            
                            if (Obj.attr("autoEvent") && Obj.attr("autoEvent") != "")
                                window[Obj.attr("autoEvent")](Obj);
                        });
                        
                        $("#" + layerId + " button[btn=search]").bind("click", function () {
                            var search = $("#" + layerId + " input[id=search]").val();
                            Custom.GetCodeSeachOpen(Obj, search, true);
                        });

                        $("#" + layerId + " input[id=search]").bind("keypress", function (e) {
                            if (Common.Enter(e) == true) {
                                var search = $(this).val();
                                Custom.GetCodeSeachOpen(Obj, search, true);
                            }
                        });
                    }
                }, 10);
            }
        } else {
            //한건만 조회된 경우
            var code = dataInfo.split("|~|")[0];
            var value = dataInfo.split("|~|")[1];
            var value2 = dataInfo.split("|~|")[2];

            Obj.val(value);

            Obj.attr("OldValue", value);
            Obj.attr("Code", code);
            Obj.attr("OldCode", code);
           // Obj.attr("value2", value2);

            //alert(value);
            //Obj.parent().find('#' + Obj.attr("id") + 'Info').text('');
            Obj.parent().find('#' + Obj.attr("id") + 'Info').text(value2 + ' ' + code);

            if (Obj.attr("autoEvent") && Obj.attr("autoEvent") != "") {
                window[Obj.attr("autoEvent")](Obj);
            }
            //alert("SUCCESS");       
        }
    });
}

Custom.GetCodeSeachOpen2 = function (Obj, search, showDia) {
    showDia = (showDia || false);

    var layerId = "layerPopup_" + Obj.attr("id");
    var reqInfo = new Common.RequestInfo();
    reqInfo.AddParameter("id", Obj.attr("id"));
    reqInfo.AddParameter("search", search);
    reqInfo.AddParameter("showDia", showDia);
    Obj.parent().find('#' + Obj.attr("id") + 'Info').text('');

    Common.Ajax(Obj.attr("url"), reqInfo, function (dataInfo) {
        if (dataInfo.Contains("layerPopup_")) {
            if ($("#" + layerId).length > 0) {
                var bodyObj = $(dataInfo);
                $("#" + layerId).html(bodyObj.html());

                $("#" + layerId + " .clickElment").bind("click", function () {
                    Obj.val($(this).attr("value"));

                    Obj.attr("OldValue", $(this).attr("value"));
                  
                    Obj.attr("Code", $(this).attr("value"));
                    Obj.attr("OldCode", $(this).attr("value"));

                
                    Obj.parent().find('#' + Obj.attr("id") + 'Info').text($(this).attr("value2"));

                    $("#" + layerId).parent().modal("hide");

                    if (Obj.attr("autoEvent") && Obj.attr("autoEvent") != "")
                        window[Obj.attr("autoEvent")](Obj);
                });

                $("#" + layerId + " button[btn=search]").bind("click", function () {
                    var search = $("#" + layerId + " input[id=search]").val();
                    Custom.GetCodeSeachOpen2(Obj, search, true);
                });

                $("#" + layerId + " input[id=search]").bind("keypress", function (e) {
                    if (Common.Enter(e) == true) {
                        var search = $(this).val();
                        Custom.GetCodeSeachOpen2(Obj, search, true);
                    }
                });
            }
            else {
                Custom.Dialog({
                    url: Obj.attr("url"), param: {
                        "id": Obj.attr("id"), "search": search, "showDia": showDia
                    }
                });

                Obj.val("");
                Obj.attr("OldValue", "");
                Obj.attr("Code", "");
                Obj.attr("OldCode", "");

                var diaObj = setInterval(function () {
                    if ($("#" + layerId).length > 0) {
                        clearInterval(diaObj);
                        $("#" + layerId + " .clickElment").bind("click", function () {
                            Obj.val($(this).attr("value"));

                            Obj.attr("OldValue", $(this).attr("value"));
                           
                            Obj.attr("Code", $(this).attr("value"));
                            Obj.attr("OldCode", $(this).attr("value"));

                          
                            Obj.parent().find('#' + Obj.attr("id") + 'Info').text($(this).attr("value2"));

                            $("#" + layerId).parent().modal("hide");
                            if (Obj.attr("autoEvent") && Obj.attr("autoEvent") != "")
                                window[Obj.attr("autoEvent")](Obj);
                        });

                        $("#" + layerId + " button[btn=search]").bind("click", function () {
                            var search = $("#" + layerId + " input[id=search]").val();
                            Custom.GetCodeSeachOpen2(Obj, search, true);
                        });

                        $("#" + layerId + " input[id=search]").bind("keypress", function (e) {
                            if (Common.Enter(e) == true) {
                                var search = $(this).val();
                                Custom.GetCodeSeachOpen2(Obj, search, true);
                            }
                        });
                    }
                }, 10);
            }
        } else {
            //한건만 조회된 경우
            if (dataInfo != "") {
                var code = dataInfo.split("|~|")[0];
                var value = dataInfo.split("|~|")[1];
                var value2 = dataInfo.split("|~|")[2];
                var title = dataInfo.split("|~|")[3];

                Obj.val(value);

                Obj.attr("OldValue", value);
                Obj.attr("Code", value);
                Obj.attr("OldCode", value);

                //Obj.parent().find('#' + Obj.attr("id") + 'Info').text('');
                Obj.parent().find('#' + Obj.attr("id") + 'Info').text(value2 + ' ' + code + ' ' + title);

                if (Obj.attr("autoEvent") && Obj.attr("autoEvent") != "") {
                    window[Obj.attr("autoEvent")](Obj);
                }
            } else {
                Obj.val("");
                Common.Msg("검색 결과가 없습니다.");
            }
            //alert("SUCCESS");
        }
    });
}
/////////////////////////////////////////////

/////////////////////////////////////////////
// 조직도
Custom.OrgShow = function (info) {
    info = (info || {});
    var kind = (info.kind || "PERSON"); // PERSON  , DEPT
    var multi = (info.multi || "N");    // 멀티선택

    Custom.Dialog({
        url: "/OrgShow", param: {
            "kind": kind
            , "multi": multi
        }, closeFn: function (orgDia) {
            if (orgDia.find("#CustomOrgShowDia").attr("Cancel") == undefined) {
                var target = (info.target || undefined);
                var callback = (info.callback || undefined);
                var dataInfo = {};
                var dataObj = orgDia.find("#CustomOrgShowDia").find(".SelectData");

                dataObj.each(function () {
                    dataInfo[$(this).attr("name")] = $(this).val();
                });

                if (target != undefined) {

                }

                if (typeof callback == "function") {
                    callback(dataInfo);
                }
            }
        }
    });
}
/////////////////////////////////////////////

/////////////////////////////////////////////
// BU 메일 수신자
Custom.MngBuMailing = function (Obj) {
    Custom.Dialog({
        url: Obj.attr("url")
        , param: {
            "pId": Obj.attr("id")
            , "title": Obj.attr("title")
        }
    });
}
/////////////////////////////////////////////

/*/////////////////////////////////////////////

NoGrid

사용법

const {객체명} = Custom.NoGrid("{컨트롤ID}");
{객체명}.Pageing =
{
active: true,	// true,false
total: {전체카운트}, // number
pageIndexCtrlID: "Page", // Page Index Control ID
pageSizeCtrl: "PageSize", // Page Size Control ID
callBack: ListSearch // callBack Function
}
{객체명}.colModel = {콜모델};
{객체명}.dataModel = {데이터로우};
{객체명}.bind(); // 구성


/////////////////////////////////////////////*/
Custom.NoGrid = function (targetDivId, pageIndexCtrlID, pageSizeCtrlID) {

    if (typeof targetDivId !== "string") return undefined;
    // target div
    const noGrid = $(`#${targetDivId}`);

    if (typeof noGrid === "undefined" || noGrid.length === 0) return undefined;

    noGrid.Pageing = {};
    noGrid.Pageing.active = false;
    if (typeof pageIndexCtrlID === "string") noGrid.Pageing.pageIndexCtrlID = pageIndexCtrlID;
    if (typeof pageSizeCtrlID === "string") noGrid.Pageing.pageSizeCtrlID = pageSizeCtrlID;

    noGrid.bind = function () {

        this.html("");

        if (typeof this.colModel === "undefined" || this.colMode === null) {
            console.error("'colModel'이 없습니다.");
            return;
        }
        else if (typeof this.dataModel === "undefined" || this.dataModel === null) {
            console.error("'dataModel'이 없습니다.");
            return;
        }

        // 페이징 기본 설정
        const pagerID = "NoGridPageing";

        if (this.Pageing.active) {

            const pageIndexDefault = 1;
            const pageSizeDefault = 15;

            const pageIndexCtrl = $(`#${this.Pageing.pageIndexCtrlID}`);
            const pageSizeCtrl = $(`#${this.Pageing.pageSizeCtrlID}`);


            if (pageIndexCtrl.length === 0 || isNaN(pageIndexCtrl.val())) this.Pageing.index = pageIndexDefault;
            else this.Pageing.index = Number(pageIndexCtrl.val());

            if (pageSizeCtrl.length === 0 || isNaN(pageSizeCtrl.val())) this.Pageing.size = pageSizeDefault;
            else this.Pageing.size = Number(pageSizeCtrl.val());

            if (typeof this.Pageing.total === "undefined") this.Pageing.total = 1;

        }

        const dce = (tagName) => { return document.createElement(tagName); }

        const table = dce("table");
        table.classList.add("table", "table-hover", "table-striped", "table-bordered");
        const thead = dce("thead");
        const tr_thead = dce("tr");
        //------------------------------------------------------------
        // Header
        //------------------------------------------------------------
        for (const cm of this.colModel) {

            if (typeof cm.hidden !== "undefined" && typeof cm.hidden === "boolean" && cm.hidden) continue;

            if (typeof cm.title === "undefined") {
                console.error("'colModel'에 'title'이 없습니다.");
                return;
            }
            const th = dce("th");
            th.textContent = cm.title;
            th.setAttribute("title", cm.title);
            if (typeof cm.valign !== "undefined" && ['left', 'right', 'center'].includes(cm.valign)) th.classList.add(`text-${cm.valign}`);
            if (typeof cm.width !== "undefined" && !isNaN(cm.width)) th.classList.add(`fw-${cm.width}`);
            if (typeof cm.style !== "undefined") th.setAttribute("style", cm.style);

            tr_thead.appendChild(th);
        }

        thead.appendChild(tr_thead);
        table.appendChild(thead);
        const tbody = dce("tbody");
        const colCount = tr_thead.querySelectorAll("th").length;

        if (this.dataModel.Count <= 0) {

            const tr = dce("tr");
            const td = dce("td");
            td.setAttribute("colspan", colCount);
            td.textContent = "데이터가 없습니다.";
            td.classList = "text-center";
            tr.appendChild(td);
            tbody.appendChild(tr);
            table.appendChild(tbody);
        }
        else {
            //------------------------------------------------------------
            // body
            //------------------------------------------------------------
            for (const rowData of this.dataModel) {

                const tr = dce("tr");
                for (const cm of this.colModel) {

                    if (typeof cm.hidden !== "undefined" && typeof cm.hidden === "boolean" && cm.hidden) continue;

                    const td = dce("td");

                    if (typeof cm.valign !== "undefined" && ['left', 'right', 'center'].includes(cm.valign)) td.classList.add(`text-${cm.valign}`);
                    if (typeof cm.style !== "undefined") td.setAttribute("style", cm.style);

                    if (typeof cm.render !== "undefined") {
                        try {
                            const fnRtn = cm.render(rowData);

                            if (typeof fnRtn !== "undefined") {
                                td.innerHTML = cm.render(rowData);
                            }
                            else {
                                td.textContent = "ERR";
                            }
                        } catch (e) {
                            td.textContent = "ERR";
                        }
                    }
                    else {
                        td.textContent = rowData[cm.dataIndx];
                    }
                    tr.appendChild(td);
                }

                tbody.appendChild(tr);
            }
            table.appendChild(tbody);
            // html 구성후 페이징을 재구성 해야 해서 앞에 있음
            if (this.Pageing.active) {
                const tfoot = dce("tfoot");
                const tr_tfoot = dce("tr");
                const td_tfoot = dce("td");
                td_tfoot.setAttribute("colspan", colCount);
                td_tfoot.setAttribute("id", pagerID);
                td_tfoot.classList.add("text-center");
                tr_tfoot.appendChild(td_tfoot);
                tfoot.appendChild(tr_tfoot);
                table.appendChild(tfoot);
            }
        }

        this.append(table);

        if (this.Pageing.active) {
            Common.Pageing({
                obj: $(`#${pagerID}`)
                , total: this.Pageing.total
                , now: this.Pageing.index
                , pageLimit: this.Pageing.size
                , listLimit: this.Pageing.size
                , paramName: this.Pageing.pageIndexCtrlID
                , callback: function (val) {
                    $(`#${noGrid.Pageing.pageIndexCtrlID}`).val(val);
                    noGrid.Pageing.callBack();
                }
            });
        }
    }

    return noGrid;
}
