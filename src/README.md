# README #

DARLE BOLA A LA WIKI

configuracion del smtp, va en el web.config
<!--SMTP-->
    <add key="SMTPHost" value="mail.creapuntos.com.ar" />
    <add key="SMTPPort" value="587" />
    <add key="SMTPEnableSsl" value="true" />
    <add key="SMTPUsername" value="pruebasmtp@creapuntos.com.ar" />
    <add key="SMTPPassword" value="PablitoenShabat!" />


Las dependencias fueron removidas del proyecto, para descargarlas, se debe ingresar en la linea de comandos de nuget, dentro del visual studio el siguiente comando:
	"update-package"

Para migrar la base de datos se debe ejecutar, posicionado en el proyecto "Model":
	"update-database"