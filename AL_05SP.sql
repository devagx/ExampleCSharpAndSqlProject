if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[sp_dv_DeleteWorkPackage]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [DOCSADM].[sp_dv_DeleteWorkPackage]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[sp_dv_GetNextID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [DOCSADM].[sp_dv_GetNextID]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_dv_InsertDay]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_dv_InsertDay]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[sp_dv_InsertProject]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [DOCSADM].[sp_dv_InsertProject]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_dv_InsertTimePeriod]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_dv_InsertTimePeriod]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[sp_dv_InsertWorkPackage]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [DOCSADM].[sp_dv_InsertWorkPackage]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_dv_UpdateDay]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_dv_UpdateDay]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[sp_dv_UpdateProject]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [DOCSADM].[sp_dv_UpdateProject]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[sp_dv_UpdateTimePeriod]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[sp_dv_UpdateTimePeriod]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[sp_dv_UpdateWorkPackage]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [DOCSADM].[sp_dv_UpdateWorkPackage]
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

setuser N'DOCSADM'
GO


CREATE PROCEDURE docsadm.sp_dv_DeleteWorkPackage 
@intSystemID INT

AS

DELETE FROM docsadm.workpackage

WHERE
	[SYSTEM_ID] = @intSystemID

GO
setuser
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

setuser N'DOCSADM'
GO

CREATE PROCEDURE docsadm.sp_dv_GetNextID
@strTableName VARCHAR(255)

AS
DECLARE @intTableCount AS INT

DECLARE @intSysId AS INT

BEGIN TRANSACTION 

	SELECT @intTableCount =
	(
		SELECT COUNT (1 )
		FROM docsadm.DOCS_UNIQUE_KEYS
		WHERE TBNAME = @strTableName
	)
	
	IF  @intTableCount >0
	BEGIN
		UPDATE docsadm.DOCS_UNIQUE_KEYS
		SET LASTKEY = LASTKEY + 1
		WHERE TBNAME = @strTableName
	END
	
	ELSE
	BEGIN
		INSERT INTO docsadm.DOCS_UNIQUE_KEYS (TBNAME, LASTKEY)
		VALUES (@strTableName,1)
	END

	SELECT @intSysId =
	(
		SELECT LASTKEY
		FROM docsadm.DOCS_UNIQUE_KEYS
		WHERE TBNAME = @strTableName
	)

COMMIT TRANSACTION

SELECT @intSysId AS 'system_id'
GO
setuser
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE sp_dv_InsertDay
@dateALDate DATETIME,
@intPeopleLink INT, 
@bitOverNite BIT, 
@intNFSubLink INT


AS

DECLARE @intID INT

EXECUTE docsadm.sp_dv_GetNextID 't_dv_ALMain' , @intID OUT

INSERT INTO t_dv_ALMain
	([ID], [AL_DATE], [PEOPLE_LINK], [OVERNIGHT], [NF_SUB_LINK], [ON_COMP_LINK], [ON_PROJ_LINK]  )
VALUES 
	( @intID,@dateALDate, @intPeopleLink, @bitOverNite, @intNFSubLink, null, null )

select @intID AS 'result'
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

setuser N'DOCSADM'
GO


CREATE PROCEDURE docsadm.sp_dv_InsertProject
@strContractCode VARCHAR(255),
@strContractName VARCHAR(255),
@intCompanyID INT,
@intSystemID INT


AS


INSERT INTO docsadm.contract
	([SYSTEM_ID], [TARGET_DOCSRVR], [CONTRACT_ID], [CLIENT_ID], [CONTRACT_NAME], [COMPANY_ID])
VALUES 
	(@intSystemID,        null,                         @strContractCode,    null, @strContractName,    @intCompanyID)

select @intSystemID AS 'result'
GO
setuser
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE sp_dv_InsertTimePeriod 
@intMainALLink INT,
@strTimePeriod VARCHAR (255),
@intID INT

AS


EXECUTE docsadm.sp_dv_GetNextID 't_dv_ALTimePeriod'

INSERT INTO t_dv_ALTimePeriod
	([ID], [MAIN_AL_LINK], [TIME_PERIOD], [NOTES], [COMPANY_LINK], [PROJECT_LINK], [WORKPACKAGE_LINK], [OFF_SICK], [ON_HOLIDAY])
VALUES 
	( @intID, @intMainALLink, @strTimePeriod, ' ', null, null,null, 0, 0)

select @intID AS 'result'
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

setuser N'DOCSADM'
GO


CREATE PROCEDURE docsadm.sp_dv_InsertWorkPackage 
@intContractID INT,
@strWPName VARCHAR(255),
@intWPNumDays INT, 
@intPeopleID INT,
@intSystemID INT


AS

INSERT INTO docsadm.workpackage
	([SYSTEM_ID], [CONTRACT_ID], [WP_NAME], [WP_NUM_DAYS], [PEOPLE_ID])
VALUES 
	(@intSystemID, @intContractID, @strWPName, @intWPNumDays, @intPeopleID)

select @intSystemID AS 'result'
GO
setuser
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE sp_dv_UpdateDay
@intID INT,
@intNFSubLink INT, 
@intONCompLink INT, 
@intONProjLink INT, 
@bitOverNight BIT


AS

UPDATE t_dv_ALMain
SET	
	[NF_SUB_LINK] = @intNFSubLink,
	[ON_COMP_LINK] = @intONCompLink,
	[ON_PROJ_LINK] = @intONProjLink ,
	[OVERNIGHT] = @bitOverNight

WHERE
	[ID] = @intID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

setuser N'DOCSADM'
GO


CREATE PROCEDURE docsadm.sp_dv_UpdateProject
@intSystemID INT,
@strProjectCode VARCHAR (255),
@strProjectName VARCHAR (255)

AS

UPDATE docsadm.contract
SET	
	[CONTRACT_ID] = @strProjectCode,
	[CONTRACT_NAME] = @strProjectName
	
WHERE
	[SYSTEM_ID] = @intSystemID

GO
setuser
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO


CREATE PROCEDURE sp_dv_UpdateTimePeriod
@intID INT,
@strNotes VARCHAR (255),
@strTimePeriod VARCHAR (255), 
@bitOffSick BIT, 
@bitOnHoliday BIT


AS

UPDATE t_dv_ALTimePeriod
SET	
	[NOTES] = @strNotes,
	[TIME_PERIOD] = @strTimePeriod,
	[OFF_SICK] = @bitOffSick ,
	[ON_HOLIDAY] = @bitOnHoliday

WHERE
	[ID] = @intID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

setuser N'DOCSADM'
GO


CREATE PROCEDURE docsadm.sp_dv_UpdateWorkPackage
@intSystemID INT,
@strWPName VARCHAR(255),
@intWPNumDays INT, 
@intPeopleID INT

AS

UPDATE docsadm.workpackage
SET	
	[WP_NAME] = @strWPName,
	[WP_NUM_DAYS] = @intWPNumDays,
	[PEOPLE_ID] = @intPeopleID
WHERE
	[SYSTEM_ID] = @intSystemID

GO
setuser
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
