(function (factory) {
    'use strict';
    if (typeof define === 'function' && define.amd) {
        define(['jquery'], factory);
    } else if (typeof exports === 'object') {
        factory(require('jquery'));
    } else {
        if (typeof jQuery === 'undefined') {
            throw 'ReportViewer requires jQuery to be loaded first.';
        }
        jQuery.expr[':'].Contains = function (a, i, m) {
            return jQuery(a).text().toUpperCase()
              .indexOf(m[3].toUpperCase()) >= 0;
        };
        jQuery.expr[':'].contains = function (a, i, m) {
            return jQuery(a).text().toUpperCase()
              .indexOf(m[3].toUpperCase()) >= 0;
        };
        factory(jQuery);
    }
}(function ($) {
    'use strict';

    $.fn.reportViewer = function (options) {
        var $this = this;
        var defaults = $.fn.reportViewer.defaults;
        for (var k in defaults) {
            options[k] = options[k] || defaults[k];
        }
        if (typeof options.scroll !== "function") {
            options.scroll = scroll;
        }
        if (options.server == null || options.server.length === 0) {
            options.onError('Please set the url for server.');
            return;
        }
        //exportType 2,4,6,9
        //executeType 0 (show),1 export,2 findString,3 toggle
        $this.request = { path: null, sessionId: null, pageIndex: 1, pageCount: 0, findString: null, exportType: null, executeType: 0,reset:false };        

        
        //loaded function return response as {status,data:{content,sessionId,pageIndex,pageCount,fileName,stream }}
        //error function
        this.loadReport = function () {
            if ($this.loadding) { return;}
            $this.loadding = true;
            if (typeof options.onLoading === 'function') {
                options.onLoading($this.request,$this);
            }           
            var request = cloneDeep($this.request);           
            if (!request) {
                options.onError('Please set request.');
                return;
            } else {
                for (var name in options.request) {
                    //下面代码是让ctor里可以直接访问使用this._super访问父类构造函数，除了ctor的其他方法，this._super都是访问父类的实例
                    if (options.request[name])
                        request[name] = options.request[name];
                }
            }
            if (!request.path || request.path.length == 0) {
                options.onError('Please set the path of request for report.');
                return;
            }
            //request.sessionId = getOrSetSession();
            console.log(request);
            jQuery.ajax({
                url: options.server, type: "POST", contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(request), dataType: "json", processData: false,
                success: function(response) {
                    $this.loadding = false;
                    $this.request.reset = false;
                    if (response.status !== 0) {
                        options.onError(response.message);
                        return;
                    }
                    try {
                        //getOrSetSession(response.data.sessionId);
                        loadedSuccess(response.data, response,$this);
                        if (typeof options.onLoaded === 'function') {
                            options.onLoaded(response, request);
                        }
                    } catch (e) {
                        options.onError(e.message);
                    }
                },
                error: function(response,b,c) {
                    $this.loadding = false;
                    options.onError(response.message, response,$this);
                }
            });
            $this.request.executeType=0;
            $this.request.exportType=12;
        };
        function cloneDeep(obj) {
            if (obj == null) return null;
            var value = {};
            for (var m in obj) {
                value[m] = obj[m];
            }
            return value;
        }
        $this.loadding = false;
        var loadedSuccess = function (data, response) {
            switch ($this.request.executeType) {
                case 0://display
                    setHtml(data.contents);
                    break;
                case 1://export
                    $this.request.executeType = 0;
                    var blob = b64toBlob(data.stream, data.mimeType);
                    if (window.navigator.msSaveOrOpenBlob) {
                        navigator.msSaveBlob(blob, data.fileName);
                    } else {
                        var a = $("<a style='display: none;'/>");
                        var url = window.URL.createObjectURL(blob);
                        a.attr("href", url);
                        a.attr("download", data.fileName);
                        $("body").append(a);
                        a[0].click();
                        setTimeout(function () {
                            window.URL.revokeObjectURL(url);
                        }, 100);
                        //a.remove();
                    }
                    return;

                case 2://findString
                    $("#ssrs_find_text").data("index", 0);
                    if (data.pageIndex < 1) {
                        $("#ssrs_find_text").val("");
                        $("#ssrs_find_next").prop("disabled", true);
                        $("#ssrs_find_btn").prop("disabled", true);
                        options.onError(data.message, response);
                        $this.request.pageIndex = $("#ssrs_page").val() * 1;
                        return;
                    }
                    setHtml(data.contents);
                    findString();
                    break;
                case 3: //toggle
                    $this.request.executeType = 0;
                    setHtml(data.contents);
                    break;
                default:
                    $this.request.executeType=0;
                    options.onError('request.renderType is incorrect.', response);
                    return;
            }
            $this.request.sessionId = data.sessionId;
            $this.request.pageCount = data.pageCount;
            $this.request.pageIndex = data.pageIndex;

            init();
        };
        function setHtml(html)
        {
            $this.html(html);
            $this.find("a").attr("href", "javascript:;");
            $this.find("a").click(function () {
                $this.request.toggleId = $(this).parent().attr("id");
                if ($this.request.toggleId && $this.request.toggleId.length > 0) {
                    $this.request.executeType = 3;
                    $this.loadReport();
                    $this.request.toggleId = null;
                }                
            });
            $this.find("img").each(function (index, item) {
                if ($(this).attr("alt") === "+") {
                    this.src = options.webRootPath + "/images/Plus.gif";
                }
                else {
                    this.src = options.webRootPath + "/images/Minus.gif";
                }
            });
        }
        function scroll() {
            var p = $this.find(".currentFind").position();
            //console.log(p, $this.offset(), $this.find(".currentFind").offset(), $this.scrollTop());
            //$("body").scrollTo(p.left, p.top);
        }
        function getOrSetSession(sessionId) {
            if (sessionId) {
                var Days = 3; //此 cookie 将被保存 3 天
                var exp = new Date();    //new Date("December 31, 9998");
                exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
                document.cookie = "SSRS_SessionId=" + escape(sessionId) + ";expires=" + exp.toGMTString();
                return sessionId;
            }
            var x = /(^| )SSRS_SessionId=([^;]+)(;|$)/gi.exec(document.cookie);
            if (x) {
                return unescape(x[2]);
            }
        }
        function init() {
            $("#ssrs_pageNum").text($this.request.pageCount);
            $("#ssrs_page").val($this.request.pageIndex);
            if (options.toolBar && $(options.toolBar).length > 0 && $this.find("#ssrs_toolbar").length > 0) {
                if ($(options.toolBar).find("#ssrs_toolbar").length == 0) {
                    $(options.toolBar).html("");
                    $(options.toolBar).append($this.find("#ssrs_toolbar"));
                    $(options.toolBar).height("50");
                } else {
                    $this.find("#ssrs_toolbar").remove();
                }
            }
            $this.hover(function () { $(".ssrs_export_list").hide(); });
            //console.log($this.request);
            $("#ssrs_prev").prop("disabled", true);
            $("#ssrs_first").prop("disabled", true);
            $("#ssrs_next").prop("disabled", true);
            $("#ssrs_last").prop("disabled", true);
            $("#ssrs_find_btn").prop("disabled", true);
            $("#ssrs_find_next").prop("disabled", true);

            if ($this.request.pageIndex > 1 && $this.request.pageCount != 1) {
                $("#ssrs_prev").prop("disabled", false);
                $("#ssrs_first").prop("disabled", false);
            }
            if ($this.request.pageIndex < $this.request.pageCount && $this.request.pageCount != 1) {
                $("#ssrs_next").prop("disabled", false);
                $("#ssrs_last").prop("disabled", false);
            }
            if ($("#oReportDiv .FindString").length > 0) {
                $("#ssrs_find_next").prop("disabled", false);
                $("#ssrs_find_btn").prop("disabled", false);
            }
            if ($("#ssrs_find_text").val() && $("#ssrs_find_text").val().length > 0) {
                $("#ssrs_find_btn").prop("disabled", false);
            }
        }
        function setSpan() {
            var data = $("#ssrs_find_text").data();
            if ($("#oReportDiv .FindString").length === 0) {
                if ($("#oReportDiv *[id*=rsoHit]").length === 0) {
                    var vaules = $("#oReportDiv *:contains('" + data.findString + "'):not(:has(*))");
                    var reg = new RegExp("(" + data.findString + ")", "ig");
                    for (var v = 0; v < vaules.length; v++) {
                        var ele = $(vaules[v]);
                        var html = ele.html();
                        html = html.replace(reg, "<span id='rsoHit" + v + "' class='FindString'>$1</span>");
                        ele.html(html);
                    }
                } else {
                    $("#oReportDiv *[id*=rsoHit]").addClass("FindString");
                }
            }

        }
        function findString() {
            var data = $("#ssrs_find_text").data();
            data.findString = $("#ssrs_find_text").val();
            if (data.findString && data.findString.length > 0) {
                setSpan();
                if ($("#oReportDiv .FindString").length === 0) {
                    if ($this.request.pageIndex < $this.request.pageCount) {
                        $this.request.pageIndex++;
                    } else {
                        $this.request.pageIndex = 1;
                    }
                    $this.request.findString = data.findString;
                    $this.request.executeType = 2;
                    $this.loadReport();
                    return;
                }
                $("#ssrs_find_text").data("index", 0);
                $("#ssrs_find_text").data("length", $("#oReportDiv *[id*=rsoHit]").length);
                $("#oReportDiv *[id*=rsoHit]").removeClass("currentFind").attr("style", "");
                $("#oReportDiv *[id=rsoHit0]").addClass("currentFind");
                $("#ssrs_find_next").prop("disabled", false);
                $("#ssrs_find_btn").prop("disabled", false);
                options.scroll();
            }
        }
        function b64toBlob(b64Data, contentType, sliceSize) {
            contentType = contentType || '';
            sliceSize = sliceSize || 512;

            var byteCharacters = atob(b64Data);
            var byteArrays = [];

            for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
                var slice = byteCharacters.slice(offset, offset + sliceSize);

                var byteNumbers = new Array(slice.length);
                for (var i = 0; i < slice.length; i++) {
                    byteNumbers[i] = slice.charCodeAt(i);
                }

                var byteArray = new Uint8Array(byteNumbers);

                byteArrays.push(byteArray);
            }

            var blob = new Blob(byteArrays, { type: contentType });
            return blob;
        }
        (function () {
            $("body").on("click", "#ssrs_export", function () {
                $(".ssrs_export_list").show();
            });
            $("body").on("click", ".ssrs_export_list li", function () {
                $this.request.executeType = 1;
                $this.request.renderType = this.id.replace("__", "") * 1;
                $this.request.findString = null;
                $this.loadReport();
            });
            $("body").on("click", "#ssrs_prev", function () {
                $this.request.executeType = 0;
                if ($this.request.pageIndex > 1) {
                    $this.request.pageIndex = $this.request.pageIndex - 1;
                    $this.loadReport();
                }
                $("#ssrs_find_text").val($this.request.findString = null);
            });
            $("body").on("click", "#ssrs_first", function () {
                $this.request.executeType = 0;
                if ($this.request.pageIndex > 1) {
                    $this.request.pageIndex = 1;
                    $this.loadReport();
                }
                $("#ssrs_find_text").val($this.request.findString = null);
            });
            $("body").on("click", "#ssrs_next", function () {
                $this.request.executeType = 0;
                if ($this.request.pageIndex < $this.request.pageCount) {
                    $this.request.pageIndex = $this.request.pageIndex + 1;
                    $this.loadReport();
                }
                $("#ssrs_find_text").val($this.request.findString = null);
            });
            $("body").on("click", "#ssrs_last", function () {
                $this.request.executeType = 0;
                if ($this.request.pageIndex < $this.request.pageCount) {
                    $this.request.pageIndex = $this.request.pageCount;
                    $this.loadReport();
                }
                $("#ssrs_find_text").val($this.request.findString = null);
            });
            $("body").on("change", "#ssrs_page", function () {
                $this.request.executeType = 0;
                var index = $(this).val() * 1;
                if (index < 1) {
                    $(this).val($this.request.pageIndex);
                    options.onError("Error page number.");
                    return;
                }
                if (index > $this.request.pageCount) {
                    $(this).val($this.request.pageIndex);
                    options.onError("The page number cannot be than total pages number.");
                    return;
                }
                $this.request.pageIndex = index;
                $this.loadReport();
            });
            $("body").on("click", "#ssrs_refresh", function () {
                $this.request.pageIndex = 1;
                $this.request.sessionId = "";
                $this.request.reset = true;
                $this.request.executeType = 0;
                $this.request.exportType = null;
                $("#ssrs_find_text").val($this.request.findString = null);
                $this.loadReport();
            });
            $("body").on("keyup", "#ssrs_find_text", function (e) {
                var value = $(this).val();
                $(this).data("index", 0);
                $(this).data("length", 0);
                $this.find(".FindString").contents().unwrap();
                if (value && value.length > 0) {
                    $("#ssrs_find_btn").prop("disabled", false);
                } else {
                    $("#ssrs_find_btn").prop("disabled", true);
                }
                $("#ssrs_find_next").prop("disabled", true);
                if (e.keyCode == 10 || e.keyCode == 13) {
                    findString();
                }
            });
            $("body").on("click", "#ssrs_find_next", function () {
                var data = $("#ssrs_find_text").data();
                if (data.index < data.length - 1) {
                    data.index++;
                    $("#oReportDiv .currentFind").removeClass("currentFind");
                    $("#oReportDiv *[id=rsoHit" + data.index + "]").addClass("currentFind");
                    options.scroll();
                } else {
                    if ($this.request.pageIndex < $this.request.pageCount) {
                        $this.request.pageIndex++;
                    } else {
                        $this.request.pageIndex = 1;
                    }
                    $this.request.findString = $("#ssrs_find_text").val();
                    $this.request.executeType = 2;
                    $this.loadReport();
                }
            });
            $("body").on("click", "#ssrs_find_btn", function () {
                findString();
            });
        })(jQuery);
        return this;
    };

    $.fn.reportViewer.defaults = {
        toolBar: null,
        webRootPath:'',
        server: null,
        request: null,
        onError: function (message, response) { alert(message); console.error(message, response); },
        onLoading: function (request) { },
        onLoaded: function (response, data) { },
        scroll: null
    };
}));