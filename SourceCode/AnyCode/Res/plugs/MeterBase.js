//获取url中的参数
$.GetRequest = function () {
    /// <summary>获取url中的参数</summary>
    var url = location.search; //获取url中"?"符后的字串

    var theRequest = new Object();
    theRequest.count = 0;
    if (url.indexOf("?") != -1) {

        var str = url.substr(1);

        var strs = str.split("&");

        for (var i = 0; i < strs.length; i++) {

            theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
            theRequest.count++;
        }

    }

    return theRequest;

};
//表示全局唯一标识符 (GUID)
$.Guid = function (g) {
    /// <summary>表示全局唯一标识符 (GUID)</summary>
    /// <param name="g" type="String">guid字符串</param>
    var arr = new Array(); //存放32位数值的数组


    if (typeof (g) == "string") { //如果构造函数的参数为字符串

        InitByString(arr, g);

    } else {

        InitByOther(arr);

    }

    //返回一个值，该值指示 Guid 的两个实例是否表示同一个值。

    this.Equals = function (o) {

        if (o && o.IsGuid) {

            return this.ToString() == o.ToString();

        } else {

            return false;

        }

    };

    //Guid对象的标记

    this.IsGuid = function () {
    };

    //返回 Guid 类的此实例值的 String 表示形式。

    this.ToString = function (format) {

        if (typeof (format) == "string") {

            if (format == "N" || format == "D" || format == "B" || format == "P") {

                return ToStringWithFormat(arr, format);

            } else {

                return ToStringWithFormat(arr, "D");

            }

        } else {

            return ToStringWithFormat(arr, "D");

        }

    };

    //由字符串加载

    function InitByString(arr, g) {

        g = g.replace(/\{|\(|\)|\}|-/g, "");

        g = g.toLowerCase();

        if (g.length != 32 || g.search(/[^0-9,a-f]/i) != -1) {

            InitByOther(arr);

        } else {

            for (var i = 0; i < g.length; i++) {

                arr.push(g[i]);

            }

        }

    }

    //由其他类型加载

    function InitByOther(arr) {

        var i = 32;

        while (i--) {

            arr.push("0");

        }

    }

    /*

    根据所提供的格式说明符，返回此 Guid 实例值的 String 表示形式。

    N  32 位： xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    D  由连字符分隔的 32 位数字 xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx

    B  括在大括号中、由连字符分隔的 32 位数字：{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}

    P  括在圆括号中、由连字符分隔的 32 位数字：(xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)

    */

    function ToStringWithFormat(arr, format) {

        switch (format) {
            case "N":
                return arr.toString().replace(/,/g, "");
            case "D":
                var str = arr.slice(0, 8) + "-" + arr.slice(8, 12) + "-" + arr.slice(12, 16) + "-" + arr.slice(16, 20) + "-" + arr.slice(20, 32);

                str = str.replace(/,/g, "");

                return str;
            case "B":
                var str = ToStringWithFormat(arr, "D");

                str = "{" + str + "}";

                return str;
            case "P":
                var str = ToStringWithFormat(arr, "D");

                str = "(" + str + ")";

                return str;
            default:
                return new Guid();
        }

    }

};

//Guid 类的默认实例，其值保证均为零。
$.Guid.Empty = $.Guid();

//初始化 Guid 类的一个新实例。
$.Guid.NewGuid = function () {
    /// <summary>初始化 Guid 类的一个新实例</summary>
    var g = "";

    var i = 32;

    while (i--) {

        g += Math.floor(Math.random() * 16.0).toString(16);

    }

    return new $.Guid(g);

};

