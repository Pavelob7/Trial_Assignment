import React from 'react';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';

const UploadFileButton = ({ onFileUpload }) => {
    const handleFileChange = (event) => {
        const file = event.target.files[0];
        onFileUpload(file);
    };

    return (
        <Box sx={{ width: '200px' }}>
            <input
                type="file"
                onChange={handleFileChange}
                style={{ display: 'none' }}
                id="file-upload"
            />
            <label htmlFor="file-upload">
                <Button variant="contained" component="span" fullWidth>
                    Загрузить файл
                </Button>
            </label>
        </Box>
    );
};

export default UploadFileButton;
