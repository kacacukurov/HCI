var predmetiLista = [];
var dodavanje = false;
var ucioniceLista = [];
var smeroviLista = [];
var smeroviNazivi = [];
var predmetiNazivi = [];
var izabraniPredmet = "";
var izabraniSmer = "";
var colors = ['Red', 'Blue', 'Green', 'Orange', 'Purple', 'Pink'];
var oldResourceId;
var oldStart;
var oldEnd;


document.addEventListener('DOMContentLoaded', function () {
   
    $('#external-events .fc-event').each(function () {

        // store data so the calendar knows to render an event upon drop
        $(this).data('event', {
            title: $.trim($(this).text()), // use the element's text as the event title
            stick: true, // maintain when user navigates (see docs on the renderEvent method)
        });

        // make the event draggable using jQuery UI
        $(this).draggable({
            zIndex: 999,
            revert: true,      // will cause the event to go back to its
            revertDuration: 0  //  original position after the drag
        });

    });

    $('#calendar').fullCalendar({
        forceEventDuration: true,
        allDay: false,
        height: "auto",
        defaultView: 'agendaWeek',
        allDaySlot: false,
        defaultDate: '2016-02-15',
        editable: true,
        selectable: true,
        droppable: true,
        eventOverlap: false,
        eventDurationEditable : false,
        minTime: "07:00:00",
        maxTime: "22:00:00",
        schedulerLicenseKey: 'CC-Attribution-NonCommercial-NoDerivatives',
        slotLabelFormat: "HH:mm",
        dayNames: ['Nedelja', 'Ponedeljak', 'Utorak', 'Sreda', 'Četvrtak', 'Petak', 'Subota'],
        header: {
            left: '',
            center: '',
            right: ''
        },
        hiddenDays: [0],
        columnFormat: 'dddd',
        groupByDateAndResource: true,
        resources: [],
        drop: function (date, jsEvent, ui, resourceId) {
            console.log('drop', date.format(), resourceId);
        },
        eventReceive: function (event) { // called when a proper external event is dropped
            dodavanje = true;
            console.log('eventReceive', event);
            var predmet;
            var terminaNema = false;
            var indeks;
            //provera da li ima jos termina
            for (var i = 0; i < predmetiLista.length; i++) {
                if (event.title.split('-')[0] == predmetiLista[i].oznaka) {
                    if (parseInt(predmetiLista[i].termini) == 0)
                        terminaNema = true;
                    else {
                        indeks = i;
                        predmet = predmetiLista[i];
                    }
                }
            }
            if (terminaNema) {
                $('#calendar').fullCalendar('removeEvents', event._id);
                alert("Ne mozete dodati predmet, nema vise termina!");
                return;
            }
            //provera table, projektora, mesta, pametne table
            var oznakaUcionice = event.resourceId;
            var odabranaUcionica;
            for (var i = 0; i < ucioniceLista.length; i++) {
                if (ucioniceLista[i].oznaka == oznakaUcionice)
                    odabranaUcionica = ucioniceLista[i];
            }
            var osPredmet = predmet.os.split(' ');
            var osUcionica = odabranaUcionica.os.split(' ');
            
            
            if ((predmet.tabla && !odabranaUcionica.tabla) || (predmet.pametnaTabla && !odabranaUcionica.pametnaTabla) || (predmet.projektor &&
                !odabranaUcionica.projektor)) {
                $('#calendar').fullCalendar('removeEvents', event._id);
                alert("Ne mozete dodati predmet, fali tabla, pametna tabla ili projektor!");
            }
            else if (parseInt(predmet.brojMesta) > parseInt(odabranaUcionica.brojMesta)) {
                $('#calendar').fullCalendar('removeEvents', event._id);
                alert("Ne mozete dodati predmet, ucionica nema dovoljno mesta!");
            }
            else {
                if (osPredmet.length == 3) {
                    if ((odabranaUcionica.os.indexOf(osPredmet[0]) == -1) || (odabranaUcionica.os.indexOf(osPredmet[2]) == -1)) {
                        $('#calendar').fullCalendar('removeEvents', event._id);
                        alert("Ne mozete dodati predmet, ucionica nema odgovarajuci OS!");
                    } else {
                        predmetiLista[indeks].termini = parseInt(predmetiLista[indeks].termini) - 1;
                        posaljiObjekat(event, true);
                        $('#termini').text(predmet.termini);
                    }
                }
                else if (odabranaUcionica.os.indexOf(predmet.os) == -1) {
                    $('#calendar').fullCalendar('removeEvents', event._id);
                    alert("Ne mozete dodati predmet, ucionica nema odgovarajuci OS!");
                } else {
                    predmetiLista[indeks].termini = parseInt(predmetiLista[indeks].termini) - 1;
                    posaljiObjekat(event, true);
                    $('#termini').text(predmet.termini);
                }
            }
        },
        eventDrop: function (event) { // called when an event (already on the calendar) is moved
            console.log('eventDrop', event);
            //posaljiObjekat(event, false);
            var predmet;
            //provera da li ima jos termina
            for (var i = 0; i < predmetiLista.length; i++) {
                if (event.title.split('-')[0] == predmetiLista[i].oznaka) {
                    predmet = predmetiLista[i];
                }
            }

            //provera table, projektora, mesta, pametne table
            var oznakaUcionice = event.resourceId;
            var odabranaUcionica;
            for (var i = 0; i < ucioniceLista.length; i++) {
                if (ucioniceLista[i].oznaka == oznakaUcionice)
                    odabranaUcionica = ucioniceLista[i];
            }
            var osPredmet = predmet.os.split(' ');
            var osUcionica = odabranaUcionica.os.split(' ');
            if ((predmet.tabla && !odabranaUcionica.tabla) || (predmet.pametnaTabla && !odabranaUcionica.pametnaTabla) || (predmet.projektor &&
                 !odabranaUcionica.projektor)) {
                event.resourceId = oldResourceId;
                event.start = oldStart;
                event.end = oldEnd;
                $('#calendar').fullCalendar('renderEvent', event, true);
                alert("Ne mozete dodati predmet, fali tabla, pametna tabla ili projektor!");
            }
            else if (parseInt(predmet.brojMesta) > parseInt(odabranaUcionica.brojMesta)) {
                event.resourceId = oldResourceId;
                event.start = oldStart;
                event.end = oldEnd;
                $('#calendar').fullCalendar('renderEvent', event, true);
                alert("Ne mozete dodati predmet, ucionica nema dovoljno mesta!");
            }
            else if (odabranaUcionica.os.indexOf(predmet.os) == -1) {
                event.resourceId = oldResourceId;
                event.start = oldStart;
                event.end = oldEnd;
                $('#calendar').fullCalendar('renderEvent', event, true);
                alert("Ne mozete dodati predmet, ucionica nema odgovarajuci OS!");
            }
            else {
                posaljiObjekat(event, true);
            }
        },
        eventDragStop: function (event, jsEvent) {
            oldResourceId = event.resourceId;
            oldStart = event.start;
            oldEnd = event.end;
            var trashEl = jQuery('#calendarTrash');
            var ofs = trashEl.offset();

            var x1 = ofs.left;
            var x2 = ofs.left + trashEl.outerWidth(true);
            var y1 = ofs.top;
            var y2 = ofs.top + trashEl.outerHeight(true);

            if (jsEvent.pageX >= x1 && jsEvent.pageX <= x2 &&
                jsEvent.pageY >= y1 && jsEvent.pageY <= y2) {
                $('#calendar').fullCalendar('removeEvents', event._id);
                var predmet;
                for (var i = 0; i < predmetiLista.length; i++) {
                    if (predmetiLista[i].oznaka == event.title.split('-')[0]) { //prolazim kroz sve predmete da bih nasla onaj ciji event brisem
                        predmetiLista[i].termini++;
                        predmet = predmetiLista[i];
                    }
                }
                cefCustomObject.obrisiPolje(event._id, event.title.split('-')[0]);
                var select = document.getElementById("predmeti");
                var oznakaPredmeta = select.options[select.selectedIndex].id;
                if(event.title.split('-')[0] == oznakaPredmeta)
                    $('#termini').text(predmet.termini);
            }
        }, eventRender: function (event, element) {
            element.qtip({
                show: 'click',
                hide: {
                    distance: 20,
                    leave: true
                },
                content: event.description
                
            });
            if (izabraniPredmet != "") {
                if (event.title.split('-')[0] != izabraniPredmet)
                    event.rendering = "background";
                else
                    event.rendering = "";
            }

            if (izabraniSmer != "") {
                if (event.title.split('-')[1] != izabraniSmer)
                    event.rendering = "background";
                else
                    event.rendering = "";
            }
        }
    });
    cefCustomObject.posaljiPodatke();
});

