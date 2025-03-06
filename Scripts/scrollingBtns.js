//код для "кнопок" меню, которые управляют скроллингом страницы
function scrollBtnClick(objThis) {
    $("html,body").animate({ scrollTop: $("#" + objThis.id + "_h2").offset().top - 112 }, 500);
    $(".navigationDiv a").each(function() {
        $(this).attr('class', 'lBtnsInactive');
    });
    $(objThis).attr('class', 'lBtnsNavActive');

    return false;
}


//Код изменения размеров блоков 'div.def_content' в зависимости от высоты окна-------------------
var divHeightsArray = [];
$(function() {
    saveOriginDivHeights();
    setDivHeights();
});
$(window).on('resize', function() {
    setDivHeights();
});
//функция сохранения оригинальных размеров блоков в массив
function saveOriginDivHeights() {
    $('div.def_content').each(function () {
        divHeightsArray.push($(this).outerHeight());
    });
}
//функция присвоения блокам на странице default.aspx высоты окна
function setDivHeights() {
    var windowHeight = $(window).height() - 100;
    $('div.def_content').each(function (index) {
        var $obj = $(this);
        if (divHeightsArray[index] < windowHeight) {
            $obj.height(windowHeight);
        } else {
            $obj.height(divHeightsArray[index]);
        }
    });
}
//-----------------------------------------------------------------------------------------------