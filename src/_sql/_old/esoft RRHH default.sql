/*select * from surveycompletionparents WHERE ID = 439
select * from surveycompletions
select * from categories
select * from surveys
select * from SurveyCompletionAnswers
select * from SurveyCompletionQuestions
select * from questions*/

BEGIN TRY
    BEGIN TRAN

DECLARE @Parent_Id	INT --= 137 --> SETEAR ESTO
DECLARE @Company_Id INT
DECLARE @Product_Id INT
DECLARE @Category_Id INT
DECLARE @Category VARCHAR(255) = ''



DECLARE @SurveyCompletion_Id	INT
DECLARE @SurveyCompletionQuestion_Id	INT

/* YA EST�N CREADOS
INSERT INTO SurveyCompletionParents (CreatedAt, [Status], PartialSave, Company_Id, Product_Id, Role_Id, Category_Id)
VALUES (GETDATE(), 'Aprobado', 0, @Company_Id, @Product_Id, 2, @Category_Id)

SET @Parent_Id = SCOPE_IDENTITY()*/

DECLARE @Survey_Id	INT = 1
DECLARE @Question VARCHAR(255)
DECLARE @Question_Id	INT

DECLARE db_cursor CURSOR FOR  
SELECT PAR.Id, PAR.Company_Id, PAR.Product_Id  FROM surveycompletionparents PAR
LEFT JOIN surveycompletions SURV on surv.parent_id = PAR.id
WHERE SURV.id IS NULL AND PAR.Category_Id = 177

OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @Parent_Id
								,@Company_Id
								,@Product_Id
								
WHILE @@FETCH_STATUS = 0   
BEGIN   

--SELECT @Company_Id = Company_Id, @Product_Id = Product_Id  FROM surveycompletionparents WHERE id = @Parent_Id


