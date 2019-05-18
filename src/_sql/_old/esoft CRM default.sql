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

/* YA ESTÁN CREADOS
INSERT INTO SurveyCompletionParents (CreatedAt, [Status], PartialSave, Company_Id, Product_Id, Role_Id, Category_Id)
VALUES (GETDATE(), 'Aprobado', 0, @Company_Id, @Product_Id, 2, @Category_Id)

SET @Parent_Id = SCOPE_IDENTITY()*/

DECLARE @Survey_Id	INT = 1
DECLARE @Question VARCHAR(255)
DECLARE @Question_Id	INT

DECLARE db_cursor CURSOR FOR  
SELECT PAR.Id, PAR.Company_Id, PAR.Product_Id  FROM surveycompletionparents PAR
LEFT JOIN surveycompletions SURV on surv.parent_id = PAR.id
WHERE SURV.id IS NULL AND PAR.Category_Id = 168

OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @Parent_Id
								,@Company_Id
								,@Product_Id
								
WHILE @@FETCH_STATUS = 0   
BEGIN   

--SELECT @Company_Id = Company_Id, @Product_Id = Product_Id  FROM surveycompletionparents WHERE id = @Parent_Id


SET @Category = 'CRM General'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 168 AND 176)
SET @Survey_Id = @Category_Id

--AGREGO LAS SUBCATS CON SUS PREGUNTAS Y RESPUESTAS
INSERT INTO SurveyCompletions (SurveyId, Category, CategoryId, Email, CreatedAt, PartialSave, Company_Id, Parent_Id, Product_Id, Role_Id)
VALUES (@Survey_Id, @Category, @Category_Id, NULL, GETDATE(), 0, @Company_Id, @Parent_Id, @Product_Id, 2)

SET @SurveyCompletion_Id = SCOPE_IDENTITY()
	SET @Question = 'En qué paises puede funcionar'
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
		
		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Brasil', 1, @SurveyCompletionQuestion_Id)

		--RESPUESTA 3
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Chile', 1, @SurveyCompletionQuestion_Id)

		--RESPUESTA 4
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Colombia', 1, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Costa Rica', 1, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Cuba', 1, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Ecuador', 1, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('El Salvador', 1, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Guatemala', 1, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Honduras', 1, @SurveyCompletionQuestion_Id)

		--RESPUESTA 5
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Mexico', 1, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Nicaragua', 1, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 2
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Panamá', 1, @SurveyCompletionQuestion_Id)

		--RESPUESTA 6
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Paraguay', 1, @SurveyCompletionQuestion_Id)

		--RESPUESTA 7
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Perú', 1, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 7
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Uruguay', 1, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 7
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Venezuela', 1, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 7
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Otro', 1, @SurveyCompletionQuestion_Id)

	--PREGUNTA 2
	SET @Question = 'Empresas con diferentes requisitos contables'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)
	
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Empresas de capital nacional', 1, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Empresas de capitales extranjeros o mixtos, con reportes sobre un plan de cuentas alternativo', 2, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Empresas con capitales extranjeros o mixtos, con contabilidad bimonetaria', 3, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Empresas que requieren reexpresar su contabilidad en otra moneda', 4, @SurveyCompletionQuestion_Id)

	--Usuarios-Cuál es el target de mercado adecuado
	SET @Question = 'Usuarios-Cuál es el target de mercado adecuado'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)
	
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()
		--Entre 6 y 10 usuarios
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 6 y 10 usuarios', 3, @SurveyCompletionQuestion_Id)
		
		--Entre 11 y 20 usuarios
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 11 y 20 usuarios', 3, @SurveyCompletionQuestion_Id)
		
		--Entre 21 y 30 usuarios
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 21 y 30 usuarios', 3, @SurveyCompletionQuestion_Id)

	--Empleados-Cuál es el target de mercado adecuado
	SET @Question = 'Empleados-Cuál es el target de mercado adecuado'
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
		
		--Entre 201 y 500 empleados
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Entre 201 y 500 empleados', 1, @SurveyCompletionQuestion_Id)

	--Presupuesto-Cuál es el target de mercado adecuado (USD)
	SET @Question = 'Presupuesto-Cuál es el target de mercado adecuado (USD)'
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
		
		--Más de 1.000.000
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Más de 1.000.000', 6, @SurveyCompletionQuestion_Id)

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
		
		--Petroleo y Gas
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Petroleo y Gas', 5, @SurveyCompletionQuestion_Id)
		
		--Maderera
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Maderera', 5, @SurveyCompletionQuestion_Id)
		
		--Electricidad
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Electricidad', 5, @SurveyCompletionQuestion_Id)
		
		--Telefonía
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Telefonía', 5, @SurveyCompletionQuestion_Id)
		
		--Datos/Conectividad
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Datos/Conectividad', 5, @SurveyCompletionQuestion_Id)
		
		--Agua
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Agua', 5, @SurveyCompletionQuestion_Id)
		
		--Gas
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Gas', 5, @SurveyCompletionQuestion_Id)
		
		--Servicios Profesionales/Consultoría
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Servicios Profesionales/Consultoría', 5, @SurveyCompletionQuestion_Id)
		
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
		
		--Educación
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Educación', 5, @SurveyCompletionQuestion_Id)
		
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

