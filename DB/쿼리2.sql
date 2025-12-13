-- 10. 아이템 테이블 생성

CREATE TABLE slimes(
	slime_id INT AUTO_INCREMENT PRIMARY KEY,
	`name` VARCHAR(100) NOT NULL,
	DESCRIPTION TEXT,
	weight FLOAT NOT NULL,
	rarity INT DEFAULT 0
)


CREATE TABLE inventories(
	inventory_id INT AUTO_INCREMENT PRIMARY KEY,
	player_id INT,
	slime_id INT,
	quantity INT DEFAULT 1,
	FOREIGN KEY(player_id) REFERENCES players(player_id),
	FOREIGN KEY(slime_id) REFERENCES slimes(slime_id)
)

-- 17. 퀘스트 테이블 생성
CREATE TABLE quests(
	quest_id INT AUTO_INCREMENT PRIMARY KEY,
	title VARCHAR(100) NOT NULL,
	description TEXT,
	reward_exp INT DEFAULT 0,
	reward_item_id INT,
	FOREIGN KEY (reward_item_id) REFERENCES items(item_id)
)

