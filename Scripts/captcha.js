// captcha

var captcha_operationCheck = false;
var $tblCaptcha, $tblCaptchaClose;
var ctrlStr, captcha_backfunc;

function captchaStart(backfunc) {

    captcha_backfunc = backfunc;

    function getCallback(result, eventArgs) {
        ctrlStr = getCtrlStr(result);
        captchaOpen();
    }
    function onError(result) {
        //
    }

    site.DataService.CaptchaGet(getCallback, onError);
}

function reFreshCaptcha(captTxtElemId) {

    captTxtElemId = '#' + captTxtElemId;
    $elem = $(captTxtElemId);

    function getCallback(result, eventArgs) {
        ctrlStr = getCtrlStr(result);
        $elem.text('');
        $elem.text(ctrlStr);
    }
    function onError(result) {
        //
    }

    site.DataService.CaptchaGet(getCallback, onError);

}

function getCtrlStr(cptch) {

    return 'Если вы не робот, ответьте на контрольный вопрос: Сколько будет ' + cptch + '?';

}

function captchaOpen() {

    var htmlString = "<table id='formOpacTbl_captcha'><tr><td>";

    htmlString += "<div id='formDiv_captcha'>";

    htmlString += "<img id='btnFormClose_captcha' src='../../../images/btnClose.png' alt='Закрыть' />";
    htmlString += "<div class='oneLineDiv_captcha divForm_Title_captcha'>ОТВЕТЬТЕ НА ВОПРОС</div>";
    htmlString += "<div id='divErr_captcha' class='oneLineDiv_captcha divError_captcha'></div>";

    htmlString += "<div class='oneLineDiv_captcha'>";
    htmlString += "<span id='captTxt_captcha' class='captTxt_captcha'>" + ctrlStr + "</span><img id='reCaptcha' class='reCaptcha' src='../../../images/refresh.png' alt='обновить' title='Обновить контрольный вопрос' />";
    htmlString += "<input id='formBot' type='text' placeholder='ответ введите цифрой' name='formBot' class='txtBoxFormUniv_captcha placeHolderForm_captcha' />";
    htmlString += "</div>";

    htmlString += "<div class='oneLineDiv_captcha'>";
    htmlString += "<span id='btnSendCaptcha' class='btnSend_captcha'>ПРОГОЛОСОВАТЬ</span>";
    htmlString += "</div>";

    htmlString += "</div>";

    htmlString += "</td></tr></table>";
    $('body').prepend(htmlString);

    $tblCaptcha = $('#formOpacTbl_captcha');
    $tblCaptcha.css('opacity', '0');

    $tblCaptchaClose = $('#btnFormClose_captcha');
    $tblCaptchaClose.hide();

    $tblCaptcha.animate({
        opacity: 1
    }, 200);

    var $formDiv = $('#formDiv_captcha');
    $tblCaptchaClose.show().offset({ top: $formDiv.offset().top - 15, left: $formDiv.offset().left + $formDiv.innerWidth() - 15 });

    $tblCaptchaClose.bind('click', function () {
        captchaClose();
    });

    $('#btnSendCaptcha').on('click', function (e) {

        if (captcha_operationCheck) { return; }
        var bot = $('#formBot').val();

        captcha_operationCheck = true;

        captchaCheck(bot);

    });

    $('#reCaptcha').on('click', function (e) {

        reFreshCaptcha('captTxt_captcha');

    });

}
function captchaCheck(bot) {

    $('.oneLineDiv_captcha').css('opacity', '0');
    $('#formDiv_captcha').append("<img id='waitImg_captcha' src='../../../images/waiting.gif' alt='Подождите..' />");

    function getCallback(result, eventArgs) {
        if (result[0] === 'ok') {
            $('#waitImg_captcha').remove();
            $('#divErr_captcha').text('').focus();
            $('#btnSendCaptcha').unbind('click');
            $('#reCaptcha').unbind('click');
            
            captcha_backfunc(result[1]);
            captchaClose();
        }
        else {
            $('#waitImg_captcha').remove();
            $('.oneLineDiv_captcha').css('opacity', '1');

            if (result[0] === 'timeout') {
                $('#divErr_captcha').text('контрольный вопрос просрочен.. обновите вопрос');
            }
            else if (result[0] === 'err') {
                $('#divErr_captcha').text('неправильный ответ на контрольный вопрос');
                reFreshCaptcha('captTxt_captcha');
            }
            else if (result[0] === 'err_ab') {
                $('#divErr_captcha').text('ты не человек..');
                reFreshCaptcha('captTxt_captcha');
            }
        }
        captcha_operationCheck = false;
    }
    function onError(result) {
        $('#waitImg_captcha').remove();
        $('.oneLineDiv_captcha').css('opacity', '1');
        $('#divErr_captcha').text('Техническая ошибка при входе. Сообщите нам об этом..');

        captcha_operationCheck = false;
    }
    site.DataService.CaptchaCheck(bot, getCallback, onError);

}
function captchaClose() {

    captcha_operationCheck = false;
    $tblCaptchaClose.unbind('click');
    $tblCaptcha.animate({
        opacity: 0
    }, 200, function () {
        $tblCaptcha.remove();
    });

}