import express from 'express';
import { getProfile, updateProfile } from '../controllers/userController.mjs';
import { verifyToken } from '../middleware/auth.mjs'; 

const router = express.Router();

// GET /api/user/profile 
router.get('/profile', verifyToken, getProfile);

// PUT /api/user/profile 
router.put('/profile', verifyToken, updateProfile);

export default router;