var funNames = undefined;//角色对于每个页面的功能权限
var funList = {
    AddData: "AddData",  //添加                    
    DeleteData: "DeleteData", //删除
    UpdateData: "UpdateData", //编辑
    SearchData: "SearchData", //查询
    ImportData: "ImportData", //导入
    ExportData: "ExportData", //导出
    PrintData: "PrintData",   //打印
    ShowData: "ShowData",//查看
    DeleteAllData: "DeleteAllData",  //批量删除
    UpdateAllData: "UpdateAllData",  //批量编辑
    ResetPassword: "ResetPassword",  //重置密码
    SignOut:"SignOut"//强制退出
};//系统所有功能
if ($.GetRequest()["funnames"] != undefined) {
    funNames = $.GetRequest()["funnames"];
}
$(function () {
    //    各种初始化

    if ($(".btn-sub").length > 0) {
        $('.btn-sub').button({
            icons: {
                primary: "ui-icon-search"
            }
        });
    }

    if (document.getElementById("fm") != null)
        $("#fm").validationEngine({
            "promptPosition": "centerRight"
        });
    $(".btn-sub").button();
    $(".btn-back").button({
        icons: {
            primary: "ui-icon-arrowreturnthick-1-w"
        },
        text: false
    });
    $(".btn-search").button({
        icons: { primary: "ui-icon-search" }
    });
    //打印
    $(".btn-add").button({
        icons: { primary: "ui-icon-plus" },
        text: false
    });
    //打印
    $(".btn-print").button({
        icons: { primary: "ui-icon-print" }
    });
    //定位
    $(".btn-position").button({
        icons: { primary: "ui-icon-arrowthickstop-1-s" }
    });
    $(".dataview_button").button();

    //文本框初始化
    $("input[type='text'],textarea,input[type='password']").focus(function () {
        $(this).addClass("text-focus");
    });
    $("input[type='text'],textarea,input[type='password']").blur(function () {
        $(this).removeClass("text-focus");
    });
    $("input[type='text'],textarea,input[type='password']").each(function () {
        $(this).addClass("text");
        if (this.className.match("required")) {
            $(this).after("<span class='span_required'>*<span>");
        }
    });
    //日期控件初始化
    $(".Wdate").click(function () {
        WdatePicker();
    });


    $(".tb-edit td:even").css({
        "text-align": "right",
        "width": "60px"
    });
    $(".tb-edit td:odd").css({
        "text-align": "left"
    });


});

//处理功能权限
function DealWithFunction(userFun, funType, funArray, defaultValue) {
    var de = "";
    if (defaultValue != undefined) {
        de = defaultValue;
    }
    if (funNames != undefined) {
        if (typeof (userFun) == "string") {
            if (funNames.indexOf(funType) >= 0) {
                return userFun;
            } else {
                return de;
            }

        } if (typeof (userFun) == "object") {
            if (funNames.indexOf(funType) >= 0) {
                funArray = funArray.concat(userFun);
                return funArray;
            } else if (de != "") {
                funArray = funArray.concat([{ text: de }, '-']);
                return funArray;
            } else {
                return funArray;
            }
        } else {
            return de;
        }
    } else {
        return de;
    }


}

//警告
function AlertWarning(c) {
    $.messager.alert("警告", c, "warning");

}
//提示
function AlertInfo(c) {
    $.messager.alert("提示", c, "info"
    );

}

//错误
function AlertError(c) {
    $.messager.alert("错误", c, "error");

}
//简单提示
function showToastMessage(message) {
    if (message == "" || message == null) {
        message = "操作失败";
    }
    $("body").cftoaster({
        content: message,
        bottomMargin: 300,
        fontColor: '#fff',
        backgroundColor: '#666'
    });
}

//返回页面
function BaseBackPage() {
    var options = parent.getSelectTabOptions();////获取当前tab的属性
    var thisTitle = options.title;
    var lastTitle = options.backTabTitile;
    var lastUrl = options.backTabUrl;
    if (parent.TabExists(lastTitle)) {
        try { parent.tempWin.refresh(); }
        catch (err) {

        }
        self.parent.selectTab(lastTitle, lastUrl);
        self.parent.refreshTab(lastTitle, lastUrl);
    }
    else {
        self.parent.addTab(lastTitle, lastUrl);
    }
    self.parent.closeTab(thisTitle);
    return false;
}

