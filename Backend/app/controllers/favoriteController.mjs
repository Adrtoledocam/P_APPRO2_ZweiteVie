import { pool } from "../config/db.mjs";


export const addFavorite = async (req, res) => {
    const { pubId } = req.body;
    try {
        await pool.execute('INSERT INTO t_favorite (useId, pubId) VALUES (?, ?)', [req.user.id, pubId]);
        res.json({ message: "Ajouté aux favoris !" });
    } catch (err) {
        res.status(500).json({ error: "Déjà dans vos favoris ou erreur serveur." });
    }
};

export const getMyFavorites = async (req, res) => {
    try {
        const query = `
            SELECT p.*, c.catName 
            FROM t_publication p
            JOIN t_favorite f ON p.pubId = f.pubId
            JOIN t_category c ON p.catId = c.catId
            WHERE f.useId = ?
        `;
        const [rows] = await pool.execute(query, [req.user.id]);
        res.json(rows);
    } catch (err) {
        res.status(500).json({ error: "Erreur lors de la récupération des favoris." });
    }
};