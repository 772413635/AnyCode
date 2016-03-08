$(function () {
    $("#createMenu,#setMenuAction").button({
        icons: {
            primary: "ui-icon-contact"
        }
    });
    createComboBox();
    createPanel();

});
//生成panel
function createPanel() {
    $("#fartherMenu").panel({
        title: "菜单管理",
        height: $(document).height() - 46,
        tools: [
            {
                id: 'btnadd',
                iconCls: 'icon-add',
                handler: function () {
                    //添加一级菜单
                    var fatherMenuCount = $("#erMenu dt").length;
                    if (fatherMenuCount < 3) {
                        $.messager.prompt("添加一级菜单", "还能添加" + (3 - fatherMenuCount) + "个一级菜单，请输入名称（4个汉字或8个字母以内)", function (v) {

                            if (WordCount(v) > 8) {
                                AlertInfo('4个汉字或8个字母以内');
                            } else {
                                $.ajax({
                                    url: "/WebChat/AddFartherMenu",
                                    type: "post",
                                    dataType: "text",
                                    data: { name: v, cfgId: $("#appList").combobox("getValue") },
                                    success: function (res) {
                                        showToastMessage(res);
                                        refshMenuManager();
                                    }
                                });
                            }
                        });
                    } else {
                        AlertInfo('一级菜单最多3个');
                    }
                }
            }, {
                id: 'btnbommanager',
                iconCls: 'icon-eye',
                handler: function () {
                    alert("预览");
                }
            }
        ]
    });
    $("#childMenu").panel({
        title: "请在左侧选择菜单",
        height: $(document).height() - 46,
        tools: [
            {
                id: 'btnadd',
                iconCls: 'icon-edit',
                handler: function () {
                    if (getMenuActiveCount() === 0) {
                        AlertWarning("请选择一个按钮后再操作");
                    } else {
                        var menu = $("#erMenu").find(".menu_active");
                        var menuid = menu.data("id");
                        $.messager.prompt("修改名称", "请输入菜单名称进行修改！", function (v) {
                            if (v) {
                                if (v != "") {
                                    $.ajax({
                                        url: "/WebChat/ReName",
                                        type: "post",
                                        dataType: "text",
                                        data: { menuid: menuid, newName: v },
                                        success: function (res) {
                                            showToastMessage(res);
                                            refshMenuManager();
                                        }

                                    });
                                }
                            }
                        });
                    }
                }
            }, {
                id: 'btnremove',
                iconCls: 'icon-remove',
                handler: function () {
                    if (getMenuActiveCount() === 0) {
                        AlertWarning("请选择一个按钮后再操作");
                    } else {
                        var menuid = $("#erMenu").find(".menu_active").data("id");
                        var isermenu = $("#erMenu").find(".menu_active").data("isermenu");
                        $.messager.confirm("提示", isermenu === 1 ? "删除此菜单的同时其下子菜单也会删除，您确定要继续？" : "您确定要删除此菜单吗？", function (r) {
                            if (r) {
                                $.ajax({
                                    url: "/WebChat/DelteMenu",
                                    data: { menuid: menuid },
                                    dataType: "text",
                                    type: "post",
                                    success: function (res) {
                                        showToastMessage(res);
                                        refshMenuManager();
                                    }
                                });
                            }
                        });

                    }
                }
            }
        ]
    });
}



//生成下拉框
function createComboBox() {
    $("#appList").combobox({
        url: "/WebChat/AccountList",
        valueField: 'Id',
        textField: 'AppName',
        method: "POST",
        onSelect: function (record) {
            $("#token").text(record.Token);
            $("#appId").text(record.AppId);
            $("#appSecret").text(record.AppSecret);
            createMenu(record.Id);
        },
        onChange: function () {
            $("#token").text("");
            $("#appId").text("");
            $("#appSecret").text("");
            $("#erMenu").html("");
            $("#childMenu").html("").prev().find(".panel-title").text("请在左侧选择菜单");

        },
        onLoadSuccess: function () {

            $(this).combobox("select", 1);
        }
    });
}