//生成options对象
function BaseOptions(url, pId, column, frozenColumns, toolbar, serachId) {
    var options = {
        idField: pId,
        nowrap: true,
        url: url,
        fit: true,
        singleSelect: true,
        selectOnCheck: false,
        checkOnSelect: false,
        loadMsg: '数据装载中......',
        striped: true,
        useRp: true,
        pageList: [5, 15, 20, 50],
        pagination: true,
        rownumbers: true,
        onSortColumn: function (sortName, sortOrder) {
            options.queryParams.SortName = sortName;
            options.queryParams.SortOrder = sortOrder;
        },
        queryParams: {},
        pageSize: 20,
        frozenColumns: frozenColumns,
        columns: column
    };
    options.queryParams.RP = options.pageSize;
    //操作栏
    if (toolbar) {
        options.toolbar = toolbar;
    }
    if (serachId) {
        options.queryParams.Query = GetQueryText(serachId);
    }
    return options;
}

//加载表
function BaseLoadTable(url, tableId, column, frozenColumns, pId, toolbar, serachId) {
    var options = BaseOptions(url, pId, column, frozenColumns, toolbar, serachId);
    $(tableId).datagrid(options);
    var p = $(tableId).datagrid('getPager');
    if (p) {
        $(p).pagination({
            onChangePageSize: function (value) {
                var queryParams = options.queryParams;
                queryParams.RP = value;
            }
        });
    }
}
//根据Options对象加载表
function BaseLoadTableByOptions(options, tableId) {
    $(tableId).datagrid(options);
    var p = $(tableId).datagrid('getPager');
    if (p) {
        $(p).pagination({
            onChangePageSize: function (value) {
                var queryParams = options.queryParams;
                queryParams.RP = value;
            }
        });
    }
}

//弹出查询框
function flexiQuery(div, title) {
    $(div).window({
        title: title,
        collapsible: false,
        minimizable: false,
        maximizable: false,
        modal: false

    });
}
//查询表
function BaseSearch(serchId, tableId) {
    var queryText = "";
    var rex = /\[\w+\]/;
    $(serchId).find(":input").each(function () {
        if (rex.test(this.name)) {
            if (this.name.indexOf("IsSystem") > 0) {
                queryText += this.name + '&' + this.checked + "^";
            } else {
                queryText += this.name + '&' + this.value + "^";
            }
        }
    });
    var options = $(tableId).datagrid('options');
    options.queryParams.Query = queryText;
    options.pageNumber = 1;
    $(tableId).datagrid(options);
}
//拼接查询字符串
function GetQueryText(serchId) {
    var queryText = "";
    var rex = /\[\w+\]/;
    $(serchId).find(":input").each(function () {
        if (rex.test(this.name)) {
            if (this.name.indexOf("IsSystem") > 0) {
                queryText += this.name + '&' + this.checked + "^";
            } else {
                queryText += this.name + '&' + this.value + "^";
            }
        }
    });
    return queryText;
}

//删除表数据
function DeleteTable(url, tableId) {
    var selection = $(tableId).datagrid('getChecked');
    if (selection.length == 0) {
        AlertWarning("请选择需要删除的数据");
    } else {
        var ids = "";
        for (var i = 0; i < selection.length; i++) {
            ids += selection[i].Id + ',';
        }
        $("#ddText").text(selection.length);
        $("#dd").show().dialog({
            width: 200,
            minHeight: 120,
            modal: true,
            buttons: [{
                text: "删除",
                handler: function () {
                    $.ajax({
                        url: url,
                        type: 'post',
                        data: { id: ids },
                        datatype: 'text',
                        success: function (data) {
                            showToastMessage(data);
                            $("#dd").dialog("close");
                            $(tableId).datagrid('reload');
                            $(tableId).datagrid('uncheckAll');
                        }
                    });
                }
            }, {
                text: "取消",
                handler: function () {
                    $("#dd").dialog("close");
                }
            }]
        });


    }
}

//删除树数据
function DeleteTree(url, id, event) {
    var ids = id;
    $("#ddText").text(1);
    $("#dd").show().dialog({
        width: 200,
        height: 120,
        modal: true,
        buttons: [{
            text: "删除",
            handler: function () {
                $.ajax({
                    url: url,
                    type: 'post',
                    data: { id: ids },
                    datatype: 'text',
                    success: function (data) {
                        showToastMessage(data);
                        $("#dd").dialog("close");
                        event();
                    }
                });
            }
        }, {
            text: "取消",
            handler: function () {
                $("#dd").dialog("close");
            }
        }]
    });

}

