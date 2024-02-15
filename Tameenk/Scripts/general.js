/*Click check box, if checked then set textbox value="Y", else Set "N"*/
//Alwayes Included
var currentPage;
function isExist(obj) {
    if (obj.length > 0) return true;
    return false;
}
function getDefaultedValue(obj) {
    if($(obj).is("select")){
        var ret = "";
        $(obj).find("option").each(function () {
            if ($(this).prop('defaultSelected') == true) {
                ret = $(this).val();
                return false;
            }
        });
        return ret;
    }else{
        return $(obj).prop("defaultValue");
    }
}
function belongHD(obj) {
    if (isExist($(obj).closest(masterContents()))) {
        return true;
    }
    return false;
}



/*Harara ***************************************************/
function nvl(val, val2) {
    if (val == undefined)
        return parseFloat(val2.toString().replace(/,/g, ''));
    if (trim(val.toString()) == "")
        return parseFloat(val2.toString().replace(/,/g, ''));
    return parseFloat(val.toString().replace(/,/g, ''));
}
function trim(str, chars) {
    if (chars == null || !chars) chars = "";
    return ltrim(rtrim(str, chars), chars);
}
function ltrim(str, chars) {
    chars = chars || "\\s";
    return str.replace(new RegExp("^[" + chars + "]+", "g"), "");
}

function rtrim(str, chars) {
    chars = chars || "\\s";
    return str.replace(new RegExp("[" + chars + "]+$", "g"), "");
}
function get_nearst(val, nearstFor) {
    if (!nearstFor || nearstFor == null || nearstFor == undefined)
        nearstFor = 2;
    var arr = new Array(10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000, 10000000000);
    var x = arr[nearstFor - 1];
    return Math.round(val * x) / x;
}
function ifZero(val, val2) {
    if (val == 0 || val == "0") {
        return val2;
    }
    return val;
}

function IsNull(val,val2) {
    if (val == ""){
        return val2;
    }
    return val;
}

  

function getObjChildNodes(obj, tagName) {
    return $(obj).children(tagName);
    //return obj.getElementsByTagName(tagName);
}
/********************************************************/

function Hash(Id) {
    return "#" + Id;
}
function arrToJson(arr) {
    var str = "";
    for (var i in arr) {
        if (str == "") {
            str = '"' + arr[i].name + '":' + '"' + arr[i].value + '"';
        } else {
            str = str + ', "' + arr[i].name + '":' + '"' + arr[i].value + '"'
        }
    }
    str = "{" + str + "}";
    return JSON.parse(str);
}
function showAlert(title, message, focusedElem) {
    if (Page.errorDialog != null) return;
    focusedElem = (focusedElem == undefined || focusedElem == null) ? $(Hash(Page.firstFocusedElemId)) : focusedElem;
    Page.errorDialog = BootstrapDialog.show({
        title: title,
        message: message,
        buttons: [{
            label: OkBtnLabel,
            action: function (errorDialog) {
                errorDialog.close(function () {
                    Page.errorDialog = null;
                });
                if ($(focusedElem).is("[type=button]") || $(focusedElem).is("[button]") || $(focusedElem).is("input[type=hidden]") || $(focusedElem).hasClass("hiddenField")) {
                    focusedElem = $(focusedElem).closest("tr").find("input,select,textarea,button").not($(".hiddenField")).filter(":visible").eq(0);
                }
                focusedElem.focus();
            }
        }]
    });
    $(window).on('hidden.bs.modal', function () {
        Page.errorDialog = null;
        $(window).off('shown.bs.modal');
        focusedElem.focus();
    });
}
function handleError(XMLHttpRequest, textStatus, error) {
    showAlert("Error", XMLHttpRequest.responseText);
    Page.endProcess();
}



function handleSuccess(result, status, req, fromType, parentTabContIfDt, fromBreakDown) {
   
    var hdr = req.getResponseHeader("content-type");
    if (hdr.indexOf("json") > -1) {
        
        if (result.errorTitle != undefined) {
            showAlert(result.errorTitle, result.errorMessage);
            Page.endProcess();
            return false;
        }
        if (result.NoShowAlert != undefined) {
            Page.endProcess();
            return false;

        }
        else if (fromType == "Save") {
            if (Page.screenType == "Search") {
                Page.searchReport();
            }
            else {
                var Id = (Page.afterSave == "Y") ? result.Id : "";
                var sentObj = { id: Id };
                sentObj = $.extend(sentObj, Page.antiFrogeryKey);
                if (Page.screenType == "Tabular") {
                    sentObj = $.extend(sentObj, Page.screenSearchParams);
                }
                if (Page.staticParam != null) {
                    sentObj = $.extend(sentObj, Page.staticParam);
                }
                var savetype = Page.saveType;
                $.post(Page.evalUrl(Page.getDataUrl), sentObj).success(function (result2, status, req) {

                    handleSuccess(result2, status, req);
                    Page.endProcess();
                    Page.runAfterSave(result.Id, savetype, result.ParentId);
                    Page.fadeMessage(result.Message);
                }).error(handleError);
            }
        }
        else if (fromType == "GridSave") {
           
            var page = currentPage;
             
            if (currentPage == "" || currentPage == undefined) {
                page = 1;
            }


            if (currentPage == -1) {
                page = 0;
            }
           

            var sentObj = arrToJson($(Hash("SearchForm")).serializeArray());
            sentObj = $.extend(sentObj, { Page: page, IsPartial: true });
            sentObj = $.extend(sentObj, Page.antiFrogeryKey);
            if (Page.screenType == "Tabular") {
                sentObj = $.extend(sentObj, Page.screenSearchParams);
            }
            if (Page.staticParam != null) {
                sentObj = $.extend(sentObj, Page.staticParam);
            }
            var savetype = Page.saveType;
            $.post(Page.evalUrl(Page.indexUrl), sentObj).success(function (result2, status, req) {
            
                handleSuccess(result2, status, req);
                Page.endProcess();
              
                Page.runAfterSave(result.Id, savetype);
               // Page.fadeMessage(result.Message);
            }).error(handleError);

        }
        else if (fromType == "CheckServer") {
            Page.runAfterServerSuccessCheck();
        }
    }
    else {
        if (fromType == "newDTRecord") {
            Page.currentDtRecord = $("<div>" + result + "</div>").children("table").eq($(result).children("table").length - 1);
            //alert($("<div>"+result+"</div>").children("table").eq($(result).children("table").length-1).html())
            parentTabContIfDt.append($(result));
            Page.currentDtRecord = parentTabContIfDt.children("table").eq(Page.currentDtRecord.children("table").length - 1);
            //alert(Page.dtFieldIndex)
            var nextElem = Page.currentDtRecord.find("input,select,textarea,button").eq(Page.dtFieldIndex);
            //alert(nextElem.attr("id"))
            Page.setFirstFocusedElemId(nextElem.attr("id"));
            nextElem.focus();
            if (fromBreakDown == undefined || fromBreakDown == null) {
                fromBreakDown = false;
            }
            if (fromBreakDown) {
                alert(result)
                Page.AssignOnlyOnceForRecord(getRecordTabContainer(Page.currentDtRecord));
            } else {
                Page.AssignOnlyOnceForRecord(Page.currentDtRecord);
            }
            Page.initAfterEachRequest();
        } else if (fromType == "SearchReport") {
            $("#SearchResultsContainer").html(result);
        }else {
            Page.setFirstFocusedElemId(Page.firstFocusedElemId);
            $(Hash(Page.ajaxUpdateTargetId)).html(result);
            if (Page.tabs.length > 0) {
                Page.drawTabs();
            }
            if (fromType == "executeSearch") {
                
                //if get data, Else stay where you are
                Page.searchIsActive = false;
                Page.searchExecuted = true;
                Page.showHideToolBar(["Next", "Prev", "First", "Last"], "show");
                Page.showHideToolBar(["ExecuteSearch", "CancelSearch"], "hide");
                if (Page.screenType == "Tabular" || Page.screenType == "MasterDetail") {
                    //alert();
                    Page.showHideToolBar(["StartSearch"], "show");
                }
            }
            Page.initAfterChangeAllPageContents();
        }
        Page.endProcess();
    }
}

function setGLobalVar(varName, varVal, varCat) {
    varCat = ($(varCat)) ? varCat : "";
    eval("window.__windowGobal" + varCat + varName + "=varVal");
}
function getGLobalVar(varName, varCat) {
    varCat = ($(varCat)) ? varCat : "";
    return eval("window.__windowGobal" + varCat + varName);
}

function ClickBox(obj) {
    var Table = $(obj).closest('table');
    var val = "N";
    if ($(obj).is(":checked")) {
        val = "Y";
    }
    Table.find('input[name="' + $(obj).attr("id").substr("chk_id_".length) + '"]').val(val);
}


function ClickBox2(obj) {
    var Table = $(obj).closest('table');
    var val = false;
    if ($(obj).is(":checked")) {
        val = true;
    }
    Table.find('input[name="' + $(obj).attr("id").substr("chk_id_".length) + '"]').val(val);
}

