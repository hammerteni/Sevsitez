//код для установки ширины блока меню (для выставления его посредине)
$(function () {
    var maxMenuWidth = 1200;
    var $navBlock = $('.navBtnsWrap');
    var $navBtnsArr = $('.navBtnsWrap a');
    var resultWidth = 0;
    $navBtnsArr.each(function (i) {
        resultWidth += $(this).outerWidth(true);
    });
    //console.log(resultWidth);
    if (resultWidth >= maxMenuWidth) resultWidth = maxMenuWidth + 10;
    $navBlock.width(resultWidth + 10);
});

// Фиксация меню вверху страницы при прокрутке страницы
var checker = false;
var $divNavigation = {};
var $objWindow = {};
$(window).on('scroll', function () {
    $divNavigation = $("div.divNavigation");
    $objWindow = $(window);
    if ($objWindow.scrollTop() >= 105) {
        if (!checker) {
            $divNavigation.css({
                position: "fixed",
                margin: "0",
                top: "0",
                width: "100%"
            });
        }
        checker = true;
    } else {
        if (checker) {
            $divNavigation.css({
                position: "",
                margin: "",
                top: "",
                width: ""
            });
        }
        checker = false;
    }
});

//МЕНЮ
$(function () {

    var $allA = $("div.divNavigation div a");

    // Для подменю 1-го уровня
    var menuTimer;
    var animHideSpeed = 200;
    var sumMenuMarginTop = 5;

    $allA.each(function () {
        if (this.id) {
            var idname = this.id;
            var $uslugi = $('#' + idname);
            var $sectUslugi = $('#section_' + idname);

            if ($sectUslugi.length > 0) {
                $sectUslugi.css({ 'opacity': '0' });
                $uslugi.on('mouseover', function () {
                    clearTimeout(menuTimer);

                    $allA.each(function () { //скрываем все открытые подменю
                        if (this.id) {
                            var idname1 = this.id;
                            var $sectUslugi1 = $('#section_' + idname1);
                            if ($sectUslugi1.length > 0) {
                                $sectUslugi1.animate({ 'opacity': 0 }, animHideSpeed);
                                $sectUslugi1.hide();
                            }
                        }
                    });

                    $sectUslugi.show();
                    $sectUslugi.offset({ top: $uslugi.offset().top + $uslugi.outerHeight(true) + sumMenuMarginTop, left: $uslugi.offset().left });
                    $sectUslugi.animate({ 'opacity': 1 }, animHideSpeed);
                });
                $uslugi.on('mouseout', function () {
                    menuTimer = setTimeout(function () {
                        $sectUslugi.animate({ 'opacity': 0 }, animHideSpeed);
                        $sectUslugi.hide();
                    }, 500);
                });
                $sectUslugi.on('mouseover', function () {
                    clearTimeout(menuTimer);
                });
                $sectUslugi.on('mouseout', function () {
                    menuTimer = setTimeout(function () {
                        $sectUslugi.animate({ 'opacity': 0 }, animHideSpeed);
                        $sectUslugi.hide();
                    }, 500);
                });
            }

        }
    });

    // Для подменю 2-го уровня
    var menuTimer1;
    var animHideSpeed1 = 100;
    var sumMenuMarginLeft = 2;

    $allA.each(function () {
        if (this.id) {
            var idname = this.id;
            var $uslugi = $('#' + idname);
            var $subSect = $('#subsection_' + idname);

            if ($subSect.length > 0) {
                $subSect.css({ 'opacity': '0' });
                $uslugi.on('mouseover', function () {
                    clearTimeout(menuTimer1);

                    $allA.each(function () { //скрываем все открытые подменю
                        if (this.id) {
                            var idname1 = this.id;
                            var $subSect1 = $('#subsection_' + idname1);
                            if ($subSect1.length > 0) {
                                $subSect1.animate({ 'opacity': 0 }, animHideSpeed1);
                                $subSect1.hide();
                            }
                        }
                    });

                    $subSect.show();
                    $subSect.offset({ top: $uslugi.offset().top, left: $uslugi.offset().left + $uslugi.outerWidth(true)  + sumMenuMarginLeft});
                    $subSect.animate({ 'opacity': 1 }, animHideSpeed1);
                });
                $uslugi.on('mouseout', function () {
                    menuTimer1 = setTimeout(function () {
                        $subSect.animate({ 'opacity': 0 }, animHideSpeed1);
                        $subSect.hide();
                    }, 300);
                });
                $subSect.on('mouseover', function () {
                    clearTimeout(menuTimer1);
                    clearTimeout(menuTimer);
                });
                $subSect.on('mouseout', function () {
                    menuTimer1 = setTimeout(function () {
                        $subSect.animate({ 'opacity': 0 }, animHideSpeed1);
                        $subSect.hide();

                        $allA.each(function () { //скрываем все открытые подменю 1 уровня
                            if (this.id) {
                                var idname1 = this.id;
                                var $sectUslugi1 = $('#section_' + idname1);
                                if ($sectUslugi1.length > 0) {
                                    $sectUslugi1.animate({ 'opacity': 0 }, animHideSpeed);
                                    $sectUslugi1.hide();
                                }
                            }
                        });

                    }, 300);
                });
            }

        }
    });
});