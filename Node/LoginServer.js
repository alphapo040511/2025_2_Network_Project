
// 필요한 모듈 불러오기
require('dotenv').config();                     // dotenv 모듈을 사용하여 환경 변수 로드

const express = require('express');
const bodyParser = require('body-parser');
const jwt = require('jsonwebtoken');
const bcrypt = require('bcrypt');

const mysql = require('mysql2/promise');
const app = express();

app.use(express.json());

// 환경 변수에서 시크릿 키와 포트 가져오기
const JWT_SECRET = process.env.JWT_SECRET;
const REFRESH_TOKEN_SECRET = process.env.REFRESH_TOKEN_SECRET;
const PORT = process.env.PORT || 4000;

const pool = mysql.createPool({
    host : 'localhost',
    user : 'root',
    password : '112233',
    database : 'slimegame'
});

// 회원가입 라우트
app.post('/register', async(req, res) => {

    const {username, password} = req.body;
    console.log(`회원 가입 시도 [유저: ${username}]`);
    try
    {
        // 유저 존재 확인
        const[players] = await pool.query(
            'SELECT player_id FROM players WHERE username = ?',
            [username]
        );

        // 유저가 있다면 실패
        if(players.length > 0)
        {
            console.log(`사용중인 아이디`);
            return res.status(409).json({
                success : false, 
                message : '사용중인 아이디입니다.'
            }); 
        }

        // 비밀번호 해쉬
        const hashedPassword = await bcrypt.hash(password, 10);

        console.log(`DB에 등록 시도`);

        // 플레이어 등록
        await pool.query(
            `INSERT INTO players (username, password_hash, created_at) VALUES (?, ?, CURRENT_TIMESTAMP)`,
            [username, hashedPassword]
        );

        console.log(`회원 가입 성공 [유저: ${username}]`);
        res.status(201).json({
            success : true,
            message : '회원 가입 성공'}
        );
    }
    catch (error)
    {
        res.status(500).json({success : false, message : error.message});
    }

})


// 플레이어 로그인
app.post('/login', async (req, res) => {
    const {username, password} = req.body;

    try
    {
        // 유저 존재 확인
        const[players] = await pool.query(
            'SELECT player_id, password_hash FROM players WHERE username = ?',
            [username]
        );

        // 유저가 없다면 실패
        if(players.length === 0)
        {
            return res.status(401).json({
                success : false, 
                message : '존재하지 않는 사용자 입니다.'
            }); 
        }

        const player = players[0];

        // 비밀번호 비교
        const isMatch = await bcrypt.compare(password, player.password_hash);

        // 비밀번호가 틀린 경우
        if (!isMatch) {
            return res.status(401).json({
                success: false,
                message: '비밀번호가 틀렸습니다.'
            });
        }

        const accessToken = generateAccessToken(player.player_id)

        // 로그인 성공, 로그인 시간 갱신
        await pool.query(
            'UPDATE players SET last_login = CURRENT_TIMESTAMP WHERE player_id = ?',
            [player.player_id]
        );

        console.log(`로그인 성공 ${player.player_id}`);

        // 성공 응답
        res.status(200).json({
            success: true,
            playerId: player.player_id,
            accessToken
        });        
    }
    catch (error)
    {
        res.status(500).json({
            success : false, 
            message : error.message
        });
    }
});

// 엑세스 토큰 생성 함수
function generateAccessToken(playerId)
{
    return jwt.sign({playerId}, JWT_SECRET, {expiresIn: '15m'});
}


app.listen(PORT, () => {
    console.log(`로그인 서버 실행 중 : ${PORT}`);
});