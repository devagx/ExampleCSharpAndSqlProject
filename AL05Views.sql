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

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[v_dv_GetTimePeriods]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [dbo].[v_dv_GetTimePeriods]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[DOCSADM].[v_dv_GetWorkPackages]') and OBJECTPROPERTY(id, N'IsView') = 1)
drop view [DOCSADM].[v_dv_GetWorkPackages]
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
	 system_id, contract_id, contract_name,  company_id
FROM        
	 DOCSADM.CONTRACT


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
	p.user_id NOT IN ('ALEX', 'DANNY', 'SALLY', 'STEWART', 'COLINU', 'RICHARD')


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
            d.[LATE]                                    AS d_late,
            d.[NF_SUB_LINK]			AS d_nf_sub_link,

            tp.[ID]					AS tp_id,
            tp.[MAIN_AL_LINK]			AS tp_main_al_link,
            tp.[TIME_PERIOD] 			AS tp_time_period,
            tp.[NOTES] 				AS tp_notes,
            tp.[OFF_SICK]				AS tp_off_sick,
            tp.[ON_HOLIDAY] 			AS tp_on_holiday,
            tp.[TIME_PERIOD] 			AS tp_label,
	    tp.[IN_GUI_USE]			AS tp_in_gui_use

 
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

