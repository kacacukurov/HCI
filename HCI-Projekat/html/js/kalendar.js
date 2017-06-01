var predmetiLista = [];

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
            right: '',
            left: ''
        },
        hiddenDays: [0],
        columnFormat: 'dddd',
        groupByDateAndResource: true,
        resources: [],
        drop: function (date, jsEvent, ui, resourceId) {
            console.log('drop', date.format(), resourceId);
         /*   var event = $(this).data('eventObject');
            var copiedEvent = $.extend({}, event);
            console.log(copiedEvent);
            var predmet;
            for (var i = 0; i < predmetiLista.length; i++) {
                if (predmetiLista[i].oznaka == event.title.split('-')[0])
                    predmet = predmetiLista[i];
            }
            // we need to copy it, so that multiple events don't have a
            // reference to the same object
            var copiedEvent = $.extend({}, event);

            // assign it the date that was reported
            copiedEvent.start = date.fomat();
            var datum = event.start.format('YYYY-MM-DD HH:mm:ss').split(' ');
            var hours = parseInt((datum[1]).split(':')[0]) + parseInt(predmet.duzina);
            end = datum[0] + ' ' + hours.toString() + ":" + (datum[1]).split(':')[1];
            copiedEvent.end = end;
            copiedEventObject.allDay = false;

            // render the event on the calendar
            // the last `true` argument determines if the event "sticks"
            // (http://arshaw.com/fullcalendar/docs/event_rendering/renderEvent/)
            $(this).remove();
            $('#calendar').fullCalendar('renderEvent', copiedEventObject, true);*/
            
        },
        eventReceive: function (event) { // called when a proper external event is dropped
            console.log('eventReceive', event);
            var predmet;
            var terminaNema = false;
            for (var i = 0; i < predmetiLista.length; i++) {
                if (event.title.split('-')[0] == predmetiLista[i].oznaka) {
                    if (parseInt(predmetiLista[i].termini) == 0)
                        terminaNema = true;
                    else {
                        predmetiLista[i].termini = parseInt(predmetiLista[i].termini) - 1;
                        predmet = predmetiLista[i];
                    }
                }
            }
            if (terminaNema) {
                $('#calendar').fullCalendar('removeEvents', event._id);
                alert("Ne mozete dodati predmet!");
            }
            else {
                posaljiObjekat(event, true);
                $('#termini').text(predmet.termini);
            }
        },
        eventDrop: function (event) { // called when an event (already on the calendar) is moved
            console.log('eventDrop', event);
            posaljiObjekat(event, false);
        },
        eventDragStop: function (event, jsEvent) {

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
        }
    });
    
    cefCustomObject.posaljiPodatke();
});

function ucitajUcionice(ucionice) {
    var res = $('#calendar').fullCalendar('getResources');
    for (var j = 0; j < res.length; j++) {
        $('#calendar').fullCalendar('removeResource', res[j]);
    }
    var data = ucionice.split("|");
    for (var i = 0; i < data.length; i++) {
        if (i+1 != data.length) {
            var resource = { id: i, title: data[i] };
            $('#calendar').fullCalendar('addResource', resource);
        }
    }
}

function ucitajSmerove(smerovi) {
    var data = smerovi.split("|");
    for (var i = 0; i < data.length; i++) {
        if (i == 0) {
            $('#poljeKalendara').text($('#poljeKalendara').text().split('-')[0] + "-" + data[0]);
        }
    }
    $('#poljeKalendara').data('event', {
        title: $('#poljeKalendara').text()
    });
}

function ucitajPredmete(predmetiString) {
    $('#predmeti').empty();
    predmetiLista = JSON.parse(predmetiString).predmeti;
    for (var i = 0; i < predmetiLista.length; i++) {
        if (i == 0) {
            $('#poljeKalendara').text(predmetiLista[0].oznaka);
            cefCustomObject.dobaviSmerovePredmeta(predmetiLista[0].oznaka);
            $('#duzina').text(predmetiLista[0].duzina);
            $('#termini').text(predmetiLista[0].termini);
        }
        
        $('#predmeti').append('<option id=' + predmetiLista[i].oznaka + '>' + predmetiLista[i].oznaka + '</option>');
        
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
        if(id == predmetiLista[i].oznaka)
            $('#termini').text(predmetiLista[i].termini);
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
    if (event.end != null)
        end = event.end.format('YYYY-MM-DD HH:mm:ss');
    else {
        var datum = event.start.format('YYYY-MM-DD HH:mm:ss').split(' ');
        var hours = parseInt((datum[1]).split(':')[0]) + parseInt(predmet.duzina);
        end = datum[0] + ' ' + hours.toString() + ":" + (datum[1]).split(':')[1];
        event.end = end;
    }
    cefCustomObject.getEvent(id, title, start, end, day, ucionica, dodat);
}

function ucitajPostojecaPolja(poljaString) {
    var nesto = JSON.parse(poljaString);
    var data = nesto.lista;
    $('#calendar').fullCalendar('removeEvents');
    for (var i = 0; i < data.length; i++) {
        var event = { id: data[i].id, resourceId:data[i].ucionica, title: data[i].naziv, start: data[i].pocetak.replace(' ', 'T'), end: data[i].kraj.replace(' ', 'T') };
        $('#calendar').fullCalendar('renderEvent', event, true);
    }
}