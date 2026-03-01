-- SQLite schema for Comments Application

CREATE TABLE IF NOT EXISTS Comments (
    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
    UserName        TEXT    NOT NULL,
    Email           TEXT    NOT NULL,
    HomePage        TEXT    NULL,
    Text            TEXT    NOT NULL,
    AttachmentPath  TEXT    NULL,
    AttachmentType  INTEGER NULL,
    CreatedAt       TEXT    NOT NULL DEFAULT (datetime('now')),
    ParentId        INTEGER NULL REFERENCES Comments(Id) ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS IX_Comments_UserName  ON Comments(UserName);
CREATE INDEX IF NOT EXISTS IX_Comments_Email     ON Comments(Email);
CREATE INDEX IF NOT EXISTS IX_Comments_CreatedAt ON Comments(CreatedAt);
CREATE INDEX IF NOT EXISTS IX_Comments_ParentId  ON Comments(ParentId);