/*select * from surveycompletionparents WHERE ID = 439
select * from surveycompletions
select * from categories
select * from surveys
select * from SurveyCompletionAnswers
select * from SurveyCompletionQuestions
select * from questions*/

BEGIN TRY
    BEGIN TRAN

DECLARE @Parent_Id	INT = 137 --> SETEAR ESTO
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
WHERE SURV.id IS NULL AND PAR.Category_Id = 141

OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @Parent_Id
								,@Company_Id
								,@Product_Id
								
WHILE @@FETCH_STATUS = 0   
BEGIN   

--SELECT @Company_Id = Company_Id, @Product_Id = Product_Id  FROM surveycompletionparents WHERE id = @Parent_Id


SET @Category = 'ERP'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
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
		VALUES ('Argentina', 1, @SurveyCompletionQuestion_Id)

		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Bolivia', 1, @SurveyCompletionQuestion_Id)

		--RESPUESTA 3
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Chile', 1, @SurveyCompletionQuestion_Id)

		--RESPUESTA 4
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Colombia', 1, @SurveyCompletionQuestion_Id)

		--RESPUESTA 5
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Mexico', 1, @SurveyCompletionQuestion_Id)

		--RESPUESTA 6
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Paraguay', 1, @SurveyCompletionQuestion_Id)

		--RESPUESTA 7
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Per�', 1, @SurveyCompletionQuestion_Id)

	--PREGUNTA 2
	SET @Question = 'Empresas con diferentes requisitos contables'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)
	
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Empresas de capital nacional', 1, @SurveyCompletionQuestion_Id)

	--Usuarios-Cu�l es el target de mercado adecuado
	SET @Question = 'Usuarios-Cu�l es el target de mercado adecuado'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)
	
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()
		--Hasta 5 usuarios
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Hasta 5 usuarios', 3, @SurveyCompletionQuestion_Id)
		
		--Entre 6 y 10 usuarios
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 6 y 10 usuarios', 3, @SurveyCompletionQuestion_Id)

	--Empleados-Cu�l es el target de mercado adecuado
	SET @Question = 'Empleados-Cu�l es el target de mercado adecuado'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)
	
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()
		--Hasta 50 empleados
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Hasta 50 empleados', 1, @SurveyCompletionQuestion_Id)
		
		--Entre 51 y 200 empleados
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 51 y 200 empleados', 1, @SurveyCompletionQuestion_Id)

	--Presupuesto-Cu�l es el target de mercado adecuado (USD)
	SET @Question = 'Presupuesto-Cu�l es el target de mercado adecuado (USD)'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)
	
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()
		--Hasta 10.000
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Hasta 10.000', 1, @SurveyCompletionQuestion_Id)
		
		--Entre 10.001 y 30.000
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 10.001 y 30.000', 2, @SurveyCompletionQuestion_Id)

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
		
		--Servicios Profesionales/Consultor�a
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Servicios Profesionales/Consultor�a', 5, @SurveyCompletionQuestion_Id)
		
		--Mantenimiento
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Mantenimiento', 5, @SurveyCompletionQuestion_Id)
		
		--Salud
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Salud', 5, @SurveyCompletionQuestion_Id)
		
		--Obras/Construcciones
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Obras/Construcciones', 5, @SurveyCompletionQuestion_Id)
		
		--Almacenes/Logistica/Distribucion
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Almacenes/Logistica/Distribucion', 5, @SurveyCompletionQuestion_Id)
		
		--Educaci�n
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Educaci�n', 5, @SurveyCompletionQuestion_Id)
		
		--Gobierno
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Gobierno', 5, @SurveyCompletionQuestion_Id)
		
		--ONG/Asociaci�n Civil
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('ONG/Asociaci�n Civil', 5, @SurveyCompletionQuestion_Id)
		
		--Banca/Finanzas
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Banca/Finanzas', 5, @SurveyCompletionQuestion_Id)


--Requerimientos para Sociedades
	--Se requiere el producto de un 3ro. para integrar
		--Multiempresa
		
SET @Category = 'Requerimientos para Sociedades'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'Se requiere el producto de un 3ro. para integrar'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Multiempresa', 5, @SurveyCompletionQuestion_Id)

