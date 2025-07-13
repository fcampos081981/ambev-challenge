import React from 'react';
import OrderForm from './components/OrderForm';
import './App.css';

function App() {
  return (
      <div className="App">
        <header className="App-header">
          <h1>Sistema de Gerenciamento de Pedidos</h1>
        </header>
        <main>
          <OrderForm />
        </main>
      </div>
  );
}

export default App;