import express from 'express';
import cors from 'cors';
import dotenv from 'dotenv';
import { testConnection } from './config/db.mjs';

import authRoutes from './routes/authRoutes.mjs';
import categoryRoutes from './routes/categoryRoutes.mjs';
import publicationRoutes from './routes/publicationRoutes.mjs';
import userRoutes from './routes/userRoutes.mjs';

dotenv.config();

const app = express();
const PORT = process.env.PORT||8080;

//Middlewares
app.use(cors());
app.use(express.json({limit: '50mb'}));
app.use(express.urlencoded({extended:true}));

//Test
testConnection();

app.get('/', (req, res) => {
  res.json({ message: "Bienvenue sur l'API ZweiteVie !", status: "Running" });
});

//Authentification
app.use('/api/auth', authRoutes);

//Cataegories
app.use('/api/categories', categoryRoutes);

//Publications
app.use('/api/publications', publicationRoutes);

//User
app.use('/api/user', userRoutes);

app.listen(PORT, () => {
  console.log(`🚀 Serveur démarré sur http://localhost:${PORT}`);
  console.log(`📡 En attente de requêtes de l'application MAUI...`);
});