function ucitajUcionice(ucionice) {
    ucioniceLista = JSON.parse(ucionice).ucionice;
    $('#ucionice').empty();
    var res = $('#calendar').fullCalendar('getResources');
    for (var j = 0; j < res.length; j++) {
        $('#calendar').fullCalendar('removeResource', res[j]);
    }
    for (var i = 0; i < ucioniceLista.length; i++) {
        var resource = { id: ucioniceLista[i].oznaka, title: ucioniceLista[i].oznaka, eventColor: colors[i % colors.length] };
        $('#calendar').fullCalendar('addResource', resource);
        $('#ucionice').append('<option id=' + ucioniceLista[i].oznaka + '>' + ucioniceLista[i].oznaka + '</option>');
    }
}

function ucitajSmerove(smerovi) {
    $('#smerFilter').empty();
    $('#smerovi').empty();
    smeroviLista = smerovi.split('|');
    for (var i = 0; i < smeroviLista.length; i++) {
        if (i + 1 != smeroviLista.length) {
            $('#smerFilter').append('<option>' + smeroviLista[i] + '</option>');
            $('#smerovi').append('<option>' + smeroviLista[i] + '</option>');
        }
    }
}

function ucitajSmer(smer) {
    $('#poljeKalendara').text($('#poljeKalendara').text().split('-')[0] + "-" + smer);
    $('#poljeKalendara').data('event', {
        title: $('#poljeKalendara').text()
    });
}

