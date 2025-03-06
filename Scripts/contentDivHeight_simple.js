//Код изменения размеров блока содержимого
$(document).ready(function () {

    var $obj = $('div#contentDiv');

    if ($obj.height() < 400) {
        $obj.height(400);
    }

});