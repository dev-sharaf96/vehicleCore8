function getNewRecord(url)
{
    $("#NewBtn").click(function () {
        var currentRowIndex = $("#ContainerDt > table").length - 1;
        $.ajax({
            type: "POST",
            url: url,
            data: { RowIndex: currentRowIndex },
        cache: false,
        success: function (result) {
            $("#ContainerDt").append(result);
        },
        error: function (XMLHttpRequest, textStatus, error) {
            //alert(XMLHttpRequest.responseText)
            var dialog = BootstrapDialog.show({
                title: 'Error',
                message: XMLHttpRequest.responseText,
                buttons: [{
                    label: 'Ok',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });

        }
    });
})
}


function hideHiddenRows() {
    $("#ContainerDt > table").has("input[type='hidden'][name$='].Deleted'][value='True']").each(function () {
        $(this).hide();
    })
}


//function deleteRecord(dataRecord) {
//    var dataRecord = $(dataRecord);
//    var IdVal = dataRecord.find("input[type=hidden][id='RecordKey']").val();
//    dataRecord.fadeOut(1000, function () {
//        if (IdVal == "0") {
//            dataRecord.remove();
//            reorderRows();
//        }
//        else {
//            dataRecord.find("input, select, textarea").each(function () {
//                $(this).val($(this).prop("defaultValue"));
//            });
//            dataRecord.find("input[type=hidden][name$='].Deleted']").val("True");
//        }
//    });
//}


//function reorderRows() {
//    $("#ContainerDt > table").each(function () {
//        var index = $(this).index();
//        $(this).find("input, select, textarea").each(function () {
//            if (this.hasAttribute("name")) {
//                newName = $(this).attr("name").replace(/\[(\d+)\]./, "[" + index + "].")
//                $(this).attr("name", newName);
//            }
//            if (this.hasAttribute("id")) {
//                newId = $(this).attr("id").replace(/\[(\d+)\]./, "[" + index + "].")
//                $(this).attr("id", newId);
//            }
//        })
//    })
//}



