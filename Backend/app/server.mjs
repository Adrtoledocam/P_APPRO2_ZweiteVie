import express from 'express';
import cors from 'cors';
import dotenv from 'dotenv';
import { fileURLToPath } from 'url';
import { dirname, join } from 'path';
import { testConnection } from './config/db.mjs';

import authRoutes from './routes/authRoutes.mjs';
import categoryRoutes from './routes/categoryRoutes.mjs';
import publicationRoutes from './routes/publicationRoutes.mjs';
import userRoutes from './routes/userRoutes.mjs';
import favoriteRoutes from './routes/favoriteRoutes.mjs';

// Resuelve el .env relativo a este archivo (Backend/app/../.env = Backend/.env)
const __dirname = dirname(fileURLToPath(import.meta.url));
dotenv.config({ path: join(__dirname, '../.env') });

const REQUIRED_ENV = ['DB_HOST', 'DB_USER', 'DB_PASSWORD', 'DB_NAME', 'JWT_SECRET', 'CLOUDINARY_CLOUD_NAME', 'CLOUDINARY_API_KEY', 'CLOUDINARY_API_SECRET'];
const missing = REQUIRED_ENV.filter(key => !process.env[key]);
if (missing.length > 0) {
    console.error(`❌ Variables de entorno faltantes: ${missing.join(', ')}`);
    console.error('Copia .env.example como .env y rellena los valores.');
    process.exit(1);
}

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

//Favorites
app.use('/api/favorites', favoriteRoutes);

app.listen(PORT, () => {
  console.log(`🚀 Serveur démarré sur http://localhost:${PORT}`);
  console.log(`📡 En attente de requêtes de l'application MAUI...`);
});