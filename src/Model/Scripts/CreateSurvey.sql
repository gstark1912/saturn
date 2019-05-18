-----------------------------------------------------------------------------------------------
-- query config
-----------------------------------------------------------------------------------------------

declare @insertSurveyAnswer INT;
SET @insertSurveyAnswer = 0;


-----------------------------------------------------------------------------------------------
-- insert survey
-----------------------------------------------------------------------------------------------

declare @categoryId INT;
declare @surveyId INT;
declare @questionId1 INT;
declare @questionId2 INT;
declare @questionId3 INT;

insert into Categories values ('Turismo-'+ CONVERT(varchar(20), getdate()), getdate());

set @categoryId = @@IDENTITY;

insert into Surveys (CreatedAt, UpdatedAt, Category_Id) values (getdate(), getdate(), @categoryId);

set @surveyId = @@IDENTITY;

insert into Questions (SupplyQuestion, DemandQuestion, AnswerType_Id, Survey_Id) values (
	'Cuenta con salida al mar?', 
	'Cuenta con salida al mar?', 
	(select Id from AnswerTypes where name like 'DropDownList'), 
	@surveyId)

set @questionId1 = @@IDENTITY;

insert into Answers (SupplyAnswer, DemandAnswer, Value, Question_Id) values  ('SI', 'SI', 1, @questionId1);
insert into Answers (SupplyAnswer, DemandAnswer, Value, Question_Id) values  ('NO', 'NO', 2, @questionId1);

insert into Questions (SupplyQuestion, DemandQuestion, AnswerType_Id, Survey_Id) values (
	'Hay papasfirtas con katchup?', 
	'Hay papasfirtas con katchup?', 
	(select Id from AnswerTypes where name like 'RadioButton'), 
	@surveyId)

set @questionId2 = @@IDENTITY;

insert into Answers (SupplyAnswer, DemandAnswer, Value, Question_Id) values  ('SI', 'SI', 1, @questionId2);
insert into Answers (SupplyAnswer, DemandAnswer, Value, Question_Id) values  ('NO', 'NO', 2, @questionId2);

insert into Questions (SupplyQuestion, DemandQuestion, AnswerType_Id, Survey_Id) values (
	'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin consectetur pharetra faucibus. Nam tempus eros massa?',
	'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin consectetur pharetra faucibus. Nam tempus eros massa?', 
	(select Id from AnswerTypes where name like 'Multiple'), 
	@surveyId)

set @questionId3 = @@IDENTITY;

insert into Answers (SupplyAnswer, DemandAnswer, Value, Question_Id) values  ('SI', 'SI', 1, @questionId3);
insert into Answers (SupplyAnswer, DemandAnswer, Value, Question_Id) values  ('NO', 'NO', 2, @questionId3);
insert into Answers (SupplyAnswer, DemandAnswer, Value, Question_Id) values  ('Talvez', 'Maybe', 3, @questionId3);
insert into Answers (SupplyAnswer, DemandAnswer, Value, Question_Id) values  ('Quizas', 'Claro que no', 4, @questionId3);

insert into SurveyCompletionQuestions (Id,QuestionId,Question,SurveyCompletion_Id) values (1,@questionId1,'Cuenta con salida al mar?',@surveyId);
insert into SurveyCompletionQuestions (Id,QuestionId,Question,SurveyCompletion_Id) values (2,@questionId2,'Hay papasfirtas con katchup?',@surveyId);
insert into SurveyCompletionQuestions (Id,QuestionId,Question,SurveyCompletion_Id) values (3,@questionId3,'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin consectetur pharetra faucibus. Nam tempus eros massa?',@surveyId);


IF (@insertSurveyAnswer = 0)
BEGIN
	delete from SurveyCompletionAnswers;
	delete from SurveyCompletions;
END

ELSE
BEGIN 
	
