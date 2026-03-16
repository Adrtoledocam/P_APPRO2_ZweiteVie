import express from "express";
import { auth } from "../middleware/auth.mjs";
import { requireRole } from "../middleware/role.mjs";
import {createContract, getMonthlyReport, getMyContracts} from "../controllers/contractController.mjs";

const router = express.Router();
router.post("/", auth, requireRole("client"), createContract);
router.get("/my-contracts", auth,getMyContracts);
router.get("/report/monthly", auth, requireRole("admin"), getMonthlyReport);


export default router;