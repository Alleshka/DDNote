create database NoteDb
go

use NoteDb
create table TUser
(
id UNIQUEIDENTIFIER primary key not null,
login nvarchar(45) unique not null,
email nvarchar(100) unique check(Email like '%@%.%') not null,
password nvarchar(max) not null,
)
go

use NoteDb
create table TNote
(
id UNIQUEIDENTIFIER primary key not null,
creator UNIQUEIDENTIFIER references TUser(ID) not null,
title nvarchar(45) not null,
content nvarchar(max) not null,
createDate smalldatetime not null,
updateDate smalldatetime
)
go

use NoteDb
create table TCategory
(
id UNIQUEIDENTIFIER primary key not null,
userId UNIQUEIDENTIFIER references TUser(ID) not null,
Name nvarchar(45) unique not null
)
go

use NoteDb
create table TShare
(
userId UNIQUEIDENTIFIER references TUser(id) not null,
noteId UNIQUEIDENTIFIER references TNote(id) not null,
primary key(userId, noteId)
)
go

use NoteDb
create table TRefCategoryNote
(
noteId UNIQUEIDENTIFIER references TNote(id) not null,
categoryId UNIQUEIDENTIFIER references TCategory(id) not null
primary key (noteId, categoryId)
)
go

create trigger NoteDel
on TNote
instead of delete 
as 
begin 
with i as (select id from deleted) delete TRefCategoryNote where noteId in (select id from i);
with i as (select id from deleted) delete TShare where noteId in (select id from i);
with i as (select id from deleted) delete TNote where id in (select id from i);
end

go
create trigger CategoryDel
on TCategory
instead of delete
as 
begin
with i as (select id from deleted) delete TRefCategoryNote where categoryId in (select id from i);
with i as (select id from deleted) delete TCategory where id in (select id from i);
end

go
create trigger UserDel
on TUser
instead of delete
as
begin
with i as (select id from deleted) delete TShare where userId in (select id from i);
with i as (select id from deleted) delete TNote where creator in (select id from i);
with i as (select id from deleted) delete TCategory where userId in (select id from i);
with i as (select id from deleted) delete TUser where id in (select id from i);
end