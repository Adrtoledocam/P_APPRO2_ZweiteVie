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

export const getUserStats = async (req, res) => {
    try {
        const query = `
            SELECT
                COUNT(*) as totalPubs,
                SUM(CASE WHEN p.pubStatus = 'Indisponible' THEN 1 ELSE 0 END) as totalDonated,
                SUM(CASE WHEN p.pubStatus = 'Indisponible' THEN c.catCo2Impact ELSE 0 END) as totalCo2
            FROM t_publication p
            JOIN t_category c ON p.catId = c.catId
            WHERE p.useId = ?
        `;
        const [stats] = await pool.execute(query, [req.user.id]);
        const [user] = await pool.execute('SELECT useName, useEmail, usePhone FROM t_user WHERE useId = ?', [req.user.id]);

        res.json({
            profile: user[0],
            stats: stats[0]
        });
    } catch (err) {
        res.status(500).json({ error: "Erreur lors du calcul de l'impact." });
    }
};