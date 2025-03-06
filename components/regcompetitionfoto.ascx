<%@ Control %>

<link rel="stylesheet" href="/components/bootstrap-vue/bootstrap-vue.min.css" />
<link rel="stylesheet" href="/components/bootstrap/bootstrap.min.css" />
<%--<link rel="stylesheet" href="/components/vue-datetime/vue-datetime.min.css" />--%>
<style>
    body {
        line-height: 1;
    }
    .vdatetime {
        display:inline-flex;
    }
    .vdatetime input {
        width: -webkit-fill-available;
    }
    .vdp-datepicker {
        display:inline-flex;
    }
    .vdp-datepicker div {
            border: none;
            border-color:none;
    }
    .vdp-datepicker div input{
        width: 70px;
            border: none;
            border-color:none;
           
    }
    .div.navBtnsWrap a {
        height: 55px !important;
    }
</style>
<script src="/Scripts/axios/axios.min.js"></script>

<div id="regCompetitionfoto">
    <div class="divMainWrap_c">
        <h1>Форма заявки<br>
            на участие в Конкурсе "Крымский вернисаж"</h1>
        <div class="panelSub_c">
            <p class="abzac_c">Добрый день! </p>
            <p class="abzac_c">Поля, отмеченные звёздочкой (<span class="span_red_c">*</span>) - <span class="span_red_c">обязательны для заполнения</span></p>
        </div>
        <div class="panelSub_c">
            <table class="table_Form_c">
                <tbody>
                    <tr>
                        <td class="td_Title_c">Номинация</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <select v-model="request.DropDown_Subsection" disabled="disabled" class="txtBox_c placeHolderForm_c" style="width: 500px;">
                                <option selected="selected" value="Фотография">Фотография</option>
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td class="td_Title_c">ФИО/Дата рождения</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <input v-model="request.txtBox_FIO" type="text" maxlength="100" class="txtBox_c placeHolderForm_c" placeholder="Ситников Андрей Игоревич" style="width: 387px; ">
                            &nbsp;/&nbsp;
                            <%--<datetime type="date" 
                                v-model="request.txtBox_Age" 
                                format="dd.MM.yyyy"
                                :phrases="{ok: 'Выбрать', cancel: 'Закрыть'}"
                                :hide-backdrop="false"
                                :backdrop-click="false"
                                class="txtBox_c placeHolderForm_c" 
                                placeholder="04.11.2001" maxlength="10" style="width:90px;">
                            </datetime>--%>

                           <datepicker 
                               v-model="request.txtBox_Age1" 
                               format="dd.MM.yyyy"
                               :language="ru"
                               placeholder="04.11.2001" 
                               class="txtBox_c"
                               maxlength="10" style="width:90px;"
                               ></datepicker>

                        </td>
                    </tr>
                    <tr>
                        <td class="td_Title_c">Название работы</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <input name="ctl00$ContentPlaceHolder1$txtBox_WorkName" type="text" maxlength="100" id="txtBox_WorkName" class="txtBox_c placeHolderForm_c" placeholder="Вечерний Севастополь"></td>
                    </tr>
                    <tr>
                        <td class="td_Title_c">Комментарий к работе</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <textarea name="ctl00$ContentPlaceHolder1$txtBox_WorkComment" rows="5" cols="20" id="txtBox_WorkComment" class="txtBox_c placeHolderForm_c" placeholder="Кратко опишите свою работу. Возможный вариант описания:
