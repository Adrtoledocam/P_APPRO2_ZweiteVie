import express from 'express';
import { getPublicationById, getPublications, createPublication, updatePublication, getMyPublications, deletePublication, donatePublication } from '../controllers/publicationController.mjs';
import {verifyToken} from '../middleware/auth.mjs'
const router = express.Router();

router.get('/', getPublications);

// Routes spécifiques AVANT /:id pour éviter les conflits de routing
router.get('/user/mine', verifyToken, getMyPublications);

router.get('/:id', getPublicationById);

router.post('/', verifyToken, createPublication);

router.put('/:id', verifyToken, updatePublication);
router.patch('/:id/donate', verifyToken, donatePublication);

router.delete('/:id', verifyToken, deletePublication);

export default router;