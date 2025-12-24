-- Create database user for managed identity and grant permissions
-- This script uses SQLCMD variables: $(ManagedIdentityName)

-- Check if user already exists, if not create it
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'$(ManagedIdentityName)')
BEGIN
    PRINT 'Creating user for managed identity: $(ManagedIdentityName)'
    CREATE USER [$(ManagedIdentityName)] FROM EXTERNAL PROVIDER;
    PRINT 'User created successfully'
END
ELSE
BEGIN
    PRINT 'User already exists: $(ManagedIdentityName)'
END
GO

-- Grant db_datareader role (read data)
IF NOT EXISTS (SELECT * FROM sys.database_role_members 
               WHERE member_principal_id = DATABASE_PRINCIPAL_ID('$(ManagedIdentityName)')
               AND role_principal_id = DATABASE_PRINCIPAL_ID('db_datareader'))
BEGIN
    PRINT 'Granting db_datareader role'
    ALTER ROLE db_datareader ADD MEMBER [$(ManagedIdentityName)];
    PRINT 'db_datareader role granted'
END
ELSE
BEGIN
    PRINT 'db_datareader role already assigned'
END
GO

-- Grant db_datawriter role (write data)
IF NOT EXISTS (SELECT * FROM sys.database_role_members 
               WHERE member_principal_id = DATABASE_PRINCIPAL_ID('$(ManagedIdentityName)')
               AND role_principal_id = DATABASE_PRINCIPAL_ID('db_datawriter'))
BEGIN
    PRINT 'Granting db_datawriter role'
    ALTER ROLE db_datawriter ADD MEMBER [$(ManagedIdentityName)];
    PRINT 'db_datawriter role granted'
END
ELSE
BEGIN
    PRINT 'db_datawriter role already assigned'
END
GO

-- Grant db_ddladmin role (run migrations)
IF NOT EXISTS (SELECT * FROM sys.database_role_members 
               WHERE member_principal_id = DATABASE_PRINCIPAL_ID('$(ManagedIdentityName)')
               AND role_principal_id = DATABASE_PRINCIPAL_ID('db_ddladmin'))
BEGIN
    PRINT 'Granting db_ddladmin role'
    ALTER ROLE db_ddladmin ADD MEMBER [$(ManagedIdentityName)];
    PRINT 'db_ddladmin role granted'
END
ELSE
BEGIN
    PRINT 'db_ddladmin role already assigned'
END
GO

-- Grant EXECUTE permission (execute stored procedures)
PRINT 'Granting EXECUTE permission'
GRANT EXECUTE TO [$(ManagedIdentityName)];
PRINT 'EXECUTE permission granted'
GO

-- Verify roles assigned
PRINT 'Roles for $(ManagedIdentityName):'
SELECT 
    dp.name AS DatabaseUserName,
    dp.type_desc AS UserType,
    drp.name AS DatabaseRole
FROM sys.database_principals dp
LEFT JOIN sys.database_role_members drm ON dp.principal_id = drm.member_principal_id
LEFT JOIN sys.database_principals drp ON drm.role_principal_id = drp.principal_id
WHERE dp.name = '$(ManagedIdentityName)'
ORDER BY dp.name, drp.name;
GO
