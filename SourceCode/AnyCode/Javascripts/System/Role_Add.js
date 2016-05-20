$(function () {
    if ($("#Id").val() == "Role_Add") {
        $("#Id").val("");
    }
    LoadQuanXian();
});

function LoadQuanXian() {
    var url = "";
    if ($("#Id").val() == "") {
        url = "/System/GetCompetencebyUserId/0"; //新增时获取所有权限
    } else {
        url = '/System/GetCompetenceByRoleId/' + $("#Id").val();; //编辑时匹配权限
    }
    $('#pTree').tree({
        checkbox: true,
        url: url
    });
}

//返回

function BackPage() {
    BaseBackPage();
}

//保存

function Save() {
    if ($("#fm").validationEngine('validate')) {
        $('.btn-sub').attr("disabled", true); //提交按钮设为,不可用   
        getChecked();
        $.ajax({
            type: "post",
            url: '/system/CreateRole',
            data: $("#fm").serialize(),
            dataType: "text",
            success: function (data) {
                $(".btn-sub").attr("disabled", false); //提交按钮设为,可用                      
                showToastMessage(data);
            }
        });
    }
    else
        return false;
}

function getChecked() {
    var tnodes = $('#pTree').tree('getChecked');
    var fnodes = new Array();
    var nodes = new Array();
    for (var n = 0; n < tnodes.length; n++) {
        if (tnodes[n].iconCls == null && tnodes[n].id != -1) {
            fnodes.push(tnodes.slice(n, parseInt(n) + 1)[0]);
        } else {
            nodes.push(tnodes.slice(n, parseInt(n) + 1)[0]);
        }

    }


    var s = '';
    var indnodes = $('#pTree').tree('getChecked', 'indeterminate');	// 获取不确定的节点
    for (var j = 0; j < indnodes.length; j++) {
        if (s != '') {
            s += ',';
        }
        s += indnodes[j].id;
    }
    for (var i = 0; i < nodes.length; i++) {
        if (s != '') s += ',';
        s += nodes[i].id;
    }
    $("#Pid").val(s);
    var fs = new Array();
    for (var f = 0 ;
    f < fnodes.length; f++) {//拼接功能权限
        var fid = $('#pTree').tree('getParent', fnodes[f].target).id;
        if (fs.length == 0) {
            fs.push({
                fid: fid,
                childid: [fnodes[f].id]
            });
            continue;
        }
        for (var c in fs) {
            if (fs[c].fid == fid) {
                fs[c].childid.push(fnodes[f].id);
                break;
            }
            if (c == fs.length - 1) {
                fs.push({
                    fid: fid,
                    childid: [fnodes[f].id]
                });
            }
        }

    }

    var funstring = "";
    for (var fi = 0 ; fi < fs.length; fi++) {
        funstring += fs[fi].fid + "[" + fs[fi].childid.join(';') + "],";
    }
    $("#Fid").val(funstring.substring(0, funstring.lastIndexOf(',')));


}