function ucitajPredmete(predmetiString) {
    $('#predmeti').empty();
    $('#predmetiFilter').empty();
    console.log(predmetiString);
    predmetiLista = JSON.parse(predmetiString).predmeti;
    console.log(predmetiLista);
    for (var i = 0; i < predmetiLista.length; i++) {
        if (i == 0) {
            $('#poljeKalendara').text(predmetiLista[0].oznaka);
            cefCustomObject.dobaviSmerovePredmeta(predmetiLista[0].oznaka);
            $('#duzina').text(predmetiLista[0].duzina);
            $('#termini').text(predmetiLista[0].termini);
        }
        $('#predmeti').append('<option id=' + predmetiLista[i].oznaka + '>' + predmetiLista[i].oznaka + '</option>');
        $('#predmetiFilter').append('<option id=' + predmetiLista[i].oznaka + '>' + predmetiLista[i].oznaka + '</option>');
    }
    $('#poljeKalendara').data('event', {
        title: $('#poljeKalendara').text()
    });
}

function predmetPromenjen() {
    var select = document.getElementById("predmeti");
    var id = select.options[select.selectedIndex].id;
    cefCustomObject.dobaviSmerovePredmeta(id);
    $('#poljeKalendara').text(id + '-' + $('#poljeKalendara').text().split('-')[1]);
    $('#poljeKalendara').data('event', {
        title: $('#poljeKalendara').text()
    });
    for (var i = 0; i < predmetiLista.length; i++) {
        if (id == predmetiLista[i].oznaka) {
            $('#termini').text(predmetiLista[i].termini);
            $('#duzina').text(predmetiLista[i].duzina);
        }
    }
}

