import React from 'react';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';

/**
 * Компонент кнопки загрузки файла.
 * 
 * @param {Object} props - Свойства компонента.
 * @param {Function} props.onFileUpload - Обработчик события загрузки файла.
 */
const UploadFileButton = ({ onFileUpload }) => {
    /**
     * Обработчик изменения выбранного файла.
     * 
     * @param {Object} event - Событие изменения ввода файла.
     */
    const handleFileChange = (event) => {
        const file = event.target.files[0];
        onFileUpload(file); // Вызываем функцию обратного вызова для передачи файла наверх
    };

    return (
        <Box sx={{ width: '200px' }}>
            {/* Невидимый input для загрузки файла, активируемый нажатием на кнопку */}
            <input
                type="file"
                onChange={handleFileChange}
                style={{ display: 'none' }}
                id="file-upload"
            />
            {/* Метка для кнопки, связанная с input для загрузки файла */}
            <label htmlFor="file-upload">
                {/* Кнопка "Загрузить файл" с использованием Material-UI */}
                <Button variant="contained" component="span" fullWidth>
                    Загрузить файл
                </Button>
            </label>
        </Box>
    );
};

export default UploadFileButton;
