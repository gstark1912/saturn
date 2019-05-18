
$(document).ready(function () {
    

    $("#add").click(function () {
        var row;
        var cell;
        var hidden;
        var input;
        var button;

        var i = $(this).attr('value');
        row = $("<tr>");

        cell = $("<td>");
        hidden = $('<input type="hidden" id="id_' + i + '" name="Answers[' + i + '].Id" value="">');
        input = $('<input type="text" id="value_' + i + '" name="Answers[' + i + '].Value" value="">');
        cell.append(hidden);
        cell.append(input);
        row.append(cell);

        cell = $("<td>");
        input = $('<input type="text" id="demandAnswer_' + i + '" name="Answers[' + i + '].DemandAnswer" value="">');
        cell.append(input);
        row.append(cell);

        cell = $("<td>");
        input = $('<input type="text" id="supplyAnswer_' + i + '" name="Answers[' + i + '].SupplyAnswer" value="">');
        cell.append(input);
        row.append(cell);

        cell = $("<td>");
        button = $('<a href="#delete_' + i + '" id="delete_' + i + '" class="btn btn-danger" value="' + i + '">X</a>')
        button.click(function () {
            deleteRow(this);
        });
        cell.append(button);
        row.append(cell);

        $("#tableQuestions").append(row);
        $(this).attr('value', (parseInt($(this).attr('value')) + 1));
    })

    $("a[id^='delete_']").click(function () {
        deleteRow(this);
    })

    function deleteRow(button) {
        $(button).closest('tr').hide();
        $(button).closest('tr').find("input[id^='id_']").val(-1);
    }

});

