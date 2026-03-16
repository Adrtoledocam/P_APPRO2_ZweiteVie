import express from 'express';
import cors from 'cors';
import dotenv from 'dotenv';
import { testConnection } from './config/db.mjs';

dotenv.config();

const app = express();
const PORT = process.env.PORT||8080;

//Middlewares
app.use(cors());
app.use(express.json({Limit: '50mb'}));
app.use(express.urlencoded({extended:true}));

testConnection();

app.get('/', (req, res) => {
  res.json({ message: "Bienvenue sur l'API ZweiteVie !", status: "Running" });
});

app.listen(PORT, () => {
  console.log(`🚀 Serveur démarré sur http://localhost:${PORT}`);
  console.log(`📡 En attente de requêtes de l'application MAUI...`);
});