import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { getOrderById, Order } from '../orderService';

const OrderDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [order, setOrder] = useState<Order | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (id) {
      getOrderById(id)
        .then(setOrder)
        .finally(() => setLoading(false));
    }
  }, [id]);

  if (loading) return <p>Loading...</p>;
  if (!order) return <p>Order not found.</p>;

  return (
    <div>
      <Link to="/">Back to Orders</Link>
      <h2>Order Details</h2>
      <p><strong>ID:</strong> {order.id}</p>
      <p><strong>Customer:</strong> {order.customerId}</p>
      <p><strong>Total:</strong> £{order.totalAmount.toFixed(2)}</p>
      <p><strong>Status:</strong> {order.status}</p>
      <p><strong>Created:</strong> {new Date(order.createdAt).toLocaleString()}</p>
    </div>
  );
};

export default OrderDetails;
