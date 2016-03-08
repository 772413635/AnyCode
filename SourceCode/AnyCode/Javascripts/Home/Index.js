$(function () {
    LoadMenu();
    //初始化连接
    wait();
    //每一小时提示一次查看消息
    HappyAsk();
});

function LoadMenu() {
    $.ajax({
        url: '/Home/GetMenu',
        data: { pids: $("#menu").val() },
        type: 'Post',
        dataType: 'json',
        success: function (data) {
            InitLeftMenu(data);
            tabCloseEven();
            tabClose();
        }
    });
}

var tempWin;
var lastUrl;
//添加标签
function addTabUrl(subtitle, url, win, last) {
    if (!$('#tabs').tabs('exists', subtitle)) {
        tempWin = win;
        lastUrl = last;
        $('#tabs').tabs('add', {
            title: subtitle,
            content: createFrame(url),
            closable: true
        });
    } else {
        $('#tabs').tabs('close', subtitle);
        ChoseTab(subtitle, url);
    }
    tabClose();
}


function ChoseTab(subtitle, url) {
    if (!$('#tabs').tabs('exists', subtitle)) {
        $('#tabs').tabs('add', {
            title: subtitle,
            content: createFrame(url),
            closable: true
        });
    } else {
        $('#tabs').tabs('select', subtitle);
        $('#mm-tabupdate').click();
    }
    tabClose();
}

function TabExists(subtitle) {
    if ($('#tabs').tabs('exists', subtitle)) {
        return true;
    }
    else {
        return false;
    }
}

//根据title刷新tab
function refreshTab(subtitle) {

    if ($('#tabs').tabs('exists', subtitle)) {
        var currTab = $('#tabs').tabs('getTab', subtitle);
        var iframe = $(currTab.panel('options').content);
        var src = iframe.attr('src');
        $('#tabs').tabs('update', { tab: currTab, options: { content: createFrame(src) } });
    }
}

//刷新当前选中的tab
function refreshSelectTab() {
    var selectTabOptions = getSelectTabOptions();//获取当前tab的options
    if ($('#tabs').tabs('exists', selectTabOptions.title)) {
        var currTab = $('#tabs').tabs('getTab', selectTabOptions.title);
        var iframe = $(currTab.panel('options').content);
        var src = iframe.attr('src');
        $('#tabs').tabs('update', { tab: currTab, options: { content: createFrame(src) } });
    }
}


function closeTab(subtitle) {
    if ($('#tabs').tabs('exists', subtitle)) {

        $('#tabs').tabs('close', subtitle);

    }

}

function backTab(srcStr) {
    var iframeElement = document.getElementsByTagName("iframe");
    for (var i = 0; i < iframeElement.length; i++) {
        if (iframeElement[i].src.indexOf(srcStr) > 0) {
            var win = iframeElement[i];
            var cWin = win.contentWindow;
            cWin.location.herf = history.go(-1);
        }
    }
}

function selectTab(subtitle, url) {
    if ($('#tabs').tabs('exists', subtitle)) {

        $('#tabs').tabs('select', subtitle);

    }
    else {
        addTab(subtitle, url);
    }
}

function doBack(subtitle, srcStr) {
    if ($('#tabs').tabs('exists', subtitle)) {

        var iframeElement = document.getElementsByTagName("iframe");
        for (var i = 0; i < iframeElement.length; i++) {
            if (iframeElement[i].src.indexOf(srcStr) >= 0) {
                var win = iframeElement[i];
                var getDoc = win.contentDocument;
                var getelement = getDoc.getElementById("btnRefreshTables");
                if (getelement != null) {
                    getelement.click();
                }
            }
        }

    }
}

function getSelectTabOptions() {
    return $("#tabs").tabs("getSelected").panel("options");
}
//设置登录窗口
function openPwd() {
    $('#w').window({
        title: '修改密码',
        width: 300,
        modal: true,
        shadow: true,
        closed: true,
        height: 220,
        resizable: true
    });
}