Место, где производилась съёмка. Кто или что отображено на фото. Почему я выбрал именно этот образ. Что я хочу сказать этой фотографией людям, которые будут просматривать и оценивать её."></textarea></td>
                    </tr>
                    <tr>
                        <td class="td_Title_c">Образовательная организация (полностью)</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <input name="ctl00$ContentPlaceHolder1$txtBox_EducationalOrganization" type="text" maxlength="150" id="txtBox_EducationalOrganization" class="txtBox_c placeHolderForm_c" placeholder="Средняя образовательная школа № 32 города Мытищи"></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td class="explanation_c">если вы не учащийся, то введите 'НЕТ' в этом поле</td>
                    </tr>
                    <tr>
                        <td class="td_Title_c">Электронная почта участника</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <input name="ctl00$ContentPlaceHolder1$txtBox_Email" type="text" maxlength="50" id="txtBox_Email" class="txtBox_c placeHolderForm_c" placeholder="a.sitnikov@mail.ru"></td>
                    </tr>
                    <tr>
                        <td class="td_Title_c">Контактный телефон участника</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <input name="ctl00$ContentPlaceHolder1$txtBox_Telephone" type="text" maxlength="11" id="txtBox_Telephone" class="txtBox_c placeHolderForm_c" placeholder="89169985137"></td>
                    </tr>
                    <tr>
                        <td class="td_Title_c">Регион</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <input name="ctl00$ContentPlaceHolder1$txtBox_Region" type="text" maxlength="50" id="txtBox_Region" class="txtBox_c placeHolderForm_c" placeholder="Московская область"></td>
                    </tr>
                    <tr>
                        <td class="td_Title_c">Город</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <select name="ctl00$ContentPlaceHolder1$DropDown_TownPrefs" id="DropDown_TownPrefs" class="txtBox_c placeHolderForm_c" style="width: 200px;">
                                <option value="Город">Город</option>
                                <option value="Аал">Аал</option>
                                <option value="Арбан">Арбан</option>
                                <option value="Аул">Аул</option>
                                <option value="Выселок">Выселок</option>
                                <option value="Дачный поселок">Дачный поселок</option>
                                <option value="Деревня">Деревня</option>
                                <option value="Курортный поселок">Курортный поселок</option>
                                <option value="Лесной поселок">Лесной поселок</option>
                                <option value="Населенный пункт">Населенный пункт</option>
                                <option value="Поселок">Поселок</option>
                                <option value="Поселок городского типа">Поселок городского типа</option>
                                <option value="Поселок при железнодорожной станции">Поселок при железнодорожной станции</option>
                                <option value="Поселок при станции">Поселок при станции</option>
                                <option value="Рабочий поселок">Рабочий поселок</option>
                                <option value="Село">Село</option>
                                <option value="Слобода">Слобода</option>
                                <option value="Станица">Станица</option>
                                <option value="Улус">Улус</option>
                                <option value="Хутор">Хутор</option>

                            </select>&nbsp;&nbsp;<input name="ctl00$ContentPlaceHolder1$txtBox_City" type="text" maxlength="50" id="txtBox_City" class="txtBox_c placeHolderForm_c" placeholder="Долгопрудный" style="width: 294px;"></td>
                    </tr>
                    <tr>
                        <td class="td_Title_c">ФИО руководителя</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <input name="ctl00$ContentPlaceHolder1$txtBox_ChiefFio" type="text" maxlength="100" id="txtBox_ChiefFio" class="txtBox_c placeHolderForm_c" placeholder="Звягенцев Олег Георгиевич"></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td class="explanation_c">если у вас нет руководителя, то введите 'НЕТ' в этом поле</td>
                    </tr>
                    <tr>
                        <td class="td_Title_c">Должность руководителя</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <input name="ctl00$ContentPlaceHolder1$txtBox_ChiefPosition" type="text" maxlength="150" id="txtBox_ChiefPosition" class="txtBox_c placeHolderForm_c" placeholder="Заведующий по учебной части"></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td class="explanation_c">если у вас нет руководителя, то введите 'НЕТ' в этом поле</td>
                    </tr>
                    <tr>
                        <td class="td_Title_c">Электронная почта руководителя</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <input name="ctl00$ContentPlaceHolder1$txtBox_ChiefEmail" type="text" maxlength="100" id="txtBox_ChiefEmail" class="txtBox_c placeHolderForm_c" placeholder="o.zvyagentsev@yandex.ru"></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td class="explanation_c">если у вас нет руководителя, то введите 'НЕТ' в этом поле</td>
                    </tr>
                    <tr>
                        <td class="td_Title_c">Контактный телефон руководителя</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <input name="ctl00$ContentPlaceHolder1$txtBox_ChiefTelephone" type="text" maxlength="20" id="txtBox_ChiefTelephone" class="txtBox_c placeHolderForm_c" placeholder="89065556677"></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td class="explanation_c">если у вас нет руководителя, то введите 'НЕТ' в этом поле</td>
                    </tr>
                    <tr>
                        <td></td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c"><span class="chkBox_c">
                            <input id="chkBox_PdnProcessing" type="checkbox" name="ctl00$ContentPlaceHolder1$chkBox_PdnProcessing"></span><p class="p_agreement_c">Я (или родитель – если Вам нет еще 14 лет) даю свое согласие на обработку представленных мной персональных данных, для регистрации меня как участника мероприятия, а именно: ФИО, возраст, место учебы/работы, класс/должность, контактный телефон, адрес электронной почты. Подтверждаю, что даю свое согласие на размещение моих фотографий по итогам проведения мероприятия на сайте, а также в официальных группах организации в социальных сетях. В случае победы в конкурсе, даю согласие на публикацию результатов на сайте, а также в официальной группе организации в социальных сетях.</p>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c"><span class="chkBox_c">
                            <input id="chkBox_PublicAgreement" type="checkbox" name="ctl00$ContentPlaceHolder1$chkBox_PublicAgreement"></span><p class="p_agreement_c">Я (или родитель – если Вам нет еще 14 лет) даю свое согласие на публикацию своих работ с сохранением авторства.</p>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="panelSub_c">
            <p class="p_agreement_c">Добавьте свою работу.</p>
            <p class="p_agreement_c">Для этого нажмите на кнопку 'ОБЗОР', в появившемся окне выберите один файл с изображением со своего жёсткого диска. Затем нажмите на кнопку 'ДОБАВИТЬ'. Обратите внимание, что допускается добавление только одного файла изображения в формате JPG, JPEG и PNG размером не более 2-х мегабайт.</p>
            <input type="file" name="ctl00$ContentPlaceHolder1$foto_ImgUpload" id="foto_ImgUpload" title="Загружайте изображение в формате JPG, JPEG и PNG размером не более 2-х мегабайт" class="fUpload_c"><a onclick="waiting('Обработка файла. Подождите..', 0); return pageReady();" title="Загружайте изображение в формате JPG и PNG размером не более 2-х мегабайт" class="lBtnAddFoto_c" href="javascript:__doPostBack('ctl00$ContentPlaceHolder1$ctl05','')">ДОБАВИТЬ</a><div id="panelWorks_c" class="divWork_c">
            </div>
        </div>
        <div class="panelSub_c">
            <p class="p_agreement_c">Добавьте протокол 1-го (отборочного) тура и итоговое Количество участников I отборочного тура согласно этого протокола.</p>
            <p class="p_agreement_c">Для этого нажмите на кнопку 'ОБЗОР', в появившемся окне выберите файл протокола со своего жёсткого диска. Затем нажмите на кнопку 'ДОБАВИТЬ'. Обратите внимание, что допускается добавление файла протокола только в формате PDF и размером не более 1-го мегабайта.</p>
            <input type="file" name="ctl00$ContentPlaceHolder1$protocolUpload" id="protocolUpload" title="Загружайте файл в формате PDF размером не более 1-го мегабайта" class="fUpload_c"><a onclick="waiting('Обработка файла. Подождите..', 0); return pageReady();" title="Загружайте файл в формате PDF размером не более 1-го мегабайта" class="lBtnAddFoto_c" href="javascript:__doPostBack('ctl00$ContentPlaceHolder1$ctl07','')">ДОБАВИТЬ</a><table class="table_Form_c" style="margin-top: 15px">
                <tbody>
                    <tr>
                        <td class="td_Title_c">Количество участников I отборочного тура</td>
                        <td class="td_Zvezda_c">*</td>
                        <td class="td_Content_c">
                            <input name="ctl00$ContentPlaceHolder1$txtBox_ProtocolPartyCount" type="text" maxlength="3" id="txtBox_ProtocolPartyCount" class="txtBox_c placeHolderForm_c" placeholder="введите цифрой Количество участников I отборочного тура согласно протоколу"></td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div id="panelWarning_c" class="divWarning_c">
        </div>
        <a onclick="return formBtnClick();" class="lBtnSendRequest_c" href="javascript:__doPostBack('ctl00$ContentPlaceHolder1$ctl08','')">ОТПРАВИТЬ ЗАЯВКУ</a>
    </div>
