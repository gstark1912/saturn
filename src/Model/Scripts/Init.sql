

INSERT INTO Roles (Id, Name, UserSelection, Discriminator) values (1, 'Admin', 0, 'ApplicationRole');
INSERT INTO Roles (Id, Name, UserSelection, Discriminator) values (2, 'Oferta', 1, 'ApplicationRole');
INSERT INTO Roles (Id, Name, UserSelection, Discriminator) values (3, 'Demanda', 1, 'ApplicationRole');

INSERT INTO Users (Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, FirstName, LastName, Discriminator)VALUES('72e7fb02-034a-4f86-8160-82a227430929', 'mgoldblat@gmail.com', 0,  'AJEtePx3o/D9D8lzUWGKPe/4JKTEEoHRm+xiFVHebfWYUQSiMzT3lhYI/F5uZhDnvw==', 'f341b16d-d815-4ff1-b3f6-d98e01ddb361',NULL,0,0,NULL, 0,0,'Admin','Martin','Goldblat','ApplicationUser');
INSERT INTO Users (Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, FirstName, LastName, Discriminator)VALUES('d12e8c21-5678-49d9-8f09-967bf7f3384c', 'mgoldblat@gmail.com', 0,  'AJEtePx3o/D9D8lzUWGKPe/4JKTEEoHRm+xiFVHebfWYUQSiMzT3lhYI/F5uZhDnvw==', 'eb89e991-7ba3-47b4-9777-9c0671ce4836',NULL,0,0,NULL, 0,0,'Demanda','Martin','Goldblat','ApplicationUser');
INSERT INTO Users (Id, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, FirstName, LastName, Discriminator)VALUES('fc2031a5-e9ae-4a5e-bbc2-15c3e027b57c', 'mgoldblat@gmail.com', 0,  'AL25b65ZaVUTFnM1HKG5lssIneTbZikm+SWSTEnu11L3KaK96C4dPWyacy89fUaIJg==', '5132d740-0698-48b4-9e74-957d4d60116e',NULL,0,0,NULL, 0,0,'Oferta','Martin','Goldblat','ApplicationUser');

INSERT INTO UsersRoles VALUES ('72e7fb02-034a-4f86-8160-82a227430929', 1, '72e7fb02-034a-4f86-8160-82a227430929');
INSERT INTO UsersRoles VALUES ('d12e8c21-5678-49d9-8f09-967bf7f3384c', 3, 'd12e8c21-5678-49d9-8f09-967bf7f3384c');
INSERT INTO UsersRoles VALUES ('fc2031a5-e9ae-4a5e-bbc2-15c3e027b57c', 2, 'fc2031a5-e9ae-4a5e-bbc2-15c3e027b57c');

insert into AnswerTypes values ('DropDownList');
insert into AnswerTypes values ('RadioButton');
insert into AnswerTypes values ('Multiple');

UPDATE USERS SET Enabled = 1, EmailConfirmed = 1;