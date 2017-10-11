create table TUser
(
id UNIQUEIDENTIFIER primary key not null,
login nvarchar(20) unique not null,
email nvarchar(100) unique check(Email like '%@%.%') not null,
password nvarchar(max) not null,
)

create table TNote
(
id UNIQUEIDENTIFIER primary key not null,
creator UNIQUEIDENTIFIER references TUser(ID) on delete cascade not null,
title nvarchar(20) not null,
content nvarchar(max) not null,
createDate smalldatetime not null,
updateDate smalldatetime
)

create table TCategory
(
id UNIQUEIDENTIFIER primary key not null,
userId UNIQUEIDENTIFIER references TUser(ID) on delete cascade not null,
Name nvarchar(25) unique not null
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