/*
delete from companies
delete from contacts
delete from products
delete from surveycompletions
delete from rankings
delete from SurveyCompletionParents
delete from SurveyCompletionQuestions
delete from SurveyCompletionAnswers
*/

SET LANGUAGE Español

DECLARE @Company_ID INT
DECLARE @Contact_ID INT
DECLARE @ProductContact_ID INT
DECLARE @Product_ID INT
DECLARE @Parent_ID	INT
DECLARE @Category_Id INT
DECLARE @Survey_ID	INT
DECLARE @Question_ID	INT
DECLARE @DefaultQuestion_ID INT

DECLARE @userid AS VARCHAR(255)
DECLARE @pwd AS VARCHAR(255)
DECLARE @tipo AS VARCHAR(255)
DECLARE @razonSocial AS VARCHAR(255)
DECLARE @desc AS VARCHAR(255)
DECLARE @web AS VARCHAR(255)
DECLARE @direccion AS VARCHAR(255)
DECLARE @ciudad AS VARCHAR(255)
DECLARE @localidad AS VARCHAR(255)
DECLARE @pais AS VARCHAR(255)
DECLARE @mesInicio AS VARCHAR(255)
DECLARE @NumMesInicio AS INT
DECLARE @mesCierre AS VARCHAR(255)
DECLARE @NumMesCierre AS INT
DECLARE @nombreProducto AS VARCHAR(255)
DECLARE @version AS VARCHAR(255)
DECLARE @desc_prod AS VARCHAR(255)
DECLARE @web_prod AS VARCHAR(255)
DECLARE @contacto_com AS VARCHAR(255)
DECLARE @contacto_com_Cargo AS VARCHAR(255)
DECLARE @contacto_com_tel AS VARCHAR(255)
DECLARE @contacto_com_mail AS VARCHAR(255)
DECLARE @contacto_prod AS VARCHAR(255)
DECLARE @contacto_prod_cargo AS VARCHAR(255)
DECLARE @contacto_prod_tel AS VARCHAR(255)
DECLARE @contacto_prod_mail AS VARCHAR(255)


DECLARE db_cursor CURSOR FOR  
SELECT * 
FROM Migracion
WHERE contacto_com_mail IS NOT NULL

OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @userid
								,@pwd
								,@tipo
								,@razonSocial
								,@desc
								,@web
								,@direccion
								,@ciudad
								,@localidad
								,@pais
								,@mesInicio
								,@mesCierre
								,@nombreProducto
								,@version
								,@desc_prod
								,@web_prod
								,@contacto_com
								,@contacto_com_Cargo
								,@contacto_com_tel
								,@contacto_com_mail
								,@contacto_prod
								,@contacto_prod_cargo
								,@contacto_prod_tel
								,@contacto_prod_mail   