var Page;
function PageClass(moduleName, screenType, antiFrogeryKey) {
    this.antiFrogeryKey = antiFrogeryKey;
    //this.reset
    this.runAfterServerSuccessCheck = function () {

    }
    this.initAfterEachRequest = function () {
        this.setSaveType();
        //this.initContents = $(Hash(this.moduleName)).serialize();

        if ($(Hash(this.firstFocusedElemId)).length > 0) {
            $(Hash(this.firstFocusedElemId)).focus();
            this.setCurrentDtSourceElem($(Hash(this.firstFocusedElemId)))
        } else {
            var elem = $('input[type=text]:visible:enabled:first');
            elem.focus();
            this.setCurrentDtSourceElem($(Hash(elem.id)))
            this.firstFocusedElemId = null;
        }


        /*if ($(Hash(this.firstFocusedElemId)).length > 0) {
            $(Hash(this.firstFocusedElemId)).focus();
        } else {
            this.setFirstFocusedElemId($('input[type=text]:visible:enabled:first').attr("id"));
            $(Hash(this.firstFocusedElemId)).focus();
            this.setCurrentDtSourceElem($(Hash(this.firstFocusedElemId)))
        }*/
        try {
            this.resetValidation();
        } catch (err) {
            alert(err.description)
        }
        this.runGlobalCss();
        this.runGlobalJScript();
        Relation.applyRelations();
        //Islam please Reminde me if any probelm occured
        this.runPageSpecificFuncs();
        //this.initContents = $(Hash(this.moduleName)).serialize();
    }
    this.runGlobalCss = function () {
        
        $("[data-val-required]:not([readonly])").addClass('ReqTextBox');
        $("input[type=text][readonly]").addClass('ReadTextBox');

        $.each($('.datepicker'), function () {
            var format = globalDateFormat;
            if ($(this).attr("dateFormat") !== typeof undefined && $(this).attr("dateFormat") !== false && $(this).attr("dateFormat") != undefined) {
                format = $(this).attr("dateFormat");
            }
            format = format.toLowerCase().replace("yyyy", "yy");
            $(this).datepicker({
                inline: true,
                showOtherMonths: true,
                dateFormat: format,
                dayNamesMin: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']
            });
        });
    };

    this.runGlobalJScript = function () {
        jQuery.validator.methods.date = function (value, element) {
            return true;
        }
    };

    this.runPageSpecificFuncs = function () {
       
    };
    this.runAfterSave = function (recordId, saveType, parentId) {

    }
    this.staticParam = null;
    this.setStaticParam = function (param) {
        this.staticParam = param;
    }

    this.rebuildVaidations = function ()
    {
        $(Hash(this.moduleName)).removeData("validator");
        $(Hash(this.moduleName)).removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($(Hash(this.moduleName)));
    }

    this.resetValidation = function () {
        this.rebuildVaidations();
        $(Hash(this.moduleName)).find("input,select,textarea").filter("[data-val]").on("blur", function () {
            $(this).removeAttr("aria-describedby");
        })
    }
    this.MenuDirection = "ltr";
    this.initAfterChangeAllPageContents = function () {
        $("#TabContents[loadDataWhenScroll=true]").on("scroll", function () {
            if (this.scrollHeight == (parseInt($(this).scrollTop()) + parseInt($(this).height()))) {
                if (!Page.validateForm(false)) return;
                prevCall = $(this).attr("prevCall");
                var currentTime = new Date().getTime();
                if (
                        (prevCall === typeof undefined || prevCall === false || prevCall == undefined)
                        ||
                        ((currentTime - prevCall) >= 10)
                   ) {
                    $(this).append("<div id='loadingImg' class='fixed_loading' style='padding-left:85%'></div>");
                    TabContentLength = $(this).children("table").length;
                    var maxId = 0;
                    $(this).children("table").each(function () {
                        var elem = $(this).find("#" + Page.PageIdElem);
                        if (elem.val() != "0") {
                            maxId = elem.val();
                        }
                    });
                    var sentObj = { CurrentCount: TabContentLength, CurrentRecId: maxId };
                    sentObj = $.extend(sentObj, Page.antiFrogeryKey);
                    var Div = this;
                    $.post(Page.evalUrl("ScrollDown"), sentObj).success(function (result, status, req) {
                        if (result != "") {
                            handleSuccess(result, status, req, "newDTRecord", $(Div));
                            $(Div).attr("prevCall", new Date().getTime())
                            $(Div).scrollTop($(Div).scrollTop() - 50)
                        }
                        $(Div).find("#loadingImg").remove();
                    }).error(handleError)
                }
            }
        });
        Page.AssignOnlyOnceForRecord();
        Page.applyHeadMustBeFilledFirst();
        Page.initAfterEachRequest();
        this.initContents = $(Hash(this.moduleName)).serialize();
    }
    this.AssignOnlyOnceForRecord = function () {

    }
    this.OnlyOnceFunc = function () {

    }
    this.firstInit = function () {
        //this.initAfterEachRequest();
        this.initAfterChangeAllPageContents();
        this.OnlyOnceFunc();
        Relation.applyRelations();
        //this.initAfterEachRequest();
        //if (this.showMenu) {
        //    $(function () {
        //        $('#main-menu').smartmenus({
        //            subMenusSubOffsetX: 1,
        //            subMenusSubOffsetY: -8,
        //            showTimeout: 0,
        //            rightToLeftSubMenus: (Page.MenuDirection == "rtl") ? true : false
        //        });
        //        if (Page.MenuDirection == "rtl") {
        //            $('#main-menu').addClass("sm-rtl");
        //        }
        //    });
        //}
    }
    this.moduleName = moduleName;
    this.screenType = screenType;
    this.initContents = $(Hash(this.moduleName)).serialize();
    this.currentContents = function () {
        return $(Hash(this.moduleName)).serialize();
    };

    this.errorDialog = null;

    this.showMenu = true;
    this.fadeMessage = function (msg) {

        var LD = $(this.loadingDiv());
      //  alert($(this.loadingDiv()).html());
        LD.addClass("loading_message");
        //LD.css({ width: "auto" });
        LD.html(msg);
        LD.show();
        //LD.appendTo($(document).find("body"));
        LD.fadeOut(5000, function () {
            LD.hide()
        });
    }
    this.indexUrl = "Index";
    this.ListAllUrl = "ListAll";
    this.searchUrl = "Search";
    this.executeSearchUrl = "ExecuteSearch";
    this.saveUrl = "Save";
    this.deleteUrl = "Delete";
    this.listUrl = "List";
    this.printUrl = "Print";
    this.loaderElem = null;
    this.loadingDiv = function () {
      
        this.loaderElem = $("#pageLoadingDiv");
        //alert(this.loaderElem.length)
        if (this.loaderElem.length == 0) {
            this.loaderElem = $("<div id='pageLoadingDiv'></div>");
            $(this.loaderElem).appendTo($(document).find("body"));
           
           
        }
        $(this.loaderElem).removeClass();
        $(this.loaderElem).html("");
        $(this.loaderElem).hide();
        return this.loaderElem;
    }
    this.startProcess = function () {
        var LD = $(this.loadingDiv());
        LD.addClass("loading");
        LD.show();
        this.processRunning = true;
        //$(".datepicker").each(function () {
        //    $(this).datepicker("hide");
        //});
    };
    this.endProcess = function () {
        $(this.loadingDiv()).hide();
        this.processRunning = false;
    };
    this.ajaxUpdateTargetId = "formContents";
    this.toolBarId = "toolBar";
    this.serializeFormToArray = function () {
        return $(Hash(this.moduleName)).serializeArray();
    }

    this.afterSave = "Y";//Y Save and get the same record, N New Empty Record
    this.searchEnabled = true;
    this.saveEnabled = true;
    this.deleteEnabled = true;
    this.PageIdElem = "RecordKey";
    this.IdVal = function () {
        return $(Hash(this.PageIdElem)).val();
    };
    this.newEnabled = true;
    this.newDtEnabled = true;
    this.searchIsActive = false;
    this.searchExecuted = false;
    this.firstFocusedElemId = null;
    this.setFirstFocusedElemId = function (id) {
        this.firstFocusedElemId = id;
    }
    this.screenSearchParams = null;
    this.getDataUrl = "GetData";
    this.evalUrl = function (url) { 
        //alert("/" + this.moduleName + "/" + url)
        return "/" + this.moduleName + "/" + url;
    }
    //omar in hr
    this.RemoveRow = function (obj) {
        if (!confirm(SureToDeletedtRecord)) return;
        var table = $(obj).closest("tr").closest("table");
        var Id = $(table).find("input[id$='Id']").val();
        if (Page.screenType == "Search") return;
        if (belongHD(Page.sourceElem)) {
            Page.deleteRecord();
        }
        else {

            $(table).fadeOut(0, function () {
                var parentTabContainer = $(table).closest("#TabContents");
                if (Id == "0") {
                    var nextRecord = $(table).nextAll().filter(":visible").first()
                    nextRecord = (nextRecord.length == 0) ? $(table).prevAll().filter(":visible").first() : nextRecord;
                    Page.setCurrentDtSourceElem(null, nextRecord);
                    Page.sourceElem.focus();
                    $(table).remove();
                    reorderRows(parentTabContainer);
                }
                else {
                    $(table).find("input, select, textarea").each(function () {
                        //$(this).val($(this).prop("defaultValue"));
                        $(table).val(getDefaultedValue(this));
                    });
                    $(table).find("input[type=hidden][name$='].Deleted']").val(true);
                    var nextRecord = $(table).nextAll().filter(":visible").first();
                    nextRecord = (nextRecord.length == 0) ? $(table).prevAll().filter(":visible").first() : nextRecord;
                    Page.setCurrentDtSourceElem(null, nextRecord);
                    Page.sourceElem.focus();
                }
                Page.endProcess();
                if (nextRecord.length == 0) {//Last Record was Delete
                    Page.newRecord(true, parentTabContainer, true);
                }
                /*Harara*/
                Page.runAfterDeleteDetailRecord()
            });
        }
    }

    this.addRowByTab = function () {
        
        $("#TabContents").children("table").filter(":visible").last().find("tbody").first().find("tr").first().find("td:last").bind('keydown', function (e) {
          
            var keyCode = e.keyCode || e.which;
            if (keyCode == 9) {
                if (this.screenType == "Search") return;
                if (belongHD(this.sourceElem)) {
                    
                    Page.navigateRecords("Next");
                } else {
                    
                    Page.navigateDTRecords("Next");

                }
                e.preventDefault();
            }
        });

    }
    //------------------------
    this.navigateRecords = function (NT) {
        if (!this.searchExecuted) return;
        if (!this.checkUserHasMakeChanges()) return;
        var JsnObj = $.extend({}, this.screenSearchParams);
        if (this.IdVal() != "0") {
            JsnObj.CurrentRecId = this.IdVal();
        }
        JsnObj.NavigationType = NT;
        $.extend(JsnObj, this.antiFrogeryKey);
        this.startProcess();
        $.post(this.evalUrl(this.executeSearchUrl), JsnObj).success(handleSuccess).error(handleError)
    }
    this.setCurrentDtSourceElem = function (src, record) {
        if (src != undefined && src != null) {
            this.sourceElem = $(src);
            this.currentDtRecord = $(this.sourceElem).closest("#RecordContainer");
        } else if (record != undefined && record != null) {
            this.currentDtRecord = record;
            this.sourceElem = this.currentDtRecord.find("input,select,textarea,button").eq(this.dtFieldIndex);
        }
        this.dtFieldIndex = this.currentDtRecord.find("input,select,textarea,button").index(this.sourceElem);
        this.currentDtRecordId = this.currentDtRecord.find("input[type=hidden][id='RecordKey']").val()
        //alert(this.sourceElem)
        this.setFirstFocusedElemId(this.sourceElem.attr("id"));
    }
    this.sourceElem = null;
    this.dtFieldIndex = null;
    this.currentDtRecord = null;
    this.navigateDTRecords = function (NT) {
        
        var parentTabContainer = this.currentDtRecord.closest("#TabContents");
        if (NT == "Next") {
            var lastRecord = parentTabContainer.children("table").filter(":visible").last();
            if (this.currentDtRecord.is(lastRecord)) {
                this.newRecord(true, parentTabContainer);
                return;
            } else {
                this.setCurrentDtSourceElem(this.currentDtRecord.next().find("input,select,textarea,button").eq(this.dtFieldIndex));

                //this.currentDtRecord = $(this.currentDtRecord).next();
            }
        } else if (NT == "Previous") {
            //this.currentDtRecord = $(this.currentDtRecord).prev();
            this.setCurrentDtSourceElem(this.currentDtRecord.prev().find("input,select,textarea,button").eq(this.dtFieldIndex));
        }
        this.sourceElem.focus();
    }
    this.checkUserHasMakeChanges = function () {
        //alert(this.initContents)
        //alert(this.currentContents())
        if (this.initContents != this.currentContents()) {
            if (!confirm("You have made some changes, Continue?")) return false;
        }
        return true;
    }
    this.checkSearchIsActive = function () {
        if (this.searchIsActive) {
            showAlert(ValidationLabel, SearchIsActive);
            return false;
        }
        return true;
    }
    this.serializeForm = function () {
        return $(Hash(this.moduleName)).serialize();
    }
    this.processRunning = false;
    this.validateForm = function (showMessage) {
        $.validator.unobtrusive.parse($(Hash(this.moduleName)));
        var isValid = $(Hash(this.moduleName)).valid();
        var validator = $(Hash(this.moduleName)).validate();
        if (showMessage == undefined) {
            showMessage = true;
        }
        for (var i = 0; i < validator.errorList.length; i++) {
            if (showMessage) {
                showAlert(ValidationLabel, this.translate(validator.errorList[i].message), validator.errorList[i].element);
            }
            return false;
        }
        if (!isValid) {
            return false;
        }
        if (!this.validateAllHDDTExtra()) {
            return false;
        }
        return true;
    }

    this.validateHD = function (showMessage) {
        if (Page.screenType == "Tabular") return true;
        var M = $(Hash(this.moduleName)).find(masterContents());
        if (!M.find("input,select,textarea").valid()) {
            if (!Page.validateForm()) {
                return false;
            }
        }
        if (!this.validateAllHDDTExtra()) {
            return false;
        }
        return true;
    }
    this.enableMasterLess = true;
    this.applyHeadMustBeFilledFirst = function () {
        if (this.enableMasterLess) {
            detailContents().focusin(function () {
                if (!Page.validateHD(true)) return;
            })
        }
    }

    this.checkDtRecordEmpty = function (record) {
        var isEmpty = true;
        var rightCompareWithVal = "";
        var cntrlCrntVal = "";
        //alert($(record).html())
        $(record).find("input,select,textarea").filter(":not([readonly]):visible").each(function () {
            if ($(record).find("#RecordKey").val() == "0") {
                //if ($(this).is("input")) {
                //    rightCompareWithVal = getDefaultedValue(this);
                //} else if ($(this).is("select")) {
                //    rightCompareWithVal = getDefaultedValue(this);
                //}
                rightCompareWithVal = getDefaultedValue(this);
            }
            //alert($(this).attr("name") + "\nCurrent:" + cntrlCrntVal+", Old:"+rightCompareWithVal)

            cntrlCrntVal = $(this).val();
            if ($(this).is("input[type=checkbox]")) {
                cntrlCrntVal = $(this).is(":checked") ? "Y" : "";
            }
            //alert($(this).attr("name")+"\n\n"+cntrlCrntVal +"\n"+ rightCompareWithVal)
            if ( (parseInt(cntrlCrntVal) == 0 && parseInt(rightCompareWithVal) == 0) || (parseInt(cntrlCrntVal) == 0 && rightCompareWithVal == "") || (parseInt(rightCompareWithVal) == 0 && cntrlCrntVal == "")) {
                cntrlCrntVal =0;
                rightCompareWithVal = 0;
            }
            if (cntrlCrntVal != rightCompareWithVal)
            {
                isEmpty = false;
                return false;
            }
        });
        return isEmpty;
    }
    this.enableDisableDetailValidations = function (param, targetTab) {
        if (targetTab == null || targetTab == undefined) {
            targetTab = detailContents();
        }
        targetTab.find("input,select,textarea").filter("[data-val]").attr("data-val", param);
        this.resetValidation();
    }
    this.myValidate = function () {
        this.enableDisableDetailValidations(false);
        return this.validateForm();
    }
    this.saveType = "Save"; //By default;
    this.setSaveType = function () {//Save Or Edit
        if (this.IdVal() == "0") {
            this.saveType = "Save";
        } else {
            this.saveType = "Edit";
        }
    }
    this.saveRecord = function () {
        if (!this.saveEnabled) {
            showAlert(ValidationLabel, SaveNotAllowedLabel/* + this.moduleName*/);
            return;
        }
        if (this.processRunning) {
            showAlert("Process already running", "Please wait...");
            return;
        }
        if (!this.checkSearchIsActive()) return;
        /*if (this.screenType == "Tabular") {
            if (!(this.currentDtRecord.find("#RecordKey").val() == "0" && this.currentDtRecord.closest("#TabContents").children("table").filter(":visible").length == 1)) {
                if (!this.validateForm()) return;
            }
        }
        else */
        if (this.screenType == "MasterDetail" || this.screenType == "Tabular") {
            
            if (!this.myValidate()) return;
            var currentTabIndex = this.focusedTabIndex;
            for (var i = 0; i < this.tabs.length; i++) {
                $("#" + this.tabs[i].tabLinkId).trigger("click");
                var tabChilds = $("#" + this.tabs[i].targetId).find("#TabContents").children("table").filter(":visible");
                this.enableDisableDetailValidations(true, tabChilds);
                //Check if Can leave first record empty in Tab
                //alert(this.tabs[i].title + "\n" + this.checkDtRecordEmpty(tabChilds.eq(0)) + "\n" + this.tabs[i].settings.CanBeDtEmpty + "\n" + tabChilds.length)
                if (this.tabs[i].settings.CanBeDtEmpty && tabChilds.length == 1) {
                    //Check Empty of first and only alone record
                    if (this.checkDtRecordEmpty(tabChilds.eq(0))) {
                        this.enableDisableDetailValidations(false, tabChilds);
                        this.setDeleted(tabChilds.eq(0), true);
                        continue;
                    } else {
                        this.enableDisableDetailValidations(true, tabChilds);
                    }
                }
                if (!this.validateForm()) return;
            }
            $("#" + this.tabs[currentTabIndex].tabLinkId).trigger("click");
        }
        else {
            if (!this.validateForm()) return;
        }
        if (!this.validateBeforeSave()) {
            return;
        }
        this.startProcess();
        //alert(JSON.stringify(this.serializeFormToArray()))
        var params = this.serializeFormToArray();
        if (this.staticParam != null) {
            jQuery.each(this.staticParam, function (key, val) {
                //alert({ name: key, value: val })
                params[params.length] = { name: key, value: val };
            });
            //alert(params)
        }
        $.post(this.evalUrl(this.saveUrl), params).success(function (result, status, req) {
            handleSuccess(result, status, req, "Save");
        }).error(handleError);
    }

    this.GridsaveRecord = function () {
      
        if (!this.saveEnabled) {
            showAlert(ValidationLabel, SaveNotAllowedLabel/* + this.moduleName*/);
            return;
        }
        if (this.processRunning) {
            showAlert("Process already running", "Please wait...");
            return;
        }
        if (!this.checkSearchIsActive()) return;
        /*if (this.screenType == "Tabular") {
            if (!(this.currentDtRecord.find("#RecordKey").val() == "0" && this.currentDtRecord.closest("#TabContents").children("table").filter(":visible").length == 1)) {
                if (!this.validateForm()) return;
            }
        }
        else */if (this.screenType == "MasterDetail" || this.screenType == "Tabular") {
            if (!this.myValidate()) return;
            var currentTabIndex = this.focusedTabIndex;
            for (var i = 0; i < this.tabs.length; i++) {
                $("#" + this.tabs[i].tabLinkId).trigger("click");
                var tabChilds = $("#" + this.tabs[i].targetId).find("#TabContents").children("table").filter(":visible");
                this.enableDisableDetailValidations(true, tabChilds);
                //Check if Can leave first record empty in Tab
                if (this.tabs[i].settings.CanBeDtEmpty && tabChilds.length == 1) {
                    //Check Empty of first and only alone record
                    if (this.checkDtRecordEmpty(tabChilds.eq(0))) {
                        this.enableDisableDetailValidations(false, tabChilds);
                        this.setDeleted(tabChilds.eq(0), true);
                        continue;
                    } else {
                        this.enableDisableDetailValidations(true, tabChilds);
                    }
                }
                if (!this.validateForm()) return;
            }
            $("#" + this.tabs[currentTabIndex].tabLinkId).trigger("click");
        }
        else {
            if (!this.validateForm()) return;
        }
        if (!this.validateBeforeSave()) {
            return;
        }
        this.startProcess();
        //alert(JSON.stringify(this.serializeFormToArray()))
        var params = this.serializeFormToArray();
        if (this.staticParam != null) {
            jQuery.each(this.staticParam, function (key, val) {
                //alert({ name: key, value: val })
                params[params.length] = { name: key, value: val };
            });
            //alert(params)
        }
        $.post(this.evalUrl(this.saveUrl), params).success(function (result, status, req) {
            handleSuccess(result, status, req, "GridSave");
            //Page.fadeMessage(result.Message);
            this.loaderElem = $("#pageLoadingDiv1");
            //alert(this.loaderElem.length)
            if (this.loaderElem.length == 0) {
                this.loaderElem = $("<div id='pageLoadingDiv1'></div>");
                $(this.loaderElem).appendTo($("#resData"));
            }

            $(this.loaderElem).removeClass();
            $(this.loaderElem).html("");
            $(this.loaderElem).hide();
            var LD = $("#pageLoadingDiv1");
            //  alert($(this.loadingDiv()).html());
            LD.addClass("loading_message");
            //LD.css({ width: "auto" });
            LD.html(result.Message);
            LD.show();
            //LD.appendTo($(document).find("body"));
            LD.fadeOut(5000, function () {
                LD.hide()
            });


        }).error(handleError);
    }
    this.searchReportForRep = function () {
        if (!this.validateForm()) return;
        this.startProcess();
        var params = this.serializeFormToArray();
        if (this.staticParam != null) {
            jQuery.each(this.staticParam, function (key, val) {
                params[params.length] = { name: key, value: val };
            });
        }
     
        $.post(this.evalUrl(this.executeSearchUrl), params).success(function (result, status, req) {
            handleSuccess(result, status, req, "SearchReport");
        }).error(handleError);
    }


   
    this.searchReport = function () {
        //if (!this.validateForm()) return;
        this.startProcess();
        var params = this.serializeFormToArray();
        if (this.staticParam != null) {
            jQuery.each(this.staticParam, function (key, val) {
                params[params.length] = { name: key, value: val };
            });
        }
        $.post(this.evalUrl(this.indexUrl), params).success(function (result, status, req) {

            handleSuccess(result, status, req, "SearchReport");
        }).error(handleError);
    }

    this.GridPager = function (page) {

        var data = { page: page, IsPartial: true };
        $.ajax({
            url: this.evalUrl(this.indexUrl),
            data: data,
            type: 'GET',
            dataType: 'html',
            success: function (result, status, req) {

                $("#GridContainer").html(result);
            },
            error: function () {
                alert("error");
            }

        });
    }

    //this.getPage = function (page) {
    //    $("#Page").val(page);
    //    this.searchReport();
    //}
    this.listAll = function () {
        //if (!this.validateForm()) return;
        this.startProcess();
        var params = this.serializeFormToArray();
        if (this.staticParam != null) {
            jQuery.each(this.staticParam, function (key, val) {
                params[params.length] = { name: key, value: val };
            });
        }
        $.post(this.evalUrl(this.ListAllUrl), params).success(function (result, status, req) {
            handleSuccess(result, status, req, "SearchReport");
        }).error(handleError);
    }
    this.getPageForList = function (page) {
       
        currentPage = page;
        $("#Page").val(page);
        this.listAll();
        
    }
    this.getPage = function (page) {
        currentPage = page;
        $("#Page").val(page);
        
        if ($("#PagerType").val() == "" || $("#PagerType").val() == "searchReport") {
           
            this.searchReport();
        }
        else if ($("#PagerType").val() == "Grid") {
            this.GridPager(page);
        }
        else if ($("#PagerType").val() == "GridSearch") {
            this.GridSearch(page);

        }
    }
    this.goBack=function goBack() {
        // window.history.go(-1);
        location.href = "/Users/getUrl";
  
    }

    this.GriddeleteRecord = function (RecordId, isDetail, deleteCallBack) {

        var page = currentPage;
        if (currentPage == "" || currentPage == undefined) {
            page = 1;
        }

        if (currentPage == -1) {
            page = 0;
        }

        if (!isDetail) {
            if (this.IdVal() == "0") {
                showAlert(ValidationLabel, DeleteEmptyRecordNotAllowedLabel);
                return;
            }
        }
        if (!isDetail && !this.deleteEnabled) {
            showAlert(ValidationLabel, DeleteNotAllowedLabel + " " + this.moduleName);
            return;
        }
        //alert(this.processRunning)
        if (this.processRunning) {
            showAlert(ProcessingWait, PleaseWait + "...");
            return;
        }


        if (!this.checkSearchIsActive()) return;
        if (!isDetail) {
            if (!confirm(SureToDeleteRecord + " " + this.moduleName)) return;
        }
        if (isDetail) {
            var canDelete = Page.currentDtRecord.attr("canDelete");
            if (canDelete !== typeof undefined && canDelete !== false && canDelete != undefined) {
                if (canDelete.toLowerCase() == "false") {
                    showAlert(ValidationLabel, DeleteNotAllowedLabel + " " + this.moduleName);
                    return;
                }
            }
        }
        this.startProcess();
        if (!isDetail) {
            var JsonParams = this.deleteParams;
            if (this.deleteParams == null) {
                //JsonParams = { Id: this.IdVal() };
                JsonParams = { RecordId: this.IdVal() };
            }


            this.screenSearchParams = arrToJson($(Hash("SearchForm")).serializeArray());
            this.startProcess();
            var params = this.screenSearchParams;
            if (this.staticParam != null) {
                params = $.extend(params, this.staticParam, this.antiFrogeryKey);
            }

            params = $.extend(params, this.antiFrogeryKey);
            params = $.extend(params, { Page: page, RecordId: RecordId });
            $.post(this.evalUrl(this.deleteUrl), params).success(function (result, status, req) {
                var reply = handleSuccess(result, status, req);
                if (reply != false) {
                    reply = true;
                }
                if (reply) {
                    Page.fadeMessage("تم الحذف بنجاح...");
                }
                Page.runAfterDelete(reply);
            }).error(handleError);

        } else {
            try {
                this.deleteDetailRecord();
            } catch (err) {
                this.endProcess();
            }
        }
    }


    this.GridSearch = function (page) {

        this.screenSearchParams = arrToJson($(Hash("SearchForm")).serializeArray());
        this.startProcess();
        var params = this.screenSearchParams;
        if (this.staticParam != null) {
            params = $.extend(params, this.staticParam, this.antiFrogeryKey);
            
        }

        params = $.extend(params, this.antiFrogeryKey);
        params = $.extend(params, { Page: page, IsPartial: true });
        $.post(this.evalUrl(this.indexUrl), params).success(function (result, status, req) {
            handleSuccess(result, status, req);
        }).error(handleError);
    }


    this.showReport = function (ActionMethod) {
        if (!this.validateForm()) return;
        if (ActionMethod == null || ActionMethod == undefined) {
            ActionMethod ="ShowReport";
        }
        var oldAction = $(Hash(Page.moduleName)).attr("action");
        $(Hash(Page.moduleName)).attr("action", $(Hash(Page.moduleName)).attr("action") + "/" + ActionMethod);
        $(Hash(this.moduleName)).attr("target", "_new");
        $(Hash(this.moduleName)).submit();
        $(Hash(Page.moduleName)).attr("action", oldAction);
    }

    this.sortSearchResults = function (Order) {
        $(Hash(this.moduleName)).find("#Order").val(Order);
        this.searchReport();
    }



    this.checkServerRequestUrl = "";
    this.checkServerRequest = function () {
        if (!this.validateForm()) return;
        this.startProcess();
        $.post(this.evalUrl(this.checkServerRequestUrl), this.serializeFormToArray()).success(function (result, status, req) {
            handleSuccess(result, status, req, "CheckServer");
        }).error(handleError);
    }



    this.translateUrl = "/Translator";
    this.translate = function (p_word) {
        var reslt;
        $.ajax({
            type: 'POST',
            url: this.translateUrl,
            data: { word: p_word },
            success: function (result, status, req) { reslt = result; },
            async: false
        });
        return reslt;
    }
    this.list = function () {
        var params = "";
        if (this.staticParam != null) {
            jQuery.each(this.staticParam, function (key, val) {
                //alert({ name: key, value: val })
                if (params == "") {
                    params = key + "=" + val;
                } else {
                    params = params +"&"+ key + "=" + val;
                }
            });
            //alert(params)
            params = "?" + params;
        }

        window.open(this.evalUrl(this.listUrl) + params);
    }
    this.print = function () {
        if (this.IdVal() != 0) {
            window.open(this.evalUrl(this.printUrl) + "?Id=" + this.IdVal());
        }
    }
    this.newRecordParams = null;
    this.newRecord = function (isDetail, parentTabCont, fromDeleteLastRecord) {
      
        if (fromDeleteLastRecord == null || fromDeleteLastRecord == undefined) {
            fromDeleteLastRecord = false;
        }
        if (!fromDeleteLastRecord) {
            if ((!this.newEnabled && !isDetail) || (!this.newDtEnabled && isDetail)) {
                showAlert(ValidationLabel, CreateNotAllowedLabel + " " + this.moduleName);
                return;
            }
        }
        if (!this.checkSearchIsActive()) return;
        if (!isDetail) {
            if (!this.checkUserHasMakeChanges()) return;
        }
        var TabContentLength = 0;
        if (isDetail) {
            
            //this.enableDisableDetailValidations(true, $("#" + this.tabs[this.focusedTabIndex].targetId))
            if (!fromDeleteLastRecord) {
                if (!this.validateForm()) return;

            }
            TabContentLength = parentTabCont.children("table").length - 1
        }
        this.startProcess();
        var classObject = this;
        var JsonParam = this.newRecordParams;
        
        if (this.newRecordParams == null) {
            
            JsonParam = { RecordType: ((isDetail) ? "DT" : ""), RowIndex: TabContentLength, TabIndex: this.focusedTabIndex }
            JsonParam = $.extend(JsonParam, this.staticParam);
        }
         
        $.post(this.evalUrl(this.getDataUrl), $.extend(JsonParam, this.antiFrogeryKey)).success(function (result, status, req) {
            
            if (isDetail) {
                handleSuccess(result, status, req, "newDTRecord", parentTabCont);
            } else {
                handleSuccess(result, status, req);
                classObject.showHideToolBar(["ExecuteSearch", "CancelSearch"], "hide");
                classObject.showHideToolBar(["StartSearch"], "show");
                classObject.showHideToolBar(["Next", "Prev", "First", "Last"], "hide");
            }
            //}
        }).error(handleError);
    }

    this.newLoadFormRecord = function (isDetail, parentTabCont, fromDeleteLastRecord) {

        if (fromDeleteLastRecord == null || fromDeleteLastRecord == undefined) {
            fromDeleteLastRecord = false;
        }
        if (!fromDeleteLastRecord) {
            if ((!this.newEnabled && !isDetail) || (!this.newDtEnabled && isDetail)) {
                showAlert(ValidationLabel, CreateNotAllowedLabel + " " + this.moduleName);
                return;
            }
        }
        if (!this.checkSearchIsActive()) return;
        if (!isDetail) {
            if (!this.checkUserHasMakeChanges()) return;
        }
        var TabContentLength = 0;
        if (isDetail) {

            //this.enableDisableDetailValidations(true, $("#" + this.tabs[this.focusedTabIndex].targetId))
            if (!fromDeleteLastRecord) {
                if (!this.validateForm()) return;

            }
            TabContentLength = parentTabCont.children("table").length - 1
        }
        this.startProcess();
        var classObject = this;
        var JsonParam = this.newRecordParams;

        if (this.newRecordParams == null) {

            JsonParam = { RecordType: ((isDetail) ? "DT" : ""), RowIndex: TabContentLength, TabIndex: this.focusedTabIndex }
            JsonParam = $.extend(JsonParam, this.staticParam);
        }

        $.post(this.evalUrl(this.getDataUrl), $.extend(JsonParam, this.antiFrogeryKey)).success(function (result, status, req) {

            if (isDetail) {
                handleSuccess(result, status, req, "newDTRecord", parentTabCont);
            } else {
                handleSuccess(result, status, req);
                classObject.showHideToolBar(["ExecuteSearch", "CancelSearch"], "hide");
                classObject.showHideToolBar(["StartSearch"], "hide");
                classObject.showHideToolBar(["Next", "Prev", "First", "Last"], "hide");
            }
            //}
        }).error(handleError);
    }
    this.deleteParams = null;
    this.runAfterDelete = function () {

    }
    this.deleteRecord = function (isDetail, deleteCallBack) {
      
        if (!isDetail) {
            if (this.IdVal() == "0") {
                showAlert(ValidationLabel, DeleteEmptyRecordNotAllowedLabel);
                return;
            }
        }
        if (!isDetail && !this.deleteEnabled) {
            showAlert(ValidationLabel, DeleteNotAllowedLabel + " " + this.moduleName);
            return;
        }
        //alert(this.processRunning)
        if (this.processRunning) {
            showAlert(ProcessingWait, PleaseWait + "...");
            return;
        }

        if (!isDetail) {
            if (!this.checkUserHasMakeChanges()) return;
        }
        if (!this.checkSearchIsActive()) return;
        if (!isDetail) {
            if (!confirm(SureToDeleteRecord + " " + this.moduleName)) return;
        }
        if (isDetail) {
            var canDelete = Page.currentDtRecord.attr("canDelete");
            if (canDelete !== typeof undefined && canDelete !== false && canDelete != undefined) {
                if (canDelete.toLowerCase() == "false") {
                    showAlert(ValidationLabel, DeleteNotAllowedLabel + " " + this.moduleName);
                    return;
                }
            }
        }
        this.startProcess();
        if (!isDetail) {
            
            var JsonParams = this.deleteParams;
            if (this.deleteParams == null) {
             
                JsonParams = { RecordId: this.IdVal() };
                //edit by omar 2017
                if (this.staticParam != null) {
                    JsonParams = $.extend(JsonParams, this.staticParam);
                }

              
            }
            $.post(this.evalUrl(this.deleteUrl), $.extend(JsonParams, this.antiFrogeryKey)).success(function (result, status, req) {
                var reply = handleSuccess(result, status, req);
                if (reply != false) {
                    reply = true;
                }
                if (reply) {
                    Page.fadeMessage("تم الحذف بنجاح...");
                }
                Page.runAfterDelete(reply);
            }).error(handleError);
        } else {
           
            try {
                this.deleteDetailRecord();
            } catch (err) {
                this.endProcess();
            }
        }
    }

    this.deleteDetailRecord = function () {
        var classObject = this;
        this.currentDtRecord.fadeOut(0, function () {
            var parentTabContainer = $(this).closest("#TabContents");
            if (Page.currentDtRecordId == "0") {
                var nextRecord = $(this).nextAll().filter(":visible").first()
                nextRecord = (nextRecord.length == 0) ? $(this).prevAll().filter(":visible").first() : nextRecord;
                classObject.setCurrentDtSourceElem(null, nextRecord);
                classObject.sourceElem.focus();
                $(this).remove();
                reorderRows(parentTabContainer);
            }
            else {

                $(this).find("input, select, textarea").each(function () {
                    //$(this).val($(this).prop("defaultValue"));
                    $(this).val(getDefaultedValue(this));
                });
                classObject.setDeleted(this, true);
                var nextRecord = $(this).nextAll().filter(":visible").first()
                nextRecord = (nextRecord.length == 0) ? $(this).prevAll().filter(":visible").first() : nextRecord;
                //alert(nextRecord)
                classObject.setCurrentDtSourceElem(null, nextRecord);
                classObject.sourceElem.focus();
            }
            classObject.endProcess();
            if (nextRecord.length == 0) {//Last Record was Delete
                classObject.newRecord(true, parentTabContainer, true);
            }
            //alert(1)
            /*Harara*/
            Page.runAfterDeleteDetailRecord()
        });
    }
    /*Harara*/
    this.runAfterDeleteDetailRecord = function () {

    }

    this.setDeleted = function (record, val) {
        $(record).find("input[type=hidden][name$='].Deleted']").val(val);
    }
    this.validateBeforeSave = function () {
        return true;
    }

    this.validateHDExtra = function () {
        return true;
    }

    this.validateDTExtra = function () {
        return true;
    }
    this.validateAllHDDTExtra = function () {
        if (!this.validateHDExtra() || !this.validateDTExtra()) {
            return false;
        }
        return true;
    }
    this.prepareForSearch = function () {
        //Override your function
    }
    this.startSearchMode = function () {
        if (!this.checkSearchIsActive()) return;
        if (!this.searchEnabled) {
            showAlert(ValidationLabel, SearchNotAllowed);
            return;
        }
        if (!this.checkUserHasMakeChanges()) return;
        this.startProcess();
        var classObject = this;
        var params = this.antiFrogeryKey;
        if (this.staticParam != null) {
            params = $.extend(params, this.staticParam);
        }
        $.post(this.evalUrl(this.searchUrl), params).success(function (result, status, req) {
            var reply = handleSuccess(result, status, req);
            if (reply == false) {
                return;
            }
            classObject.searchIsActive = true;
            classObject.searchExecuted = false;
            classObject.prepareForSearch();
            classObject.showHideToolBar(["ExecuteSearch", "CancelSearch"], "show");
            classObject.showHideToolBar(["StartSearch"], "hide");
            classObject.showHideToolBar(["Next", "Prev", "First", "Last"], "hide");
        }).error(handleError);
    }

    this.executeSearch = function () {
    
        if (!this.searchEnabled) {
           
            showAlert(ValidationLabel, SearchNotAllowed);
            return;
        }

        if (this.searchIsActive) {//F8 From F7
          
            this.screenSearchParams = arrToJson(this.serializeFormToArray());
        } else {
            //this.screenSearchParams = {};
            if (!this.checkUserHasMakeChanges()) return;
            this.screenSearchParams = $.extend({}, this.antiFrogeryKey);
        }
        //alert(JSON.stringify(this.screenSearchParams))
        //alert(JSON.stringify(this.screenSearchParams))
        this.startProcess();
        var params = this.screenSearchParams;
        if (this.staticParam != null) {
            params = $.extend(params, this.staticParam);
        }
        $.post(this.evalUrl(this.executeSearchUrl), params).success(function (result, status, req) {
           
            handleSuccess(result, status, req, "executeSearch");
        }).error(handleError);
    }


    this.cancelSearch = function () {
        
        this.startProcess();
        var classObject = this;
        $.post(this.evalUrl(this.getDataUrl), this.antiFrogeryKey).success(function (result, status, req) {
            
            handleSuccess(result, status, req);
            classObject.screenSearchParams = null;
            classObject.searchIsActive = false;
            classObject.searchExecuted = false;
            classObject.showHideToolBar(["ExecuteSearch", "CancelSearch"], "hide");
            classObject.showHideToolBar(["StartSearch"], "show");
        }).error(handleError);
    }

    this.enableToolBar = function (p_arr, param) {
        if (p_arr != "*") {
            for (var i = 0; i < p_arr.length; i++) {
                p_arr[i] = p_arr[i] + "Btn";
            }
        }
        if (param == undefined || param == null) {
            param = false;
        }
        if (p_arr == "*") {
            $(hash(this.toolBarId) + " input").attr("disabled", param);
        } else {
            for (var i = 0; i < p_arr.length; i++) {
                $(Hash(this.toolBarId)).find(Hash(p_arr[i])).attr("disabled", param);
            }
        }
    }
    this.showHideToolBar = function (p_arr, param) {
        if (p_arr != "*") {
            for (var i = 0; i < p_arr.length; i++) {
                p_arr[i] = p_arr[i] + "Btn";
            }
        }
        if (param == undefined || param == null) {
            param = "hide";
        }
        if (param == "hide") {
            if (p_arr == "*") {
                $(Hash(this.toolBarId) + " input").hide();
            } else {
                for (var i = 0; i < p_arr.length; i++) {
                    $(Hash(this.toolBarId)).find(Hash(p_arr[i])).hide();
                }
            }
        }
        else if (param == "show") {
            if (p_arr == "*") {
                $(Hash(this.toolBarId) + " input").show();
            } else {
                for (var i = 0; i < p_arr.length; i++) {
                    $(Hash(this.toolBarId)).find(Hash(p_arr[i])).show();
                }
            }
        }
    }

    this.tabs = [];
    this.focusedTabIndex = 0;
    function tab(title, targetId, settings) {
        this.title = title;
        this.targetId = targetId;
        this.settings = settings;
        this.tabLinkId = null;
        if (settings == undefined || settings == null) {
            this.settings = { CanBeDtEmpty: false };
        }
    }

    this.addTab = function (title, targetId, settings) {
        if (targetId == null || targetId == undefined) {
            targetId = title.replace(/ /g, "");
        }
        this.tabs.push(new tab(title, targetId, settings));
        return this.tabs[this.tabs.length - 1];
    }
    this.drawTabs = function () {
        var tabsStr = '<ul id="tabsLabels" class="nav nav-tabs">';
        for (var i = 0; i < this.tabs.length; i++) {
            this.tabs[i].tabLinkId = "tabLink" + this.tabs[i].targetId;
            tabsStr = tabsStr + '<li><a id="' + this.tabs[i].tabLinkId + '" href="#' + this.tabs[i].targetId + '" data-toggle="tab">' + this.tabs[i].title + '</a></li>';
        }
        tabsStr = tabsStr + '</ul>';
        var tabsObj = $(tabsStr);
        tabsObj.find("a").each(function (index) {
            $(this).on("click", function () {
                if (Page.focusedTabIndex == index) return;
                var tabChilds = $("#" + Page.tabs[Page.focusedTabIndex].targetId).find("#TabContents").children("table").filter(":visible");
                //alert(tabChilds.length+"\n"+Page.checkDtRecordEmpty(tabChilds.eq(0)))
                if (tabChilds.length == 1) {
                    if (Page.checkDtRecordEmpty(tabChilds.eq(0))) {
                        tabChilds.find("input,select,textarea").filter("[data-val]").attr("data-val", "false");
                        Page.resetValidation();
                    } else {
                        tabChilds.find("input,select,textarea").filter("[data-val]").attr("data-val", "true");
                        Page.resetValidation();
                        if (!Page.validateForm()) {
                            event.cancelBubble = true;
                            return;
                        }
                    }
                }
                Page.resetValidation();
                if (!Page.validateForm()) {
                    event.cancelBubble = true;
                    return;
                }
                Page.focusedTabIndex = index;
            })
        });
        detailContents().prepend(tabsObj);
        tabsObj.find("a").eq(this.focusedTabIndex).trigger("click");
    }
    this.focusNextElem = function (currentObj) {
        var currentIndex = $("#formContents").find("input,select,textarea").filter(":visible").index($(currentObj));
        var nextElem = $("#formContents").find("input,select,textarea").filter(":visible").eq(currentIndex + 1);
        nextElem.focus();
    }
    this.handleKeys = function (ev) {
        if (this.errorDialog != null) return;
        //alert(ev.target.id)
        this.setCurrentDtSourceElem(ev.target);
        if (ev.which == 13)//Enter
        {
            this.focusNextElem(ev.target);
            ev.preventDefault();
        }
        else if (ev.which == 120)//F120 LOV
        {
            clickBtnLov(ev);
            ev.preventDefault();
        }
        else if (ev.which == 121)//F10 Save record
        {
            if (this.screenType == "Search") return;
            this.saveRecord();
            ev.preventDefault();
        }
        else if (ev.which == 118)//F7 Search Mode
        {
            if (this.screenType == "Search") return;
            this.startSearchMode();
            ev.preventDefault();
        }
        else if (ev.which == 119)//F8 Execute Search Mode
        {
            if (this.screenType == "Search") return;
            this.executeSearch();
            ev.preventDefault();
        }
       

        else if (ev.which == 33)//PageUp previous record
        {
            if (this.screenType == "Search") return;
            if (belongHD(this.sourceElem)) {
                this.navigateRecords("Previous");
            } else {
                this.navigateDTRecords("Previous");
            }
            ev.preventDefault();
        }
        
        //else if (ev.which == 9) {
        //        if (this.screenType == "Search") return;
        //        if (belongHD(this.sourceElem)) {
        //            this.navigateRecords("Next");
        //        } else {
        //            $("#TabContents").children("table").filter(":visible").last().find("tbody").first().find("tr").first().find("td:last").keydown(function (e) {
        //                alert("200");
        //                ev.preventDefault();
        //                    Page.navigateDTRecords("Next");    
        //            });
        //        }
              
        //}
        else if (ev.which == 45)//34PageDown Next record
        {


            if (this.screenType == "Search") return;
            if (belongHD(this.sourceElem)) {
                this.navigateRecords("Next");
            } else {
                //here call func tab
                this.navigateDTRecords("Next");
            }
            ev.preventDefault();
        }
        else if (ev.which == 46)//123F12 Delete record,46 delete button
        {
            if (!confirm(SureToDeletedtRecord)) return;
            if (this.screenType == "Search") return;
            if (belongHD(this.sourceElem)) {
                this.deleteRecord();
            }
            else {
                //this.currentDtRecord = $(this.sourceElem).closest("#RecordContainer");
                //deleteRecord(srcTable);
                this.deleteRecord(true);
            }
           ev.preventDefault();
        }

    }
}

