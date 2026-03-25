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

CREATE TABLE t_publication(
   pubId INT AUTO_INCREMENT,
   pubTitle VARCHAR(50) NOT NULL,
   pubDescription TEXT,
   pubCondition ENUM(
    'Neuf', 
    'Bon état', 
    'Dommages superficiels', 
    'Endommagé mais fonctionnel', 
    'Cassé / Pour pièces'
   ) NOT NULL,
   pubImage VARCHAR(255) NOT NULL,
   pubStatus ENUM('Disponible', 'Donné', 'Indisponible') NOT NULL DEFAULT 'Disponible',
   pubLocation VARCHAR(150),
   pubCreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
   catId INT NOT NULL,
   useId INT NOT NULL,
   PRIMARY KEY(pubId),
   CONSTRAINT FK_pub_cat FOREIGN KEY(catId) REFERENCES t_category(catId),
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


INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES 
('Meubles', NULL, 0.00), 
('Électronique', NULL, 0.00), 
('Vêtements', NULL, 0.00), 
('Outils', NULL, 0.00);

INSERT INTO t_category (catName, parentId, catCo2Impact) VALUES 
('Table', 1, 15.50), 
('Chaise', 1, 5.20), 
('Smartphone', 2, 80.00), 
('T-Shirt', 3, 2.10);