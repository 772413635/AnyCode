var editor;
KindEditor.ready(function (K) {
    K.each({
        'plug-align': {
            name: '对齐方式',
            method: {
                'justifyleft': '左对齐',
                'justifycenter': '居中对齐',
                'justifyright': '右对齐'
            }
        },
        'plug-order': {
            name: '编号',
            method: {
                'insertorderedlist': '数字编号',
                'insertunorderedlist': '项目编号'
            }
        },
        'plug-indent': {
            name: '缩进',
            method: {
                'indent': '向右缩进',
                'outdent': '向左缩进'
            }
        }
    }, function (pluginName, pluginData) {
        var lang = {};
        lang[pluginName] = pluginData.name;
        KindEditor.lang(lang);
        KindEditor.plugin(pluginName, function (K) {
            var self = this;
            self.clickToolbar(pluginName, function () {
                var menu = self.createMenu({
                    name: pluginName,
                    width: pluginData.width || 100
                });
                K.each(pluginData.method, function (i, v) {
                    menu.addItem({
                        title: v,
                        checked: false,
                        iconClass: pluginName + '-' + i,
                        click: function () {
                            self.exec(i).hideMenu();
                        }
                    });
                });
            });
        });
    });
    editor = K.create('#Content', {
        themeType: 'qq',
        items: [
            'bold', 'italic', 'underline', 'fontname', 'fontsize', 'forecolor', 'hilitecolor', 'plug-align', 'plug-order', 'plug-indent', 'lineheight', 'link', 'hr', 'table', 'removeformat', 'source'
        ]
    });

});

function BackPage() {
    BaseBackPage();
}

function Save() {
    if (editor.isEmpty()) {
        KindEditor.ready(function (K) {
            var dialog = K.dialog({
                width: 300,
                title: '提示',
                body: '<div style="margin:10px;"><strong>内容不可为空</strong></div>',
                closeBtn: {
                    name: '关闭',
                    click: function (e) {
                        dialog.remove();
                    }
                }
            });
        });

    } else if (editor.html().length > 8000) {
        KindEditor.ready(function (K) {
            var dialog = K.dialog({
                width: 300,
                title: '提示',
                body: '<div style="margin:10px;"><strong>内容过长</strong></div>',
                closeBtn: {
                    name: '关闭',
                    click: function (e) {
                        dialog.remove();
                    }
                }
            });
        });
    } else if ($("#fm").validationEngine('validate')) {
        $("#Content").val((editor.html()));
        $('.btn-sub').attr("disabled", true); //提交按钮设为,不可用
        $.ajax({
            type: "post",
            url: '/Content/CreateText',
            data: $("#fm").serialize(),
            dataType: "text",
            success: function (data) {
                $(".btn-sub").attr("disabled", false); //提交按钮设为,可用
                showToastMessage(data);
            }
        });
    } else
        return false;

}