//删除tab
function RemoveTabs() {
    var tabs = $("#tabs").tabs('tabs');
    var length = tabs.length;
    for (var i = 1; i < length;) {
        $("#tabs").tabs('close', 1);
        length--;
    }
}
//Json日期转换
$.JsonToDate = function (val) {
    /// <summary>Json日期转换</summary>
    /// <param name="data" type="Object">Json格式的日期</param>
    var returnData = null;
    if (val != null) {
        var date = new Date(parseInt(val.replace("/Date(", "").replace(")/", ""), 10));
        //月份为0-11，所以+1，月份小于10时补个0
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var fullDataString = date.getFullYear() + "-" + month + "-" + currentDate + " " + (date.getHours() < 10 ? "0" + date.getHours() : date.getHours()) + ":" + (date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes()) + ":" + (date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds());
        returnData = new $.JsonDate(date, fullDataString);
    }
    return returnData;
};

//Json
$.JsonDate = function (dateObject, fullDateString) {
    this.dateObject = dateObject;
    this.fullDateString = fullDateString;
};

//比较2个对象是否相等
Objectequals = function (oldObj, obj, isFormat) {
    if (isFormat) {
        var regex = /\s/gm;
        if (typeof (obj) == "string" && typeof (obj) == "string" && (oldObj.replace(regex, "") == obj.replace(regex, "")))
            return true;
    }
    else if (oldObj == obj)
        return true;
    if (typeof (obj) == "undefined" || obj == null || typeof (obj) != "object")
        return false;
    var length = 0; var length1 = 0;
    for (var ele in oldObj) {
        length++;
    }
    for (var ele in obj) {
        length1++;
    }
    if (length != length1)
        return false;
    if (obj.constructor == oldObj.constructor) {
        for (var ele in oldObj) {
            if (typeof (oldObj[ele]) == "object") {
                if (!Objectequals(oldObj[ele], obj[ele]))
                    return false;
            }
            else if (typeof (oldObj[ele]) == "function") {
                if (!Objectequals(oldObj[ele].toString(), obj[ele].toString(), true))
                    return false;
            }
            else if (oldObj[ele] != obj[ele])
                return false;
        }
        return true;
    }
    return false;
};

//在指定位置连接2个数组，并且去除重复
Array.prototype.addItem = function (index, array) {
    var flag = true; //默认不存在
    var oldArray = this;
    $(array).each(function (i, n) {
        $(oldArray).each(function (j, m) {
            if (Objectequals(n, m)) {
                flag = false; //存在此对象
            }
        });
        if (flag) {
            oldArray.splice(6, 0, n);
        }
    });
};

//移除元素
removeItem = function (array, title) {
    $(array).each(function (i, n) {
        if (n.title == title) {
            array.splice(i, 1);
        }
    });

};
//获取鼠标的坐标
function getPoint(e) {
    var x = e.pageX;
    var y = e.pageY;
    return x + ',' + y;
}

//easyui gridview合并列
function MergeCellsByField(tableId, colList) {
    var table = $(tableId); //获取表的列名集合
    if (colList != "" && colList != null) {
        var arrCols = MergeCellSort(colList, table);
        var strIdField = arrCols[0];
        var intRowLength = table.datagrid("getRows").length;//所有行数
        var intRow = 1;//每条数据的跨行数
        var firstMerge = 1;
        arrCols.splice(0, 1);
        for (var i = 0; i < intRowLength; i++) {
            var strNextValue;
            var strThisValue = table.datagrid("getRows")[i][strIdField];//当前行需合并首字段值
            if (i == intRowLength - 1) {
                strNextValue = null; //最后一行数据的下一行数据值为null
            } else {
                strNextValue = table.datagrid("getRows")[i + 1][strIdField];//下一行需合并首字段值
            }
            if (strThisValue == strNextValue) {
                intRow = intRow + 1;//计算数据的跨行数，跨行加1,比如计算到3
            } else {//当前值和下一个值不相等
                if (intRow != 1) { //需合并首字段值向下相等个数
                    firstMerge = intRow;
                    var result = MergeChildCell(arrCols, intRow, firstMerge, table, i);
                    firstMerge = result.MaxMerge;
                    var childCell = result.RequiredCell;
                    table.datagrid('mergeCells', {
                        index: i - intRow + 1,
                        field: strIdField,
                        rowspan: firstMerge,
                        colspan: null
                    });
                    $.each(childCell, function (index, colName) {
                        table.datagrid('mergeCells', {
                            index: i - intRow + 1,
                            field: colName,
                            rowspan: firstMerge,
                            colspan: null
                        });
                    });
                    i = i - (intRow - firstMerge);
                }
                firstMerge = 1;
                intRow = 1;
            }
        }
    }
}

