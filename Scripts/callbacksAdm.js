//Для панели редактирования отзывов

function addNewGuestCallback() {
    formOpen('посетитель','',false);
}

function addNewModerCallback() {
    formOpen('администратор','',false);
}

function replyToCallback(callbackId) {
    formOpen('администратор',callbackId,false);
}

//-------------------------------------------------------

var $btnClose;
var $tbl;
var operationCheck = false;

function formOpen(role, callbackId, _public) {

    var htmlString = "<table id='formOpacTbl'><tr><td>";
    htmlString += "<div id='formDiv'>";
    htmlString += "<img id='btnFormClose' src='../../../images/btnClose.png' alt='Close' />";
    htmlString += "<div id='divErr' class='oneLineDiv divError'></div>";
    if (role === 'посетитель') {
        htmlString += "<div class='oneLineDiv'>";
        htmlString += "<label>*</label><input id='formName' type='text' placeholder='Имя' name='formName' class='txtBoxFormUniv placeHolderForm' />";
        htmlString += "</div>";
    }
    htmlString += "<div class='oneLineDiv'>";
    htmlString += "<label>*</label><textarea id='formText' placeholder='Текст отзыва или комментария' name='formText' class='txtBoxFormUniv placeHolderForm' cols='25' rows='5' />";
    htmlString += "</div>";
    htmlString += "<div class='oneLineDiv'>";
    htmlString += "<span id='addCallback'>ДОБАВИТЬ</span>";
    htmlString += "</div>";
    htmlString += "</div>";
    htmlString += "</td></tr></table>";
    $('body').prepend(htmlString);

    $tbl = $('#formOpacTbl');
    $tbl.css('opacity', '0');

    $btnClose = $('#btnFormClose');
    $btnClose.hide();

    $tbl.animate({
        opacity: 1
    }, 200);

    var $formDiv = $('#formDiv');
    $btnClose.show().offset({ top: $formDiv.offset().top - 15, left: $formDiv.offset().left + $formDiv.innerWidth() - 15 });

    $btnClose.bind('click', function () {
        opacClose();
    });

    $('#addCallback').bind('click', function () {
        clickFunction();
    });

    function clickFunction() {
        if (operationCheck) { return; }

        var name = '';
        if (role === 'посетитель')
            name = $('#formName').val();
        var text = $('#formText').val();
        var $err = $('#divErr'); $err.text('');

        if (role === 'посетитель') {
            if (name.trim() == "") { $err.text('Введите своё имя'); return; }
            else if (name.indexOf('<') != -1 || name.indexOf('>') != -1) { $err.text('Имя содержит недопустимые символы'); return; }
            else if (name.length > 30) { $err.text('Вы ввели слишком длинное имя'); return; }
        }

        if (text.trim() == "") { $err.text('Введите текст обращения'); return; }
        else if (text.indexOf('<') != -1 || text.indexOf('>') != -1) { $err.text('Текст содержит недопустимые символы'); return; }
        else if (text.length > 2000) { $err.text('Вы ввели слишком длинный текст'); return; }

        operationCheck = true;
        addCallback(name, text, role, callbackId, _public);
    }
}

function opacClose() {
    $btnClose.unbind('click');
    $tbl.animate({
        opacity: 0
    }, 200, function () {
        $tbl.remove();
        location.reload(true);  //перезагрузка текущей страницы
    });
};

function addCallback(name, text, role, callbackId, _public) {

    $('.oneLineDiv').css('opacity', '0');
    $('#formDiv').append("<img id='waitImg' src='../../../images/wait.gif' alt='Подождите..' />");

    function onSuccess(result, eventArgs) {
        if (result === 'ok') {
            $('#divErr').text('');
            $('#waitImg').remove();

            var htmlString = "<div id='divSuccess'>";
            htmlString += "<img src='../../../images/galochka.png' alt='Успешно' /><br/><br/>";
            htmlString += "<span>Отзыв добавлен.</span>";
            htmlString += "</div>";
            $('#formDiv').append(htmlString);
        } else if (result === 'err') {
            $('#waitImg').remove();
            $('.oneLineDiv').css('opacity', '1');
            $('#divErr').text('Отзыв не добавлен. Повторите или сообщите администратору.');
        }
        operationCheck = false;
    }

    function onError(result) {
        $('#waitImg').remove();
        $('.oneLineDiv').css('opacity', '1');
        $('#divErr').text('Отзыв не добавлен (onerror). Повторите или сообщите администратору.');

        operationCheck = false;
    }

    var c = document.getElementById('callbackcode');
    site.DataService.SaveCallbackAdm(role, callbackId, name, _public, text, c.childNodes[0].nodeValue, onSuccess, onError);
}