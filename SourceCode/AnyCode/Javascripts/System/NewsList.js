/// <reference path="NewsList.js" />
var option =
{
    iconCls: 'icon-save',
    idField: 'Id',
    nowrap: true,
    url: '/News/GetData',
    fit: true,
    loadMsg: '数据装载中......',
    striped: true,
    useRp: true,
    pageList: [5, 10, 15, 20],
    pagination: true,
    queryParams: {},
    pageSize: 20,
    rownumbers: true,
    singleSelect: true,
    frozenColumns: [[
        {
            field: 'optt',
            title: '操作',
            width: 100,
            align: 'center',
            rowspan: 1,
            formatter: function (value, rowData, rowIndex) {
                return '<a href="#" style="text-decoration:none;color:blue" onclick="showDetails(\'' + rowData.Id + '\');">查看</a>';
            }
        }
    ]],
    columns: [[
        {
            title: '标题', field: 'Title', align: 'center',width: 300, formatter: function (value, row, rowIndex) {
                if (row.IsRead == 'True') {
                    return value;
                } else {
                    return '<span style="font-weight:bold;color:red">' + value + '</span>';
                }
            }
        },
        { title: '发送时间', field: 'CreateTime', align: 'center', width: 120,formatter:function(val) {
            return $.JsonToDate(val).fullDateString;
        } },
        { title: '发送人', field: 'MyName', align: 'center', width: 100 }
    ]],
    toolbar: [
        {
            id: 'btncut',
            text: '查询',
            iconCls: 'icon-search',
            handler: function () {
                flexiQuery();
            }
        }]
};
option.queryParams = { RP: option.pageSize, Query: GetQueryText("#formsearch") };

$(function () {
    $('.btn-sub').button({
        icons: {
            primary: "ui-icon-search"
        }
    });

    $('#test').datagrid(option);
    var p = $('#test').datagrid('getPager');
    if (p) {
        $(p).pagination({
            onChangePageSize: function () {
                var queryParams = $('#test').datagrid('options').queryParams;
                queryParams.RP = $(p).pagination('options').pageSize;
            }
        });
    }


});

function Search() {
    var search = "";
    $('#formsearch').find(":text,:selected,select,textarea,:hidden,:checkbox,:password").each(function () {
        if (this.name.indexOf('[') == 0)//我们认为只有[开头的为需要处理的
            search = search + this.name + "&" + this.value + "^";
    });
    $("#test").datagrid('options').queryParams.Query = search;
    $("#test").datagrid('load');
}

function showDetails(id) {
    parent.addTabUrl("查看公告", '/News/ShowNews/' + id, window, "ShowNews");
    $("#test").datagrid('reload'); //刷新
    return;
}
function flexiQuery() {
    $('#divQuery').window({
        collapsible: false,
        minimizable: false,
        maximizable: false

    });
    $('#divQuery').window('open');
}