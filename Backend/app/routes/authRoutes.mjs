import express from "express";
import { login, register } from "../controllers/authController.mjs";
import { verifyToken, isAdmin } from "../middleware/auth.mjs";

const router = express.Router();

// POST /api/auth/login
router.post("/login", login); 

// POST /api/auth/register
router.post("/register", register);

//Get /api/admin-test
router.get("/admin-test", verifyToken, isAdmin, (req, res) => {
    res.json({ 
        message: "Bienvenue, Administrateur ! Accès autorisé." 
    });
});

export default router;