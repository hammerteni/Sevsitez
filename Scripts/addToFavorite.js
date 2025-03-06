//Функции для кнопки добавления сайта в избранное 
//(вид ссылки для вызова функции - <noindex><a href='' onClick='return add_favorite(this);'>В закладки</a></noindex>)
function add_favorite(a) {
    title = document.title;
    url = document.location;
    try {
        // Internet Explorer
        eval("window.external.AddFa-vorite(url, title)".replace(/-/g, ''));
    }
    catch (e) {
        try {
            // Mozilla
            window.sidebar.addPanel(title, url, "");
        }
        catch (e) {
            // Opera
            if (typeof (opera) == "object") {
                a.rel = "sidebar";
                a.title = title;
                a.url = url;
                return true;
            }
            else {
                // Остальные браузеры
                alert('Нажмите Ctrl + D, чтобы добавить страницу в закладки');
            }
        }
    }
    return false;
}