//生成菜单
function createMenu(id) {
    $.ajax({
        url: '/WebChat/MenuList',
        data: { id: id },
        type: "post",
        dataType: 'json',
        success: function (data) {
            $("#erMenu").html("");
            var erMenuContent = "";
            var iconDot = '<i class="icon_dot">●</i>';
            for (var i = 0; i < data.button.length; i++) {

                if (data.button[i]["sub_button"] != undefined && data.button[i]["sub_button"].length > 0) {//有子菜单
                    erMenuContent += ' <dt data-isermenu=' + 1 + ' data-id=' + data.button[i].id + ' data-ermenucount=' + data.button[i].sub_button.length + ' data-btntype=' + data.button[i].typeid + ' data-url=\'' + data.button[i].url + '\' data-key=\'' + data.button[i].key + '\'>' + data.button[i].name;
                    for (var j = 0; j < data.button[i].sub_button.length; j++) {
                        var id = data.button[i].sub_button[j].id;
                        var type = data.button[i].sub_button[j].typeid;
                        var url = data.button[i].sub_button[j].url || null;
                        var key = data.button[i].sub_button[j].key || null;
                        erMenuContent += '<dd data-id=' + id + ' data-key=\'' + key + '\' data-url=\'' + url + '\' data-btntype=' + type + '>' + iconDot + '<a href="javascript:;">' + data.button[i].sub_button[j].name + '</a>';
                    }
                    erMenuContent += "</dt>";
                } else {//没有子菜单
                    erMenuContent += ' <dt data-isermenu=' + 0 + ' data-id=' + data.button[i].id + ' data-btntype=' + data.button[i].typeid + ' data-url=\'' + data.button[i].url + '\' data-key=\'' + data.button[i].key + '\'>' + data.button[i].name+ '</dt>';
                }
            }
            $("#erMenu").html(erMenuContent);
            menuAction();
        }
    });
}

//菜单事件
function menuAction() {
    $("#erMenu dt").bind("click", function () {
        $("#erMenu dt").removeClass("menu_active");
        $("#erMenu dd").removeClass("menu_active");
        $(this).addClass("menu_active");
        $("#childMenu").panel("setTitle", "一级菜单:" + $(this).text());
        var isErMenu = $(this).data("isermenu");
        if (isErMenu == 1) {//有二级菜单
            var addAction = $("#addAction").clone().attr("id", "#addAction_neww");
            var erMenuCount = $(this).data("ermenucount");//二级菜单个数
            addAction.find(".actionTitle").html("已经为“<span class='btnName'>" + $(this).text() + "</span>”添加了二级菜单，无法设置其他动作。你还可以添加<span class='erMenuCount'>" + (5 - erMenuCount) + "</span>个二级菜单");
            addAction.find("#actionList li:eq(1)").remove();//移除设置菜单事件图标
            $("#childMenu").html(addAction.show());
        } else {//没有二级菜单
            var btnType = $(this).data("btntype");//获取菜单类型，默认为null
            if (btnType != null) {//设置了菜单的事件

                SetFatherEvent();
                $("#ChildrenAction #reAddChlMenu").remove();
                $("#ChildrenAction:first li:last").append('<button id="reAddChlMenu" type="button" class="btn-sub ui-button-orange" onclick="AddChlMenu()">添加二级菜单</button>');
                $("#reAddChlMenu").button({
                    icons: {
                        primary: "ui-icon-grip-dotted-vertical"
                    }
                });
            } else {//未设置菜单的事件
                var addAction2 = $("#addAction").clone().attr("id", "#addAction_neww");
                addAction2.find(".actionTitle").html("请选择订阅者点击“<span class='btnName'>" + $(this).text() + "</span>”菜单后，公众号做出的响应动作");
                $("#childMenu").html(addAction2.show());
            }

        }

    });

    //二级菜单点击事件（未开发完成）
    $("#erMenu dd").bind("click", function () {
        $("#erMenu dt").removeClass("menu_active");
        $("#erMenu dd").removeClass("menu_active");
        $(this).addClass("menu_active");
        var childrenAction = $("#ChildrenAction").clone();
        var btnType = $(this).data("btntype");//按钮类型
        var btnName = $(this).find("a").text();//按钮名称
        var key = $(this).data("key");//按钮的code值
        var url = $(this).data("url");//url值
        var id = $(this).data("id");//id值
        $("#childMenu").panel("setTitle", "二级菜单:" + btnName);
        childrenAction.find(".menuType").val(btnType);
        childrenAction.find(".inputtext").val( url|| key);
        childrenAction.find(".btn-sub").attr("onclick", "SetChildrenBtn(" + id + ")");
        childrenAction.find(".actionTitle").html("请设置订阅者点击“<span class='btnName'>" + btnName + "</span>”菜单后，公众号做出的响应动作");
        $("#childMenu").html(childrenAction.show());
        $(".actionForm").validationEngine({
            promptPosition: "centerRight"
        });
    });
}