-----------------------------------------------------------------------------------------------
-- insert survey completion for Oferta
-----------------------------------------------------------------------------------------------

	-- Question1

	-- SurveyCompletion
	insert into SurveyCompletions 
		(
		User_id,
		CreatedAt,
		SurveyId,
		Category,
		CategoryId,
		SurveyCompletionQuestions,
		Role_Id
		) 
		values  
		(
		(select id from Users where UserName like 'oferta'),
		getDate(),
		@surveyId,
		(select name from Categories where id = @categoryId),
		@categoryId,
		(select Id from SurveyCompletionQuestions where id = @questionId1),
		(select id from Roles where Id = 2)
		);

	-- SurveyCompletionAnswer
	insert into SurveyCompletionAnswers
		(
		Answer,
		AnswerValue
		)
		values
		(
		(Select supplyAnswer from Answers where question_id = @questionId1 and value = 1),
		(Select value from Answers where question_id = @questionId1 and value = 1)
		)

	-- Question 2

	-- SurveyCompletion
	insert into SurveyCompletions 
		(
		User_id,
		CreatedAt,
		SurveyId,
		Category,
		CategoryId,
		QuestionId,
		Question,
		Role_Id
		) 
		values  
		(
		(select id from Users where UserName like 'oferta'),
		getDate(),
		@surveyId,
		(select name from Categories where id = @categoryId),
		@categoryId,
		@questionId2,
		(select supplyQuestion from Questions where id = @questionId2),
		(select id from Roles where Id = 2)
		);

	-- SurveyCompletionAnswer
	insert into SurveyCompletionAnswers
		(
		Answer,
		AnswerValue
		)
		values
		(
		(Select supplyAnswer from Answers where question_id = @questionId2 and value = 2),
		(Select value from Answers where question_id = @questionId2 and value = 2)
		)

	-- Question3

	-- SurveyCompletion
	insert into SurveyCompletions 
		(
		User_id,
		CreatedAt,
		SurveyId,
		Category,
		CategoryId,
		QuestionId,
		Question,
		Role_id
		) 
		values  
		(
		(select id from Users where UserName like 'oferta'),
		getDate(),
		@surveyId,
		(select name from Categories where id = @categoryId),
		@categoryId,
		@questionId3,
		(select supplyQuestion from Questions where id = @questionId3),
		(select id from Roles where Id = 2)
		);

	-- SurveyCompletionAnswer
	insert into SurveyCompletionAnswers
		(
		Answer,
		AnswerValue
		)
		values
		(
		(Select supplyAnswer from Answers where question_id = @questionId3 and value = 1),
		(Select value from Answers where question_id = @questionId3 and value = 1)
		)

	insert into SurveyCompletionAnswers
		(
		Answer,
		AnswerValue
		)
		values
		(
		(Select supplyAnswer from Answers where question_id = @questionId3 and value = 4),
		(Select value from Answers where question_id = @questionId3 and value = 4)
		)

-----------------------------------------------------------------------------------------------
-- insert survey completion for Demanda
-----------------------------------------------------------------------------------------------

	-- Question1

	-- SurveyCompletion
	insert into SurveyCompletions 
		(
		User_id,
		CreatedAt,
		SurveyId,
		Category,
		CategoryId,
		QuestionId,
		Question
		) 
		values  
		(
		(select id from Users where UserName like 'demanda'),
		getDate(),
		@surveyId,
		(select name from Categories where id = @categoryId),
		@categoryId,
		@questionId1,
		(select demandQuestion from Questions where id = @questionId1)
		);

	-- SurveyCompletionAnswer
	insert into SurveyCompletionAnswers
		(
		Answer,
		AnswerValue
		)
		values
		(
		(Select demandAnswer from Answers where question_id = @questionId1 and value = 1),
		(Select value from Answers where question_id = @questionId1 and value = 1)
		)

	-- Question 2

	-- SurveyCompletion
	insert into SurveyCompletions 
		(
		User_id,
		CreatedAt,
		SurveyId,
		Category,
		CategoryId,
		QuestionId,
		Question
		) 
		values  
		(
		(select id from Users where UserName like 'oferta'),
		getDate(),
		@surveyId,
		(select name from Categories where id = @categoryId),
		@categoryId,
		@questionId2,
		(select demandQuestion from Questions where id = @questionId2)
		);

	-- SurveyCompletionAnswer
	insert into SurveyCompletionAnswers
		(
		Answer,
		AnswerValue
		)
		values
		(
		(Select demandAnswer from Answers where question_id = @questionId2 and value = 2),
		(Select value from Answers where question_id = @questionId2 and value = 2)
		)

	-- Question3

	-- SurveyCompletion
	insert into SurveyCompletions 
		(
		User_id,
		CreatedAt,
		SurveyId,
		Category,
		CategoryId,
		QuestionId,
		Question
		) 
		values  
		(
		(select id from Users where UserName like 'oferta'),
		getDate(),
		@surveyId,
		(select name from Categories where id = @categoryId),
		@categoryId,
		@questionId3,
		(select demandQuestion from Questions where id = @questionId3)
		);

	-- SurveyCompletionAnswer
	insert into SurveyCompletionAnswers
		(
		Answer,
		AnswerValue
		)
		values
		(
		(Select DemandAnswer from Answers where question_id = @questionId3 and value = 1),
		(Select value from Answers where question_id = @questionId3 and value = 1)
		)

	insert into SurveyCompletionAnswers
		(
		Answer,
		AnswerValue
		)
		values
		(
		(Select demandAnswer from Answers where question_id = @questionId3 and value = 4),
		(Select value from Answers where question_id = @questionId3 and value = 4)
		);
END

 