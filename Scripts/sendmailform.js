//Код для работы кнопки НАПИСАТЬ НАМ
$(document).ready(function() {

    $('#btn_Writeus').on('click', function() {
        formOpen();
        return false;
    });

    var $btnClose;
    var $tbl;
    var operationCheck = false;

    function FormRequest() {
        this.fam = "";      //
        this.name = "";     //*
        this.tel = "";      //
        this.mail = "";     //*
        this.text = "";     //*
    }
    var formReq = new FormRequest();

    function formOpen() {

        var htmlString = "<table id='formOpacTbl'><tr><td>";
        htmlString += "<div id='formDiv'>";
        htmlString += "<img id='btnFormClose' src='../../../images/btnClose.png' alt='Close' />";
        htmlString += "<div id='divForm_Title' class='oneLineDiv divForm_Title'>Форма обращения</div>";
        htmlString += "<div id='divErr' class='oneLineDiv divError'></div>";
        htmlString += "<div class='oneLineDiv'>";
        htmlString += "<label>&nbsp;&nbsp;</label><input id='formFam' type='text' placeholder='Ваша фамилия (пример: Ситников)' name='formFam' class='txtBoxFormUniv placeHolderForm' />";
        htmlString += "</div>";
        htmlString += "<div class='oneLineDiv'>";
        htmlString += "<label>*</label><input id='formName' type='text' placeholder='Ваше имя (пример: Андрей)' name='formName' class='txtBoxFormUniv placeHolderForm' />";
        htmlString += "</div>";
        htmlString += "<div class='oneLineDiv'>";
        htmlString += "<label>&nbsp;&nbsp;</label><input id='formTel' type='text' placeholder='Ваш телефон (пример: 89169986134)' name='formTel' class='txtBoxFormUniv placeHolderForm' />";
        htmlString += "</div>";
        htmlString += "<div class='oneLineDiv'>";
        htmlString += "<label>*</label><input id='formMail' type='text' placeholder='Ваша почта (пример: a.sitnikov@yandex.ru)' name='formTel' class='txtBoxFormUniv placeHolderForm' />";
        htmlString += "</div>";
        htmlString += "<div class='oneLineDiv'>";
        htmlString += "<label>*</label><textarea id='formText' placeholder='ТЕКСТ ОБРАЩЕНИЯ\nВНИМАНИЕ! Если обращение связано с Конкурсом, то обязательно укажите здесь название этого Конкурса (и Номинацию).' name='formText' class='txtBoxFormUniv placeHolderForm' cols='25' rows='5' />";
        htmlString += "</div>";
        htmlString += "<div class='oneLineDiv'>";
        htmlString += "<span id='sendmail'>ОТПРАВИТЬ ОБРАЩЕНИЕ</span>";
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

        $('#sendmail').bind('click', function () {

            if (operationCheck) { return; }

            formReq.fam = $('#formFam').val();
            formReq.name = $('#formName').val(); formReq.mail = $('#formMail').val();
            formReq.tel = $('#formTel').val(); formReq.text = $('#formText').val();
            var $err = $('#divErr'); $err.text('');

            if (formReq.fam.indexOf('<') != -1 || formReq.fam.indexOf('>') != -1) { $err.text('Фамилия содержит недопустимые символы'); return; }
            else if (formReq.name.length > 30) { $err.text('Вы ввели слишком длинную фамилию'); return; }

            if (formReq.name.trim() == "") { $err.text('Введите своё имя'); return; }
            else if (formReq.name.indexOf('<') != -1 || formReq.name.indexOf('>') != -1) { $err.text('Имя содержит недопустимые символы'); return; }
            else if (formReq.name.length > 30) { $err.text('Вы ввели слишком длинное имя'); return; }

            if (formReq.mail.trim() == "") { $err.text('Введите адрес почты для связи с Вами'); return; }
            else if (!(/^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}$/i).test(formReq.mail)) { $err.text('Адрес почты введён неверно..'); return; }

            if (formReq.tel.indexOf('<') != -1 || formReq.tel.indexOf('>') != -1) { $err.text('Номер телефона содержит недопустимые символы'); return; }
            else if (formReq.tel.length > 20) { $err.text('Слишком длинный номер телефона'); return; }

            if (formReq.text.trim() == "") { $err.text('Введите текст обращения'); return; }
            else if (formReq.text.indexOf('<') != -1 || formReq.text.indexOf('>') != -1) { $err.text('Текст содержит недопустимые символы'); return; }
            else if (formReq.text.length > 666) { $err.text('Вы ввели слишком длинный текст'); return; }

            operationCheck = true;

            sendForm(formReq);
        });
    }

    function opacClose() {
        $btnClose.unbind('click');
        $tbl.animate({
            opacity: 0
        }, 200, function () {
            $tbl.remove();
        });
    };

    function sendForm(formReq) {

        //делаем невидимыми элементы в форме запроса
        $('.oneLineDiv').css('opacity', '0');
        //запускает анимированное окно ожидания
        $('#formDiv').append("<img id='waitImg' src='../../../images/wait.gif' alt='Подождите..' />");

        function getCallback(result, eventArgs) {
            if (result === 'ok') {
                $('#divErr').text('');
                $('#waitImg').remove();
                //появление изображения и текста, показывающие успешную отсылку запроса
                var htmlString = "<div id='divSuccess'>";
                htmlString += "<img src='../../../images/galochka.png' alt='Успешно' /><br/><br/>";
                htmlString += "<span>Ваше обращение принято. Мы свяжемся с Вами в ближайшее время.</span>";
                htmlString += "</div>";
                $('#formDiv').append(htmlString);
            }
            else if (result === 'err') {
                $('#waitImg').remove();
                $('.oneLineDiv').css('opacity', '1');
                $('#divErr').text('Не удалось отправить ваше обращение..');
            }
            operationCheck = false;
        }
        function onError(result) {
            $('#waitImg').remove();
            $('.oneLineDiv').css('opacity', '1');
            $('#divErr').text('Не удалось отправить ваше обращение..');

            operationCheck = false;
        }
        site.DataService.SentRequest(formReq, getCallback, onError);
    };
});