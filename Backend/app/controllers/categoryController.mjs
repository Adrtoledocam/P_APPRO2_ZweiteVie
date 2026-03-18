import { pool } from "../config/db.mjs";

export const getCategories = async (req, res) => {
    try {
        const [rows] = await pool.execute('SELECT * FROM t_category ORDER BY catName ASC')
        res.json(rows);
    } catch (err){
        res.status(500).json({ error: "Erreur lors de la récupération des catégories." });
    }
};