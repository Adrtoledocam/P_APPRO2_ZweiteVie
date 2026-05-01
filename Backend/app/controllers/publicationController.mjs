import { pool } from "../config/db.mjs";
import cloudinary from "../config/cloudinary.mjs";

export const getPublications = async (req, res) => {
    try {
        const { conId, search, catId } = req.query;

        let query = `
            SELECT p.*, c.catName, u.useName as donorName, cond.conName
            FROM t_publication p
            JOIN t_category c ON p.catId = c.catId
            JOIN t_user u ON p.useId = u.useId
            JOIN t_condition cond ON p.conId = cond.conId
            WHERE p.pubStatus = 'Disponible'
        `;
        const params = [];

        if (conId) {
            query += " AND p.conId = ?";
            params.push(conId);
        }
        if (catId) {
            query += " AND p.catId = ?";
            params.push(catId);
        }
        if (search) {
            query += " AND p.pubTitle LIKE ?";
            params.push(`%${search}%`);
        }

        query += " ORDER BY p.pubCreatedAt DESC";

        const [rows] = await pool.execute(query, params);
        res.json(rows);
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: "Erreur lors de la recherche des publications." });
    }
};

export const getPublicationById = async (req, res) => {
    const { id } = req.params;
    const authHeader = req.headers['authorization'];
    const token = authHeader && authHeader.split(' ')[1];

    try {
        const query = `
            SELECT p.*, c.catName, u.useName as donorName, u.useEmail, u.usePhone, cond.conName
            FROM t_publication p
            JOIN t_category c ON p.catId = c.catId
            JOIN t_user u ON p.useId = u.useId
            JOIN t_condition cond ON p.conId = cond.conId
            WHERE p.pubId = ?
        `;
        const [rows] = await pool.execute(query, [id]);
        
        if (rows.length === 0) return res.status(404).json({ message: "Annonce introuvable." });

        let publication = rows[0];

        if (!token) {
            publication.useEmail = "Connectez-vous pour voir l'email";
            publication.usePhone = "Connectez-vous pour voir le téléphone";
        }

        res.json(publication);
    } catch (err) {
        res.status(500).json({ error: "Erreur serveur." });
    }
};

export const getMyPublications = async (req, res) => {
    try {
        const [rows] = await pool.execute(
            `SELECT p.*, c.catName, cond.conName
             FROM t_publication p
             JOIN t_category c ON p.catId = c.catId
             JOIN t_condition cond ON p.conId = cond.conId
             WHERE p.useId = ?
             ORDER BY p.pubCreatedAt DESC`,
            [req.user.id]
        );
        res.json(rows);
    } catch (err) {
        res.status(500).json({ error: "Erreur lors de la récupération de vos annonces." });
    }
};

export const createPublication = async (req, res) => {
    const { title, description, conId, location, catId, imageBase64 } = req.body;
    const userId = req.user.id; 

    try {    
        const uploadResponse = await cloudinary.uploader.upload(imageBase64, {
            folder: 'zweitevie_items'
        });

        const [result] = await pool.execute(
            `INSERT INTO t_publication (pubTitle, pubDescription, conId, pubImage, pubLocation, catId, useId) 
            VALUES (?, ?, ?, ?, ?, ?, ?)`,
                [title, description, conId, uploadResponse.secure_url, location, catId, userId ]
        );

        res.status(201).json({ message: "Annonce créée !", pubId: result.insertId });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: "Erreur lors de la creación." });
    }
};

export const updatePublication = async (req, res) => {
    const { id } = req.params;
    const { title, description, conId, status, location } = req.body;
    const userId = req.user.id;
    const isAdmin = req.user.isAdmin;

    try {
        const [pub] = await pool.execute('SELECT useId FROM t_publication WHERE pubId = ?', [id]);

        if (pub.length === 0) return res.status(404).json({ message: "Annonce non trouvée" });
        if (pub[0].useId !== userId && !isAdmin) return res.status(403).json({ message: "Accès refusé" });

        await pool.execute(
            `UPDATE t_publication
             SET pubTitle = COALESCE(?, pubTitle),
                 pubDescription = COALESCE(?, pubDescription),
                 conId = COALESCE(?, conId),
                 pubStatus = COALESCE(?, pubStatus),
                 pubLocation = COALESCE(?, pubLocation)
             WHERE pubId = ?`,
            [title, description, conId, status, location, id]
        );
        res.json({ message: "Annonce mise à jour !" });
    } catch (err) {
        res.status(500).json({ error: "Erreur serveur" });
    }
};

export const donatePublication = async (req, res) => {
    const { id } = req.params;
    const userId = req.user.id;
    try {
        const [pub] = await pool.execute('SELECT useId FROM t_publication WHERE pubId = ?', [id]);
        if (pub.length === 0) return res.status(404).json({ message: "Annonce non trouvée" });
        if (pub[0].useId !== userId) return res.status(403).json({ message: "Accès refusé" });

        await pool.execute(
            "UPDATE t_publication SET pubStatus = 'Indisponible' WHERE pubId = ?",
            [id]
        );
        res.json({ message: "Annonce marquée comme indisponible !" });
    } catch (err) {
        console.error(err);
        res.status(500).json({ error: "Erreur serveur" });
    }
};

export const deletePublication = async (req, res) => {
    const { id } = req.params;
    const userId = req.user.id;
    const isAdmin = req.user.isAdmin;

    try {
        const [pub] = await pool.execute('SELECT useId FROM t_publication WHERE pubId = ?', [id]);
        if (pub.length === 0) return res.status(404).json({ message: "Annonce non trouvée." });

        if (pub[0].useId !== userId && !isAdmin) {
            return res.status(403).json({ message: "Action interdite : vous n'êtes pas l'auteur ou admin." });
        }
        await pool.execute('DELETE FROM t_publication WHERE pubId = ?', [id]);
        
        res.json({ message: isAdmin ? "Annonce supprimée par l'administrateur." : "Votre annonce a été supprimée." });
    } catch (err) {
        res.status(500).json({ error: "Erreur lors de la suppression." });
    }
};