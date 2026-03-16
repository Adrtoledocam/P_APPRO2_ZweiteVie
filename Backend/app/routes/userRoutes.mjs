import express from "express";
import { auth } from "../middleware/auth.mjs";
import { requireRole } from "../middleware/role.mjs";

const router = express.Router();

//Admin
router.get("/admin", auth, requireRole("admin"), (req, res) => {
  res.json({ message: "admin" });
});

// Photograph
router.get("/photographer", auth, requireRole("photographer"), (req, res) => {
  res.json({ message: "Photographer" });
});

// Cliente
router.get("/client", auth, requireRole("client"), (req, res) => {
  res.json({ message: "Client" });
});

// Info Profile
router.get("/profile", auth, async (req, res) => {
  try {
    const [rows] = await pool.query(
      "SELECT userId, useName, useEmail, useRole FROM t_users WHERE userId = ?", 
      [req.user.id]
    );
    res.json(rows[0]);
  } catch (err) {
    res.status(500).json({ error: "Error profil" });
  }
});

export default router;
