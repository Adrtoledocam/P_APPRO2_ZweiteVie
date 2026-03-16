DROP DATABASE IF EXISTS prophotostock;
CREATE DATABASE prophotostock CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE prophotostock;

CREATE TABLE t_users (
    userId INT AUTO_INCREMENT PRIMARY KEY,
    useName VARCHAR(50) NOT NULL,
    useEmail VARCHAR(100) NOT NULL UNIQUE,
    useRole ENUM('admin', 'photographer', 'client') NOT NULL,
    usePassword VARCHAR(255) NOT NULL,
    useSalt VARCHAR(255)
);

CREATE TABLE t_photographers (
    photographerId INT AUTO_INCREMENT PRIMARY KEY,
    fkUser INT NOT NULL UNIQUE,
    FOREIGN KEY (fkUser) REFERENCES t_users(userId) ON DELETE CASCADE
);

CREATE TABLE t_tags (
    tagId INT AUTO_INCREMENT PRIMARY KEY,
    tagName VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE t_photos (
    photoId INT AUTO_INCREMENT PRIMARY KEY,
    photoTitle VARCHAR(100) NOT NULL,
    photoUrl VARCHAR(255) NOT NULL,
    uploadDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    isVisible BOOLEAN DEFAULT TRUE,
    status VARCHAR(20) DEFAULT 'available',
    fkPhotographer INT NOT NULL,
    FOREIGN KEY (fkPhotographer) REFERENCES t_photographers(photographerId) ON DELETE CASCADE
);

CREATE TABLE t_photo_tags (
    fkPhoto INT NOT NULL,
    fkTag INT NOT NULL,
    PRIMARY KEY (fkPhoto, fkTag),
    FOREIGN KEY (fkPhoto) REFERENCES t_photos(photoId) ON DELETE CASCADE,
    FOREIGN KEY (fkTag) REFERENCES t_tags(tagId) ON DELETE CASCADE
);

CREATE TABLE t_contract_types (
    typeId INT AUTO_INCREMENT PRIMARY KEY,
    typeName VARCHAR(50) NOT NULL, 
    isExclusive BOOLEAN NOT NULL
);

CREATE TABLE t_usage (
    usageId INT AUTO_INCREMENT PRIMARY KEY,
    usageName VARCHAR(50) NOT NULL, 
    priceExclusive DECIMAL(15,2) NOT NULL,
    priceDiffusion DECIMAL(15,2) NOT NULL
);

CREATE TABLE t_contracts (
    contractId INT AUTO_INCREMENT PRIMARY KEY,
    startDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    endDate DATETIME NOT NULL,
    price DECIMAL(15,2) NOT NULL,
    status VARCHAR(20) DEFAULT 'active',
    fkUsage INT NOT NULL,
    fkType INT NOT NULL,
    fkPhoto INT NOT NULL,
    fkUser INT NOT NULL, 
    FOREIGN KEY (fkUsage) REFERENCES t_usage(usageId),
    FOREIGN KEY (fkType) REFERENCES t_contract_types(typeId),
    FOREIGN KEY (fkPhoto) REFERENCES t_photos(photoId),
    FOREIGN KEY (fkUser) REFERENCES t_users(userId)
);

-- ============================
-- INSERTS
-- ============================


INSERT INTO t_users (useName, useEmail, useRole, usePassword) VALUES 
('photo', 'adri@prophoto.com', 'photographer', '123456'),
('client', 'fede@gmail.com', 'client', '123456'),
('Responsable Admin', 'admin@prophotostock.ch', 'admin', '123456');

INSERT INTO t_photographers (fkUser) VALUES (1);

INSERT INTO t_tags (tagName) VALUES ('Sport'), ('Paysage'), ('Portrait');

INSERT INTO t_contract_types (typeName, isExclusive) VALUES 
('Exclusif', TRUE), 
('Diffusion', FALSE);

INSERT INTO t_usage (usageName, priceExclusive, priceDiffusion) VALUES 
('Publicité', 740.00, 370.00), 
('Graphisme', 1280.00, 640.00), 
('Média', 1400.00, 700.00);
