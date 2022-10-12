CREATE TABLE DataRecieved (
	DataId nvarchar(450) not null primary key,
	DeviceId nvarchar(450) not null,
	DeviceName nvarchar(max) not null,
	ConnectionString nvarchar(max) not null,
	StringData nvarchar(max) not null,
	TimeData nvarchar(max) not null
)