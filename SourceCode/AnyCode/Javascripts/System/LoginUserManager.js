var toolbar = [];
var columns = [[
    { field: 'ck', checkbox: true },
    { title: '用户名', field: 'Name', align: 'center', width: 100, sortable: true },
    { title: '名称', field: 'MyName', align: 'center', width: 150, sortable: true },
    { title: 'IP地址', field: 'IP', align: 'center', width: 140, sortable: true },
    {
        title: '登录时间', field: 'CreateTime', align: 'center', width: 200, sortable: true, formatter: function (val) {
            if (val) {
                return $.JsonToDate(val).fullDateString;
            } else {
                return "---";
            }
            
        }
    },
    {
        title: '上一次登出时间', field: 'SignoutTime', align: 'center', width: 200, sortable: true, formatter: function (val) {
            if (val) {
                return $.JsonToDate(val).fullDateString;
            } else {
                return "——";
            }

        }
    }
]];
toolbar = DealWithFunction([{
    id: 'btncut',
    text: '查询',
    iconCls: 'icon-search',
    handler: function () {
        flexiQuery('#divQuery', "查询");
    }
}, '-'], funList.SearchData, toolbar);
toolbar = DealWithFunction([{
    id: 'btnout',
    text: '强制退出',
    iconCls: 'icon-remove',
    handler: function () {
        SingOutUser("/System/SingOutUser", "#test");
    }
}], funList.SignOut, toolbar);

$(function () {
    var option = BaseOptions("/System/OnlineUser", "Id", columns, null, toolbar, "#formsearch");
    BaseLoadTableByOptions(option, "#test");
});

//查询
function Search() {
    BaseSearch("#formsearch", "#test");
}


//强制退出
function SingOutUser(url, tableId) {
    var selection = $(tableId).datagrid('getChecked');
    if (selection.length == 0) {
        AlertWarning("请选择需要强退的用户");
    } else {
        var ids = "";
        for (var i = 0; i < selection.length; i++) {
            ids += selection[i].Id + ',';
        }
        $("#ddText").text(selection.length);
        $("#dd").show().dialog({
            width: 200,
            minHeight: 120,
            modal: true,
            buttons: [{
                text: "强制退出",
                handler: function () {
                    $.ajax({
                        url: url,
                        type: 'post',
                        data: { id: ids },
                        datatype: 'text',
                        success: function (data) {
                            showToastMessage(data);
                            $("#dd").dialog("close");
                            $(tableId).datagrid('reload');
                            $(tableId).datagrid('uncheckAll');
                        }
                    });
                }
            }, {
                text: "取消",
                handler: function () {
                    $("#dd").dialog("close");
                }
            }]
        });


    }
}