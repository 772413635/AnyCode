var toolbar = [];
var table;
var columns = [[
    { field: 'ck', checkbox: true },
    {
        field: 'MsgOrGroupId',
        title: '操作',
        width: 80,
        align: 'center',
        rowspan: 1,
        formatter: function (value, rowData, rowIndex) {
        }
    },
    { title: '公众号名称', field: 'AppName', align: 'center', width: 140, sortable: true },
    { title: 'Token', field: 'Token', align: 'center', width: 100, sortable: true },
    { title: '菜单Code', field: 'Code', align: 'center', width: 200, sortable: true },
    { title: '消息类型', field: 'MsgTypeName', align: 'center', width: 250, sortable: true }
]];

/*************功能权限***************/
toolbar = DealWithFunction([{
    id: 'btncut',
    text: '查询',
    iconCls: 'icon-search',
    handler: function () {
        flexiQuery('#divQuery', "查询");
    }
}, '-'], funList.SearchData, toolbar);


$(function () {
    BaseLoadTable("/WebChat/MsgMapList", "#table", columns, null, "Id", toolbar, "#formsearch");
});


//查询
function Search() {
    BaseSearch("#formsearch", "#table");
}