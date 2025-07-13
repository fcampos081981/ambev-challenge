import React, { useState } from 'react';

const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:8080/api';
const OrderList = () => {
    const [orders, setOrders] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const fetchOrders = async () => {
        setLoading(true);
        setError('');
        setOrders(null);

        try {

            const response = await fetch(`${API_URL}/orders`);

            if (!response.ok) {
                throw new Error('Falha ao buscar os pedidos. Verifique a rede ou a API.');
            }

            const data = await response.json();
            setOrders(data.$values || []);
        } catch (err) {
            setError(err.message);
            setOrders([]);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ padding: '20px', fontFamily: 'Arial, sans-serif' }}>
            <h1>Lista de Pedidos</h1>
            <button onClick={fetchOrders} disabled={loading} style={{ padding: '10px 15px', fontSize: '16px', marginBottom: '20px' }}>
                {loading ? 'Buscando...' : 'Consultar Pedidos'}
            </button>

            {error && <p style={{ color: 'red' }}><strong>Erro:</strong> {error}</p>}

            {orders && orders.map(order => (
                <div key={order.id} style={{ border: '1px solid #ccc', borderRadius: '8px', padding: '15px', marginBottom: '15px' }}>
                    <h3>Pedido: {order.externalOrderId}</h3>
                    <p><strong>Status:</strong> <span style={{ color: '#007bff' }}>{order.status}</span></p>
                    <p><strong>Valor Total:</strong> R$ {order.totalPrice.toFixed(2)}</p>
                    <p><strong>Data de Criação:</strong> {new Date(order.createdAt).toLocaleString()}</p>

                    <h4>Itens do Pedido:</h4>
                    <ul style={{ listStyle: 'none', paddingLeft: '0' }}>
                        {order.items.$values.map(item => (
                            <li key={item.id} style={{ background: '#f8f9fa', padding: '10px', borderRadius: '4px', marginBottom: '5px' }}>
                                <strong>Produto ID:</strong> {item.productId} <br />
                                <strong>Quantidade:</strong> {item.quantity} <br />
                                <strong>Preço Unitário:</strong> R$ {item.unitPrice.toFixed(2)}
                            </li>
                        ))}
                    </ul>
                </div>
            ))}
        </div>
    );
};

export default OrderList;