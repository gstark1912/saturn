﻿
$(document).ready(function () {
    formRegister = $("#formRegister");
    formRegister.bind('submit',function (e) {validateNewUser(e)});
});

function validateNewUser(e) {
   var obj = new Object();
   txtNewUser = $("#txtUserName");
   obj.userName = txtNewUser.val();
   value = false;
   $.ajax({
        async:false,
        type: "POST",
        url: "/oferta/isValidNewUser",
        data: JSON.stringify(obj),
        success: function (result) {
            if (!result) {
                if ($("li#userExist").length == 0) {
                    $(".validation-summary-valid,.validation-summary-errors").addClass('validation-summary-errors').eq(0).find('ul:eq(0)').append("<li id='userExist'>El nombre de usuario ya existe.</li>")
                    $(".validation-summary-errors").removeClass('validation-summary-valid')
                }
                value = false
            } else {
                   value=true
            }
        },
        dataType: "json",
        traditional: true,
        contentType: "application/json; charset=utf-8"
    });
    if (!value) {
        e.preventDefault()
    }
}