--Automatización de Marketing
	--El producto soporta directamente y sin desarrollos las siguientes funcionalidades
		--Planeamiento de campañas
		--Ejecución y administración de campañas
		
SET @Category = 'Automatización de Marketing'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 168 AND 176)
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
		VALUES ('Planeamiento de campañas', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Ejecución y administración de campañas', 6, @SurveyCompletionQuestion_Id)
		
		
SET @Category = 'Automatización de fuerza de ventas'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 168 AND 176)
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
		VALUES ('Administracion de cuentas y contactos', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administración de actividades y calendarios', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administración de datos de interesados (Leads)', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administración de oportunidades (Deals)', 6, @SurveyCompletionQuestion_Id)

	SET @Question = 'Se requiere el producto de un 3ro. para integrar'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Propuestas y cuotas', 5, @SurveyCompletionQuestion_Id)
		
SET @Category = 'Servicios y Soporte al cliente'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 168 AND 176)
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
		VALUES ('Creación de nuevos casos o requerimientos de servicios', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Asignación de casos', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Escalamiento o derivación de casos no resueltos', 6, @SurveyCompletionQuestion_Id)
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Creación y mantenimiento de una base de conocimientos', 6, @SurveyCompletionQuestion_Id)

	SET @Question = 'Se requiere el producto de un 3ro. para integrar'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Autoservio on-line', 5, @SurveyCompletionQuestion_Id)
		
SET @Category = 'Administración de partners'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 168 AND 176)
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
		VALUES ('Administración de partners', 4, @SurveyCompletionQuestion_Id)		

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Portal WEB del Partner', 4, @SurveyCompletionQuestion_Id)		

SET @Category = 'Manejo de contratos'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 168 AND 176)
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
		VALUES ('Creación de contratos', 0, @SurveyCompletionQuestion_Id)		

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administración de contratos', 0, @SurveyCompletionQuestion_Id)	
		
SET @Category = 'Administración de equipos de proyecto'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 168 AND 176)
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
		VALUES ('Reasignación de equipos y miembros de proyectos', 6, @SurveyCompletionQuestion_Id)		

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administración de proyectos', 6, @SurveyCompletionQuestion_Id)	
		
SET @Category = 'Funcionalidades B2C'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 168 AND 176)
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
		VALUES ('Ventas por Internet', 5, @SurveyCompletionQuestion_Id)		

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administración de respuestas por e-mail', 5, @SurveyCompletionQuestion_Id)	
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Analíticos y BI', 5, @SurveyCompletionQuestion_Id)	
		
SET @Category = 'Otras áreas de aplicación'
SET @Category_Id = (SELECT id FROM categories WHERE name = @Category AND id BETWEEN 168 AND 176)
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
		VALUES ('e-mail marketing', 6, @SurveyCompletionQuestion_Id)		

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Software para encuestas', 6, @SurveyCompletionQuestion_Id)	
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Configurador de productos y precios', 6, @SurveyCompletionQuestion_Id)	
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Herramientas para conferencia por WEB', 6, @SurveyCompletionQuestion_Id)	
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Herramientas para movilidad', 6, @SurveyCompletionQuestion_Id)	
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administración de canales', 6, @SurveyCompletionQuestion_Id)	
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Soluciones para el retail', 6, @SurveyCompletionQuestion_Id)	
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Administración de comisiones', 6, @SurveyCompletionQuestion_Id)	
		
		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Funciones para atención de servicios en campo (Field services)', 6, @SurveyCompletionQuestion_Id)	
		
	SET @Question = 'El producto se comercializa como (Marque solo las opciones posibles)'
	SET @Question_Id = (SELECT id FROM Questions WHERE supplyquestion = @Question AND Survey_Id = @Survey_Id)

	--PREGUNTA 1
	INSERT INTO SurveyCompletionQuestions (QuestionId, Question, SurveyCompletion_Id)
	VALUES (@Question_Id, @Question, @SurveyCompletion_Id)

	SET @SurveyCompletionQuestion_Id = SCOPE_IDENTITY()

		--RESPUESTA 1
		INSERT INTO SurveyCompletionAnswers (Answer, AnswerValue, SurveyCompletionQuestion_Id)
		VALUES ('Las licencias son propiedad de la empresa cliente y el software está Instalado en los servidores de la empresa cliente (On Premise)', 1, @SurveyCompletionQuestion_Id)	

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