function posaljiObjekat(event, dodat) {
    var predmet;
    for (var i = 0; i < predmetiLista.length; i++) {
        if (predmetiLista[i].oznaka == event.title.split('-')[0])
            predmet = predmetiLista[i];
    }
    var id = event._id;
    var ucionica = event.resourceId;
    var title = event.title;
    var start = event.start.format('YYYY-MM-DD HH:mm:ss');
    var day = event.start.format('dddd');
    var end;
    if (dodavanje) {
        dodavanje = false;
        var drugi = event;
        $('#calendar').fullCalendar('removeEvents', event._id);
        var datum = event.start.format('YYYY-MM-DD HH:mm:ss').split(' ');
        var hours = parseInt((datum[1]).split(':')[0]) + parseInt(predmet.duzina);
        if (hours < 10)
            end = datum[0] + ' 0' + hours.toString() + ":" + (datum[1]).split(':')[1] + ':00';
        else
            end = datum[0] + ' ' + hours.toString() + ":" + (datum[1]).split(':')[1] + ':00';
        console.log(end);
        drugi.end = end;
        var nazivP = "";
        var nazivS = "";
        for (var j = 0; j < predmetiNazivi.length; j++) {
            if (drugi.title.split('-')[0] == predmetiNazivi[j].oznaka)
                nazivP = predmetiNazivi[j].naziv;
        }
        for (var j = 0; j < smeroviNazivi.length; j++) {
            if (drugi.title.split('-')[1] == smeroviNazivi[j].oznaka)
                nazivS = smeroviNazivi[j].naziv;
        }
        drugi.description = 'Predmet: ' + nazivP + ' \nSmer: ' + nazivS;
        $('#calendar').fullCalendar('renderEvent', drugi, true);
        cefCustomObject.getEvent(id, title, start, end, day, ucionica, dodat);
    } else {
        end = event.end.format('YYYY-MM-DD HH:mm:ss');
        cefCustomObject.getEvent(id, title, start, end, day, ucionica, dodat);
    }
    
   
}

function ucitajPostojecaPolja(poljaString) {
    var nesto = JSON.parse(poljaString);
    var data = nesto.lista;
    dodajDogadjaje(data);
}

function dodajDogadjaje(data) {
    $('#calendar').fullCalendar('removeEvents');
    for (var i = 0; i < data.length; i++) {
        var nazivP = "";
        var nazivS = "";
        for (var j = 0; j < predmetiNazivi.length; j++) {
            if (data[i].naziv.split('-')[0] == predmetiNazivi[j].oznaka)
                nazivP = predmetiNazivi[j].naziv;
        }
        for (var j = 0; j < smeroviNazivi.length; j++) {
            if (data[i].naziv.split('-')[1] == smeroviNazivi[j].oznaka)
                nazivS = smeroviNazivi[j].naziv;
        }
        var event = {
            id: data[i].id, resourceId: data[i].ucionica, title: data[i].naziv, start: data[i].pocetak.replace(' ', 'T'),
            end: data[i].kraj.replace(' ', 'T'), description: 'Predmet: ' + nazivP + ' \nSmer: ' + nazivS, rendering: ""
        };
        $('#calendar').fullCalendar('renderEvent', event, true);
    }
}

function prikaziPoDanu() {
    izabraniPredmet = "";
    izabraniSmer = "";
    var allEvents = $('#calendar').fullCalendar('clientEvents');
    for (var i = 0; i < allEvents.length; i++) {
        allEvents[i].rendering = "";
    }
    var dan = $('#podaciFiltera').val();
    if(dan == 'Ponedeljak')
        $('#calendar').fullCalendar('changeView', 'agendaDay', '2016-02-15');
    else if(dan == 'Utorak')
        $('#calendar').fullCalendar('changeView', 'agendaDay', '2016-02-16');
    else if(dan == 'Sreda')
        $('#calendar').fullCalendar('changeView', 'agendaDay', '2016-02-17');
    else if(dan == 'Četvrtak')
        $('#calendar').fullCalendar('changeView', 'agendaDay', '2016-02-18');
    else if(dan == 'Petak')
        $('#calendar').fullCalendar('changeView', 'agendaDay', '2016-02-19');
    else if(dan == 'Subota')
        $('#calendar').fullCalendar('changeView', 'agendaDay', '2016-02-20');
}

function prikaziCeluNedelju() {
    izabraniPredmet = "";
    izabraniSmer = "";
    var res = $('#calendar').fullCalendar('getResources');
    for (var j = 0; j < res.length; j++) {
        res[j].rendering = "";
        $('#calendar').fullCalendar('removeResource', res[j]);
    }
    $('#calendar').fullCalendar('changeView', 'agendaWeek');
    for (var i = 0; i < ucioniceLista.length; i++) {
        var resource = { id: ucioniceLista[i].oznaka, title: ucioniceLista[i].oznaka, eventColor: colors[i % colors.length] };
        $('#calendar').fullCalendar('addResource', resource);
        
    }
    cefCustomObject.ucitajPolja();
}

