-- Use Sql server SSIS package to import data from excel to sql server and then run this script:


SELECT * FROM SDTS_SUBMODULE_MASTER AS ssm WHERE ssm.branch_code = 'JNR'
SELECT * FROM SDTS_MODULE_MASTER AS smm WHERE smm.branch_code = 'JNR'

BEGIN TRAN 
UPDATE SubModuleMaster1 SET AngularUIRoute = NULL WHERE AngularUIRoute = 'Null'
UPDATE SubModuleMaster1 SET Icon = NULL WHERE Icon = 'Null'
UPDATE SubModuleMaster1 SET Class = NULL WHERE Class = 'Null'
UPDATE SubModuleMaster1 SET Label = NULL WHERE Label = 'Null'
UPDATE SubModuleMaster1 SET LabelClass = NULL WHERE LabelClass = 'Null'

UPDATE m
SET AngularUIRoute = ssm.AngularUIRoute, Icon = ssm.Icon, Class = ssm.Class, Label = ssm.Label, LabelClass = ssm.LabelClass
FROM SDTS_SUBMODULE_MASTER AS m JOIN SubModuleMaster1 AS ssm
ON ssm.obj_id = m.obj_id AND ssm.company_code = m.company_code



UPDATE ModuleMaster1 SET AngularUIRoute = NULL WHERE AngularUIRoute = 'Null'
UPDATE ModuleMaster1 SET Icon = NULL WHERE Icon = 'Null'
UPDATE ModuleMaster1 SET Class = NULL WHERE Class = 'Null'
UPDATE ModuleMaster1 SET Label = NULL WHERE Label = 'Null'
UPDATE ModuleMaster1 SET LabelClass = NULL WHERE LabelClass = 'Null'
UPDATE m
SET m.AngularUIRoute = mm.AngularUIRoute, m.Icon = mm.Icon, m.Class = mm.Class
FROM SDTS_MODULE_MASTER AS m JOIN ModuleMaster1 AS mm ON mm.obj_id = m.obj_id AND mm.company_code = m.company_code

COMMIT