WHILE @@FETCH_STATUS = 0   
BEGIN   
	BEGIN TRY
		BEGIN TRAN
			
			IF (@tipo = 'rrhh') SET @Category_Id = 177
			IF (@tipo = 'erp') SET @Category_Id = 141
			IF (@tipo = 'crm') SET @Category_Id = 168
			
			--SET @Category_Id = 5
			SET @NumMesInicio = DATEPART(MM, @mesInicio + ' 01 2011')
			SET @NumMesCierre = DATEPART(MM, @mesCierre + ' 01 2011')
			
			IF (EXISTS(SELECT * FROM Users WHERE UserName = @contacto_com_mail)) --SI EL USUARIO YA ESTA ASOCIADO, RECUPERO EL ID
			BEGIN
				SELECT @userid = id, @Company_ID = CompanyId FROM Users WHERE UserName = @contacto_com_mail
			END
			ELSE
			BEGIN
				--INSERTO CONTACTO
				INSERT INTO Contacts (FullName
								,Position
								,Phone
								,Email)
						VALUES (@contacto_com
								,@contacto_com_Cargo
								,@contacto_com_tel
								,@contacto_com_mail)
				SET @Contact_ID = SCOPE_IDENTITY()

				--INSERTO COMPANY
				INSERT INTO Companies (CompanyName
								,CompanyDescription
								,CompanyWebSite
								,CompanyCountry
								,CompanyCity
								,CompanyAddress
								,CompanyPostalCode
								,CompanyBranchOfficesIn
								,CompanyFiscalStartDate
								,CompanyFiscalEndDate
								,CompanyPeopleInCompany
								,CompanyLogo
								,ComercialContactId
								,CompanyDomain)
						VALUES (@razonSocial
								,@desc
								,@web
								,@pais
								,@ciudad
								,@direccion
								,0
								,''
								,@NumMesInicio
								,@NumMesCierre
								,0
								,''
								,@Contact_ID
								,SUBSTRING(@contacto_prod_mail, charindex('@', @contacto_prod_mail) + 1, LEN(@contacto_prod_mail)))
				SET @Company_ID = SCOPE_IDENTITY()
				
				--ACTUALIZO
				UPDATE Users 
				SET	Email = @contacto_com_mail
					,UserName = @contacto_com_mail--SUBSTRING(@contacto_com_mail, 0, charindex('@', @contacto_com_mail))
					,FirstName = SUBSTRING(@contacto_com, 0, charindex(' ', @contacto_com))
					,LastName = SUBSTRING(@contacto_com, charindex(' ', @contacto_com) + 1, LEN(@contacto_com))
					,CompanyId = @Company_ID
				WHERE id = @userid
			END
			
			--INSERTO CONTACTO PRODUCTO
			INSERT INTO Contacts (FullName
							,Position
							,Phone
							,Email)
					VALUES (@contacto_prod
							,@contacto_prod_cargo
							,@contacto_prod_tel
							,@contacto_prod_mail )
			SET @ProductContact_ID = SCOPE_IDENTITY()
			
			--INSERTO PRODUCTO
			INSERT INTO Products ([CompanyId]
								,[Name]
								,[Version]
								,[Description]
								,[WebSite]
								,[ProductContactId]
								,[User_Id])
					VALUES (@Company_ID
							,@nombreProducto
							,@version
							,@desc_prod
							,@web_prod
							,@ProductContact_ID
							,@userid)
			SET @Product_ID = SCOPE_IDENTITY()
			
			--INSERTO SURVEY PARENT
			INSERT INTO SurveyCompletionParents (CreatedAt
											,DeletedAt
											,[Status]
											,PartialSave
											,PartialSaveKey
											,CompleteReminderSentAt
											,UpdateReminderSentAt
											,Company_Id
											,Customer_Id
											,Product_Id
											,Role_Id
											,Category_Id
											,Email)
					VALUES (GETDATE()
							,NULL
							,'Aprobado'
							,0
							,NULL
							,NULL
							,NULL
							,@Company_ID
							,NULL
							,@Product_ID
							,'2'
							,@Category_Id
							,@contacto_prod_mail)
			SET @Parent_ID = SCOPE_IDENTITY()
			
			--INSERTO SURVEY
			/*
			INSERT INTO SurveyCompletions (SurveyId
										,Category
										,CategoryId
										,Email
										,CreatedAt
										,DeletedAt
										,PartialSave
										,PartialSaveKey
										,CompleteReminderSentAt
										,UpdateReminderSentAt
										,Company_Id
										,Customer_Id
										,Parent_Id
										,Product_Id
										,Role_Id)
					VALUES (@Category_Id
							,NULL
							,@Category_Id
							,@contacto_prod_mail
							,GETDATE()
							,NULL
							,0
							,NULL
							,NULL
							,NULL
							,@Company_ID
							,NULL
							,@Parent_ID
							,@Product_ID
							,'2')
			SET @Survey_ID = SCOPE_IDENTITY()
			*/
					
			--AGREGO LA PREGUNTA
			/*IF (@Category_Id = 144) SET @DefaultQuestion_ID = 35
			IF (@Category_Id = 7) SET @DefaultQuestion_ID = 52
			IF (@Category_Id = 7) SET @DefaultQuestion_ID = 52
			INSERT INTO SurveyCompletionQuestions (QuestionId
												,Question
												,SurveyCompletion_Id)
					VALUES						(@DefaultQuestion_ID
												,(SELECT SupplyQuestion FROM questions WHERE id = @DefaultQuestion_ID) 
												,@Survey_ID)
			SET @Question_ID = SCOPE_IDENTITY()
			
			--AGREGO LA RESPUESTA
			INSERT INTO SurveyCompletionAnswers (Answer
											,AnswerValue
											,SurveyCompletionQuestion_Id)
					VALUES					('Brasil'
											,1
											,@Question_ID)*/
			
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		SELECT 'Error con: ' + @razonSocial + '  ' + ERROR_MESSAGE()
		IF(@@TRANCOUNT > 0)
			ROLLBACK TRAN
	END CATCH
	
	
	FETCH NEXT FROM db_cursor INTO @userid
								,@pwd
								,@tipo
								,@razonSocial
								,@desc
								,@web
								,@direccion
								,@ciudad
								,@localidad
								,@pais
								,@mesInicio
								,@mesCierre
								,@nombreProducto
								,@version
								,@desc_prod
								,@web_prod
								,@contacto_com
								,@contacto_com_Cargo
								,@contacto_com_tel
								,@contacto_com_mail
								,@contacto_prod
								,@contacto_prod_cargo
								,@contacto_prod_tel
								,@contacto_prod_mail 
END   

CLOSE db_cursor   
DEALLOCATE db_cursor


SET LANGUAGE us_english