import {pool} from "../config/db.mjs";
import bcrypt from "bcryptjs";
import jwt from "jsonwebtoken";

export const register = async (req, res) => {
    const {username, email, password} = req.body;

    if (!username || !password || !email)
        return res.status(400).json({error:"Des champs requis sont manquants"});
    if(username.length <3 || username.length >50)
        return res.status(400).json({error: "Le nom d'utilisateur doit contenir entre 3 et 50 caractères."})
    
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email))
        return res.status(400).json({error: "Le format de l'adresse email est invalide"})

    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[-+_!@#$%^&*.,?]).{8,}$/;    
    if (!passwordRegex.test(password)) 
        return res.status(400).json({ error: "Le mot de passe doit contenir au moins 8 caractères, une majuscule, une minuscule, un chiffre et un symbole."});
    
    try{
        
        const [existingUser] = await pool.execute('SELECT * FROM t_user WHERE useEmail = ?',[email]);
        if(existingUser.length>0){
            return res.status(400).json({message: "Cet email est déjà utilisé."})
        }
        
        const salt =  await bcrypt.genSalt(10);
        const hashed = await bcrypt.hash(password, salt);

        await pool.execute(
            'INSERT INTO t_user (useName, useEmail, usePassword) VALUES (?,?,?)',[username, email, hashed]
        );

        res.json ({message : "Utilisateur créé avec succès !"});
    } catch (err) {
        res.status(500).json({error: "Erreur serveur lors de l'inscription."});
    }
}; 

export const login = async (req, res) => {
    const {email, password} = req.body;

    try {
        const [users] = await pool.execute("SELECT * FROM t_user WHERE useEmail = ?", [email]);

        if (users.length === 0)
        return res.status(401).json({ error: "Cet utilisateur n'est pas enregistré" });

        const user = users[0];

        if (!user || !(await bcrypt.compare(password, user.usePassword))){
            return res.status(401).json({ message : "Identifiants invalides."});
        }
        
        const token = jwt.sign(
            {
                id: user.useId,
                isAdmin: user.useIsAdmin,
            },
            process.env.JWT_SECRET || "secret_key_toledoc",
            { expiresIn: "2h" }
        );

        res.json({ token, user: { id: user.useId, username: user.useName, email: user.useEmail, isAdmin : user.useIsAdmin }});
    } catch (error) {    
        res.status(500).json({ message: "Erreur serveur lors de la connexion." });
  }
};
