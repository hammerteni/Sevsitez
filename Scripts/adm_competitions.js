/* competitions.aspx */
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
            else if (result === 'partycount_false')
            {
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
        site.DataService.UpdateRequestData(reqid, newValue, fieldName, code, onSuccess, onError);
    }

    $reqFields.each(function () {

        $(this).on('keydown', function (event) {
            if (operationCheck) return false;
            if (key(event) === 13 && event.ctrlKey) {
                $(this).val($(this).val() + '\r\n');
            }
            else if (key(event) === 13) {
                console.log('updateRequestData');
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
            else if (result === 'empty_fio') {
                alert('Введите ФИО');
            }
            else if (result === 'incorrect_fio') {
                alert('ФИО введено неверно, формат - Фамилия Имя Отчество');
            }
            else if (result === 'date_false') {
                alert('Неправильно введена дата. Формат даты - ДД.ММ.ГГГГ');
            }
            else if (result === 'weight_false') {
                alert('Вес введён неверно');
            }
            else if (result === 'empty_school') {
                alert('Введите Место учёбы');
            }
            else if (result === 'empty_classroom') {
                alert('Введите Класс с литерой');
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
        site.DataService.UpdateRequestDataGroup(reqid, newValue, fieldName, position, code, onSuccess, onError);
    }

    
    $reqFields1.each(function () {
        $(this).on('keydown', function (event) {
            if (operationCheck1)
                return false;
               
            if (key(event) === 13 && event.ctrlKey) {
                $(this).val($(this).val() + '\r\n');
            }
            else if (key(event) === 13) {
                console.log('updateRequestDataGroup');
                operationCheck1 = true;
                updateRequestDataGroup($(this).data('reqid'), $(this).val(), $(this).data('fieldname'), $(this).data('position'));
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

        site.DataService.UpdateRequestDataGroup_Ext(reqid, newValue, fieldName, position, code, onSuccess, onError);
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


    // Part 3 Для кнопок публикации работ в таблице

    var $reqFields2 = $('.js_publishOnOff');
    var operationCheck2 = false;
    var btnIndex = "0";

    function updateRequestPublish(reqid, checker) {

        function onSuccess(result, eventArgs) {

            var elem;

            if (result === '0') {
                elem = $('#btnPublishOnOff_' + btnIndex);
                elem.attr('class', 'btnOff');
                elem.data('checker', result);
            }
            else if (result === '1') {
                elem = $('#btnPublishOnOff_' + btnIndex);
                elem.attr('class', 'btnOn');
                elem.data('checker', result);
            }
            else if (result === 'err') {
                alert('Не удалось опубликовать работу. Попробуйте повторить.');
            }

            operationCheck2 = false;
            waitingClose();
        }
        function onError(result) {
            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck2 = false;
            waitingClose();
        }
        site.DataService.UpdateRequestPublish(reqid, checker, code, onSuccess, onError);
    }

    $reqFields2.each(function () {

        $(this).on('click', function (event) {
            if (operationCheck2) return false;

            waiting('Операция выполняется. Подождите...', 1000)

            operationCheck2 = true;
            btnIndex = $(this).data('index');
            updateRequestPublish($(this).data('reqid'), $(this).data('checker'), code);
            return false;
        });

    });


    // Part 3.1 Для кнопок публикации работы на странице редактирования заявки

    var $btnPublishOnOff = $('#btnPublishOnOff');
    $btnPublishOnOff.on('click', function (event) {
        if (operationCheck3) return false;

        waiting('Операция выполняется. Подождите...', 1000)

        operationCheck3 = true;
        updateRequestPublish1($(this).data('reqid'), $(this).data('checker'), code);
        return false;
    });
    var operationCheck3 = false;

    function updateRequestPublish1(reqid, checker) {

        function onSuccess(result, eventArgs) {

            if (result === '0') {
                $btnPublishOnOff.attr('class', 'btnPublishOnOff_Off');
                $btnPublishOnOff.data('checker', result);
            }
            else if (result === '1') {
                $btnPublishOnOff.attr('class', 'btnPublishOnOff_On');
                $btnPublishOnOff.data('checker', result);
            }
            else if (result === 'err') {
                alert('Не удалось опубликовать работу. Попробуйте повторить.');
            }

            operationCheck3 = false;
            waitingClose();
        }
        function onError(result) {
            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck3 = false;
            waitingClose();
        }
        site.DataService.UpdateRequestPublish(reqid, checker, code, onSuccess, onError);
    }


    // Part 3.2 Для кнопок проверено администратором работ в таблице

    var $reqFields32 = $('.js_checkedAdminOnOff');
    var operationCheck32 = false;
    var btnIndex = "0";

    function updateRequestCheckedAdmin(reqid, checker) {

        function onSuccess(result, eventArgs) {

            var elem;

            if (result === '0') {
                elem = $('#btnCheckedAdminOnOff_' + btnIndex);
                elem.attr('class', 'btnOff');
                elem.data('checker', result);
            }
            else if (result === '1') {
                elem = $('#btnCheckedAdminOnOff_' + btnIndex);
                elem.attr('class', 'btnOn');
                elem.data('checker', result);
            }
            else if (result === 'err') {
                alert('Не удалось проставить или снять признак проверено. Попробуйте повторить.');
            }

            operationCheck32 = false;
            waitingClose();
        }
        function onError(result) {
            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck32 = false;
            waitingClose();
        }
        site.DataService.UpdateRequestCheckedAdmin(reqid, checker, code, onSuccess, onError);
    }

    $reqFields32.each(function () {

        $(this).on('click', function (event) {
            if (operationCheck32) return false;

            waiting('Операция выполняется. Подождите...', 1000)

            operationCheck32 = true;
            btnIndex = $(this).data('index');
            updateRequestCheckedAdmin($(this).data('reqid'), $(this).data('checker'), code);
            return false;
        });

    });


    // Part 3.2 Для кнопок публикации работы на странице радактирования заявки

    var $btnCheckedAdminOnOff = $('#btnCheckedAdminOnOff');
    $btnCheckedAdminOnOff.on('click', function (event) {
        if (operationCheck33) return false;

        waiting('Операция выполняется. Подождите...', 1000)

        operationCheck33 = true;
        updateRequestCheckedAdmin1($(this).data('reqid'), $(this).data('checker'), code);
        return false;
    });
    var operationCheck33 = false;

    function updateRequestCheckedAdmin1(reqid, checker) {

        function onSuccess(result, eventArgs) {

            if (result === '0') {
                $btnCheckedAdminOnOff.attr('class', 'btnCheckedAdminOnOff_Off');
                $btnCheckedAdminOnOff.data('checker', result);
            }
            else if (result === '1') {
                $btnCheckedAdminOnOff.attr('class', 'btnCheckedAdminOnOff_On');
                $btnCheckedAdminOnOff.data('checker', result);
            }
            else if (result === 'err') {
                alert('Не удалось проставить или снять признак проверено. Попробуйте повторить.');
            }

            operationCheck33 = false;
            waitingClose();
        }
        function onError(result) {
            alert('Техническая ошибка при сохранении данных. Данные не сохранены. Попробуйте повторить или сообщите в техподдержку.');
            operationCheck33 = false;
            waitingClose();
        }
        site.DataService.UpdateRequestCheckedAdmin(reqid, checker, code, onSuccess, onError);
    }

    //Part 4 Для добавления нового участника

    var $reqFields4 = $('#js_btnAddParty, .js_btnAddParty');
    var operationCheck4 = false;

    $reqFields4.on('click', function (event) {

        if (operationCheck4) return false;
        
        waiting('Операция выполняется. Подождите...', 500)

        var protocolType = ""
        if ($(this).data('protocoltype') === undefined)
            protocolType = "";
        else
            protocolType = $(this).data('protocoltype');

        operationCheck4 = true;
        var cname = $(this).data('cname');
        var reqid = $(this).data('reqid');
        var weigth4 = $('#js_newWeight' + (protocolType == '' || protocolType == '2' ? '' : '_' + protocolType) + '').val();
        var fio = $('#js_newFio' + (protocolType == '' || protocolType == '2' ? '' : '_' + protocolType) + '').val();
        var age = $('#js_newAge' + (protocolType == '' || protocolType == '2' ? '' : '_' + protocolType) + '').val();
        var schools = ($('#js_newSchools' + (protocolType == '' || protocolType == '2' ? '' : '_' + protocolType) + '').val() === undefined ? null : $('#js_newSchools' + (protocolType == '' || protocolType == '2' ? '' : '_' + protocolType) + '').val());
        var classRooms = ($('#js_newClassRooms' + (protocolType == '' || protocolType == '2' ? '' : '_' + protocolType) + '').val() === undefined ? null : $('#js_newClassRooms' + (protocolType == '' || protocolType == '2' ? '' : '_' + protocolType) + '').val());

        if (weigth4)        //для спортивного конкурса
        {
            updateRequestNewParty(reqid, fio, age, weigth4, cname, schools, classRooms, protocolType);
        }
        else {
            updateRequestNewPartyNoWeight(reqid, fio, age, cname, schools, classRooms, protocolType);
        }
    });

    function updateRequestNewParty(reqid, fio, age, weight, cname, schools, classRooms, protocolType) {

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
            else if (result === 'no_schools') {
                alert('Введите Место учёбы');
            }
            else if (result === 'no_classRooms') {
                alert('Введите Класс с литерой');
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

        site.DataService.UpdateRequestAddNewParty(reqid, fio, age, weight, cname, code, schools, classRooms, protocolType, onSuccess, onError);
    }
    function updateRequestNewPartyNoWeight(reqid, fio, age, cname, schools, classRooms, protocolType) {

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
            else if (result === 'no_schools') {
                alert('Введите Место учёбы');
            }
            else if (result === 'no_classRooms') {
                alert('Введите Класс с литерой');
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

        site.DataService.UpdateRequestAddNewPartyNoWeight(reqid, fio, age, cname, code, schools, classRooms, protocolType, onSuccess, onError);
    }


    //Part 4_1 Для добавления нового участника

    var operationCheck4_1 = false;

    $('#js_btnAddParty_Ext').on('click', function (event) {

        if (operationCheck4_1) return false;

        waiting('Операция выполняется. Подождите...', 0)

        operationCheck4_1 = true;

        updateRequestNewParty_Ext($(this).data('reqid'), $('#js_newFio').val(), $('#js_newAge').val(), $('#js_newAgeCategory').val(),
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

        site.DataService.UpdateRequestAddNewParty_Ext(reqid, fio, age, ageCategory, weight, kvalif, program, code, onSuccess, onError);
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

                var protocolType = ""
                if ($(this).data('protocoltype') === undefined)
                    protocolType = "";
                else
                    protocolType = $(this).data('protocoltype') ;

                updateRequestDelParty($(this).data('reqid'), $(this).data('position'), $(this).data('cname'), (protocolType == '' || protocolType == '2' ? '' : protocolType));
            }
        });

    });

    function updateRequestDelParty(reqid, position, cname, protocolType) {

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

        site.DataService.UpdateRequestDelParty(reqid, position, cname, code, protocolType, onSuccess, onError);
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

        site.DataService.UpdateRequestDelParty_Ext(reqid, position, code, onSuccess, onError);
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

        site.DataService.UpdateRequestAddNewChief(reqid, fio, position, code, onSuccess, onError);
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

        site.DataService.UpdateRequestDelChief(reqid, position, code, onSuccess, onError);
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

        site.DataService.UpdateRequestAddNewAthor(reqid, fio, code, onSuccess, onError);
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

        site.DataService.UpdateRequestDelAthor(reqid, position, code, onSuccess, onError);
    }

});