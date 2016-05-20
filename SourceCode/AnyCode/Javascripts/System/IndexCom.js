$(function () {
    LoadTale();
});

function LoadTale() {
    var column = [[
    {
        title: '状态', field: 'statusName', align: 'center', width: 100,
        styler: function (value, row, index) {
            if (row.statusName == "启用")
                return 'color:green;';
            else
                return 'color:red;';
        }
    },
    { title: '手机号码', field: 'MobilePhoneNumber', align: 'center', width: 100 },
    { title: '办公电话', field: 'PhoneNumber', align: 'center', width: 100 },
    {
        title: '创建时间', field: 'CreateTime', align: 'center', width: 200, formatter: function (val) {
            return $.JsonToDate(val).fullDateString;
        }
    },
    {
        title: '更新时间', field: 'UpdateTime', align: 'center', width: 200, formatter: function (val) {
            if (val!=null) {
                return $.JsonToDate(val).fullDateString;
            }
           
        }
    },
        {
            title: '说明', field: 'ss', align: 'center', width: 120,
            formatter: function (value, rowData) {
                if (rowData.Id == 2) {
                    return "系统内置用户";
                } else {
                    return "用户自定义用户";
                }
            }
        }
    ]];
    var frozenColumns = [[
        { field: 'ck', checkbox: true },
        {
            field: 'opt', title: '操作', width: 150, align: 'center', rowspan: 2,
            formatter: function (value, rowData, rowIndex) {

                var update = '<a href="#" style="text-decoration:none;color:blue" onclick="showEdit(\'' + rowData.Id + '\');">编辑</a>';
                var resetPassword = '&nbsp&nbsp<a href="#" style="text-decoration:none;color:blue" onclick="ReplacePwd(\'' + rowData.Id + '\',\'' + rowData.Name + '\');">重置密码</a>';
                update = DealWithFunction(update, funList.UpdateData);
                resetPassword = DealWithFunction(resetPassword, funList.ResetPassword);
                return update + resetPassword;
            }
        },
        { title: '用户名', field: 'Name', sortable: true, align: 'center', width: 100 },
        { title: '姓名', field: 'MyName', align: 'center', width: 100 },
        { title: '角色', field: 'RoleName', align: 'center', width: 100 }
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
    BaseLoadTable('/System/GetUserTable/' + companyid, '#test', column, frozenColumns, "Id", toolbar);
}

function flexiQuery() {
    $("#divQuery").show().dialog({
        title: '查询',
        width: 400,
        height: 165,
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

//添加用户
function flexiCreate() {
    parent.addTab("新建用户", "/System/User_Add");
}

//重置密码
function ReplacePwd(id, name) {
    var confir = confirm("您确定要修改用户 " + name + " 的密码吗？");
    if (!confir) {
        return;
    }
    $.ajax({
        url: '/System/ReplacePwd',
        type: "post",
        data: { id: id },
        success: function (data) {
            if (data.length == 6) {
                alert("修码成功，新密码为：" + data);
            } else {
                alert("修改失败!");
            }

        }
    });
}

//编辑用户
function showEdit(data) {
    var url = '/System/User_Edit/' + data;
    parent.addTab("编辑用户", url);
}
//删除用户
function deleteItem() {
    var isadmin = false;
    var selection = $('#test').datagrid('getSelections');
    for (var i = 0; i < selection.length; i++) {
        if (selection[i].Id == 2) {
            AlertWarning("包含系统内置账户,无法删除!");
            isadmin = true;
            return false;
        }
    }
    if (!isadmin) {
        DeleteTable('/System/DeleteUser', '#test');
    }
}