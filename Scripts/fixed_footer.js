//Фиксация подвала внизу страницы

var $footer;
var windowWidth_f;
var windowHeight_f;

$(document).ready(function () {
    $footer = $('div#footerDiv');
    setFooter();
});
$(window).on('resize', function () {
    setFooter();
});
$(window).on('scroll', function () {
    setFooter();
});

function setFooter() {

    if ($footer) {

        windowHeight_f = $(window).outerHeight(true) + $(window).scrollTop();
        windowWidth_f = $(window).outerWidth(true);
        $footer.css({ top: (windowHeight_f - 40) + 'px', left: (windowWidth_f - $footer.width()) / 2 + 'px' });

    }

}