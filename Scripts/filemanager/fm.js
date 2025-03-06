/* Простой файловый менеджер. Умеет читать файлы из одной директории, просматривать их и удалять.
Требуются подключённые библиотеки JQuery*/

var operationCheck = false;
var objBtn_fm = {};
var pathToScrDir_fm = "";
var pathToFlsDir_fm = "";
var objArr = [];
var c = "325kjghksfdjgher9874354idshfdgkjsht348576sjefkhdsjkg";
var listView_fm = "tile";   //tile, list
var $divFmContentWrapper = {};
var $divFmWrapper = {};
var $btnFmExit, $btnFmDel, $btnFmListView, $btnFmTileView;
var $lblNameFm, $lblHrefFm, $lblFsizeFm, $lblDateFm;
var IsFmOpened = false;
var $activeFObj = null;

function fmStart(objBtn, pathToScrDir, pathToFlsDir) {

    if (operationCheck) {
        alert('Дождитесь открытия файлового менеджера');
        return;
    }
    if (IsFmOpened) {
        alert('Можно открыть только одно такое окно');
        return;
    }

    IsFmOpened = true;
    operationCheck = true;
    objBtn_fm = objBtn;
    pathToScrDir_fm = pathToScrDir;
    pathToFlsDir_fm = pathToFlsDir;

    fmGetData();
}
function fmGetData() {

    function onSuccess(result, eventArgs) {
        if (result[0].FName === 'error') {
            alert('Произошла ошибка во время получения данных. Попробуйте повторить или обратитесь к разработчику.');
            operationCheck = false;
            return;
        }
        else if (result[0].FName === 'null') {
            objArr = {};
        } else {
            objArr = result;
        }
        
        fmOpen();

        operationCheck = false;
    }
    function onError(result) {
        alert('Произошла техническая ошибка во время получения данных. Попробуйте повторить или обратитесь к разработчику.');
        operationCheck = false;
    }
    site.DataService.GetFiles(pathToFlsDir_fm, c, onSuccess, onError);
}
function fmOpen() {

    var htmlString = "<div id='divFmWrapper'>";

    htmlString += "<div id='divFmOptionsWrapper'>";

    htmlString += "<span id='btnFmDel' title='Удалить выбранный файл'></span>";
    htmlString += "<span id='btnFmListView' title='Отображать списком'></span>";
    htmlString += "<span id='btnFmTileView' title='Отображать плиткой'></span>";
    htmlString += "<span id='btnFmExit' title='Закрыть это окно'></span>";

    htmlString += "</div>";

    htmlString += "<div id='divFmContentWrapper'>";

    // заполняется функцией fillFileContainer()

    htmlString += "</div>";

    htmlString += "<div id='divFmInfoWrapper'>";

    htmlString += "<div><span class='fmTxtPre'>Имя файла:</span>";
    htmlString += "<span class='fmTxtInfo' id='lblNameFm'></span></div>";
    htmlString += "<div><span class='fmTxtPre'>Ссылка:</span>";
    htmlString += "<span class='fmTxtInfo' id='lblHrefFm'></span></div>";
    htmlString += "<div><span class='fmTxtPre'>Размер(kB):</span>";
    htmlString += "<span class='fmTxtInfo' id='lblFsizeFm'></span></div>";
    htmlString += "<div><span class='fmTxtPre'>Дата:</span>";
    htmlString += "<span class='fmTxtInfo' id='lblDateFm'></span></div>";

    htmlString += "</div>";
    htmlString += "</div>";
    $('body').prepend(htmlString);

    fillFileContainer();

    $divFmWrapper = $('#divFmWrapper');
    $divFmWrapper.css('opacity', '0');
    var $objBtnFm = $(objBtn_fm);
    $divFmWrapper.offset({ top: $objBtnFm.offset().top, left: $objBtnFm.offset().left + 50 });

    $btnFmExit = $('#btnFmExit');
    $btnFmDel = $('#btnFmDel');
    $btnFmListView = $('#btnFmListView');
    $btnFmTileView = $('#btnFmTileView');

    $btnFmExit.bind('click', function () {
        fmClose();
    });

    $btnFmDel.bind('click', function () {
        fmFileDel();
    });

    $btnFmListView.bind('click', function () {
        fmListView();
    });

    $btnFmTileView.bind('click', function () {
        fmTileView();
    });

    if (listView_fm === 'list') {
        $btnFmListView.css('backgroundColor', '#b2c6ef');
    }
    else if (listView_fm === 'tile') {
        $btnFmTileView.css('backgroundColor', '#b2c6ef');
    }

    $divFmWrapper.animate({
        opacity: 1
    }, 500);

    $lblNameFm = $('#lblNameFm');
    $lblHrefFm = $('#lblHrefFm');
    $lblFsizeFm = $('#lblFsizeFm');
    $lblDateFm = $('#lblDateFm');

    $divFmWrapper.draggable({
        containment: "parent",
        handle: '#divFmOptionsWrapper'
    });
}
function fillFileContainer() {

    $divFmContentWrapper = $('#divFmContentWrapper');
    var htmlString = "", fName = "";
    for (var i = 0; i < objArr.length; i++) {

        if (listView_fm === 'tile') {

            htmlString += "<div class='divFileTileFm' id='divFileFmWrapper" + i +
                        "' data-name='" + objArr[i].FName + "' data-lname='" + objArr[i].FLName + "' data-size='" + objArr[i].FSize + "'" +
                        "data-date='" + objArr[i].FDate + "' data-type='" + objArr[i].FType + "' " +
                        "onmouseover='fileDivOver(this);' ondblclick='fileDivDblClick(this);' " +
                        "onmouseout='fileDivMouseOut(this);' onclick='fileDivClick(this);'>";
            if (objArr[i].FType === 'pdf') {
                htmlString += "<div class='fileIconWrapper_fm'><img class='fileIcon_fm' src='.." + pathToScrDir_fm + "filemanager/img/pdf.png'/></div>";
            }
            else if (objArr[i].FType === 'txt') {
                htmlString += "<div class='fileIconWrapper_fm'><img class='fileIcon_fm' src='.." + pathToScrDir_fm + "filemanager/img/txt.png'/></div>";
            }
            else if (objArr[i].FType === 'arch') {
                htmlString += "<div class='fileIconWrapper_fm'><img class='fileIcon_fm' src='.." + pathToScrDir_fm + "filemanager/img/arch.png'/></div>";
            }
            else if (objArr[i].FType === 'img') {
                htmlString += "<div class='fileIconWrapper_fm'><img class='fileIcon_fm' src='.." + pathToFlsDir_fm + objArr[i].FLName + "'/></div>";
            }
            else if (objArr[i].FType === 'office') {
                htmlString += "<div class='fileIconWrapper_fm'><img class='fileIcon_fm' src='.." + pathToScrDir_fm + "filemanager/img/office.png'/></div>";
            }
            else if (objArr[i].FType === '?') {
                htmlString += "<div class='fileIconWrapper_fm'><img class='fileIcon_fm' src='.." + pathToScrDir_fm + "filemanager/img/another.png'/></div>";
            }

            if (objArr[i].FName.length > 10) {
                fName = objArr[i].FName.substring(0, 10) + '..';
            } else {
                fName = objArr[i].FName;
            }
            htmlString += "<span class='fName_fm'>" + fName + "</span>";
            htmlString += "</div>";

        }
        else if (listView_fm === 'list') {

            htmlString += "<div class='divFileListFm' id='divFileFmWrapper" + i +
                        "' data-name='" + objArr[i].FName + "' data-lname='" + objArr[i].FLName + "' data-size='" + objArr[i].FSize + "'" +
                        "data-date='" + objArr[i].FDate + "' data-type='" + objArr[i].FType + "' " +
                        "onmouseover='fileDivOver(this);' ondblclick='fileDivDblClick(this);' " +
                        "onmouseout='fileDivMouseOut(this);' onclick='fileDivClick(this);'>";
            if (objArr[i].FType === 'pdf') {
                htmlString += "<div class='fileIconWrapper_fm'><img class='fileIconList_fm' src='.." + pathToScrDir_fm + "filemanager/img/pdf.png'/></div>";
            }
            else if (objArr[i].FType === 'txt') {
                htmlString += "<div class='fileIconWrapper_fm'><img class='fileIconList_fm' src='.." + pathToScrDir_fm + "filemanager/img/txt.png'/></div>";
            }
            else if (objArr[i].FType === 'arch') {
                htmlString += "<div class='fileIconWrapper_fm'><img class='fileIconList_fm' src='.." + pathToScrDir_fm + "filemanager/img/arch.png'/></div>";
            }
            else if (objArr[i].FType === 'img') {
                htmlString += "<div class='fileIconWrapper_fm'><img class='fileIconList_fm' src='.." + pathToFlsDir_fm + objArr[i].FLName + "'/></div>";
            }
            else if (objArr[i].FType === 'office') {
                htmlString += "<div class='fileIconWrapper_fm'><img class='fileIconList_fm' src='.." + pathToScrDir_fm + "filemanager/img/office.png'/></div>";
            }
            else if (objArr[i].FType === '?') {
                htmlString += "<div class='fileIconWrapper_fm'><img class='fileIconList_fm' src='.." + pathToScrDir_fm + "filemanager/img/another.png'/></div>";
            }

            htmlString += "<span class='fNameList_fm'>" + objArr[i].FName + "</span>";
            htmlString += "<span class='fSizeList_fm'>" + objArr[i].FSize + " kb</span>";
            htmlString += "<span class='fDateList_fm'>" + objArr[i].FDate + "</span>";
            htmlString += "</div>";

        }
    }
    $divFmContentWrapper.html(htmlString);

}
function deleteFile($obj) {

    if (!operationCheck) {

        operationCheck = true;
        var fName = $obj.data('name');
        var flName = $obj.data('lname');

        function onSuccess(result, eventArgs) {
            if (result === 'error') {
                alert('Произошла ошибка во время удаления файла. Попробуйте повторить или обратитесь к разработчику.');
                operationCheck = false;
                return;
            }

            $obj.remove();
            var index = -1;
            for (var i = 0; i < objArr.length; i++) {
                if (objArr[i].FName === fName) {
                    index = i; break;
                }
            }
            if (index !== -1) {
                objArr.splice(index, 1);
            }
            operationCheck = false;
        }
        function onError(result) {
            alert('Произошла техническая ошибка во время удаления файла. Попробуйте повторить или обратитесь к разработчику.');
            operationCheck = false;
        }
        site.DataService.DelFile(pathToFlsDir_fm, fName, flName, c, onSuccess, onError);

    }
}

