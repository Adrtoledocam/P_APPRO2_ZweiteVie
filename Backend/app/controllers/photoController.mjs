import { pool } from "../config/db.mjs";
import { v2 as cloudinary } from 'cloudinary';
cloudinary.config({ 
  cloud_name: 'dzguf69ws', 
  api_key: '785519479823275', 
  api_secret: 'M_-iDEtOueXwp3uHB9zFzORCxsw' 
});


// GET /api/photos
export const getAllPhotos = async (req, res) => {
  try {
    const sql = `
      SELECT 
        p.photoId, 
        p.photoTitle, 
        p.photoUrl, 
        u.useName AS useName, 
        GROUP_CONCAT(t.tagName SEPARATOR ', ') AS tagName
      FROM t_photos p
      JOIN t_photographers ph ON p.fkPhotographer = ph.photographerId
      JOIN t_users u ON ph.fkUser = u.userId
      LEFT JOIN t_photo_tags pt ON p.photoId = pt.fkPhoto
      LEFT JOIN t_tags t ON pt.fkTag = t.tagId
      WHERE p.isVisible = TRUE
      GROUP BY p.photoId
    `;
    const [rows] = await pool.query(sql);
    res.json(rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: "Failed to fetch photos" });
  }
};

// GET /api/photos/:id
export const getPhotoById = async (req, res) => {
  try {
    const [rows] = await pool.query(
      "SELECT * FROM t_photos WHERE photoId = ?",
      [req.params.id]
    );

    if (rows.length === 0)
      return res.status(404).json({ error: "Photo not found" });

    res.json(rows[0]);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: "Failed to fetch photo" });
  }
};

export const getMyPhotos = async (req, res) => {
  const userId = req.user.id; 
  try {
    const sql = `
      SELECT p.* FROM t_photos p
      JOIN t_photographers ph ON p.fkPhotographer = ph.photographerId
      WHERE ph.fkUser = ?
      ORDER BY p.uploadDate DESC
    `;
    const [rows] = await pool.query(sql, [userId]);
    res.json(rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: "Error" });
  }
};


// GET /api/photos/popular
export const getPopularPhotos = async (req, res) => {
  try {
    const sql = `
      SELECT p.*, COUNT(c.contractId) as totalSales
      FROM t_photos p
      LEFT JOIN t_contracts c ON p.photoId = c.fkPhoto
      WHERE p.isVisible = TRUE
      GROUP BY p.photoId
      ORDER BY totalSales DESC, p.uploadDate DESC
    `;
    const [rows] = await pool.query(sql);
    res.json(rows);
  } catch (err) {
    res.status(500).json({ error: "Error" });
  }
};
// GET /api/photos/tag
export const getPhotosByTag = async (req, res) => {
  const { tagName } = req.params;
  try {
    const sql = `
      SELECT 
        p.photoId, 
        p.photoTitle, 
        p.photoUrl, 
        u.useName AS useName, 
        (SELECT GROUP_CONCAT(t2.tagName SEPARATOR ', ') 
         FROM t_photo_tags pt2 
         JOIN t_tags t2 ON pt2.fkTag = t2.tagId 
         WHERE pt2.fkPhoto = p.photoId) AS tagName
      FROM t_photos p
      JOIN t_photographers ph ON p.fkPhotographer = ph.photographerId
      JOIN t_users u ON ph.fkUser = u.userId
      JOIN t_photo_tags pt ON p.photoId = pt.fkPhoto
      JOIN t_tags t ON pt.fkTag = t.tagId
      WHERE t.tagName = ? AND p.isVisible = TRUE
      GROUP BY p.photoId
    `;
    const [rows] = await pool.query(sql, [tagName]);
    res.json(rows);
  } catch (err) {
    res.status(500).json({ error: "Error" });
  }
};

// POST /api/photos
export const createPhoto = async (req, res) => {
  const { photoTitle, photoBase64, uploadDate, status, tags } = req.body;
  const idUserLogin = req.user.id;

  if (!photoBase64) {
    return res.status(400).json({ error: "Image not found" });
  }

  let cloudinaryUrl = "";

  try {
    const uploadResult = await cloudinary.uploader.upload(`data:image/jpeg;base64,${photoBase64}`, {
      folder: "prophotostock_uploads", 
    });
    
    cloudinaryUrl = uploadResult.secure_url; 

    const connection = await pool.getConnection();
    try {
      await connection.beginTransaction();

      const [photographerRows] = await connection.query(
        "SELECT photographerId FROM t_photographers WHERE fkUser = ?", 
        [idUserLogin]
      );

      if (photographerRows.length === 0) {
        await connection.rollback();
        return res.status(403).json({ error: "Utilisateur pas valide" });
      }

      const photographerId = photographerRows[0].photographerId;

      const sqlPhoto = `
        INSERT INTO t_photos (photoTitle, photoUrl, uploadDate, isVisible, status, fkPhotographer)
        VALUES (?, ?, ?, TRUE, ?, ?)
      `;

      const [photoResult] = await connection.query(sqlPhoto, [
        photoTitle,
        cloudinaryUrl, 
        uploadDate || new Date(),
        status || 'available',
        photographerId
      ]);

      const newPhotoId = photoResult.insertId;

      if (tags && Array.isArray(tags) && tags.length > 0) {
        const tagQueries = tags.map(tagId => [newPhotoId, tagId]);
        await connection.query(
          "INSERT INTO t_photo_tags (fkPhoto, fkTag) VALUES ?", 
          [tagQueries]
        );
      }

      await connection.commit();
      res.json({ message: "Photo post", photoId: newPhotoId, url: cloudinaryUrl });

    } catch (dbError) {
      await connection.rollback();
      throw dbError; 
    } finally {
      connection.release();
    }
  } catch (err) {
    console.error("Error uploade:", err);
    res.status(500).json({ error: "Error ", details: err.message });
  }
};


// PUT /api/photos/:id
export const updatePhoto = async (req, res) => {
  const { photoTitle, status, isVisible } = req.body;

  try {
    const sql = `
      UPDATE t_photos
      SET photoTitle = ?, status = ?, isVisible = ?
      WHERE photoId = ?
    `;

    await pool.query(sql, [
      photoTitle,
      status,
      isVisible,
      req.params.id
    ]);

    res.json({ message: "Photo updated successfully" });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: "Failed to update photo" });
  }
};

// DELETE /api/photos/:id
export const deletePhoto = async (req, res) => {
  try {
    await pool.query("DELETE FROM t_photos WHERE photoId = ?", [
      req.params.id
    ]);

    res.json({ message: "Photo deleted successfully" });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: "Failed to delete photo" });
  }
};

export const deleteMyPhoto = async (req, res) => {
  const photoId = req.params.id;
  const userId = req.user.id;

  try {
    const [photoData] = await pool.query(
      `SELECT p.photoId, 
        (SELECT COUNT(*) FROM t_contracts WHERE fkPhoto = p.photoId) as contractCount
       FROM t_photos p
       JOIN t_photographers ph ON p.fkPhotographer = ph.photographerId
       WHERE p.photoId = ? AND ph.fkUser = ?`,
      [photoId, userId]
    );

    if (photoData.length === 0) {
      return res.status(403).json({ error: "Not allow " });
    }

    if (photoData[0].contractCount > 0) {
      return res.status(400).json({ 
        error: "Impossible de supprimer: Cette photo est liée a un contrat existant." 
      });
    }

    await pool.query("DELETE FROM t_photos WHERE photoId = ?", [photoId]);

    res.json({ message: "Photo suprimée" });
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: "Error" });
  }
};