$(document).ready(function () {
    $(document).on("keydown", function (ev) {
        Page.handleKeys(ev);
    });
});



function reorderRows(parentDetailTab) {
    //alert($(parentDetailTab).children("table").length)
    $(parentDetailTab).children("table").each(function () {
        var index = $(this).index();
        //alert(index)
        $(this).find("input, select, textarea").each(function () {
            if (this.hasAttribute("name")) {
                newName = $(this).attr("name").replace(/\[(\d+)\]./, "[" + index + "].")
                $(this).attr("name", newName);
            }
            if (this.hasAttribute("id")) {
                newId = $(this).attr("id").replace(/\_(\d+)\_./, "_" + index + "__")
                $(this).attr("id", newId);
            }
        })
    })
}

function Translate(obj) {
    var dialog = BootstrapDialog.show({
        title: TranslateLabel,
        onshow: function (dialog) {
            dialog.setMessage("<p align=center class='loading' style='padding:6px;'>&nbsp;</p>");
        },
        onshown: function (dialog) {
            $.post("/Translator/TranslateOnFlyEdit", { Word: $(obj).text() }).success(function (result, status, req) {
                var translatePopUp = $('<div id="translatePopUp"></div>')
                translatePopUp.append(result);
                dialog.setMessage(translatePopUp);
                translatePopUp.find("input[type=text]:not([readonly]):visible:enabled:first").focus()
            }).error(handleError);
        },
        draggable: true,
        buttons: [{
            label: OkBtnLabel,
            action: function (dialog) {
                var form = $(dialog.getMessage()).find("#Translator").first();
                $.validator.unobtrusive.parse(form);
                var isValid = form.valid();
                var validator = form.validate();
                for (var i = 0; i < validator.errorList.length; i++) {
                    showAlert(ValidationLabel, Page.translate(validator.errorList[i].message), validator.errorList[i].element);
                    return;
                }
                if (!isValid) {
                    return;
                }
                $.post($(form).attr("action"), $(form).serializeArray()).success(function (result, status, req) {
                    $(obj).parent().html(result);
                    dialog.close();
                }).error(handleError);
            }
        }, {
            label: CancelBtnLabel,
            action: function (dialog) {
                dialog.close();
            }
        }]
    });
    //$(window).on('shown.bs.modal', function () {
    //    drawLovContents();
    //    for (var propKey in modalOptions) {
    //        $(".modal-dialog").css(propKey, modalOptions[propKey]);
    //    }
    //});
    //$(window).on('hidden.bs.modal', function () {
    //    lovDialog = null;
    //    $(window).off('shown.bs.modal');
    //});

    //$.post("/Translator/TranslateOnFlyEdit", { Word: $(obj).text() }).success(function (result, status, req) {
    //    //handleSuccess(result, status, req, "Save");
    //}).error(handleError);
}

