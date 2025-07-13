import React, { useState } from 'react';


const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:8080/api';

const OrderSearch = () => {
    const [externalOrderId, setExternalOrderId] = useState('');
    const [orderData, setOrderData] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleSearch = async () => {
        if (!externalOrderId.trim()) {
            setError('Por favor, insira um ID de pedido externo.');
            return;
        }

        setLoading(true);
        setError('');
        setOrderData(null);

        try {

            const response = await fetch(`${API_URL}/orders/external/${externalOrderId}`);


            if (!response.ok) {
                throw new Error('Pedido não encontrado ou erro na rede.');
            }

            const data = await response.json();
            setOrderData(data);
        } catch (err) {
                       setError(err.message);
        } finally {

            setLoading(false);
        }
    };

    return (
        <div style={{ padding: '20px', fontFamily: 'Arial, sans-serif' }}>
            <h1>Consultar Pedido</h1>
            <div style={{ marginBottom: '20px' }}>
                <input
                    type="text"
                    value={externalOrderId}
                    onChange={(e) => setExternalOrderId(e.target.value)}
                    placeholder="Digite o externalOrderId"
                    style={{ padding: '8px', marginRight: '10px' }}
                />
                <button onClick={handleSearch} disabled={loading} style={{ padding: '8px 12px' }}>
                    {loading ? 'Buscando...' : 'Buscar'}
                </button>
            </div>

            {/* Exibir mensagem de erro, se houver */}
            {error && <p style={{ color: 'red' }}>{error}</p>}

            {/* Exibir os dados do pedido, se encontrados */}
            {orderData && (
                <div>
                    <h2>Detalhes do Pedido</h2>
                    <pre style={{ background: '#f4f4f4', padding: '15px', borderRadius: '5px' }}>
            {JSON.stringify(orderData, null, 2)}
          </pre>
                    <hr />
                    <h3>Informações Formatadas:</h3>
                    <p><strong>ID:</strong> {orderData.id}</p>
                    <p><strong>ID Externo do Pedido:</strong> {orderData.externalOrderId}</p>
                    <p><strong>Valor Total:</strong> R$ {orderData.totalAmount.toFixed(2)}</p>
                    <p><strong>Status:</strong> {orderData.status}</p>
                    <p><strong>Criado em:</strong> {new Date(orderData.createdAt).toLocaleString()}</p>
                    <h4>Itens:</h4>
                    <ul>
                        {orderData.items.$values.map(item => (
                            <li key={item.id}>
                                <strong>Produto ID:</strong> {item.productId} | <strong>Quantidade:</strong> {item.quantity} | <strong>Preço Unitário:</strong> R$ {item.unitPrice.toFixed(2)}
                            </li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
};

export default OrderSearch;