--Seguridad, Integridad, Auditoria
SET @Category = 'Seguridad, Integridad, Auditoria'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

--Seguridad
	--El producto soporta directamente y sin desarrollos las siguientes funcionalidades
		--Definici�n de Usuarios y Perfiles
		
SET @Category = 'Seguridad'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'El producto soporta directamente y sin desarrollos las siguientes funcionalidades'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Definici�n de Usuarios y Perfiles', 6, @SurveyCompletionQuestion_Id)

--Auditoria
	--El producto soporta directamente y sin desarrollos las siguientes funcionalidades
		--Registra el timestamp en cada transacci�n
		
SET @Category = 'Auditoria'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'El producto soporta directamente y sin desarrollos las siguientes funcionalidades'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Registra el timestamp en cada transacci�n', 6, @SurveyCompletionQuestion_Id)

--Integridad
	--El producto soporta directamente y sin desarrollos las siguientes funcionalidades
		--Garantizar la integridad de datos mediante el uso transaccional del motor de base de datos
		
SET @Category = 'Integridad'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'El producto soporta directamente y sin desarrollos las siguientes funcionalidades'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Garantizar la integridad de datos mediante el uso transaccional del motor de base de datos', 6, @SurveyCompletionQuestion_Id)

--Pol�tica comercial
	--El producto soporta directamente y sin desarrollos las siguientes funcionalidades
		--Maneja "n" Listas de Precios
		
SET @Category = 'Pol�tica comercial'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'El producto soporta directamente y sin desarrollos las siguientes funcionalidades'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Maneja "n" Listas de Precios', 6, @SurveyCompletionQuestion_Id)
		
--M�dulo de ventas
	--El producto soporta directamente y sin desarrollos las siguientes funcionalidades
		--Permite introducir algoritmos de autorizaci�n de �rdenes de venta
		
SET @Category = 'M�dulo de ventas'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'El producto soporta directamente y sin desarrollos las siguientes funcionalidades'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Permite introducir algoritmos de autorizaci�n de �rdenes de venta', 6, @SurveyCompletionQuestion_Id)

--M�dulo de compras
	--El producto soporta directamente y sin desarrollos las siguientes funcionalidades
		--Permite definir una jerarqu�a de autorizaciones en la Orden de Compra
		
SET @Category = 'M�dulo de compras'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'El producto soporta directamente y sin desarrollos las siguientes funcionalidades'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Permite definir una jerarqu�a de autorizaciones en la Orden de Compra', 6, @SurveyCompletionQuestion_Id)

--M�dulo de Cuentas corrientes
	--El producto soporta directamente y sin desarrollos las siguientes funcionalidades
		--Permite manejo multimonetario
		
SET @Category = 'M�dulo de Cuentas corrientes'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'El producto soporta directamente y sin desarrollos las siguientes funcionalidades'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Permite manejo multimonetario', 6, @SurveyCompletionQuestion_Id)

--M�dulo de tesorer�a
	--El producto soporta directamente y sin desarrollos las siguientes funcionalidades
		--Manejo de Letras
		
SET @Category = 'M�dulo de tesorer�a'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'El producto soporta directamente y sin desarrollos las siguientes funcionalidades'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Manejo de Letras', 6, @SurveyCompletionQuestion_Id)

--M�dulo de inventarios
	--El producto soporta directamente y sin desarrollos las siguientes funcionalidades
		--Maneja estructura f�sica de los dep�sitos
		
SET @Category = 'M�dulo de inventarios'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'El producto soporta directamente y sin desarrollos las siguientes funcionalidades'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Maneja estructura f�sica de los dep�sitos', 6, @SurveyCompletionQuestion_Id)

--M�dulo de servicios
	--Funcionalidades no soportadas
		--Permite reservar items de uso de inventario para la prestaci�n de un servicio
		
SET @Category = 'M�dulo de servicios'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'Funcionalidades no soportadas'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Permite reservar items de uso de inventario para la prestaci�n de un servicio', 0, @SurveyCompletionQuestion_Id)

--M�dulo de contabilidad
	--El producto soporta directamente y sin desarrollos las siguientes funcionalidades
		--Manejo multimoneda
		
