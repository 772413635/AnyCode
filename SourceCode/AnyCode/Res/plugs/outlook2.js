
easyloader.locale = "zh_CN"; // 本地化设置
easyloader.theme = "metro-gray"; // 设置主题
$.extend($.fn.tabs.defaults, {
    backTabTitile: "",//返回标题
    backTabUrl: "",//返回tab的ur
    thisUrl: ""//当前tab的ur
});
using(['messager', 'window', 'accordion', 'tabs'], function () {

    $('#editpass').click(function () {
        $('#w').window('open');
    });

    $('#btnEp').click(function () {
    });

    $('#btnCancel').click(function () {
    });
});

//初始化左侧
function InitLeftMenu(menus) {
    $.each(menus, function (i, n) {
        var menulist = '<div title="' + n.MenuName + '" iconCls="' + n.Icon + '" style="overflow:auto;padding:10px;">';
        menulist += '<ul>';
        $.each(n.Menus, function (j, o) {
            menulist += '<li><div><a ref="' + o.MenuId + '" href="javascript:;" rel="/' + o.Url + '" title="' + (o.Remark||"") + '" ><span class="icon ' + o.Icon + '" >&nbsp;</span><span class="nav">' + o.MenuName + '</span></a></div></li> ';
        });
        menulist += '</ul></div>';
        $('#nav').append(menulist);
    });
    
    $("#nav").accordion({
        fit: true,
        border: false,
        animate: true
    });

    $('.accordionDiv li a').click(function () {
        var tabTitle = $(this).children('.nav').text();     
        var url = $(this).attr("rel");
        var stitle = $(this).attr("title");
        var menuid = $(this).attr("ref");
        var icon = getIcon(menus, menuid);

        addTab(tabTitle, url, icon, stitle);
        $('.accordionDiv li div').removeClass("selected");
        $(this).parent().addClass("selected");
    }).hover(function () {
        $(this).parent().addClass("hover");
    }, function () {
        $(this).parent().removeClass("hover");
    });

}
//获取左侧导航的图标
function getIcon(menus, menuid) {
    var icon = 'icon ';
    $.each(menus, function (i, n) {
        $.each(n.Menus, function (j, o) {
            if (o.MenuId == menuid) {
                icon += o.Icon;
            }
        });
    });

    return icon;
}

function addTab(subtitle, url, icon) {
    if (!$('#tabs').tabs('exists', subtitle)) {
        var options = $("#tabs").tabs("getSelected").panel("options");//获取当前tab的配置
        $('#tabs').tabs('add', {
            title: subtitle,
            backTabTitile: options.title,
            backTabUrl: options.thisUrl,
            thisUrl: url,
            content: createFrame(url),
            closable: true,
            icon: icon,
            
        });
    } else {
        $('#tabs').tabs('select', subtitle);
        $('#mm-tabupdate').click();
    }
    tabClose();
}

function createFrame(url) {

    var s = '<iframe scrolling="hidden" frameborder="0"  src="' + url + '" class="iframe"></iframe>';
    return s;
}

function tabClose() {
    /*双击关闭TAB选项卡*/
    $(".tabs-inner").dblclick(function () {
        var subtitle = $(this).children(".tabs-closable").text();
        $('#tabs').tabs('close', subtitle);
    });
    /*为选项卡绑定右键*/
    $(".tabs-inner").bind('contextmenu', function (e) {
        $('#mm').menu('show', {
            left: e.pageX,
            top: e.pageY
        });

        var subtitle = $(this).children(".tabs-title").text();
        $('#tabs').tabs('select', subtitle);
        return false;
    });
}
//绑定右键菜单事件
function tabCloseEven() {
    //刷新
    $('#mm-tabupdate').click(function () {
        var currTab = $('#tabs').tabs('getSelected');
        if ($('#tabs').tabs('getTabIndex', currTab) == 0) {
            return false;
        }
        var url = $(currTab.panel('options').content).attr('src');
        $('#tabs').tabs('update', {
            tab: currTab,
            options: {
                content: createFrame(url)
            }
        });
    });
    //关闭当前
    $('#mm-tabclose').click(function () {
        var currTab = $('#tabs').tabs('getSelected');
        var index = $('#tabs').tabs("getTabIndex", currTab);
        if (index == 0) {
            return false;
        }
        $('#tabs').tabs('close', index);
    });
    //全部关闭
    $('#mm-tabcloseall').click(function () {
        $('.tabs-inner span').each(function (i, n) {
            if (i != 0) {
                var t = $(n).text();
                $('#tabs').tabs('close', t);
            }
        });
    });
    //关闭除当前之外的TAB
    $('#mm-tabcloseother').click(function () {
        $('#mm-tabcloseright').click();
        $('#mm-tabcloseleft').click();
    });
    //关闭当前右侧的TAB
    $('#mm-tabcloseright').click(function () {
        var nextall = $('.tabs-selected').nextAll();
        nextall.each(function (i, n) {
            var t = $('a:eq(0) span', $(n)).text();
            $('#tabs').tabs('close', t);
        });
        return false;
    });
    //关闭当前左侧的TAB
    $('#mm-tabcloseleft').click(function () {
        var prevall = $('.tabs-selected').prevAll();
        prevall.each(function (i, n) {
            if (i != prevall.length-1) {
                var t = $('a:eq(0) span', $(n)).text();
                $('#tabs').tabs('close', t);
            }

        });
        return false;
    });
}

//弹出信息窗口 title:标题 msgString:提示信息 msgType:信息类型 [error,info,question,warning]
function msgShow(title, msgString, msgType) {
    $.messager.alert(title, msgString, msgType);
}
