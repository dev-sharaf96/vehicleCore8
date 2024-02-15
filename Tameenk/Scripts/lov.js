function clickBtnLov(event) {
    var srcElem = $(event.target);
    //alert(1)
    if (srcElem.attr("lov")) {
        if ($(srcElem).is('[readonly]')) {
            return;
        }
        setGLobalVar("currentLovElem", srcElem, "Lov");
        //alert(srcElem.attr("lov"))
        eval(srcElem.attr("lov"));
    }
}
function trToArray(tr) {
    var arr = [];
    $(tr).find("td").each(function () {
        arr[arr.length] = $(this).text();
    })
    return arr;
}


function clickToFilterLov(filterColVal, sortColVal) {
    if (sortColVal != undefined && sortColVal != null) {
        if (sortColVal == lovDialog.params.sortCol) {
            lovDialog.params.sortCol = -sortColVal;
        }
        else {
            lovDialog.params.sortCol = sortColVal;

        }
    }
    $.extend(lovDialog.params, { filter: lovDialog.params.filter, filterCol: filterColVal, sortCol: lovDialog.params.sortCol });
    drawLovContents();
}
function changeSearchText(obj, ev) {
    if (lovDialog.textSearchOldVal == obj.value) return;
    $.extend(lovDialog.params, { filter: obj.value, filterCol: lovDialog.params.filterCol });
    drawLovContents(lovDialog, lovDialog.url, lovDialog.params);
}
function drawLovContents() {//(dialog, url, params, textSearch) {
    $(lovDialog.textSearch).find("#loading").show();
    $.ajax({
        type: "POST",
        url: lovDialog.url,
        data: lovDialog.params,
        cache: false,
        success: function (result) {
            Page.endProcess();
            var dataPannel = $(lovDialog.dialog.getMessage()).find("#searchResult");
            if (dataPannel.length == 0) {
                var lovDoc = $('<div id="LovDocument"></div>')
                lovDoc.append(lovDialog.textSearch);
                lovDoc.append("<div id='searchResult'>" + result + "</div>");
                lovDialog.dialog.setMessage(lovDoc);
                lovDoc.find("#SearchBy").focus();
                dataPannel = lovDoc.find("#searchResult");
            }
            else {
                dataPannel.html($(result));
                lovDialog.textSearchOldVal = $(lovDialog.textSearch).find("#SearchBy").val();
            }
            //alert(dataPannel.find("#AllDataContainer").find("#FilterBy").val() + "\n" + lovDialog.params.filterCol)
            dataPannel.find("#AllDataContainer").find("#FilterBy").val(lovDialog.params.filterCol);
            var pannelDivHeight = dataPannel.find("#LovDivContent").height();
            var pannelTableHeight = dataPannel.find("#LovDivContent").find("#LovTableContent").height();
            //alert(pannelTableHeight +"\n"+ pannelDivHeight)
            if (pannelTableHeight > pannelDivHeight) {
                dataPannel.find("#LovDivContent").attr("class", "LovDivContentAuto");
                dataPannel.find("#LovDivHeader").attr("class", "LovDivHeaderPadded");
            } else {
                dataPannel.find("#LovDivContent").attr("class", "LovDivContentAuto2");
                dataPannel.find("#LovDivHeader").attr("class", "LovDivHeaderInitial");
            }
            //Give class to active column after drawing contents
            dataPannel.find("#LovTableHeader").first("tr").find("th").each(function (index, obj) {
                if (Math.abs(lovDialog.params.sortCol) == (index + 1)) {
                    $(this).attr("class", "LovHeaderActiveCol");
                } else {
                    $(this).attr("class", "");
                }
            });

            //Attach onclick events to header
            var filterColVal = dataPannel.find("#AllDataContainer").find("#FilterBy").val();
            //alert(dataPannel.find("#LovDivContent").html())
            dataPannel.find("#LovTableHeader").first("tr").find("th").each(function (index, obj) {
                //alert(lovDialog.params.filterCol + "\n" + filterColVal)
                $(this).on("click", function () {
                    clickToFilterLov(filterColVal, (index+1));
                });
            });

            //Attach onmouseover,mouseout events to Header
            dataPannel.find("#LovTableHeader").first("tr").find("th").each(function (index, obj) {
                $(this).on("mouseover", function () {
                    if (lovDialog.params.sortCol == (index + 1)) return;
                    $(this).attr("class", "LovHeaderOverCol")
                });
                $(this).on("mouseout", function () {
                    if (lovDialog.params.sortCol == (index + 1)) return;
                    $(this).attr("class", "")
                });
            });


            //Data Rows Over & Out & Click
            dataPannel.find("#LovTableContent").find("tr").each(function (index, obj) {
                $(this).on("mouseover", function () {
                    if (lovDialog == null) return;
                    if (lovDialog.selectedDataRow == this) return;
                    $(this).attr("class", "LovContenOverRow")
                });
                $(this).on("mouseout", function () {
                    if (lovDialog == null) return;
                    if (lovDialog.selectedDataRow == this) return;
                    $(this).attr("class", "")
                });
                $(this).on("click", function () {
                    
                    if (lovDialog.selectedDataRow == this) return;
                    $(lovDialog.selectedDataRow).attr("class", "")
                    $(this).attr("class", "LovContenActiveRow");
                    lovDialog.selectedDataRow = this;
                });
                $(this).on("dblclick", function () {
                    $(lovDialog.selectedDataRow).attr("class", "")
                    $(this).attr("class", "LovContenActiveRow");
                    lovDialog.selectedDataRow = this;
                    setLovValues();
                });
            });
        },
        error: function (XMLHttpRequest, textStatus, error) {
            lovDialog.dialog.setMessage(XMLHttpRequest.responseText);
        },
        complete: function () {
            $(lovDialog.textSearch).find("#loading").hide();
        }
    });
}
function setLovValues() {
    if (lovDialog.selectedDataRow == null)
    {
        alert("Please, Select data record");
        return;
    }
    var resultArr = trToArray(lovDialog.selectedDataRow);
    var srcTable = $(lovDialog.currentItem).parents("#RecordContainer");
    $.each(lovDialog.elements, function (index, value) {
        $(srcTable).find("[name=\"" + value + "\"]").val(resultArr[index]);
    });
    if (lovDialog.fireFunc != undefined && lovDialog.fireFunc != null) {
        lovDialog.fireFunc(resultArr);
    }
    if (lovDialog.fireOnchange != undefined && lovDialog.fireOnchange != null) {
        $(lovDialog.LovSrcElement).trigger("change");
    }
    lovDialog.dialog.close();
    lovDialog = null;
}