//设置子菜单
function SetChildrenBtn(id) {
    if ($(".actionForm").validationEngine('validate')) {
        var type = $("#childactionList .menuType").val();
        var val = $("#childactionList .inputtext").val();
        $.ajax({
            url: "/WebChat/SetChildrenBtn",
            type: "post",
            data: { id: id, type: type, val: val },
            dataType: "text",
            success: function (message) {
                showToastMessage(message);
                refshMenuManager();
            }
        });
    } else {
        return false;
    }

}

//设置一级菜单的事件
function SetFatherEvent() {
    var key = $(".menu_active").data("key");//按钮的code值
    var url = $(".menu_active").data("url");//url值
    var id = $(".menu_active").data("id");//id值
    var btnType = $(".menu_active").data("btntype");//按钮类型
    var btnName = $(".menu_active").text();//按钮名称
    var childrenAction = $("#ChildrenAction").clone();
    childrenAction.find(".menuType").val(btnType);
    childrenAction.find(".inputtext").val(url || key);
    childrenAction.find(".btn-sub").attr("onclick", "SetChildrenBtn(" + id + ")");
    childrenAction.find(".actionTitle").html("请设置订阅者点击“<span class='btnName'>" + btnName + "</span>”菜单后，公众号做出的响应动作");
    $("#childMenu").html(childrenAction.show());
    $(".actionForm").validationEngine({
        promptPosition: "centerRight"
    });
}

//获取选中按钮的个数
function getMenuActiveCount() {
    return $("#erMenu").find(".menu_active").length;
}

//添加二级菜单
function AddChildrenMenu() {
    if (getMenuActiveCount() === 0) {
        AlertWarning("请选择一个按钮后再操作");
    } else {
        var menuid = $(".menu_active").data("id");
        var fatherMenuCount = $(".menu_active～dd").length;
        if (fatherMenuCount < 5) {
            $.messager.prompt("添加二级菜单", "还能添加" + (5 - fatherMenuCount) + "个二级菜单，请输入名称（8个汉字或16个字母以内)", function (v) {
                var wc = WordCount(v);//输入文字的个数
                if (wc == 0) {
                    //未输入字符
                }
                else if (wc > 16) {
                    AlertInfo('8个汉字或16个字母以内');
                } else {
                    $.ajax({
                        url: "/WebChat/AddChildrenMenu",
                        type: "post",
                        dataType: "text",
                        data: { name: v, cfgId: $("#appList").combobox("getValue"), fid: menuid },
                        success: function (res) {
                            showToastMessage(res);
                            refshMenuManager();
                        }
                    });
                }
            });
        } else {
            AlertInfo('二级菜单最多5个');
        }
    }
}

//生成菜单
function CreateMenu() {
    $.ajax({
        url: "/WebChat/CreateMenu",
        type: 'post',
        dataType: "json",
        data: { cfgid: $("#appList").combobox("getValue"), appId: $("#appId").text() },
        success: function (data) {
            if (data == null || data == "") {
                AlertError("生成菜单发生错误");
            } else {
                if (data.errcode != 0) {
                    AlertError("生成菜单发生错误：" + data.errmsg);
                } else {
                    AlertInfo("生成菜单成功："+data.errmsg);
                }
                
            }
        }
    });
}

//计算文字长度
function WordCount(word) {
    if (word == null || word == "") {
        return 0;
    } else {
        var regex = /[a-zA-Z0-9]+/g;
        var regex2 = /[\u4e00-\u9fa5]+/g;
        var en = word.match(regex);
        var zh = word.match(regex2);
        var wordCount = 0;
        $(en).each(function (i, z) {
            wordCount += z.length;
        });
        $(zh).each(function (i, z) {
            wordCount += z.length * 2;
        });
        return wordCount;
    }

}

//刷新菜单配置界面
function refshMenuManager() {
    var value = $("#appList").combobox('getValue');
    $("#appList").combobox('select', value);
    $("#childMenu").html("").prev().find(".panel-title").text("请在左侧选择菜单");
}

//重新选择二级菜单
function AddChlMenu() {
    $.messager.confirm("警告", "添加二级菜单将移除此菜单上面的事件",function(r) {
        if (r) {
            AddChildrenMenu();
        }
    });
}