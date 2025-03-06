/* Применяется в классе CompetitionsForm, на страницах голосования за конкурсные работы */

var operVotingCheck = false;
var voting_objId = '';
var voting_reqid = '';
var voting_act = '';

function voting(objId, reqid, act) {

    voting_objId = objId;
    voting_reqid = reqid;
    voting_act = act;
    captchaStart(voting_action);

    return false;
}
function voting_action(code) {

    if (!operVotingCheck) {

        operVotingCheck = true;

        function onSuccess(result, eventArgs) {
            if (result === 'ok') {
                var $obj = $(voting_objId);
                if ($obj) {
                    $obj.text(Number($obj.text()) + 1);
                }
            }
            else if (result === 'err') {
                //
            }
            operVotingCheck = false;
        }
        function onError(result) {
            operVotingCheck = false;
        }
        site.DataService.Voting(voting_reqid, voting_act, code, onSuccess, onError);

    }

}


var operSumVotingCheck = false;
var votingSum_objId = '';
var votingSum_reqid = '';

function votingSum(objId, reqid) {

    votingSum_objId = objId;
    votingSum_reqid = reqid;
    captchaStart(votingSum_action);

    return false;

}
function votingSum_action(code) {

    if (!operSumVotingCheck) {

        operSumVotingCheck = true;

        function onSuccess(result, eventArgs) {
            if (result === 'ok') {
                var $obj = $(votingSum_objId);
                if ($obj) {
                    $obj.text(Number($obj.text()) + 1);
                }
            }
            else if (result === 'err') {
                //
            }
            operSumVotingCheck = false;
        }
        function onError(result) {
            operSumVotingCheck = false;
        }
        site.DataService.VotingSum(votingSum_reqid, code, onSuccess, onError);

    }

}