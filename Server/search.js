function GetTextBoxValue(id) {
    return document.getElementById(id).value;
}

function GetValues(type) {
    var icao = GetTextBoxValue('icao');
    var affected = GetTextBoxValue('affected');
    var obs_type = GetTextBoxValue('obs_type');
    var elevation = GetTextBoxValue('elevation');
    var latitude = GetTextBoxValue('latitude');
    var longitude = GetTextBoxValue('longitude');
    var marking = GetTextBoxValue('marking');
    var remark = GetTextBoxValue('remark');

    return { icao: icao, affected: affected, obs_type: obs_type, elevation: elevation, latitude: latitude, longitude: longitude, marking: marking, remark: remark, data_type: type };
}
$('#Preview2D').click(function() {
    $.getJSON('/search', GetValues('kml'), function(data) { console.log(data); });
})