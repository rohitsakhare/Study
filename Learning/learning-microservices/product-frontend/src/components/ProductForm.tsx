import React, { useState } from "react";
import { createProduct } from "../api";

export const ProductForm: React.FC<{ onCreated: () => void }> = ({ onCreated }) => {
  const [name, setName] = useState("");
  const [price, setPrice] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name || !price) return alert("Name and price required");

    try {
      await createProduct({ name, price: parseFloat(price) });
      setName("");
      setPrice("");
      onCreated();
    } catch {
      alert("Failed to create product");
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input placeholder="Name" value={name} onChange={e => setName(e.target.value)} />
      <input placeholder="Price" type="number" value={price} onChange={e => setPrice(e.target.value)} />
      <button type="submit">Add Product</button>
    </form>
  );
};
