if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_dv_DVTProjectLookup]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_dv_DVTProjectLookup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_dv_RDProjectLookup]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_dv_RDProjectLookup]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_dv_GetAccommodationDetails]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_dv_GetAccommodationDetails]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[v_dv_GetCases]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [DOCSADM].[v_dv_GetCases]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[v_dv_GetCompanies]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [DOCSADM].[v_dv_GetCompanies]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_dv_GetDays]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_dv_GetDays]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[v_dv_GetProjects]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [DOCSADM].[v_dv_GetProjects]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[v_dv_GetTechnicalPeople]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [DOCSADM].[v_dv_GetTechnicalPeople]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[v_dv_GetTechnicalSupportPeople]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [DOCSADM].[v_dv_GetTechnicalSupportPeople]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_dv_GetTimePeriods]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_dv_GetTimePeriods]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[v_dv_GetWorkPackages]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [DOCSADM].[v_dv_GetWorkPackages]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_dv_GetLocationValues]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_dv_GetLocationValues]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

 --'Other' as col, 5 (this is a default value therefore cannot change due to side effects of altering C# Class files. (frmActivityLogger.cs and TimePeriod.cs)

CREATE VIEW dbo.v_dv_GetLocationValues

AS
select TOP 100 percent col,ord from (


            select 'Home' as col, 3 as ord

            union

            select 'On Site' as col, 2 as ord

            union

            select 'Travel' as col, 4 as ord

            union

            select 'Other' as col, 5 as ord

 	    union

            select 'Office' as col, 1 as ord

) as new_table order by new_table.ord



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW dbo.v_dv_GetAccommodationDetails

AS

SELECT
	a.[ID] AS 'a_id',
	a.[NAME] AS 'a_name', 
	a.[PRICE] AS 'a_price', 
	a.[LOCATION] AS 'a_location', 
	a.[CLIENT_DISTANCE_MILES] AS 'a_client_distance_miles', 
	ISNULL(a.[PLACES_EAT_NEARBY], '') AS 'a_places_eat_nearby', 
	ISNULL(a.[PLACES_DRINK_NEARBY], '') AS 'a_places_drink_nearby', 
	ISNULL(a.[PREFFERED_TRANSPORT], '') AS 'a_preffered_transport', 
	a.[FACILITY_GAMES] AS 'a_facility_games', 
	a.[FACILITY_SKY_TV] AS 'a_facility_sky_tv', 
	a.[FACILITY_RESTAURANT] as 'a_facility_restaurant', 
	a.[FACILITY_BAR] AS 'a_facility_bar', 
	a.[FACILITY_GYM] AS 'a_facility_gym', 
	a.[FACILITY_SWIMMING_POOL] AS 'a_facility_swimming_pool', 
	a.[FACILITY_SAUNA] AS 'a_facility_sauna', 
	a.[FACILITY_POOL_TABLE] AS 'a_facility_pool_table', 
	a.[RATING_SHOWER] AS 'a_rating_shower', 
	a.[RATING_MEAL] AS 'a_rating_meal', 
	a.[RATING_OVERALL] AS 'a_rating_overall',  
	ISNULL (a.[HOTEL_URL], '') AS 'a_hotel_url',
	a.[PEOPLE_NAME] AS 'a_people_name',
	c.[SYSTEM_ID] AS 'c_system_id',
	c.[COMPANY_NAME] AS 'c_company_name',  
	r.[ID] AS 'r_id',
	r.[REVIEW_TEXT] AS 'r_review_text'
FROM
	t_dv_accommodation a INNER JOIN t_dv_accommodation_review r ON a.ID = r.accommodation_link
	INNER JOIN docsadm.company c ON c.system_id = a.company_link



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

CREATE VIEW docsadm.v_dv_GetCases
AS
SELECT  
	d.[al_date] AS d_al_date,
	d.[nf_sub_link] AS d_nf_sub_link,
	p.[system_id] AS p_system_id,
	p.[user_id] AS p_user_id,
	d.[late] AS d_late
	
FROM     
	t_dv_almain d INNER JOIN docsadm.people p ON d.[people_link] = p.[system_id]


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


CREATE VIEW docsadm.v_dv_GetCompanies
AS

SELECT   DISTINCT  
	system_id, company_name
FROM        
	DOCSADM.COMPANY



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


CREATE VIEW dbo.v_dv_GetDays
AS
SELECT    DISTINCT 
	 [ID], [AL_DATE], [PEOPLE_LINK], [OVERNIGHT], [NF_SUB_LINK], [USER_ID]
FROM        
	 dbo.t_dv_ALMain m INNER JOIN docsadm.people p ON m.people_link = p.system_id





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


CREATE VIEW docsadm.v_dv_GetProjects
AS

SELECT DISTINCT    
	 proj.[system_id] AS system_id,
	 proj.[contract_id] AS contract_id,
	 proj.[contract_name] AS contract_name,  
	 proj.[company_id] AS company_id,
	 proj.[contract_id] + '; '+ proj.[contract_name]  AS proj_details
FROM        
	 DOCSADM.CONTRACT proj




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




CREATE VIEW docsadm.v_dv_GetTechnicalPeople
AS
SELECT DISTINCT
	p.user_id, p.system_id, p.full_name
FROM 
	DOCSADM.PEOPLE p INNER JOIN DOCSADM.PEOPLEGROUPS pg ON pg.people_system_id = p.system_id
	INNER JOIN DOCSADM.GROUPS g ON pg.groups_system_id = g.system_id
WHERE 
	g.group_id = 'Technical' AND
	p.user_id NOT IN ('ALEX', 'DANNY', 'SALLY', 'STEWART', 'COLINU', 'RICHARD', 'CHRIS', 'LEE', 'DARREN', 'GARETH')


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

CREATE VIEW docsadm.v_dv_GetTechnicalSupportPeople
AS
SELECT DISTINCT
	p.user_id, p.system_id, p.full_name
FROM 
	DOCSADM.PEOPLE p INNER JOIN DOCSADM.PEOPLEGROUPS pg ON pg.people_system_id = p.system_id
	INNER JOIN DOCSADM.GROUPS g ON pg.groups_system_id = g.system_id
WHERE 
	g.group_id = 'Technical' AND
	p.user_id NOT IN ('ALEX', 'DANNY', 'SALLY', 'STEWART', 'COLINU', 'RICHARD', 'CHRIS', 'GARETH')


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

CREATE VIEW dbo.v_dv_GetTimePeriods

AS

SELECT DISTINCT

            pl.[SYSTEM_ID]           			AS pl_system_id, 
            pl.[USER_ID]     				AS pl_user_id, 

            ISNULL(com.[SYSTEM_ID], -1) 		AS com_system_id, 
            ISNULL(com.[COMPANY_NAME], '') 	AS com_company_name,

            ISNULL(com2.[SYSTEM_ID], -1) 		AS com2_system_id, 
            ISNULL(com2.[COMPANY_NAME], '') 	AS com2_company_name,

            ISNULL(proj.[SYSTEM_ID], -1) 		AS proj_system_id,
            ISNULL(proj.[CONTRACT_ID], -1) 	AS proj_contract_code,
            ISNULL(proj.[CONTRACT_NAME], '') 	AS proj_contract_name, 
            ISNULL(proj.[COMPANY_ID], -1) 		AS proj_company_id,
            proj.[contract_id] + ', '+ '"' + proj.[contract_name] + '"' AS proj_details,

            ISNULL(proj2.[SYSTEM_ID], -1) 		AS proj2_system_id,
            ISNULL(proj2.[CONTRACT_ID], -1) 	AS proj2_contract_code,
            ISNULL(proj2.[CONTRACT_NAME], '') 	AS proj2_contract_name, 
            ISNULL(proj2.[COMPANY_ID], -1) 	AS proj2_company_id,

            ISNULL(wp.[SYSTEM_ID], -1) 		AS wp_system_id, 
            ISNULL(wp.[CONTRACT_ID] ,-1)		AS wp_contract_id, 
            ISNULL(wp.[WP_NAME], '')		AS wp_wp_name, 
            ISNULL(wp.[WP_NUM_DAYS], 0) 	AS wp_wp_num_days, 
            ISNULL(wp.[PEOPLE_ID], -1) 		AS wp_people_id,

            d.[ID]					AS d_id, 
            d.[AL_DATE] 				AS d_al_date,
            d.[PEOPLE_LINK] 			AS d_people_link,
            d.[OVERNIGHT]				AS d_overnight,
            d.[LATE]                                    		AS d_late,
            d.[NF_SUB_LINK]			AS d_nf_sub_link,

            tp.[ID]					AS tp_id,
            tp.[MAIN_AL_LINK]			AS tp_main_al_link,
            tp.[TIME_PERIOD] 			AS tp_time_period,
            tp.[NOTES] 				AS tp_notes,
            tp.[OFF_SICK]				AS tp_off_sick,
            tp.[ON_HOLIDAY] 			AS tp_on_holiday,
            tp.[TIME_PERIOD] 			AS tp_label,
	tp.[IN_GUI_USE]			AS tp_in_gui_use,
	ISNULL(tp.[LOCATION],'')		AS tp_location

 
FROM

            dbo.t_dv_almain d INNER JOIN t_dv_altimeperiod tp ON d.[ID] = tp.[MAIN_AL_LINK]

                        INNER JOIN docsadm.people pl ON d.[PEOPLE_LINK] = pl.[SYSTEM_ID]

	           LEFT OUTER JOIN docsadm.company com2 ON com2.[SYSTEM_ID] = d.[ON_COMP_LINK]
	           LEFT OUTER JOIN docsadm.contract proj2 ON proj2.[SYSTEM_ID] = d.[ON_PROJ_LINK]
	  
                        LEFT OUTER JOIN docsadm.company com ON tp.[COMPANY_LINK] = com.[SYSTEM_ID]
                        LEFT OUTER JOIN docsadm.contract proj ON tp.[PROJECT_LINK] = proj.[SYSTEM_ID]
                        LEFT OUTER JOIN docsadm.workpackage wp ON tp.[WORKPACKAGE_LINK] = wp.[SYSTEM_ID]

 	        
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



CREATE VIEW docsadm.v_dv_GetWorkPackages
AS


SELECT DISTINCT     
	 wp.system_id 			AS wp_system_id,
	 wp.contract_id 			AS wp_contract_id, 
	 wp.wp_name 			AS wp_wp_name, 
	 wp.wp_num_days 		AS wp_wp_num_days, 
	 ISNULL(p.user_id, '') 		AS p_user_id, 
	 ISNULL(wp.people_id, -1) 	AS wp_people_id
FROM        
	 DOCSADM.WORKPACKAGE wp LEFT OUTER JOIN DOCSADM.PEOPLE p ON wp.people_id = p.system_id



GO
setuser
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW v_dv_DVTProjectLookup
AS

SELECT TOP 100 PERCENT *  FROM
(
	SELECT TOP 5 
		contract_id, contract_name
	FROM         
		docsadm.v_dv_GetProjects
	WHERE     
		(LEFT(CONTRACT_ID, 3) = 'DVT') AND (LEN(CONTRACT_ID) > 7)
	AND
		contract_id NOT IN('DVT 05/2000', 'DVT 05/5000')

	ORDER BY 
		CONVERT(int, RIGHT(CONTRACT_ID, LEN(CONTRACT_ID) - 7)) 
DESC
)
A
ORDER BY 
	CONVERT(int, RIGHT(CONTRACT_ID, LEN(CONTRACT_ID) - 7))


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

CREATE VIEW v_dv_RDProjectLookup
AS

SELECT TOP 100 PERCENT * FROM
(
	SELECT TOP 5
		contract_id, contract_name
	FROM         
		docsadm.v_dv_GetProjects
	WHERE     
		(LEFT(CONTRACT_ID, 3) = 'R&D') AND (LEN(CONTRACT_ID) > 4)
	ORDER BY 
		CONVERT(int, RIGHT(CONTRACT_ID, LEN(CONTRACT_ID) - 4))
	DESC
)
A
ORDER BY 
	CONVERT(int, RIGHT(CONTRACT_ID, LEN(CONTRACT_ID) - 4))



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

