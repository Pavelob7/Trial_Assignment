// RouteGraph.styles.js

import { styled } from '@mui/system';
import { Box, Button, ButtonGroup } from '@mui/material';

// Стилизованный контейнер для кнопок
export const ButtonContainer = styled(Box)(({ theme }) => ({
    display: 'flex',
    justifyContent: 'center',
    gap: theme.spacing(5), // Отступы между кнопками 5 
    marginBottom: theme.spacing(2), // Отступ снизу 2 
}));

// Группа кнопок даты
export const DateButtonGroup = styled(ButtonGroup)(({ theme }) => ({
    display: 'flex',
    justifyContent: 'center',
    gap: theme.spacing(2), // Отступы между кнопками 2
    marginBottom: theme.spacing(2), // Отступ снизу 2
}));

// Стилизованная кнопка
export const StyledButton = styled(Button)(({ theme }) => ({
    flex: 1, // Растягиваем кнопку на всю доступную ширину внутри родительского контейнера
}));
