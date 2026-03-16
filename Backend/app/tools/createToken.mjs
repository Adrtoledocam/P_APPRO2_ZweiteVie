// tools/createToken.mjs
import jwt from 'jsonwebtoken';
import dotenv from 'dotenv';

dotenv.config();

export const createToken = (payload) => {
  return jwt.sign(
    payload,
    process.env.JWT_SECRET || 'dev_secret',
    { expiresIn: '2h' }
  );
};
