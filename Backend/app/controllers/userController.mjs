import { pool } from "../config/db.mjs";

export const getProfile = async (req, res) => {
    try {
        const [rows] = await pool.execute(
            'SELECT useId, useName, useEmail, usePhone FROM t_user WHERE useId = ?', 
            [req.user.id]
        );
        res.json(rows[0]);
    } catch (err) {
        res.status(500).json({ error: "Erreur profil" });
    }
};

export const updateProfile = async (req, res) => {
    const { username, phone } = req.body;
    try {
        await pool.execute(
            'UPDATE t_user SET useName = ?, usePhone = ? WHERE useId = ?',
            [username, phone, req.user.id]
        );
        res.json({ message: "Profil mis à jour !" });
    } catch (err) {
        res.status(500).json({ error: "Erreur mise à jour" });
    }
};