PRAGMA foreign_keys = ON;

drop table if EXISTS `User`;
CREATE TABLE `User` (
    `UserID` integer NOT NULL PRIMARY KEY,
    `UserName` text not NULL,
    `PasswordHash` text not NULL
);

drop table if EXISTS `LoginAttempt`;
CREATE TABLE `LoginAttempt` (
    `AttemptID` integer NOT NULL PRIMARY KEY,
    `SessionCookie` text NOT NULL UNIQUE,
    `UserID` integer not null,
    `AttemptTime` integer not null,
    `Success` integer not null,
    FOREIGN KEY (`UserID`) REFERENCES `User`(`UserID`)
);

drop table if EXISTS `Message`;
CREATE TABLE `Message` (
    `MessageID` INTEGER NOT NULL PRIMARY KEY,
    `ReceiverID` INTEGER NOT NULL,
    `Content` TEXT NOT NULL,
    `SentTime` INTEGER NOT NULL,
    `Delivered` INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (`ReceiverID`) REFERENCES `User`(`UserID`)
);