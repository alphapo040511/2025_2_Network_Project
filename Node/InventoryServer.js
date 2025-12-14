const express = require('express');
const mysql = require('mysql2/promise');
const app = express();

app.use(express.json());

const pool = mysql.createPool({
    host : 'localhost',
    user : 'root',
    password : '112233',
    database : 'slimegame'
});


// 플레이어 인벤토리 조회
app.get('/inventory/:player_id', async (req,res) => {
    console.log(`인벤토리 조회 시도 User:${req.params.player_id}`);
    try
    {
        const[inventory] = await pool.query(
            'SELECT inv.quantity, s.slime_key FROM inventories inv JOIN slimes s ON inv.slime_id = s.slime_id WHERE player_id = ?',
            [req.params.player_id]
        );
        res.status(200).json(inventory);
        console.log('인벤토리 로드 성공');
    }
    catch (error)
    {
        res.status(500).json({success : false, message : error.message});
        console.log(`인벤토리 로드 실패 ${error.message}`);
    }
});

// 플레이어 슬라임 추가
app.post('/inventory/getslime', async (req,res) => {
    const {player_id, slime_key} = req.body;

        console.log(`슬라임 추가 시도 User:${player_id}`);

    if (!player_id || slime_key == null) {
        return res.status(400).json({ success: false, message: "player_id나 slime_key가 없습니다." });
    }

    try
    {
        // 슬라임 id 찾기
        const [slimeRow] = await pool.query(
            'SELECT slime_id FROM slimes WHERE slime_key = ?',
            [slime_key]
        );

        if(slimeRow.length === 0) {
            return res.status(400).json({ success: false, message: "존재하지 않는 슬라임 키입니다." });
        }

        const slimeId = slimeRow[0].slime_id;


        const[inventory] = await pool.query(
            'SELECT 1 FROM inventories WHERE player_id = ? AND slime_id = ?',
            [player_id, slimeId]
        );

        // 보유중이 아닌 경우 새롭게 추가
        if(inventory.length === 0)
        {
            await pool.query(
                'INSERT INTO inventories(player_id, slime_id, quantity) VALUES (?, ?, 1);',
                [player_id, slimeId]
            );
        }
        else
        {
            // 존재하는 경우 개수 +1
            await pool.query(
                'UPDATE inventories SET quantity = quantity + 1 WHERE player_id = ? AND slime_id = ?',
                [player_id, slimeId]
            );
        }

        res.status(200).json({ success: true, message: "슬라임 추가 완료" });
    }
    catch (error)
    {
        res.status(500).json({ success: false, message: error.message });
    }
});

// 플레이어 골드 조회
app.get('/gold/:player_id', async (req,res) => {
    console.log(`골드 조회 시도 User:${req.params.player_id}`);
    try
    {
        const[inventory] = await pool.query(
            'SELECT gold FROM players WHERE player_id = ?',
            [req.params.player_id]
        );

        if(inventory.length === 0)
        {
            res.status(500).json({success : false, message : error.message});
            console.log(`골드 로드 실패 ${error.message}`);
            return;
        }

        const gold = inventory[0].gold;

        res.status(200).json(gold);
        console.log('골드 로드 성공');
    }
    catch (error)
    {
        res.status(500).json({success : false, message : error.message});
        console.log(`골드 로드 실패 ${error.message}`);
    }
});

// 플레이어 골드 업데이트
app.post('/gold/update', async (req,res) => {
    const {player_id, gold} = req.body;

    console.log(`골드 업데이트 시도 User:${player_id}`);

    if (!player_id || gold == null) {
        return res.status(400).json({ success: false, message: "player_id나 gold가 없습니다." });
    }

    try
    {
        await pool.query(
            'UPDATE players SET gold = ? WHERE player_id = ?',
            [gold, player_id]
        );

        res.status(200).json({ success: true, message: "골드 저장 완료" });
    }
    catch (error)
    {
        res.status(500).json({ success: false, message: error.message });
    }
});

const PORT = 4001;

app.listen(PORT, () => {
    console.log(`데이터 서버 실행 중 : ${PORT}`);
});