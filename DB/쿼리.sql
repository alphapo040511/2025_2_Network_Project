-- 1. 데이터 베이스 생성gametest
CREATE DATABASE `SlimeGame` /*!40100 COLLATE 'utf8mb4_0900_ai_ci' */;

-- 2. 테이블 생성
CREATE TABLE `players` (
	`player_id` INT NOT NULL AUTO_INCREMENT,
	`username` VARCHAR(50) NULL DEFAULT NULL,
	`email` VARCHAR(50) NULL DEFAULT NULL,
	`password_hash` VARCHAR(255) NULL DEFAULT NULL,
	`created_at` TIMESTAMP NULL DEFAULT NULL,
	`last_login` TIMESTAMP NULL DEFAULT NULL,			
	PRIMARY KEY (`player_id`),
	UNIQUE INDEX `username` (`username`),
	UNIQUE INDEX `email` (`email`)
);



-- 플레이어 추가
INSERT INTO players(username, password_hash, created_at) VALUES
('hero234', 'hashed_password1', CURRENT_TIMESTAMP);