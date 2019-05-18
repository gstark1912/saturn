// ATENCIÓN
//-------------------------------------------------------------------------------------------------------------------------------
// Se debe incluir el lamado al archivo de jquery para poder utilizar este archivo
//-------------------------------------------------------------------------------------------------------------------------------


//llamadoAWebService: Ejecuta un Servicio Web
//-------------------------------------------------------------------------------------------------------------------------------
//urlServicioWeb		  = Url del Servicio Web que serà llamado por POST (ej: )
//datos					  = Datos en formato jSon a enviar al Servicio Web indicado (ej: )
//datosServicioWeb        = Indica si el llamado al Servicio Web es asincrònico
//funcionEscenarioExitoso = Función que se ejecutarà en caso de que sea exitoso el llamado al Servicio Web indicado.
//							El paràmetro se recibe como un String, pero al realizar el eval se ejecuta como funciòn.							                            
//funcionEscenarioErroneo = Función que se ejecutarà en caso de que devuelva error el llamado al Servicio Web indicado.
//							El paràmetro se recibe como un String, pero al realizar el eval se ejecuta como funciòn.
//-------------------------------------------------------------------------------------------------------------------------------

function llamadoAjax(urlServicioWeb, datosServicioWeb, esAsincronico, funcionEscenarioExitoso, funcionEscenarioErroneo) {

    $.ajax({
        type: "POST",
        url: urlServicioWeb,
        data: datosServicioWeb,
        contentType: "application/json; charset=utf-8",
        async: esAsincronico,
        dataType: "json",
        traditional: true,
        cache: false,
        success: function (jsonDeRetorno, a) {
            var res;
            if (!jsonDeRetorno.error) {
                res = window[funcionEscenarioExitoso](jsonDeRetorno);
                return res;
            }
            else {
                res = window[funcionEscenarioErroneo](jsonDeRetorno.error);
                return res;
            }
        },
        error: function (e, a, i) {
            return window[funcionEscenarioErroneo](e.responseText);

        }
    });
}