var lovDialog = null;
function Lov(url, params, elements, modalOptions, fireFunc, dependsOnElems, LovSrcElement, fireOnchange) {
    if (!Page.searchIsActive) {
        if (dependsOnElems != undefined && dependsOnElems != null) {
            for (var i = 0; i < dependsOnElems.length; i++) {
                var elem = $("[name='" + dependsOnElems[i] + "']");
                if (elem.val() == "") {
                    showAlert(ValidationLabel, "This field must be filled.", elem);
                    $.each(elements, function (index, value) {
                        $("[name='" + value + "']").val("");
                    })
                    return;
                }
            }
        }
    }
    
    if (Page.processRunning) {
        showAlert(ProcessingWait, PleaseWait + "...");
        return;
    }
    Page.startProcess();
    
    var textSearch = "<div><label for='SearchBy' style='font-weight:normal;padding:5px;'>"+SearchLabel+"</label><input type='text' id='SearchBy' style='max-width:250px !important' onkeyup='changeSearchText(this,event)' value=''><span id='loading' style='border:0px solid red;display:none;margin-left:3px;padding:5px 8px 5px 8px;' class='loading'>&nbsp;</span></div>";
    var dialog = BootstrapDialog.show({
        title: ListOfValuesLabel,
        message: function (dialog) {
            lovDialog = new dialogKeep(dialog);
        },
        draggable: true,
        onshow: function (dialog) {
            dialog.setMessage("<p align=center class='loading' style='padding:6px;'>&nbsp;</p>");
        },
        onshown:function(){
            drawLovContents();
            for (var propKey in modalOptions) {
                $(".modal-dialog").css(propKey, modalOptions[propKey]);
            }
        },
        buttons: [{
            label: OkBtnLabel,
            action: function (dialog) {
                setLovValues();
            }
        }, {
            label: CancelBtnLabel,
            action: function (dialog) {
                lovDialog = null;
                dialog.close();
            }
        }]
    });
    
    function dialogKeep(dialog) {
        this.dialog = dialog;
        this.url = url;
        this.params = params;
        if (this.params.filterCol === undefined) {
            $.extend(this.params, { filterCol: 1 });
        }
        if (this.params.sortCol === undefined) {
            $.extend(this.params, { sortCol: 1 });
        }
        this.currentItem = getGLobalVar("currentLovElem", "Lov");
        this.elements = elements;
        this.fireFunc = fireFunc;
        this.fireOnchange = fireOnchange;
        if (LovSrcElement != undefined && LovSrcElement != null) {
            if ($(LovSrcElement).is("[type=button]") || $(LovSrcElement).is("[button]")) {
                this.LovSrcElement = $(LovSrcElement).closest("td").prev().find("input,select,textarea,button").filter(":visible").eq(0);
            } else {
                this.LovSrcElement = $(LovSrcElement);
            }
            //alert(this.LovSrcElement.prop("outerHTML"))
        }
        this.textSearch = $(textSearch);
        this.textSearchOldVal = "";
        this.selectedDataRow  = null;
    }
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


}
//GLobal Validation Func
function validFunc(url, params, currentItem, elements, fireFunc, dependsOnElems) {
    if (!Page.searchIsActive) {
        if (dependsOnElems != undefined) {
            for (var i = 0; i < dependsOnElems.length; i++) {
                var elem = $("[name='" + dependsOnElems[i] + "']");
                if (elem.val() == "") {
                    showAlert(ValidationLabel, "This field must be filled.", elem);
                    $.each(elements, function (index, value) {
                        $("[name='" + value + "']").val("");
                    })
                    return;
                }
            }
        }
    }
    if (Page.processRunning) {
        showAlert(ProcessingWait, PleaseWait + "...");
        return;
    }
    Page.startProcess();
    var srcTable = $(currentItem).parents("#RecordContainer");
    $.ajax({
        type: "POST",
        url: url,
        data: params,
        cache: false,
        success: function (result) {
            $.each(elements, function (index, value) {
                var retVal;
                var Res = result;
                eval("retVal = Res.Col" + index);
                $(srcTable).find("[name=\"" + value + "\"]").val(retVal);
            });
            if (fireFunc != undefined && fireFunc != null) {
                var arr = $.map(result, function(el) { return el; });
                fireFunc(arr);
            }
            Page.endProcess();
        },
        error: function (XMLHttpRequest, textStatus, error) {
            alert(XMLHttpRequest.responseText)
        }
    });
}