function EmptyMasterElems(Prefix, arr) {
    if (Prefix != "") {
        Prefix = Prefix + ".";
    }
    for (var i = 0; i < arr.length; i++) {
        masterContents().find("[name='" + Prefix + arr[i] + "']").val("");
    }
}

function AddClassToMasterElems(Prefix, arr, className) {
    if (Prefix != "") {
        Prefix = Prefix + ".";
    }
    for (var i = 0; i < arr.length; i++) {
        masterContents().find("[name='" + Prefix + arr[i] + "']").addClass(className);
    }
}


function RemoveClassFromMasterElems(Prefix, arr, className) {
    if (Prefix != "") {
        Prefix = Prefix + ".";
    }
    for (var i = 0; i < arr.length; i++) {
        masterContents().find("[name='" + Prefix + arr[i] + "']").removeClass(className);
    }
}

function fillSelectBox(url, params, srcElement, TargetElementName, addEmptyOption, fireFunc) {
    addEmptyOption = (addEmptyOption == undefined || addEmptyOption == null) ? false : addEmptyOption;
    var srcName = $(srcElement).attr("name");
    var prefix = srcName.substr(0, srcName.lastIndexOf(".") + 1)
    var TargetElement = $(srcElement).closest("#RecordContainer").find("[name='" + prefix + TargetElementName + "']");
    $.post(url, params).success(function (result, status, req) {
        //handleSuccess(result, status, req);
        TargetElement.empty();
        if (addEmptyOption) {
            $("<option>", { value: "", text: "" }).appendTo(TargetElement);
        }
        $(result).each(function (index) {
            $("<option>", { value: this.Id, text: this.Name }).appendTo(TargetElement);
        });
        if (fireFunc != undefined) {
            fireFunc();
        }
    }).error(handleError);
}
function getServerValues(Container, url, params, elements, fireFunc) {
    params = $.extend(params, Page.antiFrogeryKey);
    $.post(url, params).success(function (result, status, req) {
        $(result).each(function (index) {
            var elem;
            if (Container == "") {
                elem = getElemByName(elements[index]);
            } else {
                getDtElemByName(elements[index], Container);
            }
            elem.val(this.Val);
            elem.trigger("change");//fireOf elements changes
        });
        if (fireFunc != undefined) {
            fireFunc();
        }
        

    }).error(handleError);
}

