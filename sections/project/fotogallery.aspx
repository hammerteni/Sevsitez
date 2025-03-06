<%@ Page Title="Фотогалерии к конкурсам образовательного проекта «Москва - Крым - Территория талантов» (Воссоединение Крыма с Россией)" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="fotogallery.aspx.cs" Inherits="site.sections.project.fotogallery" EnableSessionState="True"
    MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="/App_Themes/site/stages.css?v=0" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Panel runat="server" ID="panelBtnBack">
        <asp:LinkButton Text="" runat="server" CssClass="lBtnBack" PostBackUrl="~/sections/project/fotogallery.aspx" />
    </asp:Panel>

    <asp:Panel runat="server" ID="addPanel"></asp:Panel>
    
    <asp:Panel ID="panelLinks" runat="server">
        <a class="aCompetitions" href="?p=galaconcert">Гала-концерт</a>
        <a class="aCompetitions" href="?p=fotogalleryfoto">Фотогалерея к конкурсу «Крымский вернисаж»</a>
        <%--<a class="aCompetitions" href="?p=fotogalleryfoto2021">Фотогалерея к конкурсу «Крымский вернисаж»</a>--%>
        <a class="aCompetitions" href="?p=fotogallerytheatrehudslovo">Фотогалерея к конкурсу творческого мастерства «Мастер сцены» - Художественное слово</a>
        <a class="aCompetitions" href="?p=fotogallerytheatrevokal">Фотогалерея к конкурсу творческого мастерства «Мастер сцены» - Вокал</a>
        <%--<a class="aCompetitions" href="?p=fotogallerytheatrevokal2021">Фотогалерея к конкурсу творческого мастерства «Мастер сцены» - Вокал</a>--%>
        <a class="aCompetitions" href="?p=fotogallerytheatreinstrum">Фотогалерея к конкурсу творческого мастерства «Мастер сцены» - Инструментальный жанр</a>
        <a class="aCompetitions" href="?p=fotogallerytheatre2021">Фотогалерея к конкурсу творческого мастерства «Мастер сцены» - Театральное искусство</a>
        <a class="aCompetitions" href="?p=fotogallerytheatrehoreo2021">Фотогалерея к конкурсу творческого мастерства «Мастер сцены» - Хореография</a>
        
        <a class="aCompetitions" href="?p=fotogallerysportfootball">Фотогалерея к спортивному конкурсу «Футбол»</a>

        <%--<a class="aCompetitions" href="?p=fotogallerysportedinob2021">Фотогалерея к спортивному турниру - Простейшие единоборства</a>
        <a class="aCompetitions" href="?p=fotogallerymultimedia2021">Фотогалерея к конкурсу мультимедиа проектов</a>
        <a class="aCompetitions" href="?p=fotogalleryrobotech2021">Фотогалерея к конкурсу Робототехники и 3D моделированию «Шаг в будущее»</a>--%>

        <a class="aCompetitions" href="?p=fotogalleryrobotech">Фотогалерея к конкурсу Робототехники и 3D моделированию «Шаг в будущее»</a>
        <a class="aCompetitions" href="?p=fotogallerykorablik">Фотогалерея к конкурсу «Кораблик детства»</a>
        <a class="aCompetitions" href="?p=fotogalleryclothes">Фотогалерея к конкурсу «Индустрия моды». Чудесные лоскутки</a>
        <a class="aCompetitions" href="?p=fotogallerykuturie">Фотогалерея к конкурсу «Индустрия моды». Юный кутюрье</a>
        <a class="aCompetitions" href="?p=fotogalleryteatrmody">Фотогалерея к конкурсу «Индустрия моды. Театр Моды»</a>
        <%--<a class="aCompetitions" href="?p=fotogallerykorablik2021">Фотогалерея к конкурсу «Кораблик детства»</a>
        <a class="aCompetitions" href="?p=fotogalleryclothes2021">Фотогалерея к конкурсу «Индустрия моды»</a>--%>

        <a class="aCompetitions" href="?p=fotogalleryshahmaty">Фотогалерея к Турниру по Шахматам «64 поля»</a>
       <%-- <a class="aCompetitions" href="?p=fotogalleryshahmaty2021">Фотогалерея к Турниру по Шахматам «64 поля»</a>--%>
        <%--<a class="aCompetitions" href="?p=fotogalleryshashki2021">Фотогалерея к Турниру по Шашкам «32 поля»</a>--%>
        <a class="aCompetitions" href="?p=fotogalleryshashki">Фотогалерея к Турниру по Шашкам «32 поля»</a>
        <a class="aCompetitions" href="?p=fotogalleryscience">Фотогалерея к конкурсу научных работ «В моей лаборатории вот что...»</a>
        
    </asp:Panel>
        
</asp:Content>