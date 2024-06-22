import React from 'react';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';

// Компонент кнопки для скачивания Excel файла
const DownloadExcelButton = () => {
    // Функция для скачивания файла
    const downloadFile = () => {
        // URL файла для скачивания (в данном случае, примерный путь к файлу)
        const fileUrl = process.env.PUBLIC_URL + '\excels\data.xlsx';

        // Создание элемента <a> для скачивания файла
        const link = document.createElement('a');
        link.href = fileUrl; // Устанавливаем URL файла
        link.setAttribute('download', 'example.xlsx'); // Устанавливаем атрибут download для указания имени файла
        document.body.appendChild(link);

        link.click(); // Запускаем скачивание файла
        document.body.removeChild(link);
    };

    return (
        <Box sx={{ width: '200px' }}>
            {/* Кнопка для запуска функции скачивания файла */}
            <Button onClick={downloadFile} variant="contained" color="primary" fullWidth>
                Скачать файл
            </Button>
        </Box>
    );
};

export default DownloadExcelButton;