function winChange() {
    $('#divChange').window({
        collapsible: false,
        minimizable: false,
        maximizable: false
        
    });
    $('#divChange').window('open');
};

function showpwdedit() {
    winChange();
}

function Exit() {
    $.messager.confirm('系统提示', '您确定要退出本次登录吗?', function (r) {
        if (r) {
            $.ajax({
                url: '/Home/Exit',
                type: "post",
                success: function (data) {
                    if (data == '1') {
                        var url = '/Account/Index';
                        window.location.href = url;
                    }
                    else {
                        alert("退出失败！");
                    }
                }
            });
        }
    });

}

function show(data) {
    $.messager.show({
        height: 300,
        width: 300,
        title: '新消息',
        msg: escapeString(data),
        timeout: 0,
        showType: 'slide'
    });

}
function show2(data) {
    $.messager.show({
        height: 300,
        width: 300,
        title: '友情提醒',
        msg: data,
        timeout: 0,
        showType: 'slide'
    });

}

function NewsList() {

    parent.addTabUrl("公告列表", '/News/NewsList', window, "NewsList");


}

function ShowNews(id) {
    parent.addTabUrl("查看公告", '/News/ShowNews/' + id, window, "ShowNews");
    $(".newsli[data-id='" + id + "']").find(".ntitleNoRead").removeClass("ntitleNoRead");

}
//刷新公告列表
function RefenNews() {
    $.ajax({
        url: '/News/HomeNewsList',
        type: "Post",
        success: function (data) {
            var ss = $('#tb-news');
            jQuery(ss).find("li").remove();
            if (data.length > 0) {
                $("#noNews").remove();//移除没有消息的提示
                if ($(".t-list").length == 0) {
                    $("#tb-news").after('<div class="t-list">' +
                                       '<a href="#" onclick="NewsList()">更多>></a>' +
                                   '</div>');
                }

                for (var i = 0, length = data.length; i < length; i++) {
                    var news = "<span class='ntitle'>" + data[i]["Title"] + "</span>";
                    var dateString = $.JsonToDate(data[i]["CreateTime"]).fullDateString;
                    var date = dateString.substring(0, dateString.indexOf(' '));
                    if (data[i]["IsRead"] == '0') {//未读
                        news = "<span class='ntitle ntitleNoRead'>" + data[i]["Title"] + "</span>";
                    }
                    ss.append('<li class="newsli" data-id="' + data[i]["Id"] + '"><a href="#" onclick="ShowNews(\'' + data[i]["Id"] + '\')" >' + news + '<span class="ndate">[' + date + ']</span></a></li>');


                }
            }

        }
    }
    );
}

function wait() {
    $.post("comet_broadcast.asyn", { content: "-1", code: $("#userId").val(), loginUserType: $("#isSystem").val() },
        function (data, status) {
            show(data);
            RefenNews();
            window.setTimeout("wait()", 1000 * 1);
        }, "html");


}
function HappyAsk() {
    $.post("comet_broadcast.asyn", { content: "-2", code: $("#userId").val(), loginUserType: $("#isSystem").val() },
        function (data, status) {
            var news = data;
            if (news > 0) {
                var divNews = $("#news").clone().append("您有<span style='color:red'>" + news + "</span>条未查看的消息，请及时查看");
                show2(divNews);
            }
            window.setTimeout("HappyAsk()", 1000 * 60 * 60);
        }, "html");


}

function changepwd() {
    $.ajax({
        url: '/System/ChangePwd',
        type: 'post',
        data: $("#formChange").serialize(),
        dataType: 'text',
        success: function (data) {
            showToastMessage(data);
            $('#divChange').window('close');
        }
    });
}

//字符串转义函数

function escapeString(s) {
    return s;
}

//弹出弹出框
function FatherWindow(title, dmo, width, height, tools) {
    $("#content").html(dmo);
    $("#windowContent").window({
        title:title,
        width: width,
        height: height,
        modal: true,
        minimizable: false,
        maximizable:false,
        collapsible: false,
        tools: tools
    });
}