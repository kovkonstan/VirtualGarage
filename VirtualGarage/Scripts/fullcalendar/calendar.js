$(document).ready(function() {
$.fullCalendar.monthAbbrevs = ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'];
$.fullCalendar.dayNamesShort = ['0Siun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
$('#calendar').fullCalendar({
monthNames: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
monthNamesShort: ['Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн', 'Июл', 'Авг', 'Сен', 'Окт', 'Ноя', 'Дек'],
dayNames: ['Воскресенье', 'Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница','Суббота'],
dayNamesShort: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
firstDay: 1,
theme: true,
buttonText: {
today: 'Сегодня',
month: 'месяц',
week: 'день',
day: 'неделя'
},
header: {
left: 'prev,next today',
center: 'title',
right: 'month,agendaWeek'
},
editable: true,
events: "/ajax.php?act=json-load-events&user_id="+user_id,
loading: function(bool) {
if (bool)
{
$('#calendar-loading').show();
}
else
{
$('#calendar-loading').hide();
if (calendar_remind_date)
{
console.log()
if (typeof calendar_remind_date == 'string') {
calendar_remind_date = calendar_remind_date.split('-')
}
var year = calendar_remind_date[0],
month = calendar_remind_date[1]-1,
day = calendar_remind_date[2];
$('#calendar').data('fullCalendar').gotoDate(year, month, day);
}
}
},
eventDrop: function(event, dayDelta, minuteDelta, allDay, revertFunc, jsEvent, ui, view)
{
$.ajax({
url: '/ajax.php',
dataType: 'json',
data: {
act: 'json-update-event',
id: event.id,
dayDelta: dayDelta
}
})
},
dayClick: function(day)
{
new EventForm({day: day}).init();
},
eventClick: function(event)
{
new EventForm({
is_finished: event.is_finished,
remind_on: event.remind_on,
day: event.start,
remind_run: event.remind_run,
comment: event.comment,
car_id: event.car_id,
title: event.title,
reminder_title: event.reminder_title,
reminder_id: event.id,
})
}
});
}); 