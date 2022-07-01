


let apiUrl = 'http://thbpoprodap-mes.delta.corp/mes_notify_prod/';
//let apiUrl = 'https://localhost:44357/';

function setCookie() {
    $.cookie('meslogtdconline_cookie', 'seaitmes', { expires: 7 });
};

function setAlert(msg, type) {
    notif({
        msg: msg,
        type: type,
        opacity: 0.8
    });
}
