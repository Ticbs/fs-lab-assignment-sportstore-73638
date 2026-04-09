import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import OrdersDashboard from './components/OrdersDashboard';
import OrderDetails from './components/OrderDetails';
import FailedOrders from './components/FailedOrders';
import './App.css';

function App() {
  return (
    <Router>
      <div style={{ display: 'flex', minHeight: '100vh' }}>
        <nav style={{ width: '220px', background: '#1a1a2e', color: 'white', padding: '20px' }}>
          <h2 style={{ color: '#e94560', marginBottom: '30px' }}>Admin Dashboard</h2>
          <ul style={{ listStyle: 'none', padding: 0 }}>
            <li style={{ marginBottom: '15px' }}>
              <Link to="/" style={{ color: 'white', textDecoration: 'none' }}>?? All Orders</Link>
            </li>
            <li style={{ marginBottom: '15px' }}>
              <Link to="/failed" style={{ color: 'white', textDecoration: 'none' }}>? Failed Orders</Link>
            </li>
          </ul>
        </nav>
        <main style={{ flex: 1, padding: '20px', background: '#f5f5f5' }}>
          <Routes>
            <Route path="/" element={<OrdersDashboard />} />
            <Route path="/orders/:id" element={<OrderDetails />} />
            <Route path="/failed" element={<FailedOrders />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;