//对colList进行排序,按照gridview中列的排序方式
function MergeCellSort(colList, table) {
    var rex = /^,+|,+$/gi;
    colList = "," + colList.replace(rex, "") + ",";
    var arrCols = new Array();
    var arrtcols = table.datagrid("getColumnFields");
    var arrfcols = table.datagrid("getColumnFields", true);
    arrtcols = arrfcols.join(',') + ',' + arrtcols.join(','); //获取表所有的字段字符串
    arrtcols = arrtcols.replace(rex, "").split(',');
    $.each(arrtcols, function (n, colName) {
        if (colList.indexOf("," + colName + ",") > -1) {
            arrCols.push(colName); //如果字段在colList字符串中存在，则把元素加入arrCols中
        }
    });
    return arrCols;
}

//获得可以合并数据的其他列
function MergeChildCell(arrCols, intRow, firstMerge, table, i) {
    var colSetp = 0;
    var childCell = new Array();//紧紧挨着的带有顺序的列集合
    $.each(arrCols, function (index, colName) {
        if (index == colSetp) {
            var childCount = 1;
            for (var j = i - intRow + 1; j < i - (intRow - firstMerge) ; j++) {
                var strChildThisValue = table.datagrid("getRows")[j][colName];//当前行值
                var strChildNextValue = table.datagrid("getRows")[j + 1][colName];//下一行值
                if (strChildThisValue == strChildNextValue) {
                    childCount++;
                    if (j == i - (intRow - firstMerge) - 1) {
                        colSetp++;
                        childCell.push(colName);
                        firstMerge = childCount;
                        break;
                    }
                } else {
                    if (childCount > 1) {
                        firstMerge = childCount;
                        colSetp++;
                        childCell.push(colName);
                        firstMerge = childCount;
                    } else {
                        for (var k = j + 1; k < i - (intRow - firstMerge) ; k++) {
                            if (table.datagrid("getRows")[k][colName] == table.datagrid("getRows")[k + 1][colName]) {
                                firstMerge = k-j;
                            }
                        }

                    }
                    break;
                }
            }
        }
    });
    return {
        RequiredCell: childCell,
        MaxMerge: firstMerge
    }
}

//easyui扩展
$.extend($.fn.validatebox.defaults.rules, {
    mixLength: {
        validator: function (value, param) {
            return value.length >= param;
        },
        message: '最少{0}个字符'
    }
});
$.extend($.fn.validatebox.defaults.rules, {
    maxLength: {
        validator: function (value, param) {
            return value.length <= param;
        },
        message: '最多{0}个字符'
    }
});
$.extend($.fn.validatebox.defaults.rules, {
    number: {
        validator: function (value, param) {
            var regex = new RegExp(/^[\-\+]?(([0-9]+)([\.,]([0-9]+))?|([\.,]([0-9]+))?)$/);
            return regex.test(value);
        },
        message: '无效的数字'
    }
});
$.extend($.fn.validatebox.defaults.rules, {
    numberLength: {
        validator: function (value, param) {
            return param[0] <= value && value <= param[1];
        },
        message: '数字范围为{0}到{1}'
    }
});
$.extend($.fn.validatebox.defaults.rules, {
    integer: {
        validator: function (value, param) {
            var regex = new RegExp(/^[\-\+]?\d+$/);
            return regex.test(value);
        },
        message: '不是有效的整数'
    }
});

