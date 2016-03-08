$(function () {
    $("#fm").validationEngine();
    if ($("#Id").val() != "") {//编辑
        LoadJueSe();
    } else {//添加,默认角色为普通用户，权限为普通用户的权限
        $("#info").hide();
        $("#cc").combobox({
            url: '/System/GetRoleList/' + $("#companyId").val(),
            valueField: 'id',
            textField: 'text',
            editable: false,
            disabled: false,
            onSelect: function (record) {
                $("#RoleId").val(record.id);
                LoadpQuanXian('/System/GetSystemSelectTree/' + record.id);
            },
            onLoadSuccess: function () {
                $("#cc").combobox('select', 1);
            }, onLoadError: function () {
                $.messager.alert('角色数据加载失败', "warning");
            }
        });


    }
});

function LoadJueSe() {
    var isSystemUser;
    if ($("#Id").val() == 2) {
        isSystemUser = true;
    } else {
        $("#info").hide();
        isSystemUser = false;
    }
    $("#cc").combobox({
        url: '/System/GetRoleList/' + $("#companyId").val(),
        valueField: 'id',
        textField: 'text',
        editable: false,
        disabled: isSystemUser,
        onSelect: function (record) {
            $("#RoleId").val(record.id);
            LoadpQuanXian('/System/GetSystemSelectTree/' + record.id);
        },
        onLoadSuccess: function () {
            $("#cc").combobox('select', $("#RoleId").val());
        }, onLoadError: function () {
            $.messager.alert('角色数据加载失败', "warning");
        }

    });
}

function LoadpQuanXian(url) {
    $('#pTree').tree({
        checkbox: false,
        url: url
    });
    $("#pTree").attr("disabled", true);
}

function BackPage() {
    BaseBackPage();
}

function Save() {
    if ($("#fm").validationEngine('validate')) {
        $('.btn-sub').attr("disabled", true); //提交按钮设为,不可用   
        $.ajax({
            type: "post",
            url: '/system/CreateUser',
            data: $("#fm").serialize(),
            dataType: "text",
            success: function (data) {
                $(".btn-sub").attr("disabled", false); //提交按钮设为,可用                      
                showToastMessage(data);
            }
        });
    }
    else
        return false;
}