SET @Category = 'M�dulo de contabilidad'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'El producto soporta directamente y sin desarrollos las siguientes funcionalidades'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Manejo multimoneda', 6, @SurveyCompletionQuestion_Id)

--M�dulo de activos fijos
	--Se requiere personalizar el producto para
		--Seguimiento de Activos
		
SET @Category = 'M�dulo de activos fijos'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'Se requiere personalizar el producto para'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Seguimiento de Activos', 5, @SurveyCompletionQuestion_Id)

--M�dulo de importaciones
	--Funcionalidades no soportadas
		--Maneja Carpeta de Importaci�n
		
SET @Category = 'M�dulo de importaciones'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'Funcionalidades no soportadas'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Maneja Carpeta de Importaci�n', 0, @SurveyCompletionQuestion_Id)

--Control presupuestario
	--Funcionalidades no soportadas
		--Permite comparar Presupuestado vs. Real
		
SET @Category = 'Control presupuestario'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'Funcionalidades no soportadas'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Permite comparar Presupuestado vs. Real', 0, @SurveyCompletionQuestion_Id)

--M�dulo de recursos humanos
	--El producto soporta directamente y sin desarrollos las siguientes funcionalidades
		--Maneja n�mina
		
SET @Category = 'M�dulo de recursos humanos'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'El producto soporta directamente y sin desarrollos las siguientes funcionalidades'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Maneja n�mina', 6, @SurveyCompletionQuestion_Id)

--M�dulo de Pre ventas
	--Funcionalidades no soportadas
		--Maneja Clientes y Clientes Potenciales
		
SET @Category = 'M�dulo de Pre ventas'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'Funcionalidades no soportadas'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Maneja Clientes y Clientes Potenciales', 0, @SurveyCompletionQuestion_Id)

--M�dulo de Posventa y servicio t�cnico
	--Se requiere personalizar el producto para
		--Seguimiento del historial de reparaciones
		
SET @Category = 'M�dulo de Posventa y servicio t�cnico'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'Se requiere personalizar el producto para'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Seguimiento del historial de reparaciones', 4, @SurveyCompletionQuestion_Id)



--M�dulo de proyectos

SET @Category = 'M�dulo de proyectos'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'Funcionalidades no soportadas'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Manejar resultados por Proyecto', 0, @SurveyCompletionQuestion_Id)


--M�dulo de control de la producci�n

SET @Category = 'M�dulo de control de la producci�n'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'Funcionalidades no soportadas'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Producci�n por Pedido (MTO)', 0, @SurveyCompletionQuestion_Id)


--M�dulo de planeamiento de la producci�n

SET @Category = 'M�dulo de planeamiento de la producci�n'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'Funcionalidades no soportadas'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Planeamiento de Capacidad (MRP II)', 0, @SurveyCompletionQuestion_Id)


--M�dulo de Mantenimiento

SET @Category = 'M�dulo de Mantenimiento'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'Funcionalidades no soportadas'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Mantenimiento Correctivo', 0, @SurveyCompletionQuestion_Id)


--M�dulo de control de calidad

SET @Category = 'M�dulo de control de calidad'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'Funcionalidades no soportadas'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Controla Calidad de procesos productivos', 0, @SurveyCompletionQuestion_Id)


--Forma de contrataci�n

SET @Category = 'Forma de contrataci�n'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 141 AND 167)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()

	SET @Question = 'El producto se comercializa como (Marque solo las opciones posibles)'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Las licencias son propiedad de la empresa cliente y el software est� Instalado en los servidores de la empresa cliente (On Premise)', 1, @SurveyCompletionQuestion_Id)

FETCH NEXT FROM db_cursor INTO @Parent_Id
								,@Company_Id
								,@Product_Id
END   

CLOSE db_cursor   
DEALLOCATE db_cursor

COMMIT TRAN
END TRY
BEGIN CATCH
    PRINT 'In CATCH Block'
    CLOSE db_cursor   
	DEALLOCATE db_cursor
    
    IF(@@TRANCOUNT > 0)
        ROLLBACK TRAN

    SELECT   
        ERROR_NUMBER() AS ErrorNumber  
       ,ERROR_MESSAGE() AS ErrorMessage
       ,ERROR_LINE() AS ErrorLine
       ,@Question
       ,@Survey_Id
END CATCH