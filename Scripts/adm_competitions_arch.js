/* Применяется в классе CompetitionsForm, на странице - competitionsarch.aspx */
$(function () {

    var code = document.getElementById('competitions_code').childNodes[0].nodeValue;

    //Part1
    var $reqFields = $('.js_txtBoxReqs');
    var operationCheck = false;

    function key(event) {
        return ('which' in event) ? event.which : event.keyCode;
    }

    function updateRequestData(reqid, newValue, fieldName) {
        function onSuccess(result, eventArgs) {
            if (result === 'ok') {
                //
            }
            else if (result === 'err') {
                alert('Не удалось сохранить данные. Попробуйте повторить.');
            }
            else if (result === 'date_false') {
                alert('Неправильно введена дата. Формат даты - ДД.ММ.ГГГГ');
            }
            else if (result === 'weight_false') {
                alert('Вес введён неверно');
            }
            else if (result === 'agecategory_false') {
                alert('Возрастная категория введена неверно. Может быть - Дошкольная, Младшая, Средняя, Молодежь, Смешанная, Профи, 1 группа, 2 группа, 3 группа, 4 группа');
            }
            else if (result === 'partycount_false') {
                alert('Количество участников I отборочного тура по заявке введено неверно, может быть 1 участник и более..');
            }
            else if (result === 'points_false') {
                alert('Кол-во баллов не может быть меньше 0 и должно быть в пределах от 0 до 36');
            }
            operationCheck = false;
            window.location.reload();
        }
        function onError(result) {
            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck = false;
        }
        site.DataService.UpdateRequestData_Arch(reqid, newValue, fieldName, code, onSuccess, onError);
    }

    $reqFields.each(function () {

        $(this).on('keydown', function (event) {
            if (operationCheck) return false;
            if (key(event) === 13 && event.ctrlKey) {
                $(this).val($(this).val() + '\r\n');
            }
            else if (key(event) === 13) {
                operationCheck = true;
                updateRequestData($(this).data('reqid'), $(this).val(), $(this).data('fieldname'));
                return false;
            }
        });

    });



    //Part2
    var $reqFields1 = $('.js_txtBoxReqsGroup');
    var operationCheck1 = false;

    function updateRequestDataGroup(reqid, newValue, fieldName, position) {
        function onSuccess(result, eventArgs) {
            if (result === 'ok') {
                //
            }
            else if (result === 'err') {
                alert('Не удалось сохранить данные. Попробуйте повторить.');
            }
            else if (result === 'date_false') {
                alert('Неправильно введена дата. Формат даты - ДД.ММ.ГГГГ');
            }
            else if (result === 'weight_false') {
                alert('Вес введён неверно');
            }
            else if (result === 'empty_fio') {
                alert('Введите ФИО');
            }
            else if (result === 'incorrect_fio') {
                alert('ФИО введено неверно, формат - Фамилия Имя Отчество');
            }
            else if (result === 'empty_position') {
                alert('Введите ДОЛЖНОСТЬ');
            }

            operationCheck1 = false;
            window.location.reload();
        }
        function onError(result) {
            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck1 = false;
        }
        site.DataService.UpdateRequestDataGroup_Arch(reqid, newValue, fieldName, position, code, onSuccess, onError);
    }

    $reqFields1.each(function () {

        $(this).on('keydown', function (event) {
            if (operationCheck1) return false;
            if (key(event) === 13 && event.ctrlKey) {
                $(this).val($(this).val() + '\r\n');
            }
            else if (key(event) === 13) {
                operationCheck1 = true;
                updateRequestDataGroup($(this).data('reqid'), $(this).val(), $(this).data('fieldname'), $(this).data('position'));
                return false;
            }
        });

    });


    //Part2_1
    var $reqFields1_1 = $('.js_txtBoxReqsGroup_Ext');
    var operationCheck1_1 = false;

    function updateRequestDataGroup_Ext(reqid, newValue, fieldName, position) {

        function onSuccess(result, eventArgs) {

            if (result === 'ok') {
                //
            }
            else if (result === 'err') {
                alert('Не удалось сохранить данные. Попробуйте повторить.');
            }
            else if (result === 'date_false') {
                alert('Неправильно введена дата. Формат даты - ДД.ММ.ГГГГ');
            }
            else if (result === 'empty_value') {
                alert('Значение не должно быть пустым');
            }
            else if (result === 'incorrect_fio') {
                alert('ФИО введено неверно, формат - Фамилия Имя Отчество');
            }

            operationCheck1_1 = false;
        }
        function onError(result) {

            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck1_1 = false;
        }

        site.DataService.UpdateRequestDataGroup_Ext_Arch(reqid, newValue, fieldName, position, code, onSuccess, onError);
    }

    $reqFields1_1.each(function () {

        $(this).on('keydown', function (event) {
            if (operationCheck1_1) return false;
            if (key(event) === 13 && event.ctrlKey) {
                $(this).val($(this).val() + '\r\n');
            }
            else if (key(event) === 13) {
                operationCheck1_1 = true;
                updateRequestDataGroup_Ext($(this).data('reqid'), $(this).val(), $(this).data('fieldname'), $(this).data('position'));
                return false;
            }
        });

    });


    //Part 4 Для добавления нового участника
    var $reqFields4 = $('#js_btnAddParty');
    var operationCheck4 = false;

    $reqFields4.on('click', function (event) {
        if (operationCheck4) return false;

        waiting('Операция выполняется. Подождите...', 500)

        operationCheck4 = true;

        var weigth4 = document.getElementById('js_newWeight');
        if (weigth4)        //для спортивного конкурса
        {
            updateRequestNewParty($reqFields4.data('reqid'), $('#js_newFio').val(), $('#js_newAge').val(), $('#js_newWeight').val(), $reqFields4.data('cname'));
        }
        else {
            updateRequestNewPartyNoWeight($reqFields4.data('reqid'), $('#js_newFio').val(), $('#js_newAge').val(), '', $reqFields4.data('cname'));
        }
    });

    function updateRequestNewParty(reqid, fio, age, weight, cname) {

        function onSuccess(result, eventArgs) {

            if (result === 'ok') {
                window.location.reload();
            }
            else if (result === 'no_fio') {
                alert('Введите ФИО');
            }
            else if (result === 'no_age') {
                alert('Введите ДАТУ РОЖДЕНИЯ');
            }
            else if (result === 'age_err') {
                alert('Дата рождения введена неверно, формат - ДД.ММ.ГГГГ');
            }
            else if (result === 'no_weight') {
                alert('Введите ВЕС');
            }
            else if (result === 'weight_err') {
                alert('Вес введён неверно');
            }
            else if (result === 'err') {
                alert('Не удалось добавить участника. Попробуйте повторить.');
            }

            operationCheck4 = false;
            waitingClose();
        }

        function onError(result) {
            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck4 = false;
            waitingClose();
        }

        site.DataService.UpdateRequestAddNewParty_Arch(reqid, fio, age, weight, cname, code, onSuccess, onError);
    }
    function updateRequestNewPartyNoWeight(reqid, fio, age, cname) {

        function onSuccess(result, eventArgs) {

            if (result === 'ok') {
                window.location.reload();
            }
            else if (result === 'no_fio') {
                alert('Введите ФИО');
            }
            else if (result === 'no_age') {
                alert('Введите ДАТУ РОЖДЕНИЯ');
            }
            else if (result === 'age_err') {
                alert('Дата рождения введена неверно, формат - ДД.ММ.ГГГГ');
            }
            else if (result === 'weight_err') {
                alert('Вес введён неверно');
            }
            else if (result === 'err') {
                alert('Не удалось добавить участника. Попробуйте повторить.');
            }

            operationCheck4 = false;
            waitingClose();
        }

        function onError(result) {
            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck4 = false;
            waitingClose();
        }

        site.DataService.UpdateRequestAddNewPartyNoWeight_Arch(reqid, fio, age, cname, code, onSuccess, onError);
    }


    //Part 4_1 Для добавления нового участника
    var $reqFields4_1 = $('#js_btnAddParty_Ext');
    var operationCheck4_1 = false;

    $reqFields4_1.on('click', function (event) {

        if (operationCheck4_1) return false;

        waiting('Операция выполняется. Подождите...', 0)

        operationCheck4_1 = true;

        updateRequestNewParty_Ext($reqFields4_1.data('reqid'), $('#js_newFio').val(), $('#js_newAge').val(), $('#js_newAgeCategory').val(),
            $('#js_newWeight').val(), $('#js_newKvalif').val(), $('#js_newProgrm').val());
    });

    function updateRequestNewParty_Ext(reqid, fio, age, ageCategory, weight, kvalif, program) {

        function onSuccess(result, eventArgs) {

            if (result === 'ok') {
                window.location.reload();
            }
            else if (result === 'empty_value') {
                alert('Не должно быть пустых значений');
            }
            else if (result === 'incorrect_fio') {
                alert('ФИО введено неверно, формат - Фамилия Имя Отчество');
            }
            else if (result === 'date_false') {
                alert('Неправильно введена дата. Формат даты - ДД.ММ.ГГГГ');
            }
            else if (result === 'err') {
                alert('Не удалось добавить участника. Попробуйте повторить.');
            }

            operationCheck4_1 = false;
            waitingClose();
        }

        function onError(result) {

            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck4_1 = false;
            waitingClose();
        }

        site.DataService.UpdateRequestAddNewParty_Ext_Arch(reqid, fio, age, ageCategory, weight, kvalif, program, code, onSuccess, onError);
    }


    //Part 5 Для удаления участника
    var $reqFields5 = $('.js_btnDelParty');
    var operationCheck5 = false;

    $reqFields5.each(function () {

        $(this).on('click', function (event) {
            if (operationCheck5) return false;

            if (confirm('Участник будет удалён из заявки. Удалить?')) {
                waiting('Операция выполняется. Подождите...', 500)

                operationCheck5 = true;
                updateRequestDelParty($(this).data('reqid'), $(this).data('position'), $(this).data('cname'));
            }
        });

    });

    function updateRequestDelParty(reqid, position, cname) {

        function onSuccess(result, eventArgs) {

            if (result === 'ok') {
                window.location.reload();
            }
            else if (result === 'err') {
                alert('Не удалось удалить участника. Попробуйте повторить.');
            }

            operationCheck5 = false;
            waitingClose();
        }

        function onError(result) {

            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck5 = false;
            waitingClose();

        }

        site.DataService.UpdateRequestDelParty_Arch(reqid, position, cname, code, onSuccess, onError);
    }


    //Part 5_1 Для удаления участника
    var $reqFields5_1 = $('.js_btnDelParty_Ext');
    var operationCheck5_1 = false;

    $reqFields5_1.each(function () {

        $(this).on('click', function (event) {
            if (operationCheck5_1) return false;

            if (confirm('Участник будет удалён из заявки. Удалить?')) {
                waiting('Операция выполняется. Подождите...', 0)

                operationCheck5_1 = true;
                updateRequestDelParty_Ext($(this).data('reqid'), $(this).data('position'));
            }
        });

    });

    function updateRequestDelParty_Ext(reqid, position) {

        function onSuccess(result, eventArgs) {

            if (result === 'ok') {
                window.location.reload();
            }
            else if (result === 'err') {
                alert('Не удалось удалить участника. Попробуйте повторить.');
            }

            operationCheck5_1 = false;
            waitingClose();
        }

        function onError(result) {

            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck5_1 = false;
            waitingClose();

        }

        site.DataService.UpdateRequestDelParty_Ext_Arch(reqid, position, code, onSuccess, onError);
    }


    //Part 6 Для добавления нового педагога
    var $reqFields6 = $('#js_btnAddChief');
    var operationCheck6 = false;

    $reqFields6.on('click', function (event) {
        if (operationCheck6) return false;

        waiting('Операция выполняется. Подождите...', 500)

        operationCheck6 = true;

        updateRequestNewChief($reqFields6.data('reqid'), $('#js_newChiefFio').val(), $('#js_newChiefPosition').val());
    });

    function updateRequestNewChief(reqid, fio, position) {

        function onSuccess(result, eventArgs) {

            if (result === 'ok') {
                window.location.reload();
            }
            else if (result === 'no_fio') {
                alert('Введите ФИО');
            }
            else if (result === 'no_position') {
                alert('Введите ДОЛЖНОСТЬ');
            }
            else if (result === 'incorrect_fio') {
                alert('ФИО введено неверно, формат - Фамилия Имя Отчество');
            }
            else if (result === 'err') {
                alert('Не удалось добавить педагога. Попробуйте повторить.');
            }

            operationCheck6 = false;
            waitingClose();
        }

        function onError(result) {
            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck6 = false;
            waitingClose();
        }

        site.DataService.UpdateRequestAddNewChief_Arch(reqid, fio, position, code, onSuccess, onError);
    }


    //Part 7 Для удаления педагога
    var $reqFields7 = $('.js_btnDelChief');
    var operationCheck7 = false;

    $reqFields7.each(function () {

        $(this).on('click', function (event) {
            if (operationCheck5) return false;

            if (confirm('Педагог будет удалён из заявки. Удалить?')) {
                waiting('Операция выполняется. Подождите...', 500)

                operationCheck7 = true;
                updateRequestDelChief($(this).data('reqid'), $(this).data('position'));
            }
        });

    });

    function updateRequestDelChief(reqid, position) {

        function onSuccess(result, eventArgs) {

            if (result === 'ok') {
                window.location.reload();
            }
            else if (result === 'err') {
                alert('Не удалось удалить педагога. Попробуйте повторить.');
            }

            operationCheck7 = false;
            waitingClose();
        }

        function onError(result) {

            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck7 = false;
            waitingClose();

        }

        site.DataService.UpdateRequestDelChief_Arch(reqid, position, code, onSuccess, onError);
    }


    //Part 8 Для добавления нового автора коллекции
    var $reqFields8 = $('#js_btnAddAthor');
    var operationCheck8 = false;

    $reqFields8.on('click', function (event) {
        if (operationCheck8) return false;

        waiting('Операция выполняется. Подождите...', 500)

        operationCheck8 = true;

        updateRequestNewAthor($reqFields8.data('reqid'), $('#js_newAthorFio').val());
    });

    function updateRequestNewAthor(reqid, fio) {

        function onSuccess(result, eventArgs) {

            if (result === 'ok') {
                window.location.reload();
            }
            else if (result === 'no_fio') {
                alert('Введите ФИО');
            }
            else if (result === 'incorrect_fio') {
                alert('ФИО введено неверно, формат - Фамилия Имя Отчество');
            }
            else if (result === 'err') {
                alert('Не удалось добавить автора. Попробуйте повторить.');
            }

            operationCheck8 = false;
            waitingClose();
        }

        function onError(result) {
            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck8 = false;
            waitingClose();
        }

        site.DataService.UpdateRequestAddNewAthor_Arch(reqid, fio, code, onSuccess, onError);
    }


    //Part 9 Для удаления автора
    var $reqFields9 = $('.js_btnDelAthor');
    var operationCheck9 = false;

    $reqFields9.each(function () {

        $(this).on('click', function (event) {
            if (operationCheck9) return false;

            if (confirm('Педагог будет удалён из заявки. Удалить?')) {
                waiting('Операция выполняется. Подождите...', 500)

                operationCheck9 = true;
                updateRequestDelAthor($(this).data('reqid'), $(this).data('position'));
            }
        });

    });

    function updateRequestDelAthor(reqid, position) {

        function onSuccess(result, eventArgs) {

            if (result === 'ok') {
                window.location.reload();
            }
            else if (result === 'err') {
                alert('Не удалось удалить автора. Попробуйте повторить.');
            }

            operationCheck9 = false;
            waitingClose();
        }

        function onError(result) {

            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck9 = false;
            waitingClose();

        }

        site.DataService.UpdateRequestDelAthor_Arch(reqid, position, code, onSuccess, onError);
    }

});