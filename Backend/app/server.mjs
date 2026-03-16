import http from 'node:http';
import express from 'express';
import cors from 'cors';
import cookieParser from 'cookie-parser';
import bodyParser from 'body-parser';
import dotenv from 'dotenv';

import { pool } from "./config/db.mjs";
import authRoutes from "./routes/authRoutes.mjs"; 
import userRoutes from "./routes/userRoutes.mjs";
import photoRoutes from "./routes/photoRoutes.mjs";
import contractRoutes from "./routes/contractRoutes.mjs";
import tagRoutes from "./routes/tagRoutes.mjs";

dotenv.config();

const app = express();

app.use(express.json({ limit: '50mb' }));
app.use(express.urlencoded({ limit: '50mb', extended: true }));
app.use(bodyParser.json());
app.use(cookieParser())
app.use(cors({
    origin: process.env.FRONTEND_URL || 'http://localhost:5173',
    credentials : true
}));

const testDatabaseConnection = async()=>{
    try
    {
        const connection = await pool.getConnection();
        console.log('Connected to MySQL');
        connection.release();
    }
    catch(error)
    {
        //console.error('MySQL connection failed :', error)
    } 
}

await testDatabaseConnection();

app.get("/", (req, res) => { res.send("Backend ProPhotoStock works"); });

app.use("/api/auth", authRoutes); 
app.use("/api/users", userRoutes);
app.use("/api/photos", photoRoutes);
app.use("/api/contracts", contractRoutes )
app.use("/api/tags", tagRoutes);

const portHttp = process.env.PORT || 8080;

http.createServer(app).listen(portHttp, () => { 
    console.log(`🚀 Server HTTP running on http://localhost:${portHttp}`); 
});
