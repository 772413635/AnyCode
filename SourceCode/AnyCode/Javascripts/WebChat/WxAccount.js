var toolbar = [];
var table;
var columns = [[
    { field: 'ck', checkbox: true },
    {
        field: 'optt',
        title: '操作',
        width: 80,
        align: 'center',
        rowspan: 1,
        formatter: function (value, rowData, rowIndex) {
            var b = DealWithFunction('<a href="#" style="text-decoration:none;color:blue" onclick="showEdit(\'' + rowData.Id + '\');"> 编辑</a>', funList.UpdateData);
            return b;
        }
    },
    { title: '名称', field: 'AppName', align: 'center', width: 140, sortable: true },
    { title: 'Token', field: 'Token', align: 'center', width: 100, sortable: true },
    { title: 'AppId', field: 'AppId', align: 'center', width: 130, sortable: true },
    { title: 'AppSecret', field: 'AppSecret', align: 'center', width: 250, sortable: true },
    { title: '用户名', field: 'UserName', align: 'center', width: 150, sortable: true },
    { title: '密码', field: 'PassWord', align: 'center', width: 150, sortable: true }
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

toolbar = DealWithFunction([{
    id: 'btnadd',
    text: '添加',
    iconCls: 'icon-add',
    handler: function () {
        parent.addTab("添加公众号", "/WebChat/WxAccount_Add");
    }
}], funList.AddData, toolbar);

toolbar = DealWithFunction([{
    id: 'btndel',
    text: '删除',
    iconCls: 'icon-remove',
    handler: function () {
        DeleteTable("/WebChat/DeleteAccount", "#table");
    }
}], funList.DeleteAllData, toolbar);
/***********************************/

$(function () {
    BaseLoadTable("/WebChat/WxCfgList", "#table", columns, null, "Id", toolbar, "#formsearch");
});


//查询
function Search() {
    BaseSearch("#formsearch", "#table");
}
//编辑
function showEdit(id) {
    parent.addTab("编辑公众号", "/WebChat/WxAccount_Edit/" + id);
}