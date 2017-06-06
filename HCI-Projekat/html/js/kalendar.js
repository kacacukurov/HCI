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
            stick: true // maintain when user navigates (see docs on the renderEvent method)
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
        defaultTimedEventDuration: '01:00:00',
        defaultDate: '2016-02-15',
        editable: true,
        selectable: true,
        droppable: true,
        eventOverlap: false,
        eventDurationEditable : false,
        minTime: "07:00:00",
        maxTime: "22:00:00",
        eventConstraint: {
            start: '07:00:00',
            end: '22:00:00'
        },
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
        eventReceive: function (event) { // poziva se kada se uradi drop novog elementa
            dodavanje = true;
            console.log('eventReceive', event);
            var predmet;
            var terminaNema = false;
            var indeks;
            //provera da li ima jos termina
            for (var i = 0; i < predmetiLista.length; i++) {
                if (event.title.split('-')[0] == predmetiLista[i].oznaka) {     //trazimo predmet koji se dodaje
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
                cefCustomObject.alert("Ne mozete dodati predmet, nema vise termina!");
            } else {        // ako ima mesta
                //provera table, projektora, mesta, pametne table
                var oznakaUcionice = event.resourceId;
                var odabranaUcionica;
                for (var i = 0; i < ucioniceLista.length; i++) {
                    if (ucioniceLista[i].oznaka == oznakaUcionice)
                        odabranaUcionica = ucioniceLista[i];    //nadjemo ucionicu u koju ju uradio drop
                }
                
                if (predmet.tabla) {
                    if (!odabranaUcionica.tabla) {
                        $('#calendar').fullCalendar('removeEvents', event._id);
                        cefCustomObject.alert("Ne mozete dodati predmet u ovu ucionicu, nema tablu!");
                    }
                }
                else if (predmet.pametnaTabla && !odabranaUcionica.pametnaTabla) {
                    $('#calendar').fullCalendar('removeEvents', event._id);
                    cefCustomObject.alert("Ne mozete dodati predmet u ovu ucionicu, nema pametnu tablu!");
                }
                else if (predmet.projektor && !odabranaUcionica.projektor) {
                    $('#calendar').fullCalendar('removeEvents', event._id);
                    cefCustomObject.alert("Ne mozete dodati predmet u ovu ucionicu, nema projektor!");
                }
                else if (parseInt(predmet.brojMesta) > parseInt(odabranaUcionica.brojMesta)) {
                    $('#calendar').fullCalendar('removeEvents', event._id);
                    cefCustomObject.alert("Ne mozete dodati predmet, ucionica nema dovoljno mesta!");
                }
                else
                {
                    var brInstaliranih = 0;
                    for (var i = 0; i < predmet.softveri.length; i++) {
                        for (var j = 0; j < odabranaUcionica.softveri.length; j++) {
                            if (odabranaUcionica.softveri[j].oznaka.trim() == predmet.softveri[i].oznaka.trim())
                                brInstaliranih++;
                        }
                    }
                    if (brInstaliranih == predmet.softveri.length) {
                        predmetiLista[indeks].termini = parseInt(predmetiLista[indeks].termini) - 1;
                        posaljiObjekat(event, true);
                        $('#termini').text(predmet.termini);
                    } else {
                        $('#calendar').fullCalendar('removeEvents', event._id);
                        cefCustomObject.alert("Ne mozete dodati predmet, ucionica nema odgovarajuće softvere!");
                    }
                }
            }
        },
        eventDrop: function (event) { // called when an event (already on the calendar) is moved
            console.log('eventDrop', event);

            for (var i = 0; i < predmetiLista.length; i++) {    //trazimo ppolje predmeta koje se pomera
                if (event.title.split('-')[0] == predmetiLista[i].oznaka) {
                    predmet = predmetiLista[i];
                }
            }

            var oznakaUcionice = event.resourceId;
            var odabranaUcionica;       //trazimo ucionicu u koju je uradio drop
            for (var i = 0; i < ucioniceLista.length; i++) {
                if (ucioniceLista[i].oznaka == oznakaUcionice)
                    odabranaUcionica = ucioniceLista[i];
            }
            var osPredmet = predmet.os.split(' ');
            var osUcionica = odabranaUcionica.os.split(' ');
            if (predmet.tabla && !odabranaUcionica.tabla) {
                $('#calendar').fullCalendar('removeEvents', event._id);
                event.resourceId = oldResourceId;
                event.start = oldStart;
                event.end = oldEnd;
                event.source = null;
                cefCustomObject.alert("Ne mozete dodati predmet u ovu ucionicu, nema tablu!");
                $('#calendar').fullCalendar('renderEvent', event, true);
            }
            else if (predmet.pametnaTabla && !odabranaUcionica.pametnaTabla) {
                $('#calendar').fullCalendar('removeEvents', event._id);
                event.resourceId = oldResourceId;
                event.start = oldStart;
                event.end = oldEnd;
                event.source = null;
                $('#calendar').fullCalendar('renderEvent', event, true);
                cefCustomObject.alert("Ne mozete dodati predmet u ovu ucionicu, nema pametnu tablu!");
            }
            else if (predmet.projektor && !odabranaUcionica.projektor) {
                $('#calendar').fullCalendar('removeEvents', event._id);
                event.resourceId = oldResourceId;
                event.start = oldStart;
                event.end = oldEnd;
                event.source = null;
                $('#calendar').fullCalendar('renderEvent', event, true);
                cefCustomObject.alert("Ne mozete dodati predmet u ovu ucionicu, nema projektor!");
            }
            else if (parseInt(predmet.brojMesta) > parseInt(odabranaUcionica.brojMesta)) {
                $('#calendar').fullCalendar('removeEvents', event._id);
                event.resourceId = oldResourceId;
                event.start = oldStart;
                event.end = oldEnd;
                event.source = null;
                $('#calendar').fullCalendar('renderEvent', event, true);
                cefCustomObject.alert("Ne mozete dodati predmet, ucionica nema dovoljno mesta!");
            }
            else {
                var brInstaliranih = 0;
                for (var i = 0; i < predmet.softveri.length; i++) {
                    for (var j = 0; j < odabranaUcionica.softveri.length; j++) {
                        if (odabranaUcionica.softveri[j].oznaka.trim() == predmet.softveri[i].oznaka.trim())
                            brInstaliranih++;
                    }
                }
                if (brInstaliranih == predmet.softveri.length) {
                    posaljiObjekat(event, false);
                } else {
                    $('#calendar').fullCalendar('removeEvents', event._id);
                    event.resourceId = oldResourceId;
                    event.start = oldStart;
                    event.end = oldEnd;
                    event.source = null;
                    $('#calendar').fullCalendar('renderEvent', event, true);
                    cefCustomObject.alert("Ne mozete dodati predmet, ucionica nema odgovarajuće softvere!");
                }
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
                if (event.title.split('-')[0] != izabraniPredmet)   //postavimo da imaju background ukoliko nisu filtrirani
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

function ucitajUcionice(ucioniceString) {
    ucioniceLista = JSON.parse(ucioniceString).ucionice;
    var res = $('#calendar').fullCalendar('getResources'); 
    for (var j = 0; j < res.length; j++) {       //brisem postojece ucionice u kalendaru
        $('#calendar').fullCalendar('removeResource', res[j]);
    }
    for (var i = 0; i < ucioniceLista.length; i++) {        //dodaj one koje su poslate sa servera
        var resource = { id: ucioniceLista[i].oznaka, title: ucioniceLista[i].oznaka, eventColor: colors[i % colors.length] };
        $('#calendar').fullCalendar('addResource', resource);
        if (ucioniceLista[i].tabla == 'True')
            ucioniceLista[i].tabla = true;
        else
            ucioniceLista[i].tabla = false;
        if (ucioniceLista[i].pametnaTabla == 'True')
            ucioniceLista[i].pametnaTabla = true;
        else
            ucioniceLista[i].pametnaTabla = false;
        if (ucioniceLista[i].projektor == 'True')
            ucioniceLista[i].projektor = true;
        else
            ucioniceLista[i].projektor = false;
    }
}

function ucitajSmerove(smerovi) {
    $('#smerovi').empty();  //dodajemo smerove da moze da ih bira
    smeroviLista = smerovi.split('|');
    for (var i = 0; i < smeroviLista.length; i++) {
        if (i + 1 != smeroviLista.length) {
            $('#smerovi').append('<option>' + smeroviLista[i] + '</option>');
        }
    }
}

function ucitajSmer(smer) {     //ovo se poziva kada se odabere predmet da bi pisao smer na eventu pored njega
    $('#poljeKalendara').text($('#poljeKalendara').text().split('-')[0] + "-" + smer);
    $('#poljeKalendara').data('event', {
        title: $('#poljeKalendara').text()
    });
}

function ucitajPredmete(predmetiString) {   //ucitavanje predmeta smera, postavljanje ispisa njegovog broja termina i duzine
    $('#predmeti').empty();
    var predmetiListaSmera = JSON.parse(predmetiString).predmeti;
    for (var i = 0; i < predmetiListaSmera.length; i++) {
        if (i == 0) {
            $('#poljeKalendara').text(predmetiListaSmera[0].oznaka);
            cefCustomObject.dobaviSmerovePredmeta(predmetiListaSmera[0].oznaka);
            $('#duzina').text(predmetiListaSmera[0].duzina);
            $('#termini').text(predmetiListaSmera[0].termini);
        }
        $('#predmeti').append('<option id=' + predmetiListaSmera[i].oznaka + '>' + predmetiListaSmera[i].oznaka + '</option>');
    }
    $('#poljeKalendara').data('event', {
        title: $('#poljeKalendara').text()
    });
}

function ucitajSvePredmete(predmetiString) {  //ucitavanje svih predmeta
    predmetiLista = JSON.parse(predmetiString).predmeti;
    for (var i = 0; i < predmetiLista.length; i++) {
        if (predmetiLista[i].tabla == 'True')
            predmetiLista[i].tabla = true;
        else
            predmetiLista[i].tabla = false;
        if (predmetiLista[i].pametnaTabla == 'True')
            predmetiLista[i].pametnaTabla = true;
        else
            predmetiLista[i].pametnaTabla = false;
        if (predmetiLista[i].projektor == 'True')
            predmetiLista[i].projektor = true;
        else
            predmetiLista[i].projektor = false;
    }
}

function ucitajNazive(nazivi) {     //puni nazivi smerova i predmeta
    var data = JSON.parse(nazivi);
    predmetiNazivi = data.predmeti;
    smeroviNazivi = data.smerovi;
}


function ucitajPostojecaPolja(poljaString) {    //ucitavanje vec postojecih polja kalendara
    var nesto = JSON.parse(poljaString);
    var data = nesto.lista;
    dodajDogadjaje(data);
}

function dodajDogadjaje(data) {     //dodavanje vec postojecih  polja u kalendar
    $('#calendar').fullCalendar('removeEvents');    //uklanjamo sva postojeca
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

function predmetPromenjen() {   //kada se odabere novi predmet u comboboxu
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

function posaljiObjekat(event, dodat) { //poziva se kada se doda novo polje ili se starom promeni mesto
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
        //provera da li se preklapa sa nekim predmetom u istom danu
        var preklapaSe = false;
        var allEvents = $('#calendar').fullCalendar('clientEvents');
        for (var i = 0; i < allEvents.length; i++) {
            if ((allEvents[i].start.format('YYYY-MM-DD').trim() == datum[0].trim()) && (allEvents[i].resourceId == drugi.resourceId)) //ako se poklapaju datum i ucionica poredimo vremena
            {
                if (allEvents[i].start.format('HH:mm:ss') > drugi.start.format('HH:mm:ss')) { //ako nivi pocinje pre starog
                    if (drugi.end.split(' ')[1] > allEvents[i].start.format('HH:mm:ss'))    // a zavrsava se posle pocetka starog
                        preklapaSe = true;
                }
            }
        }

        drugi.description = 'Predmet: ' + nazivP + ' \nSmer: ' + nazivS;
        if (!preklapaSe) {
            $('#calendar').fullCalendar('renderEvent', drugi, true);
            cefCustomObject.getEvent(id, title, start, end, day, ucionica, dodat);
        } else
            cefCustomObject.alert("Novi predmet se preklapa sa nekim postojecim zbog duzine trajanja termina!");
        
    } else {
        end = event.end.format('YYYY-MM-DD HH:mm:ss');
        cefCustomObject.getEvent(id, title, start, end, day, ucionica, dodat);
    }
}

function prikaziPoDanu() {  //kada korisnik odabere prikaz po samo jednom danu
    izabraniPredmet = "";
    izabraniSmer = "";
    var allEvents = $('#calendar').fullCalendar('clientEvents');
    for (var i = 0; i < allEvents.length; i++) {
        allEvents[i].rendering = "";    //svima skidam background, jer se ponovo postavlja u redner-u
    }
    var dan = $('#podaciFiltera').val();        //dobavimo dan
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

function prikaziCeluNedelju() {     //kada korisnik odabere da vidi celu nedelju
    izabraniPredmet = "";
    izabraniSmer = "";
    var res = $('#calendar').fullCalendar('getResources');
    for (var j = 0; j < res.length; j++) {  //uklonim sve ucionice
        res[j].rendering = "";
        $('#calendar').fullCalendar('removeResource', res[j]);
    }
    $('#calendar').fullCalendar('changeView', 'agendaWeek');
    for (var i = 0; i < ucioniceLista.length; i++) {    //opet ih dodam
        var resource = { id: ucioniceLista[i].oznaka, title: ucioniceLista[i].oznaka, eventColor: colors[i % colors.length] };
        $('#calendar').fullCalendar('addResource', resource);
    }
    $('#calendar').fullCalendar('removeEvents');    //uklonim sva polja
    cefCustomObject.ucitajPolja();  //ucitam polja
}

function prikaziPoUcionici() {  //kada korisnik odabere da prikaze po ucionici
    $('#calendar').fullCalendar('changeView', 'agendaWeek');
    var allEvents = $('#calendar').fullCalendar('clientEvents');
    for (var i = 0; i < allEvents.length; i++) {
        allEvents[i].rendering = "";
    }
    izabraniPredmet = "";
    izabraniSmer = "";
    var ucionica = $('#podaciFiltera').val();
    var res = $('#calendar').fullCalendar('getResources');
    for (var j = 0; j < res.length; j++) {  //uklonim sve ucionice koje nisu ona odabrana
        $('#calendar').fullCalendar('removeResource', res[j]);
    }
    var resource = { id: ucionica, title: ucionica };
    $('#calendar').fullCalendar('addResource', resource);
}

function prikaziPoPredmetu() {      //kada korisnik odabere prikaz po predmetu
    $('#calendar').fullCalendar('changeView', 'agendaWeek');
    var res = $('#calendar').fullCalendar('getResources');
    for (var j = 0; j < res.length; j++) {  //uklonim sve ucionice
        res[j].rendering = "";
        $('#calendar').fullCalendar('removeResource', res[j]);
    }
    $('#calendar').fullCalendar('changeView', 'agendaWeek');
    for (var i = 0; i < ucioniceLista.length; i++) {    //opet ih dodam
        var resource = { id: ucioniceLista[i].oznaka, title: ucioniceLista[i].oznaka, eventColor: colors[i % colors.length] };
        $('#calendar').fullCalendar('addResource', resource);
    }
    izabraniSmer = "";
    izabraniPredmet = $('#podaciFiltera').val();
    var allEvents = $('#calendar').fullCalendar('clientEvents');
    for (var i = 0; i < allEvents.length; i++) {
        $('#calendar').fullCalendar('renderEvent', allEvents[i]);
    }
}

function prikaziPoSmeru() {     //kada korisnik odabere prikaz po smeru
    $('#calendar').fullCalendar('changeView', 'agendaWeek');
    var res = $('#calendar').fullCalendar('getResources');
    for (var j = 0; j < res.length; j++) {  //uklonim sve ucionice
        res[j].rendering = "";
        $('#calendar').fullCalendar('removeResource', res[j]);
    }
    $('#calendar').fullCalendar('changeView', 'agendaWeek');
    for (var i = 0; i < ucioniceLista.length; i++) {    //opet ih dodam
        var resource = { id: ucioniceLista[i].oznaka, title: ucioniceLista[i].oznaka, eventColor: colors[i % colors.length] };
        $('#calendar').fullCalendar('addResource', resource);
    }
    izabraniPredmet = "";
    izabraniSmer = $('#podaciFiltera').val();
    var allEvents = $('#calendar').fullCalendar('clientEvents');
    for (var i = 0; i < allEvents.length; i++) {
        $('#calendar').fullCalendar('renderEvent', allEvents[i]);
    }
}

function smerPromenjen() {
    cefCustomObject.dobaviPredmeteSmera($('#smerovi').val());
}

function filtrirajPodatke() {       //u zavisnosti od toga koji je filter odabran, taj se poziva
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

function vrstaFilteraPromenjena() {     //kada se promeni vrsta filtera, da se menja i combobox ispod
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