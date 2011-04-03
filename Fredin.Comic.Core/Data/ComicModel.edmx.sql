
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 05/19/2010 14:49:57
-- Generated from EDMX file: D:\Dev\Fredin\Fredin.Comic.Web\Models\ComicModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Comic];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Comic_Template]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Comic] DROP CONSTRAINT [FK_Comic_Template];
GO
IF OBJECT_ID(N'[dbo].[FK_Comic_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Comic] DROP CONSTRAINT [FK_Comic_User];
GO
IF OBJECT_ID(N'[dbo].[FK_ComicPhoto_Comic]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ComicPhoto] DROP CONSTRAINT [FK_ComicPhoto_Comic];
GO
IF OBJECT_ID(N'[dbo].[FK_ComicPhoto_Photo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ComicPhoto] DROP CONSTRAINT [FK_ComicPhoto_Photo];
GO
IF OBJECT_ID(N'[dbo].[FK_ComicPhoto_TemplateItem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ComicPhoto] DROP CONSTRAINT [FK_ComicPhoto_TemplateItem];
GO
IF OBJECT_ID(N'[dbo].[FK_ComicTextBubble_Comic]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ComicTextBubble] DROP CONSTRAINT [FK_ComicTextBubble_Comic];
GO
IF OBJECT_ID(N'[dbo].[FK_ComicTextBubble_TextBubbleDirection]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ComicTextBubble] DROP CONSTRAINT [FK_ComicTextBubble_TextBubbleDirection];
GO
IF OBJECT_ID(N'[dbo].[FK_Photo_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Photo] DROP CONSTRAINT [FK_Photo_User];
GO
IF OBJECT_ID(N'[dbo].[FK_TemplateItem_Template]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TemplateItem] DROP CONSTRAINT [FK_TemplateItem_Template];
GO
IF OBJECT_ID(N'[dbo].[FK_TextBubbleDirection_TextBubble]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TextBubbleDirection] DROP CONSTRAINT [FK_TextBubbleDirection_TextBubble];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Comic]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Comic];
GO
IF OBJECT_ID(N'[dbo].[ComicPhoto]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ComicPhoto];
GO
IF OBJECT_ID(N'[dbo].[ComicTextBubble]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ComicTextBubble];
GO
IF OBJECT_ID(N'[dbo].[Photo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Photo];
GO
IF OBJECT_ID(N'[dbo].[Template]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Template];
GO
IF OBJECT_ID(N'[dbo].[TemplateItem]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TemplateItem];
GO
IF OBJECT_ID(N'[dbo].[TextBubble]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TextBubble];
GO
IF OBJECT_ID(N'[dbo].[TextBubbleDirection]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TextBubbleDirection];
GO
IF OBJECT_ID(N'[dbo].[User]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Comics'
CREATE TABLE [dbo].[Comics] (
    [ComicId] bigint IDENTITY(1,1) NOT NULL,
    [Uid] bigint  NOT NULL,
    [TemplateId] bigint  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [UpdateTime] datetime  NOT NULL,
    [PublishTime] datetime  NULL,
    [FeatureTime] datetime  NULL,
    [IsPublished] bit  NOT NULL,
    [Title] varchar(100)  NOT NULL,
    [Description] varchar(max)  NOT NULL,
    [ShareText] varchar(100)  NULL,
    [IsPrivate] bit  NOT NULL,
    [IsTemporary] bit  NOT NULL,
    [ImageStorageKey] varchar(255)  NULL,
    [ThumbStorageKey] varchar(255)  NULL,
    [IsDeleted] bit  NOT NULL
);
GO

-- Creating table 'ComicPhotoes'
CREATE TABLE [dbo].[ComicPhotoes] (
    [ComicPhotoId] bigint IDENTITY(1,1) NOT NULL,
    [ComicId] bigint  NOT NULL,
    [PhotoId] bigint  NOT NULL,
    [TemplateItemId] bigint  NOT NULL
);
GO

-- Creating table 'ComicTextBubbles'
CREATE TABLE [dbo].[ComicTextBubbles] (
    [ComicTextBubbleId] bigint IDENTITY(1,1) NOT NULL,
    [ComicId] bigint  NOT NULL,
    [TextBubbleDirectionId] bigint  NOT NULL,
    [Text] varchar(max)  NOT NULL,
    [X] int  NOT NULL,
    [Y] int  NOT NULL
);
GO

-- Creating table 'Photos'
CREATE TABLE [dbo].[Photos] (
    [PhotoId] bigint IDENTITY(1,1) NOT NULL,
    [Uid] bigint  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [StorageKey] varchar(255)  NOT NULL
);
GO

-- Creating table 'Templates'
CREATE TABLE [dbo].[Templates] (
    [TemplateId] bigint IDENTITY(1,1) NOT NULL,
    [Width] int  NOT NULL,
    [Height] int  NOT NULL,
    [Rows] tinyint  NOT NULL,
    [Columns] tinyint  NOT NULL,
    [Ordinal] int  NOT NULL,
    [Description] varchar(100)  NOT NULL,
    [ThumbUrl] varchar(255)  NOT NULL,
    [IsDeleted] bit  NOT NULL
);
GO

-- Creating table 'TemplateItems'
CREATE TABLE [dbo].[TemplateItems] (
    [TemplateItemId] bigint IDENTITY(1,1) NOT NULL,
    [TemplateId] bigint  NOT NULL,
    [X] int  NOT NULL,
    [Y] int  NOT NULL,
    [Width] int  NOT NULL,
    [Height] int  NOT NULL,
    [Ordinal] int  NOT NULL
);
GO

-- Creating table 'TextBubbles'
CREATE TABLE [dbo].[TextBubbles] (
    [TextBubbleId] bigint IDENTITY(1,1) NOT NULL,
    [Title] varchar(50)  NOT NULL,
    [BaseScaleX] int  NOT NULL,
    [BaseScaleY] int  NOT NULL,
    [TextScaleX] int  NOT NULL,
    [TextScaleY] int  NOT NULL,
    [StorageKey] varchar(255)  NOT NULL,
    [IsDeleted] bit  NOT NULL
);
GO

-- Creating table 'TextBubbleDirections'
CREATE TABLE [dbo].[TextBubbleDirections] (
    [TextBubbleDirectionId] bigint  NOT NULL,
    [TextBubbleId] bigint  NOT NULL,
    [Direction] varchar(2)  NOT NULL,
    [StorageKey] varchar(255)  NOT NULL,
    [IsDeleted] bit  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Uid] bigint  NOT NULL,
    [Email] varchar(max)  NULL,
    [Name] varchar(50)  NULL,
    [Nickname] varchar(50)  NULL,
    [IsDeleted] bit  NOT NULL,
    [ProfileLink] varchar(max)  NULL
);
GO

-- Creating table 'UserSubscriptions'
CREATE TABLE [dbo].[UserSubscriptions] (
    [Uid] bigint IDENTITY(1,1) NOT NULL,
    [SubscriptionType] nvarchar(max)  NOT NULL,
    [Token] nvarchar(max)  NOT NULL,
    [Users_Uid] bigint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ComicId] in table 'Comics'
ALTER TABLE [dbo].[Comics]
ADD CONSTRAINT [PK_Comics]
    PRIMARY KEY CLUSTERED ([ComicId] ASC);
GO

-- Creating primary key on [ComicPhotoId] in table 'ComicPhotoes'
ALTER TABLE [dbo].[ComicPhotoes]
ADD CONSTRAINT [PK_ComicPhotoes]
    PRIMARY KEY CLUSTERED ([ComicPhotoId] ASC);
GO

-- Creating primary key on [ComicTextBubbleId] in table 'ComicTextBubbles'
ALTER TABLE [dbo].[ComicTextBubbles]
ADD CONSTRAINT [PK_ComicTextBubbles]
    PRIMARY KEY CLUSTERED ([ComicTextBubbleId] ASC);
GO

-- Creating primary key on [PhotoId] in table 'Photos'
ALTER TABLE [dbo].[Photos]
ADD CONSTRAINT [PK_Photos]
    PRIMARY KEY CLUSTERED ([PhotoId] ASC);
GO

-- Creating primary key on [TemplateId] in table 'Templates'
ALTER TABLE [dbo].[Templates]
ADD CONSTRAINT [PK_Templates]
    PRIMARY KEY CLUSTERED ([TemplateId] ASC);
GO

-- Creating primary key on [TemplateItemId] in table 'TemplateItems'
ALTER TABLE [dbo].[TemplateItems]
ADD CONSTRAINT [PK_TemplateItems]
    PRIMARY KEY CLUSTERED ([TemplateItemId] ASC);
GO

-- Creating primary key on [TextBubbleId] in table 'TextBubbles'
ALTER TABLE [dbo].[TextBubbles]
ADD CONSTRAINT [PK_TextBubbles]
    PRIMARY KEY CLUSTERED ([TextBubbleId] ASC);
GO

-- Creating primary key on [TextBubbleDirectionId] in table 'TextBubbleDirections'
ALTER TABLE [dbo].[TextBubbleDirections]
ADD CONSTRAINT [PK_TextBubbleDirections]
    PRIMARY KEY CLUSTERED ([TextBubbleDirectionId] ASC);
GO

-- Creating primary key on [Uid] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Uid] ASC);
GO

-- Creating primary key on [SubscriptionType], [Uid] in table 'UserSubscriptions'
ALTER TABLE [dbo].[UserSubscriptions]
ADD CONSTRAINT [PK_UserSubscriptions]
    PRIMARY KEY CLUSTERED ([SubscriptionType], [Uid] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [TemplateId] in table 'Comics'
ALTER TABLE [dbo].[Comics]
ADD CONSTRAINT [FK_Comic_Template]
    FOREIGN KEY ([TemplateId])
    REFERENCES [dbo].[Templates]
        ([TemplateId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Comic_Template'
CREATE INDEX [IX_FK_Comic_Template]
ON [dbo].[Comics]
    ([TemplateId]);
GO

-- Creating foreign key on [Uid] in table 'Comics'
ALTER TABLE [dbo].[Comics]
ADD CONSTRAINT [FK_Comic_User]
    FOREIGN KEY ([Uid])
    REFERENCES [dbo].[Users]
        ([Uid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Comic_User'
CREATE INDEX [IX_FK_Comic_User]
ON [dbo].[Comics]
    ([Uid]);
GO

-- Creating foreign key on [ComicId] in table 'ComicPhotoes'
ALTER TABLE [dbo].[ComicPhotoes]
ADD CONSTRAINT [FK_ComicPhoto_Comic]
    FOREIGN KEY ([ComicId])
    REFERENCES [dbo].[Comics]
        ([ComicId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ComicPhoto_Comic'
CREATE INDEX [IX_FK_ComicPhoto_Comic]
ON [dbo].[ComicPhotoes]
    ([ComicId]);
GO

-- Creating foreign key on [ComicId] in table 'ComicTextBubbles'
ALTER TABLE [dbo].[ComicTextBubbles]
ADD CONSTRAINT [FK_ComicTextBubble_Comic]
    FOREIGN KEY ([ComicId])
    REFERENCES [dbo].[Comics]
        ([ComicId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ComicTextBubble_Comic'
CREATE INDEX [IX_FK_ComicTextBubble_Comic]
ON [dbo].[ComicTextBubbles]
    ([ComicId]);
GO

-- Creating foreign key on [PhotoId] in table 'ComicPhotoes'
ALTER TABLE [dbo].[ComicPhotoes]
ADD CONSTRAINT [FK_ComicPhoto_Photo]
    FOREIGN KEY ([PhotoId])
    REFERENCES [dbo].[Photos]
        ([PhotoId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ComicPhoto_Photo'
CREATE INDEX [IX_FK_ComicPhoto_Photo]
ON [dbo].[ComicPhotoes]
    ([PhotoId]);
GO

-- Creating foreign key on [TemplateItemId] in table 'ComicPhotoes'
ALTER TABLE [dbo].[ComicPhotoes]
ADD CONSTRAINT [FK_ComicPhoto_TemplateItem]
    FOREIGN KEY ([TemplateItemId])
    REFERENCES [dbo].[TemplateItems]
        ([TemplateItemId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ComicPhoto_TemplateItem'
CREATE INDEX [IX_FK_ComicPhoto_TemplateItem]
ON [dbo].[ComicPhotoes]
    ([TemplateItemId]);
GO

-- Creating foreign key on [TextBubbleDirectionId] in table 'ComicTextBubbles'
ALTER TABLE [dbo].[ComicTextBubbles]
ADD CONSTRAINT [FK_ComicTextBubble_TextBubbleDirection]
    FOREIGN KEY ([TextBubbleDirectionId])
    REFERENCES [dbo].[TextBubbleDirections]
        ([TextBubbleDirectionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ComicTextBubble_TextBubbleDirection'
CREATE INDEX [IX_FK_ComicTextBubble_TextBubbleDirection]
ON [dbo].[ComicTextBubbles]
    ([TextBubbleDirectionId]);
GO

-- Creating foreign key on [Uid] in table 'Photos'
ALTER TABLE [dbo].[Photos]
ADD CONSTRAINT [FK_Photo_User]
    FOREIGN KEY ([Uid])
    REFERENCES [dbo].[Users]
        ([Uid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Photo_User'
CREATE INDEX [IX_FK_Photo_User]
ON [dbo].[Photos]
    ([Uid]);
GO

-- Creating foreign key on [TemplateId] in table 'TemplateItems'
ALTER TABLE [dbo].[TemplateItems]
ADD CONSTRAINT [FK_TemplateItem_Template]
    FOREIGN KEY ([TemplateId])
    REFERENCES [dbo].[Templates]
        ([TemplateId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TemplateItem_Template'
CREATE INDEX [IX_FK_TemplateItem_Template]
ON [dbo].[TemplateItems]
    ([TemplateId]);
GO

-- Creating foreign key on [TextBubbleId] in table 'TextBubbleDirections'
ALTER TABLE [dbo].[TextBubbleDirections]
ADD CONSTRAINT [FK_TextBubbleDirection_TextBubble]
    FOREIGN KEY ([TextBubbleId])
    REFERENCES [dbo].[TextBubbles]
        ([TextBubbleId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TextBubbleDirection_TextBubble'
CREATE INDEX [IX_FK_TextBubbleDirection_TextBubble]
ON [dbo].[TextBubbleDirections]
    ([TextBubbleId]);
GO

-- Creating foreign key on [Users_Uid] in table 'UserSubscriptions'
ALTER TABLE [dbo].[UserSubscriptions]
ADD CONSTRAINT [FK_UserSubscriptionUser]
    FOREIGN KEY ([Users_Uid])
    REFERENCES [dbo].[Users]
        ([Uid])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserSubscriptionUser'
CREATE INDEX [IX_FK_UserSubscriptionUser]
ON [dbo].[UserSubscriptions]
    ([Users_Uid]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------