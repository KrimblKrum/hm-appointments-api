CREATE TABLE Appointment (
	AppointmentId	varchar(36),
	ClientId		varchar(36),
	ProviderId		varchar(36),
	StartUtc		datetime,
	EndUtc			datetime,
	IsConfirmed		int,
	CreatedUtc		datetime,
    CONSTRAINT PK_Appointment PRIMARY KEY  (AppointmentId)
);
