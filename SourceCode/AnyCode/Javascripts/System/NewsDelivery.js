$(function () {
    var pp = {
        panelWidth: 555,
        value: '',
        idField: 'Id',
        textField: 'MyName',
        multiple: 'true',
        editable: true,
        url: '/System/GetUserList/all',
        columns: [[
            { field: 'ck', checkbox: true },
            { field: 'Id', title: 'ID号', hidden: true },
            { field: 'Name', title: '帐号', width: 55 },
            { field: 'MyName', title: '名称', width: 160 }
        ]],
        onCheck: function (rowIndex, rowData) {
            var text = $("#ddlSeat").combo("getText");
            var texts = text.split(',');
            if (texts != "" && texts.length == 1) {
                $('#ddlSeat').combo("setText", rowData.MyName);
            }

        }
    };

    $('#ddlSeat').combogrid(pp);
    $("#btnSend").click(function () { send(); });
    $("#content").keypress(function (event) {
        if (event.keyCode == 13) {
            send();
        }
    });
    $("#sendfm").validationEngine({
        "promptPosition": "centerRight"
    });
    $(".span_required:first").css({
        position: "relative",
        left: "-148px"
    });
});
function backList() {
    BaseBackPage("消息推送", "公告列表", '/News/NewsList/');
}
//检查用户输入是否有效
function searchClick() {
    if ($("#ddlSeat").combo("getText") == $("#ddlSeat").combo("getValues")) {
        $("#usersel").val("");
    } else {
        $("#usersel").val($("#ddlSeat").combo("getValues"));
    }
   
    return $("#sendfm").validationEngine('validate');


}

function send() {

    if (searchClick() == true) {
        if (confirm("确定要发送吗？")) {
            $.post(
                "comet_broadcast.asyn",
                { title: $("#title").val(), content: $("#News").val(), code: $("#ddlSeat").combo("getValues"), createPeople: $("#People").val() },
                function (result) {

                    showToastMessage(result);

                });
        }

    }
}
