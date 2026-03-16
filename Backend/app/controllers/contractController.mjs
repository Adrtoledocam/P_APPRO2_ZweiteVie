import { pool } from "../config/db.mjs";

export const createContract = async (req, res) => {
  const { fkPhoto, fkUsage, fkType } = req.body;
  const userId = req.user.id; 

  try {
    const [usageRows] = await pool.query("SELECT * FROM t_usage WHERE usageId = ?", [fkUsage]);
    const [typeRows] = await pool.query("SELECT * FROM t_contract_types WHERE typeId = ?", [fkType]);

    if (usageRows.length === 0 || typeRows.length === 0) {
      return res.status(404).json({ error: "Contrat information invalid" });
    }

    const usage = usageRows[0];
    const contractType = typeRows[0];

    const finalPrice = contractType.isExclusive ? usage.priceExclusive : usage.priceDiffusion;
    const commission = finalPrice * 0.20;

    const startDate = new Date();
    const endDate = new Date();
    endDate.setFullYear(startDate.getFullYear() + 1);

    const sqlContract = `
      INSERT INTO t_contracts (startDate, endDate, price, status, photographerCommission, fkUsage, fkType, fkPhoto, fkUser)
      VALUES (?, ?, ?, 'active', ?, ?, ?, ?, ?)
    `;

    await pool.query(sqlContract, [
      startDate, endDate, finalPrice, commission, fkUsage, fkType, fkPhoto, userId
    ]);

    if (contractType.isExclusive) {
      await pool.query("UPDATE t_photos SET isVisible = FALSE WHERE photoId = ?", [fkPhoto]);
    }

    res.json({ 
      message: "Contract completé", 
      details: { total: finalPrice, exclusive: !!contractType.isExclusive } 
    });

  } catch (err) {
    console.error(err);
    res.status(500).json({ error: "Error contrat" });
  }
};


export const getMonthlyReport = async (req, res) => {
  try {
    const sql = `
      SELECT 
        u.useName AS photographerName, 
        p.photoTitle,
        c.contractId,                
        c.price,
        c.startDate,                 
        c.photographerCommission,
        c.status                     
      FROM t_contracts c
      JOIN t_photos p ON c.fkPhoto = p.photoId
      JOIN t_photographers ph ON p.fkPhotographer = ph.photographerId
      JOIN t_users u ON ph.fkUser = u.userId
    `;

    const [rows] = await pool.query(sql);
    res.json(rows);
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: "Error" });
  }
};

export const getMyContracts = async (req, res) => {
  const userId = req.user.id;
  const role = req.user.role;

  try {
    let sql;
    if (role === 'client') {
      sql = `SELECT c.*, p.photoTitle, p.photoUrl FROM t_contracts c 
             JOIN t_photos p ON c.fkPhoto = p.photoId 
             WHERE c.fkUser = ?`;                   
    } else if (role === 'photographer') {
      sql = `SELECT c.*, p.photoTitle, p.photoUrl FROM t_contracts c
             JOIN t_photos p ON c.fkPhoto = p.photoId
             JOIN t_photographers ph ON p.fkPhotographer = ph.photographerId
             WHERE ph.fkUser = ?`;
    } else if (role === 'admin') {
      sql = `SELECT c.*, p.photoTitle, p.photoUrl 
               FROM t_contracts c 
               JOIN t_photos p ON c.fkPhoto = p.photoId`;
    }

    const [rows] = await pool.query(sql, [userId]);
    res.json(rows);
  } catch (err) {
    res.status(500).json({ error: "Error " });
  }
};
