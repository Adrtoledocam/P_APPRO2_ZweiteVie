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
        const [rows] = await pool.execute(`
            SELECT
                u.useName, u.useEmail, u.usePhone,
                COUNT(p.pubId) as totalPubs,
                SUM(CASE WHEN p.pubStatus = 'Indisponible' THEN 1 ELSE 0 END) as totalDonated,
                SUM(CASE WHEN p.pubStatus = 'Indisponible' THEN c.catCo2Impact ELSE 0 END) as totalCo2
            FROM t_user u
            LEFT JOIN t_publication p ON u.useId = p.useId
            LEFT JOIN t_category c ON p.catId = c.catId
            WHERE u.useId = ?
            GROUP BY u.useId
        `, [req.user.id]);

        const row = rows[0];
        res.json({
            profile: { useName: row.useName, useEmail: row.useEmail, usePhone: row.usePhone },
            stats:   { totalPubs: row.totalPubs, totalDonated: row.totalDonated, totalCo2: row.totalCo2 }
        });
    } catch (err) {
        res.status(500).json({ error: "Erreur lors du calcul de l'impact." });
    }
};