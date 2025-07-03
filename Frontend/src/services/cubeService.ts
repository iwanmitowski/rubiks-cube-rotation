import axios from 'axios';
import type { ExplodedView, MoveRequest } from '../types/api';

const API_BASE = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api';

export const getExplodedView = async (): Promise<ExplodedView> => {
  const res = await axios.get<ExplodedView>(`${API_BASE}/RubiksCube/exploded`);
  return res.data;
};

export const resetCube = async (): Promise<void> => {
  await axios.post(`${API_BASE}/RubiksCube/reset`);
};

export const moveCube = async (req: MoveRequest): Promise<void> => {
  await axios.post(`${API_BASE}/RubiksCube/move`, req);
};