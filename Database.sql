create database stock;
\c stock;

create sequence id_sortie;
create sequence id_entrer;


create table article (
    id_article varchar(10) PRIMARY KEY,
    categorie varchar(10) not null,
    nom varchar(50) not null,
    type_mouvement varchar(10) not null,
    unite VARCHAR(10) NOT NULL
);
insert into article values ('ART1','snacks','Biscuit','FIFO','unit');
insert into article values ('ART2','boisson','THB','LIFO','unit');
insert into article values ('ART3','laitier','Fromage','LIFO','unit');

CREATE TABLE Unite (
    id_article VARCHAR(10),
    quantite DOUBLE PRECISION,
    nom VARCHAR(50)
);
insert into unite values ('ART1',50,'Sachet');
insert into unite values ('ART1',1,'unite');
insert into unite values ('ART2',8,'Cagot');
insert into unite values ('ART2',1,'unite');
insert into unite values ('ART3',10,'Carton');
insert into unite values ('ART3',1,'unite');

create table reel(
    id_mouvement varchar(10),
    nom varchar(50),
    quantite DECIMAL,
    mouvement INT,
    etat DECIMAL
);

create table magasin (
    id_magasin varchar(10) PRIMARY KEY,
    nom varchar(10) not null
);

insert into magasin values ('M1','M1');
insert into magasin values ('M2','M2');
insert into magasin values ('M3','M3');

CREATE TABLE entrer_stock (
    id_mouvement VARCHAR(10) PRIMARY KEY,
    id_magasin VARCHAR(10) REFERENCES magasin(id_magasin),
    id_article VARCHAR(50) REFERENCES article(id_article),
    date_mouvement TIMESTAMP NOT NULL,
    quantite DECIMAL NOT NULL,
    prix_unitaire DECIMAL NOT NULL
);
INSERT INTO entrer_stock VALUES ('ENT2', 'M1', 'ART1', '2023-01-01 12:00:00', 500 , 230 );


CREATE TABLE sortie_stock (
    id_mouvement VARCHAR(50),
    id_entrer VARCHAR(50) REFERENCES entrer_stock(id_mouvement),
    id_magasin VARCHAR(50) REFERENCES magasin(id_magasin),
    id_article VARCHAR(50) REFERENCES article(id_article),
    date_mouvement TIMESTAMP,
    quantite DOUBLE PRECISION
);


CREATE TABLE Brouillard (
    id_mouvement VARCHAR(50) PRIMARY KEY,
    id_article VARCHAR(50),
    date_mouvement TIMESTAMP,
    id_magasin VARCHAR(50),
    quantite DOUBLE PRECISION,
    etat DOUBLE PRECISION

);

create table valider (
    id_mouvement VARCHAR(50),
    id_article VARCHAR(50),
    date_mouvement TIMESTAMP,
    id_magasin VARCHAR(50),
    quantite DOUBLE PRECISION,
    date_validation TIMESTAMP
);



CREATE OR REPLACE VIEW entrer_stock_vue AS SELECT * FROM entrer_stock;
CREATE OR REPLACE VIEW sortie_stock_vue AS SELECT * FROM sortie_stock;

CREATE OR REPLACE VIEW etat_stock_details AS
SELECT
    e.id_mouvement,
    e.date_mouvement,
    e.id_article,
    e.ID_magasin,
    e.quantite AS quantite_initial,
    e.quantite - COALESCE(SUM(s.quantite), 0) AS quantite_final,
    e.prix_unitaire
FROM
    entrer_stock_vue e
LEFT JOIN
    sortie_stock_vue s ON e.id_mouvement = s.id_entrer
GROUP BY
    e.id_mouvement, e.date_mouvement, e.id_article, e.ID_magasin, e.quantite, e.prix_unitaire;


CREATE OR REPLACE VIEW etat_stock AS
SELECT 
    esd.ID_magasin,
    a.nom AS id_article,
    SUM(esd.quantite_initial) AS quantite_initial,
    SUM(esd.quantite_final) AS quantite_final,
    CASE 
        WHEN SUM(esd.quantite_final) > 0 THEN SUM(esd.quantite_final * esd.prix_unitaire) / SUM(esd.quantite_final)
        ELSE 0
    END AS PUMP
FROM 
    etat_stock_details esd
    JOIN article a ON esd.id_article = a.id_article
GROUP BY 
    a.nom, esd.ID_magasin;


delete from valider;
delete from Brouillard;
delete from sortie_stock;
delete from entrer_stock;

delete from article;
delete from magasin;
delete from unite;

drop view etat_stock;
drop view etat_stock_details;
drop view sortie_stock_vue;
drop view entrer_stock_vue;










CREATE OR REPLACE VIEW vue_reel_entree AS
SELECT
    r.id_mouvement,
    r.nom,
    r.quantite,
    'Entrée' AS mouvement_type,
    r.etat,
    e.id_magasin,
    e.id_article,
    e.date_mouvement AS date_mouvement_entree,
    e.quantite AS quantite_entree
FROM
    reel r
    JOIN entrer_stock e ON r.id_mouvement = e.id_mouvement
WHERE
    r.etat = 1;




CREATE OR REPLACE VIEW vue_reel_sortie AS
SELECT
    r.id_mouvement,
    r.nom,
    r.quantite,
    'Sortie' AS mouvement_type,
    r.etat,
    s.id_magasin,
    s.id_article,
    s.id_entrer,
    s.date_mouvement AS date_mouvement_sortie,
    s.quantite AS quantite_sortie
FROM
    reel r
    JOIN sortie_stock s ON r.id_mouvement = s.id_mouvement
WHERE
    r.etat = 2;
