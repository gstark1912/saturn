
    $.validator.addMethod('dategreaterthan', function (value, element, params) {
        return true; //harcod

        var controls = element.form.elements;
        for (var i = 0, iLen = controls.length; i < iLen; i++) {
            if (controls[i].name.indexOf("Answers") > -1) {
                alert("name: " + controls[i].name);
                alert("value: " + controls[i].value);
                if (controls[i].value != "" && controls[i].value != "Seleccione...") {
                    return true;
                }
            }
        }
        return false;
    }, '');

    $.validator.unobtrusive.adapters.add('dategreaterthan', ['otherpropertyname'], function (options) {
        options.rules["dategreaterthan"] = "#surveyCompletionDTOs";
        options.messages["dategreaterthan"] = options.message;
    });

/* File Created: January 16, 2012 */

// Value is the element to be validated, params is the array of name/value pairs of the parameters extracted from the HTML, element is the HTML element that the validator is attached to
