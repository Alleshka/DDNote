
create table TUser
(
id UNIQUEIDENTIFIER primary key not null,
login nvarchar(45) unique not null,
email nvarchar(100) unique check(Email like '%@%.%') not null,
password nvarchar(max) not null,
)

create table TNote
(
id UNIQUEIDENTIFIER primary key not null,
creator UNIQUEIDENTIFIER references TUser(ID) not null,
title nvarchar(45) not null,
content nvarchar(max) not null,
createDate smalldatetime not null,
updateDate smalldatetime
)

create table TCategory
(
id UNIQUEIDENTIFIER primary key not null,
userId UNIQUEIDENTIFIER references TUser(ID) not null,
Name nvarchar(45) unique not null
)

create table TShare
(
userId UNIQUEIDENTIFIER references TUser(id) not null,
noteId UNIQUEIDENTIFIER references TNote(id) not null,
primary key(userId, noteId)
)

create table TRefCategoryNote
(
noteId UNIQUEIDENTIFIER references TNote(id) not null,
categoryId UNIQUEIDENTIFIER references TCategory(id) not null
primary key (noteId, categoryId)
)

go
-- Триггер на удаление заметок
create trigger NoteDel
on TNote
instead of delete 
as 
begin 
declare @id UNIQUEIDENTIFIER = (select id from deleted)
delete TRefCategoryNote where noteId=@id -- Удаляем отношение
delete TShare where noteId=@id -- Удаляем из списка делёжек
delete TNote where id=@id -- удаляем саму заметку
end

go
create trigger CategoryDel
on TCategory
instead of delete
as 
begin
declare @id UNIQUEIDENTIFIER = (select id from deleted)
delete TRefCategoryNote where categoryId=@id -- Удаляем отношение
delete TCategory where id=@id -- Удаляем категорию
end

go
create trigger UserDel
on TUser
instead of delete
as
begin
declare @id UNIQUEIDENTIFIER = (select id from deleted)
delete TShare where userId=@id -- Удаляем все шары
delete TNote where creator=@id -- Удаляем заметки пользователя
delete TCategory where userId=@id -- Удаляем категории пользователя
delete TUser where id=@id -- Удаляем самого пользователя
end