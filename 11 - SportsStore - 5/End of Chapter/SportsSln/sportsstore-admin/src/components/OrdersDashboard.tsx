import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { getOrders, Order } from '../orderService';

const statusColors: Record<string, string> = {
  Completed: '#28a745',
  Failed: '#dc3545',
  Submitted: '#007bff',
  PaymentApproved: '#17a2b8',
  InventoryConfirmed: '#6f42c1',
  ShippingCreated: '#20c997',
};

const OrdersDashboard: React.FC = () => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [filter, setFilter] = useState('All');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    getOrders()
      .then(setOrders)
      .catch(() => setError('Could not load orders. Is the OrderApi running?'))
      .finally(() => setLoading(false));
  }, []);

  const statuses = ['All', 'Submitted', 'Completed', 'Failed', 'PaymentApproved'];
  const filtered = filter === 'All' ? orders : orders.filter(o => o.status === filter);

  if (loading) return <p>Loading orders...</p>;
  if (error) return <p style={{ color: 'red' }}>{error}</p>;

  return (
    <div>
      <h2>Orders Dashboard</h2>
      <div style={{ marginBottom: '15px', display: 'flex', gap: '10px' }}>
        <span>Total: {orders.length}</span>
        <span style={{ color: '#28a745' }}>Completed: {orders.filter(o => o.status === 'Completed').length}</span>
        <span style={{ color: '#dc3545' }}>Failed: {orders.filter(o => o.status === 'Failed').length}</span>
      </div>
      <div style={{ marginBottom: '15px' }}>
        {statuses.map(s => (
          <button key={s} onClick={() => setFilter(s)}
            style={{ marginRight: '8px', padding: '5px 12px',
              background: filter === s ? '#1a1a2e' : '#ddd',
              color: filter === s ? 'white' : 'black',
              border: 'none', borderRadius: '4px', cursor: 'pointer' }}>
            {s}
          </button>
        ))}
      </div>
      <table style={{ width: '100%', borderCollapse: 'collapse', background: 'white' }}>
        <thead>
          <tr style={{ background: '#1a1a2e', color: 'white' }}>
            <th style={{ padding: '10px' }}>Order ID</th>
            <th style={{ padding: '10px' }}>Customer</th>
            <th style={{ padding: '10px' }}>Total</th>
            <th style={{ padding: '10px' }}>Status</th>
            <th style={{ padding: '10px' }}>Date</th>
            <th style={{ padding: '10px' }}>Actions</th>
          </tr>
        </thead>
        <tbody>
          {filtered.map(order => (
            <tr key={order.id} style={{ borderBottom: '1px solid #ddd' }}>
              <td style={{ padding: '10px' }}>{order.id.substring(0, 8)}...</td>
              <td style={{ padding: '10px' }}>{order.customerId}</td>
              <td style={{ padding: '10px' }}>£{order.totalAmount.toFixed(2)}</td>
              <td style={{ padding: '10px' }}>
                <span style={{ padding: '3px 8px', borderRadius: '4px',
                  background: statusColors[order.status] || '#6c757d', color: 'white' }}>
                  {order.status}
                </span>
              </td>
              <td style={{ padding: '10px' }}>{new Date(order.createdAt).toLocaleDateString()}</td>
              <td style={{ padding: '10px' }}>
                <Link to={'/orders/' + order.id}>View</Link>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default OrdersDashboard;
