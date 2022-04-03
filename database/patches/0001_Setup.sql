CREATE TABLE [HuntType](
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
    [Type] TEXT NOT NULL
);

INSERT INTO [HuntType]([Type])
VALUES
    ('Random Encounter'),
    ('Soft Reset'),
    ('Chain Fishing'),
    ('Pokeradar'),
    ('DexNav'),
    ('Wonder Trade');

CREATE TABLE [Game](
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
    [Name] TEXT NOT NULL,
    [Generation] INTEGER NOT NULL
);

INSERT INTO [Game]([Name], [Generation])
VALUES 
    ('Gold', 2),
    ('Silver', 2),
    ('Crystal', 2),

    ('Ruby', 3),
    ('Sapphire', 3),
    ('Emerald', 3),
    ('Fire Red', 3),
    ('Leaf Green', 3),

    ('Diamond', 4),
    ('Pearl', 4),
    ('Platinum', 4),
    ('Heart Gold', 4),
    ('Soul Silver', 4),

    ('Black', 5),
    ('White', 5),
    ('Black 2', 5),
    ('White 2', 5),

    ('X', 6),
    ('Y', 6),
    ('Alpha Sapphire', 6),
    ('Omega Ruby', 6),

    ('Moon', 6),
    ('Sun', 6),
    ('Ultra Moon', 6),
    ('Ultra Sun', 6),
    ('Let''s Go, Eevee', 6),
    ('Let''s Go, Pikachu', 6),


    ('Shield', 8),
    ('Sword', 8),
    ('Brilliant Diamond', 8),
    ('Shining Pearl', 8),

    ('Scarlet', 9),
    ('Violet', 9);

CREATE TABLE [Hunt](
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
    [Type] INTEGER NOT NULL,
    [Target] TEXT,
    [Game] INTEGER NOT NULL,
    [Complete] INTEGER NOT NULL,
    FOREIGN KEY([Type]) REFERENCES [HuntType]([Id]),
    FOREIGN KEY([Game]) REFERENCES [Game]([Id])
);

CREATE TABLE [Encounter](
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
    [HuntId] INTEGER NOT NULL,
    [Count] INTEGER NOT NULL,
    [DateTime] TEXT NOT NULL,
    FOREIGN KEY([HuntId]) REFERENCES [Hunt]([Id])
);

CREATE INDEX [HuntEncounters] ON [Encounter]([HuntId]);

CREATE TABLE [Catch](
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
    [HuntId] INTEGER NOT NULL,
    [Name] TEXT NOT NULL,
    [Nickname] TEXT,
    FOREIGN KEY([HuntId]) REFERENCES [Hunt]([Id])
);

CREATE TABLE [Fail](
    [Id] INTEGER PRIMARY KEY AUTOINCREMENT,
    [HuntId] INTEGER NOT NULL,
    [Encounter] INTEGER NOT NULL,
    [Name] TEXT NOT NULL,
    FOREIGN KEY([HuntId]) REFERENCES [Hunt]([Id])
);

