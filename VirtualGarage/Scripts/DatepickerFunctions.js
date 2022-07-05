function datepickerInit() {
    $.datepicker.setDefaults(
                            $.extend($.datepicker.regional["ru"])
        			          );

    $(".datepicker").datepicker({
        changeMonth: "true",
        changeYear: "true",
        showOn: "button",
        buttonImage: "/Scripts/datepicker/calendar.gif",
        buttonImageOnly: true
    });

    if ($(".datepicker").val() == "01.01.0001 0:00:00") {
        $(".datepicker").attr("value", "");
    };
};

function StartLoader() {
    $("#ajax_loader_div").css('display', 'block');
};

function FinishLoader() {
    $("#ajax_loader_div").css('display', 'none');
};