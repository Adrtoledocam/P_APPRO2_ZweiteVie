import express from "express";
import { auth } from "../middleware/auth.mjs";
import { requireRole } from "../middleware/role.mjs";
import { getAllPhotos, getPhotoById, createPhoto, updatePhoto, deletePhoto, getPopularPhotos, getPhotosByTag, getMyPhotos } from "../controllers/photoController.mjs";

const router = express.Router();

// Public
router.get("/", getAllPhotos);
router.get("/popular", getPopularPhotos);
router.get("/tag/:tagName", getPhotosByTag );
router.get("/:id", getPhotoById);

// Photographer
router.post("/", auth, requireRole("photographer"), createPhoto);
router.put("/:id", auth, requireRole("photographer"), updatePhoto);
router.get("/my-photos", auth, requireRole("photographer"), getMyPhotos);

// Admin
router.delete("/:id", auth, requireRole("admin"), deletePhoto);



export default router;
