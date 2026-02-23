-- Compatible with MySQL Workbench visualization (generic SQL)

CREATE TABLE Comments (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    UserName        NVARCHAR(100) NOT NULL,
    Email           NVARCHAR(200) NOT NULL,
    HomePage        NVARCHAR(500) NULL,
    Text            NVARCHAR(MAX) NOT NULL,
    AttachmentPath  NVARCHAR(500) NULL,
    AttachmentType  NVARCHAR(10)  NULL CHECK (AttachmentType IN ('Image', 'Text')),
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ParentId        INT NULL REFERENCES Comments(Id) ON DELETE NO ACTION
);

CREATE INDEX IX_Comments_UserName  ON Comments(UserName);
CREATE INDEX IX_Comments_Email     ON Comments(Email);
CREATE INDEX IX_Comments_CreatedAt ON Comments(CreatedAt);
CREATE INDEX IX_Comments_ParentId  ON Comments(ParentId);