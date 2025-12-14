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

// 슬라임 등급별 확률% (고정)
const rarity = [30, 25, 20, 15, 10];
const cumSum = [];
rarity.reduce((sum, val, i) => cumSum[i] = sum + val, 0);

// 랜덤 슬라임 반환
app.get('/gacha/refresh/:count', async (req,res) => {
    console.log('랜덤 슬라임 반환 시도');
    const result = [];
    for (let i = 0; i < req.params.count; i++) {
        const grade = chooseGrade();       // 등급 결정
        const slime = getRandomSlime(grade); // 등급별 랜덤 슬라임
        result.push(slime);
    }
    res.status(200).json(result);   // 클라이언트로 반환
});

function chooseGrade() {
    const rand = Math.random() * 100; // 0~100 사이 난수
    for (let i = 0; i < cumSum.length; i++) {
        if (rand < cumSum[i]) return i;
    }
    return cumSum.length - 1; // 안전 장치
}

function getRandomSlime(rarity)
{
    const list = slimesByGrade[rarity];

    if (!list || list.length === 0) {
        console.warn(`등급 ${grade} 슬라임이 없음`);
        return null;
    }

    const randIndex = Math.floor(Math.random() * list.length);
    return list[randIndex];
}

const PORT = 4002;

app.listen(PORT, () => {
    loadSlimes();
    console.log(`뽑기 서버 실행 중 : ${PORT}`);
});



// 등급별 슬라임을 저장할 객체
// key: 등급, value: 해당 등급 슬라임 배열
const slimesByGrade = {};


// 서버 시작 시 DB에서 슬라임 불러오기
async function loadSlimes() {
    
    for(let i = 0; i <= 4; i++)
    {
        slimesByGrade[i] = [];          // 초기화

        const [rows] = await pool.query(
            'SELECT slime_id, slime_key FROM slimes WHERE rarity = ?',
            [i]
        );          // 가중치는 작업 시간 부족으로 없이 진행

        slimesByGrade[i] = rows;        // 목록 할당
    }

    console.log(`슬라임 데이터 로드 완료`);
    for(let grade in slimesByGrade) {
        console.log(`등급 ${grade}: ${slimesByGrade[grade].length}종`);
    }
}

function getRandomIntExclusive(max, min=0) {
    return Math.floor(Math.random() * (max - min)) + min;
}