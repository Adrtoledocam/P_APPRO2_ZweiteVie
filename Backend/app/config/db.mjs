import mysql from 'mysql2/promise';
import dotenv from 'dotenv';

dotenv.config();

const pool = mysql.createPool({
    host : process.env.DB_HOST || 'db',
    port : process.env.DB_PORT || '3306',
    user : process.env.DB_USER || 'db_root',
    password : process.env.DB_PASSWORD || 'db_root',
    database : process.env.DB_NAME || 'zweitevie_db',
    waitForConnections: true,
    connectionLimit: 10,
    queueLimit: 0
});

//Test
export const testConnection = async () => {
    try{
        const connection = await pool.getConnection();
        console.log('Connecté à la base de données MySQL');
        connection.release();
    } catch (err) {
        console.error('Erruer de connexion à la base de données :', err.message);
    }
};

export default pool;