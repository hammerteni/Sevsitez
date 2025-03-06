//Код изменения размеров блоков 'div.def_content' в зависимости от высоты окна-------------------
var divHeight = 0;
var deltaHeight = 314;
$(document).ready(function () {
    saveOriginDivHeight1();
    setDivHeight1();
});
$(window).on('resize', function () {
    setDivHeight1();
});
//функция сохранения оригинальных размеров блоков в массив
function saveOriginDivHeight1() {
    divHeight = $('div#contentDiv').outerHeight(true);
}
//функция присвоения блокам на странице default.aspx высоты окна
function setDivHeight1() {
    var windowHeight = $(window).height() - deltaHeight;
    var $obj = $('div#contentDiv');

    if (divHeight < windowHeight) {
        $obj.height(windowHeight);
    } else {
        $obj.height(divHeight);
    }
}
//-----------------------------------------------------------------------------------------------