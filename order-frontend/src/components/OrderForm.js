import React, { useState } from 'react';
import './OrderForm.css';

const API_URL = process.env.REACT_APP_API_URL || 'http://localhost:8080/api';

const OrderForm = () => {
    const [externalOrderId, setExternalOrderId] = useState(`ORD-${Date.now()}`);
    const [items, setItems] = useState([
        { productId: '', productName: '', quantity: 1, unitPrice: 0.0 }
    ]);
    const [statusMessage, setStatusMessage] = useState('');
    const [isError, setIsError] = useState(false);

    const handleItemChange = (index, event) => {
        const values = [...items];
        values[index][event.target.name] = event.target.value;
        setItems(values);
    };

    const handleAddItem = () => {
        setItems([...items, { productId: '', productName: '', quantity: 1, unitPrice: 0.0 }]);
    };

    const handleRemoveItem = (index) => {
        const values = [...items];
        if (values.length > 1) {
            values.splice(index, 1);
            setItems(values);
        }
    };

    const handleSubmit = async (event) => {
        event.preventDefault();
        setStatusMessage('Enviando pedido...');
        setIsError(false);

        const orderData = {
            externalOrderId,
            items: items.map(item => ({
                ...item,
                quantity: parseInt(item.quantity, 10),
                unitPrice: parseFloat(item.unitPrice)
            }))
        };

        try {
            const response = await fetch(`${API_URL}/orders`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(orderData)
            });

            const responseData = await response.json();

            if (!response.ok) {
                throw new Error(responseData.message || `Erro ${response.status}`);
            }

            setStatusMessage(`Pedido ${responseData.externalOrderId} criado com sucesso!`);
            setExternalOrderId(`ORD-${Date.now()}`);
            setItems([{ productId: '', productName: '', quantity: 1, unitPrice: 0.0 }]);

        } catch (error) {
            setStatusMessage(`Falha ao enviar pedido: ${error.message}`);
            setIsError(true);
        }
    };


    return (
        <form onSubmit={handleSubmit} className="order-form">
            <div>
                <h2>Order Form</h2>
                <p>API is configured at: {API_URL ? API_URL : "Not set"}</p>
            </div>
            <h2>Criar Novo Pedido</h2>

            <div className="form-group">
                <label>ID do Pedido Externo</label>
                <input
                    type="text"
                    value={externalOrderId}
                    onChange={(e) => setExternalOrderId(e.target.value)}
                    required
                />
            </div>

            <h3>Itens do Pedido</h3>
            {items.map((item, index) => (
                <div key={index} className="item-row">
                    <input type="text" name="productId" placeholder="ID do Produto" value={item.productId} onChange={e => handleItemChange(index, e)} required />
                    <input type="text" name="productName" placeholder="Nome do Produto" value={item.productName} onChange={e => handleItemChange(index, e)} />
                    <input type="number" name="quantity" placeholder="Qtd." value={item.quantity} min="1" onChange={e => handleItemChange(index, e)} required />
                    <input type="number" name="unitPrice" placeholder="Preço Unit." value={item.unitPrice} step="0.01" min="0.01" onChange={e => handleItemChange(index, e)} required />
                    <button type="button" className="remove-btn" onClick={() => handleRemoveItem(index)}>–</button>
                </div>
            ))}

            <div className="form-actions">
                <button type="button" onClick={handleAddItem}>Adicionar Item</button>
                <button type="submit">Enviar Pedido</button>
            </div>

            {statusMessage && (
                <div className={`status-message ${isError ? 'error' : 'success'}`}>
                    {statusMessage}
                </div>
            )}
        </form>
    );
};

export default OrderForm;