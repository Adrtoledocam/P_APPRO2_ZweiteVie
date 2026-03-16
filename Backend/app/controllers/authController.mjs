import {pool} from "../config/db.mjs";
import crypto from "crypto";
import jwt from "jsonwebtoken";

const generateSalt = () => crypto.randomBytes(16).toString("hex");

const hashPassword = (password, salt) => 
    crypto.createHmac("sha256", salt).update(password).digest("hex");

export const register = async (req, res) => {
    const {username, password, role, email} = req.body;

    if (!username || !password || !role)
        return res.status(400).json({error:"missing fields"});

    const allowedRoles = ["admin", "photographer", "client"];
    if (!allowedRoles.includes(role)) 
        return res.status(400).json({ error: "Rôle invalide." });
    try{
        const salt =  generateSalt();
        const hashed = hashPassword(password, salt);

        const sqlUser = `INSERT INTO t_users (useName, usePassword, useSalt, useRole, useEmail) VALUES (?,?,?,?,?)`;
        const [userResult] = await pool.query(sqlUser, [username, hashed, salt, role, email]);

        const newUserId = userResult.insertId;
        if (role === "photographer") {
            const sqlPhotog = `INSERT INTO t_photographers (fkUser) VALUES (?)`;
            await pool.query(sqlPhotog, [newUserId]);
        }
        res.json ({message : "User registered sucessfully"});
    } catch (err) {
        console.error(err);
        res.status(500).json({error: "Registration failed"});
    }
}; 

export const login = async (req, res) => {
    const {email, password} = req.body;

    try{
        const [rows] = await pool.query("SELECT * FROM t_users WHERE useEmail = ?", [email]);

        if (rows.length === 0)
      return res.status(401).json({ error: "Invalid credentials" });

    const user = rows[0];

    const hashed = hashPassword(password, user.useSalt);

    if (hashed !== user.usePassword)
      return res.status(401).json({ error: "Invalid credentials" });

    const token = jwt.sign(
      {
        id: user.userId,
        username: user.userName,
        role: user.useRole,
      },
      process.env.JWT_SECRET || "secret123",
      { expiresIn: "2h" }
    );

    res.json({ 
    token, 
    user: {
        username: user.useName, 
        role: user.useRole,
        email: user.useEmail
    } 
});
  } catch (err) {
    console.error(err);
    res.status(500).json({ error: "Login failed" });
  }
};
