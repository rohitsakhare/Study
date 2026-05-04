import React, { useState } from "react";
import './App.css';
import { ProductList } from "./components/ProductList";
import { ProductForm } from "./components/ProductForm";

const App: React.FC = () => {
  const [refreshKey, setRefreshKey] = useState(0);

  const triggerRefresh = () => setRefreshKey(prev => prev + 1);

  return (
    <div style={{ padding: "20px" }}>
      <h1>Product CRUD</h1>
      <ProductForm onCreated={triggerRefresh} />
      <ProductList key={refreshKey} />
    </div>
  );
};

export default App;