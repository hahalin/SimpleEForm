
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