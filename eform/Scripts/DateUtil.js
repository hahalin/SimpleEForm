
function getLocalTime()
{
    return (new Date()) - (new Date()).getTimezoneOffset() * 60000;
}

function getLocalDateStr()
{
    var res = new Date(getLocalTime()).toISOString();
    return res.slice(0, 10);
}
function getLocalDateTimeStr() {
    var res = new Date(getLocalTime()).toISOString();
    return res.slice(0, 16);
}

Date.prototype.yyyymmdd = function () {
    var mm = this.getMonth() + 1; // getMonth() is zero-based
    var dd = this.getDate();

    return [this.getFullYear(),
    (mm > 9 ? '' : '0') + mm,
    (dd > 9 ? '' : '0') + dd
    ].join('');
};

