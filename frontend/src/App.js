// App.js
import React from 'react';
import RouteGraph from './components/RouteGraph/RouteGraph';
import { Typography } from '@mui/material';

const App = () => {
    return (
        <div>
            <Typography variant="h4" color="primary" align="center" gutterBottom>
                Тестовое задание в АО «НИИАС»
            </Typography>
            <RouteGraph />
        </div>
    );
};

export default App;
