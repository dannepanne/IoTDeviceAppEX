CREATE TABLE DataRecieved (
	DeviceId nvarchar(450) not null primary key,
	DeviceName nvarchar(max) not null,
	ConnectionString nvarchar(max) not null,
	StringData nvarchar(max) not null
)