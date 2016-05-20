var tempData, selectNode;
$(function () {
    loadTable();
    LoadColumn();

    //初始化图标表
    $("#Iconic").IconList({
        url: "/System/IconList",
        type:'post',
        height: 100,
        left: 6,
        top: 5
    });
});
function loadTable() {
    var cloumn = [[
         { field: 'ck', checkbox: true },
         {
             title: '名称', field: 'Name', align: 'center', width: 180, sortable: true,
             formatter: function (value, rowData) {
                 var fun = "<a href='javascript:;' style='color:blue' onclick='Edit(" + rowData.Id + ")'>" + value + "</a>";
                 return DealWithFunction(fun, funList.UpdateData, null, "没有权限");
             }
         },
         { title: '属于', field: 'ParentId', align: 'center', width: 70, sortable: true },
         { title: '排序序列', field: 'Sort', align: 'center', width: 80, sortable: true },
         { title: '路径', field: 'Url', align: 'center', width: 200, sortable: true },
         { title: '控制器', field: 'Controller', align: 'center', width: 80, sortable: true },
         { title: '方法', field: 'Action', align: 'center', width: 120, sortable: true },
         {
             title: '图标', field: 'Iconic', align: 'center', width: 100, sortable: true,
             formatter: function (value) {
                 return "<span class='icon " + value + "'>&nbsp;&nbsp;&nbsp;</span>";
             }
         },
         { title: '备注', field: 'Remark', align: 'center', width: 120, sortable: true },
         {
             title: '状态', field: 'State', align: 'center', width: 50, sortable: true,
             formatter: function (value) {
                 if (value) {
                     value = 0;
                 } else {
                     value = 1;
                 }

                 var strs = new Array("启用", "禁用");
                 var colors = new Array("green", "red");
                 return "<span style='color:" + colors[value] + "'>" + strs[value] + "</span>";
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
        id: 'btncut',
        text: '新增',
        iconCls: 'icon-add',
        handler: function () {
            parent.addTab("新增菜单", "/System/MenuManagerAdd");
        }
    }], funList.AddData, toolbar);

    toolbar = DealWithFunction([{
        text: '编辑(可批量)',
        iconCls: 'icon-edit',
        handler: function () {
            MoreEdit();
        }
    }], funList.UpdateAllData, toolbar);

    toolbar = DealWithFunction([{
        id: 'btncut',
        text: '删除',
        iconCls: 'icon-remove',
        handler: function () {
            destroyItem();
        }
    }], funList.DeleteAllData, toolbar);

    BaseLoadTable("/System/GetMenu", "#test", cloumn, null, "Id", toolbar, "#formsearch");
}

function Edit(jj) {
    parent.addTab("编辑菜单", "/System/MenuManagerEdit/" + jj);
}

function flexiQuery() {
    $("#divQuery").show().dialog({
        title: '查询',
        width: 400,
        height: 130,
        resizable: true,
        modal: false,
        closed: true,
        buttons: [{
            text: "确定",
            handler: function () {
                Serch();
            }
        }]
    });
    $("#divQuery").dialog('open');
}

function Serch() {
    $(".tree-node").each(function () {
        if ($(this).attr("node-id") == $("#formsearch #ParentId").val())
            $(this).addClass("tree-node-selected");
        else
            $(this).removeClass("tree-node-selected");
    });

    BaseSearch("#formsearch", "#test");
}

function destroyItem() {
    DeleteTable('/System/DeleteMenu', '#test');
}

function MoreEdit() {
    var selection = $("#test").datagrid('getChecked');
    var ids = "";
    if (selection.length == 0) {
        AlertWarning("请选择需要编辑的数据");
    } else if (selection.length == 1) {
        Edit(selection[0].Id);
    } else {
        for (var i = 0; i < selection.length; i++) {
            ids += selection[i].Id + ",";
        }
        parent.addTab("批量编辑菜单", "/System/EditAllMenu/" + ids);
    }
}

//加载树
function LoadColumn() {
    $.ajax({
        url: '/system/GetColumn',
        type: 'post',
        data: { id: $("#left_div_search_text").val() },
        success: function (data) {
            var drop = "<option value=''>所有</option>";
            $("#ParentId").html("");
            for (var i = 0; i < data.length; i++) {
                drop += "<option value='" + data[i].id + "'>" + data[i].text + "</option>";
            }
            $("#ParentId").append(drop);
            tempData = data;
            $("#tree").tree({
                dnd: false,//拖放
                data: data,
                onClick: function (node) {
                    selectNode = node;
                    $('#tree').tree('select', node.target);
                    $("#formsearch #ParentId").val(node.id);
                    Serch();
                },
                onContextMenu: function (e, node) {
                    selectNode = node;
                    e.preventDefault();
                    $('#tree').tree('select', node.target);
                    $('#mm_Manager').menu('show', {
                        left: e.pageX,
                        top: e.pageY
                    });
                },
                onLoadSuccess: function (node, data) {
                    $(".tree-node").keydown(function (event) {
                        if (event.keyCode == '0x2E')
                            DeleteTree('/System/DeleteMenu', selectNode.id, LoadColumn);
                    });
                }
            });
        }
    });

}

function searchTree() {
    LoadColumn();
}

function treeEdit(title) {
    if (title != "添加菜单类别") {
        for (var i = 0; i < tempData.length; i++) {
            if (selectNode != null && tempData[i].id == selectNode.id) {
                $("#Id").val(tempData[i].id);
                $("#Name").val(tempData[i].text);
                $("#Sort").val(tempData[i].sort);
                $("#Iconic").val(tempData[i].iconCls);
                break;
            }
        }
    }

    $("#treeEdit").show().window({
        title: title,
        width: 420,
        height: 190,
        modal: true,//遮罩
        resizable: true,//拖放
        collapsible: false,//折叠
        minimizable: false,//最小化
        maximizable: true//最大化
    });
}
//更新树
function treeEditSave() {
    $.ajax({
        url: '/system/MenuManagerAdd',
        type: 'post',
        data: $("#treeEditFrom").serialize(),
        success: function (message) {
            showToastMessage(message);
            $("#treeEdit").hide().window('close');
            $("#Id").val("");
            $("#Name").val("");
            $("#Sort").val("");
            $("#Iconic").val("");
            LoadColumn();
        }
    });
}

//删除树
function treeDelete() {
    DeleteTree('/System/DeleteMenu', selectNode.id, LoadColumn);
}
