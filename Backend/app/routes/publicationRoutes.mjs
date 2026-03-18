import express from 'express';
import { getPublicationById, getPublications, createPublication, updatePublication, getMyPublications, deletePublication} from '../controllers/publicationController.mjs';
import {verifyToken} from '../middleware/auth.mjs'
const router = express.Router();

router.get('/', getPublications);

router.get('/:id', getPublicationById);


// POST /api/publications 
router.post('/', verifyToken, createPublication);

// PUT /api/publications/:id 
router.put('/:id', verifyToken, updatePublication);


// GET /api/my/all
router.get('/user/mine', verifyToken, getMyPublications);

// DELETE 
router.delete('/:id', verifyToken, deletePublication);

export default router;