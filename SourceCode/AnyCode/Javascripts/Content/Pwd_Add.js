function BackPage() {
    BaseBackPage();
}

function Save() {
if ($("#fm").validationEngine('validate')) {
        $('.btn-sub').attr("disabled", true); //提交按钮设为,不可用
        $.ajax({
            type: "post",
            url: '/Content/CreatePwd',
            data: $("#fm").serialize(),
            dataType: "text",
            success: function (data) {
                $(".btn-sub").attr("disabled", false); //提交按钮设为,可用
                showToastMessage(data);
            }
        });
    } else
        return false;

}
$(function () {
    var usertoken = $("#UserToken").val();
    var userName = uncMe($("#UserName").val(), usertoken);
    var password = uncMe($("#Password").val(), usertoken);
    var password2 = uncMe($("#Password2").val(), usertoken);
    $("#UserName").val(userName);
    $("#Password").val(password);
    $("#Password2").val(password2);
})