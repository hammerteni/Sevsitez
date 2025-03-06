//функция замены стандартного отображения статус бара браузера при наведении на кнопки перехода между страницами
/*function replace(elem) {
window.status = "Перейти на страницу: " + elem.text;
}*/

//универсальная фукнция, которая проверяет содержимое текстбокса на пустое значение.
//Если в текстбокс ничего не введено(или только пробелы), то функция заполняет лэйбу соответствующим
//предупреждением и возвращает false. В функцию передаются id тексбокса и id лэйбы для вывода предупреждения
function CheckEmpty(txtBoxId, lblErrId) {
    var commTxt = document.getElementById(txtBoxId).value;
    var errLbl = document.getElementById(lblErrId);
    if (commTxt.trim() === "") {
        errLbl.innerText = "* текст поля не должен быть пустым..";
        return false;
    }
    else { return true; }
};

//код для закрытия Предупреждением
function warnVisible() {
    var obj = $('#divWarnWrapper');
    obj.animate({ top: -35 + 'px' }, 500, function () { obj.remove(); });
    return false;
}

//код сокрытия элемента с id 
function windowVisible(elemId) {
    var imgbtnwarn = document.getElementById(elemId);
    imgbtnwarn.style.display = 'none';
}

//код для защиты картинок от копирования способом перетаскивания мышкой
//onmousedown="return false;"
$(document).ready(function () {
    $('img').bind('mousedown', function () {
        return false;
    });
    $("input[type='image']").bind('mousedown', function () {
        return false;
    });
});

//код для сворачивания/разворачивания блока со счётчиками
function hideLeftBlock(hideDivId, btnId) {
    var position = $(hideDivId).offset().left;
    if (position >= 0) {       //если надо скрыть
        $(hideDivId).animate({
            'left': '-=95px'
        }, 300, function () {
            $(btnId).css({ 'background-image': 'url(\'../../../images/arrshow.png\')' });
            $(btnId).attr('title','показать');
        });
    }
    else if (position < 0) {  //если надо показать
        $(hideDivId).animate({
            'left': '+=95px'
        }, 300, function () {
            $(btnId).css({ 'background-image': 'url(\'../../../images/arr.png\')' });
            $(btnId).attr('title', 'скрыть');
        });
    }
};

//Функция проверки нажатости кнопки отправки формы (предотвращает множественные клики)
var isFormBtnClicked = false
function formBtnClick() {
    if (isFormBtnClicked) {
        return false
    }
    else {
        isFormBtnClicked = true;
        return true
    }
}

//Функция проверки нажатости кнопки скачивания файла (предотвращает множественные клики)
var isPageReady = false
function pageReady() {
    if (document.readyState === "complete") {
        isPageReady = true;
        return true;
    }
    else {
        return false;
    }
}

function handleEnter(obj, event) {
    var keyCode = event.keyCode ? event.keyCode : event.which ? event.which : event.charCode;
    if (keyCode == 13) {
        document.getElementById(obj).click();
        return false;
    }
    else {
        return true;
    }
} 

function asyncLoadingInmages(img) {
    function onSuccess(result, eventArgs) {
        img.src = result.replace("../", "/") + "?dt=" + Date.now().toString();
        img.removeAttribute('data-src');
        setTimeout(function () { img.style.width = "785px"; }, 100);
    }
    function onError(result) {
        alert('Не удалось отобразить файл с картинкой. Попробуйте повторить или сообщите в техподдержку.');
    }
    site.DataService.ImgLazyLoad(img.dataset.src, onSuccess, onError)
}

//код для записи полного URL в href ссылок
$(function() {
    var alllinks = document.links;
    for (var i = 0; i < alllinks.length; i++) {
        alllinks[i].href = alllinks[i].href;
    }
});

//Подгрузка картинок
var wait_Img, img_Close, img_Mag_nullfoto, img_People_nullfoto, img_Refresh, img_Success, img_Remark;

$(function () {
    wait_Img = new Image();
    wait_Img.src = "../../../images/waiting.gif";

    (async () => {
        //if ('loading' in HTMLImageElement.prototype) {
        const images = document.querySelectorAll("img.lazyload");
        images.forEach(img => {
            setTimeout(function(){ asyncLoadingInmages(img) }, 100);
        });
        //} else {
        //    console.log("3")
        //    // Динамически импортируем библиотеку LazySizes
        //    const lazySizesLib = await import('/lazysizes.min.js');
        //    // Инициализируем LazySizes (читаем data-src & class=lazyload)
        //    lazySizes.init(); // lazySizes применяется при обработке изображений, находящихся на странице.
        //}
    })();

    
});