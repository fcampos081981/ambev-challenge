import React from 'react';
import OrderSearch from './components/OrderSearch';
import OrderList from './components/OrderList';

function App() {
    return (
        <div className="App">
            <OrderSearch />

            <OrderList />
        </div>
    );
}

export default App;