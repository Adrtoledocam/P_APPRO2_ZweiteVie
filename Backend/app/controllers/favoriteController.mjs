import { pool } from "../config/db.mjs";


export const addFavorite = async (req, res) => {
    const { pubId } = req.body;
    if (!pubId) return res.status(400).json({ error: "pubId est requis." });
    try {
        await pool.execute('INSERT INTO t_favorite (useId, pubId) VALUES (?, ?)', [req.user.id, pubId]);
        res.json({ message: "Ajouté aux favoris !" });
    } catch (err) {
        if (err.code === 'ER_DUP_ENTRY') {
            return res.status(409).json({ error: "Cette annonce est déjà dans vos favoris." });
        }
        res.status(500).json({ error: "Erreur serveur lors de l'ajout aux favoris." });
    }
};

export const removeFavorite = async (req, res) => {
    const pubId = parseInt(req.params.pubId);
    if (!pubId) return res.status(400).json({ error: "pubId est requis." });
    try {
        await pool.execute('DELETE FROM t_favorite WHERE useId = ? AND pubId = ?', [req.user.id, pubId]);
        res.json({ message: "Retiré des favoris !" });
    } catch (err) {
        res.status(500).json({ error: "Erreur lors de la suppression du favori." });
    }
};

export const getMyFavorites = async (req, res) => {
    try {
        const query = `
            SELECT p.*, c.catName, u.useName as donorName, cond.conName
            FROM t_publication p
            JOIN t_favorite f ON p.pubId = f.pubId
            JOIN t_category c ON p.catId = c.catId
            JOIN t_user u ON p.useId = u.useId
            JOIN t_condition cond ON p.conId = cond.conId
            WHERE f.useId = ?
            ORDER BY p.pubCreatedAt DESC
        `;
        const [rows] = await pool.execute(query, [req.user.id]);
        res.json(rows);
    } catch (err) {
        res.status(500).json({ error: "Erreur lors de la récupération des favoris." });
    }
};