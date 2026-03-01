-- =====================================================
-- Database Schema for Comments Application
-- MySQL-compatible version (for MySQL Workbench visualization)
-- =====================================================

DROP TABLE IF EXISTS Comments;

CREATE TABLE Comments (
    Id              INT AUTO_INCREMENT PRIMARY KEY,
    UserName        VARCHAR(100) NOT NULL,
    Email           VARCHAR(200) NOT NULL,
    HomePage        VARCHAR(500) NULL,
    `Text`          TEXT NOT NULL,
    AttachmentPath  VARCHAR(500) NULL,
    AttachmentType  ENUM('Image', 'Text') NULL,
    CreatedAt       DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ParentId        INT NULL,

    CONSTRAINT FK_Comments_Parent
        FOREIGN KEY (ParentId) REFERENCES Comments(Id)
        ON DELETE RESTRICT,

    INDEX IX_Comments_UserName (UserName),
    INDEX IX_Comments_Email (Email),
    INDEX IX_Comments_CreatedAt (CreatedAt),
    INDEX IX_Comments_ParentId (ParentId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;