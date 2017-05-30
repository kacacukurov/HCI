document.addEventListener('DOMContentLoaded', function () {
    $('#calendar').fullCalendar({
        defaultView: 'agendaWeek',
        allDaySlot: false,
        defaultDate: '2016-02-15',
        editable: true,
        selectable: true,
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

        resources: [
				{ id: 'a', title: 'Room A' },
				{ id: 'b', title: 'Room B', eventColor: 'green' },
				{ id: 'c', title: 'Room C', eventColor: 'orange' },
				{ id: 'd', title: 'Room D', eventColor: 'red' }
        ],
        events: [
            { id: '1', resourceId: 'b', start: '2016-02-16T09:00:00', end: '2016-02-16T11:00:00', title: 'event 1' },
            { id: '2', resourceId: 'c', start: '2016-02-15T09:00:00', end: '2016-02-15T14:00:00', title: 'event 2' },
            { id: '3', resourceId: 'd', start: '2016-02-16T09:00:00', end: '2016-02-16T14:00:00', title: 'event 2' },
            { id: '4', resourceId: 'a', start: '2016-02-17T09:00:00', end: '2016-02-17T14:00:00', title: 'event 2' },
            { id: '5', resourceId: 'd', start: '2016-02-18T09:00:00', end: '2016-02-18T14:00:00', title: 'event 2' },
            { id: '6', resourceId: 'b', start: '2016-02-19T09:00:00', end: '2016-02-19T14:00:00', title: 'event 2' }
        ]
    });
});