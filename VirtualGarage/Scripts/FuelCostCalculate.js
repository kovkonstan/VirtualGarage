
// Заменить запятую точкой
function ReplacePoint(str) {
    var index = str.indexOf(",");
    if (index != -1) {
        var resultStr = str.substr(0, index) + "." + str.substr(index + 1);
        return resultStr;
    }
    return str;
};

// Заменить точку запятой
function ReplaceComma(str) {
    var index = str.indexOf(".");
    if (index != -1) {
        var resultStr = str.substr(0, index) + "," + str.substr(index + 1);
        return resultStr;
    }
    return str;
};

function Calculate() {
    if ($("#GeneralCost").val() != "" && $("#FuelCount").val() != "" && $("#UnitCost").val() == "") {
        var generalCost = ReplacePoint($("#GeneralCost").val());
        var fuelCount = ReplacePoint($("#FuelCount").val());
        if (!isNaN(generalCost / fuelCount)) {
            var result = (generalCost / fuelCount).toString();
            $("#UnitCost").attr("value",
                        ReplaceComma((result.indexOf(".") == -1 ? result : result.substring(0, (result.indexOf(".") + 3)))));
        }
    };

    if ($("#GeneralCost").val() != "" && $("#FuelCount").val() == "" && $("#UnitCost").val() != "") {
        var generalCost = ReplacePoint($("#GeneralCost").val());
        var unitCost = ReplacePoint($("#UnitCost").val());
        if (!isNaN(generalCost / unitCost)) {
            var result = (generalCost / unitCost).toString();
            $("#FuelCount").attr("value",
                        ReplaceComma((result.indexOf(".") == -1 ? result : result.substring(0, (result.indexOf(".") + 3)))));
        }
    };

    if ($("#GeneralCost").val() == "" && $("#FuelCount").val() != "" && $("#UnitCost").val() != "") {
        var fuelCount = ReplacePoint($("#FuelCount").val());
        var unitCost = ReplacePoint($("#UnitCost").val());
        if (!isNaN(fuelCount * unitCost)) {
            var result = (fuelCount * unitCost).toString();
            $("#GeneralCost").attr("value",
                        ReplaceComma((result.indexOf(".") == -1 ? result : result.substring(0, (result.indexOf(".") + 3)))));
        }
    };
};