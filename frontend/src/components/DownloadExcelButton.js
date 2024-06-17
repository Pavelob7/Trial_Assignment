import React from 'react';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';

const DownloadExcelButton = () => {
    const downloadFile = () => {
        const fileUrl = process.env.PUBLIC_URL + '/excels/example.xlsx';
        const link = document.createElement('a');
        link.href = fileUrl;
        link.setAttribute('download', 'example.xlsx');
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    };

    return (
        <Box sx={{ width: '200px' }}>
            <Button onClick={downloadFile} variant="contained" color="primary" fullWidth>
                Скачать файл
            </Button>
        </Box>
    );
};

export default DownloadExcelButton;
