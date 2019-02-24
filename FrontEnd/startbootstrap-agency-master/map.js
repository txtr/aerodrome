var kml2dBase = "kml";
var kmlNetworkLinkBase = "kml";
// Source https://github.com/heremaps/maps-api-for-javascript-examples/blob/master/map-with-interactive-kml-objects/js/app.js
function renderKML(map, ui, renderControls, icao) {
    // Create a reader object, that will load data from a KML file
    var url = kml2dBase + "/" + icao + ".KML";
    var reader = new H.data.kml.Reader(url);

    // Request document parsing. Parsing is an asynchronous operation.
    reader.parse();

    reader.addEventListener("statechange", function() {
        // Wait till the KML document is fully loaded and parsed
        if (this.getState() === H.data.AbstractReader.State.READY) {
            const parsedObjects = reader.getParsedObjects();
            // Create a group from our objects to easily zoom to them
            const container = new H.map.Group({ objects: parsedObjects });

            // Set the Map Bounds to the bounds given in the KML
            map.setViewBounds(container.getBounds());

            // Render buttons for zooming into parts of the airport.
            // Function is not a part of API. Scroll to the bottom to see the source.
            renderButtons({
                "Download in 3D": function() {
                    window.location = kmlNetworkLinkBase + "/" + icao + ".KML";
                }
            });

            // Let's make kml ballon visible by tap on its owner
            // Notice how we are using event delegation for it
            container.addEventListener(
                "tap",
                function(evt) {
                    // Let's use out custom (non-api) function for displaying a baloon
                    var bubble = new H.ui.InfoBubble(evt.target.getPosition(), {
                        // read custom data
                        content: evt.target.getData()['description']
                    });
                    // evt.target.hide();
                    console.log('Hello')
                        // show info bubble
                    console.log(ui.addBubble(bubble));
                },
                false
            );

            // Make objects visible by adding them to the map
            map.addObject(container);
        }
    });
}
/**
 * Boilerplate map initialization code starts below:
 */

// Step 1: initialize communication with the platform
const platform = new H.service.Platform({
    app_id: "iR8ykbifoBUlJW6RdfLr",
    app_code: "s76E4-KlQ79tDPa4mH1Zwg",
    useCIT: true
});
var defaultLayers = platform.createDefaultLayers();

// Step 2: initialize a map
// Please note, that default layer is set to satellite mode
var map = new H.Map(
    document.getElementById("mapContainer"),
    defaultLayers.satellite.map, {
        zoom: 7
    }
);
window.addEventListener('resize', function () {
    map.getViewPort().resize(); 
});

// Step 3: make the map interactive
// MapEvents enables the event system
// Behavior implements default interactions for pan/zoom (also on mobile touch environments)
var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));

// Template function for our controls
function renderButtons(buttons) {
    var containerNode = document.createElement("div");
    containerNode.setAttribute(
        "style",
        "position:absolute;top:0;left:0;background-color:#fff; padding:10px;"
    );
    containerNode.className = "btn-group";

    Object.keys(buttons).forEach(function(label, className) {
        var input = document.createElement("input");
        input.value = label;
        input.type = "button";
        input.onclick = buttons[label];
        input.className = className;
        containerNode.appendChild(input);
    });

    map.getElement().appendChild(containerNode);
}

var getUrlParameter = function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split("&"),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split("=");

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ?
                true :
                decodeURIComponent(sParameterName[1]);
        }
    }
};

// Step 4: create the default UI component, for displaying bubbles
var ui = H.ui.UI.createDefault(map, defaultLayers);

// Step 5: main logic goes here
function renderAirport(icao) {
    // ICAO Codes are of 4 Characters
    if (icao.length == 4) renderKML(map, ui, renderButtons, icao);
    // Go Back
    else window.history.go(-1);
}

// Get ICAO Parameter from Uri
var ICAO = getUrlParameter("icao");
renderAirport(ICAO);