function fileDivOver(obj) {
    //
}
function fileDivClick(obj) {

    var $obj = $(obj);
    $activeFObj = $(obj);

    $lblNameFm.text($obj.data('name'));
    $lblHrefFm.text(pathToFlsDir_fm + $obj.data('name'));
    $lblFsizeFm.text($obj.data('size'));
    $lblDateFm.text($obj.data('date'));

    if (listView_fm === 'list') {
        $('.divFileListFm').each(function () {
            $(this).css('backgroundColor', '');
            $(this).css('border', '1px transparent solid');
        });
    }
    else if (listView_fm === 'tile') {
        $('.divFileTileFm').each(function () {
            $(this).css('backgroundColor', '');
            $(this).css('border', '1px transparent solid');
        });
    }

    $(obj).css('backgroundColor', '#9de4fc');
    $(obj).css('border', '1px #65afc8 solid');

}
function fileDivDblClick(obj) {
    var href = pathToFlsDir_fm + $(obj).data('name');
    window.open(href, '_blank');
}
function fileDivMouseOut(obj) {
    //
}

function fmClose() {

    $divFmWrapper.animate({
        opacity: 0
    }, 200, function () {
        $(this).remove();
        IsFmOpened = false;
        $activeFObj = null;
    });

}
function fmFileDel() {

    if ($activeFObj) {
        if (confirm('Будет удалён файл - ' + $activeFObj.data('name') + ' Продолжить?')) {
            deleteFile($activeFObj);
        }
    }
    else {
        alert('Сначала выделите файл, который нужно удалить..');
    }

}
function fmListView() {

    listView_fm = "list";
    $divFmContentWrapper.empty();
    fillFileContainer();

    $btnFmListView.css('backgroundColor', '#b2c6ef');
    $btnFmTileView.css('backgroundColor', '');

    $activeFObj = null;

}
function fmTileView() {

    listView_fm = "tile";
    $divFmContentWrapper.empty();
    fillFileContainer();

    $btnFmListView.css('backgroundColor', '');
    $btnFmTileView.css('backgroundColor', '#b2c6ef');

    $activeFObj = null;

}
