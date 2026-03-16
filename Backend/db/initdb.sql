CREATE DATABASE IF NOT EXISTS zweitevie_db;
USE zweitevie_db;

CREATE TABLE t_user(
   idUser INT AUTO_INCREMENT,
   name VARCHAR(50) NOT NULL,
   email VARCHAR(100) NOT NULL,
   password VARCHAR(255) NOT NULL,
   isAdmin BOOLEAN NOT NULL DEFAULT FALSE,
   PRIMARY KEY(idUser),
   UNIQUE(email)
) ENGINE=InnoDB;

CREATE TABLE t_category(
   idCategory INT AUTO_INCREMENT,
   nameCategory VARCHAR(50) NOT NULL,
   PRIMARY KEY(idCategory)
) ENGINE=InnoDB;

CREATE TABLE t_publication(
   idPublication INT AUTO_INCREMENT,
   title VARCHAR(50) NOT NULL,
   description TEXT,
   image_url VARCHAR(255) NOT NULL,
   status VARCHAR(50),
   location VARCHAR(150),
   createdAt DATETIME DEFAULT CURRENT_TIMESTAMP,
   idCategory INT NOT NULL,
   idUser INT NOT NULL,
   PRIMARY KEY(idPublication),
   CONSTRAINT FK_pub_cat FOREIGN KEY(idCategory) REFERENCES t_category(idCategory),
   CONSTRAINT FK_pub_user FOREIGN KEY(idUser) REFERENCES t_user(idUser) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE t_interest(
   idUser INT,
   idPublication INT,
   dateInterest DATETIME DEFAULT CURRENT_TIMESTAMP,
   PRIMARY KEY(idUser, idPublication),
   CONSTRAINT FK_int_user FOREIGN KEY(idUser) REFERENCES t_user(idUser) ON DELETE CASCADE,
   CONSTRAINT FK_int_pub FOREIGN KEY(idPublication) REFERENCES t_publication(idPublication) ON DELETE CASCADE
) ENGINE=InnoDB;

-- Insertion de quelques catégories pour tester
INSERT INTO t_category (nameCategory) VALUES ('Meubles'), ('Électronique'), ('Vêtements'), ('Outils');