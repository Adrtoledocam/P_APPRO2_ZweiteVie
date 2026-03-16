import CryptoJS from 'crypto-js';

export const hashPassword = (password) => {
  return CryptoJS.SHA256(password).toString();
};

export const comparePassword = (password, hashed) => {
  return CryptoJS.SHA256(password).toString() === hashed;
};