function prikaziPoUcionici() {
    $('#calendar').fullCalendar('changeView', 'agendaWeek');
    var allEvents = $('#calendar').fullCalendar('clientEvents');
    for (var i = 0; i < allEvents.length; i++) {
        allEvents[i].rendering = "";
    }

    izabraniPredmet = "";
    izabraniSmer = "";
    var ucionica = $('#podaciFiltera').val();
    var res = $('#calendar').fullCalendar('getResources');
    var i;
    for (var j = 0; j < res.length; j++) {
        $('#calendar').fullCalendar('removeResource', res[j]);
    }
    for (var k = 0; k < ucioniceLista.length; k++) {
        if (ucioniceLista[k] == ucionica)
            i = k;
    }

    var resource = { id: ucionica, title: ucionica };
    $('#calendar').fullCalendar('addResource', resource);
}

function prikaziPoPredmetu() {
    $('#calendar').fullCalendar('changeView', 'agendaWeek');
    izabraniSmer = "";
    izabraniPredmet = $('#podaciFiltera').val();
    var allEvents = $('#calendar').fullCalendar('clientEvents');
    for (var i = 0; i < allEvents.length; i++) {
        $('#calendar').fullCalendar('renderEvent', allEvents[i]);
        
    }
}

function prikaziPoSmeru() {
    $('#calendar').fullCalendar('changeView', 'agendaWeek');
    izabraniPredmet = "";
    izabraniSmer = $('#podaciFiltera').val();
    var allEvents = $('#calendar').fullCalendar('clientEvents');
    for (var i = 0; i < allEvents.length; i++) {
        $('#calendar').fullCalendar('renderEvent', allEvents[i]);
    }
}

function ucitajNazive(nazivi) {
    var data = JSON.parse(nazivi);
    predmetiNazivi = data.predmeti;
    smeroviNazivi = data.smerovi;
}

function smerPromenjen() {
    cefCustomObject.dobaviPredmeteSmera($('#smerovi').val());
}

function filtrirajPodatke() {
    var vrstaFiltera = $('#vrstaFiltera').val();
    if (vrstaFiltera == "Nedelja") {
        prikaziCeluNedelju();
    } else if (vrstaFiltera == "Dan") {
        prikaziPoDanu();
    } else if (vrstaFiltera == "Ucionica") {
        prikaziPoUcionici();
    } else if (vrstaFiltera == "Smer") {
        prikaziPoSmeru();
    } else if (vrstaFiltera == "Predmet") {
        prikaziPoPredmetu();
    }
}

function vrstaFilteraPromenjena() {
    $('#podaciFiltera').empty();
    var vrstaFiltera = $('#vrstaFiltera').val();
    if (vrstaFiltera == "Dan") {
        $('#podaciFiltera').append('<option>Ponedeljak</option>');
        $('#podaciFiltera').append('<option>Utorak</option>');
        $('#podaciFiltera').append('<option>Sreda</option>');
        $('#podaciFiltera').append('<option>Četvrtak</option>');
        $('#podaciFiltera').append('<option>Petak</option>');
        $('#podaciFiltera').append('<option>Subota</option>');
    } else if (vrstaFiltera == "Ucionica") {
        for (var i = 0; i < ucioniceLista.length; i++) {
            $('#podaciFiltera').append('<option>' + ucioniceLista[i].oznaka + '</option>');
        }

    } else if (vrstaFiltera == "Smer") {
        for (var i = 0; i < smeroviLista.length; i++) {
            if(i + 1 != smeroviLista.length)
                $('#podaciFiltera').append('<option>' + smeroviLista[i] + '</option>');
        }
    } else if (vrstaFiltera == "Predmet") {
        for (var i = 0; i < predmetiLista.length; i++) {
            $('#podaciFiltera').append('<option>' + predmetiLista[i].oznaka + '</option>');
        }
    }
}