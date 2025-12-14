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

const PORT = 4001;

app.listen(PORT, () => {
    console.log(`데이터 서버 실행 중 : ${PORT}`);
});