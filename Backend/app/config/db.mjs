import mysql from 'mysql2/promise';
import dotenv from 'dotenv';

dotenv.config();

export const pool = mysql.createPool({
    host : process.env.DB_HOST || 'db',
    port : process.env.DB_PORT || '3306',
    user : process.env.DB_USER ||'db_root',
    password : process.env.DB_PASSWORD ||'db_root',
    database : process.env.DB_NAME || 'prophotostock',
    waitForConnections : true,
    connectionLimit : 10,
})


/*
const host= process.env.DB_HOST || 'db';
//const host= process.env.DB_HOST || 'localhost';
const port= process.env.DB_PORT || 3306;
const user= process.env.DB_USER || 'root';
const password = process.env.DB_PASSWORD || 'root';
const database = process.env.DB_NAME || "prophotostock";

const dbConfig = {
    host,
    port,
    user,
    password,
    database
};

export const connect = async () => {
    try
    {
        const connection = await mysql.createConnection(dbConfig);
        console.log('Connected to the database');
        return connection;
    }
    catch (error) {
        console.error('Error connecting to the database : ', error);
        throw error;
    }
};

export const databaseConnection = async (req, res, next) => {
    try 
    {
        req.db = await connect();
        next();
    } 
    catch (error) {
        console.error('Error connection to the database : ', error);
        res.status(500).json({error: 'Internal Server Error'});
    }
};

*/