SET @Category = 'Capital Humano'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 177 AND 177)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()
	SET @Question = 'En qu� paises puede funcionar'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Argentina', 6, @SurveyCompletionQuestion_Id)

		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Bolivia', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Brasil', 6, @SurveyCompletionQuestion_Id)

		--RESPUESTA 3
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Chile', 6, @SurveyCompletionQuestion_Id)

		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Puerto Rico', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Nicaragua', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Panam�', 6, @SurveyCompletionQuestion_Id)

		--RESPUESTA 6
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Paraguay', 6, @SurveyCompletionQuestion_Id)

		--RESPUESTA 7
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Per�', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 7
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Uruguay', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 7
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Venezuela', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 7
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Otro', 6, @SurveyCompletionQuestion_Id)

	--PREGUNTA 2
	SET @Question = 'Empresas con diferentes requisitos contables'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)
	
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Empresas de capital nacional', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Empresas de capitales extranjeros o mixtos, con reportes sobre un plan de cuentas alternativo', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Empresas con capitales extranjeros o mixtos, con contabilidad bimonetaria', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Empresas que requieren reexpresar su contabilidad en otra moneda', 6, @SurveyCompletionQuestion_Id)

	--Usuarios-Cu�l es el target de mercado adecuado
	SET @Question = 'Como se comporta el sistema de acuerdo a cantidad de usuarios'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)
	
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()
			
		--hasta 5 usuarios
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Hasta 5 usuarios', 3, @SurveyCompletionQuestion_Id)
	
		--Entre 6 y 10 usuarios
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 6 y 10 usuarios', 3, @SurveyCompletionQuestion_Id)
		
		--Entre 11 y 20 usuarios
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 11 y 20 usuarios', 3, @SurveyCompletionQuestion_Id)
	

	--Empleados-Cu�l es el target de mercado adecuado
	SET @Question = 'Empleados-Cu�l es el target de mercado adecuado'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)
	
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()
		--Hasta 50 empleados
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Hasta 50 empleados', 6, @SurveyCompletionQuestion_Id)
		
		--Entre 51 y 200 empleados
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 51 y 200 empleados', 6, @SurveyCompletionQuestion_Id)
		
		--Entre 201 y 500 empleados
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 201 y 500 empleados', 6, @SurveyCompletionQuestion_Id)

	--Presupuesto-Cu�l es el target de mercado adecuado (USD)
	SET @Question = 'Presupuesto-Cu�l es el target de mercado adecuado (USD)'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)
	
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()
		--Hasta 10.000
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Hasta 10.000', 6, @SurveyCompletionQuestion_Id)
		
		--Entre 10.001 y 30.000
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 10.001 y 30.000', 6, @SurveyCompletionQuestion_Id)
		
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 30.000 y 100.000', 6, @SurveyCompletionQuestion_Id)
		
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 100.000 y 300.000', 6, @SurveyCompletionQuestion_Id)
		
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 300.000 y 1.000.000', 6, @SurveyCompletionQuestion_Id)
		
		--M�s de 1.000.000
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('M�s de 1.000.000', 6, @SurveyCompletionQuestion_Id)

	--El producto es excelente para
	SET @Question = 'El producto es excelente para'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)
	
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()
		--Alimentos y Bebidas
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Alimentos y Bebidas', 5, @SurveyCompletionQuestion_Id)
		
		--Laboratorio Medicinal
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Laboratorio Medicinal', 5, @SurveyCompletionQuestion_Id)
		
		--Metalurgica
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Metalurgica', 5, @SurveyCompletionQuestion_Id)
		
		--Automotriz
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Automotriz', 5, @SurveyCompletionQuestion_Id)
		
		--Pl�sticos
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Pl�sticos', 5, @SurveyCompletionQuestion_Id)
		
		--ONG/Asociaci�n Civil
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('ONG/Asociaci�n Civil', 5, @SurveyCompletionQuestion_Id)
		
		--Banca/Finanzas
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Banca/Finanzas', 5, @SurveyCompletionQuestion_Id)
		
		--Forestal/Agro
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Forestal/Agro', 5, @SurveyCompletionQuestion_Id)
		
		--Edici�n/Medios/Publicidad
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Edici�n/Medios/Publicidad', 5, @SurveyCompletionQuestion_Id)
		
		--Minoristas
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Minoristas', 5, @SurveyCompletionQuestion_Id)
		
		--Mayoristas
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Mayoristas', 5, @SurveyCompletionQuestion_Id)
		
		--Retail
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Retail', 5, @SurveyCompletionQuestion_Id)
		
		--Transporte
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Transporte', 5, @SurveyCompletionQuestion_Id)
		
		--Intermediarios / Comisionistas
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Intermediarios / Comisionistas', 5, @SurveyCompletionQuestion_Id)

	SET @Question = 'El producto soporta directamente y sin desarrollos las siguientes funcionalidades'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administraci�n de pr�stamos', 5, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Liquidaci�n de sueldos/haberes', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Definici�n de pol�ticas de compensaciones', 5, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Definici�n de pol�ticas de beneficios', 5, @SurveyCompletionQuestion_Id)
		
	SET @Question = 'Funcionalidades no soportadas'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Gesti�n de pagos (Acreditaciones bancarias)', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administraci�n de ex�menes preocupacionales', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administraci�n de historias cl�nicas del personal', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Registro de accidentes laborales', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administraci�n de legajos o carpetas por empleado', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administraci�n de historia laboral', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administraci�n de licencias', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administraci�n de diferentes formas de contrataci�n', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administraci�n de sanciones', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Descripci�n de puestos', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Descripci�n de tareas', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Gesti�n de tiempos del personal (turnos)', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Control de visitas a la empresa', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Generaci�n de presupuesto', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Adninistraci�n de presupuestos', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Integraci�n entre compensaci�n y evaluaci�n', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Selecci�n de postulantes', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Capacitaci�n y desarrollo', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Evaluaci�n de desempe�o', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Cuadros de mando para el seguimiento de la ev. de desempe�o', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Planificaci�n de carrera', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Plan de sucesi�n', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Indicadores de gesti�n de talento', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Gesti�n por competencias', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Gesti�n por objetivos', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Portal de autogesti�n de empleados', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Cartelera de novedades', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Firma Digital (Recibo de Sueldos y otros documentos)', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Seguridad y auditoria', 0, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administraci�n de cargos que ocupa una persona', 0, @SurveyCompletionQuestion_Id)
		
	SET @Question = 'El producto se comercializa como (Marque solo las opciones posibles)'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)
		
	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Las licencias son propiedad de la empresa cliente y el software est� Instalado en los servidores de la empresa cliente (On Premise)', 3, @SurveyCompletionQuestion_Id)
		
FETCH NEXT FROM db_cursor INTO @Parent_Id
								,@Company_Id
								,@Product_Id
END   

CLOSE db_cursor   
DEALLOCATE db_cursor
		
COMMIT TRAN
END TRY
BEGIN CATCH
	CLOSE db_cursor   
	DEALLOCATE db_cursor
    PRINT 'In CATCH Block'
    IF(@@TRANCOUNT > 0)
        ROLLBACK TRAN

    SELECT   
        ERROR_NUMBER() AS ErrorNumber  
       ,ERROR_MESSAGE() AS ErrorMessage
       ,ERROR_LINE() AS ErrorLine
END CATCH