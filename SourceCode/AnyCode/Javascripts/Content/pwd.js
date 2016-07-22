
var toolbar = [];
var table;
var columns = [[
    { field: 'ck', checkbox: true },
    {
        field: 'optt',
        title: '操作',
        width: 100,
        align: 'center',
        rowspan: 1,
        formatter: function (value, rowData, rowIndex) {
            var a = DealWithFunction('<button  type=\'button\' class=\'ui-button-blue\' onclick="showEdit(\'' + rowData.Id + '\');"> 编辑</button>', funList.UpdateData);
            a += "&nbsp;<button type='button' class='showbtn ui-button-pink' onclick='showPwd(" + rowIndex + ",1)'>显示</button>";
            return a;
           
        }
    },

    { title: '名称', field: 'Name', align: 'center', width: 200, sortable: true },
    { title: '用户名', field: 'UserName', align: 'center', width: 200, sortable: true,formatter:function(v) {
        return "<span class='nameSpan'>******</span>&nbsp";
    } },
    {
        title: '密码', field: 'Password', align: 'center', width: 200, sortable: true, formatter: function (v, d, n) {
            return "<span class='pwdSpan'>******</span>&nbsp";
        }
    },
    { title: '备注', field: 'Remark', align: 'center', width: 360, sortable: true },
    { title: '创建时间', field: 'CreateTime', align: 'center', width: 150, sortable: true,formatter: function(v) {
        return $.JsonToDate(v).fullDateString;
    } }

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
        parent.addTab("添加PassPage", "/Content/Pwd_Add");
    }
}], funList.AddData, toolbar);

toolbar = DealWithFunction([{
    id: 'btndel',
    text: '删除',
    iconCls: 'icon-remove',
    handler: function () {
        DeleteTable("/Content/DeletePwd", "#table");
    }
}], funList.DeleteAllData, toolbar);
/***********************************/
$(function () {
    var option = BaseOptions("/Content/PwdList", "Id", columns, null, toolbar, "#formsearch");
    option.queryParams.SortName = "CreateTime";
    option.queryParams.SortOrder = "desc";
    BaseLoadTableByOptions(option, "#table");
});
//查询
function Search() {
    BaseSearch("#formsearch", "#table");
}
//编辑
function showEdit(id) {
    parent.addTab("编辑PassPage", "/Content/Pwd_Edit/" + id);
}
//显示密码

function showPwd(n, type) {
    switch (type) {
        case 1://显示
            {

                var data = $("#table").datagrid('getData');
                var userName = data.rows[n].UserName;
                var password = data.rows[n].Password;
                var usertoken = $("#UserToken").val();
                $(".showbtn:eq(" + n + ")").attr("onclick", "showPwd(" + n + ",0)").text("隐藏").removeClass("ui-button-pink").addClass("ui-button-green");
                $(".nameSpan:eq(" + n + ")").text(uncMe(userName, usertoken));
                $(".pwdSpan:eq(" + n + ")").text(uncMe(password, usertoken));
            } break;
        case 0://隐藏
            {
                $(".showbtn:eq(" + n + ")").attr("onclick", "showPwd(" + n + ",1)").text("显示").removeClass("ui-button-green").addClass("ui-button-pink");

                $(".nameSpan:eq(" + n + ")").text("******");
                $(".pwdSpan:eq(" + n + ")").text("******");


            } break;
    }

}