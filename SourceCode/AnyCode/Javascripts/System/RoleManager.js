$(function () {
    LoadTale();
});

function LoadTale() {
    var frozenColumns = [[
    { field: 'ck', checkbox: true },
    {
        field: 'opt', title: '操作', width: 150, align: 'center', rowspan: 2,
        formatter: function (value, rowData, rowIndex) {
            var update = '<a href="#" style="text-decoration:none;color:blue" onclick="showEdit(\'' + rowData.Id + '\');">编辑</a>';
            return DealWithFunction(update, funList.UpdateData);
        }
    },
    { title: '角色名', field: 'RoleName', sortable: true, align: 'center', width: 100 }
    ]];
    var column = [[
    {
        title: '管理员角色', field: 'IsSystem', align: 'center', width: 100,
        formatter: function (value) {
            if (value == "True") {
                return "<span style='color:green'>是</span>";
            } else {
                return "<span style='color:red'>否</span>";
            }
        }
    },
            {
                title: '说明', field: 'ss', align: 'center', width: 120,
                formatter: function (value, rowData) {
                    if (rowData.Id == 7 || rowData.Id == 1) {
                        return "系统内置角色";
                    } else {
                        return "用户自定义角色";
                    }
                }
            }
    ]];

    var toolbar = [];

    toolbar = DealWithFunction([{
        id: 'btncut',
        text: '查询',
        iconCls: 'icon-search',
        handler: function () {
            flexiQuery();
        }
    }, '-'], funList.SearchData, toolbar);
    toolbar = DealWithFunction([{
        id: 'btnadd',
        text: '新建',
        iconCls: 'icon-add',
        handler: function () {
            flexiCreate();
        }
    }], funList.AddData, toolbar);
    toolbar = DealWithFunction([{
        id: 'btncut',
        text: '删除',
        iconCls: 'icon-remove',
        handler: function () {
            deleteItem();
        }
    }], funList.DeleteAllData, toolbar);



    var companyid = $("#companyId").val();
    BaseLoadTable('/System/GetRoleTable/' + companyid, '#test', column, frozenColumns, "Id", toolbar);
}


//查询
function flexiQuery() {
    $("#divQuery").show().dialog({
        title: '查询',
        width: 400,
        height: 130,
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

//删除
function deleteItem() {
    var isadmin = false;
    var selection = $('#test').datagrid('getSelections');
    for (var i = 0; i < selection.length; i++) {
        if (selection[i].Id == 7 || selection[i].Id == 1) {
            AlertWarning("包含系统内置角色,无法删除!");
            isadmin = true;
            return false;
        }
    }
    if (!isadmin) {
        DeleteTable('/System/DeleteRole', '#test');
    }
}

//新建
function flexiCreate() {
    parent.addTab("新建角色", "/System/Role_Add");
}
//编辑 
function showEdit(id) {
    parent.addTab("编辑角色", "/System/GetRoleModelView/" + id);
}