$(function () {
    loadFiles();
});
var tdata;
function loadFiles() {
    var li = "";
    $.ajax({
        url: '/System/GetErrorFilesList',
        type: 'post',
        dataType: 'json',
        success: function (data) {
            tdata = data;
            for (var i = 0; i < data.length; i++) {
                li += "<li title='" + data[i]["Name"] + "' onclick='readyReadFile(" + i + ",this)'>" +data[i]["Name"] + "</li>";
            }
            $("#errorFiles").append(li);
        }
    });
}
var key;
var clickKey;

function readyReadFile(path, dmo) {
    $("#errorFiles li").removeClass("li_active");
    $(dmo).addClass("li_active");
    if (key != null) {
        clearTimeout(key);
    }
    $("#errorContent").text("");
    $("#dia").window({
        title: '正在加载文档，请稍后！',
        width: 300,
        height: 70,
        shadow: true,
        modal: true,
        resizable: false,
        collapsible: false,
        minimizable: false,
        maximizable: false,
        closable: false
    });

    $("#errorbai").show();
    $("#dia").window('open');
   
    readFile(path);

}

function readFile(path) {
    $.ajax({
        url: '/system/ReadFile',
        type: 'post',
        dataType: 'text',
        data: { path: tdata[path]["Path"] },
        success: function (data) {
            if (data.split('$')[2] != 0) {//空文件
                var chu = data.split('$')[1] / data.split('$')[2];
                $("#errorContent").append(formaterText(data.split('$')[0]));
                $("#errorbai").progressbar("setValue", Math.floor(chu * 100));
                if (chu == 1) {
                    clearTimeout(key);//读完了就不再读取了
                    key = null;
                    $("#dia").window('close');
                } else {
                    key = setTimeout(readFile(path), 1000);
                }
            } else {
                $("#errorbai").progressbar("setValue", Math.floor(100));
                $("#errorContent").text("没有内容");
                $("#dia").window('close');
            }
           
        }
    });
}

function formaterText(text) {
    var z5 = /\n/g;
    var z6 = /\r\n/g;
    var z7 = /\d+/g;
    var q5 = "<br>";
    var q6 = "\n";
    
    var newtext = text.replace(z5, q5);
    newtext = newtext.replace(z6, q6);
    var matchResult = newtext.match(z7);
    for (var m in matchResult) {
        newtext = newtext.replace(matchResult[m], "<span style='color:red;font-weight:bold'>" + matchResult[m] + "</span>");
    }
    return newtext;
}