function getElemRecord(elem) {
    return $(elem).closest("#RecordContainer");
}

function getSafeCrncy(safeElemName, CurrencyElemName, fireFunc) {
    var safeId = getElemByName(safeElemName).val()
    getServerValues("", "/Valids/ChangeSafe", { SafeId: safeId }, [CurrencyElemName], fireFunc);
}



function getCrncyRate(crncyElemName, dateElemName, rateElemName, fireFunc) {
    var crncyId = getElemByName(crncyElemName).val()
    getServerValues("", "/Valids/GetCurrencyRate", { CurrencyId: crncyId, Date: getElemByName(dateElemName).val() }, [rateElemName], fireFunc);
}


function getDtCrncyRate(evt, crncyElemName, dateElemName, rateElemName, fireFunc) {
    var Record = getElemRecord($(evt.target));
    var crncyId = getDtElemByName(crncyElemName, Record).val()
    getServerValues(Record, "/Valids/GetCurrencyRate", { CurrencyId: crncyId, Date: getDtElemByName(dateElemName, Record).val() }, [rateElemName], fireFunc);
}



//////////////////// Islam //////////////////////////////////////////////////////




function IsDetailEmpty() {

    var records = $("#TabContents").find("table[id=RecordContainer]").filter(":visible");
    var check = "Y";

    if ($(records).length > 1) {
        return "N";
    }

     

    if (Page.checkDtRecordEmpty($(records).first()) == false)
    {
        return "N";
    }

    


    return check;

}


