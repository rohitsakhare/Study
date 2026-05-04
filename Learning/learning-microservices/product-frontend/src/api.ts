import axios from "axios";
import { Product } from "./types";

const API_BASE = `${process.env.REACT_APP_API_URL || "http://localhost:8080"}/api/products`;

export const getProducts = () => axios.get<Product[]>(API_BASE);
export const getProduct = (id: string) =>
  axios.get<Product>(`${API_BASE}/${id}`);
export const createProduct = (product: Omit<Product, "id" | "createdAt">) =>
  axios.post<Product>(API_BASE, product);
export const updateProduct = (
  id: string,
  product: Omit<Product, "id" | "createdAt">
) => axios.put(`${API_BASE}/${id}`, product);
export const deleteProduct = (id: string) =>
  axios.delete(`${API_BASE}/${id}`);