$.extend($.fn.validatebox.defaults.rules, {
    isDouble: {
        validator: function (value, param) {
            var regex = new RegExp(/^[\-\+]?(([0-9]+)([\.,]([0-9]+))?|([\.,]([0-9]+))?)$/);
            var f1 = regex.test(value);
            var s = value.toString();
            var f2 = false;
            if (s.length <= param[0]) {
                f2 = true;
            }
            var f3 = false;
            if ((s.indexOf('.') > 0 && s.substring(s.indexOf('.'), s.length - 1).length <= param[1] + 1) || s.indexOf('.') == -1) {
                f3 = true;
            }
            return f1 && f2 && f3;
        },
        message: '无效的小数'
    }
});

$.extend($.fn.validatebox.defaults.rules, {
    integerLength: {
        validator: function (value, param) {
            return param[0] <= value && value <= param[1];
        },
        message: '整数范围为{0}到{1}'
    }
});

$.extend($.fn.validatebox.defaults.rules, {
    onlyLetterNumber: {
        validator: function (value, param) {
            var regex = new RegExp(/^[0-9a-zA-Z\ \- \*]+$/);
            var f1 = regex.test(value);
            var f2 = false;
            if (value.length >= param[0] && value.length <= param[1])
                f2 = true;
            return f1 && f2;
        },
        message: '例:DPC-16*14,长度范围为{0}到{1}'
    }
});

$.extend($.fn.validatebox.defaults.rules, {
    chinese: {
        validator: function (value, param) {
            var regex = new RegExp(/^[\u4e00-\u9fa5 \，\：\“\”\。\；\……\、]+$/);
            var f1 = regex.test(value);
            var f2 = false;
            if (value.length >= param[0] && value.length <= param[1])
                f2 = true;
            return f1 && f2;
        },
        message: '请输入中文,长度范围为{0}到{1}'
    }
});

$.extend($.fn.validatebox.defaults.rules, {
    english: {
        validator: function (value, param) {
            var regex = new RegExp(/^[\w\'\,\"\:\.\;\ \?]+$/);
            var f1 = regex.test(value);
            var f2 = false;
            if (value.length >= param[0] && value.length <= param[1])
                f2 = true;
            return f1 && f2;
        },
        message: '请输入英文,长度范围为{0}到{1}'
    }
});





(function ($) {
    $.fn.IconList = function (options) {

        var icondiv = '<div class="icondiv">';
        var dmo = this.selector;//获得元素的id
        $(dmo).parent().css({
            position: "relative"
        });
        $(dmo).bind("click", function () {

            var width = $(dmo)[0].clientWidth;//获得元素的宽度
            var height = $(dmo)[0].clientHeight;//获得元素的高度
            var defaults = { height: 300, left: 0, top: 0 };
            var opts = $.extend(defaults, options);
            if ($(".icondiv").length > 0) {//如果已经存在该元素
                $(".icondiv").show();
            } else {
                $.ajax({//ajax加载数据
                    url: opts.url,
                    type: opts.type,
                    dataType: "json",
                    success: function (data) {
                        var iconstring = "";
                        for (var i = 0; i < data.length; i++) {
                            iconstring += '<span class="icondiv_content icon ' + data[i].IconName + '" title="' + data[i].Title + '" iconName="' + data[i].IconName + '"></span>';
                        }
                        icondiv = icondiv + iconstring + "<div style='clear:both'></div></div>";

                        $(dmo).after(icondiv);
                        $(".icondiv").css({
                            left: (opts.left) + "px",
                            top: (opts.top + height) + "px",
                            width: width + "px",
                            minHeight: opts.height + "px"
                        });

                        $(".icondiv_content").bind("click", function () {
                            $(dmo).val($(this).attr("iconName"));

                        });
                        $(".icondiv").bind("mouseleave", function () {
                            $(".icondiv").hide();
                        });
                    }
                });
            }



        });



    };
})(jQuery)