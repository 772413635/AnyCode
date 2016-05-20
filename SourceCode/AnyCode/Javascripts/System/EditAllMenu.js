$(function() {
});

function SaveFull() {
    if ($("#fm").validationEngine('validate')) {
        $.ajax({
            url: '/system/EditAllMenu',
            type: 'post',
            data: $("#fm").serialize(),
            dataType: 'text',
            success: function(data) {
                if (data.match("成功"))
                    showToastMessage(data);
                else
                    showToastMessage("操作失败");
            }
        });
    } else {
        $(".span_required").css("color", "red");
    }
}

function IsEnabled(dmo) {
    if (dmo.checked != true) {
        $("#"+dmo.value).attr("disabled", true);
    } else {
        $("#" + dmo.value).attr("disabled", false);
    }
}