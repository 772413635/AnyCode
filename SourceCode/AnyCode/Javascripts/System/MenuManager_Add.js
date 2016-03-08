$(function () {
    if ($("#isEdit").val() == "0") {
        $("#Id").val("");
    }
    $("#Iconic").IconList({
        url: "/System/IconList",
        type: 'post',
        height: 100,
        left: 2,
        top: 5
    });
});

function Save() {
    if ($("#fm").validationEngine('validate')) {
        var selectids = new Array();
        var functionUl = $(".functionUl");
        $(functionUl).each(function (i,u) {
            $(u).find("input[type='checkbox']").each(function (j, m) {
                if (m.checked) {
                    selectids.push($(m).val());
                }
                
            });
        });
        $("#Function").val(selectids.join(','));
        $.ajax({
            url: '/system/MenuManagerAdd',
            type: 'post',
            data: $("#fm").serialize(),
            dataType: 'text',
            success: function (data) {
                if (data.match("成功"))
                    showToastMessage(data);
                else
                    showToastMessage("操作失败");
            }
        });
        $(".span_required").css("color", "black");
    }
    else {
        $(".span_required").css("color", "red");
    }

}