</div>

<script type="text/javascript" src="/components/bootstrap-vue/bootstrap-vue.min.js"></script>
<script type="text/javascript" src="/components/vuejs-datepicker/locale/translations/ru.js"></script>

<script type="module">
    import Datepicker from '/components/vuejs-datepicker/vuejs-datepicker.esm.js';
    var vueApp = new Vue({
        el: "#regCompetitionfoto",
        components: {
            Datepicker
        },
        data: {
            isBusy: false,
            ru: vdp_translation_ru.js,
            request: {
                DropDown_Subsection: "Фотография",
                txtBox_FIO: "",
                txtBox_Age: new Date(),
                txtBox_Age1: new Date(),
            },
        },
        methods: {
            loadPage: function () {
                console.log('load page')
            },
            getData: function () {
                vueApp.isBusy = true;
                return axios({
                    method: 'get',
                    url: '/api/statistics',
                }).then(function (result) {
                    vueApp.statData = result.data.statData;
                    vueApp.unknowData = result.data.unknownRegionRequests;
                    vueApp.unknownMoscowData = result.data.unknownMoscowRequests;
                    vueApp.isBusy = false;
                }).catch(function (err) {
                    console.log(err);
                    vueApp.isBusy = false;
                });
            }
        },
        created: function () {
            this.loadPage();
        }
    });

</script>
