import express from "express";
import { pool } from "../config/db.mjs";

const router = express.Router();

// GET /api/tags
router.get("/", async (req, res) => {
  try {
    const [rows] = await pool.query("SELECT * FROM t_tags");
    res.json(rows);
  } catch (err) {
    res.status(500).json({ error: "Error " });
  }
});

export default router;