CREATE DATABASE IF NOT EXISTS zweitevie_db;
USE zweitevie_db;

CREATE TABLE t_user(
   useId INT AUTO_INCREMENT,
   useName VARCHAR(50) NOT NULL,
   useEmail VARCHAR(100) NOT NULL,
   usePassword VARCHAR(255) NOT NULL,
   useIsAdmin BOOLEAN NOT NULL DEFAULT FALSE,
   usePhone VARCHAR(20),
   PRIMARY KEY(useId),
   UNIQUE(useEmail)
) ENGINE=InnoDB;

CREATE TABLE t_category(
   catId INT AUTO_INCREMENT,
   catName VARCHAR(50) NOT NULL,
   parentId INT NULL,
   catCo2Impact DECIMAL(5,2) DEFAULT 0.00,
   PRIMARY KEY(catId),
   CONSTRAINT FK_cat_parent FOREIGN KEY (parentId) REFERENCES t_category(catId)
) ENGINE=InnoDB;

CREATE TABLE t_condition (
    conId INT AUTO_INCREMENT PRIMARY KEY,
    conName VARCHAR(50) NOT NULL
) ENGINE=InnoDB;

CREATE TABLE t_publication(
   pubId INT AUTO_INCREMENT,
   pubTitle VARCHAR(50) NOT NULL,
   pubDescription TEXT,
   pubImage VARCHAR(255) NOT NULL,
   pubStatus ENUM('Disponible', 'Donné', 'Indisponible') NOT NULL DEFAULT 'Disponible',
   pubLocation VARCHAR(150),
   pubCreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
   catId INT NOT NULL,
   useId INT NOT NULL,
   conId INT NOT NULL,
   PRIMARY KEY(pubId),
   CONSTRAINT FK_pub_cat FOREIGN KEY(catId) REFERENCES t_category(catId),
   CONSTRAINT FK_pub_condition FOREIGN KEY (conId) REFERENCES t_condition(conId),
   CONSTRAINT FK_pub_user FOREIGN KEY(useId) REFERENCES t_user(useId) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE t_favorite (
   useId INT,
   pubId INT,
   intDate DATETIME DEFAULT CURRENT_TIMESTAMP,
   PRIMARY KEY(useId, pubId),
   CONSTRAINT FK_int_user FOREIGN KEY(useId) REFERENCES t_user(useId) ON DELETE CASCADE,
   CONSTRAINT FK_int_pub FOREIGN KEY(pubId) REFERENCES t_publication(pubId) ON DELETE CASCADE
) ENGINE=InnoDB;

INSERT INTO t_condition (conName) VALUES
('Neuf'),
('Bon état'),
('Dommages superficiels'),
('Endommagé mais fonctionnel'),
('Cassé / Pour pièces');

INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES
('Meubles',             NULL, 0.00),   -- 1
('Électronique',        NULL, 0.00),   -- 2
('Vêtements',           NULL, 0.00),   -- 3
('Outils',              NULL, 0.00),   -- 4
('Livres & Médias',     NULL, 0.00),   -- 5
('Sport & Loisirs',     NULL, 0.00),   -- 6
('Jouets & Enfants',    NULL, 0.00),   -- 7
('Jardin & Extérieur',  NULL, 0.00),   -- 8
('Cuisine & Maison',    NULL, 0.00),   -- 9
('Véhicules & Pièces',  NULL, 0.00),   -- 10
('Animaux',             NULL, 0.00),   -- 11
('Autres',              NULL, 0.00);   -- 12

-- ─────────────────────────────────────────
-- CATÉGORIES ENFANTS
-- ─────────────────────────────────────────

-- Meubles (parentId = 1)
INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES
('Table',           1, 15.50),
('Chaise',          1,  5.20),
('Canapé',          1, 30.00),
('Armoire',         1, 25.00),
('Bureau',          1, 18.00),
('Étagère',         1,  8.00),
('Lit',             1, 35.00);

-- Électronique (parentId = 2)
INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES
('Smartphone',      2, 80.00),
('Ordinateur',      2,150.00),
('Tablette',        2, 70.00),
('Télévision',      2,120.00),
('Console de jeux', 2, 90.00),
('Appareil photo',  2, 60.00),
('Accessoires',     2, 10.00);

-- Vêtements (parentId = 3)
INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES
('T-Shirt',         3,  2.10),
('Pantalon',        3,  4.50),
('Veste / Manteau', 3,  8.00),
('Chaussures',      3,  6.00),
('Robe / Jupe',     3,  3.50),
('Accessoires mode',3,  1.00);

-- Outils (parentId = 4)
INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES
('Outillage électrique', 4, 20.00),
('Outillage manuel',     4,  5.00),
('Matériel de bricolage',4, 10.00);

INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES
('Livres',          5,  1.50),
('DVD / Blu-ray',   5,  0.80),
('Musique / CD',    5,  0.50),
('Jeux vidéo',      5,  3.00);

INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES
('Vélo',            6, 40.00),
('Musculation',     6, 15.00),
('Sports collectifs',6, 5.00),
('Sports de glisse',6, 20.00),
('Camping',         6, 10.00);

INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES
('Jouets',          7,  5.00),
('Poussette',       7, 25.00),
('Vêtements enfant',7,  1.50),
('Puériculture',    7, 10.00);

INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES
('Mobilier jardin', 8, 20.00),
('Outillage jardin',8,  8.00),
('Plantes',         8,  0.50);

INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES
('Electroménager',  9, 50.00),
('Vaisselle',       9,  3.00),
('Décoration',      9,  2.00),
('Linge de maison', 9,  2.50);

INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES
('Pièces auto',    10, 15.00),
('Pièces moto',    10, 10.00),
('Accessoires auto',10, 5.00);

INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES
('Accessoires animaux', 11, 3.00),
('Alimentation animaux',11, 1.00);

INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES
('Autres',         12,  0.00);
