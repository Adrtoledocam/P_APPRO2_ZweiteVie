import express from 'express';
import { addFavorite, getMyFavorites, removeFavorite } from '../controllers/favoriteController.mjs';
import { verifyToken } from '../middleware/auth.mjs';

const router = express.Router();

// POST /api/favorites
router.post('/', verifyToken, addFavorite);

// GET /api/favorites
router.get('/', verifyToken, getMyFavorites);

// DELETE /api/favorites/:pubId
router.delete('/:pubId', verifyToken, removeFavorite);

export default router;
