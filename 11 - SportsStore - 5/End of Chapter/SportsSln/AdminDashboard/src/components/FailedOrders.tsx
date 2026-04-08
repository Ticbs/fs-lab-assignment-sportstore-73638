import React, { useEffect, useState } from 'react';
import { getOrders, Order } from '../orderService';

const FailedOrders: React.FC = () => {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    getOrders()
      .then(data => setOrders(data.filter(o => o.status === 'Failed' || o.status === 'InventoryFailed' || o.status === 'PaymentFailed')))
      .catch(() => setError('Could not load orders.'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p>Loading...</p>;
  if (error) return <p style={{ color: 'red' }}>{error}</p>;

  return (
    <div>
      <h2>Failed Orders</h2>
      {orders.length === 0 ? (
        <p style={{ color: 'green' }}>No failed orders!</p>
      ) : (
        <table style={{ width: '100%', borderCollapse: 'collapse', background: 'white' }}>
          <thead>
            <tr style={{ background: '#dc3545', color: 'white' }}>
              <th style={{ padding: '10px' }}>Order ID</th>
              <th style={{ padding: '10px' }}>Customer</th>
              <th style={{ padding: '10px' }}>Total</th>
              <th style={{ padding: '10px' }}>Status</th>
              <th style={{ padding: '10px' }}>Date</th>
            </tr>
          </thead>
          <tbody>
            {orders.map(order => (
              <tr key={order.id} style={{ borderBottom: '1px solid #ddd' }}>
                <td style={{ padding: '10px' }}>{order.id.substring(0, 8)}...</td>
                <td style={{ padding: '10px' }}>{order.customerId}</td>
                <td style={{ padding: '10px' }}>£{order.totalAmount.toFixed(2)}</td>
                <td style={{ padding: '10px', color: '#dc3545' }}>{order.status}</td>
                <td style={{ padding: '10px' }}>{new Date(order.createdAt).toLocaleDateString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default FailedOrders;