function deleteAllRecords() {
   
    if (!masterContents().find("input,select,textarea").valid())
        if (!Page.validateForm()) return;

    if (IsDetailEmpty() == "N") {
        BootstrapDialog.show({
            title: "Warning",
            message: 'Are you sure deleteing records?',
            buttons: [{
                label: 'Ok',
                action: function (dialog) {
                    $("#TabContents").find("table[id=RecordContainer]").filter(":visible").each(function () {
                        Page.currentDtRecord = $(this);
                        Page.processRunning = false;
                        Page.deleteRecord(true);
                    });
                    dialog.close();

                }
            }, {
                label: 'Cancel',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });

    }

}

function getBreakDownDt(url, params) {

    if (!masterContents().find("input,select,textarea").valid())
        if (!Page.validateForm()) return;
    if (IsDetailEmpty() == "N") {
        showAlert(ValidationLabel, Page.translate("Record must be deleted first."));
        return;
    }

    $("#TabContents").find("table[id=RecordContainer]").filter(":visible").remove();
    params = $.extend(params, { RowIndex: ($("#TabContents").find("table[id=RecordContainer]").length - 1) }, Page.antiFrogeryKey);
    $.post(url, params).success(function (result, status, req) {
        handleSuccess(result, status, req, "newDTRecord", $("#TabContents"), true);

    });

}


//////////////////// Islam //////////////////////////////////////////////////////


function getElemByName(p_name) {
    return $("[name='" + p_name + "']");
}


function getDtElemByName(p_name, container) {
    if (container == null || container == undefined) {
        return $("[name$=']." + p_name + "']");
    }
    var elem = container.find("[name$=']." + p_name + "']");
    if ($.isEmptyObject($(elem)[0])) {
        return container.find("[name='" + p_name + "']")
    }
    return elem;
}



function EmptyDetailElems(container, arr) {
    EmptyElems(container, "]", arr)
    //Prefix = "].";
    //for (var i = 0; i < arr.length; i++) {
    //    container.find("[name$='" + Prefix + arr[i] + "']").val("");
    //}
}
function EmptyHeadElems(container, arr) {
    EmptyElems(container, "", arr)
    //Prefix = "";
    //for (var i = 0; i < arr.length; i++) {
    //    container.find("[name$='" + Prefix + arr[i] + "']").val("");
    //}
}

function EmptyElems(container, Prefix, arr) {
    if (Prefix != "") {
        Prefix = Prefix + ".";
    }
    var expr = "name=";
    if (Prefix == "].") {
        expr = "name$=";
    }
    for (var i = 0; i < arr.length; i++) {
        container.find("[" + expr + "'" + Prefix + arr[i] + "']").val("");
    }
}
function distinctObjArr(array) {
    var singles = Array();
    $.each(array, function (i, el) {
        var exist = false;
        $.each(singles, function (x, y) {
            if ($(y).is($(el))) {
                exist = true;
            }
        });
        if (!exist) {
            singles.push(el);
        }
    });
    return singles;
}

var Relation = new Relation();
function Relation() {
    this.allowEmpty = true;
    this.DTTabObj = Array();
    this.DTElements = Array();
    this.DTDepends = Array();
    this.DTProps = Array();
    this.DTExcldFrmDisblReadOnly = Array();
    this.addDtRelation = function (tabObj, elemName, dependants, props, ExcldFrmDisblReadOnly) {
        this.DTTabObj[this.DTTabObj.length] = tabObj;
        this.DTElements[this.DTElements.length] = elemName;
        this.DTDepends[this.DTDepends.length] = dependants;
        this.DTProps[this.DTProps.length] = props;
        this.DTExcldFrmDisblReadOnly[this.DTExcldFrmDisblReadOnly.length] = ExcldFrmDisblReadOnly;
    }
    this.applyDtRelations = function () {
        var distinctTabObjs = distinctObjArr(this.DTTabObj);
        var Rel = this;
        $.each(distinctTabObjs, function (tabIndex, tabObj) {
            //alert(tabIndex + "\n" + tabObj.targetId)
            var tabContainer = detailContents().find("#" + tabObj.targetId).find("#TabContents")
            tabContainer.children("table").each(function () {
                var tableRecord = $(this);
                $.each(Rel.DTTabObj, function (index2, tabObj2) {
                    if ($(tabObj).is($(tabObj2))) {
                        addRecordRelation(tableRecord, Rel.DTElements[index2], Rel.DTDepends[index2], Rel.DTProps[index2], Rel.DTExcldFrmDisblReadOnly[index2])
                    }
                });


            })
        })
    }

    this.HDElements = Array();
    this.HDDepends = Array();
    this.HDProps = Array();
    this.HDExcldFrmDisblReadOnly = Array();
    this.HDfireEventIfTrue = Array();
    this.HDexecuteWhenEvenetFinishedFired = Array();
    this.addHdRelation = function (elemName, dependants, props, ExcldFrmDisblReadOnly, fireEventIfTrue, executeWhenEvenetFinishedFired) {
        this.HDElements[this.HDElements.length] = elemName;
        this.HDDepends[this.HDDepends.length] = dependants;
        this.HDProps[this.HDProps.length] = props;
        this.HDExcldFrmDisblReadOnly[this.HDExcldFrmDisblReadOnly.length] = ExcldFrmDisblReadOnly;
        if (fireEventIfTrue == undefined || fireEventIfTrue == null) {
            fireEventIfTrue = null;
        }
        this.HDfireEventIfTrue[this.HDfireEventIfTrue.length] = fireEventIfTrue;
        if (executeWhenEvenetFinishedFired == undefined || executeWhenEvenetFinishedFired == null) {
            executeWhenEvenetFinishedFired = null;;
        }
        this.HDexecuteWhenEvenetFinishedFired[this.HDexecuteWhenEvenetFinishedFired.length] = executeWhenEvenetFinishedFired;
    }
    this.applyHdRelations = function () {
        var Rel = this;
        $.each(Rel.HDElements, function (index, elemName) {
            addRecordRelation(masterContents().find("#RecordContainer"), elemName, Rel.HDDepends[index], Rel.HDProps[index], Rel.HDExcldFrmDisblReadOnly[index], Rel.HDfireEventIfTrue[index], Rel.HDexecuteWhenEvenetFinishedFired[index])
        });
    }
    this.applyRelations = function () {
        this.applyHdRelations();
        this.applyDtRelations();
        this.applyClosingFields();
    }

    this.closedHeadFields = Array();
    this.closeHeadFields = function(f){
        this.closedHeadFields = f;
    }
    this.applyClosingFields = function(){
        $(masterContents()).on("mouseover", function () {
            $.each(Relation.closedHeadFields, function (index, value) {
                var Records = getTabContentsContainer(value.Tab).children("table[id='RecordContainer']").filter(":visible");
                var readOnly;
                //alert(Records.eq(0).prop("outerHTML"))
                if (Records.length > 1 || (Records.length >= 0 && !Page.checkDtRecordEmpty(Records.eq(0)) ) )  {
                    readOnly = true;
                } else {
                    readOnly = false;
                }
                SetHdItemsProperty(value.closeElements, "readonly", readOnly,true);
            });
        });
    }

}
function closeHeadFields(fields){
    //alert(fields)
    Relation.closeHeadFields(fields)
}
function addHdRelation(elemName, Dependants, props, ExcldFrmDisblReadOnly, fireEventIfTrue, executeWhenEvenetFinishedFired) {
    Relation.addHdRelation(elemName, Dependants, props, ExcldFrmDisblReadOnly, fireEventIfTrue, executeWhenEvenetFinishedFired);
}


function addDtRelation(tabObj, elemName, Dependants, props, ExcldFrmDisblReadOnly) {
    Relation.addDtRelation(tabObj, elemName, Dependants, props, ExcldFrmDisblReadOnly);
}

function addRecordRelation(Record, elemName, Dependants, props, ExcldFrmDisblReadOnly, fireEventIfTrue, executeWhenEvenetFinishedFired) {
    var Prefix = "";
    if (!belongHD(Record)) {
        Prefix = "]";
    }
    var elem = getDtElemByName(elemName, Record);
    var func = function (obj) {
        var fn = function () {
            var newDependants = $.grep(Dependants, function (val) {
                return ($.inArray(val, ExcldFrmDisblReadOnly) == -1);
            });
            if (props.readOnly != undefined) {
                //alert(elem.attr("name") + "\n" + elem.val())
                if ($.inArray(elem.val(), props.readOnly[1]) > -1 || props.readOnly[1].length == 0) {
                    SetItemsProperty(Record, Prefix, newDependants, "readOnly", props.readOnly[0]);
                } else {
                    SetItemsProperty(Record, Prefix, newDependants, "readOnly", !props.readOnly[0]);
                }
            }
            if (props.disabled != undefined) {
                if ($.inArray(elem.val(), props.disabled[1]) > -1 || props.disabled[1].length == 0) {
                    SetItemsProperty(Record, Prefix, newDependants, "disabled", props.disabled[0]);
                } else {
                    SetItemsProperty(Record, Prefix, newDependants, "disabled", !props.disabled[0]);
                }
            }
            if (props.empty != undefined && Relation.allowEmpty) {
                if ($.inArray(elem.val(), props.empty[1]) > -1 || props.empty[1].length == 0) {
                    EmptyElems(Record, Prefix, Dependants);
                }
            }
            clearInterval(interval);
            if (executeWhenEvenetFinishedFired != null && executeWhenEvenetFinishedFired != undefined) {
                executeWhenEvenetFinishedFired();
            }
        }
        //alert(fireEventIfTrue)
        if (fireEventIfTrue != null) {
            var interval = setInterval(function () {
                if (!fireEventIfTrue()) return;
                fn();
            }, 10);
        } else {
            fn();
        }
    };
    elem.bind("change", func);
    Relation.allowEmpty = false;
    func();
    Relation.allowEmpty = true;
}
function disableSelect(elem) {
    elem.parent().find("select[BackUpControl='Y'][disabled]").remove();
    //alert("D")
    var o = getSameSelect(elem);
    elem.hide();
    elem.parent().append(o);
    o.attr("disabled", true);
    o.show();
    o.addClass("ReadTextBox");
}
function enableSelect(elem) {
    elem.parent().find("select[BackUpControl='Y'][disabled]").remove();
    elem.show();
}
function getSameSelect(sel) {
    var o2 = $(sel).clone();
    o2.attr("BackUpControl", "Y");
    o2.val($(sel).val());
    return o2;
}
function SetItemsProperty(Container, Prefix, arr, prptyName, prprtyVal, changeCss) {
    if (changeCss == null || changeCss == undefined) {
        changeCss = false;
    }
    if (Prefix != "") {
        Prefix = Prefix + ".";
    }
    var expr = "name=";
    if (Prefix == "].") {
        expr = "name$=";
    }
    for (var i = 0; i < arr.length; i++) {
        if (prptyName.toLowerCase() == "class") {
            Container.find("[" + expr + "'" + Prefix + arr[i] + "']:not([BackUpControl])").each(function () {
                if ($(this).hasClass("hiddenField")) {
                    $(this).addClass(prprtyVal);
                } else {
                    $(this).attr(prptyName, prprtyVal);
                }
            })
        } else {
            Container.find("[" + expr + "'" + Prefix + arr[i] + "']:not([BackUpControl])").attr(prptyName, prprtyVal);
        }
    }
    prptyName = prptyName.toLowerCase();
    if (prptyName == "readonly" || prptyName == "disabled") {
        className = "";
        var elem;
        for (var i = 0; i < arr.length; i++) {
            elemCss = Container.find("[" + expr + "'" + Prefix + arr[i] + "']");
            elem = Container.find("[" + expr + "'" + Prefix + arr[i] + "']:not([BackUpControl])");
            if (prprtyVal) {
                className = "ReadTextBox";
            }
            else if (elemCss.is("[data-val-required]") || elemCss.is("[RequiredHasNoAttr]")) {
                className = "ReqTextBox";
            }
            else{
                className = "TextBox";
            }
            if (changeCss && !elem.is("[type=button]")) {
                elemCss.attr("class", className);
            }
            elemCss.attr(prptyName, prprtyVal);
            if (elem.is("select") && prptyName == "readonly" && prprtyVal == true) {
                if (elem.parent().find("select[BackUpControl='Y'][disabled]").length == 0) {
                    disableSelect(elem);
                }
            }

            if (elem.is("select") && prptyName == "readonly" && prprtyVal == false) {
                if (elem.parent().find("select[BackUpControl='Y'][disabled]").length > 0) {
                    enableSelect(elem);
                }
            }

            if (elem.is("[type=button]") && prptyName == "readonly" && prprtyVal == true) {
                elem.attr("disabled", true);
            }
            if (elem.is("[type=button]") && prptyName == "readonly" && prprtyVal == false) {
                elem.attr("disabled", false);
            }

        }
    }

}

function SetDtItemsProperty(Container, arr, prptyName, prprtyVal, changeCss) {
    Prefix = "]";
    SetItemsProperty(Container, Prefix, arr, prptyName, prprtyVal, changeCss);
}

function SetHdItemsProperty(arr, prptyName, prprtyVal, changeCss) {
    SetItemsProperty(masterContents(), "", arr, prptyName, prprtyVal, changeCss);
}
  
function enableContainer(container, param) {
    //container.find("input,textarea").attr("readOnly", !param);
    //container.find("#DetailContents").find("input,textarea").attr("disabled", !param);
    //container.find("select").attr("disabled", !param);
    //container.find("input[type=checkbox], input[type=button],button").attr("disabled", !param);

    //container.find("input:not([readonly]),textarea:not([readonly])").attr("readOnly", !param);
    //container.find(detailContents()).find("input:not([disabled]),textarea:not([disabled])").attr("disabled", !param);
    //container.find("select:not([disabled])").attr("disabled", !param);
    //container.find("input[type=checkbox]:not([disabled]), input[type=button]:not([disabled]),button:not([disabled])").attr("disabled", !param);
    //enableCalendares(container, param);
    container.find("input:not(.ReadTextBox),textarea:not(.ReadTextArea)").attr("readOnly", !param);
    container.find(detailContents()).find("input:not(.ReadTextBox),textarea:not(.ReadTextArea)").attr("disabled", !param);
    container.find("select:not(.ReadSelect)").attr("disabled", !param);
    container.find("input[type=checkbox], input[type=button]:not(.ReadTextBox),button:not(.ReadTextBox)").attr("disabled", !param);
    enableCalendares(container, param);
}



function addHDFormula(targets, formulaElems, fireElems) {
    var func = function (i) {
        var result = 0;
        var str = formulaElems[i].expr;
        for (var F = 0; F < formulaElems[i].elements.length; F++) {
            elemName = formulaElems[i].elements[F].elemName;
            var elem = getElemByName(elemName);
            str = str.replace(("P" + F), nvl(elem.val(),0));
        }
        //console.info(targets[i] + "  " + str)
        result = eval(str);
        getElemByName(targets[i]).val(result.toFixed(2));
        if (fireElems != undefined && fireElems != null) {
            if ($.inArray(targets[i], fireElems)>-1) {
                getElemByName(targets[i]).trigger("change");
            }
        }
    }
    $.each(targets, function (x, c) {
        for (var y = 0; y < formulaElems[x].elements.length; y++) {
            elemName = formulaElems[x].elements[y].elemName;
            var currentElem = getElemByName(elemName);
            var fn = function () {
                func(x);
            }
            currentElem.bind("change", fn);
            //if (Page.IdVal() != "0") {
                fn();
            //}
        }
    });
}




function addDTFormula(Parent, targets, formulaElems, calcSumTargets, allContainer) {
    var func = function (i, Record) {
        var result = 0;
        var str = formulaElems[i].expr;
        for (var F = 0; F < formulaElems[i].elements.length; F++) {
            elemName = formulaElems[i].elements[F].elemName;
            var elem;
            switch (formulaElems[i].elements[F].pos) {
                case "HD":
                    elem = getElemByName(elemName);
                    break;
                case "DT":
                    elem = getDtElemByName(elemName, Record);
                    break;
                default:
                    elem = getDtElemByName(elemName, Record);
                    break;
            }
            str = str.replace(("P" + F), nvl(elem.val(), 0));
        }
        result = eval(str);
        getDtElemByName(targets[i], Record).val(result.toFixed(2));
        if (calcSumTargets != undefined && calcSumTargets != null) {
            if (calcSumTargets[i] != "" && calcSumTargets[i] != null && calcSumTargets[i] != undefined) {
                var sumVals = 0;
                //alert(allContainer.length)
                allContainer.find("table[id=RecordContainer]:has(tr td input[type=hidden][name$='].Deleted'][value='False'])").each(function () {
                    //alert(getDtElemByName(targets[i], $(this)).attr("name") + "\n" + nvl(getDtElemByName(targets[i], $(this)).val(), 0))
                    sumVals = sumVals + nvl(getDtElemByName(targets[i], $(this)).val(), 0);
                    //console.info("mahmoud");
                });
                //console.info("x");
                getElemByName(calcSumTargets[i]).val(sumVals.toFixed(2));
                getElemByName(calcSumTargets[i]).trigger("change");
            }
        }
    }

    var Container = Parent;
    var Container2;
    if (Container.prop("tagName").toLowerCase() == "div") {
        allContainer = Container;
        Container2 = Container.find("table[id=RecordContainer]:has(tr td input[type=hidden][name$='].Deleted'][value='False'])");
    } else {
        if (allContainer == null || allContainer == undefined) {
            allContainer = Container.closest("#TabContents");//.find("table[id=RecordContainer]:has(tr td input[type=hidden][name$='].Deleted'][value='False'])")
        } else {
            //$.each($(tabsArray), function (i, v) {
            //    alert(v)
            //    toSumContainer = toSumContainer.add(getTabContentsContainer(v));
            //});
            //toSumContainer = allContainer.find("table[id=RecordContainer]:has(tr td input[type=hidden][name$='].Deleted'][value='False'])")
        }
        Container2 = Container;
    }
    //if (tabsArray = null || tabsArray == undefined) {
    //    Cont = Container;
    //} else {
    //    $.each(tabsArray, function (i, v) {
    //        Cont = Cont.add(getTabContentsContainer(v));
    //    });
    //}
    Container2.each(function () {
        var Record = $(this);
        //alert(Record.html())
        $.each(targets, function (x, c) {
            if (formulaElems[x].execute == undefined || formulaElems[x].execute == null) {
                $.extend(formulaElems[x], { execute: true });
            }
            for (var y = 0; y < formulaElems[x].elements.length; y++) {
                elemName = formulaElems[x].elements[y].elemName;
                var currentElem;
                switch (formulaElems[x].elements[y].pos) {
                    case "HD":
                        currentElem = getElemByName(elemName);
                        break;
                    case "DT":
                        currentElem = getDtElemByName(elemName, Record);
                        break;
                    default:
                        currentElem = getDtElemByName(elemName, Record);
                        break;
                }
                var fn = function () {
                    func(x, Record);
                }
                if (formulaElems[x].execute) {
                    currentElem.bind("change", fn);
                    //if (Page.IdVal() != "0") {
                        fn();
                    //}
                }
            }
        });
    });
}



function enableEdit(param) {
    enableContainer($("#formContents"), param);
    enableSave(param);
}

function enableSave(param) {
    Page.saveEnabled = param;
    Page.enableToolBar(["Save"], !param);
}
function enableNew(param) {
    Page.newEnabled = param;
    Page.enableToolBar(["New"], !param);
}
function enableSearch(param) {
    Page.searchEnabled = param;
    Page.enableToolBar(["StartSearch"], !param);
}
function enableDelete(param) {
    Page.deleteEnabled = param;
    //Page.enableToolBar(["Delete"], !param);
}
function enablePrint(param) {
    Page.enableToolBar(["Print"], !param);
}
function enableList(param) {
    Page.enableToolBar(["List"], !param);
}
function enablePage(param) {
    enableContainer($("#formContents"), param);
    enableSave(param);
    enableNew(param);
    enableSearch(param);
    enableList(param);
    enablePrint(param);
}
function enableCalendares(container, param) {
    
    var str = "enable";
    if (!param) {
        str = "disable";
    }
    //alert(param+"\n"+str)
    container.find(".datepicker").each(function () {
        //alert($(this).attr("id"))
        $(this).datepicker(str);
    });
}
function getRecordContainer(elem) {
    return $(elem).closest("#RecordContainer");
}


function getFirstAvailTableInTab(Tab) {
    return getTabContentsContainer(Tab).children("table").filter(":visible");
}

function getTabContentsContainer(Tab) {
    return detailContents().find("#" + Tab.targetId).find("#TabContents");
}
function getRecordTabContainer(record) {
    return detailContents().find(record).closest("#TabContents");
}

function setHdRequired(arr, msgArr) {
    for (var i = 0; i < arr.length; i++) {
        SetHdItemsProperty([arr[i]], "data-val", true);
        SetHdItemsProperty([arr[i]], "aria-required", true);
        SetHdItemsProperty([arr[i]], "aria-invalid", true);
        SetHdItemsProperty([arr[i]], "data-val-required", msgArr[i]);
        SetHdItemsProperty([arr[i]], "class", "ReqTextBox");
    }
    Page.rebuildVaidations();
}

function removeHdRequired(arr) {
    for (var i = 0; i < arr.length; i++) {
        SetHdItemsProperty([arr[i]], "data-val", false);
        SetHdItemsProperty([arr[i]], "aria-required", false);
        SetHdItemsProperty([arr[i]], "aria-invalid", false);
        SetHdItemsProperty([arr[i]], "class", "TextBox");
    }
    Page.rebuildVaidations();
}



function setDtRequired(Container, arr, msgArr) {
    for (var i = 0; i < arr.length; i++) {
        SetDtItemsProperty(Container, [arr[i]], "data-val", true);
        SetDtItemsProperty(Container, [arr[i]], "aria-required", true);
        SetDtItemsProperty(Container, [arr[i]], "aria-invalid", true);
        SetDtItemsProperty(Container, [arr[i]], "data-val-required", msgArr[i]);
        SetDtItemsProperty(Container, [arr[i]], "class", "ReqTextBox");
    }
    Page.rebuildVaidations();
}

function removeDtRequired(Container, arr) {
    for (var i = 0; i < arr.length; i++) {
        SetDtItemsProperty(Container, [arr[i]], "data-val", false);
        SetDtItemsProperty(Container, [arr[i]], "aria-required", false);
        SetDtItemsProperty(Container, [arr[i]], "aria-invalid", false);
        SetDtItemsProperty(Container, [arr[i]], "class", "TextBox");
    }
    Page.rebuildVaidations();
}
//function showModal(title, url, options, addBtn, btnTitle) {

//    var btn = [];

//    if (addBtn) {
//        btn = [{
//            label: btnTitle,
//            action: function (dialog) {
//                showModalBtnAction(dialog);

//            }
//        }];
//    }

//    var dialog = BootstrapDialog.show({
//        title: title,
//        draggable: true,
//        onshow: function (dialog) {
//            dialog.setMessage("<p align=center class='loading' style='padding:6px;'>&nbsp;</p>");
//        },
//        buttons: btn,
//        onshown: function (dialog) {

//            //height = '" + options.height + "'
//            if (options == undefined || options.width == undefined) {
//                options.width = "auto";
//            }

//            dialog.setMessage("<iframe id='IframeContent' src='" + url + "' style='width:100%' onload='autoResizeIframe(this)' frameborder='0'></iframe>");
//            //document.getElementById("IframeContent").contentWindow.document.body.innerHTML = "<div class='loading'></div>";
//            $(".modal-dialog").css({ width: options.width, height: "auto" });
//            //$(".modal-body").css({ width: "auto", height: "auto" });
//            //$(".modal-content").css("width", "auto");
//        }
//    });
//}

function showModal(title, url, options, refreshPage, addBtn, btnTitle) {//refreshPage,
    var btn = [];

    if (addBtn)
    {
        btn = [{
            label: btnTitle,
            action: function (dialog) {
                showModalBtnAction(dialog);

            }
        }];
    }
    
    var dialog = BootstrapDialog.show({
        title: title,
        draggable: true,
        onshow: function (dialog) {
            dialog.setMessage("<p align=center class='loading' style='padding:6px;'>&nbsp;</p>");
        },
        buttons:btn,
        onshown: function (dialog) {
             
            //height = '" + options.height + "'
            if (options == undefined || options.width == undefined) {
                options.width = "auto";
            }

            dialog.setMessage("<iframe id='IframeContent' src='" + url + "' style='width:100%' onload='autoResizeIframe(this)' frameborder='0'></iframe>");
            //document.getElementById("IframeContent").contentWindow.document.body.innerHTML = "<div class='loading'></div>";
            $(".modal-dialog").css({ width: options.width, height: "auto" });
            //$(".modal-body").css({ width: "auto", height: "auto" });
            //$(".modal-content").css("width", "auto");
        },
        onhidden: function (dialog) {
            if (refreshPage != undefined || refreshPage != null) {
               location.href=refreshPage
            }
        }
        
    });
   
}

function openEntry(entryId,entryNo) {
    showModal("Entry # " + entryNo,"/JournalHD/Index?Id="+ entryId +"&IsPopUp=Y", {width:"100%"});
}




function autoResizeIframe(obj)
{
    //alert(obj.readyState)
    var newheight;
    var newwidth;
    newheight = $(obj).contents().find('body').prop("scrollHeight");
    newwidth = $(obj).contents().find('body').prop("scrollWidth");
    //obj.contentWindow.document.body.innerHTML = "<div class='loading'></div>";
    //$(obj).append("<div id='loadingImg' class='fixed_loading' style='padding-left:85%'></div>");
    obj.height = (newheight) + "px";
    obj.width = (newwidth) + "px";
}

function masterContents(){
    return $("#MasterContents");
}
function detailContents() {
    return $("#DetailContents");
}

function checkAllBoxes(obj,Id)
{
    //alert($("#SearchResultsContainer").find("[type=checkbox][id=" + Id + "]").length)
    $("#SearchResultsContainer").find("[type=checkbox][id=chk_id_" + Id + "]").prop("checked", $(obj).is(":checked"))
    $("#SearchResultsContainer").find("[type=hidden][name=" + Id + "]").val(($(obj).is(":checked")) ? "Y" : "N");
}