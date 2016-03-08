
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
            var a = DealWithFunction('<a href="#" style="text-decoration:none;color:blue" onclick="showDetails(\'' + rowData.Id + '\',\'' + rowData.Title + '\');">查看 </a>', funList.ShowData);
            var b = DealWithFunction('<a href="#" style="text-decoration:none;color:blue" onclick="showEdit(\'' + rowData.Id + '\');"> 编辑</a>', funList.UpdateData);
            return a + b;
        }
    },

    { title: '标题', field: 'Title', align: 'center', width: 420, sortable: true },
     {
         title: '添加时间', field: 'CreateTime', align: 'center', width: 120, sortable: true, formatter: function (val) {
             return $.JsonToDate(val).fullDateString;
         }
     },

    { title: '添加人', field: 'MyName', align: 'center', width: 100, sortable: true }

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
        parent.addTab("添加备忘", "/Content/Text_Add");
    }
}], funList.AddData, toolbar);

toolbar = DealWithFunction([{
    id: 'btndel',
    text: '删除',
    iconCls: 'icon-remove',
    handler: function () {
        DeleteTable("/Content/DeleteText", "#test");
    }
}], funList.DeleteAllData, toolbar);
/***********************************/

$(function () {
    var option = BaseOptions("/Content/TextList", "Id", columns, null, toolbar, "#formsearch");
    //option.onLoadSuccess = function() {
    //    MergeCellsByField("#test", "Title,CreateTime");
    //};
    BaseLoadTableByOptions(option, "#test");
});
//查询
function Search() {
    BaseSearch("#formsearch", "#test");
}
//查看
function showDetails(id, title) {
    $.ajax({
        url: "/Content/ShowContent",
        type: "post",
        data: { id: id },
        dataType: "text",
        success: function (val) {

            window.parent.FatherWindow("查看内容", val, $(window).width() - 20, $(window).height() - 20, [{
                iconCls: "icon-print",
                handler: function () {
                    var options = {
                        mode: "iframe",
                        popClose: false,
                        extraCss: "",
                        retainAttr: ["class", "id", "style"],
                        extraHead: '<meta charset="utf-8" />,<meta http-equiv="X-UA-Compatible" content="IE=edge"/>'
                    };
                    $(window.parent.$("#content")).printArea(options);
                }
            }]);
        }
    });
}
//编辑
function showEdit(id) {
    parent.addTab("编辑备忘", "/Content/Text_Edit/" + id);
}