import { v2 as cloudinary } from 'cloudinary';
import dotenv from 'dotenv';

dotenv.config();

cloudinary.config({
  cloud_name: process.env.CLOUDINARY_CLOUD_NAME || 'dzguf69ws',
  api_key: process.env.CLOUDINARY_API_KEY || '785519479823275',
  api_secret: process.env.CLOUDINARY_API_SECRET || 'iDEtOueXwp3uHB9zFzORCxsw'
});

export default cloudinary;