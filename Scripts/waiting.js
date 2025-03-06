// универсальное окно, отображающее ожидание (стили в файле waiting.css)

var $tblWaiting;
var timeOutWaiting;
function waiting(text, delay) {
    timeOutWaiting = setTimeout(function () { waitingOpen(text); } , delay);
}
function waitingOpen(text) {
    var htmlString = "<table id='formWaitingBckgrd'><tr><td>";
    htmlString += "<div id='formWaiting'>";
    htmlString += "<div id='waiting_tbl_wrap'>";
    htmlString += "<img src='' id='waiting_img' />";
    htmlString += "<span id='spanWaitingText'>" + text + "</span>";
    htmlString += "</div>";
    htmlString += "</div>";
    htmlString += "</td></tr></table>";
    $('body').prepend(htmlString);

    try{
        document.getElementById('waiting_img').src = wait_Img.src;
    }
    catch(ex){
        document.getElementById('waiting_img').src = "../../../images/waiting.gif";
    }

    $tblWaiting = $('#formWaitingBckgrd');
    $tblWaiting.css('opacity', '0');

    $tblWaiting.animate({
        opacity: 1
    }, 200);
}
function waitingClose() {

    if (timeOutWaiting) {
        clearTimeout(timeOutWaiting);
    }

    if ($tblWaiting) {
        $tblWaiting.animate({
            opacity: 0
        }, 200, function () {
            $tblWaiting.remove();
        });
    }

}