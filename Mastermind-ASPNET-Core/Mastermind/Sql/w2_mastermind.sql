DROP TABLE IF EXISTS `tp6_config`;
CREATE TABLE `tp6_config` (
    `key` varchar(50) NOT NULL PRIMARY KEY,
    `value` varchar(200) DEFAULT NULL
);

INSERT INTO `tp6_config` (`key`,`value`) VALUES 
 ('nb-colors', '6'),
 ('nb-positions', '4'),
 ('nb-attempts', '10');

DROP TABLE IF EXISTS `tp6_Members`;
CREATE TABLE `tp6_Members`
(
  `Id` integer AUTO_INCREMENT PRIMARY KEY,
  `FullName` nvarchar(20),
  `Email` nvarchar(50),
  `Username` nvarchar(20),
  `Password` nvarchar(100),
  `Role` nvarchar(20),
  `ImagePath` nvarchar(100)
);

/* Le mot de passe est : 12345 */
/*
INSERT INTO `tp6_Members`(`FullName`,`Email`,`Username`,`Password`,`Role`,`ImagePath`)
VALUES 
  ("Admin","admin@mondomaine.com","admin","zU9cErcQTXtnsDNOvPJFY02XSSjDUbkivTPFbymuE+gGUJX+","Admin","");
*/


--script sql pour la table qui stocke les statistiques des parties des membres: 
DROP TABLE IF exists `tp6_StatsMembre`;
CREATE TABLE `tp6_StatsMembre` (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    MemberId INT,
    GamesWon INT DEFAULT 0,
    GamesLost INT DEFAULT 0,
    BestPerformance INT DEFAULT NULL, -- Le moins de coups pour gagner une partie
    FOREIGN KEY (MemberId) REFERENCES tp6_Members(Id)
);
