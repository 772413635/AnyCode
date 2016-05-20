$(function () {
    if (document.getElementById("fm") != null)
        $("#fm").validationEngine();
    $(".btn-sub").button();
    $(".btn-back").button({
        icons: {
            primary: "ui-icon-arrowreturnthick-1-w"
        },
        text: false
    });
    $(".btn-search").button({
        icons: { primary: "ui-icon-search" }
    });
    //打印
    $(".btn-add").button({
        icons: { primary: "ui-icon-plus" },
        text: false
    });
    //打印
    $(".btn-print").button({
        icons: { primary: "ui-icon-print" }
    });
    //定位
    $(".btn-position").button({
        icons: { primary: "ui-icon-arrowthickstop-1-s" }
    });
    $(".dataview_button").button();

    $("button,input,a,div").tooltip({
        show: null,
        position: {
            my: "left top",
            at: "left bottom"
        },
        open: function (event, ui) {
            ui.tooltip.animate({
                top: ui.tooltip.position().top + 5
            }, "fast");
        }
    });
    //文本框初始化
    $("input[type='text'],textarea,input[type='password']").focus(function () {
        $(this).addClass("text-focus-2");
    });
    $("input[type='text'],textarea,input[type='password']").blur(function () {
        $(this).removeClass("text-focus-2");
    });
    $("input[type='text'],textarea,input[type='password']").each(function () {
        $(this).addClass("text");
        if (this.className.match("required")) {
            $(this).after("<span class='span_required'>*<span>");
        }
    });
    //日期控件初始化
    $(".Wdate").click(function () {
        WdatePicker();
    });
});

//简单提示
function showToastMessage(message) {
    if (message == "" || message == null) {
        message = "登陆失败";
    }
    $("body").cftoaster({
        content: message,
        bottomMargin: 300,
        fontColor: '#fff',
        backgroundColor: '#666'
    });
}
