$(function () {
    try {
        $('input').pretext();
    } catch (ex) {
        $('input').each(function () {
            var $this = $(this),
                $label = $('label[for="' + $this.attr('id') + '"]');
            $($label).remove();
        });
    }
    var getCookie = $.cookie("rememberPassword");
    //赋值
    if (getCookie != null) {
        var valus = getCookie.split(',');
        $("#Name").val(valus[0]);
        $("#Password").val(valus[1]);
        $("#remember").attr('checked', 'checked');
    }
});

//输入新帐号，单选框不选中
function changeText() {
    if ($("#remember").attr("checked") == "checked") {
        $("#remember").attr("checked", false);
    }
}

//登录事件
function Login() {
    if ($("#remember").attr('checked') == 'checked') {
        $.cookie("rememberPassword", $("#Name").val() + "," + $("#Password").val(), { expires: 30, path: '/', secure: false }); //创建一个cookie
    } else {
        $.cookie("rememberPassword", null); //删除cookie
    }
    if ($("#fm").validationEngine('validate')) {
        $.ajax({
            url: '/Account/Login',
            dataType: 'text',
            type: 'post',
            data: $("#fm").serialize(),
            success: function (data) {
                if (data == "1") {
                    window.location.href = "/Home/Index";
                } else {
                    showToastMessage(data);
                }
               
            }
        });
    } else
        return false;
}


//按钮按下事件

function keydown() {
    if (event.keyCode == 13) {
        Login();
    } else {
        return false;
    }
}