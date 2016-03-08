$(function () {
    LoadTable();
});

function LoadTable() {
    var frozenColumns = [[
    { title: '时间', field: 'CreateTime', align: 'center', width: 150, sortable: true,formatter: function(val) {
        return $.JsonToDate(val).fullDateString;
    } },
    { title: '用户', field: 'UserName', align: 'center', width: 100, sortable: true },
    { title: 'IP地址', field: 'Ip', align: 'center', width: 300, sortable: true }
    ]];
    var columns = [[
        { title: '控制器', field: 'Controller', align: 'center', width: 150, sortable: true },
        { title: '方法', field: 'Action', align: 'center', width: 100, sortable: true },
        { title: '参数', field: 'Params', align: 'center', width: 300 }
    ]];
    var toolbar = [{
        text: '查询',
        iconCls: 'icon-search',
        handler: function () {
            $('#divQuery').show().dialog({
                title: '查询',
                width: 510,
                height: 140,
                resizable: true,
                modal: false,
                collapsible: false,
                closed: true,
                buttons: [{
                    text: "确定",
                    handler: function () {
                        BaseSearch("#formsearch", "#test");
                    }
                }]
            });
            $("#divQuery").dialog('open');
        }
    }];
    BaseLoadTable("/System/GetPerformLogList", "#test", columns, frozenColumns, "Id", toolbar);
}