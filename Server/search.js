error_value = "No Object found to satisfy given parameters";
function GetTextBoxValue(id) {
    return document.getElementById(id).value;
}

function GetTextBoxValues(format) {
    var icao = GetTextBoxValue('icao');
    var affected = GetTextBoxValue('affected');
    var obs_type = GetTextBoxValue('obs_type');
    var elevation = GetTextBoxValue('elevation');
    var latitude = GetTextBoxValue('latitude');
    var longitude = GetTextBoxValue('longitude');
    var marking = GetTextBoxValue('marking');
    var remark = GetTextBoxValue('remark');

    return {
        icao: icao,
        affected: affected,
        obs_type: obs_type,
        elevation: elevation,
        latitude: latitude,
        longitude: longitude,
        marking: marking,
        remark: remark,
        format: format
    };
}
function SetErrorValue(str) {
    $('#error_class').text(str);
}
$('#Preview2D').click(function () {
    $.getJSON('/search', GetTextBoxValues('kml'), function (data) {
        var code = data.code;
        if (code) {
            var url = '/map.html?icao=' + code + '&name=';
            window.open(url, '_blank');
            SetErrorValue('');
        }
        else {
            SetErrorValue(error_value);
        }
    });
});

$('#Download3D').click(function () {
    $.getJSON('/search', GetTextBoxValues('kmz'), function (data) {
        var code = data.code;
        if (code) {
            var url = '/kmz/' + code;
            window.open(url, '_blank');
            SetErrorValue('');
        }
        else {
            SetErrorValue(error_value);
        }
    });
});
$('#List').click(function () {
    $.getJSON('/search', GetTextBoxValues('list'), function (array) {
        if(typeof array != "undefined" && array != null && array.length != null && array.length > 0){
            // SetErrorValue('Hello');
        }
        else {
            SetErrorValue(error_value);
        }
    });
});
